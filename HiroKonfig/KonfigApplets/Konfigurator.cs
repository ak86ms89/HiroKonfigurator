using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HiroKonfig
{
    public class Konfigurator
    {
        public enum Modi { frei, Kunde, Aktion }
        public Modi Modus { get; set; }
        public string Produktgruppe { get; set; }
        public int Version { get; set; }
        public DateTime Datum { get; set; }
        public string AnfrageNummer => Aktion?.VKBeleg.Belegnr;
        public string ReservierteAngebotsNummer { get; set; }
        public decimal AngebotspreisVorRabatt { get; set; }
        public decimal Rabattprozent { get; set; }
        public decimal Rabattbetrag => Math.Round(AngebotspreisVorRabatt * Rabattprozent / 100, 2);
        public decimal AngebotspreisInklRabatt => AngebotspreisVorRabatt - Rabattbetrag;
        public decimal Mehrwersteuerfaktor { get; set; } = 0.19m;
        public decimal Mehrwersteuersatz => Mehrwersteuerfaktor * 100;
        public decimal Mehrwersteuerbetrag => Math.Round(AngebotspreisInklRabatt * Mehrwersteuerfaktor, 2);
        public decimal AngebotspreisInklMWSt => AngebotspreisInklRabatt + Mehrwersteuerbetrag;

        public bool IsKonfigurationVollstaendig => KonfigurationVollstaendig();

        public string Kundenadresse => GetKundenadresse();

        public KonfiguratorCheckliste Checkliste { get; set; }

        public Aktion Aktion { get; set; } // wenn vorhanden, die dem Angebot zu grunde liegende Aktion (Aktion enthält die Anfrage

        public Kunde Kunde { get; set; } // wenn Konfiguration ohne Aktion erfolgt

        public Konfigurator()
        {

        }

        private string GetKundenadresse()
        {
            string r = "---";
            switch (Modus)
            {
                case Modi.frei:
                    r = "--- freie Konfiguration ---";
                    break;
                case Modi.Kunde:
                    r = Kunde != null ? Kunde.GetAdressenName() : "--- Kunde nicht gefunden ---";
                    break;
                case Modi.Aktion:
                    r = Aktion.VKBeleg.AdressenName;
                    break;
            }
            return r;
        }
        public void AssignChecklist(string checklistcode)
        {
            Checkliste = new KonfiguratorCheckliste();
            Checkliste = System.Text.Json.JsonSerializer.Deserialize<KonfiguratorCheckliste>(System.IO.File.ReadAllText(Pfade.Checklistpfad + @"\" + checklistcode + ".json"));
        }
        public void Speichern()
        {
            string fname;
            string jsonstream;
            FileMan fileman = new FileMan() { FileRoutePath = Pfade.Konfigurationenpfad};
            fname = fileman.GetNextFileVersion("Konfig" + (Aktion != null ? Aktion.ID.ToString() : 0) + ".json");
            Version = fileman.ReadFileVersion(fname);
            Datum = System.DateTime.Now;
            jsonstream = System.Text.Json.JsonSerializer.Serialize(this);
            System.IO.File.WriteAllText(fname, jsonstream);
        }

        public void SpeicherChecklist()
        {
            string fname;
            string jsonstream;
            fname = Pfade.Checklistpfad + @"\" + Checkliste.ChecklistCode + ".json";
            Checkliste.Datum = System.DateTime.Now.Date;
            Checkliste.Produktgruppe = "Treppenlift";
            jsonstream = System.Text.Json.JsonSerializer.Serialize(Checkliste);
            System.IO.File.WriteAllText(fname, jsonstream);
        }
        public bool KonfigurationVollstaendig()
        {
            int z = 0;
            if (Checkliste != null && Checkliste.Merkmalgruppen != null)
            {
                foreach (KonfiguratorCheckliste.Merkmalgruppe merkmalgruppe in Checkliste.Merkmalgruppen)
                {
                    z = merkmalgruppe.Merkmale.Where(d => d.AlleOptionenDeaktiviert == false).Count(s => s.FeldinhaltGefuellt == false);
                    z += merkmalgruppe.Merkmale.Count(l => l.LimitbreaksExists);
                    if (z > 0) break;
                }
            }
            return z == 0;
        }
        public KonfiguratorCheckliste.Merkmalgruppe GetMerkmalgruppeByID(string merkmalgruppeid)
        {
            return Checkliste.GetMerkmalgruppeByID(merkmalgruppeid);
        }
        public KonfiguratorCheckliste.Merkmal GetMerkmal(string merkmalgruppeid, string merkmalid)
        {
            return Checkliste.GetMerkmal(merkmalgruppeid, merkmalid);
        }
        public KonfiguratorCheckliste.Merkmal GetMerkmal(KonfiguratorCheckliste.InExcludeOption selection)
        {
            return Checkliste.GetMerkmal(selection.MerkmalgruppeID, selection.MerkmalID);
        }
 
        public KonfiguratorCheckliste.Option GetOption(string merkmalgruppeid, string merkmalid, string optioncode)
        {
            return Checkliste.GetOption(merkmalgruppeid, merkmalid, optioncode);
        }
        public KonfiguratorCheckliste.Option GetOption(KonfiguratorCheckliste.InExcludeOption selection)
        {
            return GetOption(selection.MerkmalgruppeID, selection.MerkmalID, selection.OptionCode);
        }

        /// <summary>
        /// enabled oder disabled alle Merkmale gemäß gesetztem wert des übergebenen Merkmals für die verbundenen Merkmale
        /// </summary>
        /// <param name="merkmal"></param>
        public void EnableDisable(KonfiguratorCheckliste.Merkmal merkmal)
        {
            // überprüfen, ob die Auswahl ein anderes Merkmal excludiert oder includiert und in den entsprechenden Optionen enablen oder disablen
            Checkliste.EnableDisable(merkmal);
            AngebotspreisVorRabatt = Checkliste.GetAngebotspreis();
            KonfigurationVollstaendig();
        }
        public void EnableDisableOptions(KonfiguratorCheckliste.InExcludeOption pointertooption)
        {
            Checkliste.EnableDisableOptions(pointertooption);
        }

        public decimal GetAngebotspreis()
        {
            return Checkliste.GetAngebotspreis();
        }
    }
}
