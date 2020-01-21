using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;


namespace Tagger
{
    static class FileProcessor
    {
        public static List<FileInfo> ScanDirectories(string path, bool isSubDirectories)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            List<FileInfo> files = new List<FileInfo>();
            AddFilesToList(directory.GetFiles(), files);
            if (isSubDirectories)
                foreach (var current in directory.GetDirectories("*.*", SearchOption.AllDirectories))
                {
                    AddFilesToList(current.GetFiles(), files);
                }
            return files;
        }

        public static List<string> GetTagsFromDirectory(List<FileInfo> files)
        {
            List<string> tags = new List<string>();
            foreach(var file in files)
            {
                var currentTags = GetTagsFromFile(file);
                foreach(var current in currentTags)
                {
                    if (!tags.Contains(current))
                        tags.Add(current);
                }
            }
            return tags;
        }

        public static List<string> GetTagsFromFile(FileInfo file)
        {
            if (file != null)
            {
                var nameAndType = file.Name.Split('.');
                var tags = nameAndType[0].Split('%').ToList();
                tags.RemoveAt(0);
                var distinct = tags.Distinct().ToList();
                return distinct;
            }
            else
            {
                MessageBox.Show("ERROR!");
                throw new Exception("FileIsNull");
            }
        }

        public static void AddTag(List<FileInfo> files, string tag)
        {
            foreach(var file in files)
            {
                var tags = GetTagsFromFile(file);
                if (!tags.Contains(tag))
                {
                    tags.Add(tag);
                    var name = file.FullName.Split('.')[0].Split('%')[0];
                    var res = file.FullName.Split('.')[1];
                    StringBuilder sb = new StringBuilder();
                    tags.ForEach(x => sb.Append('%' + x));
                    var newName = string.Format(name + sb + '.' + res);
                    file.CopyTo(newName);
                    file.Delete();
                }
            }
        }

        public static void RemoveTag(List<FileInfo> files, string tag)
        {
            foreach (var file in files)
            {
                var tags = GetTagsFromFile(file);
                if (tags.Contains(tag))
                {
                    tags.Remove(tag);
                    var name = file.FullName.Split('%')[0];
                    var res = file.Name.Split('.')[1];
                    StringBuilder sb = new StringBuilder();
                    tags.ForEach(x => sb.Append('%' + x));
                    var newName = string.Format(name + sb + '.' + res);
                    file.CopyTo(newName);
                    file.Delete();
                }
            }
        }

        private static void AddFilesToList(FileInfo[] files, List<FileInfo> list)
        {
            foreach(var file in files)
            {
                list.Add(file);
            }
        }

        //public static bool isCopyNames(List<FileInfo> files)
        //{
        //    var checkedFiles = files;
        //    foreach(var file in checkedFiles)
        //    {
        //        //bool isCopy = false;
        //        var tags = GetTagsFromFile(file);
        //        foreach(var tag in tags)
        //        {
        //            if (tag.Contains("(%)"))
        //            {
        //                //isCopy = true;
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        //public static void ReplaceCopyNames(List<FileInfo> files)
        //{
        //    foreach (var file in files)
        //    {
        //        var tags = GetTagsFromFile(file);
        //        var corruptedTag = tags.FindIndex(x => x.Contains("("));
        //        tags[corruptedTag] = tags[corruptedTag].Split(' ')[0];
        //        var name = file.FullName.Split('%')[0];
        //        var res = file.Name.Split('.')[1];
        //        StringBuilder sb = new StringBuilder();
        //        tags.ForEach(x => sb.Append('%' + x));
        //        var newName = string.Format(name + sb + '.' + res);
        //        file.CopyTo(newName);
        //        file.Delete();
        //    }
        //}
    }
}
