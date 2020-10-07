using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Data;
using System.Collections;
using CSharpContactlessSmartCardSample;
using System.Dynamic;

namespace ZebraPrinters.Classes
{
    class KaartCoderenMetCim
    {
        #region properties
        string _persNummer; 
        string _sallandKey;
        string _karakterEigenFirma;
        string _badgecode;
        string _UID_Badge;
        string _datumVan;
        string _RFIDdevice;

        public string RFIDdevice
        {
            get { return _RFIDdevice; }
            set { _RFIDdevice = value; }
        }

        public string DatumVan
        {
            get { return _datumVan; }
            set { _datumVan = value; }
        }
        string _datumTot;

        public string DatumTot
        {
            get { return _datumTot; }
            set { _datumTot = value; }
        }
        string _naam;

        public string Naam
        {
            get { return _naam; }
            set { _naam = value; }
        }

        public string UID_Badge
        {
            get { return _UID_Badge; }
            set { _UID_Badge = value; }
        }
        public string Badgecode
        {
            get { 
                
                return _badgecode; }
            set { _badgecode = value; }
        }
        public string KarakterEigenFirma
        {
            get { return _karakterEigenFirma; }
            set { _karakterEigenFirma = value; }
        }

        public string SallandKey
        {
            get { return _sallandKey; }
            set { _sallandKey = value; }
        }

        public string PersNummer
        {
            get { return _persNummer; }
            set { _persNummer = value; }
        }

        StringBuilder _blokkenGecodeerd;
        public StringBuilder BlokkenGecodeerd
        {
            get
            {
                if (_blokkenGecodeerd == null)
                {
                    _blokkenGecodeerd = new StringBuilder();
                }
                return _blokkenGecodeerd;
            }
            set { _blokkenGecodeerd = value; }
        }

        bool _buscodering, _blackboxcodering, _sallandcodering, _segmentNul;

        public bool SegmentNul
        {
            get { return _segmentNul; }
            set { _segmentNul = value; }
        }

        public bool Sallandcodering
        {
            get { return _sallandcodering; }
            set { _sallandcodering = value; }
        }

        public bool Blackboxcodering
        {
            get { return _blackboxcodering; }
            set { _blackboxcodering = value; }
        }

        public bool Buscodering
        {
            get { return _buscodering; }
            set { _buscodering = value; }
        }

        bool _DirectCoderen = false;
        public bool DirectCoderen
        {
            get { return _DirectCoderen; }
            set { _DirectCoderen = value; }
        }

        # endregion

        #region Global Variables 
        /**************************************************/
        //////////////////Global Variables//////////////////
        /**************************************************/
        IntPtr hContext;                                        //Context Handle value
        String readerName;                                      //Global Reader Variable
        int retval;                                             //Return Value
        uint dwscope;                                           //Scope of the resource manager context
        Boolean IsAuthenticated;                                //Boolean variable to check the authentication
        Boolean release_flag;                                   //Flag to release 
        IntPtr hCard;                                           //Card handle
        IntPtr protocol;                                        //Protocol used currently
        Byte[] ATR = new Byte[33];                              //Array stores Card ATR
        int card_Type;                                          //Stores the card type
        byte[] sendBuffer = new byte[263];                        //Send Buffer in SCardTransmit
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x16)]
        // public byte receiveBuffer;
        Byte[] receiveBuffer = new Byte[255];                   //Receive Buffer in SCardTransmit

        public byte[] SendBuff = new byte[263];
        public byte[] RecvBuff = new byte[263];
        public int SendLen, RecvLen, nBytesRet, reqType, Aprotocol, dwProtocol, cbPciLength;

        int sendbufferlen;
        int  receivebufferlen;                    //Send and Receive Buffer length in SCardTransmit
        Byte bcla;                                             //Class Byte
        Byte bins;                                             //Instruction Byte
        Byte bp1;                                              //Parameter Byte P1
        Byte bp2;                                              //Parameter Byte P2
        Byte len;                                              //Lc/Le Byte
        Byte[] data = new Byte[255];                            //Data Bytes
        HiDWinscard.SCARD_READERSTATE ReaderState;              //Object of SCARD_READERSTATE
        int value_Timeout;                                      //The maximum amount of time to wait for an action
        uint ReaderCount;                                       //Count for number of readers
        String ReaderList;                                      //List Of Reader
        System.Object sender1;                                  //Object of the Sender
        //System.Windows.RoutedEventArgs e1;                      //Object of the Event
        Byte currentBlock;                                      //Stores the current block selected
        //String keych;                                           //Stores the string in key textbox
        int discarded;                                          //Stores the number of discarded character
        public delegate void DelegateTimer();                   //delegate of the Timer
        private System.Timers.Timer timer;                      //Object of the Timer
        public bool bTxtWrongInputChange;                       //Variable to check the wrong input in key textbox. Used in text change event
        bool read_pressed;                                      //flag to check read pressed

        private const int IOCTL_CCID_ESCAPE = 0x3136B0;
        # endregion

        #region coderen
        public bool CodeerBadge()
        {
            try
            {
                if (EstablishContext())
                {
                    if(Connect())
                    {
                        if (AuthenticateCard("5")) { CodeerCard(); }
                        if (AuthenticateCard("9")) { CoderenSalland(); }                       
                       return true;                        
                    }
                }
                return false;
            }
            catch (Exception err)
            {

                return false;
            }            
        }

        public bool CodeerBadgeZebra()
        {
            string errMsg = string.Empty;
            try
            {
                byte[] key = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }; 
                if (EstablishContextZebra())
                {
                    if (ConnectToZebra())   
                    {
                        if (LoadKey("KeyA", key.Length, key, out errMsg) != 0)
                        {
                           // errMsg = "Mifare Load Key Failed";
                            return false;
                        }
                        else
                        {
                            if (AuthenticateCardZebra("5")) { CodeerCardZebra(); }
                            if (AuthenticateCardZebra("9")) { CoderenSallandZebra(); }                    
                        }
                        return true;
                    }
                    else
                    {
                        //DisconnectPreviousHandle();
                    }
                }
                return false;
            }
            catch (Exception err)
            {

                return false;
            }
        }

        public bool testCodeerBadgeZebra( string errMsg)
        {
            errMsg = string.Empty;
            try
            {
                byte[] key = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

                if (EstablishContextZebra())
                {
                    if (ConnectToZebra())
                    //if (Connect())
                    //if(ConnectToHID())
                    {
                        if (LoadKey("KeyA", key.Length, key, out errMsg) != 0)
                        {
                            errMsg = "Mifare Load Key Failed";
                            return false;
                        }
                        else
                        {
                            //passed = MifareTestAllBlocks(smartCardType, "KeyA", 1, fullTest, out errMsg);
                            if (AuthenticateCardZebra("5")) { CodeerCardZebra(); }
                            //if (AuthenticateCardZebra("9")) { CoderenSallandZebra(); }
                            //if (AuthenticateCardZebra("35")) { CoderenBlackBoxZebra(); }
                            //if (AuthenticateCardZebra("1")) { CoderenSegmentNulZebra(); }
                        }
                        return true;
                    }
                    else
                    {
                        //DisconnectPreviousHandle();
                    }
                }
                return false;
            }
            catch (Exception err)
            {
                string fiy = err.Message;

                return false;
            }
        }

        // Load Key - loads a key into a reader
        public int LoadKey(string keyType, int keyLength, byte[] key, out string errMsg)
        {
            errMsg = string.Empty;
            byte[] cmd = null;
            byte[] respBuf = null;
            try
            {
                byte kType = 0x00;
                switch (keyType.ToLower())
                {
                    case "keya":
                    case "key a":
                        kType = 0x60;
                        break;

                    case "keyb":
                    case "key b":
                        kType = 0x61;
                        break;
                }

                cmd = new byte[5 + keyLength];
                cmd[0] = 0xFF;
                cmd[1] = 0x82;
                cmd[2] = 0x00;
                cmd[3] = kType;
                cmd[4] = (byte)keyLength;

                for (int i = 0; i < keyLength; i++)
                    cmd[i + 5] = key[i];

                int cmdLength = keyLength + 5;

                int respSize = 128;
                respBuf = new byte[respSize];

                WinSCard.SCARD_IO_REQUEST sIO = new WinSCard.SCARD_IO_REQUEST();
                sIO.dwProtocol = _activeProtocol;
                sIO.cbPciLength = 8;

                int ret = WinSCard.SCardTransmit(_cardHandle, ref sIO, ref cmd[0], cmdLength, ref sIO,
                                                    ref respBuf[0], ref respSize);
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd = null;
                respBuf = null;
            }
        }


        private bool CodeerCardZebra()
        {
            try
            {
                WinSCard.SCARD_IO_REQUEST sIO = new WinSCard.SCARD_IO_REQUEST();
                sIO.dwProtocol = 0;// _activeProtocol;
                sIO.cbPciLength = 8;

                byte[] respBuf = null;
                int respSize = 8;
                respBuf = new byte[respSize];

                HiDWinscard.SCARD_IO_REQUEST rioreq;
                rioreq.cbPciLength = 8;
                rioreq.dwProtocol = 0x2; 
                string s4 = string.Format("{0}{1}", "000", Badgecode);
               // string s4 = string.Format("{0}{1}", "000", "1234560494492"); 
                string tmpStr = s4;

                sendBuffer[0] = 0xFF;                                     // CLA
                sendBuffer[1] = 0xD6;                                     // INS
                sendBuffer[2] = 0x00;                                     // P1
                sendBuffer[3] = (byte)int.Parse("05");            // P2 : Starting Block No.
                sendBuffer[4] = (byte)int.Parse("16");            // P3 : Data length

                for (int k1 = 0; k1 <= (tmpStr).Length - 1; k1++)
                {

                    sendBuffer[k1 + 5] = (byte)tmpStr[k1];

                }
                int SendLen = sendBuffer[4] + 5;
                int RecvLen = 0x02;

                retval = WinSCard.SCardTransmit(_cardHandle, ref sIO, ref sendBuffer[0], SendLen, ref sIO, ref respBuf[0], ref RecvLen);


                //retval = WinSCard.SCardTransmit(_cardHandle, ref sIO, ref cmd[0], cmdLength, ref sIO,
                //             ref respBuf[0], ref respSize);



                //retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, SendLen, ref rioreq, receiveBuffer, ref RecvLen);
                //retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, SendLen, IntPtr.Zero, receiveBuffer, ref RecvLen);
                if (retval == 0)
                {
                    //BlokkenGecodeerd.Append(string.Format("Sector 1 data blok 5-{0};", tmpStr));
                    return true;
                    //return ((receiveBuffer[(int)receivebufferlen - 2] == 0x90) && (receiveBuffer[(int)receivebufferlen - 1] == 0));
                }
                else
                {
                    throw new Exception("Fout in CodeerCard()");
                    //return false;
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }
        private bool CodeerCard()
        {
            try
            {
                HiDWinscard.SCARD_IO_REQUEST sioreq;
                sioreq.dwProtocol = 0x2;
                sioreq.cbPciLength = 8;
                HiDWinscard.SCARD_IO_REQUEST rioreq;
                rioreq.cbPciLength = 8;
                rioreq.dwProtocol = 0x2;
                string s4 = string.Format("{0}{1}", "000", Badgecode);
                //string s4 = string.Format("{0}{1}", "000", "123456"); 
                string tmpStr = s4;

                sendBuffer[0] = 0xFF;                                     // CLA
                sendBuffer[1] = 0xD6;                                     // INS
                sendBuffer[2] = 0x00;                                     // P1
                sendBuffer[3] = (byte)int.Parse("05");            // P2 : Starting Block No.
                sendBuffer[4] = (byte)int.Parse("16");            // P3 : Data length

                for (int k1 = 0; k1 <= (tmpStr).Length - 1; k1++)
                {

                    sendBuffer[k1 + 5] = (byte)tmpStr[k1];

                }
                int SendLen = sendBuffer[4] + 5;
                int RecvLen = 0x02;

                //retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, SendLen, ref rioreq, receiveBuffer, ref RecvLen);
                retval = HID.SCardTransmit((IntPtr) _cardHandle, ref sioreq, sendBuffer, SendLen, IntPtr.Zero, receiveBuffer, ref RecvLen);
                if (retval == 0)
                {
                    BlokkenGecodeerd.Append(string.Format("Sector 1 data blok 5-{0};", tmpStr));
                    return ((receiveBuffer[(int)receivebufferlen - 2] == 0x90) && (receiveBuffer[(int)receivebufferlen - 1] == 0));
                }
                else
                {
                    throw new Exception("Fout in CodeerCard()");
                    //return false;
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }



        bool CoderenSallandZebra()
        {

            //vanhier
            WinSCard.SCARD_IO_REQUEST sIO = new WinSCard.SCARD_IO_REQUEST();
            sIO.dwProtocol = 0;// _activeProtocol;
            sIO.cbPciLength = 8;

            byte[] respBuf = null;
            int respSize = 8;
            respBuf = new byte[respSize];

            HiDWinscard.SCARD_IO_REQUEST rioreq;
            rioreq.cbPciLength = 8;
            rioreq.dwProtocol = 0x2;
            string s4 = string.Format("{0}{1}", "000", Badgecode);
            //string s4 = string.Format("{0}{1}", "000", "123456"); 
            string tmpStr = s4;

            sendBuffer[0] = 0xFF;                                     // CLA
            sendBuffer[1] = 0xD6;                                     // INS
            sendBuffer[2] = 0x00;                                     // P1
            sendBuffer[3] = (byte)int.Parse("09");            // P2 : Starting Block No.
            sendBuffer[4] = (byte)int.Parse("16");            // P3 : Data length

            for (int k1 = 0; k1 <= (tmpStr).Length - 1; k1++)
            {

                sendBuffer[k1 + 5] = (byte)tmpStr[k1];

            }
            int SendLen = sendBuffer[4] + 5;
            int RecvLen = 0x02;
            // tot hier

            retval = WinSCard.SCardTransmit(_cardHandle, ref sIO, ref sendBuffer[0], SendLen, ref sIO, ref respBuf[0], ref RecvLen);
            if (retval == 0)
            {
                BlokkenGecodeerd.Append(string.Format("Sector 2 data blok 9-{0};", tmpStr));

                // wijzigen authenticatie


                tmpStr = "00 F4 5D C0 1F A2 FF 07 80 69 00 00 00 00 00 00";
                //ClearBuffers();
                sendBuffer[0] = 0xFF;                                     // CLA
                sendBuffer[1] = 0xD6;                                     // INS
                sendBuffer[2] = 0x00;                                     // P1
                sendBuffer[3] = (byte)int.Parse("11");            // P2 : Starting Block No.
                sendBuffer[4] = (byte)int.Parse("16");            // P3 : Data length

                string[] arrItems = tmpStr.Split(' ');
                int intArr = 0;
                foreach (string s in arrItems)
                {
                    sendBuffer[intArr + 5] = byte.Parse(s, System.Globalization.NumberStyles.HexNumber);
                    intArr++;
                }

                SendLen = sendBuffer[4] + 5;
                RecvLen = 0x02;

                //retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, SendLen, ref rioreq, receiveBuffer, ref RecvLen);
                //retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, SendLen, IntPtr.Zero, receiveBuffer, ref RecvLen);
                retval = WinSCard.SCardTransmit(_cardHandle, ref sIO, ref sendBuffer[0], SendLen, ref sIO, ref respBuf[0], ref RecvLen);
                if (retval == 0)
                {
                    //if (s4 == MifareLezen("9"))
                    //{
                        Sallandcodering = true;
                        BlokkenGecodeerd.Append(string.Format("Sector 2 KeyA blok 11-{0};", tmpStr));
                    //}
                    //else
                    //{
                    //    throw new Exception("Fout in CoderenSalland()");
                    //    //CoderenSalland2();
                    //}
                    return true;
                }
                else
                {
                    throw new Exception("Fout in CoderenSallandZebra()");
                    //return false;
                }
            }
            else
            {
                throw new Exception("Fout in CoderenSallandZebra()");
                //return false;
            }
        }

        bool CoderenSalland()
        {
            HiDWinscard.SCARD_IO_REQUEST sioreq;
            sioreq.dwProtocol = 0x2;
            sioreq.cbPciLength = 8;
            HiDWinscard.SCARD_IO_REQUEST rioreq;
            rioreq.cbPciLength = 8;
            rioreq.dwProtocol = 0x2;
            string s4 = string.Format("{0}{1}", "000", Badgecode);
            string tmpStr = s4;

            sendBuffer[0] = 0xFF;                                     // CLA
            sendBuffer[1] = 0xD6;                                     // INS
            sendBuffer[2] = 0x00;                                     // P1
            sendBuffer[3] = (byte)int.Parse("09");            // P2 : Starting Block No.
            sendBuffer[4] = (byte)int.Parse("16");            // P3 : Data length

            for (int k1 = 0; k1 <= (tmpStr).Length - 1; k1++)
            {

                sendBuffer[k1 + 5] = (byte)tmpStr[k1];

            }
            int SendLen = sendBuffer[4] + 5;
            int RecvLen = 0x02;

            //retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, SendLen, ref rioreq, receiveBuffer, ref RecvLen);
            retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, SendLen, IntPtr.Zero, receiveBuffer, ref RecvLen);
            if (retval == 0)
            {
                BlokkenGecodeerd.Append(string.Format("Sector 2 data blok 9-{0};", tmpStr));

                // wijzigen authenticatie


                tmpStr = "00 F4 5D C0 1F A2 FF 07 80 69 00 00 00 00 00 00";
                //ClearBuffers();
                sendBuffer[0] = 0xFF;                                     // CLA
                sendBuffer[1] = 0xD6;                                     // INS
                sendBuffer[2] = 0x00;                                     // P1
                sendBuffer[3] = (byte)int.Parse("11");            // P2 : Starting Block No.
                sendBuffer[4] = (byte)int.Parse("16");            // P3 : Data length

                string[] arrItems = tmpStr.Split(' ');
                int intArr = 0;
                foreach (string s in arrItems)
                {
                    sendBuffer[intArr + 5] = byte.Parse(s, System.Globalization.NumberStyles.HexNumber);
                    intArr++;
                }

                SendLen = sendBuffer[4] + 5;
                RecvLen = 0x02;

                //retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, SendLen, ref rioreq, receiveBuffer, ref RecvLen);
                retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, SendLen, IntPtr.Zero, receiveBuffer, ref RecvLen);

                if (retval == 0)
                {
                    if (s4 == MifareLezen("9"))
                    {
                        Sallandcodering = true;
                        BlokkenGecodeerd.Append(string.Format("Sector 2 KeyA blok 11-{0};", tmpStr));
                    }
                    else
                    { 
                        CoderenSalland2();
                    }
                    return true;
                }
                else
                {
                    throw new Exception("Fout in CoderenSalland()");
                    //return false;
                }
            }
            else
            {
                throw new Exception("Fout in CoderenSalland()");
                //return false;
            }
        }


        string CoderenSalland2()
        {
            StandaardAuto("9");
            HiDWinscard.SCARD_IO_REQUEST sioreq;
            sioreq.dwProtocol = 0x2;
            sioreq.cbPciLength = 8;
            HiDWinscard.SCARD_IO_REQUEST rioreq;
            rioreq.cbPciLength = 8;
            rioreq.dwProtocol = 0x2;
            string s4 = string.Format("{0}{1}", "000", Badgecode);
            string tmpStr = s4;

            sendBuffer[0] = 0xFF;                                     // CLA
            sendBuffer[1] = 0xD6;                                     // INS
            sendBuffer[2] = 0x00;                                     // P1
            sendBuffer[3] = (byte)int.Parse("09");            // P2 : Starting Block No.
            sendBuffer[4] = (byte)int.Parse("16");            // P3 : Data length

            for (int k1 = 0; k1 <= (tmpStr).Length - 1; k1++)
            {

                sendBuffer[k1 + 5] = (byte)tmpStr[k1];

            }
            int SendLen = sendBuffer[4] + 5;
            int RecvLen = 0x02;

            //retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, SendLen, ref rioreq, receiveBuffer, ref RecvLen);
            retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, SendLen, IntPtr.Zero, receiveBuffer, ref RecvLen);
            if (retval == 0)
            {
                // wijzigen authenticatie


                tmpStr = "00 F4 5D C0 1F A2 FF 07 80 69 00 00 00 00 00 00";
                //ClearBuffers();
                sendBuffer[0] = 0xFF;                                     // CLA
                sendBuffer[1] = 0xD6;                                     // INS
                sendBuffer[2] = 0x00;                                     // P1
                sendBuffer[3] = (byte)int.Parse("11");            // P2 : Starting Block No.
                sendBuffer[4] = (byte)int.Parse("16");            // P3 : Data length

                string[] arrItems = tmpStr.Split(' ');
                int intArr = 0;
                foreach (string s in arrItems)
                {
                    sendBuffer[intArr + 5] = byte.Parse(s, System.Globalization.NumberStyles.HexNumber);
                    intArr++;
                }

                SendLen = sendBuffer[4] + 5;
                RecvLen = 0x02;

                //retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, SendLen, ref rioreq, receiveBuffer, ref RecvLen);
                retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, SendLen, IntPtr.Zero, receiveBuffer, ref RecvLen);

                if (retval == 0)
                {
                    if (s4 == MifareLezen("9"))
                    {
                        Sallandcodering = true;
                        BlokkenGecodeerd.Append(string.Format("Sector 2 KeyA blok 11-{0};", tmpStr));
                    }
                    else
                    {
                        CoderenSalland3();
                                            if (s4 == MifareLezen("9"))
                    {
                        Sallandcodering = true;
                        BlokkenGecodeerd.Append(string.Format("Sector 2 KeyA blok 11-{0};", tmpStr));
                    }

                    }
                }
                return s4;
                //else
                //{
                //    throw new Exception("Fout in CoderenSalland()");
                //    //return false;
                //}

                //if (retval == 0)
                //{

                //    //if (tmpStr == MifareLezen("9"))
                //    //{
                //    //    BlokkenGecodeerd.Append(string.Format("Sector 2 KeyA blok 11-{0};", tmpStr));
                //    //}
                //    return s4;
                //}
                //else
                //{
                //    throw new Exception("Fout in CoderenSalland2()");
                //    //return false;
                //}
            }
            else
            {
                throw new Exception("Fout in CoderenSalland2()");
                //return false;
            }
        }

        string CoderenSalland3()
        {
            AuthenticateCard("9");
            HiDWinscard.SCARD_IO_REQUEST sioreq;
            sioreq.dwProtocol = 0x2;
            sioreq.cbPciLength = 8;
            HiDWinscard.SCARD_IO_REQUEST rioreq;
            rioreq.cbPciLength = 8;
            rioreq.dwProtocol = 0x2;
            string s4 = string.Format("{0}{1}", "000", Badgecode);
            string tmpStr = s4;

            sendBuffer[0] = 0xFF;                                     // CLA
            sendBuffer[1] = 0xD6;                                     // INS
            sendBuffer[2] = 0x00;                                     // P1
            sendBuffer[3] = (byte)int.Parse("09");            // P2 : Starting Block No.
            sendBuffer[4] = (byte)int.Parse("16");            // P3 : Data length

            for (int k1 = 0; k1 <= (tmpStr).Length - 1; k1++)
            {

                sendBuffer[k1 + 5] = (byte)tmpStr[k1];

            }
            int SendLen = sendBuffer[4] + 5;
            int RecvLen = 0x02;

            //retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, SendLen, ref rioreq, receiveBuffer, ref RecvLen);
            retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, SendLen, IntPtr.Zero, receiveBuffer, ref RecvLen);
            if (retval == 0)
            {
                // wijzigen authenticatie


                tmpStr = "00 F4 5D C0 1F A2 FF 07 80 69 00 00 00 00 00 00";
                //ClearBuffers();
                sendBuffer[0] = 0xFF;                                     // CLA
                sendBuffer[1] = 0xD6;                                     // INS
                sendBuffer[2] = 0x00;                                     // P1
                sendBuffer[3] = (byte)int.Parse("11");            // P2 : Starting Block No.
                sendBuffer[4] = (byte)int.Parse("16");            // P3 : Data length

                string[] arrItems = tmpStr.Split(' ');
                int intArr = 0;
                foreach (string s in arrItems)
                {
                    sendBuffer[intArr + 5] = byte.Parse(s, System.Globalization.NumberStyles.HexNumber);
                    intArr++;
                }

                SendLen = sendBuffer[4] + 5;
                RecvLen = 0x02;

                //retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, SendLen, ref rioreq, receiveBuffer, ref RecvLen);
                retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, SendLen, IntPtr.Zero, receiveBuffer, ref RecvLen);

                if (retval == 0)
                {
                    if (s4 == MifareLezen("9"))
                    {
                        Sallandcodering = true;
                        BlokkenGecodeerd.Append(string.Format("Sector 2 KeyA blok 11-{0};", tmpStr));
                    }
                }
                return s4;
                //else
                //{
                //    throw new Exception("Fout in CoderenSalland()");
                //    //return false;
                //}

                //if (retval == 0)
                //{

                //    //if (tmpStr == MifareLezen("9"))
                //    //{
                //    //    BlokkenGecodeerd.Append(string.Format("Sector 2 KeyA blok 11-{0};", tmpStr));
                //    //}
                //    return s4;
                //}
                //else
                //{
                //    throw new Exception("Fout in CoderenSalland2()");
                //    //return false;
                //}
            }
            else
            {
                throw new Exception("Fout in CoderenSalland2()");
                //return false;
            }
        }
        #endregion

        private bool EstablishContext()
        {
            readerName = RFIDdevice;                
            dwscope = 2;
            retval = HID.SCardEstablishContext((uint)dwscope, IntPtr.Zero, IntPtr.Zero, out hContext);
            if (retval == 0)
            {
                IsAuthenticated = false;
                release_flag = true;
                return true;
            }
            else
            {
                throw new Exception("Fout in EstablishContext()");
            }
        }


        private bool EstablishContextZebra()
        {    
            retval = WinSCard.SCardEstablishContext(WinSCard.SCARD_SCOPE_USER, 0, 0, ref _context);            
            if (retval == 0)
            {                
                IsAuthenticated = false;
                release_flag = true;
                return true;
            }
            else
            {
                throw new Exception("Fout in EstablishContext()");
              
            } 
        }

        private uint _activeProtocol = 0;
        private int _context = 0;
        private int _cardHandle = 0;
        private int _previousCardHandle = 0;

        private bool ConnectToZebra()
        {        
            try
            {
                //for (int i = 0; i < 10; i++)
                //{                    
                //    retval = WinSCard.SCardConnect((int)hContext, RFIDdevice, WinSCard.SCARD_SHARE_SHARED,
                //                                WinSCard.SCARD_PROTOCOL_T0 | WinSCard.SCARD_PROTOCOL_T1,
                //                                ref _cardHandle, ref _activeProtocol);
                //    if (retval == 0)
                //        return true;
                //}
                // dit is de code uit de volgens mij werkende ZXP3 hieronder dus
                for (int i = 0; i < 10; i++)
                {
                    retval = WinSCard.SCardConnect(_context, RFIDdevice, WinSCard.SCARD_SHARE_SHARED,
                                                WinSCard.SCARD_PROTOCOL_T0 | WinSCard.SCARD_PROTOCOL_T1,
                                                ref _cardHandle, ref _activeProtocol);
                    if (retval == 0)
                        return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ConnectToZebra() " + ex.Message);
            }
            return false;
        }

        private bool ConnectToHID()
        {
            {
                retval = HID.SCardConnect( (IntPtr)hContext, RFIDdevice, (uint)HiDWinscard.SCARD_SHARE_SHARED, (uint)HiDWinscard.SCARD_PROTOCOL_T1,
                                 out hCard, out protocol
                                  );       //Command to connect the card ,protocol T=1
            }
            ReaderState.RdrName = RFIDdevice; // readerName;
            ReaderState.RdrCurrState = HiDWinscard.SCARD_STATE_UNAWARE;
            ReaderState.RdrEventState = 0;
            ReaderState.UserData = "Mifare Card";
            value_Timeout = 0;
            ReaderCount = 1;
            if (retval == 0)
            {
                retval = HID.SCardGetStatusChange(hContext, value_Timeout, ref ReaderState, ReaderCount);
                if (ReaderState.ATRValue[ReaderState.ATRLength - 0x6].Equals(1))
                {
                    card_Type = 1;
                    ATR_UID(card_Type);
                }
                else if (ReaderState.ATRValue[ReaderState.ATRLength - 0x6].Equals(2))
                {
                    card_Type = 2;
                    ATR_UID(card_Type);
                }
                else
                {
                    card_Type = 3;
                    ATR_UID(card_Type);
                }
                return true;
            }
            else
            {
                throw new Exception("Fout in ConnectToHID()");
                //return false;
            }            
        }

        public bool Connect()
        {

            {
                retval = HID.SCardConnect(hContext, RFIDdevice, (uint)HiDWinscard.SCARD_SHARE_SHARED, (uint)HiDWinscard.SCARD_PROTOCOL_T1,
                                 out hCard, out protocol
                                  );       //Command to connect the card ,protocol T=1
            }
            ReaderState.RdrName = RFIDdevice;
            ReaderState.RdrCurrState = HiDWinscard.SCARD_STATE_UNAWARE;
            ReaderState.RdrEventState = 0;
            ReaderState.UserData = "Mifare Card";
            value_Timeout = 0;
            ReaderCount = 1;
            if (retval == 0)
            {
                retval = HID.SCardGetStatusChange(hContext, value_Timeout, ref ReaderState, ReaderCount);
                if (ReaderState.ATRValue[ReaderState.ATRLength - 0x6].Equals(1))
                {
                    card_Type = 1;
                    ATR_UID(card_Type);
                }
                else if (ReaderState.ATRValue[ReaderState.ATRLength - 0x6].Equals(2))
                {
                    card_Type = 2;
                    ATR_UID(card_Type);
                }
                else
                {
                    card_Type = 3;
                    ATR_UID(card_Type);
                }
                return true;
            }
            else
            {
                //throw new Exception("Fout in ConnectToHID()");
                return false;
            }
        }

        // Block To Bytes - converts the block integer to a byte array
        private byte[] BlockToBytes(int block)
        {
            byte[] blk = { (byte)((block & 0xFF00) >> 8), (byte)(block & 0x00FF) };
            return blk;
        }


        // Set Card Type - configures the type of card the reader will support
        public int SetCardType(string cardType)
        {
            byte[] rcvBuf = null;
            try
            {
                byte cType = 0x02;

                switch (cardType.ToLower())
                {
                    case "keya":
                    case "key a":
                        cType = 0x00;
                        break;

                    case "keyb":
                    case "key b":
                        cType = 0x01;
                        break;
                }

                byte[] cmd = { 0x95, cType };

                int rcvBufLen = 262;
                int bytesRet = 0;

                rcvBuf = new byte[262];
                Array.Clear(rcvBuf, 0, 262);

                return WinSCard.SCardControlEx(_cardHandle, IOCTL_CCID_ESCAPE, ref cmd, 2, ref rcvBuf,
                                               rcvBufLen, ref bytesRet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                rcvBuf = null;
            }
        }

        private bool AuthenticateCardZebra(string AddressLSB)
        {

            int block = int.Parse(AddressLSB);
            byte[] respBuf = null;
            int ret = -1;

            byte kType = 0x60;
            byte[] blk = BlockToBytes(block);
            byte[] apdu = { 0xFF, 0x86, 0x00, 0x00, 0x05, 0x01, blk[0], blk[1], kType, 1 };
            respBuf = new byte[1024];
            int respSize = respBuf.Length;

            int sendLen = 10;

            WinSCard.SCARD_IO_REQUEST sIO = new WinSCard.SCARD_IO_REQUEST();
            sIO.dwProtocol = _activeProtocol;
            sIO.cbPciLength = 8;

            ret = WinSCard.SCardTransmit(_cardHandle, ref sIO, ref apdu[0], sendLen, ref sIO, ref respBuf[0], ref respSize);

            if (ret != 0)
            {
               // errMsg = WinSCard.GetSCardErrMsg(ret);
                return false;
            }

            if (respBuf[0] != 0x90 && respBuf[1] != 0x00)
            {
               // errMsg = "Invalid Key";
                return false;
            }
            return true;

                //HiDWinscard.SCARD_IO_REQUEST sioreq;
                //sioreq.dwProtocol = 0x2;
                //sioreq.cbPciLength = 8;
                //HiDWinscard.SCARD_IO_REQUEST rioreq;
                //rioreq.cbPciLength = 8;
                //rioreq.dwProtocol = 0x2;
                ////'********************************************************************
                //// '           For Authentication using key number
                //// '*********************************************************************

                //bcla = 0xFF;
                //bins = 0x86;
                //bp1 = 0x0;
                //bp2 = 0x0;//'currentBlock
                //len = 0x5;
                //sendBuffer[0] = bcla;
                //sendBuffer[1] = bins;
                //sendBuffer[2] = bp1;
                //sendBuffer[3] = bp2;

                //sendBuffer[4] = len;
                //sendBuffer[5] = 0x1;           //Version
                //sendBuffer[6] = 0x0;           //Address MSB
                //sendBuffer[7] = (byte)int.Parse(AddressLSB);   //Address LSB

                //sendBuffer[8] = 0x60; //Key Type A

                //sendBuffer[9] = (byte)(0);  //Key Number

                //sendbufferlen = 0xA;
                //receivebufferlen = 255;


                //WinSCard.SCARD_IO_REQUEST sIO = new WinSCard.SCARD_IO_REQUEST();
                //sIO.dwProtocol = 2;// _activeProtocol;
                //sIO.cbPciLength = 8;

                ////retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, sendbufferlen, ref rioreq, receiveBuffer, ref receivebufferlen);
                ////retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, sendbufferlen, IntPtr.Zero, receiveBuffer, ref receivebufferlen);

                //byte[] blk = BlockToBytes(int.Parse(AddressLSB));
                //byte[] apdu = { 0xFF, 0x86, 0x00, 0x00, 0x05, 0x01, blk[0], blk[1], 0x60, (byte)(0) };

                //int h = (int)hContext;
                //byte[] respBuf = null;
                //respBuf = new byte[1024];
                //int respSize = respBuf.Length;

                //retval = CSharpContactlessSmartCardSample.WinSCard.SCardTransmit(_context, ref sIO, ref apdu[0], sendbufferlen, ref sIO, ref respBuf[0], ref respSize);
                //if (retval == 0)
                //{

                //    if (respBuf[0] != 0x90 && respBuf[1] != 0x00)
                //    {
                //        return false;
                //    }
                //    return true;
                //}
                //else
                //{
                //    throw new Exception(string.Format("Fout in AuthenticateCard({0})", AddressLSB));
                //}
                //return IsAuthenticated;
            
        }

        private bool AuthenticateCard(string AddressLSB)
        {
            HiDWinscard.SCARD_IO_REQUEST sioreq;
            sioreq.dwProtocol = 0x2;
            sioreq.cbPciLength = 8;
            HiDWinscard.SCARD_IO_REQUEST rioreq;
            rioreq.cbPciLength = 8;
            rioreq.dwProtocol = 0x2;
            //'********************************************************************
            // '           For Authentication using key number
            // '*********************************************************************

            bcla = 0xFF;
            bins = 0x86;
            bp1 = 0x0;
            bp2 = 0x0;//'currentBlock
            len = 0x5;
            sendBuffer[0] = bcla;
            sendBuffer[1] = bins;
            sendBuffer[2] = bp1;
            sendBuffer[3] = bp2;

            sendBuffer[4] = len;
            sendBuffer[5] = 0x1;           //Version
            sendBuffer[6] = 0x0;           //Address MSB
            sendBuffer[7] = (byte)int.Parse(AddressLSB);   //Address LSB

            sendBuffer[8] = 0x60; //Key Type A

            sendBuffer[9] = (byte)(0);  //Key Number

            sendbufferlen = 0xA;
            receivebufferlen = 255;
            //retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, sendbufferlen, ref rioreq, receiveBuffer, ref receivebufferlen);
            retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, sendbufferlen, IntPtr.Zero, receiveBuffer, ref receivebufferlen);
            if (retval == 0)
            {
                IsAuthenticated = ((receiveBuffer[(int)receivebufferlen - 2] == 0x90) && ((int)receiveBuffer[(int)receivebufferlen - 1] == 0));                
            }
            else
            {
                throw new Exception(string.Format( "Fout in AuthenticateCard({0})",AddressLSB));
            }
            return IsAuthenticated;
        }

        //********************************************************
        //Function Name:ATR_UID
        //Description:Gives ATR and UID of the card 
        //********************************************************
        private void ATR_UID(int card_type)
        {
            try
            {
                HiDWinscard.SCARD_IO_REQUEST sioreq;
                sioreq.dwProtocol = 0x2;
                sioreq.cbPciLength = 8;
                HiDWinscard.SCARD_IO_REQUEST rioreq;
                rioreq.cbPciLength = 8;
                rioreq.dwProtocol = 0x2;

                String uid_temp;
                String atr_temp;
                String s;
                atr_temp = "";
                uid_temp = "";
                s = "";
                StringBuilder hex = new StringBuilder(ReaderState.ATRValue.Length * 2);
                foreach (byte b in ReaderState.ATRValue)
                    hex.AppendFormat("{0:X2}", b);
                atr_temp = hex.ToString();
                atr_temp = atr_temp.Substring(0, ((int)(ReaderState.ATRLength)) * 2);
                if (ReaderState.ATRLength > 0)
                {
                    for (int k = 0; k <= ((ReaderState.ATRLength) * 2 - 1); k += 2)
                    {
                        s = s + atr_temp.Substring(k, 2) + " ";
                    }
                }

                atr_temp = s;

                bcla = 0xFF;
                bins = 0xCA;
                bp1 = 0x0;
                bp2 = 0x0;
                len = 0x0;

                sendBuffer[0] = bcla;
                sendBuffer[1] = bins;
                sendBuffer[2] = bp1;
                sendBuffer[3] = bp2;
                sendBuffer[4] = len;
                sendbufferlen = 0x5;
                receivebufferlen = 255;
                //retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, sendbufferlen, ref rioreq, receiveBuffer, ref receivebufferlen);
                retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, sendbufferlen, IntPtr.Zero, receiveBuffer, ref receivebufferlen);

                if (retval == 0)
                {
                    string tmpStr = "";
                    int indx;
                    int SendLen = 5;
                    int RecvLen = sendBuffer[4] + 2;
                    for (indx = RecvLen - 2; indx <= RecvLen - 1; indx++)
                    {
                        tmpStr = tmpStr + " " + string.Format("{0:X2}", receiveBuffer[indx]);
                    }

                    if (receivebufferlen > 2)
                    {
                        if ((receiveBuffer[(int)receivebufferlen - 2] == 0x90) && (receiveBuffer[(int)receivebufferlen - 1] == 0))
                        {
                            StringBuilder hex1 = new StringBuilder(((int)receivebufferlen - 2) * 2);
                            foreach (byte b in receiveBuffer)
                                hex1.AppendFormat("{0:X2}", b);
                            uid_temp = hex1.ToString();
                            uid_temp = uid_temp.Substring(0, ((int)(receivebufferlen - 2)) * 2);
                        }
                        else
                        {
                            ;
                        }
                    }
                }
                else
                {


                }
                if (uid_temp == "")
                {
                }
                else
                {
                    s = "";
                    for (int k = 0; k <= (((int)receivebufferlen - 2) * 2 - 1); k += 2)
                    {
                        s = s + uid_temp.Substring(k, 2) + " ";
                    }
                    uid_temp = s;
                    UID_Badge = s;
                }
            }
            catch (Exception err)
            {
                Functies.ScrhijfNaarEventLog(string.Format("Fout in ATR_UID(): {0}", err.Message), System.Diagnostics.EventLogEntryType.Warning);
                //throw new Exception(string.Format("Fout in ATR_UID(): {0}",err.Message) );
            }
        }

        public string  GetAtr()
        {
            ATR_UID(1);
            return UID_Badge;
        }


        public List<string> rfidDevices()
        {
            List<string> apparaten = new List<string>();
            apparaten.Add("Selecteer device");
            if (EstablishContext())
            {
                string ReaderList = "" + Convert.ToChar(0);
                int indx;
                int pcchReaders = 0;
                string rName = "";
                retval = ModWinsCard.SCardListReaders(this.hContext, null, null, ref pcchReaders);
                if (retval == 0)
                {
                    byte[] ReadersList = new byte[pcchReaders];

                    // Fill reader list
                    retval = ModWinsCard.SCardListReaders(this.hContext, null, ReadersList, ref pcchReaders);
                    if (retval == 0)
                    {
                        
                        indx = 0;
                        // Convert reader buffer to string
                        
                        while (ReadersList[indx] != 0)
                        {

                            while (ReadersList[indx] != 0)
                            {
                                rName = rName + (char)ReadersList[indx];
                                indx = indx + 1;
                            }

                            //Add reader name to list
                            apparaten.Add(rName);
                            rName = "";
                            indx = indx + 1;
                        }
                    }
                }                
            }
            return apparaten;
        }

        private void ClearBuffers()
        {

            long indx;

            for (indx = 0; indx <= 262; indx++)
            {

                RecvBuff[indx] = 0;
                SendBuff[indx] = 0;

            }

        }

        #region lezen 
        
        public string MifareLezen(string block)
        {
            try
            {
                string tmpStr;
               // ClearBuffers();
                if (block=="5" || block == "9" || block == "35")
                {
                    // Load Authentication Keys command
                    sendBuffer[0] = 0xFF;                                                                        // Class
                    sendBuffer[1] = 0x82;                                                                        // INS
                    sendBuffer[2] = 0x00;                                                                        // P1 : Key Structure
                    sendBuffer[3] = byte.Parse("00", System.Globalization.NumberStyles.HexNumber);
                    sendBuffer[4] = 0x06;                                                                        // P3 : Lc
                    if (block == "5")
                    {
                        sendBuffer[5] = byte.Parse("FF", System.Globalization.NumberStyles.HexNumber);        // Key 1 value
                        sendBuffer[6] = byte.Parse("FF", System.Globalization.NumberStyles.HexNumber);        // Key 2 value
                        sendBuffer[7] = byte.Parse("FF", System.Globalization.NumberStyles.HexNumber);        // Key 3 value
                        sendBuffer[8] = byte.Parse("FF", System.Globalization.NumberStyles.HexNumber);        // Key 4 value
                        sendBuffer[9] = byte.Parse("FF", System.Globalization.NumberStyles.HexNumber);        // Key 5 value
                        sendBuffer[10] = byte.Parse("FF", System.Globalization.NumberStyles.HexNumber);       // Key 6 value
                    }
                    if (block == "9")
                    {
                        sendBuffer[5] = byte.Parse("00", System.Globalization.NumberStyles.HexNumber);        // Key 1 value
                        sendBuffer[6] = byte.Parse("F4", System.Globalization.NumberStyles.HexNumber);        // Key 2 value
                        sendBuffer[7] = byte.Parse("5D", System.Globalization.NumberStyles.HexNumber);        // Key 3 value
                        sendBuffer[8] = byte.Parse("C0", System.Globalization.NumberStyles.HexNumber);        // Key 4 value
                        sendBuffer[9] = byte.Parse("1F", System.Globalization.NumberStyles.HexNumber);        // Key 5 value
                        sendBuffer[10] = byte.Parse("A2", System.Globalization.NumberStyles.HexNumber);       // Key 6 value
                    }
                    if (block == "35")
                    {
                        sendBuffer[5] = byte.Parse("C0", System.Globalization.NumberStyles.HexNumber);        // Key 1 value
                        sendBuffer[6] = byte.Parse("1F", System.Globalization.NumberStyles.HexNumber);        // Key 2 value
                        sendBuffer[7] = byte.Parse("A2", System.Globalization.NumberStyles.HexNumber);        // Key 3 value
                        sendBuffer[8] = byte.Parse("00", System.Globalization.NumberStyles.HexNumber);        // Key 4 value
                        sendBuffer[9] = byte.Parse("F4", System.Globalization.NumberStyles.HexNumber);        // Key 5 value
                        sendBuffer[10] = byte.Parse("5D", System.Globalization.NumberStyles.HexNumber);       // Key 6 value
                    }
                    sendbufferlen = 11;
                    receivebufferlen = 2;
                    HiDWinscard.SCARD_IO_REQUEST sioreq;
                    sioreq.dwProtocol = (uint)Aprotocol;
                    sioreq.cbPciLength = 8;
                    int SendLen = 11;
                    int RecvLen = sendBuffer[10] + 2;
                    //retCode = ModWinsCard.SCardTransmit(hCard, ref pioSendRequest, ref SendBuff[0], SendLen, ref pioSendRequest, ref RecvBuff[0], ref RecvLen);
                    if (_cardHandle > 0)
                    {
                        hCard = (IntPtr)_cardHandle;
                    }
                    retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, SendLen, IntPtr.Zero, receiveBuffer, ref RecvLen);
                    if (retval != ModWinsCard.SCARD_S_SUCCESS)
                    {
                        return string.Empty ;
                    }
                }

                if (AuthenticateCard(block))
                {
                    HiDWinscard.SCARD_IO_REQUEST sioreq;
                    sioreq.dwProtocol = (uint)Aprotocol;
                    sioreq.cbPciLength = 8;
                    sendBuffer[0] = 0xFF;                                     // CLA
                    sendBuffer[1] = 0xB0;                                     // INS
                    sendBuffer[2] = 0x00;                                     // P1
                    sendBuffer[3] = (byte)int.Parse(block);            // P2 : Starting Block No.
                    sendBuffer[4] = (byte)int.Parse("16");            // P3 : Data length
                    int SendLen = 5;
                    int RecvLen = sendBuffer[4] + 2;

                    retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, SendLen, IntPtr.Zero, receiveBuffer, ref RecvLen);
                    //Classes.Functies.ScrhijfNaarEventLog(retval.ToString(), System.Diagnostics.EventLogEntryType.Warning);
                    tmpStr = string.Empty;
                    if (retval == 0)
                    {
                        for (int indx = 0; indx <= (int)RecvLen - 3; indx++)
                        {
                            tmpStr = tmpStr + Convert.ToChar(receiveBuffer[indx]);
                        }                        
                        return tmpStr;
                    }
                    else
                    {
                        return string.Empty;
                    }

                }
                return string.Empty; 

            }
            catch (Exception err)
            {
                Classes.Functies.ScrhijfNaarEventLog(err.Message, System.Diagnostics.EventLogEntryType.Error);
                //FoutmeldingOplaan(err);
                return string.Empty ;
            }           
        }


        public string StandaardAuto(string block)
        {
            try
            {
                string tmpStr;
                // ClearBuffers();
                if (block == "5" || block == "9" || block == "35")
                {
                    // Load Authentication Keys command
                    sendBuffer[0] = 0xFF;                                                                        // Class
                    sendBuffer[1] = 0x82;                                                                        // INS
                    sendBuffer[2] = 0x00;                                                                        // P1 : Key Structure
                    sendBuffer[3] = byte.Parse("00", System.Globalization.NumberStyles.HexNumber);
                    sendBuffer[4] = 0x06;                                                                        // P3 : Lc
                    sendBuffer[5] = byte.Parse("FF", System.Globalization.NumberStyles.HexNumber);        // Key 1 value
                    sendBuffer[6] = byte.Parse("FF", System.Globalization.NumberStyles.HexNumber);        // Key 2 value
                    sendBuffer[7] = byte.Parse("FF", System.Globalization.NumberStyles.HexNumber);        // Key 3 value
                    sendBuffer[8] = byte.Parse("FF", System.Globalization.NumberStyles.HexNumber);        // Key 4 value
                    sendBuffer[9] = byte.Parse("FF", System.Globalization.NumberStyles.HexNumber);        // Key 5 value
                    sendBuffer[10] = byte.Parse("FF", System.Globalization.NumberStyles.HexNumber);       // Key 6 value
                    sendbufferlen = 11;
                    receivebufferlen = 2;
                    HiDWinscard.SCARD_IO_REQUEST sioreq;
                    sioreq.dwProtocol = (uint)Aprotocol;
                    sioreq.cbPciLength = 8;
                    int SendLen = 11;
                    int RecvLen = sendBuffer[10] + 2;
                    //retCode = ModWinsCard.SCardTransmit(hCard, ref pioSendRequest, ref SendBuff[0], SendLen, ref pioSendRequest, ref RecvBuff[0], ref RecvLen);
                    retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, SendLen, IntPtr.Zero, receiveBuffer, ref RecvLen);
                    if (retval != ModWinsCard.SCARD_S_SUCCESS)
                    {
                        return string.Empty;
                    }
                }

                if (AuthenticateCard(block))
                {
                    HiDWinscard.SCARD_IO_REQUEST sioreq;
                    sioreq.dwProtocol = (uint)Aprotocol;
                    sioreq.cbPciLength = 8;
                    sendBuffer[0] = 0xFF;                                     // CLA
                    sendBuffer[1] = 0xB0;                                     // INS
                    sendBuffer[2] = 0x00;                                     // P1
                    sendBuffer[3] = (byte)int.Parse(block);            // P2 : Starting Block No.
                    sendBuffer[4] = (byte)int.Parse("16");            // P3 : Data length
                    int SendLen = 5;
                    int RecvLen = sendBuffer[4] + 2;

                    retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, SendLen, IntPtr.Zero, receiveBuffer, ref RecvLen);
                    //Classes.Functies.ScrhijfNaarEventLog(retval.ToString(), System.Diagnostics.EventLogEntryType.Warning);
                    tmpStr = string.Empty;
                    if (retval == 0)
                    {
                        for (int indx = 0; indx <= (int)RecvLen - 3; indx++)
                        {
                            tmpStr = tmpStr + Convert.ToChar(receiveBuffer[indx]);
                        }
                        return tmpStr;
                    }
                    else
                    {
                        return string.Empty;
                    }

                }
                return string.Empty;

            }
            catch (Exception err)
            {
                Classes.Functies.ScrhijfNaarEventLog(err.Message, System.Diagnostics.EventLogEntryType.Error);
                //FoutmeldingOplaan(err);
                return string.Empty;
            }
        }
        #endregion



    }

    //Class for Constants
    public class HiDWinscard
    {
        // Context Scope

        public const int SCARD_STATE_UNAWARE = 0x0;

        //The application is unaware about the curent state, This value results in an immediate return
        //from state transition monitoring services. This is represented by all bits set to zero

        public const int SCARD_SHARE_SHARED = 2;

        // Application will share this card with other 
        // applications.

        //   Disposition

        public const int SCARD_UNPOWER_CARD = 2; // Power down the card on close

        //   PROTOCOL

        public const int SCARD_PROTOCOL_T0 = 0x1;                  // T=0 is the active protocol.
        public const int SCARD_PROTOCOL_T1 = 0x2;                  // T=1 is the active protocol.
        public const int SCARD_PROTOCOL_UNDEFINED = 0x0;

        //IO Request Control
        public struct SCARD_IO_REQUEST
        {
            public UInt32 dwProtocol;
            public UInt32 cbPciLength;
        }


        //Reader State

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SCARD_READERSTATE
        {
            public string RdrName;
            public string UserData;
            public uint RdrCurrState;
            public uint RdrEventState;
            public uint ATRLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x24, ArraySubType = UnmanagedType.U1)]
            public byte[] ATRValue;
        }
        //Card Type
        public const int card_Type_Mifare_1K = 1;
        public const int card_Type_Mifare_4K = 2;

    }

    //**************************************************************************
    //class for Hexidecimal to Byte and Byte to Hexidecimal conversion
    //**************************************************************************
    public class HexToBytenByteToHex
    {
        public HexToBytenByteToHex()
        {

            // constructor

        }
        public static int GetByteCount(string hexString)
        {
            int numHexChars = 0;
            char c;
            // remove all none A-F, 0-9, characters
            for (int i = 0; i < hexString.Length; i++)
            {
                c = hexString[i];
                if (IsHexDigit(c))
                    numHexChars++;
            }
            // if odd number of characters, discard last character
            if (numHexChars % 2 != 0)
            {
                numHexChars--;
            }
            return numHexChars / 2; // 2 characters per byte
        }

        public static byte[] GetBytes(string hexString, out int discarded)
        {
            discarded = 0;
            string newString = "";
            char c;
            // remove all none A-F, 0-9, characters
            for (int i = 0; i < hexString.Length; i++)
            {
                c = hexString[i];
                if (IsHexDigit(c))
                    newString += c;
                else
                    discarded++;
            }
            // if odd number of characters, discard last character
            if (newString.Length % 2 != 0)
            {
                discarded++;
                newString = newString.Substring(0, newString.Length - 1);
            }

            int byteLength = newString.Length / 2;
            byte[] bytes = new byte[byteLength];
            string hex;
            int j = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                hex = new String(new Char[] { newString[j], newString[j + 1] });
                bytes[i] = HexToByte(hex);
                j = j + 2;
            }
            return bytes;
        }
        public static string ToString(byte[] bytes)
        {
            string hexString = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                hexString += bytes[i].ToString("X2");
            }
            return hexString;
        }
        public static bool InHexFormat(string hexString)
        {
            bool hexFormat = true;

            foreach (char digit in hexString)
            {
                if (!IsHexDigit(digit))
                {
                    hexFormat = false;
                    break;
                }
            }
            return hexFormat;
        }

        public static bool IsHexDigit(Char c)
        {
            int numChar;
            int numA = Convert.ToInt32('A');
            int num1 = Convert.ToInt32('0');
            c = Char.ToUpper(c);
            numChar = Convert.ToInt32(c);
            if (numChar >= numA && numChar < (numA + 6))
                return true;
            if (numChar >= num1 && numChar < (num1 + 10))
                return true;
            return false;
        }
        private static byte HexToByte(string hex)
        {
            if (hex.Length > 2 || hex.Length <= 0)
                throw new ArgumentException("hex must be 1 or 2 characters in length");
            byte newByte = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return newByte;
        }


    }
    /// <summary>
    /// //Declaration of all dll used in the project
    /// </summary>
    class HID
    {
        //*********************************************************************************************************
        // Define Constants, To Add "About Dialog Box" in System Menu
        //*********************************************************************************************************
        public const Int32 WM_SYSCOMMAND = 0x112;
        public const Int32 MF_SEPARATOR = 0x800;
        public const Int32 MF_BYPOSITION = 0x400;
        public const Int32 MF_STRING = 0x0;

        public const Int32 _SettingsSysMenuID = 1000;
        public const Int32 _AboutSysMenuID = 1001;


        //*********************************************************************************************************
        // Function Name: GetSystemMenu
        // In Parameter : hWnd - A handle to the window that will own a copy of the window menu.
        //                bRevert - The action to be taken. If this parameter is FALSE, GetSystemMenu returns a handle to the copy of the window menu currently in use. 
        // Out Parameter: ------
        // Description  : Enables the application to access the window menu for copying and modifying.
        //*********************************************************************************************************
        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);


        //*********************************************************************************************************
        // Function Name: InsertMenu
        // In Parameter : hWnd - A handle to the menu to be changed. 
        //                Position - The menu item before which the new menu item is to be inserted, as determined by the uFlags parameter. 
        //                Flag - Controls the interpretation of the uPosition parameter and the content, appearance, and behavior of the new menu item.
        //                IDNewItem - The identifier of the new menu item or, if the uFlags parameter has the MF_POPUP flag set, a handle to the drop-down menu or submenu.
        //                newItem - The content of the new menu item.
        // Out Parameter: ---------
        // Description  : Inserts a new menu item into a menu, moving other items down the menu.
        //*********************************************************************************************************
        [DllImport("user32.dll")]
        public static extern bool InsertMenu(IntPtr hMenu, Int32 wPosition, Int32 wFlags, Int32 wIDNewItem, string lpNewItem);


        // *********************************************************************************************************
        // Function Name: SCardEstablishContext
        // In Parameter : dwScope - Scope of the resource manager context.
        //                pvReserved1 - Reserved for future use and must be NULL
        //                pvReserved2 - Reserved for future use and must be NULL.
        // Out Parameter: phContext - A handle to the established resource manager context
        // Description  : Establishes context to the reader
        //*********************************************************************************************************
        [DllImport("WinScard.dll")]
        public static extern int SCardEstablishContext(uint dwScope,
        IntPtr notUsed1,
        IntPtr notUsed2,
        out IntPtr phContext);

 

        // *********************************************************************************************************
        // Function Name: SCardReleaseContext
        // In Parameter : phContext - A handle to the established resource manager context              
        // Out Parameter: -------
        // Description  :Releases context from the reader
        //*********************************************************************************************************
        [DllImport("WinScard.dll")]
        public static extern int SCardReleaseContext(IntPtr phContext);


        // *********************************************************************************************************
        // Function Name: SCardConnect
        // In Parameter : hContext - A handle that identifies the resource manager context.
        //                cReaderName  - The name of the reader that contains the target card.
        //                dwShareMode - A flag that indicates whether other applications may form connections to the card.
        //                dwPrefProtocol - A bitmask of acceptable protocols for the connection.  
        // Out Parameter: ActiveProtocol - A flag that indicates the established active protocol.
        //                hCard - A handle that identifies the connection to the smart card in the designated reader. 
        // Description  : Connect to card on reader
        //*********************************************************************************************************
        [DllImport("WinScard.dll")]
        public static extern int SCardConnect(IntPtr hContext, 
        string cReaderName,
        uint dwShareMode,
        uint dwPrefProtocol,
        out IntPtr hCard,
        out IntPtr ActiveProtocol);


        // *********************************************************************************************************
        // Function Name: SCardDisconnect
        // In Parameter : hCard - Reference value obtained from a previous call to SCardConnect.
        //                Disposition - Action to take on the card in the connected reader on close.  
        // Out(Parameter)
        // Description  : Disconnect card from reader
        //*********************************************************************************************************
        [DllImport("WinScard.dll")]
        public static extern int SCardDisconnect(IntPtr hCard, int Disposition);


        //    *********************************************************************************************************
        // Function Name: SCardListReaders
        // In Parameter : hContext - A handle to the established resource manager context
        //                mszReaders - Multi-string that lists the card readers with in the supplied readers groups
        //                pcchReaders - length of the readerlist buffer in characters
        // Out Parameter: mzGroup - Names of the Reader groups defined to the System
        //                pcchReaders - length of the readerlist buffer in characters
        // Description  : List of all readers connected to system 
        //*********************************************************************************************************
        [DllImport("WinScard.dll", EntryPoint = "SCardListReadersA", CharSet = CharSet.Ansi)]
        public static extern int SCardListReaders(
          IntPtr hContext,
          byte[] mszGroups,
          byte[] mszReaders,
          ref UInt32 pcchReaders
          );


        // *********************************************************************************************************
        // Function Name: SCardState
        // In Parameter : hCard - Reference value obtained from a previous call to SCardConnect.
        // Out Parameter: state - Current state of smart card in  the reader
        //                protocol - Current Protocol
        //                ATR - 32 bytes buffer that receives the ATR string
        //                ATRLen - Supplies the length of ATR buffer
        // Description  : Current state of the smart card in the reader
        //*********************************************************************************************************
        [DllImport("WinScard.dll")]
        public static extern int SCardState(IntPtr hCard, ref IntPtr state, ref IntPtr protocol, ref Byte[] ATR, ref int ATRLen);


        // *********************************************************************************************************
        // Function Name: SCardTransmit
        // In Parameter : hCard - A reference value returned from the SCardConnect function.
        //                pioSendRequest - A pointer to the protocol header structure for the instruction.
        //                SendBuff- A pointer to the actual data to be written to the card.
        //                SendBuffLen - The length, in bytes, of the pbSendBuffer parameter. 
        //                pioRecvRequest - Pointer to the protocol header structure for the instruction ,Pointer to the protocol header structure for the instruction, 
        //                followed by a buffer in which to receive any returned protocol control information (PCI) specific to the protocol in use.
        //                RecvBuffLen - Supplies the length, in bytes, of the pbRecvBuffer parameter and receives the actual number of bytes received from the smart card.
        // Out Parameter: pioRecvRequest - Pointer to the protocol header structure for the instruction ,Pointer to the protocol header structure for the instruction, 
        //                followed by a buffer in which to receive any returned protocol control information (PCI) specific to the protocol in use.
        //                RecvBuff - Pointer to any data returned from the card.
        //                RecvBuffLen - Supplies the length, in bytes, of the pbRecvBuffer parameter and receives the actual number of bytes received from the smart card.
        // Description  : Transmit APDU to card 
        //*********************************************************************************************************
        [DllImport("WinScard.dll")]
        public static extern int SCardTransmit(IntPtr hCard,
                                                ref HiDWinscard.SCARD_IO_REQUEST pioSendRequest,
                                                byte[] SendBuff,
                                                int SendBuffLen,
                                                IntPtr pioRecvRequest,
                                                byte[] RecvBuff, 
                                                ref int RecvBuffLen);

        //[DllImport("winscard.dll")]
        //public static extern int SCardTransmit(int hCard, ref HiDWinscard.SCARD_IO_REQUEST pioSendRequest, ref byte SendBuff, int SendBuffLen, ref HiDWinscard.SCARD_IO_REQUEST pioRecvRequest, ref byte RecvBuff, ref int RecvBuffLen);

        [DllImport("winscard.dll")]
        public static extern int SCardTransmit(int hCard, ref WinSCard.SCARD_IO_REQUEST pioSendRequest, ref byte SendBuff, int SendBuffLen, ref WinSCard.SCARD_IO_REQUEST pioRecvRequest, ref byte RecvBuff, ref int RecvBuffLen);


        [DllImport("winscard.dll")]
        public static extern int SCardTransmit(int hCard, ref HiDWinscard.SCARD_IO_REQUEST pioSendRequest, ref byte SendBuff, int SendBuffLen, ref HiDWinscard.SCARD_IO_REQUEST pioRecvRequest, ref byte RecvBuff, ref int RecvBuffLen);

        // *********************************************************************************************************
        // Function Name: SCardGetStatusChange
        // In Parameter : hContext - A handle that identifies the resource manager context.
        //                value_TimeOut - The maximum amount of time, in milliseconds, to wait for an action.
        //                ReaderState -  An array of SCARD_READERSTATE structures that specify the readers to watch, and that receives the result.
        //                ReaderCount -  The number of elements in the rgReaderStates array.
        // Out Parameter: ReaderState - An array of SCARD_READERSTATE structures that specify the readers to watch, and that receives the result.
        // Description  : The current availability of the cards in a specific set of readers changes.
        //*********************************************************************************************************
        [DllImport("winscard.dll", CharSet = CharSet.Unicode)]
        public static extern int SCardGetStatusChange(IntPtr hContext,
        int value_Timeout,
        ref HiDWinscard.SCARD_READERSTATE ReaderState,
        uint ReaderCount);

        public class WinSCard
        {
            #region Data Structures

            [StructLayout(LayoutKind.Sequential)]
            public struct SCARD_IO_REQUEST
            {
                public uint dwProtocol;
                public int cbPciLength;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct APDURec
            {
                public byte bCLA;
                public byte bINS;
                public byte bP1;
                public byte bP2;
                public byte bP3;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
                public byte[] Data;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
                public byte[] SW;
                public bool IsSend;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct SCARD_READERSTATE
            {
                public string RdrName;
                public int UserData;
                public int RdrCurrState;
                public int RdrEventState;
                public int ATRLength;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 37)]
                public byte ATRValue;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct VERSION_CONTROL
            {
                public int SmclibVersion;
                public byte DriverMajor;
                public byte DriverMinor;
                public byte FirmwareMajor;
                public byte FirmwareMinor;
                public byte UpdateKey;
            }

            #endregion //Data Structures

            #region Constants

            public const int SCARD_S_SUCCESS = 0;
            public const int SCARD_ATR_LENGTH = 33;

            #region Memory Card Types

            public const int CT_MCU = 0x00;                   // MCU
            public const int CT_IIC_Auto = 0x01;               // IIC (Auto Detect Memory Size)
            public const int CT_IIC_1K = 0x02;                 // IIC (1K)
            public const int CT_IIC_2K = 0x03;                 // IIC (2K)
            public const int CT_IIC_4K = 0x04;                 // IIC (4K)
            public const int CT_IIC_8K = 0x05;                 // IIC (8K)
            public const int CT_IIC_16K = 0x06;                // IIC (16K)
            public const int CT_IIC_32K = 0x07;                // IIC (32K)
            public const int CT_IIC_64K = 0x08;                // IIC (64K)
            public const int CT_IIC_128K = 0x09;               // IIC (128K)
            public const int CT_IIC_256K = 0x0A;               // IIC (256K)
            public const int CT_IIC_512K = 0x0B;               // IIC (512K)
            public const int CT_IIC_1024K = 0x0C;              // IIC (1024K)
            public const int CT_AT88SC153 = 0x0D;              // AT88SC153
            public const int CT_AT88SC1608 = 0x0E;             // AT88SC1608
            public const int CT_SLE4418 = 0x0F;                // SLE4418
            public const int CT_SLE4428 = 0x10;                // SLE4428
            public const int CT_SLE4432 = 0x11;                // SLE4432
            public const int CT_SLE4442 = 0x12;                // SLE4442
            public const int CT_SLE4406 = 0x13;                // SLE4406
            public const int CT_SLE4436 = 0x14;                // SLE4436
            public const int CT_SLE5536 = 0x15;                // SLE5536
            public const int CT_MCUT0 = 0x16;                  // MCU T=0
            public const int CT_MCUT1 = 0x17;                  // MCU T=1
            public const int CT_MCU_Auto = 0x18;               // MCU Autodetect

            #endregion //Memory Card Types

            #region SCard Commands

            public const int SCARD_ATTR_VENDOR_NAME = 65792;
            public const int SCARD_ATTR_VENDOR_IFD_TYPE = 65793;
            public const int SCARD_ATTR_VENDOR_IFD_VERSION = 65794;
            public const int SCARD_ATTR_CHANNEL_ID = 131344;
            public const int SCARD_ATTR_DEFAULT_CLK = 196897;
            public const int SCARD_ATTR_DEFAULT_DATA_RATE = 196899;
            public const int SCARD_ATTR_MAX_CLK = 196898;
            public const int SCARD_ATTR_MAX_DATA_RATE = 196900;
            public const int SCARD_ATTR_MAX_IFSD = 196901;
            public const int SCARD_ATTR_ICC_PRESENCE = 590592;
            public const int SCARD_ATTR_ATR_STRING = 590595;
            public const int SCARD_ATTR_CURRENT_CLK = 524802;
            public const int SCARD_ATTR_CURRENT_F = 524803;
            public const int SCARD_ATTR_CURRENT_D = 524804;
            public const int SCARD_ATTR_CURRENT_N = 524805;
            public const int SCARD_ATTR_CURRENT_CWT = 524810;
            public const int SCARD_ATTR_CURRENT_BWT = 524809;
            public const int SCARD_ATTR_CURRENT_IFSC = 524807;
            #endregion //SCard Commands

            #region Context Scope

            /*===============================================================	
        ' Note: The context is a user context, and any database operations 
        '       are performed within the domain of the user.
        ===============================================================	*/
            public const int SCARD_SCOPE_USER = 0;

            /*===============================================================
            ' The context is that of the current terminal, and any database 
            'operations are performed within the domain of that terminal.  
            '(The calling application must have appropriate access permissions 
            'for any database actions.)
            '===============================================================*/
            public const int SCARD_SCOPE_TERMINAL = 1;

            /*===============================================================
            ' The context is the system context, and any database operations 
            ' are performed within the domain of the system.  (The calling
            ' application must have appropriate access permissions for any 
            ' database actions.)
            '===============================================================*/
            public const int SCARD_SCOPE_SYSTEM = 2;

            #endregion //Context Scope

            #region State

            /*===============================================================
        ' The application is unaware of the current state, and would like 
        ' to know. The use of this value results in an immediate return
        ' from state transition monitoring services. This is represented
        ' by all bits set to zero.
        '===============================================================*/
            public const int SCARD_STATE_UNAWARE = 0x00;

            /*===============================================================
            ' The application requested that this reader be ignored. No other
            ' bits will be set.
            '===============================================================*/
            public const int SCARD_STATE_IGNORE = 0x01;

            /*===============================================================
            ' This implies that there is a difference between the state 
            ' believed by the application, and the state known by the Service
            ' Manager.When this bit is set, the application may assume a
            ' significant state change has occurred on this reader.
            '===============================================================*/
            public const int SCARD_STATE_CHANGED = 0x02;

            /*===============================================================
            ' This implies that the given reader name is not recognized by
            ' the Service Manager. If this bit is set, then SCARD_STATE_CHANGED
            ' and SCARD_STATE_IGNORE will also be set.
            '===============================================================*/
            public const int SCARD_STATE_UNKNOWN = 0x04;

            /*===============================================================
            ' This implies that the actual state of this reader is not
            ' available. If this bit is set, then all the following bits are
            ' clear.
            '===============================================================*/
            public const int SCARD_STATE_UNAVAILABLE = 0x08;

            /*===============================================================
            '  This implies that there is no card in the reader. If this bit
            '  is set, all the following bits will be clear.
             ===============================================================*/
            public const int SCARD_STATE_EMPTY = 0x10;

            /*===============================================================
            '  This implies that there is a card in the reader.
             ===============================================================*/
            public const int SCARD_STATE_PRESENT = 0x20;

            /*===============================================================
            '  This implies that there is a card in the reader with an ATR
            '  matching one of the target cards. If this bit is set,
            '  SCARD_STATE_PRESENT will also be set.  This bit is only returned
            '  on the SCardLocateCard() service.
             ===============================================================*/
            public const int SCARD_STATE_ATRMATCH = 0x40;

            /*===============================================================
            '  This implies that the card in the reader is allocated for 
            '  exclusive use by another application. If this bit is set,
            '  SCARD_STATE_PRESENT will also be set.
             * ===============================================================*/
            public const int SCARD_STATE_EXCLUSIVE = 0x80;

            /*===============================================================
            '  This implies that the card in the reader is in use by one or 
            '  more other applications, but may be connected to in shared mode. 
            '  If this bit is set, SCARD_STATE_PRESENT will also be set.
             ===============================================================*/
            public const int SCARD_STATE_INUSE = 0x100;

            /*===============================================================
            '  This implies that the card in the reader is unresponsive or not
            '  supported by the reader or software.
            ' ===============================================================*/
            public const int SCARD_STATE_MUTE = 0x200;

            /*===============================================================
            '  This implies that the card in the reader has not been powered up.
            ' ===============================================================*/
            public const int SCARD_STATE_UNPOWERED = 0x400;

            /*===============================================================
            ' This application is not willing to share this card with other 
            'applications.
            '===============================================================*/
            public const int SCARD_SHARE_EXCLUSIVE = 1;

            /*===============================================================
            ' This application is willing to share this card with other 
            'applications.
            '===============================================================*/
            public const int SCARD_SHARE_SHARED = 2;

            /*===============================================================
            ' This application demands direct control of the reader, so it 
            ' is not available to other applications.
            '===============================================================*/
            public const int SCARD_SHARE_DIRECT = 3;

            #endregion //State

            #region Disposition

            public const int SCARD_LEAVE_CARD = 0;   // Don't do anything special on close
            public const int SCARD_RESET_CARD = 1;   // Reset the card on close
            public const int SCARD_UNPOWER_CARD = 2;   // Power down the card on close
            public const int SCARD_EJECT_CARD = 3;   // Eject the card on close

            #endregion //Disposition

            #region ACS IOCTL

            public const long FILE_DEVICE_SMARTCARD = 0x310000; // Reader action IOCTLs

            public const long IOCTL_SMARTCARD_DIRECT = FILE_DEVICE_SMARTCARD + 2050 * 4;
            public const long IOCTL_SMARTCARD_SELECT_SLOT = FILE_DEVICE_SMARTCARD + 2051 * 4;
            public const long IOCTL_SMARTCARD_DRAW_LCDBMP = FILE_DEVICE_SMARTCARD + 2052 * 4;
            public const long IOCTL_SMARTCARD_DISPLAY_LCD = FILE_DEVICE_SMARTCARD + 2053 * 4;
            public const long IOCTL_SMARTCARD_CLR_LCD = FILE_DEVICE_SMARTCARD + 2054 * 4;
            public const long IOCTL_SMARTCARD_READ_KEYPAD = FILE_DEVICE_SMARTCARD + 2055 * 4;
            public const long IOCTL_SMARTCARD_READ_RTC = FILE_DEVICE_SMARTCARD + 2057 * 4;
            public const long IOCTL_SMARTCARD_SET_RTC = FILE_DEVICE_SMARTCARD + 2058 * 4;
            public const long IOCTL_SMARTCARD_SET_OPTION = FILE_DEVICE_SMARTCARD + 2059 * 4;
            public const long IOCTL_SMARTCARD_SET_LED = FILE_DEVICE_SMARTCARD + 2060 * 4;
            public const long IOCTL_SMARTCARD_LOAD_KEY = FILE_DEVICE_SMARTCARD + 2062 * 4;
            public const long IOCTL_SMARTCARD_READ_EEPROM = FILE_DEVICE_SMARTCARD + 2065 * 4;
            public const long IOCTL_SMARTCARD_WRITE_EEPROM = FILE_DEVICE_SMARTCARD + 2066 * 4;
            public const long IOCTL_SMARTCARD_GET_VERSION = FILE_DEVICE_SMARTCARD + 2067 * 4;
            public const long IOCTL_SMARTCARD_GET_READER_INFO = FILE_DEVICE_SMARTCARD + 2051 * 4;
            public const long IOCTL_SMARTCARD_SET_CARD_TYPE = FILE_DEVICE_SMARTCARD + 2060 * 4;

            #endregion //ACS IOCTL

            #region Error Codes

            public const int SCARD_F_INTERNAL_ERROR = -2146435071;
            public const int SCARD_E_CANCELLED = -2146435070;
            public const int SCARD_E_INVALID_HANDLE = -2146435069;
            public const int SCARD_E_INVALID_PARAMETER = -2146435068;
            public const int SCARD_E_INVALID_TARGET = -2146435067;
            public const int SCARD_E_NO_MEMORY = -2146435066;
            public const int SCARD_F_WAITED_TOO_LONG = -2146435065;
            public const int SCARD_E_INSUFFICIENT_BUFFER = -2146435064;
            public const int SCARD_E_UNKNOWN_READER = -2146435063;


            public const int SCARD_E_TIMEOUT = -2146435062;
            public const int SCARD_E_SHARING_VIOLATION = -2146435061;
            public const int SCARD_E_NO_SMARTCARD = -2146435060;
            public const int SCARD_E_UNKNOWN_CARD = -2146435059;
            public const int SCARD_E_CANT_DISPOSE = -2146435058;
            public const int SCARD_E_PROTO_MISMATCH = -2146435057;


            public const int SCARD_E_NOT_READY = -2146435056;
            public const int SCARD_E_INVALID_VALUE = -2146435055;
            public const int SCARD_E_SYSTEM_CANCELLED = -2146435054;
            public const int SCARD_F_COMM_ERROR = -2146435053;
            public const int SCARD_F_UNKNOWN_ERROR = -2146435052;
            public const int SCARD_E_INVALID_ATR = -2146435051;
            public const int SCARD_E_NOT_TRANSACTED = -2146435050;
            public const int SCARD_E_READER_UNAVAILABLE = -2146435049;
            public const int SCARD_P_SHUTDOWN = -2146435048;
            public const int SCARD_E_PCI_TOO_SMALL = -2146435047;

            public const int SCARD_E_READER_UNSUPPORTED = -2146435046;
            public const int SCARD_E_DUPLICATE_READER = -2146435045;
            public const int SCARD_E_CARD_UNSUPPORTED = -2146435044;
            public const int SCARD_E_NO_SERVICE = -2146435043;
            public const int SCARD_E_SERVICE_STOPPED = -2146435042;

            public const int SCARD_W_UNSUPPORTED_CARD = -2146435041;
            public const int SCARD_W_UNRESPONSIVE_CARD = -2146435040;
            public const int SCARD_W_UNPOWERED_CARD = -2146435039;
            public const int SCARD_W_RESET_CARD = -2146435038;
            public const int SCARD_W_REMOVED_CARD = -2146435037;

            #endregion //Error Codes

            #region Protocol

            public const int SCARD_PROTOCOL_UNDEFINED = 0x00;          // There is no active protocol.
            public const int SCARD_PROTOCOL_T0 = 0x01;                 // T=0 is the active protocol.
            public const int SCARD_PROTOCOL_T1 = 0x02;                 // T=1 is the active protocol.
            public const int SCARD_PROTOCOL_RAW = 0x04;
            public const int SCARD_PROTOCOL_T15 = 0x0008;
            //public const int SCARD_PROTOCOL_RAW = 0x00010000;         // Raw is the active protocol.
            public const uint SCARD_PROTOCOL_DEFAULT = 0x80000000;      // Use implicit PTS.

            #endregion //Protocol

            #region Reader State

            /*===============================================================
        ' This value implies the driver is unaware of the current 
        ' state of the reader.
        '===============================================================*/
            public const int SCARD_UNKNOWN = 0;

            /*===============================================================
            ' This value implies there is no card in the reader.
            '===============================================================*/
            public const int SCARD_ABSENT = 1;

            /*===============================================================
            ' This value implies there is a card is present in the reader, 
            'but that it has not been moved into position for use.
            '===============================================================*/
            public const int SCARD_PRESENT = 2;

            /*===============================================================
            ' This value implies there is a card in the reader in position 
            ' for use.  The card is not powered.
            '===============================================================*/
            public const int SCARD_SWALLOWED = 3;

            /*===============================================================
            ' This value implies there is power is being provided to the card, 
            ' but the Reader Driver is unaware of the mode of the card.
            '===============================================================*/
            public const int SCARD_POWERED = 4;

            /*===============================================================
            ' This value implies the card has been reset and is awaiting 
            ' PTS negotiation.
            '===============================================================*/
            public const int SCARD_NEGOTIABLE = 5;

            /*===============================================================
            ' This value implies the card has been reset and specific 
            ' communication protocols have been established.
            '===============================================================*/
            public const int SCARD_SPECIFIC = 6;

            #endregion //Reader State

            #endregion //Constants

            #region Kernel32 Dll Imports
            /**************************************************************************************************
        * Purpose: Importation of required kernel32.dll functions 
        * 
        * Parameters: None
        * 
        * Returns: None
        * 
        * History:
        * Date             Who             Comment
        * 12/13/2010       ACT             Functions creation.
        ***************************************************************************************************/
            [DllImport("kernel32.dll")]
            private extern static IntPtr LoadLibrary(string fileName);

            [DllImport("kernel32.dll")]
            private extern static void FreeLibrary(IntPtr handle);

            [DllImport("kernel32.dll")]
            private extern static IntPtr GetProcAddress(IntPtr handle, string functionName);

            #endregion //Kernel32 Dll Imports

            #region WinSCard Dll Imports
            /**************************************************************************************************
        * Purpose: Importation of required winscard.dll functions 
        * 
        * Parameters: None
        * 
        * Returns: None
        * 
        * History:
        * Date             Who             Comment
        * 12/13/2010       ACT             Functions creation.
        ***************************************************************************************************/
            [DllImport("winscard.dll")]
            public static extern int SCardEstablishContext(uint dwScope, int pvReserved1, int pvReserved2, ref int phContext);

            [DllImport("winscard.dll")]
            public static extern int SCardReleaseContext(int phContext);

            [DllImport("winscard.dll")]
            public static extern int SCardConnect(int hContext, string szReaderName, uint dwShareMode, uint dwPrefProtocol, ref int phCard, ref uint ActiveProtocol);

            [DllImport("winscard.dll")]
            public static extern int SCardReconnect(int hContext, uint dwShareMode, uint dwPrefProtocols, uint dwInitialization, ref uint ActiveProtocol);

            [DllImport("winscard.dll")]
            public static extern int SCardBeginTransaction(int hCard);

            [DllImport("winscard.dll")]
            public static extern int SCardDisconnect(int hCard, int Disposition);

            [DllImport("winscard.dll")]
            public static extern int SCardListReaderGroups(int hContext, ref string mzGroups, ref int pcchGroups);

            [DllImport("winscard.DLL", EntryPoint = "SCardListReadersA", CharSet = CharSet.Ansi)]
            public static extern int SCardListReaders(int hContext, byte[] Groups, byte[] Readers,
                                                      ref int pcchReaders);

            [DllImport("winscard.dll")]
            public static extern int SCardStatus(int hCard, string szReaderName, ref int pcchReaderLen, ref int State, ref int Protocol, ref byte ATR, ref int ATRLen);

            [DllImport("winscard.dll")]
            private static extern int SCardStatus(int hCard, string szReaderName, ref int pcchReaderLen, ref int State, ref uint Protocol, IntPtr ATR, ref int ATRLen);

            /**************************************************************************************************
            * Function Name: SCardStatusEx
            * 
            * Purpose: To create a public wrapper method for SCardStatus API. This method also provides 
            *          unmanaged-to-managed & managed-to-unmanaged marshalling.
            *           
            * 
            * Parameters:       hCard = int containing the handle to the smartcard
            *            szReaderName = string containing the smartcard reader name
            *           pcchReaderLen = int containing the length of the reader name in bytes
            *                   state = int containing the reader connection state returned
            *                Protocol = unsigned int containing the reader connection protocol returned
            *                     ATR = byte array containing the answer to reset response returned
            *                  ATRLen = int containing the length in bytes of the answer to reset response  
            *                 
            * Returns: 0 = Success
            *          A non-Zero responses is a PC/SC error code
            * 
            * History:
            * Date             Who             Comment
            * 12/13/2010       ACT             Function creation.
            ***************************************************************************************************/
            public static int SCardStatusEx(int hCard, string szReaderName, ref int pcchReaderLen, ref int State, ref uint Protocol, ref byte[] ATR, ref int ATRLen)
            {
                IntPtr pnt = IntPtr.Zero;
                int result = -1;
                try
                {
                    // Initialize unmanaged memory to hold the array.
                    int size = Marshal.SizeOf(ATR[0]) * ATR.Length;

                    pnt = Marshal.AllocHGlobal(size);

                    // Copy the array to unmanaged memory.
                    Marshal.Copy(ATR, 0, pnt, ATR.Length);

                    result = SCardStatus(hCard, szReaderName, ref pcchReaderLen, ref State, ref Protocol, pnt, ref size);

                    // Copy the unmanaged array back.
                    Marshal.Copy(pnt, ATR, 0, size);
                    ATRLen = size;

                }
                finally
                {
                    // Free the unmanaged memory.
                    Marshal.FreeHGlobal(pnt);
                }
                return result;
            }

            [DllImport("winscard.dll")]
            public static extern int SCardEndTransaction(int hCard, int Disposition);

            [DllImport("winscard.dll")]
            public static extern int SCardState(int hCard, ref uint State, ref uint Protocol, ref byte ATR, ref uint ATRLen);

            [DllImport("winscard.dll")]
            public static extern int SCardTransmit(int hCard, ref SCARD_IO_REQUEST pioSendRequest, ref byte SendBuff, int SendBuffLen, ref SCARD_IO_REQUEST pioRecvRequest, ref byte RecvBuff, ref int RecvBuffLen);

            [DllImport("winscard.dll")]
            private static extern int SCardTransmit(int hCard, ref SCARD_IO_REQUEST pioSendRequest, IntPtr SendBuff, int SendBuffLen, ref SCARD_IO_REQUEST pioRecvRequest, IntPtr RecvBuff, ref int RecvBuffLen);

            /**************************************************************************************************
            * Function Name: SCardTransmitEx
            * 
            * Purpose: To create a public wrapper method for SCardTransmit API. This method also provides 
            *          unmanaged-to-managed & managed-to-unmanaged marshalling.
            *           
            * 
            * Parameters:        hCard = int containing the handle to the smartcard
            *           pioSendRequest = struct containing the required parameters for the transmit request
            *                 SendBuff = byte array containing the data to be transmitted to the card
            *              SendBuffLen = int containing the number of bytes in SendBuff
            *           pioRecvRequest = struct containing the response to the transmit request
            *                 RecvBuff = byte array containing the response to be transmitted data
            *              RecvBuffLen = int containing the number of bytes in RecvBuff  
            *                 
            * Returns: 0 = Success
            *          A non-Zero responses is a PC/SC error code
            * 
            * History:
            * Date             Who             Comment
            * 12/13/2010       ACT             Function creation.
            ***************************************************************************************************/
            public static int SCardTransmitEx(int hCard, ref SCARD_IO_REQUEST pioSendRequest, ref byte[] SendBuff, int SendBuffLen, ref SCARD_IO_REQUEST pioRecvRequest, ref byte[] RecvBuff, ref int RecvBuffLen)
            {
                int SendSize = 0;
                int RecSize = 0;
                IntPtr Sendpnt = IntPtr.Zero;
                IntPtr Recpnt = IntPtr.Zero;

                int result = -1;

                try
                {
                    if (SendBuff != null)
                    {
                        SendSize = Marshal.SizeOf(SendBuff[0]) * SendBuffLen;
                    }
                    Sendpnt = Marshal.AllocHGlobal(SendSize);

                    if (RecvBuff != null)
                    {
                        RecSize = Marshal.SizeOf(RecvBuff[0]) * RecvBuffLen;
                    }
                    Recpnt = Marshal.AllocHGlobal(RecSize);


                    // Copy the array to unmanaged memory.
                    if (SendBuff != null)
                        Marshal.Copy(SendBuff, 0, Sendpnt, SendBuffLen);

                    if (RecvBuff != null)
                        Marshal.Copy(RecvBuff, 0, Recpnt, RecvBuffLen);

                    result = SCardTransmit(hCard, ref pioSendRequest, Sendpnt, SendSize, ref pioRecvRequest,
                                           Recpnt, ref RecSize);

                    // Copy the unmanaged array back.
                    Marshal.Copy(Recpnt, RecvBuff, 0, RecSize);
                    RecvBuffLen = RecSize;
                }
                finally
                {
                    // Free the unmanaged memory.
                    Marshal.FreeHGlobal(Sendpnt);
                    Marshal.FreeHGlobal(Recpnt);
                }
                return result;
            }

            [DllImport("winscard.dll")]
            private static extern int SCardControl(int hCard, uint dwControlCode, IntPtr SendBuff, int SendBuffLen, ref VERSION_CONTROL RecvBuff, int RecvBuffLen, ref int pcbBytesReturned);

            /**************************************************************************************************
            * Function Name: SCardControlEx
            * 
            * Purpose: To create a public wrapper method for SCardControl API. This method also provides 
            *          unmanaged-to-managed & managed-to-unmanaged marshalling.
            *           
            * 
            * Parameters:        hCard = int containing the handle to the smartcard
            *            dwControlCode = unsigned int containing the control code
            *                 SendBuff = byte array containing the data to be transmitted to the card
            *              SendBuffLen = int containing the number of bytes in SendBuff
            *                 RecvBuff = struct containing the smartcard reader version information
            *              RecvBuffLen = int containing the number of bytes in RecvBuff  
            *         pcbBytesReturned = int containing the number of bytes returned 
            *                 
            * Returns: 0 = Success
            *          A non-Zero responses is a PC/SC error code
            * 
            * History:
            * Date             Who             Comment
            * 12/13/2010       ACT             Function creation.
            ***************************************************************************************************/
            public static int SCardControlEx(int hCard, uint dwControlCode, ref byte[] SendBuff, int SendBuffLen, ref VERSION_CONTROL RecvBuff, int RecvBuffLen, ref int pcbBytesReturned)
            {
                int SendSize = 0;
                int RecSize = 0;
                IntPtr Sendpnt = IntPtr.Zero;

                int result = -1;

                try
                {
                    if (SendBuff != null)
                    {
                        SendSize = Marshal.SizeOf(SendBuff[0]) * SendBuffLen;
                    }
                    Sendpnt = Marshal.AllocHGlobal(SendSize);

                    // Copy the array to unmanaged memory.
                    if (SendBuff != null)
                        Marshal.Copy(SendBuff, 0, Sendpnt, SendBuffLen);


                    result = SCardControl(hCard, dwControlCode, Sendpnt, SendSize,
                                          ref RecvBuff, RecvBuffLen, ref RecSize);

                    pcbBytesReturned = RecSize;
                }
                finally
                {
                    // Free the unmanaged memory.
                    Marshal.FreeHGlobal(Sendpnt);
                }
                return result;
            }

            [DllImport("winscard.dll")]
            public static extern int SCardControl(int hCard, uint dwControlCode, IntPtr SendBuff, int SendBuffLen, IntPtr RecvBuff, int RecvBuffLen, ref int pcbBytesReturned);

            /**************************************************************************************************
            * Function Name: SCardTransmitEx
            * 
            * Purpose: To create a public wrapper method for SCardTransmit API. This method also provides 
            *          unmanaged-to-managed & managed-to-unmanaged marshalling.
            *           
            * 
            * Parameters:        hCard = int containing the handle to the smartcard
            *            dwControlCode = unsigned int containing the control code
            *                 SendBuff = byte array containing the data to be transmitted to the card
            *              SendBuffLen = int containing the number of bytes in SendBuff
            *                 RecvBuff = byte array containing the response to be transmitted data
            *              RecvBuffLen = int containing the number of bytes in RecvBuff  
            *         pcbBytesReturned = int containing the number of bytes returned
            *                 
            * Returns: 0 = Success
            *          A non-Zero responses is a PC/SC error code
            * 
            * History:
            * Date             Who             Comment
            * 12/13/2010       ACT             Function creation.
            ***************************************************************************************************/
            public static int SCardControlEx(int hCard, uint dwControlCode, ref byte[] SendBuff, int SendBuffLen, ref byte[] RecvBuff, int RecvBuffLen, ref int pcbBytesReturned)
            {
                int SendSize = 0;
                int RecSize = 0;
                IntPtr Sendpnt = IntPtr.Zero;
                IntPtr Recpnt = IntPtr.Zero;
                int result = -1;
                try
                {
                    if (SendBuff != null)
                    {
                        SendSize = Marshal.SizeOf(SendBuff[0]) * SendBuffLen;
                    }
                    Sendpnt = Marshal.AllocHGlobal(SendSize);

                    if (RecvBuff != null)
                    {
                        RecSize = Marshal.SizeOf(RecvBuff[0]) * RecvBuffLen;
                    }
                    Recpnt = Marshal.AllocHGlobal(RecSize);

                    // Copy the array to unmanaged memory.
                    if (SendBuff != null)
                        Marshal.Copy(SendBuff, 0, Sendpnt, SendBuffLen);

                    if (RecvBuff != null)
                        Marshal.Copy(RecvBuff, 0, Recpnt, RecvBuffLen);

                    result = SCardControl(hCard, dwControlCode, Sendpnt, SendBuffLen,
                                          Recpnt, RecvBuffLen, ref  RecSize);

                    // Copy the unmanaged array back.
                    Marshal.Copy(Recpnt, RecvBuff, 0, RecSize);
                    pcbBytesReturned = RecSize;
                }
                finally
                {
                    // Free the unmanaged memory.
                    Marshal.FreeHGlobal(Sendpnt);
                    Marshal.FreeHGlobal(Recpnt);
                }
                return result;
            }

            [DllImport("winscard.dll")]
            private static extern int SCardGetAttrib(int hCard, uint dwAttrId, IntPtr pbAttr, ref int pcbAttrLen);

            /**************************************************************************************************
            * Function Name: SCardGetAttribEx
            * 
            * Purpose: To create a public wrapper method for SCardGetAttrib API. This method also provides 
            *          unmanaged-to-managed & managed-to-unmanaged marshalling.
            *           
            * 
            * Parameters:        hCard = int containing the handle to the smartcard
            *               dwAttribId = unsigned int containing the attribute code
            *                   pbAttr = byte array containing the attribute returend
            *               pcbAttrLen = int containing the number of bytes in pbAttr
            *                                
            * Returns: 0 = Success
            *          A non-Zero responses is a PC/SC error code
            * 
            * History:
            * Date             Who             Comment
            * 12/13/2010       ACT             Function creation.
            ***************************************************************************************************/
            public static int SCardGetAttribEx(int hCard, uint dwAttrId, ref byte[] pbAttr, ref int pcbAttrLen)
            {
                int size = 0;
                int result = -1;
                IntPtr pnt = IntPtr.Zero;

                try
                {
                    size = Marshal.SizeOf(pbAttr[0]) * pcbAttrLen;
                    pnt = Marshal.AllocHGlobal(size);

                    // Copy the array to unmanaged memory.
                    Marshal.Copy(pbAttr, 0, pnt, pcbAttrLen);

                    result = SCardGetAttrib(hCard, dwAttrId, pnt, ref size);

                    // Copy the unmanaged array back.
                    Marshal.Copy(pnt, pbAttr, 0, size);
                    pcbAttrLen = size;
                }
                finally
                {
                    // Free the unmanaged memory.
                    Marshal.FreeHGlobal(pnt);
                }
                return result;
            }
            #endregion WinSCard Dll Imports

            #region WinsCard.dll Wrapper Functions
            /**************************************************************************************************
        * Purpose: Public wrappers for imported winscard.dll functions 
        * 
        * Parameters: None
        * 
        * Returns: None
        * 
        * History:
        * Date             Who             Comment
        * 12/13/2010       ACT             Functions creation.
        ***************************************************************************************************/
            public int SCardEstablishContextWrapper(uint dwScope, int pvReserved1, int pvReserved2, ref int phContext)
            {
                return SCardEstablishContext(dwScope, pvReserved1, pvReserved2, ref phContext);
            }

            public int SCardReleaseContextWrapper(int phContext)
            {
                return SCardReleaseContext(phContext);
            }

            public int SCardConnectWrapper(int hContext, string szReaderName, uint dwShareMode, uint dwPrefProtocol, ref int phCard, ref uint ActiveProtocol)
            {
                return SCardConnect(hContext, szReaderName, dwShareMode, dwPrefProtocol, ref phCard, ref ActiveProtocol);
            }

            public int SCardBeginTransactionWrapper(int hCard)
            {
                return SCardBeginTransaction(hCard);
            }

            public int SCardDisconnectWrapper(int hCard, int Disposition)
            {
                return SCardDisconnect(hCard, Disposition);
            }

            public int SCardListReaderGroupsWrapper(int hContext, ref string mzGroups, ref int pcchGroups)
            {
                return SCardListReaderGroups(hContext, ref mzGroups, ref pcchGroups);
            }

            public int SCardListReadersWrapper(int hContext, byte[] Groups, byte[] Readers,
                                               ref int pcchReaders)
            {
                return SCardListReaders(hContext, Groups, Readers, ref pcchReaders);
            }

            public int SCardStatusWrapper(int hCard, string szReaderName, ref int pcchReaderLen, ref int State, ref int Protocol, ref byte ATR, ref int ATRLen)
            {
                return SCardStatus(hCard, szReaderName, ref pcchReaderLen, ref State, ref Protocol, ref ATR, ref ATRLen);
            }

            public int SCardEndTransactionWrapper(int hCard, int Disposition)
            {
                return SCardEndTransaction(hCard, Disposition);
            }

            public int SCardStateWrapper(int hCard, ref uint State, ref uint Protocol, ref byte ATR, ref uint ATRLen)
            {
                return SCardState(hCard, ref State, ref Protocol, ref ATR, ref ATRLen);
            }

            public int SCardTransmitWrapper(int hCard, ref SCARD_IO_REQUEST pioSendRequest, ref byte SendBuff, int SendBuffLen, ref SCARD_IO_REQUEST pioRecvRequest, ref byte RecvBuff, ref int RecvBuffLen)
            {
                return SCardTransmit(hCard, ref pioSendRequest, ref SendBuff, SendBuffLen, ref pioRecvRequest, ref RecvBuff, ref RecvBuffLen);
            }

            public int SCardControlWrapper(int hCard, uint dwControlCode, ref byte[] SendBuff, int SendBuffLen, ref byte[] RecvBuff, int RecvBuffLen, ref int pcbBytesReturned)
            {
                return SCardControlEx(hCard, dwControlCode, ref SendBuff, SendBuffLen, ref RecvBuff, RecvBuffLen, ref pcbBytesReturned);
            }

            public int SCardGetAttribWrapper(int hCard, uint dwAttrId, ref byte[] pbAttr, ref int pcbAttrLen)
            {
                return SCardGetAttribEx(hCard, dwAttrId, ref pbAttr, ref pcbAttrLen);
            }
            #endregion //WinsCard.dll Wrapper Functions

            #region PCI Address

            /**************************************************************************************************
        * Function Name: GetPciT0
        * 
        * Purpose: To retrieve the T0 PCI address from Winscard.dll.
        *           
        * 
        * Parameters:   None
        *                                
        * Returns: Success = Address to PCI
        *          Failure = null handle
        * 
        * History:
        * Date             Who             Comment
        * 12/13/2010       ACT             Function creation.
        ***************************************************************************************************/
            private static IntPtr GetPciT0()
            {
                IntPtr handle = IntPtr.Zero;
                IntPtr pci = IntPtr.Zero;
                try
                {
                    handle = LoadLibrary("Winscard.dll");
                    pci = GetProcAddress(handle, "g_rgSCardT0Pci");
                }
                catch
                {
                    pci = IntPtr.Zero;
                }
                finally
                {
                    FreeLibrary(handle);
                }
                return pci;
            }

            /**************************************************************************************************
            * Function Name: GetPciT1
            * 
            * Purpose: To retrieve the T1 PCI address from Winscard.dll.
            *           
            * 
            * Parameters:   None
            *                                
            * Returns: Success = Address to PCI
            *          Failure = null handle
            * 
            * History:
            * Date             Who             Comment
            * 12/13/2010       ACT             Function creation.
            ***************************************************************************************************/
            private static IntPtr GetPciT1()
            {
                IntPtr handle = IntPtr.Zero;
                IntPtr pci = IntPtr.Zero;
                try
                {
                    handle = LoadLibrary("Winscard.dll");
                    pci = GetProcAddress(handle, "g_rgSCardT1Pci");
                }
                catch
                {
                    pci = IntPtr.Zero;
                }
                finally
                {
                    FreeLibrary(handle);
                }
                return pci;
            }

            /**************************************************************************************************
            * Function Name: GetPciRaw
            * 
            * Purpose: To retrieve the Raw PCI address from Winscard.dll.
            *           
            * 
            * Parameters:   None
            *                                
            * Returns: Success = Address to PCI
            *          Failure = null handle
            * 
            * History:
            * Date             Who             Comment
            * 12/13/2010       ACT             Function creation.
            ***************************************************************************************************/
            private static IntPtr GetPciRaw()
            {
                IntPtr handle = IntPtr.Zero;
                IntPtr pci = IntPtr.Zero;
                try
                {
                    handle = LoadLibrary("Winscard.dll");
                    pci = GetProcAddress(handle, "g_rgSCardRawPci");
                }
                catch
                {
                    pci = IntPtr.Zero;
                }
                finally
                {
                    FreeLibrary(handle);
                }
                return pci;
            }

            #endregion //PCI Address

            #region Get Protocol
            /**************************************************************************************************
        * Function Name: GetProtocolStructure
        * 
        * Purpose: To retrieve the appropriate PCI address from Winscard.dll.
        *           
        * 
        * Parameters:   protocol = int containing the protocol in use
        *                                
        * Returns: Success = Address to PCI for the protocol in use
        *          Failure = null handle
        * 
        * History:
        * Date             Who             Comment
        * 12/13/2010       ACT             Function creation.
        ***************************************************************************************************/
            public static IntPtr GetProtocolStructure(int protocol)
            {
                try
                {
                    if (protocol == 1)
                        return GetPciT0();

                    else if ((protocol == 2) || (protocol == 3))
                        return GetPciT1();

                    else
                        return GetPciRaw();
                }
                catch
                {
                }
                return IntPtr.Zero;
            }

            /**************************************************************************************************
            * Function Name: GetProtocolStructureEx
            * 
            * Purpose: To provide a public wrapper for the static function GetProtocolStructure.
            *           
            * 
            * Parameters:   protocol = int containing the protocol in use
            *                                
            * Returns: Success = Address to PCI for the protocol in use
            *          Failure = null handle
            * 
            * History:
            * Date             Who             Comment
            * 12/13/2010       ACT             Function creation.
            ***************************************************************************************************/
            public IntPtr GetProtocolStructureEx(int protocol)
            {
                return GetProtocolStructure(protocol);
            }
            #endregion //Get Protocol

            #region Public Methods

            /**************************************************************************************************
        * Function Name: GetPCSReaders
        * 
        * Purpose: To retrieve the contactless & contact smartcard reader names from windows.
        *           
        * 
        * Parameters:   contactlessReader = string containing the name of the contactless reader
        *                   contactReader = string containing the name of the contact reader
        *                          errMsg = string containing an error message if an error is encountered 
        *                                
        * Returns: Success = 0
        *          Failure = PC/SC error code
        * 
        * History:
        * Date             Who             Comment
        * 12/13/2010       ACT             Function creation.
        ***************************************************************************************************/
            public static int GetPCSCReaders(out string contactlessReader, out string contactReader,
                                             out string errMsg)
            {
                contactlessReader = contactReader = errMsg = "";
                byte[] readersList = null;
                try
                {
                    int context = 0;
                    int ret = SCardEstablishContext(WinSCard.SCARD_SCOPE_USER, 0, 0, ref context);
                    if (ret != 0)
                    {
                        errMsg = "WinSCard: GetPCSCReader: EstablishContext Error: " + ret.ToString();
                        return ret;
                    }

                    int byteCnt = 0;
                    ret = WinSCard.SCardListReaders(context, null, null, ref byteCnt);
                    if (ret != SCARD_S_SUCCESS)
                    {
                        errMsg = "WinSCard: GetPCSCReader: ListReaders Error: " + ret.ToString();
                        return ret;
                    }

                    readersList = new byte[byteCnt];
                    ret = WinSCard.SCardListReaders(context, null, readersList, ref byteCnt);
                    if (ret != SCARD_S_SUCCESS)
                    {
                        errMsg = "WinSCard: GetPCSCReader: ListReaders Error: " + ret.ToString();
                        return ret;
                    }

                    int indx = 0;
                    string readerName = "";

                    while (readersList[indx] != 0)
                    {
                        while (readersList[indx] != 0)
                        {
                            readerName = readerName + (char)readersList[indx++];
                        }

                        if (readerName.Contains("ACR38U") || readerName.Contains("ACS") || readerName.Contains("SDI010 Smart Card Reader"))
                            contactReader = readerName;

                        else if (readerName.Contains("SDI010 Contactless"))
                            contactlessReader = readerName;

                        readerName = "";
                        indx++;
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
                finally
                {
                    readersList = null;
                }
                return 0;
            }

            /**************************************************************************************************
            * Function Name: GetSCardErrMsg
            * 
            * Purpose: To retrieve the error message string for a specific error code.
            *      
            * 
            * Parameters:   ReturnCode = int containing the error code
            *                    
            *                                
            * Returns: Success = string containing the error message
            *          Failure = Exception message
            * 
            * History:
            * Date             Who             Comment
            * 12/13/2010       ACT             Function creation.
            ***************************************************************************************************/
            public static string GetSCardErrMsg(int ReturnCode)
            {
                string errMsg = string.Empty;
                try
                {
                    switch (ReturnCode)
                    {
                        case SCARD_E_CANCELLED:
                            return ("The action was canceled by an SCardCancel request.");
                        case SCARD_E_CANT_DISPOSE:
                            return ("The system could not dispose of the media in the requested manner.");
                        case SCARD_E_CARD_UNSUPPORTED:
                            return ("The smart card does not meet minimal requirements for support.");
                        case SCARD_E_DUPLICATE_READER:
                            return ("The reader driver didn't produce a unique reader name.");
                        case SCARD_E_INSUFFICIENT_BUFFER:
                            return ("The data buffer for returned data is too small for the returned data.");
                        case SCARD_E_INVALID_ATR:
                            return ("An ATR string obtained from the registry is not a valid ATR string.");
                        case SCARD_E_INVALID_HANDLE:
                            return ("The supplied handle was invalid.");
                        case SCARD_E_INVALID_PARAMETER:
                            return ("One or more of the supplied parameters could not be properly interpreted.");
                        case SCARD_E_INVALID_TARGET:
                            return ("Registry startup information is missing or invalid.");
                        case SCARD_E_INVALID_VALUE:
                            return ("One or more of the supplied parameter values could not be properly interpreted.");
                        case SCARD_E_NOT_READY:
                            return ("The reader or card is not ready to accept commands.");
                        case SCARD_E_NOT_TRANSACTED:
                            return ("An attempt was made to end a non-existent transaction.");
                        case SCARD_E_NO_MEMORY:
                            return ("Not enough memory available to complete this command.");
                        case SCARD_E_NO_SERVICE:
                            return ("The smart card resource manager is not running.");
                        case SCARD_E_NO_SMARTCARD:
                            return ("The operation requires a smart card, but no smart card is currently in the device.");
                        case SCARD_E_PCI_TOO_SMALL:
                            return ("The PCI receive buffer was too small.");
                        case SCARD_E_PROTO_MISMATCH:
                            return ("The requested protocols are incompatible with the protocol currently in use with the card.");
                        case SCARD_E_READER_UNAVAILABLE:
                            return ("The specified reader is not currently available for use.");
                        case SCARD_E_READER_UNSUPPORTED:
                            return ("The reader driver does not meet minimal requirements for support.");
                        case SCARD_E_SERVICE_STOPPED:
                            return ("The smart card resource manager has shut down.");
                        case SCARD_E_SHARING_VIOLATION:
                            return ("The smart card cannot be accessed because of other outstanding connections.");
                        case SCARD_E_SYSTEM_CANCELLED:
                            return ("The action was canceled by the system, presumably to log off or shut down.");
                        case SCARD_E_TIMEOUT:
                            return ("The user-specified timeout value has expired.");
                        case SCARD_E_UNKNOWN_CARD:
                            return ("The specified smart card name is not recognized.");
                        case SCARD_E_UNKNOWN_READER:
                            return ("The specified reader name is not recognized.");
                        case SCARD_F_COMM_ERROR:
                            return ("An internal communications error has been detected.");
                        case SCARD_F_INTERNAL_ERROR:
                            return ("An internal consistency check failed.");
                        case SCARD_F_UNKNOWN_ERROR:
                            return ("An internal error has been detected, but the source is unknown.");
                        case SCARD_F_WAITED_TOO_LONG:
                            return ("An internal consistency timer has expired.");
                        case SCARD_S_SUCCESS:
                            return ("No error was encountered.");
                        case SCARD_W_REMOVED_CARD:
                            return ("The smart card has been removed, so that further communication is not possible.");
                        case SCARD_W_RESET_CARD:
                            return ("The smart card has been reset, so any shared state information is invalid.");
                        case SCARD_W_UNPOWERED_CARD:
                            return ("Power has been removed from the smart card, so that further communication is not possible.");
                        case SCARD_W_UNRESPONSIVE_CARD:
                            return ("The smart card is not responding to a reset.");
                        case SCARD_W_UNSUPPORTED_CARD:
                            return ("The reader cannot communicate with the card, due to ATR string configuration conflicts.");
                        default:
                            return ("?");
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
                return errMsg;
            }
            #endregion //Public Methods
        }
    }
}
