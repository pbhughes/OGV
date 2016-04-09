using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Infrastructure.ExtendedObjects;
using System.Drawing;
using System.Xml.Linq;
using Infrastructure.Extensions;
using System.Xml;

namespace Infrastructure.ExtendedObjects
{
    public partial class ExtendedTreeNode : System.Windows.Forms.TreeNode
    {

        private Item _item;

        public Item AgendaItem
        {
            get
            {
                return _item;
            }

            set
            {
                _item = value;
            }
        }

        private long _startingHash;

        public long StartingHash
        {
            get
            {
                return _startingHash;
            }

            set
            {
                _startingHash = value;
            }
        }

        public void MarkItemStamped(string titleText, int seconds, bool advance = true)
        {

            TimeSpan stamp = new TimeSpan(0, 0, seconds);
            if (ImageKey.ToLower().Contains("edited"))
            {
                ImageKey = "stamped_edited";
                SelectedImageKey = "stamped_edited";
            }
            else
            {
                ImageKey = "stamped";
                SelectedImageKey = "stamped";
            }

            _item.TimeStamp = (int)stamp.TotalSeconds;
            string newTitle = Stamp((int)stamp.TotalSeconds);
            Text = newTitle;
            BackColor = Color.LightBlue;

            if (NextVisibleNode != null)
                this.TreeView.SelectedNode = NextVisibleNode;
           
         }

        public void MarkItemUnstamped()
        {
            SetTimeStamp(0);

            ImageKey = "unselected";
            SelectedImageKey = "unselected";

            BackColor = Color.Transparent;

           
        }

        private string Stamp(int seconds)
        {
       
            string newValue = AgendaItem.StampTitle(seconds);
            System.Diagnostics.Debug.WriteLine(string.Format("Stamping Item: Old value: {0} new Value {1}", AgendaItem.Title, newValue));
            return newValue;

        }

        public void SetTimeStamp(int seconds)
        {
            Text = Stamp(seconds);
            AgendaItem.Title = Text;
        }

        public ExtendedTreeNode():base()
        {

        }

        public ExtendedTreeNode(Item item) : base()
        {

        }

        public ExtendedTreeNode(string title): base(title)
        {
            _item = new Item() { Title = title, TimeStamp = 0};
        }

        public override string ToString()
        {
            StringBuilder content = new StringBuilder();
            
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.OmitXmlDeclaration = true;
            settings.CheckCharacters = true;
            settings.Indent = true;
            settings.CloseOutput = true;
            settings.IndentChars = "\t";
            using (var xmlWr = XmlWriter.Create(content, settings))
            {

                xmlWr.WriteStartElement("item");
                xmlWr.WriteElementString("title", _item.Title);
                if( !string.IsNullOrEmpty(_item.Description))
                {
                    xmlWr.WriteElementString("description", _item.Description);
                }
                
                if(_item.TimeStamp != null)
                {
                    xmlWr.WriteElementString("timestamp", XmlConvert.ToString(_item.TimeStamp));
                }
               
                xmlWr.WriteStartElement("items");
                if (Nodes.Count > 0)
                {
                    foreach (ExtendedTreeNode etn in Nodes)
                    {
                        xmlWr.WriteRaw(etn.ToString());
                    }
                }

                xmlWr.WriteEndElement();
                xmlWr.WriteEndElement();
            }
            return content.ToString();
        }
    }
}
