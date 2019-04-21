using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Tagger
{
    class Tag
    {
        protected string tag;
        protected string parent;
        protected string[] childrens;
        protected Tag(string tag, string parent, string[] childrens)
        {
            this.tag = tag;
            this.parent = parent;
            this.childrens = childrens;
        }
    }                                                               // можно не трогать, сами тэги

    class Tags : Tag
    {
        public static string[] dataFromFile; 
        public static bool flagOfReading = false;
        public static int fileSize = 0;
        Tags[] listOfTags;
        private Tags(int size)
        {
            listOfTags = new Tags[size];
        }
        public tags this[string index]                  // индексатор, где для удобства нахождения, индекс соответствует названию самого тега
        {
            get
            {
                return listOfTags[index];
            }
            set
            {
                listOfTags[index] = value;
            }
        }

        public static void addTag(string tag, string parent, string[] childrens)
        {
            File.AppendText("AllTags.txt") = tag + ';' + parent + ';' + writeChildrens(childrens);
            flagOfReading = false;
        }

        private static void readFromFile()
        {
            dataFromFile = File.ReadAllLines("All_Tags.txt");
            fileSize = dataFromFile.Length;
        }

        private static string writeChildrens(string[] childrens)
        {
            string allChilds;
            foreach (string child in childrens)
            {
                allChilds += child + ',';
            }
            return allChilds;
        }
        public static Tag takeTag(string nameOfTag)
        {
            readFromFile();
            if (flagOfReading = false)
            {
                Tags tags = new Tags();
                foreach (string line in dataFromFile)
                {
                    var elementsOfLine = line.Split(';');
                    var elemenstsOfChildrens = elementsOfLine[3].Split(',');
                    tags[elementsOfLine[0]] = new Tag { elementsOfLine[0], elementsOfLine[1], elemenstsOfChildrens };
                }
                flagOfReading = true;
            }
        }
    }
}
