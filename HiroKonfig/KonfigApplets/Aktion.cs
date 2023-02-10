namespace HiroKonfig
{
    public class Aktion
    {
        public int ID { get; set; }
        public string DieTabelle { get; set; }
        public int Herkunftsnr { get; set; }
        public DateTime AngelegtAm { get; set; }
        public string AngelegtVon { get; set; }
        public DateTime LetzteAenderungAm { get; set; }
        public string LetzteAenderungVon { get; set; }
        public DateTime Startdatum { get; set; }
        public string Startzeit { get; set; }
        public DateTime Enddatum { get; set; }
        public string Endzeit { get; set; }
        public string AktionFuer { get; set; }
        public string Beschreibung { get; set; }
        public byte Erledigt { get; set; }
        public int Dauer { get; set; }
        public byte Privat { get; set; }
        public int Typ { get; set; }
        public int Prioritaet { get; set; }
        public int Ergebnis { get; set; }
        public byte Tagesaktion { get; set; }
        public byte Erinnerung { get; set; }
        public int ErinnerungenVorher { get; set; }
        public int Intervall { get; set; }
        public DateTime ErledigtAm { get; set; }
        public string ErledigtVon { get; set; }
        public int Kontaktperson { get; set; }
        public int Adressennr { get; set; }
        public int Postennr { get; set; }
        public byte Archiv { get; set; }
        public int InterneVersion { get; set; }
        public byte Vertraulich { get; set; }
        public DateTime OfflineAenderungszeit { get; set; }
        public int Marketinggruppe { get; set; }
        public int Checkliste { get; set; }
        public string Beschreibung2 { get; set; }
        public string Notiz { get; set; }
        public DateTime OfflineEinfuegezeit { get; set; }
        public string OfflineAenderungVon { get; set; }

        public Aktionstyp Aktionstyp { get; set; }
        public VKBeleg VKBeleg { get; set; }
    }
}
