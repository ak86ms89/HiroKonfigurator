using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiroKonfig
{
    public class DataLoader
    {
        public object Lade<T>(string fname)
        {
            return File.Exists(fname) ? System.Text.Json.JsonSerializer.Deserialize<T>(File.ReadAllText(fname)) : null;
        }
   
    }
}
