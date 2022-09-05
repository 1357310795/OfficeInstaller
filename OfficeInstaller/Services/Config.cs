using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OfficeInstaller.Services
{
    public class Config
    {
        public static Config Default { get; set; } = new Config();
        public bool PowerPoint { get; set; } = true;
        public bool Word { get; set; } = true;
        public bool Excel { get; set; } = true;
        public bool Access { get; set; }
        public bool Lync { get; set; }
        public bool OneDrive { get; set; }
        public bool OneNote { get; set; }
        public bool Outlook { get; set; }
        public bool Publisher { get; set; }
        public bool Teams { get; set; }
        public bool Visio { get; set; } = false;
        public bool Project { get; set; }

        public bool X64 { get; set; } = true;
        public bool Eng { get; set; } = false;

        public XmlDocument GetXml()
        {
            XmlElement GetLangNode(XmlDocument doc1)
            {
                XmlElement lang = doc1.CreateElement("Language");
                lang.SetAttribute("ID", Eng ? "en-us" : "zh-cn");
                return lang;
            }

            XmlElement GetExcludeNode(XmlDocument doc1, string name)
            {
                XmlElement ex = doc1.CreateElement("ExcludeApp");
                ex.SetAttribute("ID", name);
                return ex;
            }
            
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Configuration");
            XmlElement add = doc.CreateElement("Add");
            add.SetAttribute("OfficeClientEdition", "64");
            add.SetAttribute("Channel", "PerpetualVL2021");

            XmlElement main = doc.CreateElement("Product");
            main.SetAttribute("ID", "ProPlus2021Volume");
            main.AppendChild(GetLangNode(doc));
            add.AppendChild(main);

            if (!PowerPoint) main.AppendChild(GetExcludeNode(doc, "PowerPoint"));
            if (!Word) main.AppendChild(GetExcludeNode(doc, "Word"));
            if (!Excel) main.AppendChild(GetExcludeNode(doc, "Excel"));
            if (!Access) main.AppendChild(GetExcludeNode(doc, "Access"));
            if (!Lync) main.AppendChild(GetExcludeNode(doc, "Lync"));
            if (!OneDrive) main.AppendChild(GetExcludeNode(doc, "OneDrive"));
            if (!OneNote) main.AppendChild(GetExcludeNode(doc, "OneNote"));
            if (!Outlook) main.AppendChild(GetExcludeNode(doc, "Outlook"));
            if (!Publisher) main.AppendChild(GetExcludeNode(doc, "Publisher"));
            if (!Teams) main.AppendChild(GetExcludeNode(doc, "Teams"));

            if (Visio)
            {
                XmlElement visio = doc.CreateElement("Product");
                visio.SetAttribute("ID", "VisioPro2021Volume");
                visio.AppendChild(GetLangNode(doc));
                add.AppendChild(visio);
            }

            if (Project)
            {
                XmlElement project = doc.CreateElement("Product");
                project.SetAttribute("ID", "ProjectPro2021Volume");
                project.AppendChild(GetLangNode(doc));
                add.AppendChild(project);
            }

            root.AppendChild(add);
            XmlElement remove = doc.CreateElement("Remove");
            remove.SetAttribute("All", "True");
            root.AppendChild(remove);
            doc.AppendChild(root);

            return doc;
        }
    }
}
