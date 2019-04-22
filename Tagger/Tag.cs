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
        public string tag;
        public string parent = "";
        public string[] children;

        public Tag(string tag, string[] children, string parent)
        {
            this.tag = tag;
            this.children = children;
            this.parent = parent;
        }
    }

    class Tags
    {
        public static int sizeOfTags = 0;
        //public static Tags mainTags;
        Tag[] listOfTags;

        public Tags(int size)
        {
            listOfTags = new Tag[size];
        }

        public Tag this[int index]
        {
            set
            {
                listOfTags[index] = value;
            }
            get
            {
                return listOfTags[index];
            }
        }

        public static Tags ReadFromFile(string path, int addPlace = 0)
        {
            string[] textBufer = File.ReadAllLines(path);
            sizeOfTags = textBufer.Length + addPlace;
            Tags tags = new Tags(sizeOfTags);
            if (textBufer != null)
            {
                int numOfTag = 0;
                foreach (var pos in textBufer)
                {
                    var eachLine = pos.Split(';');
                    tags[numOfTag++] = new Tag(eachLine[0], eachLine[1].Split(','), eachLine[2]);
                }
            }
            return tags;
        }

        public static void WriteIntoFile(string path, Tags tags)
        {
            string[] textBufer = new string[sizeOfTags];
            string childrenBufer = null;
            for (int pos = 0; pos < sizeOfTags; pos++)
            {
                int i = 0;
                foreach (var child in tags[pos].children)
                {
                    childrenBufer += child;
                    i++;
                    if (i != tags[pos].children.Length)
                    {
                        childrenBufer += ',';
                    }
                }
                textBufer[pos] = tags[pos].tag + ';' + childrenBufer + ';' + tags[pos].parent;
                childrenBufer = null;
            }
            File.WriteAllLines(path, textBufer);
        }

        public static void AddTag(string path, string tag, string[] children = null, string parent = null)
        {
            Tags tags = ReadFromFile(path, 1);
            tags[sizeOfTags - 1] = new Tag(tag, children, parent);
            WriteIntoFile(path, tags);
        }

        public static void DeleteTag(string path, string tag)
        {
            Tags tags = ReadFromFile(path, 0);
            Tag tagD = FindTag(tags, tag);
            tagD = null;
            sizeOfTags--;
            WriteIntoFile(path, tags);
        }

        public static Tag FindTag(Tags tags, string tag)
        {
            for (int pos = 0; pos < sizeOfTags; pos++)
            {
                if (tags[pos].tag == tag)
                {
                    return tags[pos];
                }
            }
            Tag tagNull = new Tag(null, null, null);
            return tagNull;    // костыли
        }
    }
}
