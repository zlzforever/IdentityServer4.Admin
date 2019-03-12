using System;
using System.Collections.Generic;
using System.IO;

namespace IdentityServer4.Admin.Infrastructure
{
    public class DirectoryHelper
    {
        public static void PrepareFromFilePath(string file)
        {
            var folder = Path.GetDirectoryName(file);
            if (string.IsNullOrWhiteSpace(folder))
            {
                throw new IdentityServer4AdminException("文件路径不合法");
            }

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }
        public static void Move(string sourcePath, string destPath)
        {
            if (Directory.Exists(sourcePath))
            {
                if (!Directory.Exists(destPath))
                {
                    //目标目录不存在则创建
                    try
                    {
                        Directory.CreateDirectory(destPath);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("创建目标目录失败：" + ex.Message);
                    }
                }

                //获得源文件下所有文件
                List<string> files = new List<string>(Directory.GetFiles(sourcePath));
                files.ForEach(c =>
                {
                    string destFile = Path.Combine(new[] {destPath, Path.GetFileName(c)});
                    //覆盖模式
                    if (File.Exists(destFile))
                    {
                        File.Delete(destFile);
                    }

                    File.Move(c, destFile);
                });
                //获得源文件下所有目录文件
                List<string> folders = new List<string>(Directory.GetDirectories(sourcePath));

                folders.ForEach(c =>
                {
                    string destDir = Path.Combine(new[] {destPath, Path.GetFileName(c)});
                    //采用递归的方法实现
                    Move(c, destDir);
                });
            }
            else
            {
                throw new DirectoryNotFoundException("源目录不存在！");
            }
        }
    }
}