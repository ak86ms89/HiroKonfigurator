using System;
using System.Collections.Generic;
using System.Text;

namespace HiroKonfig
{
    public class EnumsVerkaufsbeleg
    {
        public enum Kundenquelle { NV = 0, Kunde = 1, Adresse   = 2 }
        public enum Belegarten { NV = 0, Angebot = 1, Auftrag = 2 }

    }
    public class EnumItem
    {
        public enum Itemarten { leer = 1, Artikel = 2, Ressource = 3 }
        public enum Stuecklistenherkunft { alles = 1, Verkauf = 2, Konstruktion = 3 }

    }
}
