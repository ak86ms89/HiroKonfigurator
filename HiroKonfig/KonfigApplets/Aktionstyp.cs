using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace HiroKonfig
{
    public class Aktionstyp
    {
        public enum Typen { Termin1=2, Termin2= 230066 }

        public string Code { get; set; }
        public string Beschreibung { get; set; }
        public int VorgabeDauer { get; set; }
        public byte VorgabeTagesdatum { get; set; }
        public int VorgabePrioritaet { get; set; }
        public byte VorgabeErledigt { get; set; }
        public byte Tagesaktion { get; set; }
        public byte Erinnerung { get; set; }
        public int ErinnerungenVorher { get; set; }
        public int EmailVorlage { get; set; }
        public int Farbe { get; set; }
        public byte Kalender { get; set; }
        public byte ErledigungsaktionErstellen { get; set; }
        public int InterneVersion { get; set; }
        public DateTime OfflineAenderungszeit { get; set; }
        public byte KOutlookAbgleich { get; set; }
        public byte KAufgabe { get; set; }
        public DateTime OfflineEinfuegezeit { get; set; }
        public string OfflineAenderungVon { get; set; }
        public int KBenuAuf1 { get; set; }
        public int KBenuAuf2 { get; set; }
        public int KBenuAuf3 { get; set; }
        public int KBenuAuf4 { get; set; }
        public int KBenuAuf5 { get; set; }
   
    }
}
