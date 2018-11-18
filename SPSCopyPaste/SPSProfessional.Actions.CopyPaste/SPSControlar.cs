using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;

namespace SPSProfessional.Actions.CopyPaste
{
    internal sealed class SPSControlar
    {
        private readonly string _productID;
        private readonly string _productVersion;
        public const string C_REGKEY = @"SOFTWARE\spsprofessional";

        #region         public Controlar(string ProductID, string ProductVersion)

        public SPSControlar(string ProductID, string ProductVersion)
        {
            _productID = ProductID.Trim();
            _productVersion = ProductVersion.Trim().Replace(".", string.Empty);
        }

        #endregion

        #region         public bool Aceptado()

        public bool Aceptado()
        {
            bool blnReturn = false;
            bool FileFound = false;
            bool FileCorrectlyDecrypted = false;
            bool FileNameCorrect = false;
            bool LicenseGoodValidated = false;
            List<string> myMACs = new List<string>();
            List<string> myIPs = new List<string>();
            List<string> myDomains = new List<string>();
            DatosFile myFileData = new DatosFile();
            TextReader myTextReader = null;

            //#region FARM

            //try
            //{
            //    IPHostEntry host = Dns.GetHostEntry("10.64.136.90");
            //    if (!string.IsNullOrEmpty(host.HostName))
            //        return true;
            //    return false;
            //}
            //catch (SocketException ex)
            //{
            //    return false;
            //}

            //#endregion

            try
            {
                //Debug.WriteLine("** Begin Comprobation - " + DateTime.Now.Millisecond);

                //Search for spsproparts.lic file in %windows32% directory
                FileFound = EncontrarFile(ref myTextReader);

                if (FileFound)
                {
                    //Try to decrypt the file, checks that everything is correct and fill the Object with the values
                    FileCorrectlyDecrypted = LeerFile(myTextReader, ref myFileData);

                    if (FileCorrectlyDecrypted)
                    {
                        //Test code
                        //Debug.WriteLine("ProductName - " + myFileData.ProductName);
                        //Debug.WriteLine("ProductID - " + myFileData.ProductID);
                        //Debug.WriteLine("ProductVersion - " + myFileData.ProductVersion);
                        //Debug.WriteLine("LicenseID - " + myFileData.LicenseID);
                        //Debug.WriteLine("RawData - " + myFileData.RawData);
                        //Debug.WriteLine("EngineVersion - " + myFileData.LicenseEngineVersion);
                        //Debug.WriteLine("LicenseDate - " + myFileData.LicenseDate);
                        //Debug.WriteLine("LicenseSort - " + myFileData.LicenseSort);
                        //Debug.WriteLine("EvaluationDate - " + myFileData.DateExpiration);
                        //Debug.WriteLine("EvaluationDays - " + myFileData.DaysToExpiration);
                        //Debug.WriteLine("MAC - " + myFileData.MacAddress);
                        //Debug.WriteLine("IP - " + myFileData.IpAddress);
                        //Debug.WriteLine("Domain - " + myFileData.DomainName);
                        //Debug.WriteLine("ClientName - " + myFileData.ClientName);
                        //Debug.WriteLine("Other - " + myFileData.OtherInformation);

                        //The name of the file need to be consequent with the parameters inside the file
                        FileNameCorrect = ControlNombre(myFileData);

                        if (FileNameCorrect)
                        {
                            //Based in the type license, check if the license is correct
                            switch (myFileData.LicenseSort)
                            {
                                case DatosFile.TypeLicense.EvaluationDate:
                                    LicenseGoodValidated = IsInsideTimeAbsolute(myFileData.DateExpiration);
                                    break;
                                case DatosFile.TypeLicense.EvaluationDays:
                                    LicenseGoodValidated = IsInsideTimeRelative(myFileData);
                                    break;
                                case DatosFile.TypeLicense.MAC:
                                    myMACs = DireccionesM();
                                    LicenseGoodValidated = FoundInList(myMACs, myFileData.MacAddress);
                                    break;
                                case DatosFile.TypeLicense.IP:
                                    myIPs = DireccionesI();
                                    LicenseGoodValidated = FoundInList(myIPs, myFileData.IpAddress);
                                    break;
                                case DatosFile.TypeLicense.Domain:
                                    myDomains = Dominios();
                                    LicenseGoodValidated = FoundInList(myDomains, myFileData.DomainName.ToUpper());
                                    break;
                                case DatosFile.TypeLicense.Host:                                    
                                    LicenseGoodValidated = (Dns.GetHostName().ToLower()  == myFileData.HostName.ToLower());
                                    break;
                                default:
                                    LicenseGoodValidated = false;
                                    break;
                            }

                            blnReturn = LicenseGoodValidated;
                        }
                    }
                }

                Debug.WriteLine("** End Comprobation - " + DateTime.Now.Millisecond);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("BuscarControlar:Controlar.Aceptado - " + ex);
            }

            return blnReturn;
        }

        #endregion

        //Search for the License File

        #region         private bool EncontrarFile(ref TextReader myTextReader)

        private bool EncontrarFile(ref TextReader myTextReader)
        {
            bool blnReturn = false;

            try
            {
                string myFilePath = Environment.GetFolderPath(Environment.SpecialFolder.System) +
                                    @"\spsprofessional_" + _productID + "_" + _productVersion + ".lic";

                if (File.Exists(myFilePath))
                {
                    myTextReader = new StreamReader(myFilePath);
                    blnReturn = true;
                }
            }
            catch (FileNotFoundException ex)
            {
                Debug.WriteLine("BuscarControlar:Controlar.EncontrarFile - " + ex);
            }

            return blnReturn;
        }

        #endregion

        //Decript the Licence File

        #region         private bool LeerFile(TextReader myTextReader, ref DatosFile myFileData)

        private bool LeerFile(TextReader myTextReader, ref DatosFile myFileData)
        {
            /* File Schema:
             * File Format: spsproparts_[ProductID]_productVersion].lic
             *
             * spsProParts                          --> Always
             * http://www.spsproparts.net           --> Always
             * ProductName: productAAA              --> Product Name, doesn't have any meaning
             * ProductVersion: 1.0.0.0              --> Needs to be the same as in the name of the lic file (used internally without dots)
             * ProductID: 123456789ABCDEF           --> Needs to be the same as in the name of the lic file
             * LicenseID: 987654321QWERTY           --> IMPORTANT: needed als public key to decrypt the raw data
             * RXpTjTdBl7RP1oQqfqScDlYXQxgAYPg+m    --> Raw data
             * 
             * Raw data format:
             * EngineVersion|LicenseDate|TypeLicense|EvaluationDate|EvaluationDays|MAC|IP|Domain|Client|Other
             * 1.0|20071125|mac|||112233445566|||ClientAAA||
             * EngineVersion                        --> The version of this Class. In the future, the class can change
             * LicenseDate                          --> Date of issuance of the License - yyyymmdd
             * TypeLicense                          --> can be: mac - ip - days - date - domain
             * EvaluationDate                       --> If the expiration evaluation periode is absolute - yyyymmdd
             * EvaluationDays                       --> If the expiration evaluation periode is relative - number of days
             * MAC                                  --> MAC Address to check, if TypeLicense is "mac"
             * IP                                   --> IP Address to check, if TypeLicense is "ip"
             * Domain                               --> Domain name to check, if TypeLicense is "domain"
             *  Host
             * Client                               --> Client name and other information
             * Other                                --> Reserved for other information
             * Host
             */

            bool blnReturn = false;
            string OneLine = string.Empty;
            int LineCounter = 1;
            string[] TempString = new string[2];

            try
            {
                //Read each line of the File, inclusive the raw data (last line), and set it in the object
                while ((OneLine = myTextReader.ReadLine()) != null)
                {
                    //Console.WriteLine(OneLine);

                    switch (LineCounter)
                    {
                        case (1):
                            //Nothing to do --> "spsProParts"
                            break;
                        case (2):
                            //Nothing to do --> "http://www.spsrproparts.net"
                            break;
                        case (3):
                            TempString = OneLine.Split(':');
                            myFileData.ProductName = TempString[1].Trim();
                            break;
                        case (4):
                            TempString = OneLine.Split(':');
                            myFileData.ProductVersion = TempString[1].Trim().Replace(".", string.Empty);
                            break;
                        case (5):
                            TempString = OneLine.Split(':');
                            myFileData.ProductID = TempString[1].Trim();
                            break;
                        case (6):
                            TempString = OneLine.Split(':');
                            myFileData.LicenseID = TempString[1].Trim();
                            break;
                        case (7):
                            myFileData.RawData = OneLine;
                            break;
                        default:
                            break;
                    }
                    LineCounter++;
                }
                myTextReader.Close();

                string RawLine;
                //Read the values of the encrypted last line, and set it in the object
                using (CryptoString myCryptoString = new CryptoString())
                {
                    RawLine = myCryptoString.Decrypt(myFileData.RawData, myFileData.LicenseID);
                    //string RawLine = myCryptoString.Encrypt("1.0|20071125|days||1|11223344556677|||clientAA||", myFileData.LicenseID);
                    //Console.WriteLine(RawLine);
                }
                TempString = RawLine.Split('|');
                myFileData.LicenseEngineVersion = TempString[0].Trim();
                myFileData.LicenseDate = TempString[1].Trim();
                //Type Licenses can be: mac - ip - days - date - domain
                switch (TempString[2].Trim().ToLower())
                {
                    case ("mac"):
                        myFileData.LicenseSort = DatosFile.TypeLicense.MAC;
                        break;
                    case ("ip"):
                        myFileData.LicenseSort = DatosFile.TypeLicense.IP;
                        break;
                    case ("days"):
                        myFileData.LicenseSort = DatosFile.TypeLicense.EvaluationDays;
                        break;
                    case ("date"):
                        myFileData.LicenseSort = DatosFile.TypeLicense.EvaluationDate;
                        break;
                    case ("domain"):
                        myFileData.LicenseSort = DatosFile.TypeLicense.Domain;
                        break;
                    case ("host"):
                        myFileData.LicenseSort = DatosFile.TypeLicense.Host;
                        break;
                    default:
                        break;
                }
                myFileData.DateExpiration = TempString[3].Trim();
                myFileData.DaysToExpiration = TempString[4].Trim();
                myFileData.MacAddress = TempString[5].Trim();
                myFileData.IpAddress = TempString[6].Trim();
                myFileData.DomainName = TempString[7].Trim();
                myFileData.HostName = TempString[8].Trim();
                myFileData.ClientName = TempString[9].Trim();
                myFileData.OtherInformation = TempString[10].Trim();                

                blnReturn = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("BuscarControlar:Controlar.LeerFile - " + ex);
            }

            return blnReturn;
        }

        #endregion

        //The name of the file needs to be consequent with the parameters inside the file

        #region         private bool ControlNombre(DatosFile myFileData)

        private bool ControlNombre(DatosFile myFileData)
        {
            bool blnReturn = false;

            try
            {
                if (myFileData.ProductID == _productID && myFileData.ProductVersion == _productVersion)
                    blnReturn = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("BuscarControlar:Controlar.ControlNombre - " + ex);
            }

            return blnReturn;
        }

        #endregion

        //Checks if the evaluation date (Relative) is not expired 

        #region         private bool IsInsideTimeRelative(DatosFile myFileData)

        private bool IsInsideTimeRelative(DatosFile myFileData)
        {
            bool blnReturn = false;

            try
            {
                DateTime InitiationDateFromReg;
                //Make a key for the register (using ProductID and VersionID, encrypted and based in the LicenseID
                using (CryptoString myCrypto = new CryptoString())
                {
                    string RegKeyDirectory = myCrypto.Encrypt((myFileData.ProductID + myFileData.ProductVersion), myFileData.LicenseID);
                    string RegKeyValue = myCrypto.Encrypt(DateTime.Today.ToShortDateString(), myFileData.LicenseID);

                    //If the key exists in the register, retrive his value
                    string strInitiationDateFromReg = ReadFromRegistry(string.Empty, RegKeyDirectory);

                    //If the key not exists in the register, make one with the date of today (also encrypted)
                    if (strInitiationDateFromReg.Length == 0) //Key doesn't exist --> make a new one
                    {
                        WriteToRegistry(string.Empty, RegKeyDirectory, RegKeyValue);
                    }

                    //Compare the value in the register with today
                    strInitiationDateFromReg = ReadFromRegistry(string.Empty, RegKeyDirectory);
                    InitiationDateFromReg = DateTime.Parse(myCrypto.Decrypt(strInitiationDateFromReg, myFileData.LicenseID));
                }
                // DateTime ExpirationDate = InitiationDateFromReg.AddDays(int.Parse(myFileData.DaysToExpiration));
                
                if (DateTime.Today <= InitiationDateFromReg)
                    blnReturn = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("BuscarControlar:Controlar.IsInsideTimeRelative - " + ex);
            }

            return blnReturn;
        }

        #endregion

        //Checks if the evaluation date (Absolute) is not expired 

        #region         private bool IsInsideTimeAbsolute(string ExpirationDate)

        private bool IsInsideTimeAbsolute(string ExpirationDate)
        {
            bool blnReturn = false;

            try
            {
                //Expected format of Date: "yyyyMMdd"
                DateTime datExpirationDate = DateTime.ParseExact(ExpirationDate.Trim(), "yyyyMMdd", null);

                if (DateTime.Today <= datExpirationDate)
                    blnReturn = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("BuscarControlar:Controlar.IsInsideTimeAbsolute - " + ex);
            }

            return blnReturn;
        }

        #endregion

        //Retrive the MAC addresses

        #region         private List<string> DireccionesM()

        private List<string> DireccionesM()
        {
            List<string> lstReturn = new List<string>();

            try
            {
                NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface NetworkInterfaceCounter in adapters)
                {
                    lstReturn.Add(NetworkInterfaceCounter.GetPhysicalAddress().ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("BuscarControlar:Controlar.DireccionesM - " + ex);
            }

            return lstReturn;
        }

        #endregion

        //Retrive the IP addresses

        #region         private List<string> DireccionesI()

        private List<string> DireccionesI()
        {
            List<string> lstReturn = new List<string>();

            try
            {
                string HostName = Dns.GetHostName();
                IPAddress[] myIpAddress = Dns.GetHostAddresses(HostName);

                for (int IpAddressCounter = 0; IpAddressCounter < myIpAddress.Length; IpAddressCounter++)
                {
                    lstReturn.Add(myIpAddress[IpAddressCounter].ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("BuscarControlar:Controlar.DireccionesI - " + ex);
            }

            return lstReturn;
        }

        #endregion

        private List<string> Dominios()
        {
            List<string> dominios = new List<string>();
            Forest forest = Forest.GetCurrentForest();
            DomainCollection domainCollection = forest.Domains;

            foreach (Domain objDomain in domainCollection)
            {
                dominios.Add(objDomain.Name);
            }
            return dominios;
        }

        //Compare the given value in the License with the values found in the server, and gives back if the value has been found

        #region         private bool FoundInList(List<string> ListToSearch, string ValueToSearch)

        private bool FoundInList(IEnumerable<string> ListToSearch, string ValueToSearch)
        {
            bool blnReturn = false;

            ValueToSearch = ValueToSearch.Trim().ToUpper();
            
            try
            {
                foreach (string ItemInList in ListToSearch)
                {
                    if (ItemInList.Trim().ToUpper() == ValueToSearch)
                    {
                        blnReturn = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("BuscarControlar:Controlar.FoundInList - " + ex);
            }

            return blnReturn;
        }

        #endregion

        #region         public string ReadFromRegistry(string RegKeyDir, string KeyToRead)

        public string ReadFromRegistry(string RegKeyDir, string KeyToRead)
        {
            string strReturn = string.Empty;

            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(C_REGKEY + @"\" + RegKeyDir);
                if (key != null)
                {
                    strReturn = key.GetValue(KeyToRead).ToString();
                    key.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("BuscarControlar:Controlar.ReadFromRegistry - " + ex);
            }

            return strReturn;
        }

        #endregion

        #region         public bool WriteToRegistry(string RegKeyDir, string KeyToWrite, string KeyValue)

        public bool WriteToRegistry(string RegKeyDir, string KeyToWrite, string KeyValue)
        {
            bool bolReturn;

            try
            {
                RegistryKey key;

                if (RegKeyDir.Length > 0)
                {
                    if (Registry.LocalMachine.OpenSubKey(C_REGKEY + @"\" + RegKeyDir, true) == null)
                        key = Registry.LocalMachine.CreateSubKey(C_REGKEY + @"\" + RegKeyDir);
                    else
                        key = Registry.LocalMachine.OpenSubKey(C_REGKEY + @"\" + RegKeyDir, true);
                }
                else
                {
                    if (Registry.LocalMachine.OpenSubKey(C_REGKEY, true) == null)
                        key = Registry.LocalMachine.CreateSubKey(C_REGKEY);
                    else
                        key = Registry.LocalMachine.OpenSubKey(C_REGKEY, true);
                }

                key.SetValue(KeyToWrite, KeyValue);
                key.Flush();
                key.Close();
                bolReturn = true;
            }
            catch (Exception ex)
            {
                bolReturn = false;
                Debug.WriteLine("BuscarControlar:Controlar.WriteToRegistry - " + ex);
            }

            return bolReturn;
        }

        #endregion
    }

    internal sealed class DatosFile
    {
        public enum TypeLicense
        {
            EvaluationDate,
            EvaluationDays,
            MAC,
            IP,
            Domain,
            Host
        }

        private string _productID;
        private string _productName;
        private string _productVersion;
        private string _licenseEngineVersion;
        private string _licenseDate;
        private TypeLicense _licenseSort;
        private string _dateExpiration;
        private string _daysToExpiration;
        private string _macAddress;
        private string _ipAddress;
        private string _domainName;
        private string _clientName;
        private string _licenseID;
        private string _otherInformation;
        private string _rawData;
        private string _hostName;

        public string ProductID
        {
            get { return _productID; }
            set { _productID = value; }
        }

        public string ProductVersion
        {
            get { return _productVersion; }
            set { _productVersion = value; }
        }

        public string LicenseEngineVersion
        {
            get { return _licenseEngineVersion; }
            set { _licenseEngineVersion = value; }
        }

        public string LicenseDate
        {
            get { return _licenseDate; }
            set { _licenseDate = value; }
        }

        public TypeLicense LicenseSort
        {
            get { return _licenseSort; }
            set { _licenseSort = value; }
        }

        public string DateExpiration
        {
            get { return _dateExpiration; }
            set { _dateExpiration = value; }
        }

        public string DaysToExpiration
        {
            get { return _daysToExpiration; }
            set { _daysToExpiration = value; }
        }

        public string MacAddress
        {
            get { return _macAddress; }
            set { _macAddress = value; }
        }

        public string DomainName
        {
            get { return _domainName; }
            set { _domainName = value; }
        }

        public string ClientName
        {
            get { return _clientName; }
            set { _clientName = value; }
        }

        public string LicenseID
        {
            get { return _licenseID; }
            set { _licenseID = value; }
        }

        public string OtherInformation
        {
            get { return _otherInformation; }
            set { _otherInformation = value; }
        }

        public string RawData
        {
            get { return _rawData; }
            set { _rawData = value; }
        }

        public string IpAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; }
        }
        public string HostName
        {
            get { return _hostName; }
            set { _hostName = value; }
        }

        public string ProductName
        {
            get { return _productName; }
            set { _productName = value; }
        }
    }

    internal sealed class CryptoString : IDisposable
    {
        // default secret key, NEVER change this value !!!!
        private readonly string _defaultKey;

        // use Triple DES as the cryptographic provider, 
        private readonly SymmetricAlgorithm _cryptoService;

        #region         public CryptoString()

        public CryptoString()
        {
            _cryptoService = new TripleDESCryptoServiceProvider();

            unchecked
            {
                //int s = DateTime.DaysInMonth(2007, 11) + 20;

                //int g01 = (0x0B722B89 >> 0);
                //short g02 = (short)(0x3A5F0000 >> s);
                //short g03 = (short)(0x43680000 >> s);
                //byte g04 = (byte)(0x00970000 >> s);
                //byte g05 = (byte)(0x00640000 >> s);
                //byte g06 = (byte)(0x00DF0000 >> s);
                //byte g07 = (byte)(0x00550000 >> s);
                //byte g08 = (byte)(0x00AA0000 >> s);
                //byte g09 = (byte)(0x008B0000 >> s);
                //byte g10 = (byte)(0x00800000 >> s);
                //byte g11 = (byte)(0x00A60000 >> s);
                
                // {0B722B89-3A5F-4368-9764-DF55AA8B80A6}
                
                //_defaultKey = new Guid(g01, g02, g03, g04, g05, g06, g07, g08, g09, g10, g11).ToString().ToLower();
                _defaultKey = "{0b722b89-3a5f-4368-9764-df55aa8b80a6}";
            }
        }

        #endregion

        /// <summary>
        /// creates a legal key for the provider  
        /// </summary>
        /// <param name="Key">The secret key for which to create a legal key</param>
        /// <returns>A byte array containing the legal key</returns>

        #region         private byte[] GetLegalKey(string Key)

        private byte[] GetLegalKey(string Key)
        {
            // let the provider generate a random key, so we can determine the requiered length for a key
            _cryptoService.GenerateKey();

            // if passed key is to long then trim it
            if (Key.Length > _cryptoService.Key.Length)
            {
                Key = Key.Substring(0, _cryptoService.Key.Length);
                return ASCIIEncoding.ASCII.GetBytes(Key);
            }

            // if key is to short expand it with spaces
            if (Key.Length < _cryptoService.Key.Length)
            {
                Key = Key.PadRight(_cryptoService.Key.Length, ' ');
                return ASCIIEncoding.ASCII.GetBytes(Key);
            }

            // key is of correct length, so return bytes
            return ASCIIEncoding.ASCII.GetBytes(Key);
        }

        #endregion

        /// <summary>
        /// Creates a legal Initialization Vector (IV)
        /// </summary>
        /// <returns></returns>

        #region         private byte[] GetLegalIV()

        private byte[] GetLegalIV()
        {
            // our vector
            string initializationVector = new Guid("{4378E63E-9DE5-46ce-9B41-C34EA5BB87BF}").ToString();

            // let the provider generate a random vector, so we can determine the requiered length for a vector
            _cryptoService.GenerateIV();

            // if to long then trim it
            if (initializationVector.Length > _cryptoService.IV.Length)
            {
                initializationVector = initializationVector.Substring(0, _cryptoService.IV.Length);
                return ASCIIEncoding.ASCII.GetBytes(initializationVector);
            }

            // if to short then expand it
            if (initializationVector.Length < _cryptoService.IV.Length)
            {
                initializationVector = initializationVector.PadRight(_cryptoService.IV.Length, ' ');
                return ASCIIEncoding.ASCII.GetBytes(initializationVector);
            }

        #endregion

            // vector is of correct length, so return bytes
            return ASCIIEncoding.ASCII.GetBytes(initializationVector);
        }

        /// <summary>
        /// Encrypts a string
        /// </summary>
        /// <param name="Source">The string to encrypt</param>
        /// <param name="Key">The secret key, use the same key for decryption</param>
        /// <returns>The encrypted string encoded in base64 format</returns>

        #region         public string Encrypt(string Source, string Key)

        public string Encrypt(string Source, string Key)
        {
            if (string.IsNullOrEmpty(Key))
            {
                Key = _defaultKey;
            }

            // use UTF8 unicode conversion for two byte characters
            byte[] bytIn = UTF8Encoding.UTF8.GetBytes(Source);

            // create a MemoryStream so that the process can be done without I/O files
            MemoryStream myMemoryStream = new MemoryStream();

            // set the private key
            _cryptoService.Key = GetLegalKey(Key);
            _cryptoService.IV = GetLegalIV();

            // create an Encryptor from the Provider Service instance
            ICryptoTransform encrypto = _cryptoService.CreateEncryptor();

            // create Crypto Stream that transforms a stream using the encryption
            CryptoStream myCryptoStream = new CryptoStream(myMemoryStream, encrypto, CryptoStreamMode.Write);

            // write out encrypted content into MemoryStream
            myCryptoStream.Write(bytIn, 0, bytIn.Length);
            myCryptoStream.FlushFinalBlock();

            myMemoryStream.Close();
            byte[] bytOut = myMemoryStream.ToArray();

            // convert into Base64 so that the result can be used in xml
            return Convert.ToBase64String(bytOut);
        }

        #endregion

        /// <summary>
        /// Decrypts a previously encrypted string
        /// </summary>
        /// <param name="Source">The string to decrypt</param>
        /// <param name="Key">The secret key, used during encryption</param>
        /// <returns>The decrypted string</returns>

        #region         public string Decrypt(string Source, string Key)

        public string Decrypt(string Source, string Key)
        {
            if (string.IsNullOrEmpty(Key))
            {
                Key = _defaultKey;
            }

            // convert from Base64 to binary
            byte[] bytIn = Convert.FromBase64String(Source);
            // create a MemoryStream with the input
            MemoryStream myMemoryStream = new MemoryStream(bytIn, 0, bytIn.Length);

            // set the private key
            _cryptoService.Key = GetLegalKey(Key);
            _cryptoService.IV = GetLegalIV();

            // create a Decryptor from the Provider Service instance
            ICryptoTransform encrypto = _cryptoService.CreateDecryptor();

            // create Crypto Stream that transforms a stream using the decryption
            CryptoStream myCryptoStream = new CryptoStream(myMemoryStream, encrypto, CryptoStreamMode.Read);

            // read out the result from the Crypto Stream
            StreamReader myStreamReader = new StreamReader(myCryptoStream);
            return myStreamReader.ReadToEnd();
        }

        #endregion

        /// <summary>
        /// Encrypts a string with a default key
        /// </summary>
        /// <param name="source">The string to encrypt</param>
        /// <returns>The encrypted string encoded in base64 format</returns>

        #region         public string Encrypt(string source)

        public string Encrypt(string source)
        {
            return Encrypt(source, _defaultKey);
        }

        #endregion

        /// <summary>
        /// Decrypts a previously encrypted string with a default key
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>The decrypted string</returns>

        #region         public string Decrypt(string source)

        public string Decrypt(string source)
        {
            return Decrypt(source, _defaultKey);
        }

        #endregion

        #region IDisposable

        private bool disposed;

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_cryptoService != null)
                    {
                        _cryptoService.Clear();
                    }                   
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations
        /// </summary>
        ~CryptoString()
        {
            Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
      
    }
}