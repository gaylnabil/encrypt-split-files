using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
namespace Split_Any_Files
{
    [Serializable()]
    public class  MyConfiguration
    {
        private string[] paths;
        private string pathSelected;
        private int theme;

        public MyConfiguration()
        {
            theme = 5;
            paths = new string[] { "" };
            pathSelected = "";
        }
        public MyConfiguration(string[] paths, string pathSelected, int theme)
        {
            this.paths=paths;
            this.pathSelected = pathSelected;
            this.theme = theme;
        }

        public string[] getPaths()
        {
            return this.paths;
        }
        
        public void setPaths(string[] paths)
        {
            this.paths = paths;
        }

        public string getPathSelected()
        {
            return this.pathSelected;
        }

        public void setPathSelected(string pathSelected)
        {
            this.pathSelected = pathSelected;
        }

        public int getTheme()
        {
            return this.theme;
        }

        public void setTheme(int theme)
        {
            this.theme = theme;
        }

        public void saveConfiguration()
        {
            Stream file = File.OpenWrite("config.bin");
            BinaryFormatter w = new BinaryFormatter();
            w.Serialize(file, this);
            file.Close();
        }
        public void loadConfiguration()
        {
            if (File.Exists("config.bin"))
            {
                Stream file = File.OpenRead("config.bin");
                BinaryFormatter w = new BinaryFormatter();
                MyConfiguration config = (MyConfiguration)w.Deserialize(file);
                this.setPaths(config.getPaths());
                this.setPathSelected(config.getPathSelected());
                this.setTheme(config.getTheme());
                file.Close();
            }
        }

    }
}
