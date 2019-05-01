using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Tagger
{
    class TagLib
    {
        public static void WriteToFile(List<string[]> data, string path)
        {
            string[] editData = new string[data.Count];
            for (int i = 0; i < data.Count; i++)
            {
                editData[i] = data[i][0] + " " + data[i][1];
            }
            File.WriteAllLines(path, editData);
        }

        public static List<string[]> ReadFromFile(string path)
        {
            string[] rawData;
            try
            {
                rawData = File.ReadAllLines(path);
            }
            catch
            {
                WriteToFile(new List<string[]> { }, path);
                rawData = File.ReadAllLines(path);
                System.Windows.MessageBox.Show("Файл БД не найден и был создан");
            }
            List<string[]> data = new List<string[]>();
            for (int i = 0; i < rawData.Length; i++)
            {
                var couples = rawData[i].Split(' ');
                data.Add(new string[] { couples[0], couples[1] });
            }
            return data;
        }

        public static string[] TagsToWrite(List<string[]> data, string Tag)
        {
            List<string> ToUse = new List<string>();
            ToUse.Add(Tag);
            bool isFind = true;
            string lastTag = Tag;
            while(isFind)
            {
                isFind = false;
                var found = data.Find(x => x[1].Equals(lastTag));
                if (found != null)
                {
                    ToUse.Add(found[0]);
                    lastTag = found[0];
                    isFind = true;
                }
            }
            return ToUse.ToArray();
        }

        public static void DelFromTags(List<string[]> data, string Tag, string path)
        {
            var parent = data.Find(x => x[1].Equals(Tag))[0];
            var rawChilds = data.FindAll(x => x[0].Equals(Tag));
            List<string> childs = new List<string>();
            foreach (var cur in rawChilds)
                childs.Add(cur[1]);
            data.RemoveAll(x => x[0].Equals(Tag));
            data.RemoveAll(x => x[1].Equals(Tag));
            foreach (var child in childs)
                data.Add(new string[] { parent, child });
            WriteToFile(data, path);
        }

        public static string[] GetAllTags(List<string[]> data)
        {
            List<string> Tags = new List<string>();
            foreach(var cur in data)
            {
                if (!Tags.Contains(cur[0]))
                    Tags.Add(cur[0]);
                if (!Tags.Contains(cur[1]))
                    Tags.Add(cur[1]);
            }
            return Tags.ToArray();
        }
    }
}
