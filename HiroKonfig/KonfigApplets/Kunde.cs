
using System.Text;

namespace HiroKonfig
{
    public class Kunde
    {
        public int ID { get; set; }
        public string Nr { get; set; }
        public int Adressennr { get; set; }
        public byte Gesperrt { get; set; }
        public int GrundGesperrt { get; set; }
        public int MwstBuchungsgruppe { get; set; }
        public int Zahlungsbedingung { get; set; }
        public DateTime AngelegtAm { get; set; }
        public string AngelegtVon { get; set; }
        public DateTime LetzteAenderungAm { get; set; }
        public string LetzteAenderungVon { get; set; }
        public int Kostenstelle { get; set; }
        public int Kostentraeger { get; set; }
        public int Versandart { get; set; }
        public int Lieferbedingung { get; set; }
        public string UmsatzsteuerIdNr { get; set; }
        public string Steuernummer { get; set; }
        public string UnsereLieferantennr { get; set; }
        public int Blz { get; set; }
        public string Kontonr { get; set; }
        public string IbanCode { get; set; }
        public byte Bauleistender { get; set; }
        public int BewertungStatus { get; set; }
        public int Kundengruppe { get; set; }
        public int Rabattgruppe { get; set; }
        public int Formularstatus { get; set; }
        public int Verantwortlich { get; set; }
        public int KontoForderungen { get; set; }
        public int LieferadresseAbw { get; set; }
        public int RechnungsadresseAbw { get; set; }
        public decimal Kreditlimit { get; set; }
        public int Checkliste { get; set; }
        public int Kalkulationsschema { get; set; }
        public int Terminplan { get; set; }
        public byte Einzelmahnungen { get; set; }
        public int Anzahlungsplan { get; set; }
        public int ZusammenfassungAuftragsarten { get; set; }
        public int ServiceZuschlag { get; set; }
        public int Sortierschluessel { get; set; }
        public int Belastung { get; set; }
        public string Qualifikationen { get; set; }
        public byte Archiv { get; set; }
        public int ReVertreter { get; set; }
        public int ReDeckblatt { get; set; }
        public int Jobhoehe { get; set; }
        public byte InPlantafel { get; set; }
        public int AbweichendeFarbeInPl { get; set; }
        public int AbweichendeHoeheInPl { get; set; }
        public int AbweichendeHoeheKapaInPl { get; set; }
        public int MobileVersandart { get; set; }
        public string MobileEmail { get; set; }
        public int Kundenklasse { get; set; }
        public int InterneVersion { get; set; }
        public string SwiftCode { get; set; }
        public int Waehrung { get; set; }
        public byte Mahnstopp { get; set; }
        public string Mahnungsinfo { get; set; }
        public byte KVersandauftragAnlegen { get; set; }
        public DateTime OfflineAenderungszeit { get; set; }
        public string Notiz { get; set; }
        public string SepaIban { get; set; }
        public byte SepaLastMandat { get; set; }
        public string SepaMandatsreferenz { get; set; }
        public int KArbeitsnachweis { get; set; }
        public DateTime OfflineEinfuegezeit { get; set; }
        public string OfflineAenderungVon { get; set; }
        public int KDhlKundenaccount { get; set; }

        public Adresse Adresse { get; set; }

        public string GetAdressenName()
        {
            StringBuilder sb;
            sb = new StringBuilder(Nr)
                .Append(": ")
                .Append(Adresse.Name.Trim())
                .Append(" - ");
            if (Adresse.Name2.Trim().Length > 0)
            {
                sb.Append(Adresse.Name2.Trim()).Append(" - ");
            }
            sb.Append(Adresse.Strasse.Trim())
                .Append(" - ")
                .Append(Adresse.PLZ.Trim())
                .Append(' ')
                .Append(Adresse.Ort.Trim())
                .ToString();
            return sb.ToString();
        }
    }
}
