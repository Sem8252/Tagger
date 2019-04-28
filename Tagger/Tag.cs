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

        public static void WriteIntoFile(string path, Tags tags, int del = 0)
        {
            string[] textBufer = new string[sizeOfTags + del];
            string childrenBufer = null;
            int numOfLine = 0;
            for (int pos = 0; pos < sizeOfTags; pos++)
            {
                if (tags[pos] != null)
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
                    textBufer[numOfLine++] = tags[pos].tag + ';' + childrenBufer + ';' + tags[pos].parent;
                    childrenBufer = null;
                }
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
            tags[FindTag(tags, tag)] = null;
            WriteIntoFile(path, tags, -1);
        }

        public static int FindTag(Tags tags, string tag)
        {
            for (int pos = 0; pos < sizeOfTags; pos++)
            {
                if (tags[pos].tag == tag)
                {
                    return pos;
                }
            }
            Tag tagNull = new Tag(null, null, null);
            return 0;    // костыли
        }

        public static void ChangeTag(string path, string currentTag, string newTag = null, string[] newChildren = null, string newParent = null)
        {
            Tags tags = ReadFromFile(path, 0);
            Tag tagCh = tags[FindTag(tags, currentTag)];
            if (newTag != null)
            {
                tagCh.tag = newTag;
            }
            if (newChildren != null)
            {
                tagCh.children = newChildren;
            }
            if (newParent != null)
            {
                tagCh.parent = newParent;
            }
            WriteIntoFile(path, tags);
        }

        public static Tags recieveTag(string path, string tag)
        {
            Tags tags = ReadFromFile(path, 0);
            int sizeOfOut = 0;
            string currentTag = tag;
            for (int i = 0; i < sizeOfTags; i++)
            {
                Tag tagBufer = tags[FindTag(tags, currentTag)];
                if (tagBufer.parent != "")
                {
                    sizeOfOut++;
                    currentTag = tagBufer.parent;
                }
                else
                {
                    break;
                }
            }
            sizeOfOut--;
            Tags tagsOut = new Tags(sizeOfOut);
            currentTag = tag;
            for (int i = 0; i < sizeOfOut; i++)
            {
                tagsOut[i] = tags[FindTag(tags, currentTag)];
                currentTag = tagsOut[i].parent;
            }
            return tagsOut;
        }
    }
