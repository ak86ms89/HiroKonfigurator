using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiroKonfig
{
    public class Usersettings
    {
        public string Benutzer { get; set; } = "";
        public string Namenskuerzel { get; set; } = "";
        public string Datenpfad { get; set; } = "";
        public decimal Maximalrabatt { get; set; }

        public bool Exists => Namenskuerzel.Length > 0;

        public Usersettings()
        {
        }
        public void Speichern()
        {
            new FileMan() { FileRoutePath = Pfade.Settingspfad, FileName = "UserSettings.json" }.Write(System.Text.Json.JsonSerializer.Serialize(this));
        }

        public void Lade()
        {
            Usersettings usets = (Usersettings)new DataLoader().Lade<Usersettings>(Pfade.Usersettings);

            if (usets != null)
            {
                Benutzer = usets.Benutzer;
                Namenskuerzel = usets.Namenskuerzel;
                Datenpfad = usets.Datenpfad;
                Maximalrabatt = usets.Maximalrabatt;
            }
        }

    }

}
