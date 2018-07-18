using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Tagger
{
    class FileProcessor
    {
        public static List<FileInfo> scanDirectories(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            List<FileInfo> files = new List<FileInfo>();
            addArrayToList(directory.GetFiles(), files);
            foreach(var current in directory.GetDirectories("*.*", SearchOption.AllDirectories))
            {
                addArrayToList(current.GetFiles(), files);
            }
            return files;
        }

        public static void giveNumbers(List<FileInfo> files)
        {
            string extention;
            int number;
            foreach(var file in files)
            {
                extention = file.Name.Split('.')[1];
                try
                {
                    number = Convert.ToInt32(file.Name.Split('.')[0].Split('%')[0]);
                }
                catch
                {

                }
            }
        }

        private static void addArrayToList(FileInfo[] array, List<FileInfo> list)
        {
            foreach(var file in array)
            {
                list.Add(file);
            }
        }
    }
}
