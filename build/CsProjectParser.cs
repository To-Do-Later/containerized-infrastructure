using Build.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Build
{
    public static class CsProjectParser
    {
        public static NetCoreProject Parse(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be null or whitespace.", nameof(filePath));
            }

            return new NetCoreProject()
            {
                FullPath = filePath,
                Name = Path.GetFileNameWithoutExtension(filePath)
            };

        }
    }
}
