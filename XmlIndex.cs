using System;
using System.Collections.Generic;
using System.Text;

namespace XmlCutter
{
    class XmlIndex
    {


        private string root;
        private string attr;
        private string type;
        
        public string Root
        {
            get { return root; }
            set { root = value; }
        }

        public string Attr
        {
            get { return attr; }
            set { attr = value; }
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public XmlIndex()
        {

        }

        public override string ToString()
        {
            return "[Root:" + root + ",Attr:" + attr + ",Type:" + type+"]";
        }
        
    }
}
