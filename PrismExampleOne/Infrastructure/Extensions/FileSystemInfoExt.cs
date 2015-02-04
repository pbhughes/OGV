using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OGV.Infrastructure.Extensions
{
    public static class FileSystemInfoExt
    {
        public static string FileNameNoExt(this FileInfo fo)
        {
            string nameOnly = string.Empty;
            if (!string.IsNullOrEmpty(fo.Extension))
                return fo.Name.Replace(fo.Extension, "");

            return fo.Name;
        }
    }
}
