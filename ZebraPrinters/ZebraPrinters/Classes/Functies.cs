using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections;

namespace ZebraPrinters.Classes
{
        class Functies
        {

            public static void ScrhijfNaarEventLog(string msg, EventLogEntryType logtype)
            {
                try
                {

                if (!EventLog.SourceExists(Application.ProductName))
                    EventLog.CreateEventSource(Application.ProductName, "Application");
                    EventLog.WriteEntry(Application.ProductName, msg, logtype);
                 }
                catch { }
            }

   

            public static List<string> InstPrinters()
            {
                List<string> apparaten = new List<string>();
                ArrayList lijst = new ArrayList(System.Drawing.Printing.PrinterSettings.InstalledPrinters);
                apparaten.Add("Selecteer device");
                foreach (string p in lijst)
                {
                    apparaten.Add(p);
                }
                return apparaten;
            }

        }
  

}
