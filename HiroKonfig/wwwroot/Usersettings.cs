using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiroKonfig
{
    public class Usersettings
    {

        public string Namenskuerzel { get; set; } = "";
        public string Datenpfad { get; set; } = "";
        public decimal Maximalrabatt { get; set; }

        public bool Exists => Namenskuerzel.Length > 0;

        public Usersettings()
        {
        }
        public void Speichern()
        {
            new FileMan() { FileRoutePath = "./Data/", FileName = "UserSettings.json" }.Write(System.Text.Json.JsonSerializer.Serialize(this));
        }

        public void Lade()
        {
            // Usersettings usets = System.Text.Json.JsonSerializer.Deserialize<Usersettings>(File.ReadAllText(Pfade.Usersettings));

            Usersettings usets = (Usersettings)DataLoader.Lade<Usersettings>(Pfade.Usersettings);

            if (usets != null)
            {
                Namenskuerzel = usets.Namenskuerzel;
                Datenpfad = usets.Datenpfad;
                Maximalrabatt = usets.Maximalrabatt;
            }

            /*
            Usersettings usets = File.Exists(x)
                 ? System.Text.Json.JsonSerializer.Deserialize<Usersettings>(File.ReadAllText(Pfade.Usersettings))
                 : new Usersettings();
            */
        }

    }

}
