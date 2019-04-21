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

        private readFromFile()
        {
            
        }
    }
}
