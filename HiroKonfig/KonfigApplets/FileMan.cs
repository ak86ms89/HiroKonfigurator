using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace HiroKonfig
{
    public class FileMan
    {

        public string FileRoutePath { get; set; }
        public string FileName { get; set; }




        /// <summary>
        /// prüft, ob Directory existiert und legt diese gegebenenfalls an
        /// </summary>
        /// <param name="pfad">zu überprüfender Pfad</param>
        /// <returns>1, wenn vorhanden, 2, wenn neu erzeugt</returns>
        public bool CheckDirectory(string pfad)
        {
            bool r = false;

            try
            {
                if (System.IO.Directory.Exists(pfad) == false)
                {
                    System.IO.Directory.CreateDirectory(pfad);
                    r = true;
                }
                else r = true;
            }
            catch (System.IO.IOException e)
            {
                // throw (e);
            }

            return r;
        }

        public void Write (string txtstream)
        {
            CheckDirectory(FileRoutePath);
            File.WriteAllText(FileRoutePath + FileName, txtstream);
        }
        public string GetNextFileVersion(string filename)
        {
            int versionnr = 0;
            int p1 = 0, p2;
            string r = FileRoutePath + filename;
            FileInfo[] fis = new DirectoryInfo(FileRoutePath).GetFiles(Path.GetFileNameWithoutExtension(filename) + "*");

            foreach (FileInfo file in fis)
            {
                p1 = file.Name.LastIndexOf('_');
                if (p1 >=0)
                {
                    p2 = file.Name.LastIndexOf('.');
                    string v = file.Name.Substring(p1 + 1, p2 - p1 - 1);
                    int.TryParse(v, out int nextversion);
                    versionnr = nextversion > versionnr ? nextversion : versionnr;
                }
            }
            return versionnr > 0
                ? FileRoutePath + filename.Substring(0, p1) + "_" + (versionnr + 1).ToString() + Path.GetExtension(filename)
                : FileRoutePath + Path.GetFileNameWithoutExtension(filename) + "_1" + Path.GetExtension(filename);
        }

        public string GetLastFileVersion(string filename)
        {
            int versionnr = 0;
            int p1 = 0, p2;
            FileInfo[] fis = new DirectoryInfo(FileRoutePath).GetFiles(Path.GetFileNameWithoutExtension(filename) + "*");

            foreach (FileInfo file in fis)
            {
                p1 = file.Name.LastIndexOf('_');
                if (p1 >= 0)
                {
                    p2 = file.Name.LastIndexOf('.');
                    string v = file.Name.Substring(p1 + 1, p2 - p1 - 1);
                    int.TryParse(v, out int nextversion);
                    versionnr = nextversion > versionnr ? nextversion : versionnr;
                }
            }
            return versionnr > 0
                ? FileRoutePath + filename.Substring(0, p1) + "_" + versionnr.ToString() + Path.GetExtension(filename)
                : FileRoutePath + filename;
        }

        public int ReadFileVersion(string filename)
        {
            int version = 0;
            int p1 = 0, p2;
            FileInfo fi = new FileInfo(filename);
            p1 = fi.Name.LastIndexOf('_');
            if (p1 >= 0)
            {
                p2 = fi.Name.LastIndexOf('.');
                string v = fi.Name.Substring(p1 + 1, p2 - p1 - 1);
                int.TryParse(v, out version);
            }
            return version;
        }

        public string[] GetFiles(string filepattern)
        {
            List<string> r = new List<string>();
            FileInfo[] fis;
            CheckDirectory(FileRoutePath);
            fis = new DirectoryInfo(FileRoutePath).GetFiles(Path.GetFileNameWithoutExtension(filepattern) + "*");

            foreach (FileInfo file in fis)
            {
                r.Add(file.FullName);
            }

            return r.ToArray();
        }

      
    }
}
