using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace HiroKonfig
{
    public class Checklisten
    {

        public List<KonfiguratorCheckliste> Liste { get; set; }

        public Checklisten()
        {

        }

        public int ReadByProduktgruppe(string produktgruppe)
        {
            FileInfo[] fis = new DirectoryInfo(Pfade.Checklistpfad).GetFiles();
            KonfiguratorCheckliste checklist;
            Liste = new List<KonfiguratorCheckliste>();
            foreach  (FileInfo fi in fis)
            {
                if (fi.Extension == ".json")
                {
                    checklist = System.Text.Json.JsonSerializer.Deserialize<KonfiguratorCheckliste>(File.ReadAllText(fi.FullName));
                    if (checklist.Produktgruppe == produktgruppe)
                        Liste.Add(checklist);
                }
            }
            return Liste.Count;
        }
 

    }
}
