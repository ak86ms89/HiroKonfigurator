using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiroKonfig
{
    public class Adresse
    {
        public int ID { get; set; }
        public string Nr { get; set; }
        public string Notiz { get; set; }
        public int Anrede { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; }
        public string Name3 { get; set; }
        public string Strasse { get; set; }
        public int PlzID { get; set; }
        public string Ort { get; set; }
        public string Telefon { get; set; }
        public string Telefax { get; set; }
        public string Mobilfunk { get; set; }
        public string Email { get; set; }
        public string Homepage { get; set; }
        public byte Gesperrt { get; set; }
        public int GrundGesperrt { get; set; }
        public int Kundennr { get; set; }
        public int Lieferantennr { get; set; }
        public int MitarbeiterID { get; set; }
        public int Servicetechnikernr { get; set; }
        public int Status { get; set; }
        public int Branche { get; set; }
        public int Vertriebsbereich { get; set; }
        public int Verantwortlich { get; set; }
        public int Herkunft { get; set; }
        public int Land { get; set; }
        public int Sprache { get; set; }
        public int MwstBuchungsgruppe { get; set; }
        public int Zahlungsbedingung { get; set; }
        public DateTime AngelegtAm { get; set; }
        public string AngelegtVon { get; set; }
        public DateTime LetzteAenderungAm { get; set; }
        public string LetzteAenderungVon { get; set; }
        public string Marketinggruppen { get; set; }
        public int Adressengruppe { get; set; }
        public int Formularstatus { get; set; }
        public string Strasse2 { get; set; }
        public string Strasse3 { get; set; }
        public string Region { get; set; }
        public string ReName3 { get; set; }
        public string ReStrasse { get; set; }
        public int RePlz { get; set; }
        public string ReOrt { get; set; }
        public byte AbwRechnungsanschrift { get; set; }
        public string Briefanrede { get; set; }
        public byte BenutzerdefinierteBriefanrede { get; set; }
        public string TelefonOhne { get; set; }
        public string MobilfunkOhne { get; set; }
        public int InterneVersion { get; set; }
        public byte Archiv { get; set; }
        public DateTime OfflineAenderungszeit { get; set; }
        public int KKeyaccount { get; set; }
        public DateTime OfflineEinfuegezeit { get; set; }
        public string OfflineAenderungVon { get; set; }
        public string PLZ { get; set; }


    }
}
