using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using EnsureThat;
using OpenQA.Selenium.Remote;

namespace AktualizatorTEZD
{
  public partial class MainWindow : Window
  {
    private readonly string _appPath;
    private readonly string _updatePath;
    private readonly string _currentAppVersion;
    private readonly string _tempUpdateDirectory;
    private readonly FileInfo _latestUpdateFile;
    private readonly string _appName;
    private readonly string _configFilesDirectory;
    public MainWindow()
    {
      this.Title = "Aktualizator tEZD";
      InitializeComponent();
      try
      {
        _appPath = ConfigurationManager.AppSettings["appPath"]; //  App to update path
        _updatePath = ConfigurationManager.AppSettings["updatePath"]; // TeamCity virtual compiled app versions - get the newest by datetime
        _currentAppVersion = FileVersionInfo.GetVersionInfo(ConfigurationManager.AppSettings["appPath"] + "tEZD.exe").ProductVersion;
        _tempUpdateDirectory = @"Temp\";
        _latestUpdateFile = new DirectoryInfo(_updatePath).GetFiles().OrderByDescending(x => x.LastWriteTime).First();
        _appName = ConfigurationManager.AppSettings["appName"];
        _configFilesDirectory = "Config";
        appPath.Text = _appPath;
        updatePacksPath.Text = _updatePath;
        appName.Text = _appName;
      }
      catch (Exception e)
      {
        MessageBox.Show("Wczytanie danych nie powiodło się! Sprawdź konfigurację!", "Wczytanie danych", MessageBoxButton.OK);
        throw;
      }

      Ensure.That(_appPath, nameof(_appPath)).IsNotNull();
      Ensure.That(_updatePath, nameof(_updatePath)).IsNotNull();
      Ensure.That(_currentAppVersion, nameof(_currentAppVersion)).IsNotNull();
      Ensure.That(_tempUpdateDirectory, nameof(_tempUpdateDirectory)).IsNotNull();
      Ensure.That(_latestUpdateFile, nameof(_latestUpdateFile)).IsNotNull();
      Ensure.That(_appName, nameof(_appName)).IsNotNull();
      Ensure.That(_configFilesDirectory, nameof(_configFilesDirectory)).IsNotNull();
    }
    private async void Button_ClickAsync(object sender, RoutedEventArgs e)
    {
      await AsyncSaveConfiguration.SaveConfiguration(appPath.Text, updatePacksPath.Text, appName.Text);

      string lastUpdateFileNameWithPath = _updatePath + _latestUpdateFile;
      string newAppVersion = CombineNewVersionName(_latestUpdateFile.Name);
      await AsyncFilesTasks.DeleteFiles(_appPath, _tempUpdateDirectory);

      if (newAppVersion == _currentAppVersion)
      {
        MessageBox.Show("Posiadasz aktualną wersję aplikacji" + Environment.NewLine + "Wersja aplikacji: " + _currentAppVersion, "Aktualna!", MessageBoxButton.OK);
        Process.Start(_appPath + _appName + ".exe");
        Application.Current.Shutdown();
      }
      else
      {
        MessageBox.Show("Wersja aplikacji to : " + _currentAppVersion + Environment.NewLine + "Dostępna jest nowa wersja: " + newAppVersion + " , czy chcesz zaktualizować? ",
          "Aktualizuj", MessageBoxButton.OK);

        await AsyncFilesTasks.ExtractFiles(lastUpdateFileNameWithPath, _appPath + _tempUpdateDirectory);
        await AsyncFilesTasks.CopyFiles(_appPath + _tempUpdateDirectory, _appPath);
        await AsyncFilesTasks.CopyFiles(_appPath + "Config", _appPath);

        MessageBox.Show("Gotowe!", "Aktualizacja", MessageBoxButton.OK);
        DesiredCapabilities appCapabilities = new DesiredCapabilities(); appCapabilities.SetCapability("app", _appPath + _appName + ".exe");

        Application.Current.Shutdown();
      }
    }

    static string CombineNewVersionName(string fullNameOfLatestVersion)
    {
      int startVersionString = fullNameOfLatestVersion.IndexOf("_") + 2;
      int closedBracket = fullNameOfLatestVersion.IndexOf(')');

      return fullNameOfLatestVersion.Substring(startVersionString, (closedBracket - startVersionString)).Replace("_(build_", ".");
    }
  }
}
