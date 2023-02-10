using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiroKonfig.Services;

namespace HiroKonfig
{
    public class AktionMan
    {
        public async Task Aktualisiere()
        {
            Usersettings usets = new Usersettings();
            usets.Lade();
            string jsonstream = await ApiService.GetFromDB(
                new StringBuilder()
                .Append("ActivityList/GetOpenByTypName/").Append("2/").Append(usets.Benutzer).ToString()
                );
            File.Delete(Pfade.Aktionen);
            File.AppendAllText(Pfade.Aktionen, jsonstream);

            return Task.CompletedTask;
        }
    }
}
