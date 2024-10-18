using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace App_Store.Models
{
    class ImageMoveLocalFolder
    {
        public static async Task<string> ImageMoveLocalFolderAsync(string productName)
        {
            // Получаем папку приложения
            StorageFolder folder = ApplicationData.Current.LocalFolder;

            string projectImagesPath = "Images";

            var sourceFilePath = Path.Combine(projectImagesPath, productName);
            var destinationFilePath = Path.Combine(folder.Path, productName);
            

            // Проверяем, существует ли файл уже в локальной папке
            if (!File.Exists(destinationFilePath))
            {
                var sourceFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///{sourceFilePath}"));
                await sourceFile.CopyAsync(folder);
            }
            //Возращаем путь до файла в ApplicationData.Current.LocalFolder
            return destinationFilePath;
        }
    }
}
