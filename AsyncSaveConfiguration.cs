using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows;

namespace AktualizatorTEZD
{
  public static class AsyncSaveConfiguration
  {
    public static async Task SaveConfiguration(string appPath, string updatePacksPath, string appName)
    {
      await Task.Run(() =>
      {
        try
        {
          Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
          if (config.AppSettings.Settings["appPath"].Value != appPath
          && config.AppSettings.Settings["updatePath"].Value != updatePacksPath
          && config.AppSettings.Settings["appName"].Value != appName)
          {
            config.AppSettings.Settings["appPath"].Value = appPath;
            config.AppSettings.Settings["updatePath"].Value = updatePacksPath;
            config.AppSettings.Settings["appName"].Value = appName;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
          }
        }
        catch (Exception e)
        {
          MessageBox.Show("SaveConfig: " + e.Message, "Failed!", MessageBoxButton.OK);
        }
      });
    }
  }
}
