using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Tagger
{
    public static class Transfer
    {
        public static List<FileInfo> searchedFiles = new List<FileInfo>();
        public static List<FileInfo> GetSearchedFiles()
        {
            return new List<FileInfo>(searchedFiles);
        }
        public static void PutSearchedFiles(List<FileInfo> files)
        {
            searchedFiles = new List<FileInfo>(files);
        }
        public static void PutSearchedFiles(List<string> files)
        {
            
            searchedFiles.Clear();
            foreach (var file in files)
                searchedFiles.Add(new FileInfo(file));
        }

        public static void Clear()
        {
            searchedFiles.Clear();
        }
    }
}
