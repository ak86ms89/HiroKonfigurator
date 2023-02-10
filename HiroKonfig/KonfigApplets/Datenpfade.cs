using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Environment;

namespace HiroKonfig
{
    public static class Pfade
    {
        public static string Settingspfad;
        public static string Datenpfad;
        public static string Usersettings;
        public static string AktuelleAktion;
        public static string Aktionen;
        public static string Checklistpfad;
        public static string Konfigurationenpfad;
        public static string KonfigurationenOutpfad;
        public static string KonfigurationenSentpfad;
        public static string Bilderpfad;

        public static Task Init()
        {
            Settingspfad = @"C:\HiroKonfigData\";
            Usersettings = new StringBuilder(Settingspfad).Append("UserSettings.json").ToString();
            AktuelleAktion = new StringBuilder(Settingspfad).Append("AktionAktuell.json").ToString();
            Usersettings usets = new Usersettings();
            usets.Lade();
            Datenpfad = usets.Datenpfad;
            Aktionen = new StringBuilder(Datenpfad).Append(@"Aktionen\Aktionen.json").ToString();
            Checklistpfad = new StringBuilder(Datenpfad).Append(@"Checklisten\").ToString();
            Konfigurationenpfad = new StringBuilder(Datenpfad).Append(@"Konfigs\All\").ToString();
            KonfigurationenOutpfad = new StringBuilder(Datenpfad).Append(@"Konfigs\Out\").ToString();
            KonfigurationenSentpfad = new StringBuilder(Datenpfad).Append(@"Konfigs\Sent\").ToString();
            Bilderpfad = new StringBuilder(Checklistpfad).Append(@"Bilder\").ToString();


            return Task.CompletedTask;
        }
    }
}
