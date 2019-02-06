using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows;

namespace AktualizatorTEZD
{
  public static class AsyncFilesTasks
  {
    public static async Task CopyFiles(string[] files, string appPath, string directory)
    {
      await Task.Run(() =>
      {
        try
        {
          foreach (var srcPath in Directory.GetFiles(appPath + directory))
          {
            File.Copy(srcPath, srcPath.Replace(appPath + directory, appPath), true);
          }
        }
        catch (Exception e)
        {
          MessageBox.Show("Copy Failed: " + e.Message, "Failed!", MessageBoxButton.OK);
        }
      });
      //MessageBox.Show("Skopiowano pliki", "Sukces!", MessageBoxButton.OK);
    }

    public static async Task ExtractFiles(string lastUpdateFileNameWithPath, string appPath)
    {
      await Task.Run(() =>
      {
        try
        {
          ZipFile.ExtractToDirectory(lastUpdateFileNameWithPath, appPath + "Temp");
        }
        catch (Exception e)
        {
          MessageBox.Show("Extract failed: " + e.Message, "Failed!", MessageBoxButton.OK);
        }
      });
      //MessageBox.Show("Wypakowano pliki", "Sukces!", MessageBoxButton.OK);
    }

    public static async Task DeleteFiles(string appPath, string directory)
    {
      await Task.Run(() =>
      {
        try
        {
          if (Directory.Exists(appPath + directory))
          {
            Directory.Delete(appPath + directory, true);
          }
        }
        catch (Exception e)
        {
          MessageBox.Show("Delete failed: " + e.Message, "Failed!", MessageBoxButton.OK);
        }
      });
      //MessageBox.Show("Usunięto starą paczkę", "Sukces!", MessageBoxButton.OK);
    }
  }
}
