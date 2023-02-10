using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HiroKonfig
{
    public class Checkliste
    {
        #region XML
        private XmlDocument XMLDoc;
        public bool OpenXMLChecklist(string filename)
        {
            bool r = true;

            XmlReader rdr = null;

            try
            {
                XMLDoc = new XmlDocument();
                rdr = XmlReader.Create(filename);
                XMLDoc.Load(rdr);
                rdr.Close();
            }
            catch (Exception e)
            {
                r = false;
                // GlobalLib.ApplicationErrorLogger.Add(this, filename, _fehlercode = -100, filename + " konnte nicht geöffnet werden", e.Message + " - " + e.StackTrace);
            }

            return r;
        }
        public void CreateChecklist(string filename)
        {
            Merkmalgruppe merkmalgruppe;
            Merkmal merkmal;
            Option option;
            InExcluder excluder, includer;
            InExcludeSelection excludeselection, includeselection;

            OpenXMLChecklist(filename);

            XmlNodeList nlmerkmalgruppen, nlmerkmale, nloptionen, nlmerkmalexcluder
                , nlexcluder, nlexcludeselection, nlincluder, nlincludeselection, nlconectors;

            ChecklistCode = XMLDoc.SelectSingleNode("Doc/Header/Code").InnerText;

            Merkmalgruppen = new List<Merkmalgruppe>();
            nlmerkmalgruppen = XMLDoc.SelectNodes("Doc/Merkmalgruppe");
            foreach (XmlNode mgnode in nlmerkmalgruppen)
            {
                merkmalgruppe = new Merkmalgruppe
                {
                    ID = mgnode.Attributes["ID"].Value,
                    Text = mgnode.Attributes["Text"].Value
                };

                nlmerkmale = mgnode.SelectNodes("Merkmal");
                foreach (XmlNode mnode in nlmerkmale)
                {
                    merkmal = new Merkmal
                    {
                        ID = mnode.Attributes["ID"].Value,
                        Text = mnode.Attributes["Text"].Value,
                        Typ = mnode.Attributes["Typ"].Value,
                        PreisProEinheit = Convert.ToDecimal(mnode.Attributes["Preis"] != null ? mnode.Attributes["Preis"].Value : 0)
                    };

                    #region Optionen
                    // alle Excluders, die alle Optionen des Merkmals gleichermaßen betreffen 
                    // => ganze Merkmal ist deaktiviert
                    nlmerkmalexcluder = mnode.SelectNodes("Optionen/Exclude");

                    // alle Excluders für einzelne Optionen des Merkmals
                    nloptionen = mnode.SelectNodes("Optionen/Option");
                    foreach (XmlNode onode in nloptionen)
                    {
                        option = new Option
                        {
                            Code = onode.Attributes["Code"].Value,
                            Text = onode.Attributes["Text"] != null ? onode.Attributes["Text"].Value : onode.Attributes["Code"].Value,
                            Preis = onode.Attributes["Preis"] != null ? Convert.ToDecimal(onode.Attributes["Preis"].Value) : 0
                        };

                        // Optionsexculder
                        nlexcluder = onode.SelectNodes("Exclude");

                        var excludelist = nlexcluder.Cast<XmlNode>().Concat(nlmerkmalexcluder.Cast<XmlNode>()).ToList();

                        foreach (XmlNode exnode in excludelist)
                        {
                            excluder = new InExcluder
                            {
                            };

                            nlexcludeselection = exnode.SelectNodes("ExcludeSelection");
                            foreach (XmlNode exselnode in nlexcludeselection)
                            {
                                excludeselection = new InExcludeSelection
                                {
                                    MerkmalgruppeID = exselnode.Attributes["MerkmalgruppeID"].Value,
                                    MerkmalID = exselnode.Attributes["MerkmalID"].Value,
                                    OptionCode = exselnode.Attributes["Option"].Value
                                };
                                excluder.Selection.Add(excludeselection);
                            }

                            option.Excluders.Add(excluder);

                        }

                        // Optionsincluders
                        nlincluder = onode.SelectNodes("Include");
                        foreach (XmlNode exnode in nlincluder)
                        {
                            includer = new InExcluder
                            {
                            };

                            nlincludeselection = exnode.SelectNodes("IncludeSelection");
                            foreach (XmlNode inselnode in nlincludeselection)
                            {
                                includeselection = new InExcludeSelection
                                {
                                    MerkmalgruppeID = inselnode.Attributes["MerkmalgruppeID"].Value,
                                    MerkmalID = inselnode.Attributes["MerkmalID"].Value,
                                    OptionCode = inselnode.Attributes["Option"].Value
                                };
                                includer.Selection.Add(includeselection);
                            }

                            option.Includers.Add(includer);

                        }

                        merkmal.Optionen.Add(option);
                    }
                    #endregion

                    #region Connectors

                    #endregion

                    #region Bedingungen


                    #endregion
                    merkmalgruppe.Merkmale.Add(merkmal);
                }

                Merkmalgruppen.Add(merkmalgruppe);
            }

            CreateOptionpointers();
        }
        public void CreateOptionpointers()
        {
            foreach (Merkmalgruppe merkmalgruppe in Merkmalgruppen)
            {
                foreach (Merkmal merkmal in merkmalgruppe.Merkmale)
                {

                    // Merkmal Excluder und Includers
                    foreach (InExcluder excluder in merkmal.Excluders)
                    {
                        foreach (InExcludeSelection selection in excluder.Selection)
                        {
                            GetMerkmal(selection)?.PointersToOptions.Add(new Merkmal.InExcludePointer
                            {
                                MerkmalgruppenID = merkmalgruppe.ID,
                                MerkmalID = merkmal.ID,
                                OptionCode = ""
                            });
                        }
                    }
                    foreach (InExcluder includer in merkmal.Includers)
                    {
                        foreach (InExcludeSelection selection in includer.Selection)
                        {
                            GetMerkmal(selection)?.PointersToOptions.Add(new Merkmal.InExcludePointer
                            {
                                MerkmalgruppenID = merkmalgruppe.ID,
                                MerkmalID = merkmal.ID,
                                OptionCode = ""
                            });
                        }
                    }

                    // Option Excluder und Includers
                    foreach (Option option in merkmal.Optionen)
                    {
                        foreach (InExcluder excluder in option.Excluders)
                        {
                            foreach (InExcludeSelection selection in excluder.Selection)
                            {
                                GetMerkmal(selection)?.PointersToOptions.Add(new Merkmal.InExcludePointer
                                {
                                    MerkmalgruppenID = merkmalgruppe.ID,
                                    MerkmalID = merkmal.ID,
                                    OptionCode = option.Code
                                });
                            }
                        }
                        foreach (InExcluder includer in option.Includers)
                        {
                            foreach (InExcludeSelection selection in includer.Selection)
                            {
                                GetMerkmal(selection)?.PointersToOptions.Add(new Merkmal.InExcludePointer
                                {
                                    MerkmalgruppenID = merkmalgruppe.ID,
                                    MerkmalID = merkmal.ID,
                                    OptionCode = option.Code
                                });
                            }
                        }
                    }
                }
            }
        }
        #endregion

        public string ChecklistCode { get; set; }
        public string Produktgruppe { get; set; }
        public DateTime Datum { get; set; }
        public List<Merkmalgruppe> Merkmalgruppen { get; set; }
        public Checkliste()
        {

        }

        public Merkmalgruppe GetMerkmalgruppeByID(string merkmalgruppeid)
        {
            return Merkmalgruppen.FirstOrDefault(m => m.ID == merkmalgruppeid);
        }
        public Merkmal GetMerkmal(string merkmalgruppeid, string merkmalid)
        {
            return GetMerkmalgruppeByID(merkmalgruppeid)?.Merkmale.FirstOrDefault(m => m.ID == merkmalid);
        }
        public Merkmal GetMerkmal (InExcludeSelection selection)
        {
            return GetMerkmal(selection.MerkmalgruppeID, selection.MerkmalID);
        }
        public Merkmal GetMerkmal(Merkmal.InExcludePointer pointer)
        {
            return GetMerkmal(pointer.MerkmalgruppenID, pointer.MerkmalID);
        }

        public Option GetOption (string merkmalgruppeid, string merkmalid, string optioncode)
        {
            return GetMerkmal(merkmalgruppeid, merkmalid)?.Optionen.FirstOrDefault(c => c.Code == optioncode);
        }
        public Option GetOption(InExcludeSelection selection)
        {
            return GetOption(selection.MerkmalgruppeID, selection.MerkmalID, selection.OptionCode);
        }
         public Option GetOption(Merkmal.InExcludePointer pointertooption)
        {
            return GetOption(pointertooption.MerkmalgruppenID, pointertooption.MerkmalID, pointertooption.OptionCode);
        }

        /// <summary>
        /// enabled oder disabled alle Merkmale gemäß gesetztem wert des übergebenen Merkmals für die verbundenen Merkmale
        /// </summary>
        /// <param name="merkmal"></param>
        public void EnableDisableOptions(Merkmal merkmal)
        {
            // überprüfen, ob die Auswahl ein anderes Merkmal excludiert oder includiert und in den entsprechenden Optionen enablen oder disablen
            foreach (Merkmal.InExcludePointer pointer in merkmal.PointersToOptions)
            {
                EnableDisableMerkmale(pointer);
                EnableDisableOptions(pointer);
            }
        }
        public void EnableDisableMerkmale(Merkmal.InExcludePointer pointertooption)
        {
            Merkmal merkmal, selectedmerkmal;
            bool ischecked = true;

            // komplettes Merkmal wir ex- oder includiert
            merkmal = GetMerkmal(pointertooption);
            if (merkmal != null)
            {
                foreach (InExcluder excluder in merkmal.Excluders)
                {
                    foreach (InExcludeSelection selection in excluder.Selection)
                    {
                        selectedmerkmal = GetMerkmal(selection);
                        ischecked = selectedmerkmal.SelectedOption != null && selectedmerkmal.SelectedOption.Code == selection.OptionCode;
                        if (ischecked == false)
                            break;
                    }
                    if (ischecked)
                        break;
                }
                foreach (InExcluder includer in merkmal.Includers)
                {
                    foreach (InExcludeSelection selection in includer.Selection)
                    {
                        selectedmerkmal = GetMerkmal(selection);
                        ischecked = selectedmerkmal.SelectedOption != null && selectedmerkmal.SelectedOption.Code != selection.OptionCode;
                        if (ischecked == false)
                            break;
                    }
                    if (ischecked)
                        break;
                }
                merkmal.Enabled = !ischecked;
            }

        }
        public void EnableDisableOptions (Merkmal.InExcludePointer pointertooption)
        {
            Merkmal merkmal;
            Option option;
            bool ischecked = true;

           // einzelne Optionen eines Merkmals sind ex- oder includiert
            if (pointertooption.OptionDefiniert)
            {
                option = GetOption(pointertooption);
                if (option != null)
                {
                    foreach (InExcluder excluder in option.Excluders)
                    {
                        foreach (InExcludeSelection selection in excluder.Selection)
                        {
                            merkmal = GetMerkmal(selection);
                            ischecked = merkmal.SelectedOption != null && merkmal.SelectedOption.Code == selection.OptionCode;
                            if (ischecked == false)
                                break;
                        }
                        if (ischecked)
                            break;
                    }
                    foreach (InExcluder includer in option.Includers)
                    {
                        foreach (InExcludeSelection selection in includer.Selection)
                        {
                            merkmal = GetMerkmal(selection);
                            ischecked = merkmal.SelectedOption != null && merkmal.SelectedOption.Code != selection.OptionCode;
                            if (ischecked == false)
                                break;
                        }
                        if (ischecked)
                            break;
                    }
                }
                option.Enabled = !ischecked;
            }

        }

        public decimal GetAngebotspreis()
        {
            decimal r = 0;

            if (Merkmalgruppen != null)
            {
                foreach (Merkmalgruppe merkmalgruppe in Merkmalgruppen)
                {
                    r += merkmalgruppe.Merkmale.Sum(s => s.Angebotspreis);
                }
            }
            return r;
        }

        public bool KonfigurationVollstaendig()
        {
            int z = 0;
            if (Merkmalgruppen != null)
            {
                foreach (Merkmalgruppe merkmalgruppe in Merkmalgruppen)
                {
                    z = merkmalgruppe.Merkmale.Where(d => !d.AlleOptionenDeaktiviert).Count(s => !s.FeldinhaltGefuellt);
                    if (z > 0) break;
                }
            }
            return z == 0;
        }

        public class Merkmalgruppe
        {
            public string ID { get; set; }
            public string Text { get; set; }
            public List<Merkmal> Merkmale { get; set; } = new List<Merkmal>();

            public Merkmalgruppe()
            {

            }
        }
        public class Merkmal
        {
            public string ID { get; set; }
            public string Text { get; set; }
            public string Typ { get; set; }
            public bool Enabled { get; set; } = true;
            public string Feldinhalt { get; set; }
            public decimal PreisProEinheit { get; set; } 
            public decimal Angebotspreis => GetAngebotspreis();
            public string MerkmalgruppeID { get; set; }
            public List<Option> Optionen { get; set; } = new List<Option>();
            public Option SelectedOption => FeldinhaltGefuellt ? Optionen.FirstOrDefault(c => c.Code == Feldinhalt) : new Option();
            public bool AlleOptionenDeaktiviert => Optionen.Count(c => c.Enabled == true) == 0;
            public bool IsOptionSelected => SelectedOption != null ? SelectedOption.Exists : false;
            public bool FeldinhaltGefuellt => Feldinhalt != null && Feldinhalt.Length > 0;
            public List<InExcludePointer> PointersToOptions { get; set; } = new List<InExcludePointer>();
            public List<Pointer> PointersToConnectors { get; set; } = new List<Pointer>();
            public List<InExcluder> Excluders { get; set; } = new List<InExcluder>();
            public List<InExcluder> Includers { get; set; } = new List<InExcluder>();

            public Merkmal()
            {

            }

            private decimal GetAngebotspreis()
            {
                decimal r = 0;
                switch(Typ)
                {
                    case "Option":
                        r = SelectedOption.Exists ? SelectedOption.Preis : 0;
                        break;
                    case "Decimal":
                        Decimal.TryParse(Feldinhalt, out decimal menge);
                        r = menge * PreisProEinheit;
                        break;
                }
                return r;
            }

            public class Pointer
            {
                public string MerkmalgruppenID { get; set; }
                public string MerkmalID { get; set; }
            }
            public class InExcludePointer : Pointer
            {
                public string OptionCode { get; set; }

                public bool OptionDefiniert => OptionCode != null &&  OptionCode.Length > 0;
            }

            #region Events

            public delegate void OptionChangedEventHandler(object sender, OptionEventArgs e);
            public class OptionEventArgs
            {
            }

            public OptionChangedEventHandler OptionChangedEvent;
            public void InvokeOptionChangedEvent()
            {
                OptionChangedEvent?.Invoke(this, new OptionEventArgs());
            }

            public delegate void DecimalChangedEventHandler(object sender, DecimalEventArgs e);
            public class DecimalEventArgs
            {
                public DecimalEventArgs (decimal value)
                {
                    Value = value;
                }
                public decimal Value { get; set; }
            }

            public DecimalChangedEventHandler DecimalChangedEvent;
            public void InvokeDecimalChangedEvent(decimal value)
            {
                DecimalChangedEvent?.Invoke(this, new DecimalEventArgs(value));
            }

            #endregion

            public string[] GetOptionenCodes()
            {
                List<string> r = new List<string>();
                foreach (Option option in Optionen)
                {
                    r.Add(option.Code);
                }
                return r.ToArray();
            }

            public Option GetOption (string optioncode)
            {
                return Optionen.FirstOrDefault(c => c.Code == optioncode);
            }
        }
        public class Option
        {
            public string Code { get; set; }
            public string Text { get; set; }
            public bool Enabled { get; set; } = true;
            public decimal Preis { get; set; }
            public string MerkmalID { get; set; }
            public string MerkmalgruppeID { get; set; }
            public List<InExcluder> Excluders { get; set; } = new List<InExcluder>();
            public List<InExcluder> Includers { get; set; } = new List<InExcluder>();

            public bool Exists => Code != null;
            public Option()
            {

            }

         }
        public class InExcluder
        {
            public List<InExcludeSelection> Selection { get; set; } = new List<InExcludeSelection>();
            public InExcluder()
            {
            }
        }
        public class InExcludeSelection
        {
            public string MerkmalgruppeID { get; set; }
            public string MerkmalID { get; set; }
            public string OptionCode { get; set; }

            public InExcludeSelection()
            {

            }
        }
        public class Bedingung
        {


        }
    }

}
