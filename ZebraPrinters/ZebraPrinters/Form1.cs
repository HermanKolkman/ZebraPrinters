using CSharpContactlessSmartCardSample;
using MyNewBadgeMaker;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Printing;
using System.Windows.Forms;
using Zebra.Sdk.Card.Job;
using Zebra.Sdk.Card.Printer;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer.Discovery;
using ZebraPrinters.Classes;

namespace ZebraPrinters
{
    public partial class Form1 : Form
    {
        Classes.clsPasData _newPasje;
        internal Classes.clsPasData NewPasje
        {
            get { return _newPasje; }
            set { _newPasje = value; }
        }

        Classes.KaartCoderenMetCim _kc;
        private Classes.KaartCoderenMetCim Kc
        {
            get
            {
                if (null == _kc)
                {
                    _kc = new Classes.KaartCoderenMetCim();
                }
                return _kc;
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CancelPrintJobs();
            DataForBadge();
            PrintBadge();
        }
      
        private void PrintBadge()
        {
            string _printerName = cboPasPrinter.SelectedValue.ToString();
            printDocument1.PrinterSettings.PrinterName = _printerName;  
            KaartCoderenMetCim kc = new KaartCoderenMetCim();
            kc.PersNummer = NewPasje.Persid;
            kc.KarakterEigenFirma = NewPasje.Badge;
            kc.SallandKey = NewPasje.Badgenr;
            kc.DatumVan = NewPasje.Datuig;
            kc.DatumTot = NewPasje.Vervaldat;
            kc.Naam = NewPasje.Achternaam;
            if (NewPasje.Persid.Length == 7)
            {
                NewPasje.Badge = "0";
            }
            string persID = NewPasje.Persid.Length > 5 ? NewPasje.Persid.Substring(0, 6) : NewPasje.Persid;
            kc.Badgecode = string.Format("{0}{1}{2}", persID, NewPasje.Badge, NewPasje.Badgenr);
            if (_printerName.Contains("Zebra ZXP Series"))
            {
                ZBRPrinter printer = new ZBRPrinter();
                string msg = "";
                int errValue = 0;
                // Opens a connection to a printer driver
                if (!printer.Open(_printerName, out msg))
                {
                    msg = "CONTACT Open Error: " + ZBRUtil.TranslateErrorCode(errValue) + "\n" + "Error Code : " + errValue.ToString();
                    throw new Exception(msg);
                }
                // Moves the card into position for encoding 
                if (printer.MoveCardToSmartCardEncoder(2, out errValue) == 0)
                {
                    msg = "CONTACT StartCard Error:" + ZBRUtil.TranslateErrorCode(errValue) + "\n" + "Error Code : " + errValue.ToString();
                    throw new Exception(msg);
                }
                string errorMsg = "";
                kc.RFIDdevice = cboEncoder.SelectedValue.ToString();
                if (kc.CodeerBadgeZebra())
                
                {
                    SampleCodeMag mag = new SampleCodeMag();
                    msg = persID + NewPasje.Badge + NewPasje.Badgenr;
                    mag.PerformMagneticEncodeJob(this.cboPasPrinter.Text, ref msg);
                    printer.MoveSmartCardToPrintReadyPosition(2, out errValue);
                    printDocument1.Print();
                }

            }
            #region Zebra ZC350 USB Card Printer
            else if (_printerName.Contains("Zebra ZC350 USB Card Printer"))
            {
                    string msg = "";
                    int errValue = 0;
                    Connection connection = null;
                    ZebraPrinterZmotif zmotifCardPrinter = null;
                    // connection = new UsbConnection("\\\\?\\usb#vid_0a5f&pid_00a7#411738706#...");
                    string sconnectio = string.Empty;
                    foreach (DiscoveredUsbPrinter usbPrinter in UsbDiscoverer.GetZebraUsbPrinters())
                    {
                        sconnectio = usbPrinter.ToString();
                    }
                    connection = new UsbConnection(sconnectio);
                    connection.Open();
                    zmotifCardPrinter = ZebraCardPrinterFactory.GetZmotifPrinter(connection);
                    if (zmotifCardPrinter.HasSmartCardEncoder())
                    {
                        ConfigureSmartCardJobSettings(zmotifCardPrinter);
                        int jobId = zmotifCardPrinter.SmartCardEncode(1);
                    }

                    string errorMsg = "";
                    kc.RFIDdevice = cboEncoder.SelectedValue.ToString();
                //if (kc.CodeerBadgeZebra())
                    if (kc.CodeerBadge())
                    {
                        //SampleCodeMag mag = new SampleCodeMag();
                        //msg = persID + NewPasje.Badge + NewPasje.Badgenr;
                        //mag.PerformMagneticEncodeJob(this.cboPasPrinter.Text, ref msg);
                        //int jobId = zmotifCardPrinter.PositionCard();
                   
                        //printDocument1.Print();
                    }





                //        //ZBRPrinter printer = new ZBRPrinter();
                //        //string msg = "";
                //        //int errValue = 0;



                //        // Opens a connection to a printer driver
                //        //if (!printer.Open(_printerName, out msg))
                //        //{
                //        //    msg = "CONTACT Open Error: " + ZBRUtil.TranslateErrorCode(errValue) + "\n" + "Error Code : " + errValue.ToString();
                //        //    throw new Exception(msg);
                //        //}
                //        // Moves the card into position for encoding 

                //        //if (printer.MoveCardToSmartCardEncoder(2, out errValue) == 0)
                //        //{
                //        //    msg = "CONTACT StartCard Error:" + ZBRUtil.TranslateErrorCode(errValue) + "\n" + "Error Code : " + errValue.ToString();
                //        //    throw new Exception(msg);
                //        //}

                //        string _ContactlessReader = "";
                //        string _ContactReader = "";
                //        string errorMsg = "";
                //        WinSCard.GetPCSCReaders(out _ContactlessReader, out _ContactReader, out errorMsg);
                //        kc.RFIDdevice = _ContactlessReader;

                //        if (kc.CodeerBadgeZebra(_printerName, string.Format("pc {0} gebruiker {1}", System.Environment.MachineName, System.Environment.UserName), out errorMsg))
                //        {

                //            //SampleCodeMag mag = new SampleCodeMag();
                //            //msg = persID + NewPasje.Badge + NewPasje.Badgenr;
                //            //mag.PerformMagneticEncodeJob(this.cboPasPrinter.Text, ref msg);

                //            //NewPasje.BlokkenGecodeerd = kc.BlokkenGecodeerd.ToString();
                //            //NewPasje.BadgeUID = kc.UID_Badge;
                //            //this.panel1.BackColor = System.Drawing.Color.Transparent;
                //            //printer.MoveSmartCardToPrintReadyPosition(2, out errValue);
                //            //printDocument1.Print();


                //            //bool foutloos = true;

                //            //if (foutloos)
                //            //{
                //            //    Classes.clsPasData.deleteDatFile(NewPasje.Persid);
                //            //}
                //            //if (!this.IsDisposed)
                //            //{
                //            //    Application.OpenForms["NewBadge"].BringToFront();
                //            //}
                //            //SetButtons();
                //        }

                //    }

                //    else
                //    {
                //        this.panel1.BackColor = System.Drawing.Color.Transparent;
                //        printDocument1.Print();
                //        Thread.Sleep(4000);
                //        Classes.clsPasData.deleteDatFile(NewPasje.Persid);
                //        SetButtons();
                //        MiFareCoderen mf = new MiFareCoderen();
                //        mf.RfidCode.Text = string.Format("{0}{1}{2}", NewPasje.Persid, NewPasje.Badge, NewPasje.Badgenr);
                //        mf.Show();
                //    }
                //}
                //else
                //{
                //    MessageBox.Show("geen id image gevonden");
                //}
                //}

                //catch (Exception err)
                //{
                //    MessageBox.Show("PrintBadge " + err.Message);
                //}
                //finally
                //{
                //    pnlButtons.Enabled = true;
                //    Cursor.Current = Cursors.Default;
                //    if (!this.IsDisposed)
                //    {
                //        Application.OpenForms["NewBadge"].BringToFront();
                //    }
                //}
                #endregion
            }
            }



        private static void ConfigureSmartCardJobSettings(ZebraCardPrinter zebraCardPrinter)
        {
            Console.Write("Available smart card encoding types: ");
            Dictionary<string, string> smartCardEncoders = zebraCardPrinter.GetSmartCardConfigurations();
            Console.WriteLine(string.Join(", ", smartCardEncoders.Keys));

            // Configure smart card encoding type
            string encoderType = "";
            if (smartCardEncoders.ContainsKey("mifare"))
            {
                encoderType = "mifare";
            }
            else if (smartCardEncoders.ContainsKey("hf"))
            {
                encoderType = "hf";
            }
            else if (smartCardEncoders.ContainsKey("other"))
            {
                encoderType = "other";
            }
            else if (smartCardEncoders.ContainsKey("contact") || smartCardEncoders.ContainsKey("contact_station"))
            {
                encoderType = smartCardEncoders.ContainsKey("contact") ? "contact" : "contact_station";
            }

            if (!string.IsNullOrEmpty(encoderType))
            {
                Console.WriteLine($"Setting encoder type to: {encoderType}");
                if (encoderType.Contains("contact"))
                {
                    zebraCardPrinter.SetJobSetting(ZebraCardJobSettingNames.SMART_CARD_CONTACT, "yes");
                }
                else
                {
                    zebraCardPrinter.SetJobSetting(ZebraCardJobSettingNames.SMART_CARD_CONTACTLESS, encoderType);
                }
            }
        }



        private void DataForBadge()
        {
            label3.Text = "TEST BADGE ";
            lblVoorletters.Text = string.Empty;
            lblAchternaam.Text = "Kolkman";
            lblPersnr.Text = "494492";
            lblAanmaakdatum.Text = DateTime.Now.ToShortDateString();
            lblVervaldatum.Text = DateTime.Now.AddYears(2).ToShortDateString();
            string ims = string.Format("{0} |TEST BADGE ATR|-|Tata Steel IJmuiden BV |{1}|TEST BADGE ATR | TEST BADGE ATR   |18|406258|0|  |{2}|U", "494492", DateTime.Now.ToShortDateString(), DateTime.Now.AddYears(2).ToShortDateString());
            NewPasje = new Classes.clsPasData(ims);
        }

        private void CancelPrintJobs()
        {
            try
            {

                Cursor.Current = Cursors.WaitCursor;
                var ps = new PrintServer();
                var queues = ps.GetPrintQueues();
                foreach (PrintQueue pq in queues)
                {
                    if (pq != null)
                    {
                        if (pq.Description.Contains(cboPasPrinter.SelectedValue.ToString()))
                        {
                            var jobs = pq.GetPrintJobInfoCollection();
                            foreach (var job in jobs)
                            {
                                job.Cancel();
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cboPasPrinter.DataSource = Functies.InstPrinters();
            cboEncoder.DataSource =  Kc.rfidDevices();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

                try
                {                   
                    string persID = NewPasje.Persid.Length > 6 ? NewPasje.Persid.Substring(0, 6) : NewPasje.Persid;
                    string badgecode = "${2 " + persID + NewPasje.Badge + NewPasje.Badgenr + "}$";

                    e.Graphics.DrawString(badgecode, this.Font, Brushes.Black, 1, 1);
                    foreach (Control ctl in this.panel1.Controls)
                    {
                        if (ctl.Visible)
                        {
                            if (ctl.GetType() == typeof(Label))
                            {
                                e.Graphics.DrawString(ctl.Text, ctl.Font, Brushes.Black, ctl.Left, ctl.Top);
                            }

                            if (ctl.GetType() == typeof(PictureBox))
                            {                                
                                    PictureBox pb = (PictureBox)ctl;
                                    Image newImage = pb.Image; // Image.FromFile(@pb.ImageLocation);
                                    e.Graphics.DrawImage(newImage, ctl.Left, ctl.Top, ctl.Width, ctl.Height);                                
                            }
                        }
                    }


                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }


           

        }
    }
}
