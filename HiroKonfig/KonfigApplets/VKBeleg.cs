using System;
using System.Collections.Generic;
using System.Text;

namespace HiroKonfig
{
    public class VKBeleg
    {

        public string AngebotsOderAuftragsnr => Belegart == EnumsVerkaufsbeleg.Belegarten.Angebot ? Angebotsnr : Belegart == EnumsVerkaufsbeleg.Belegarten.Auftrag ? Auftragsnr : "";
        public EnumsVerkaufsbeleg.Kundenquelle Kundenquelle { get => (EnumsVerkaufsbeleg.Kundenquelle)KundenquelleID; }
        public EnumsVerkaufsbeleg.Belegarten Belegart { get => (EnumsVerkaufsbeleg.Belegarten)BelegartID; }
        public string Herkunftsarttxt => Enum.GetNames(typeof(EnumsVerkaufsbeleg.Belegarten))[(int)Herkunftsart];

        public int Herkunftsart { get; set; }
        public int BelegartID { get; set; }
        public string Belegnr { get; set; }
        public DateTime Belegdatum { get; set; }
        public string Beleguhrzeit { get; set; }
        public DateTime Buchungsdatum { get; set; }
        public string Belegtext { get; set; }
        public int KundenquelleID { get; set; }
        public int KundenoderAdressenID { get; set; }
        public string KundenoderAdressenName { get; set; }
        public string KundenoderAdressenName2 { get; set; }
        public string KundenoderAdressenName3 { get; set; }
        public string VkKontaktperson { get; set; }
        public string KundenoderAdressenStrasse { get; set; }
        public int KundenoderAdressenPLZID { get; set; }
        public string KundenoderAdressenOrt { get; set; }
        public int LieferID { get; set; }
        public string LieferName { get; set; }
        public string LieferName2 { get; set; }
        public string LieferName3 { get; set; }
        public string LieferKontaktperson { get; set; }
        public string LieferStrasse { get; set; }
        public int LieferPLZID { get; set; }
        public string LieferOrt { get; set; }
        public int ReNr { get; set; }
        public string RechnungName { get; set; }
        public string RechnungName2 { get; set; }
        public string RechnungName3 { get; set; }
        public string RechnungKontaktperson { get; set; }
        public string RechnungStrasse { get; set; }
        public int RechnungPLZID { get; set; }
        public string RechnungOrt { get; set; }
        public int AuftragsartID { get; set; }
        public DateTime Lieferdatum { get; set; }
        public string Lieferzeit { get; set; }
        public int KostenstelleID { get; set; }
        public int KostentraegerID { get; set; }
        public int LagerortID { get; set; }
        public int MwstBuchungsgruppe { get; set; }
        public int Anlagennr { get; set; }
        public int Projektnr { get; set; }
        public int Verkaeufer { get; set; }
        public int Sachbearbeiter { get; set; }
        public int Vertragsnr { get; set; }
        public int Zahlungsbedingung { get; set; }
        public int FaelligkeitTage { get; set; }
        public int SkontoTage { get; set; }
        public decimal SkontoProzent { get; set; }
        public DateTime SkontoDatum { get; set; }
        public DateTime FaelligkeitDatum { get; set; }
        public byte Einzug { get; set; }
        public int Blz { get; set; }
        public string Kontonr { get; set; }
        public string IbanCode { get; set; }
        public string Verwendungszweck { get; set; }
        public byte BelegErledigt { get; set; }
        public DateTime BelegErledigtAm { get; set; }
        public string BelegErledigtVon { get; set; }
        public int BelegErledigtDauer { get; set; }
        public string BelegErledigtBis { get; set; }
        public int Sachkonto { get; set; }
        public DateTime BelegGueltigBis { get; set; }
        public int Stoerungsnr { get; set; }
        public string IhrZeichen { get; set; }
        public string IhreNummer { get; set; }
        public DateTime IhreNummerVom { get; set; }
        public int Versandart { get; set; }
        public int Lieferbedingung { get; set; }
        public byte BelegBuchen { get; set; }
        public string Servicetechniker { get; set; }
        public DateTime AngelegtAm { get; set; }
        public string AngelegtVon { get; set; }
        public DateTime LetzteAenderungAm { get; set; }
        public string LetzteAenderungVon { get; set; }
        public int AnzahlGedruckt { get; set; }
        public int AusgleichPostennr { get; set; }
        public byte Bauleistung { get; set; }
        public int Formularstatus { get; set; }
        public int Vorgangsnr { get; set; }
        public byte BuchenNurBerechnen { get; set; }
        public int BuchenVerkaufskopfnr { get; set; }
        public int BuchenRahmenauftragsnr { get; set; }
        public int KalkulationsschemaID { get; set; }
        public int ChecklisteID { get; set; }
        public int Terminplan { get; set; }
        public DateTime Montagedatum { get; set; }
        public int Projektleiter { get; set; }
        public int Versionsnr { get; set; }
        public int Anzahlungsplan { get; set; }
        public string Fabriknr { get; set; }
        public int ReVertreter { get; set; }
        public int ReDeckblatt { get; set; }
        public int Bewertungsart { get; set; }
        public DateTime AuftragseingangBis { get; set; }
        public decimal AuftragseingangVolumen { get; set; }
        public int Verkaufskopfnr { get; set; }
        public string VkKontaktpersonTelefon { get; set; }
        public decimal RabattProzent { get; set; }
        public int LieferzeitTage { get; set; }
        public string Angebotsnr { get; set; }
        public string Auftragsnr { get; set; }
        public int GehoertZuAuftragsnr { get; set; }
        public int VkAdressennr { get; set; }
        public int RechnungsadresseID { get; set; }
        public int LiAdressennr { get; set; }
        public int VkKontaktpersonnr { get; set; }
        public int ReKontaktpersonnr { get; set; }
        public int LiKontaktpersonnr { get; set; }
        public int AnzahlungenArt { get; set; }
        public int AuftragsartAnzahlungen { get; set; }
        public byte NachtextPreisanpassungDrucken { get; set; }
        public int AngebotsID { get; set; }
        public byte Barverkauf { get; set; }
        public int SyncStatus { get; set; }
        public DateTime SyncDatum { get; set; }
        public byte Nachkalkuliert { get; set; }
        public int Archivierungsgrund { get; set; }
        public decimal LohnkostenanteilProzent { get; set; }
        public byte Differenzgutschrift { get; set; }
        public byte OhneLieferschein { get; set; }
        public int Nachberechnung { get; set; }
        public string LieferStrasse2 { get; set; }
        public string LieferStrasse3 { get; set; }
        public string LieferRegion { get; set; }
        public string LieferPLZ { get; set; }
        public int LieferLand { get; set; }
        public string RechnungStrasse2 { get; set; }
        public string RechnungStrasse3 { get; set; }
        public string RechnungRegion { get; set; }
        public string RechnungPlzText { get; set; }
        public int RechnungLand { get; set; }
        public string KundenoderAdressenStrasse2 { get; set; }
        public string KundenoderAdressenStrasse3 { get; set; }
        public string VkRegion { get; set; }
        public string VkPlzText { get; set; }
        public int VkLand { get; set; }
        public byte Stornorechnung { get; set; }
        public string SyncZeit { get; set; }
        public string BeleguhrzeitBis { get; set; }
        public int InterneVersion { get; set; }
        public int KHerkunft { get; set; }
        public int KTippgeber1 { get; set; }
        public int KTippgeber2 { get; set; }
        public int KTippgeber3 { get; set; }
        public byte KAbgerechnet { get; set; }
        public string KLiTelefon { get; set; }
        public int Waehrung { get; set; }
        public decimal Wechselkurs { get; set; }
        public DateTime KAuszahlungsdatum1 { get; set; }
        public DateTime KAuszahlungsdatum2 { get; set; }
        public DateTime KAuszahlungsdatum3 { get; set; }
        public decimal ZeitvorgabeStunden { get; set; }
        public byte LieferscheinBuchen { get; set; }
        public decimal Rechnungsrabatt { get; set; }
        public int ReVertreterKontaktpersonnr { get; set; }
        public int ReDeckblattKontaktpersonnr { get; set; }
        public string KBauvorhaben { get; set; }
        public string KProvisionsinfo { get; set; }
        public string KMontageauftragsnr { get; set; }
        public byte KAnschreibenErfolgt { get; set; }
        public int KInteresse { get; set; }
        public DateTime OfflineAenderungszeit { get; set; }
        public DateTime KFestpreisbindung { get; set; }
        public string Notiz { get; set; }
        public string Vortext { get; set; }
        public string Nachtext { get; set; }
        public string TextZahlungsbedingung { get; set; }
        public string Auftragstext { get; set; }
        public string Erledigungstext { get; set; }
        public decimal GpsLatitude { get; set; }
        public decimal GpsLongitude { get; set; }
        public string KArchivierungstext { get; set; }
        public int SyncAnnahmestatus { get; set; }
        public byte KOutlookLi { get; set; }
        public byte KOutlookRe { get; set; }
        public byte KOutlookVk { get; set; }
        public byte KNettoangebot { get; set; }
        public int KGutschriftsgrund { get; set; }
        public DateTime KStornodatum { get; set; }
        public int KAuftragsartAlt { get; set; }
        public string SepaIban { get; set; }
        public string KPaketnr { get; set; }
        public decimal KVersandBruttoGewicht { get; set; }
        public int KDhlVerfahren { get; set; }
        public int KDhlProdukt { get; set; }
        public int KDhlService { get; set; }
        public DateTime KVersanddatum { get; set; }
        public int KVerkaeuferInnendienst { get; set; }
        public DateTime OfflineEinfuegezeit { get; set; }
        public string OfflineAenderungVon { get; set; }
        public int KDhlKundenaccount { get; set; }
        public string KAnfragennr { get; set; }
        public byte KDhlEmailVersandt { get; set; }

        public Postleitzahl PLZ { get; set; }

        public string VKAdresseLang { get => KundenoderAdressenName + Environment.NewLine 
                                            + (KundenoderAdressenName2.Trim().Length > 0 ? KundenoderAdressenName2.Trim() + Environment.NewLine : "")
                                            + (KundenoderAdressenName3.Trim().Length > 0 ? KundenoderAdressenName3.Trim() + Environment.NewLine : "")
                                            + (KundenoderAdressenStrasse.Trim().Length > 0 ? KundenoderAdressenStrasse + Environment.NewLine : "")
                                            + (KundenoderAdressenStrasse2.Trim().Length > 0 ? KundenoderAdressenStrasse2 + Environment.NewLine : "")
                                            + (KundenoderAdressenStrasse3.Trim().Length > 0 ? KundenoderAdressenStrasse3 + Environment.NewLine : "")
                                            + PLZ.Code + " " + KundenoderAdressenOrt.Trim(); }
 
        public string AdressenName { get => new StringBuilder(KundenoderAdressenName)
                        .Append(" - ")
                        .Append(KundenoderAdressenStrasse)
                        .Append(" - ")
                        .Append(PLZ.Code)
                        .Append(" ")
                        .Append(KundenoderAdressenOrt)
                        .ToString();
        }
        public string PLZOrt {  get => PLZ.Code + " " + KundenoderAdressenOrt.Trim(); }

        public EnumItem.Stuecklistenherkunft Stuecklistenherkunft { get; set; } = EnumItem.Stuecklistenherkunft.Verkauf;

 
    }
}
