using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Principal;

namespace ZebraPrinters.Classes
{
    [Serializable]
    class clsPasData
    {
        #region properties;
        string _ims;
        public string Ims
        {
            get { return _ims; }
            set { _ims = value;
            if (!string.IsNullOrEmpty(_ims))
            {
                string[] ar = _ims.Split('|');
                Persid = ar[0].Trim();
                Achternaam = ar[1].Trim();
                Voorltr = ar[2].Trim();
                Firma = ar[3].Trim();
                Datuig = ar[4].Trim();
                Eenheid = ar[5].Trim();
                Kenteken = ar[6].Trim();
                Layout = ar[7].Trim();
                Badgenr = ar[8].Trim();
                Badge = ar[9].Trim();
                Milieu = ar[10].Trim();
                Vervaldat = ar[11].Trim();
                Tag = ar[12].Trim();
            }
            }
        }
        public clsPasData(string ims, string bestandpad)
        {
            Ims = ims;
            Bestandpad = bestandpad;
        }

        public clsPasData(string ims)
        {
            Ims = ims;
            Bestandpad = "onbekend";
        }

        public clsPasData()
        {            
        }
        string _persid;
        /// <summary>
        /// Het personeelsnummer
        /// </summary>
        public string Persid
        {
            get { return _persid; }
            set { _persid = value; }
        }
        string _achternaam, _voorltr, _firma, _datuig, _eenheid, _kenteken, _layout, _badgenr, badge, _milieu, _vervaldat, _tag, _badgeUID, _blokkenGecodeerd, _idpic, _bestandpad;

        public string Bestandpad
        {
            get { return _bestandpad; }
            set { _bestandpad = value; }
        }

        public string Idpic
        {
            get { return _idpic; }
            set { _idpic = value; }
        }
        int _secrutechEmployeeID;

        public int SecrutechEmployeeID
        {
            get { return _secrutechEmployeeID; }
            set { _secrutechEmployeeID = value; }
        }

        public string BlokkenGecodeerd
        {
            get { return _blokkenGecodeerd; }
            set { _blokkenGecodeerd = value; }
        }

        public string BadgeUID
        {
            get { return _badgeUID; }
            set { _badgeUID = value; }
        }

        public string Voorltr
        {
            get { return _voorltr; }
            set { _voorltr = value; }
        }

        public string Achternaam
        {
            get { return _achternaam; }
            set { _achternaam = value; }
        }
 
         public string Vervaldat
        {
            get { return _vervaldat; }
            set { _vervaldat = value; }
        }

         public string Datuig
         {
             get { return _datuig; }
             set { _datuig = value; }
         }

         public string Kenteken
         {
             get { return _kenteken; }
             set { _kenteken = value; }
         }     
        /// <summary>
        /// Dit is de kaartcode, of de Sallandkey, het nummer dat in de Slitex
        /// database in de tabel percard staat als Kaartcode
        /// </summary>
        public string Badgenr
        {
            get { return _badgenr; }
            set { _badgenr = value; }
        }

        public string Firma
        {
            get { return _firma; }
            set { _firma = value; }
        }

        public string Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }


        public string Milieu
        {
            get { return _milieu; }
            set { _milieu = value; }
        }

        public string Badge
        {
            get { return badge; }
            set { badge = value; }
        }


        public string Layout
        {
            get { return _layout; }
            set { _layout = value; }
        }


        public string Eenheid1
        {
            get { return _eenheid; }
            set { _eenheid = value; }
        }

        public string Eenheid
        {
            get { return _eenheid; }
            set { _eenheid = value; }
        }

  

        string _magneetstrip = string.Empty;
        public string Magneetstrip
        {
            get { return _magneetstrip; }
            set { _magneetstrip = value; }
        }

# endregion;

        //public static List<clsPasData> GetDataStrings()
        //{
        //    try
        //    {
        //        //try
        //        //{
        //        //    string padx = clsGlobals.PasDataPath().Replace("fs34", "fs03");
        //        //    foreach (string bestand in Directory.GetFiles(padx))
        //        //    {
        //        //        File.Move(bestand, bestand.Replace("fs03", "fs34"));
        //        //    }
        //        //}
        //        //catch { }
        //        List<clsPasData> newList = new List<clsPasData>();
        //        if (clsGlobals.PasDataPath().EndsWith("BX___") && WindowsIdentity.GetCurrent().Name.ToLower().Contains(@"494492"))
        //        {
        //            string[] balies = new string[] { "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX" };
                    
        //            foreach (string b in balies)
        //            {
        //                string pad = clsGlobals.PasDataPath().Replace("BX", b);

                    
        //            string[] bronbestanden = Directory.GetFiles(pad);


        //            foreach (string bestand in bronbestanden)
        //            {
        //                try
        //                {
        //                    if (File.Exists(bestand))
        //                    {

        //                        FileInfo fi = new FileInfo(bestand);
        //                        if (fi.Extension.ToLower().Contains("dat"))
        //                        {
        //                            using (TextReader reader = new StreamReader(fi.FullName))
        //                            {
        //                                newList.Add(new clsPasData(reader.ReadLine()));
        //                            }
        //                        }
        //                    }
        //                }
        //                catch (IOException ioErr)
        //                {
        //                    //Classes.Functies.ScrhijfNaarEventLog("GetDataStrings ioErr" + ioErr.Message, System.Diagnostics.EventLogEntryType.Warning);
        //                }
        //                catch (Exception err) { Classes.Functies.ScrhijfNaarEventLog("GetDataStrings" + err.Message, System.Diagnostics.EventLogEntryType.Warning); }
        //            }
        //            }

        //        }
        //        else{
        //            string padNaarDatFiles = string.Empty;
        //            //if(!clsGlobals.Fotostraat.ToUpper().Contains("BT"))
        //            //{
        //            //    padNaarDatFiles = clsGlobals.PasDataPath();
        //            //}
        //            //else
        //            //{
        //            //    padNaarDatFiles = clsGlobals.PasDataDBBPath();
        //            //}
        //            padNaarDatFiles = clsGlobals.PasDataPath();
        //            string[] bronbestanden = null;
        //            try
        //            {
        //                bronbestanden = Directory.GetFiles(padNaarDatFiles);
        //            }
        //            catch (Exception)
        //            {
                        
                       
        //            }
                     
        //            if (null == bronbestanden || bronbestanden.Count() == 0)
        //            {
        //                padNaarDatFiles = clsGlobals.PasDataDBBPath();
        //                bronbestanden = Directory.GetFiles(padNaarDatFiles);
        //            }


        //        foreach (string bestand in bronbestanden)
        //        {
        //            try
        //            {
        //                if (File.Exists(bestand))
        //                {

        //                    FileInfo fi = new FileInfo(bestand);
        //                    if (fi.Extension.ToLower().Contains("dat"))
        //                    {
        //                        using (TextReader reader = new StreamReader(fi.FullName))
        //                        {
                                   
        //                            newList.Add(new clsPasData(reader.ReadLine(),padNaarDatFiles));
        //                        }
        //                    }
        //                }
        //            }
        //            catch (IOException ioErr)
        //            {
        //                //Classes.Functies.ScrhijfNaarEventLog("GetDataStrings ioErr" + ioErr.Message, System.Diagnostics.EventLogEntryType.Warning);
        //            }
        //            catch (Exception err) { Classes.Functies.ScrhijfNaarEventLog("GetDataStrings" + err.Message, System.Diagnostics.EventLogEntryType.Warning); }
        //        }
        //        }
        //        return newList;
        //    }
        //    catch (Exception er)
        //    {
        //        Classes.Functies.ScrhijfNaarEventLog(er.Message, System.Diagnostics.EventLogEntryType.Warning);
        //        return null;
        //    }
        //}

        //public static void deleteDatFile(string persnr)
        //{

        //    if (persnr.Length > 6)
        //    {
        //        persnr = persnr.Substring(0, 6);
        //    }
        //    try
        //    {

        //        File.Delete(Path.Combine(clsGlobals.PasDataPath(), persnr + ".dat"));

        //    }
        //    catch { }
        //    File.Delete(Path.Combine(clsGlobals.PasDataDBBPath(), persnr + ".dat"));


        //}

        //public static void CreateDatFile(string persnummer, string ims)
        //{

        //    string path = @Path.Combine(clsGlobals.PasDataPath(),persnummer + ".DAT");
        //    if (!File.Exists(path))
        //    {
        //        StreamWriter objWriter = new StreamWriter(path);
        //        objWriter.Write(ims);
        //        objWriter.Close();
                
        //        //File.Create(path);
        //        //TextWriter tw = new StreamWriter(path);
        //        //tw.WriteLine(ims);
        //        //tw.Close();
        //    }
        //    //else if (File.Exists(path))
        //    //{
        //    //    TextWriter tw = new StreamWriter(path);
        //    //    tw.WriteLine("The next line!");
        //    //    tw.Close();
        //    //}
        //}

      

    }
}
