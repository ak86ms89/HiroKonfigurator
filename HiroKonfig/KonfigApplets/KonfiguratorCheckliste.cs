using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Server;
using static HiroKonfig.KonfiguratorCheckliste.ZahlenFormel;
using static HiroKonfig.KonfiguratorCheckliste;
using static HiroKonfig.Checkliste;


namespace HiroKonfig
{
    public class KonfiguratorCheckliste
    {
        public interface I_Vergleich
        {
            Vergleichsoperatoren Operator { get; set; }
            string Operatorname { get; }
            decimal Zahl { get; set; }

        }

        public interface I_Pointer
        {
            string MerkmalgruppeID { get; set; }
            string MerkmalID { get; set; }
        }

        public enum Pruefaktionen { keine, Hinweis, Warnung, Stop } // verschiedene Prüfmerkmaltypen, kein => Mwerkmal ist keinn PrüfmerkmaL
        public enum Sichtbarkeiten { immer, nie, nurwennenabled }
        public string ChecklistCode { get; set; }
        public string Produktgruppe { get; set; }
        public decimal Angebotspreis => GetAngebotspreis();
        public DateTime Datum { get; set; }
        public List<ZahlenFormel> Formelliste { get; set; }
        public List<Merkmalgruppe> Merkmalgruppen { get; set; }

        public KonfiguratorCheckliste()
        {

        }

        public string GetJsonStream()
        {
            RemoveEmptyExcluders();
            return System.Text.Json.JsonSerializer.Serialize(this);
        }

        #region Pointer
        public void ClearPointers()
        {
            foreach (Merkmalgruppe merkmalgruppe in Merkmalgruppen)
            {
                merkmalgruppe.ClearPointers();
            }
        }
        public void CreatePointers()
        {
            ClearPointers();
            CreateOptionpointers();
        }
        public void CreateOptionpointers()
        {
            Merkmal mrkml;

            foreach (Merkmalgruppe merkmalgruppe in Merkmalgruppen)
            {
                foreach (Merkmal merkmal in merkmalgruppe.Merkmale)
                {
                    // Ex- und Includers pro Merkmal
                    foreach (InExcluder excluder in merkmal.Excluders)
                    {
                        foreach (InExcludeOption selection in excluder.Selection)
                        {
                            GetMerkmal(selection)?.PointersToOptions.Add(new InExcludeOption
                            {
                                MerkmalgruppeID = merkmalgruppe.ID,
                                MerkmalID = merkmal.MerkmalID,
                                OptionCode = ""
                            });
                        }
                    }
                    foreach (InExcluder includer in merkmal.Includers)
                    {
                        foreach (InExcludeOption selection in includer.Selection)
                        {
                            GetMerkmal(selection)?.PointersToOptions.Add(new InExcludeOption
                            {
                                MerkmalgruppeID = merkmalgruppe.ID,
                                MerkmalID = merkmal.MerkmalID,
                                OptionCode = ""
                            });
                        }
                    }

                    // Ex- und Includers pro Option
                    foreach (Option option in merkmal.Optionen)
                    {
                        foreach (InExcluder excluder in option.Excluders)
                        {
                            foreach (InExcludeOption selection in excluder.Selection)
                            {
                                GetMerkmal(selection)?.PointersToOptions.Add(new InExcludeOption
                                {
                                    MerkmalgruppeID = merkmalgruppe.ID,
                                    MerkmalID = merkmal.MerkmalID,
                                    OptionCode = option.Code
                                });
                            }

                            foreach (ZahlVergleich vergleich in excluder.Zahlvergleiche)
                            {
                                GetMerkmal(vergleich)?.PointersToOptions.Add(new InExcludeOption
                                {
                                    MerkmalgruppeID = merkmalgruppe.ID,
                                    MerkmalID = merkmal.MerkmalID,
                                    OptionCode = option.Code
                                });
                            }

                            foreach (FormelVergleich formelVergleich in excluder.Formelvergleiche)
                            {
                                List<Merkmal> merkmallist = GetMerkmale(formelVergleich);
                                foreach (Merkmal mrkmal in merkmallist)
                                {
                                    mrkmal?.PointersToOptions.Add(new InExcludeOption
                                    {
                                        MerkmalgruppeID = merkmalgruppe.ID,
                                        MerkmalID = merkmal.MerkmalID,
                                        OptionCode = option.Code
                                    });
                                }
                            }
                        }
                        foreach (InExcluder includer in option.Includers)
                        {
                            foreach (InExcludeOption selection in includer.Selection)
                            {
                                GetMerkmal(selection)?.PointersToOptions.Add(new InExcludeOption
                                {
                                    MerkmalgruppeID = merkmalgruppe.ID,
                                    MerkmalID = merkmal.MerkmalID,
                                    OptionCode = option.Code
                                });
                            }

                            foreach (ZahlVergleich vergleich in includer.Zahlvergleiche)
                            {
                                GetMerkmal(vergleich)?.PointersToOptions.Add(new InExcludeOption
                                {
                                    MerkmalgruppeID = merkmalgruppe.ID,
                                    MerkmalID = merkmal.MerkmalID,
                                    OptionCode = option.Code
                                });
                            }

                            foreach (FormelVergleich formelVergleich in includer.Formelvergleiche)
                            {
                                List<Merkmal> merkmallist = GetMerkmale(formelVergleich);
                                foreach (Merkmal mrkmal in merkmallist)
                                {
                                    mrkmal?.PointersToOptions.Add(new InExcludeOption
                                    {
                                        MerkmalgruppeID = merkmalgruppe.ID,
                                        MerkmalID = merkmal.MerkmalID,
                                        OptionCode = option.Code
                                    });
                                }
                            }

                        }

                        foreach (Price preis in option.Preise)
                        {
                            foreach (InExcluder excluder in preis.Excluders)
                            {
                                foreach (InExcludeOption selection in excluder.Selection)
                                {
                                    GetMerkmal(selection)?.PointersToOptionPrice.Add(new InExcludeOption
                                    {
                                        MerkmalgruppeID = merkmalgruppe.ID,
                                        MerkmalID = merkmal.MerkmalID,
                                        OptionCode = option.Code
                                    });
                                }

                                foreach (ZahlVergleich vergleich in excluder.Zahlvergleiche)
                                {
                                    GetMerkmal(vergleich)?.PointersToOptions.Add(new InExcludeOption
                                    {
                                        MerkmalgruppeID = merkmalgruppe.ID,
                                        MerkmalID = merkmal.MerkmalID,
                                        OptionCode = option.Code
                                    });
                                }

                                foreach (FormelVergleich formelVergleich in excluder.Formelvergleiche)
                                {
                                    List<Merkmal> merkmallist = GetMerkmale(formelVergleich);
                                    foreach (Merkmal mrkmal in merkmallist)
                                    {
                                        mrkmal?.PointersToOptions.Add(new InExcludeOption
                                        {
                                            MerkmalgruppeID = merkmalgruppe.ID,
                                            MerkmalID = merkmal.MerkmalID,
                                            OptionCode = option.Code
                                        });
                                    }
                                }

                            }

                            foreach (InExcluder includer in preis.Includers)
                            {
                                foreach (InExcludeOption selection in includer.Selection)
                                {
                                    GetMerkmal(selection)?.PointersToOptionPrice.Add(new InExcludeOption
                                    {
                                        MerkmalgruppeID = merkmalgruppe.ID,
                                        MerkmalID = merkmal.MerkmalID,
                                        OptionCode = option.Code
                                    });
                                }

                                foreach (ZahlVergleich vergleich in includer.Zahlvergleiche)
                                {
                                    GetMerkmal(vergleich)?.PointersToOptions.Add(new InExcludeOption
                                    {
                                        MerkmalgruppeID = merkmalgruppe.ID,
                                        MerkmalID = merkmal.MerkmalID,
                                        OptionCode = option.Code
                                    });
                                }

                                foreach (FormelVergleich formelVergleich in includer.Formelvergleiche)
                                {
                                    List<Merkmal> merkmallist = GetMerkmale(formelVergleich);
                                    foreach (Merkmal mrkmal in merkmallist)
                                    {
                                        mrkmal?.PointersToOptions.Add(new InExcludeOption
                                        {
                                            MerkmalgruppeID = merkmalgruppe.ID,
                                            MerkmalID = merkmal.MerkmalID,
                                            OptionCode = option.Code
                                        });
                                    }
                                }
                            }

                        }
                    }

                    // Formeln, die einen Merkmalwert definieren und berechnen
                    foreach (Formel formel in merkmal.Formeln)
                    {
                        List<Merkmal> merkmallist = GetMerkmale(formel);
                        foreach (Merkmal mrkmal in merkmallist)
                        {
                            mrkmal?.PointersToFormel.Add(new Formel
                            {
                                MerkmalgruppeID = merkmalgruppe.ID,
                                MerkmalID = merkmal.MerkmalID,
                                Formelname = formel.Formelname
                            });
                        }
                    }

                    // Formelvergleiche, wenn Merkmal nur zur Prüfung dient
                    foreach (FormelVergleich formelVergleich in merkmal.Formelvergleiche)
                    {
                        List<Merkmal> merkmallist = GetMerkmale(formelVergleich);
                        foreach (Merkmal mrkmal in merkmallist)
                        {
                            mrkmal?.PointersToFormelvergleiche.Add(new FormelVergleich
                            {
                                MerkmalgruppeID = merkmalgruppe.ID,
                                MerkmalID = merkmal.MerkmalID,
                                Formelname = formelVergleich.Formelname
                            });
                        }
                    }

                    // Multiplikator der Menge eines Merkmals zur Ermittlung der Menge eines anderen Merkmals
                    foreach (Multiplikator multiplikator in merkmal.Multiplikatoren)
                    {
                        GetMerkmal(multiplikator.RelatedMerkmalgruppe, multiplikator.RelatedMerkmal)?.PointersToMultiplikator.Add(new Multiplikator
                        {
                            MerkmalgruppeID = merkmalgruppe.ID,
                            MerkmalID = merkmal.MerkmalID,
                            Faktor = multiplikator.Faktor
                        });
                    }

                    // Mengenlimits
                    foreach (Mengenlimiter mengenlimiter in merkmal.Mengenlimits)
                    {
                        GetMerkmal(mengenlimiter.RelatedMerkmalgruppe, mengenlimiter.RelatedMerkmal)?.PointersToMengenlimiter.Add(new Mengenlimiter
                        {
                            MerkmalgruppeID = merkmalgruppe.ID,
                            MerkmalID = merkmal.MerkmalID
                        });

                        // Verbindungen über Formel
                        List<Merkmal> merkmallist = GetMerkmaleByFormel(mengenlimiter.Formel);
                        foreach (Merkmal mrkmal in merkmallist)
                        {
                            mrkmal?.PointersToMengenlimiter.Add(new Mengenlimiter
                            {
                                MerkmalgruppeID = merkmalgruppe.ID,
                                MerkmalID = merkmal.MerkmalID,
                                Formel = mengenlimiter.Formel
                            });
                        }
                        foreach (InExcluder excluder in mengenlimiter.Excluders)
                        {
                            foreach (InExcludeOption selection in excluder.Selection)
                            {
                                mrkml = GetMerkmal(selection);
                                if (mrkml != null && mrkml.PointersToMengenlimiter.
                                    FirstOrDefault(s => s.MerkmalgruppeID == merkmalgruppe.ID && s.MerkmalID == merkmal.MerkmalID) == null)
                                {
                                    mrkml.PointersToMengenlimiter.Add(new Mengenlimiter
                                    {
                                        MerkmalgruppeID = merkmalgruppe.ID,
                                        MerkmalID = merkmal.MerkmalID
                                    });
                                }
                            }
                        }
                        foreach (InExcluder includer in mengenlimiter.Includers)
                        {
                            foreach (InExcludeOption selection in includer.Selection)
                            {
                                mrkml = GetMerkmal(selection);
                                if (mrkml != null && mrkml.PointersToMengenlimiter.
                                    FirstOrDefault(s => s.MerkmalgruppeID == merkmalgruppe.ID && s.MerkmalID == merkmal.MerkmalID) == null)
                                {
                                    mrkml.PointersToMengenlimiter.Add(new Mengenlimiter
                                    {
                                        MerkmalgruppeID = merkmalgruppe.ID,
                                        MerkmalID = merkmal.MerkmalID
                                    });
                                }
                            }
                        }
                    }

                    // Preis

                }
            }
        }
        public void CreateConnectionpointers()
        {
            foreach (Merkmalgruppe merkmalgruppe in Merkmalgruppen)
            {
                Merkmal mrkml;

                foreach (Merkmal merkmal in merkmalgruppe.Merkmale)
                {
                    foreach (Multiplikator multiplikator in merkmal.Multiplikatoren)
                    {
                        GetMerkmal(multiplikator.RelatedMerkmalgruppe, multiplikator.RelatedMerkmal)?.PointersToMultiplikator.Add(new Multiplikator
                        {
                            MerkmalgruppeID = merkmalgruppe.ID,
                            MerkmalID = merkmal.MerkmalID,
                            Faktor = multiplikator.Faktor
                        });
                    }

                    foreach (Mengenlimiter mengenlimiter in merkmal.Mengenlimits)
                    {
                        GetMerkmal(mengenlimiter.RelatedMerkmalgruppe, mengenlimiter.RelatedMerkmal)?.PointersToMengenlimiter.Add(new Mengenlimiter
                        {
                            MerkmalgruppeID = merkmalgruppe.ID,
                            MerkmalID = merkmal.MerkmalID
                        });

                        // Verbindungen über Formel
                        List<Merkmal> merkmallist = GetMerkmaleByFormel(mengenlimiter.Formel);
                        foreach (Merkmal mrkmal in merkmallist)
                        {
                            mrkmal?.PointersToMengenlimiter.Add(new Mengenlimiter
                            {
                                MerkmalgruppeID = merkmalgruppe.ID,
                                MerkmalID = merkmal.MerkmalID
                            });
                        }
                        foreach (InExcluder excluder in mengenlimiter.Excluders)
                        {
                            foreach (InExcludeOption selection in excluder.Selection)
                            {
                                mrkml = GetMerkmal(selection);
                                if (mrkml != null && mrkml.PointersToMengenlimiter.
                                    FirstOrDefault(s => s.MerkmalgruppeID == merkmalgruppe.ID && s.MerkmalID == merkmal.MerkmalID) == null)
                                {
                                    mrkml.PointersToMengenlimiter.Add(new Mengenlimiter
                                    {
                                        MerkmalgruppeID = merkmalgruppe.ID,
                                        MerkmalID = merkmal.MerkmalID
                                    });
                                }
                            }
                        }
                        foreach (InExcluder includer in mengenlimiter.Includers)
                        {
                            foreach (InExcludeOption selection in includer.Selection)
                            {
                                mrkml = GetMerkmal(selection);
                                if (mrkml != null && mrkml.PointersToMengenlimiter.
                                    FirstOrDefault(s => s.MerkmalgruppeID == merkmalgruppe.ID && s.MerkmalID == merkmal.MerkmalID) == null)
                                {
                                    mrkml.PointersToMengenlimiter.Add(new Mengenlimiter
                                    {
                                        MerkmalgruppeID = merkmalgruppe.ID,
                                        MerkmalID = merkmal.MerkmalID
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Formeln
        public class ZahlenFormel
        {
            public string Bezeichnung { get; set; } // Key
            public List<Term> Terme { get; set; } = new List<Term>();

            public ZahlenFormel()
            {

            }

            public void AddEmptyTerm(Term.Typen typ)
            {
                Terme.Add(new Term { Typ = typ });
            }

            public class Term : Pointer
            {
                public enum Typen { Merkmalbezug, Fixwert }
                public enum Operatoren { Addition, Subtraktion, Multiplikation, Division }
                public Operatoren Operator { get; set; }
                public Typen Typ { get; set; }
                public decimal Festwert { get; set; } // fester Wert ohne Merkmalsbezug

                public Term()
                {

                }

            }
        }

        private ZahlenFormel GetZahlenformelByName(string formelname)
        {
            return Formelliste.FirstOrDefault(f => f.Bezeichnung == formelname);
        }
        // gibt das Ergebnis der Formel zurück, gemäß der aktuell gesetzten Merkmalwerte
        private decimal Formelergebnis(string formelname)
        {
            return Formelergebnis(GetZahlenformelByName(formelname));
        }
        private decimal Formelergebnis(ZahlenFormel formel)
        {
            decimal result = 0;
            decimal termwert;
            Term trm;
            Merkmal merkmal;


            for (int i = 0; i < formel.Terme.Count; i++)
            {
                trm = formel.Terme[i];

                if (trm.Typ == Term.Typen.Fixwert)
                {
                    termwert = trm.Festwert;
                }
                else
                {
                    merkmal = GetMerkmal(trm.MerkmalgruppeID, trm.MerkmalID);
                    termwert = merkmal.FeldinhaltWert;
                }

                switch (trm.Operator)
                {
                    case Term.Operatoren.Addition:
                        result += termwert;
                        break;
                    case Term.Operatoren.Subtraktion:
                        result -= termwert;
                        break;
                    case Term.Operatoren.Multiplikation:
                        result *= termwert;
                        break;
                    case Term.Operatoren.Division:
                        result /= termwert;
                        break;
                }
            }

            return result;
        }

        private List<Merkmal> GetMerkmaleByFormel(string formelname)
        {
            List<Merkmal> r = new List<Merkmal>();
            Merkmal mrkmal;
            ZahlenFormel formel = Formelliste?.FirstOrDefault(f => f.Bezeichnung == formelname);

            if (formel != null)
            {
                foreach (Term term in formel.Terme)
                {
                    mrkmal = GetMerkmal(term);
                    if (mrkmal != null)
                    {
                        r.Add(mrkmal);
                    }
                }
            }

            return r;
        }
        public List<Merkmal> GetAllFormelMerkmale()
        {
            List<Merkmal> r = new();
            foreach (Merkmalgruppe merkmalgruppe in Merkmalgruppen)
            {
                r.AddRange(merkmalgruppe.Merkmale.Where(f => f.IsFormelCase));
            }
            return r;
        }
        /// <summary>
        /// berechnet für alle Merkmale, die als Typ eine Formel sind, die Werte neu. es wird die erste Formel genommen aus der Formelliste
        /// </summary>
        public void CalculateAllFormeln()
        {
            List<Merkmal> mrkmale = GetAllFormelMerkmale();
            Formel formel;
            foreach (Merkmal mrkml in mrkmale)
            {
                formel = mrkml.Formeln.FirstOrDefault();
                if (formel != null)
                    mrkml.Feldinhalt = Formelergebnis(formel.Formelname).ToString();
            }
        }
        #endregion

        #region Merkmal
        public Merkmalgruppe GetMerkmalgruppeByID(string merkmalgruppeid)
        {
            return Merkmalgruppen.FirstOrDefault(m => m.ID == merkmalgruppeid);
        }
        public Merkmalgruppe AddMerkmalgruppe(string merkmalgruppeid, string bezeichnung)
        {
            Merkmalgruppe merkmalgruppe = new Merkmalgruppe { ID = merkmalgruppeid, Text = bezeichnung };
            Merkmalgruppen.Add(merkmalgruppe);
            return merkmalgruppe;
        }

        public void RemoveMerkmalgruppe(Merkmalgruppe merkmalgruppe)
        {
            Merkmalgruppen.Remove(merkmalgruppe);
        }
        public Merkmal GetMerkmal(string merkmalgruppeid, string merkmalid)
        {
            return GetMerkmalgruppeByID(merkmalgruppeid)?.Merkmale.FirstOrDefault(m => m.MerkmalID == merkmalid);
        }
        /// <summary>
        /// gibt das erste Merkmal für die merkmalid zurück, dass gefunden wird
        /// </summary>
        /// <param name="merkmalid"></param>
        /// <returns></returns>
        public Merkmal GetFirstMerkmal(string merkmalid)
        {
            Merkmal mrkml = null;
            foreach (Merkmalgruppe merkmalgruppe in Merkmalgruppen)
            {
                mrkml = merkmalgruppe.Merkmale.FirstOrDefault(m => m.MerkmalID == merkmalid);
                if (mrkml != null) break;
            }
            return mrkml;
        }
        public Merkmal GetMerkmal(Pointer pointer)
        {
            return GetMerkmal(pointer.MerkmalgruppeID, pointer.MerkmalID);
        }
        public List<Merkmal> GetMerkmale(Formel formel)
        {
            return GetMerkmaleByFormel(formel.Formelname);
        }
        public List<Merkmal> GetMerkmale(FormelVergleich formelVergleich)
        {
            return GetMerkmaleByFormel(formelVergleich.Formelname);
        }
        public void InsertMerkmal(Merkmal movedmerkmal, Merkmal targetmerkmal)
        {
            Merkmalgruppe targetgroup = GetMerkmalgruppeByID(targetmerkmal.MerkmalgruppeID);
            /*
            if (movedmerkmal.MerkmalgruppeID != targetmerkmal.MerkmalgruppeID)
                UpdateInExcludes(movedmerkmal, targetmerkmal.MerkmalgruppeID); // wenn das Merkmal die Gruppe wechselt, müssen die in anderen Merkmalen zugeordneten InExcludes mit dem neuen MerkmalgruppenID aktualisiert werden
            */
            targetgroup.InsertMerkmal(movedmerkmal, targetmerkmal);

        }

        public List<Merkmal> GetAllMerkmale()
        {
            List<Merkmal> r = new();
            foreach (Merkmalgruppe merkmalgruppe in Merkmalgruppen)
            {
                r.AddRange(merkmalgruppe.Merkmale);
            }
            return r;
        }

        public Dictionary<string, string> GetAllMerkmaltexte()
        {
            Dictionary<string, string> r = new Dictionary<string, string>();
            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (Merkmalgruppe merkmalgruppe in Merkmalgruppen)
            {
                list = merkmalgruppe.Merkmale.ToDictionary(s => s.MerkmalID, s => s.Text);
                r = r.Union(list).ToDictionary(kvp1 => kvp1.Key, kvp1 => kvp1.Value);
            }
            return r;
        }

        public Dictionary<string, string> GetAllMerkmalwerte()
        {
            Dictionary<string, string> r = new Dictionary<string, string>();
            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (Merkmalgruppe merkmalgruppe in Merkmalgruppen)
            {
                list = merkmalgruppe.Merkmale.ToDictionary(s => s.MerkmalID, s => s.Feldinhalt);
                r = r.Union(list).ToDictionary(kvp1 => kvp1.Key, kvp1 => kvp1.Value);
            }
            return r;
        }

        public Dictionary<string, string> GetAllMerkmalinhalte()
        {
            Dictionary<string, string> r = new Dictionary<string, string>();
            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (Merkmalgruppe merkmalgruppe in Merkmalgruppen)
            {
                // Zahlen + Texte
                list = merkmalgruppe.Merkmale.Where(m => !m.IsOptionCase).ToDictionary(s => s.MerkmalID, s => s.Feldinhalt);
                r = r.Union(list).ToDictionary(kvp1 => kvp1.Key, kvp1 => kvp1.Value);
                // Optionen
                list = merkmalgruppe.Merkmale.Where(m => m.IsOptionCase && m.IsOptionSelected).ToDictionary(s => s.MerkmalID, s => s.SelectedOption.Bezeichnung);
                r = r.Union(list).ToDictionary(kvp1 => kvp1.Key, kvp1 => kvp1.Value);
            }
            return r;
        }
        public Dictionary<string, string> GetAllMerkmalpreise()
        {
            Dictionary<string, string> r = new Dictionary<string, string>();
            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (Merkmalgruppe merkmalgruppe in Merkmalgruppen)
            {
                list = merkmalgruppe.Merkmale.ToDictionary(s => s.MerkmalID, s => s.Angebotspreis.ToString("N2"));
                r = r.Union(list).ToDictionary(kvp1 => kvp1.Key, kvp1 => kvp1.Value);
            }
            return r;
        }

        #endregion

        #region Option
        private void UpdateInExcludes(Merkmal movedmerkmal, string newmerkmalgruppenid)
        {
            Merkmal merkmal;
            foreach (InExcludeOption pointer in movedmerkmal.PointersToOptions)
            {
                merkmal = GetMerkmal(pointer);
                foreach (InExcluder inexcluder in merkmal.Includers)
                {
                    UpdateInExcludeSelection(inexcluder, movedmerkmal, newmerkmalgruppenid);
                }
                foreach (InExcluder inexcluder in merkmal.Excluders)
                {
                    UpdateInExcludeSelection(inexcluder, movedmerkmal, newmerkmalgruppenid);
                }

                foreach (Option option in merkmal.Optionen)
                {
                    foreach (InExcluder inexcluder in option.Includers)
                    {
                        UpdateInExcludeSelection(inexcluder, movedmerkmal, newmerkmalgruppenid);
                    }
                    foreach (InExcluder inexcluder in option.Excluders)
                    {
                        UpdateInExcludeSelection(inexcluder, movedmerkmal, newmerkmalgruppenid);
                    }
                }
            }
            foreach (Mengenlimiter pointer in movedmerkmal.PointersToMengenlimiter)
            {
                merkmal = GetMerkmal(pointer);
                foreach (Mengenlimiter mengenlimiter in merkmal.Mengenlimits)
                {
                    if (mengenlimiter.MerkmalgruppeID == movedmerkmal.MerkmalgruppeID && mengenlimiter.MerkmalID == movedmerkmal.MerkmalID)
                        mengenlimiter.MerkmalgruppeID = newmerkmalgruppenid;
                }
            }
            foreach (Multiplikator pointer in movedmerkmal.PointersToMultiplikator)
            {
                merkmal = GetMerkmal(pointer);
                foreach (Multiplikator multiplikator in merkmal.Multiplikatoren)
                {
                    if (multiplikator.MerkmalgruppeID == movedmerkmal.MerkmalgruppeID && multiplikator.MerkmalID == movedmerkmal.MerkmalID)
                        multiplikator.MerkmalgruppeID = newmerkmalgruppenid;
                }
            }
        }
        private void UpdateInExcludeSelection(InExcluder inexcluder, Merkmal movedmerkmal, string newmaterialgruppenid)
        {
            List<InExcludeOption> selection = inexcluder.Selection.Where(s => s.MerkmalgruppeID == movedmerkmal.MerkmalgruppeID && s.MerkmalID == movedmerkmal.MerkmalID).ToList();
            foreach (InExcludeOption sel in selection)
            {
                sel.MerkmalgruppeID = newmaterialgruppenid;
            }
        }
        public Option GetOption(string merkmalgruppeid, string merkmalid, string optioncode)
        {
            return GetMerkmal(merkmalgruppeid, merkmalid)?.Optionen.FirstOrDefault(c => c.Code == optioncode);
        }
        public Option GetOption(InExcludeOption selection)
        {
            return GetOption(selection.MerkmalgruppeID, selection.MerkmalID, selection.OptionCode);
        }
        #endregion

        #region Preise
        public void ValidateAllPrices()
        {
            List<Merkmal> mrkmale = GetAllFormelMerkmale();
            foreach (Merkmal mrkml in mrkmale)
            {
                switch (mrkml.Typ)
                {
                    case Merkmal.Typen.Option:
                        if (mrkml.IsOptionSelected)
                            ValidateOptionPrice(mrkml.SelectedOption);
                        break;
                    default:
                        ValidateMerkmalPrice(mrkml);
                        break;
                }
            }

        }

        #endregion

        #region EnablerDisabler

        /// <summary>
        ///  überprüfen, ob die Auswahl ein anderes Merkmal excludiert oder includiert und in den entsprechenden Optionen enablen oder disablen
        /// </summary>
        /// <param name="merkmal"></param>
        public void EnableDisable(Merkmal merkmal)
        {
            EnableDisableOptions(merkmal);
            EnableDisableOptionPrices(merkmal);
            RunConnectoren(merkmal);
        }
        /// <summary>
        /// enabled oder disabled alle Merkmale gemäß gesetztem wert des übergebenen Merkmals für die verbundenen Merkmale
        /// </summary>
        /// <param name="merkmal"></param>
        public void EnableDisableOptions(Merkmal merkmal)
        {
            // überprüfen, ob die Auswahl ein anderes Merkmal excludiert oder includiert und in den entsprechenden Optionen enablen oder disablen
            foreach (InExcludeOption pointer in merkmal.PointersToOptions)
            {
                EnableDisableMerkmale(pointer);
                EnableDisableOptions(pointer);
                ValidateOptionPrice(pointer);
            }
        }
        /// <summary>
        /// enabled oder disabled alle Merkmale gemäß gesetztem wert des übergebenen Merkmals für die verbundenen Merkmale
        /// </summary>
        /// <param name="merkmal"></param>
        public void EnableDisableOptionPrices(Merkmal merkmal)
        {
            // überprüfen, ob die Auswahl ein anderes Merkmal excludiert oder includiert und in den entsprechenden Optionen enablen oder disablen
            foreach (InExcludeOption pointer in merkmal.PointersToOptionPrice)
            {
                ValidateOptionPrice(pointer);
            }
        }
        public bool EnableDisableMerkmale(InExcludeOption pointertooption)
        {
            Merkmal merkmal;
            bool ischecked;

            merkmal = GetMerkmal(pointertooption);
            if (merkmal == null) return false;
            ischecked = EnableDisableMerkmal(merkmal);

            if (ischecked && merkmal.Typ == Merkmal.Typen.Formel)
            {
                merkmal.Feldinhalt = Formelergebnis(merkmal.Formeln[0].Formelname).ToString();
                ValidateMerkmalPrice(merkmal);
            }

            return ischecked;
        }
        public bool EnableDisableMerkmal(Merkmal merkmal)
        {
            bool ischecked = true;

            // komplettes Merkmal wir ex- oder includiert
            if (merkmal != null)
            {
                ischecked = IncluderCheck(merkmal.Includers);

                if (ischecked)
                {
                    ischecked = ExcluderCheck(merkmal.Excluders);
                }

                if (merkmal.Typ == Merkmal.Typen.Bool)
                {
                    if (ischecked)
                    {
                        merkmal.Feldinhalt = "1";
                    }
                    else
                    {
                        merkmal.Feldinhalt = "0";
                    }
                }
                else
                {
                    merkmal.Enabled = ischecked;
                    if (!merkmal.Enabled)
                        merkmal.RemoveSelectedOption();
                }

            }

            return ischecked;
        }
        public void ValidateMerkmalPrice(Merkmal merkmal)
        {
            bool ischecked = true;
            foreach (Price price in merkmal.Preise)
            {
                ischecked = IncluderCheck(price.Includers);
                if (ischecked)
                {
                    ischecked = ExcluderCheck(price.Excluders);
                }
                price.IsValid = ischecked;
                if (ischecked)
                {
                    break;
                }
            }
        }
        public void EnableDisableOptions(InExcludeOption pointertooption)
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
                    ischecked = IncluderCheck(option.Includers);

                    if (ischecked)
                    {
                        ischecked = ExcluderCheck(option.Excluders);
                    }
                }
                option.Enabled = ischecked;

                merkmal = GetMerkmal(option);
                if (!option.Enabled &&  merkmal.SelectedOption.Exists && merkmal.SelectedOption.Code == option.Code)
                {
                    merkmal.Feldinhalt = "";
                }
            }

        }
        public void ValidateOptionPrice(InExcludeOption pointertooption)
        {
            // Preis höngt gegebenenfalls von gewählten Optionen ab
            if (pointertooption.OptionDefiniert)
            {
                ValidateOptionPrice(GetOption(pointertooption));
            }
        }
        public void ValidateOptionPrice(Option option)
        {
            bool ischecked = true;
            if (option != null)
            {
                foreach (Price price in option.Preise)
                {
                    ischecked = IncluderCheck(price.Includers);
                    if (ischecked)
                    {
                        ischecked = ExcluderCheck(option.Excluders);
                    }

                    price.IsValid = ischecked;
                    if (ischecked)
                    {
                        break;
                    }
                }
            }

        }
        private bool IncluderCheck(List<InExcluder> includerlist)
        {
            Merkmal merkmal;
            bool ischecked = true;

            foreach (InExcluder includer in includerlist)
            {
                foreach (InExcludeOption selection in includer.Selection)
                {
                    merkmal = GetMerkmal(selection);
                    ischecked = merkmal != null && merkmal.SelectedOption != null && merkmal.SelectedOption.Code == selection.OptionCode;
                    if (ischecked == false)
                        break;
                }

                if (ischecked)
                {
                    foreach (ZahlVergleich vergleich in includer.Zahlvergleiche)
                    {
                        merkmal = GetMerkmal(vergleich);

                        ischecked = merkmal != null && merkmal.CheckVergleich(vergleich);
                        if (ischecked == false)
                            break;
                    }

                    if (ischecked)
                    {
                        foreach (FormelVergleich formelvergleich in includer.Formelvergleiche)
                        {
                            ischecked = formelvergleich.CheckVergleich(Formelergebnis(GetZahlenformelByName(formelvergleich.Formelname)));
                            if (ischecked == false)
                                break;
                        }
                    }
                }

                if (ischecked)
                    break;
            }

            return ischecked;
        }
        private bool ExcluderCheck(List<InExcluder> excluderlist)
        {
            Merkmal merkmal;
            bool ischecked = true;

            foreach (InExcluder excluder in excluderlist)
            {
                if (!excluder.IsEmpty)
                {
                    foreach (InExcludeOption selection in excluder.Selection)
                    {
                        merkmal = GetMerkmal(selection);
                        ischecked = merkmal != null && merkmal.SelectedOption != null && merkmal.SelectedOption.Code == selection.OptionCode;
                        if (ischecked == false)
                            break;
                    }
                }

                if (ischecked)
                {
                    foreach (ZahlVergleich vergleich in excluder.Zahlvergleiche)
                    {
                        merkmal = GetMerkmal(vergleich);
                        ischecked = merkmal != null && merkmal.CheckVergleich(vergleich);
                        if (ischecked == false)
                            break;
                    }

                    if (ischecked)
                    {
                        foreach (FormelVergleich formelvergleich in excluder.Formelvergleiche)
                        {
                            ischecked = formelvergleich.CheckVergleich(Formelergebnis(GetZahlenformelByName(formelvergleich.Formelname)));
                            if (ischecked == false)
                                break;
                        }
                    }
                }

                ischecked = !ischecked;
                if (!ischecked)
                    break;
            }

            return ischecked;
        }
        public void RunConnectoren(Merkmal merkmal)
        {
            RunFormelPointer(merkmal);
            RunMultiplikatorPointer(merkmal);
            RunMengenlimiterPointer(merkmal);
            RunMengenlimiter(merkmal);
        }
        /// <summary>
        /// setzt den Feldinhalt eines verbunden Merkmals gemäß der Multiplikatorregel (z.B. Pro 100 cm Fahrbahn wird eine Stütze in den Feldinhalt des Stützenmerkmals geschrieben) 
        /// </summary>
        /// <param name="merkmal"></param>
        public void RunMultiplikatorPointer(Merkmal merkmal)
        {
            Merkmal connectedmerkmal;
            Multiplikator multiplikator;
            if (merkmal.PointersToMultiplikator.Count > 0)
            {
                foreach (Multiplikator connectorpointer in merkmal.PointersToMultiplikator)
                {

                    connectedmerkmal = GetMerkmal(connectorpointer);
                    multiplikator = connectedmerkmal.GetMultiplikator(merkmal.MerkmalgruppeID, merkmal.MerkmalID);
                    Decimal.TryParse(merkmal.Feldinhalt, out decimal menge);
                    multiplikator.ConnectedMenge = menge;
                    connectedmerkmal.Feldinhalt = connectedmerkmal.SumMengeConnectoren().ToString();
                }
            }
        }

        /// <summary>
        /// prüft, ob die verbundenen Merkmale das Mengenlimit erfüllen
        /// </summary>
        /// <param name="merkmal"></param>
        public void RunMengenlimiterPointer(Merkmal merkmal)
        {
            Merkmal connectedmerkmal;
            if (merkmal.PointersToMengenlimiter.Count > 0)
            {
                foreach (Mengenlimiter connectorpointer in merkmal.PointersToMengenlimiter)
                {
                    connectedmerkmal = GetMerkmal(connectorpointer);
                    if (connectedmerkmal != null) RunMengenlimiter(connectedmerkmal, merkmal.IsOptionCase);
                }
            }
        }

        public void RunFormelPointer(Merkmal merkmal)
        {
            Merkmal connectedmerkmal;
            if (merkmal.PointersToFormel.Count > 0)
            {
                foreach (Formel connectorpointer in merkmal.PointersToFormel)
                {
                    connectedmerkmal = GetMerkmal(connectorpointer);
                    if (connectedmerkmal != null && connectedmerkmal.Enabled && connectorpointer.IsEnabled)
                    {
                        RunMerkmalformel(connectedmerkmal, connectorpointer.Formelname);
                    }
                }
            }
        }

        public void RunMerkmalformel(Merkmal merkmal, string formelname)
        {
            merkmal.Feldinhalt = Formelergebnis(formelname).ToString();
            ValidateMerkmalPrice(merkmal);
        }
        /// <summary>
        /// prüft, ob der eingegebene Wert dem mengenmlimit entspricht, dass von einem anderen Merkmal abhängt (z.B. Länge Fahrbahn muss mindestens der Anzahl der Etagen * x cm sein)
        /// </summary>
        /// <param name="merkmal"></param>
        public void RunMengenlimiter(Merkmal merkmal, bool optioncase = false)
        {
            bool ischecked = true;
            Merkmal connectedmerkmal, selectedmerkmal;

            foreach (Mengenlimiter mengenlimiter in merkmal.Mengenlimits)
            {
                if (optioncase == true)
                {
                    foreach (InExcluder includer in mengenlimiter.Includers)
                    {
                        foreach (InExcludeOption selection in includer.Selection)
                        {
                            selectedmerkmal = GetMerkmal(selection);
                            ischecked = selectedmerkmal != null && selectedmerkmal.SelectedOption != null && selectedmerkmal.SelectedOption.Code == selection.OptionCode;
                            if (ischecked == false)
                                break;
                        }
                        if (ischecked)
                            break;
                    }
                    if (ischecked)
                    {
                        foreach (InExcluder excluder in mengenlimiter.Excluders)
                        {
                            foreach (InExcludeOption selection in excluder.Selection)
                            {
                                selectedmerkmal = GetMerkmal(selection);
                                ischecked = selectedmerkmal != null && selectedmerkmal.SelectedOption != null && selectedmerkmal.SelectedOption.Code == selection.OptionCode;
                                if (ischecked == false)
                                    break;
                            }
                            ischecked = !ischecked;
                            if (!ischecked)
                                break;
                        }
                    }


                    mengenlimiter.Enabled = ischecked;
                }
                else if (mengenlimiter.Enabled == true)
                {
                    // Mengenlimit in Verbindung mit anderem Merkmal
                    if (mengenlimiter.ConnectedToMerkmal)
                    {
                        connectedmerkmal = GetMerkmal(mengenlimiter.RelatedMerkmalgruppe, mengenlimiter.RelatedMerkmal);
                        mengenlimiter.Mengelimit = Convert.ToDecimal(connectedmerkmal.Feldinhalt) * mengenlimiter.ProMenge;
                    }
                    else
                    {
                        switch (mengenlimiter.Limitherkunft)
                        {
                            case Mengenlimiter.Limitherkunfte.Formel:
                                mengenlimiter.Mengelimit = Formelergebnis(mengenlimiter.Formel);
                                break;
                            default:
                                mengenlimiter.Mengelimit = mengenlimiter.ProMenge;
                                break;
                        }
                    }
                }
                if (mengenlimiter.Enabled == true)
                {
                    switch (mengenlimiter.Typ)
                    {
                        case Mengenlimiter.Typen.Min:
                            mengenlimiter.Limitbreak = merkmal.Feldinhalt != null && merkmal.Feldinhalt.Trim().Length > 0 ? (Convert.ToDecimal(merkmal.Feldinhalt) >= mengenlimiter.Mengelimit) ? Mengenlimiter.Limitbreaks.OK : Mengenlimiter.Limitbreaks.ZuKlein : Mengenlimiter.Limitbreaks.OK;
                            break;
                        case Mengenlimiter.Typen.Max:
                            mengenlimiter.Limitbreak = merkmal.Feldinhalt != null && merkmal.Feldinhalt.Trim().Length > 0 ? (Convert.ToDecimal(merkmal.Feldinhalt) <= mengenlimiter.Mengelimit) ? Mengenlimiter.Limitbreaks.OK : Mengenlimiter.Limitbreaks.ZuGross : Mengenlimiter.Limitbreaks.OK;
                            break;
                        case Mengenlimiter.Typen.Fix:
                            mengenlimiter.Mengelimit = mengenlimiter.ProMenge;
                            merkmal.Feldinhalt = mengenlimiter.Mengelimit.ToString();
                            mengenlimiter.Limitbreak = Mengenlimiter.Limitbreaks.OK;
                            break;
                        default:
                            mengenlimiter.Limitbreak = Mengenlimiter.Limitbreaks.OK;
                            break;
                    }
                }
            }
        }

        #endregion

        public void Validate()
        {
            CalculateAllFormeln();
            ValidateAllPrices();
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
        private void RemoveEmptyExcluders()
        {
            foreach (Merkmalgruppe merkmalgruppe in Merkmalgruppen)
            {
                foreach (Merkmal merkmal in merkmalgruppe.Merkmale)
                {
                    merkmal.RemoveEmptyExcluders();
                }
            }
        }
        public void SetFeldinhaltByUniqueOptionen()
        {
            foreach (Merkmalgruppe merkmalgruppe in Merkmalgruppen)
            {
                merkmalgruppe.SetFeldinhaltByUniqueOptionen();
            }
        }

        public class Merkmalgruppe
        {
            public string ID { get; set; }
            public string Text { get; set; }
            public List<Merkmal> Merkmale { get; set; } = new List<Merkmal>();

            public Merkmalgruppe()
            {

            }

            public Merkmal AddEmptyMerkmal()
            {
                return AddMerkmal("", "", Merkmal.Typen.Option);
            }
            public Merkmal AddMerkmal(string merkmalid, string bezeichung, Merkmal.Typen typ)
            {
                Merkmal merkmal = new Merkmal { MerkmalID = merkmalid, MerkmalgruppeID = ID, Text = bezeichung, Typ = typ };
                Merkmale.Add(merkmal);
                return merkmal;
            }

            public void InsertMerkmal(Merkmal merkmal, Merkmal beforemerkmal)
            {
                merkmal.MerkmalgruppeID = beforemerkmal.MerkmalgruppeID;
                Merkmale.Insert(Merkmale.IndexOf(beforemerkmal), merkmal);
            }

            public void RemoveMerkmal(Merkmal merkmal)
            {
                Merkmale.Remove(merkmal);
            }

            public void ClearPointers()
            {
                foreach (Merkmal merkmal in Merkmale)
                {
                    merkmal.ClearPointers();
                }
            }

            /// <summary>
            /// setzt für alle Merkmale die UniqueOption, wenn eine solche vorhadnden ist
            /// </summary>
            public void SetFeldinhaltByUniqueOptionen()
            {
                foreach (Merkmal merkmal in Merkmale)
                {
                    if (merkmal.Typ == Merkmal.Typen.Option) merkmal.SetFeldinhaltByUniqueOption();
                }
            }
        }
        public class Pointer : I_Pointer
        {
            public string MerkmalgruppeID { get; set; }
            public string MerkmalID { get; set; }
            public string Hinweis { get; set; }
            public bool HasHinweis => Hinweis != null && Hinweis.Length > 0;
        }
        public class Merkmal : Pointer
        {
            public enum Typen { Option, Zahl, Text, Datum, Bool, Formel, Pruefung }

            private string _artikelnr;
            private string _feldinhalt;

            // public string ID { get; set; }
            public string Text { get; set; }
            public Typen Typ { get; set; }
            public Pruefaktionen Pruefaktion { get; set; }
            public string Artikelnr { get { return GetArtikelnr(); } set { _artikelnr = value; } } // wenn Option, dann Artikelnummer aus Option
            public string Variable { get; set; } // Checklistenvariable Orderbase
            public bool Enabled { get; set; } = true;
            public Sichtbarkeiten Sichtbar { get; set; }
            public bool Drucken { get; set; } = true;
            public string Feldinhalt { get { return Typ == Typen.Option && AnzahlOptionen == 1 ? _feldinhalt = Optionen[0].Code : _feldinhalt; } set { _feldinhalt = value; } }
            public decimal FeldinhaltWert => GetFeldinhaltwert();
            public Price Preis => GetValidPrice();
            public List<Price> Preise { get; set; } = new List<Price>();
            public bool HasPrices => Preise.Count > 0 || Optionen.Where(p => p.HasPrices).Count() > 0;
            public decimal Angebotspreis => GetAngebotspreis();
            // public string MerkmalgruppeID { get; set; }
            public List<Option> Optionen { get; set; } = new List<Option>();
            public List<Multiplikator> Multiplikatoren { get; set; } = new List<Multiplikator>();
            public List<Mengenlimiter> Mengenlimits { get; set; } = new List<Mengenlimiter>();
            public List<Formel> Formeln { get; set; } = new List<Formel>();
            public List<FormelVergleich> Formelvergleiche { get; set; } = new List<FormelVergleich>();
            public List<InExcluder> Excluders { get; set; } = new List<InExcluder>();
            public List<InExcluder> Includers { get; set; } = new List<InExcluder>();
            public Option SelectedOption => FeldinhaltGefuellt ? Optionen.FirstOrDefault(c => c.Code == Feldinhalt) : new Option();
            public List<InExcludeOption> PointersToOptions { get; set; } = new List<InExcludeOption>();
            public List<InExcludeOption> PointersToOptionPrice { get; set; } = new List<InExcludeOption>();
            public List<Multiplikator> PointersToMultiplikator { get; set; } = new List<Multiplikator>();
            public List<Mengenlimiter> PointersToMengenlimiter { get; set; } = new List<Mengenlimiter>();
            public List<Formel> PointersToFormel { get; set; } = new List<Formel>();
            public List<FormelVergleich> PointersToFormelvergleiche { get; set; } = new List<FormelVergleich>();
            public bool AlleOptionenDeaktiviert => Optionen.Count(c => c.Enabled == true) == 0 || Enabled == false;
            public bool IsOptionSelected => SelectedOption != null ? SelectedOption.Exists : false;
            public bool IsOptionCase => Typ == Typen.Option;
            public bool IsZahlCase => Typ == Typen.Zahl;
            public bool IsFormelCase => Typ == Typen.Formel;
            public bool FeldinhaltGefuellt => Feldinhalt != null && Feldinhalt.Length > 0;
            public List<Mengenlimiter> LimitbreaksList => Mengenlimits.FindAll(b => b.Limitbreak != Mengenlimiter.Limitbreaks.OK);
            public bool NoLimitbreaks => LimitbreaksList.Count == 0 || !FeldinhaltGefuellt;
            public bool LimitbreaksExists => LimitbreaksList.Count > 0 && FeldinhaltGefuellt;
            public bool HasStandardOption => Optionen.Any(b => b.IsStandardoption);
            public int AnzahlOptionen => Optionen.Count;
            public bool HasOptionen => Optionen.Count > 0;
            public bool HasMultiplikatoren => Multiplikatoren.Count > 0;
            public bool HasMengenlimits => Mengenlimits.Count > 0;
            public bool HasFormeln => Formeln.Count > 0;
            public bool HasFormelvergleiche => Formelvergleiche.Count > 0;
            public bool HasExcluders => Excluders.Count > 0;
            public bool HasIncluders => Includers.Count > 0;
            public bool HasOptionenPic => Optionen.Where(b => b.HasBild).Count() > 0;
            public Merkmal()
            {

            }
            public InExcluder AddEmtpyExcluder()
            {
                InExcluder exculder = new InExcluder(InExcluder.Typen.Excluder);
                // exculder.AddEmptySelection();
                Excluders.Add(exculder);
                return exculder;
            }
            public InExcluder AddEmptyIncluder()
            {
                InExcluder inculder = new InExcluder(InExcluder.Typen.Includer);
                // inculder.AddEmptySelection();
                Includers.Add(inculder);
                return inculder;
            }
            public Option AddEmptyOption()
            {
                Option option = new Option();
                Optionen.Add(option);
                return option;
            }
            public void RemoveEmptyExcluders()
            {
                List<InExcludeOption> sels;
                List<ZahlVergleich> zvs;
                List<FormelVergleich> fvs;

                foreach (InExcluder inExcluder in Excluders)
                {
                    sels = inExcluder.Selection.FindAll(a => a.MerkmalgruppeID=="" || a.MerkmalID=="");
                    zvs = inExcluder.Zahlvergleiche.FindAll(a => a.MerkmalgruppeID=="" || a.MerkmalID=="");
                    fvs = inExcluder.Formelvergleiche.FindAll(a => a.MerkmalgruppeID=="" || a.MerkmalID=="");
                    foreach (InExcludeOption sel in sels)
                    {
                        inExcluder.Selection.Remove(sel);
                    }
                    foreach (ZahlVergleich zv in zvs)
                    {
                        inExcluder.Zahlvergleiche.Remove(zv);
                    }
                    foreach (FormelVergleich fv in fvs)
                    {
                        inExcluder.Formelvergleiche.Remove(fv);
                    }
                }
            }
            public void RemoveSelectedOption()
            {
                Feldinhalt = "";
            }
            public Price AddPreis(decimal preis)
            {
                Price p = new Price { PreisProEinheit = preis };
                Preise.Add(p);
                return p;
            }

            public void RemovePreis(Price preis)
            {
                Preise.Remove(preis);
            }
            private Price GetValidPrice()
            {

                Price r = Preise.FirstOrDefault(v => v.IsValid);

                return r ?? new Price();
            }

            public void RemoveOption(Option option)
            {
                Optionen.Remove(option);
            }
            public void RemoveMengenlimiter(Mengenlimiter mengenlimiter)
            {
                Mengenlimits.Remove(mengenlimiter);
            }
            public void RemoveMultiplikator(Multiplikator multiplikator)
            {
                Multiplikatoren.Remove(multiplikator);
            }
            public Mengenlimiter AddEmptyMengenlimiter()
            {
                Mengenlimiter mengenlimiter = new Mengenlimiter();
                Mengenlimits.Add(mengenlimiter);
                return mengenlimiter;
            }
            public Multiplikator AddEmptyMultiplikator()
            {
                Multiplikator multiplikator = new Multiplikator();
                Multiplikatoren.Add(multiplikator);
                return multiplikator;
            }
            public void ClearPointers()
            {
                PointersToMengenlimiter.Clear();
                PointersToMultiplikator.Clear();
                PointersToOptionPrice.Clear();
                PointersToOptions.Clear();
                PointersToFormelvergleiche.Clear();
                PointersToFormel.Clear();
            }

            private string GetArtikelnr()
            {
                string r = _artikelnr;

                switch (Typ)
                {
                    case Merkmal.Typen.Option:
                        if (IsOptionSelected)
                            r = SelectedOption.Artikelnr;
                        break;
                    case Merkmal.Typen.Zahl:
                        if (Preis.Artikelnr.Length > 0)
                            r = Preis.Artikelnr;
                        break;
                }
                return r;
            }
            private decimal GetAngebotspreis()
            {
                decimal r = 0;
                switch (Typ)
                {
                    case Merkmal.Typen.Option:
                        r = SelectedOption != null && SelectedOption.Exists
                            ? Math.Round(SelectedOption.Preis.PreisProEinheit, 2)
                            : 0.00m;
                        break;
                    case Merkmal.Typen.Zahl:
                    case Merkmal.Typen.Formel:
                        if (Enabled)
                        {
                            Decimal.TryParse(Feldinhalt, out decimal menge);
                            r = Math.Round(Math.Max(menge - Preis.ImPreisInkludierteMenge, 0) * Preis.PreisProEinheit, 2);
                        }
                        break;
                    case Merkmal.Typen.Bool:
                        r = Feldinhalt == "1" ? Preis.PreisProEinheit : 0.00m;
                        break;
                }
                return r;
            }

            /// <summary>
            /// wenn das Merkmal nur eine Option hat, so wird diese als Feldinhalt gesetzt
            /// </summary>
            public void SetFeldinhaltByUniqueOption()
            {
                if (Optionen.Count == 1) Feldinhalt=Optionen[0].Code;
            }
            private decimal GetFeldinhaltwert()
            {
                return decimal.TryParse(Feldinhalt, out decimal wert) == true ? wert : 0;
            }
            public decimal SumMengeConnectoren()
            {
                return Multiplikatoren.Sum(s => s.Menge);
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
                public DecimalEventArgs(decimal value)
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

            public Option GetOption(string optioncode)
            {
                return Optionen.FirstOrDefault(c => c.Code == optioncode);
            }

            public Option AddOption(string optionscode, string bezeichung, decimal preis)
            {
                Option option = new Option { Code = optionscode, Bezeichnung = bezeichung, MerkmalID = MerkmalID, MerkmalgruppeID = MerkmalgruppeID };
                option.AddPreis(preis);
                Optionen.Add(option);
                return option;
            }

            public Multiplikator GetMultiplikator(string merkmalgruppeid, string merkmalid)
            {
                return Multiplikatoren.FirstOrDefault(c => c.MerkmalgruppeID == merkmalgruppeid && c.MerkmalID == merkmalid);
            }
            public Mengenlimiter GetMengenlimiter(string merkmalgruppeid, string merkmalid)
            {
                return Mengenlimits.FirstOrDefault(c => c.MerkmalgruppeID == merkmalgruppeid && c.MerkmalID == merkmalid);
            }

            /// <summary>
            ///  gibt true zurück, wenn Vergleich erfolgreich
            /// </summary>
            /// <param name="vergleich">Vergleichzahl und Operator</param>
            /// <returns></returns>
            public bool CheckVergleich(ZahlVergleich vergleich)
            {
                bool r = false;
                if (FeldinhaltGefuellt)
                {
                    switch (vergleich.Operator)
                    {
                        case Vergleichsoperatoren.gleich:
                            r =  Convert.ToDecimal(Feldinhalt) == vergleich.Zahl;
                            break;
                        case Vergleichsoperatoren.kleiner:
                            r =  Convert.ToDecimal(Feldinhalt) < vergleich.Zahl;
                            break;
                        case Vergleichsoperatoren.kleinergleich:
                            r =  Convert.ToDecimal(Feldinhalt) <= vergleich.Zahl;
                            break;
                        case Vergleichsoperatoren.groesser:
                            r =  Convert.ToDecimal(Feldinhalt) > vergleich.Zahl;
                            break;
                        case Vergleichsoperatoren.groessergleich:
                            r =  Convert.ToDecimal(Feldinhalt) >= vergleich.Zahl;
                            break;
                        default:
                            r = false;
                            break;
                    }
                }
                return r;
            }

            #region Formel

            public Formel AddEmptyFormel()
            {
                return AddFormel("", "", "");
            }

            public Formel AddFormel(string merkmalgruppeid, string merkmalid, string formelname)
            {
                Formel formel = new Formel { MerkmalgruppeID = merkmalgruppeid, MerkmalID = merkmalid, Formelname = formelname };
                AddFormel(formel);
                return formel;
            }

            public void AddFormel(Formel formel)
            {
                Formeln.Add(formel);
            }

            public void RemoveFormel(Formel formel)
            {
                Formeln.Remove(formel);
            }

            #endregion

            #region Formelvergleich

            public FormelVergleich AddEmptyFormelvergleich()
            {
                return AddFormelvergleich("", "", "", Vergleichsoperatoren.gleich, 0, Pruefaktionen.keine);
            }
            public FormelVergleich AddFormelvergleich(string merkmalgruppeid, string merkmalid, string formelname, Vergleichsoperatoren vergleichsoperator, decimal zahl, Pruefaktionen pruefaktion)
            {
                FormelVergleich vergleich = new FormelVergleich { MerkmalgruppeID = merkmalgruppeid, MerkmalID = merkmalid, Operator=vergleichsoperator, Zahl=zahl, Pruefaktion=pruefaktion, Formelname = formelname };
                Formelvergleiche.Add(vergleich);
                return vergleich;
            }
            public void AddFormelvergleich(FormelVergleich formelvergleich)
            {
                Formelvergleiche.Add(formelvergleich);
            }
            public void RemoveFormelvergleich(FormelVergleich formelvergleich)
            {
                Formelvergleiche.Remove(formelvergleich);
            }
            #endregion

        }
        public class Option : Pointer
        {

            private string _artikelnr;

            public string Code { get; set; }
            public string Bezeichnung { get; set; }
            public string Artikelnr { get { return GetArtikelnr(); } set { _artikelnr = value; } } // wenn Option, dann Artikelnummer aus Option
            public string Langtext { get; set; }
            public string Bild { get; set; }
            public bool Enabled { get; set; } = true;
            public bool IsStandardoption { get; set; } = false;
            public Price Preis => GetValidPrice();
            public List<Price> Preise { get; set; } = new List<Price>();
            public List<InExcluder> Excluders { get; set; } = new List<InExcluder>();
            public List<InExcluder> Includers { get; set; } = new List<InExcluder>();

            public bool HasBild => Bild != null && Bild.Length > 0;
            public bool HasConditionsOrPrice => Preise.Count + Excluders.Count + Includers.Count > 0;
            public bool HasPrices => Preise.Count > 0;
            public bool Exists => Code != null;
            public Option()
            {

            }
            public InExcluder AddEmtpyExcluder()
            {
                InExcluder exculder = new InExcluder(InExcluder.Typen.Excluder);
                // exculder.AddEmptySelection();
                Excluders.Add(exculder);
                return exculder;
            }
            public InExcluder AddEmptyIncluder()
            {
                InExcluder inculder = new InExcluder(InExcluder.Typen.Includer);
                // inculder.AddEmptySelection();
                Includers.Add(inculder);
                return inculder;
            }
            public void RemoveExcluder(InExcluder excluder)
            {
                Excluders.Remove(excluder);
            }
            public void RemoveIncluder(InExcluder includer)
            {
                Excluders.Remove(includer);
            }
            public Price AddPreis(decimal preis)
            {
                Price p = new Price { PreisProEinheit = preis };
                Preise.Add(p);
                return p;
            }

            public void RemovePreis(Price preis)
            {
                Preise.Remove(preis);
            }
            private Price GetValidPrice()
            {
                Price r = Preise.FirstOrDefault(v => v.IsValid);

                return r != null ? r : new Price();
            }
            private string GetArtikelnr()
            {
                string r = _artikelnr;

                if (Preis.IsValid && Preis.Artikelnr.Length > 0)
                    r = Preis.Artikelnr;

                return r;
            }

        }
        public class InExcluder
        {
            public enum Typen { Excluder, Includer }
            public Typen Typ { get; set; }
            public List<InExcludeOption> Selection { get; set; } = new List<InExcludeOption>();
            public List<ZahlVergleich> Zahlvergleiche { get; set; } = new List<ZahlVergleich>();
            public List<FormelVergleich> Formelvergleiche { get; set; } = new List<FormelVergleich>();

            public bool IsEmpty => Selection.Count + Zahlvergleiche.Count + Formelvergleiche.Count == 0;

            public InExcluder(Typen typ)
            {
                Typ = typ;
            }

            #region Selection
            public InExcludeOption AddSelectedOption(string merkmalgruppeid, string merkmalid, string optionscode)
            {
                InExcludeOption selection = new InExcludeOption { MerkmalgruppeID = merkmalgruppeid, MerkmalID = merkmalid, OptionCode = optionscode };
                Selection.Add(selection);
                return selection;
            }
            public InExcludeOption AddEmptySelection()
            {
                return AddSelectedOption("", "", "");
            }
            public void RemoveSelection(InExcludeOption selection)
            {
                Selection.Remove(selection);
            }
            #endregion

            #region Zahlvergleich
            public ZahlVergleich AddZahlvergleich(string merkmalgruppeid, string merkmalid, Vergleichsoperatoren vergleichsoperator, decimal zahl)
            {
                ZahlVergleich vergleich = new ZahlVergleich { MerkmalgruppeID = merkmalgruppeid, MerkmalID = merkmalid, Operator=vergleichsoperator, Zahl=zahl };
                Zahlvergleiche.Add(vergleich);
                return vergleich;
            }
            public ZahlVergleich AddEmptyZahlvergleich()
            {
                return AddZahlvergleich("", "", Vergleichsoperatoren.gleich, 0);
            }
            public void RemoveZahlvergleich(ZahlVergleich vergleich)
            {
                Zahlvergleiche.Remove(vergleich);
            }
            #endregion

            #region Formelvergleich

            public FormelVergleich AddEmptyFormelvergleich()
            {
                return AddFormelvergleich("", "", Vergleichsoperatoren.gleich, 0);
            }
            public FormelVergleich AddFormelvergleich(string merkmalgruppeid, string merkmalid, Vergleichsoperatoren vergleichsoperator, decimal zahl)
            {
                FormelVergleich vergleich = new FormelVergleich { MerkmalgruppeID = merkmalgruppeid, MerkmalID = merkmalid, Operator=vergleichsoperator, Zahl=zahl };
                Formelvergleiche.Add(vergleich);
                return vergleich;
            }
            public void AddFormelvergleich(FormelVergleich formelvergleich)
            {
                Formelvergleiche.Add(formelvergleich);
            }
            public void RemoveFormelvergleich(FormelVergleich formelvergleich)
            {
                Formelvergleiche.Remove(formelvergleich);
            }
            #endregion
        }
        public class InExcludeOption : Pointer
        {
            public string OptionCode { get; set; }
            public bool OptionDefiniert => OptionCode != null && OptionCode.Length > 0;

            public InExcludeOption()
            {

            }
        }

        public enum Vergleichsoperatoren { gleich, kleiner, kleinergleich, groesser, groessergleich }

        public class ZahlVergleich : Pointer, I_Vergleich
        {

            public Vergleichsoperatoren Operator { get; set; }
            public string Operatorname => Operator.ToString();
            public decimal Zahl { get; set; }
            public Pruefaktionen Pruefaktion { get; set; }

            public ZahlVergleich()
            {

            }

            public bool CheckVergleich(decimal vergleichszahl)
            {
                bool r = false;
                switch (Operator)
                {
                    case Vergleichsoperatoren.gleich:
                        r = Zahl == vergleichszahl;
                        break;
                    case Vergleichsoperatoren.kleiner:
                        r = vergleichszahl < Zahl;
                        break;
                    case Vergleichsoperatoren.kleinergleich:
                        r = vergleichszahl <= Zahl;
                        break;
                    case Vergleichsoperatoren.groesser:
                        r = vergleichszahl > Zahl;
                        break;
                    case Vergleichsoperatoren.groessergleich:
                        r = vergleichszahl >= Zahl;
                        break;
                    default:
                        r = false;
                        break;
                }
                return r;
            }
        }
        public class FormelVergleich : ZahlVergleich
        {
            public string Formelname { get; set; }

            public FormelVergleich()
            {

            }

        }

        public class Formel : Pointer
        {
            public string Formelname { get; set; }
            public bool IsEnabled { get; set; } = true;

            public Formel()
            {

            }
        }

        public class Multiplikator : Pointer
        {
            public string RelatedMerkmalgruppe { get; set; } = "";
            public string RelatedMerkmal { get; set; } = "";
            public decimal Faktor { get; set; }
            public decimal ProMenge { get; set; }
            public decimal ConnectedMenge { get; set; }
            public decimal Menge => ProMenge > 0 ? Math.Round(ConnectedMenge / ProMenge, 0) * Faktor : 0;
        }
        public class Mengenlimiter : Pointer
        {
            public enum Limitherkunfte { Fix, Formel }
            public enum Typen { Min, Max, Fix }
            public enum Limitbreaks { OK, ZuKlein, ZuGross }
            public Typen Typ { get; set; }
            public Limitherkunfte Limitherkunft { get; set; }
            public bool Enabled { get; set; } = true;
            public string RelatedMerkmalgruppe { get; set; } = "";
            public string RelatedMerkmal { get; set; } = "";
            public decimal ProMenge { get; set; } = 1;
            public decimal Mengelimit { get; set; }
            public string Formel { get; set; }
            public Limitbreaks Limitbreak { get; set; }
            public bool ConnectedToMerkmal => RelatedMerkmalgruppe.Length > 0 && RelatedMerkmal.Length > 0;
            public List<InExcluder> Excluders { get; set; } = new List<InExcluder>();
            public List<InExcluder> Includers { get; set; } = new List<InExcluder>();

            public Mengenlimiter()
            {

            }
            public InExcluder AddEmtpyExcluder()
            {
                InExcluder exculder = new InExcluder(InExcluder.Typen.Excluder);
                exculder.AddEmptySelection();
                Excluders.Add(exculder);
                return exculder;
            }
            public InExcluder AddEmptyIncluder()
            {
                InExcluder inculder = new InExcluder(InExcluder.Typen.Includer);
                inculder.AddEmptySelection();
                Includers.Add(inculder);
                return inculder;
            }


        }
        public class Price
        {
            bool _isvalid = false;

            public decimal PreisProEinheit { get; set; } = 0;
            public decimal ImPreisInkludierteMenge { get; set; } = 0;
            public string Artikelnr { get; set; } = "";
            public List<InExcluder> Excluders { get; set; } = new List<InExcluder>();
            public List<InExcluder> Includers { get; set; } = new List<InExcluder>();
            public bool IsValid { get { return Excluders.Count + Includers.Count > 0 ? _isvalid : true; } set { _isvalid = value; } }
            public Price()
            {

            }
            public InExcluder AddEmtpyExcluder()
            {
                InExcluder exculder = new InExcluder(InExcluder.Typen.Excluder);
                exculder.AddEmptySelection();
                Excluders.Add(exculder);
                return exculder;
            }
            public InExcluder AddEmptyIncluder()
            {
                InExcluder inculder = new InExcluder(InExcluder.Typen.Includer);
                inculder.AddEmptySelection();
                Includers.Add(inculder);
                return inculder;
            }


        }

    }

}
