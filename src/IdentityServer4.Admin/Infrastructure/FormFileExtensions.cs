using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IdentityServer4.Admin.Infrastructure
{
    public static class FormFileExtensions
    {
        public static async Task<string> SaveRandomNameFileAsync(this IFormFile formFile, string storageRoot,
            string interval)
        {
            var fileExtension = Path.GetExtension(formFile.FileName);
            var fileName = $"{Guid.NewGuid():N}{fileExtension}";
            var filePath = $"{storageRoot}{interval}{fileName}";

            DirectoryHelper.PrepareFromFilePath(filePath);

            using (var stream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                await formFile.CopyToAsync(stream);
            }

            return $"{interval}{fileName}";
        }

        public static async Task SaveFileAsync(this string content, string filePath)
        {
            DirectoryHelper.PrepareFromFilePath(filePath);

            await File.WriteAllTextAsync(filePath, content);
        }


        public static async Task SaveFileAsync(this IFormFile formFile, string filePath)
        {
            DirectoryHelper.PrepareFromFilePath(filePath);

            using (var stream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                await formFile.CopyToAsync(stream);
            }
        }
    }
}