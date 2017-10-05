using System;
using System.IO;
using System.Security.Cryptography;

namespace Jbta.SearchEngine.Utils
{
    internal static class FileHashing
    {
        public static string Md5(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    return Convert.ToBase64String(md5.ComputeHash(fs));
                }
            }
        }
    }
}