using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows;

namespace AktualizatorTEZD
{
  public static class AsyncFilesTasks
  {
    public static async Task CopyFiles(string sourcePath, string destinationPath)
    {
      await Task.Run(() =>
      {
        try
        {
          foreach (string dirPath in Directory.GetDirectories(sourcePath, "*",
            SearchOption.AllDirectories))
            Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));

          foreach (string newPath in Directory.GetFiles(sourcePath, "*.*",
            SearchOption.AllDirectories))
            File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
        }
        catch (Exception e)
        {
          MessageBox.Show("Copy Failed: " + e.Message, "Failed!", MessageBoxButton.OK);
        }
      });
    }

    public static async Task ExtractFiles(string lastUpdateFileNameWithPath, string appPath)
    {
      await Task.Run(() =>
      {
        try
        {
          ZipFile.ExtractToDirectory(lastUpdateFileNameWithPath, appPath);
        }
        catch (Exception e)
        {
          MessageBox.Show("Extract failed: " + e.Message, "Failed!", MessageBoxButton.OK);
        }
      });
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
    }
  }
}
