using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace BlueprintSystem
{
    public class ModInfo
    {
		public const string CoreLabel = "Animocity";

        public string label;
        private string path;
        private string author;
        private string description;

        private bool active;

        public static List<ModInfo> CurrentMods;

        public string Path{
       	 	get{
        		return path;
        	}
        }

		public string Author{
       	 	get{
        		return author;
        	}
        }
		public string Description{
       	 	get{
        		return description;
        	}
        }

        public ModInfo(string path, ref bool errored)
        {
            this.path = path;

            string sAbout = System.IO.Path.Combine(path, "About.xml");
            if(!File.Exists(sAbout)){
                Console.WriteLine(string.Format("No About.xml file found in mod at path {0}.", path));
                errored = true;
            }
            else{
                XmlDocument xml = new XmlDocument();
                xml.Load(sAbout);
                label           = xml.SelectSingleNode("/About/Title").InnerText;
                author          = xml.SelectSingleNode("/About/Author").InnerText;
                description     = xml.SelectSingleNode("/About/Description").InnerText;
				errored = false;
            }
        }


        public bool Active{
            get{
                return this.active;
            }
            set{
                this.active = value;
            }
        }
    }
}


