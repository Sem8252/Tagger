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

        //~Tag()
        //{
        //    // деструктор
        //}
    }                                                               // можно не трогать, сами тэги

    class Tags : Tag
    {
        private bool isReaded = false;
        private Tags tagsMain;
        protected Tag[] listOfTags;
        protected int sizeOfTags = 0;
        private Tags(int size)
        {
            listOfTags = new Tag[size];
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

        public Tags readTagsFromFile(string tag, string path)
        {
            string[][] line;
            int numOfLine = 0;
            if (!isReaded)
            {
                foreach (var Line in File.ReadAllLines(path))
                {
                    line[numOfLine] = Line.Split(';');
                    numOfLine++;
                }
                sizeOfTags += numOfLine + 2;      // чтобы в массиве listOfTags был запас в один элемент (на всякий случай)
                tagsMain = new Tags(sizeOfTags);
                foreach (var Line in line)
                {
                    tags[Line[0]] = new Tag { Line[0], Line[1], Line[2].Split(',') };
                }
                isReaded = true;
            }
            return tagsMain[tag];
        }

        public void writeTagsIntoFile(string path)
        {
            string[] tagsBufer;
            int pos = 0;
            foreach (string index in tagsMain)
            {
                string child = null;
                foreach (var element in tagsMain[index].childrens)
                {
                    child += element + ',';
                }
                child -= ',';
                tagsBufer[pos] = tagsMain[index].tag + ';' + tagsMain[index].parent + ';' + child;
            }
            File.WriteAllLines(path, tagsBufer);
            isReaded = false;
        }

        public void addTag(string tag, string parent, string[] childrens, string path)
        {
            sizeOfTags++;
            readTagsFromFile(path);
            tagsMain[tag] = new Tag(tag, parent, childrens);
            writeTagsIntoFile(path);
        }

        public void deleteTag(string tag, string path)
        {
            readTagsFromFile(path);                                                          // доделать
            sizeOfTags--;
            tagsMain[tag] = null;
            writeTagsIntoFile(path);
        }

        public void changeTag(string currentTag, string path, string newNameOfTag = null, string newParent = null, string[] newChildrens = null)
        {
            readTagsFromFile(path);
            if (newNameOfTag = null)
            {
                newNameOfTag = currentTag;
            }
            
            if (newParent = null)
            {
                newParent = tagsMain[currentTag].parent;
            }
            if (newChildrens = null)
            {
                newChildrens = tagsMain[currentTag].childrens;
            }
            tagsMain[currentTag] = null;
            tagsMain[newNameOfTag] = new Tag(newNameOfTag, newParent, newChildrens);
            writeTagsIntoFile(path);
        }
    }
}
