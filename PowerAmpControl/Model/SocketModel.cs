using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PowerAmpControl.Model
{
    public class TransportModel : INotifyPropertyChanged
    {

        #region TransportEndPoint
        /// <summary>
        /// The <see cref="TransportEndPoint" /> property's name.
        /// </summary>
        public const string TransportEndPointPropertyName = "TransportEndPoint";

        private EndPoint _transportEndPoint;

        /// <summary>
        /// Sets and gets the SocketClients property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public EndPoint TransportEndPoint
        {
            get
            {
                return _transportEndPoint;
            }

            set
            {
                if (_transportEndPoint == value)
                {
                    return;
                }

                _transportEndPoint = value;
                RaisePropertyChanged(TransportEndPointPropertyName);
            }
        }

        #endregion TransportEndPoint

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged
    }

    public class ServerModel : INotifyPropertyChanged
    {

        #region IpAddress
        /// <summary>
        /// The <see cref="IpAddress" /> property's name.
        /// </summary>
        public const string IpAddressPropertyName = "IpAddress";

        private String _ipAddress;
        
        /// <summary>
        /// Sets and gets the IpAddServer property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public String IpAddress
        {
            get
            {
                return _ipAddress;
                
            }

            set
            {
                if (_ipAddress == value)
                {
                    return;
                }

                _ipAddress = value;
                RaisePropertyChanged(IpAddressPropertyName);
            }
        }
        #endregion IpAddress

        #region PortNum
        /// <summary>
        /// The <see cref="PortNum" /> property's name.
        /// </summary>
        public const string PortNumPropertyName = "PortNum";

        private int _portNum;

        /// <summary>
        /// Sets and gets the PortNum property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int PortNum
        {
            get
            {
                return _portNum;
            }

            set
            {
                if (_portNum == value)
                {
                    return;
                }

                _portNum = value;
                RaisePropertyChanged(PortNumPropertyName);
            }
        }
        #endregion PortNum

        #region SwitchNameString
        /// <summary>
        /// The <see cref="SwitchNameString" /> property's name.
        /// </summary>
        public const string SwitchNameStringPropertyName = "SwitchNameString";

        private string _switchNameString;

        /// <summary>
        /// Sets and gets the SocketSwitchNam property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SwitchNameString
        {
            get
            {
                return _switchNameString;
            }

            set
            {
                if (_switchNameString == value)
                {
                    return;
                }

                _switchNameString = value;
                RaisePropertyChanged(SwitchNameStringPropertyName);
            }
        }
        #endregion SwitchNameString

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged
    }

    public class ClientModel : INotifyPropertyChanged
    {

        #region Client
        /// <summary>
        /// The <see cref="Client" /> property's name.
        /// </summary>
        public const string ClientPropertyName = "Client";

        private EndPoint _client;

        /// <summary>
        /// Sets and gets the SelectedClient property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public EndPoint Client
        {
            get
            {
                return _client;
            }

            set
            {
                if (_client == value)
                {
                    return;
                }

                _client = value;
                RaisePropertyChanged(ClientPropertyName);
            }
        }
        #endregion Client

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged
    }

    public class SerialModel : INotifyPropertyChanged
    {
        #region prop

        //////////////////////////////////////////////////////////////////////////////////

        #region PortNumCollectionString
        private readonly ObservableCollection<String> _portNumCollectionString = new ObservableCollection<string>(SerialPort.GetPortNames());

        public ObservableCollection<string> PortNumCollectionString
        {
            get { return _portNumCollectionString; }

            //set { _portNumCollectionString = value; }
            //RaisePropertyChanged(PortNumCollectionString);
        }

        #endregion PortNumCollectionString

        #region BaudrateCollectionInt

        private readonly ObservableCollection<int> _baudrateCollectionInt = new ObservableCollection<int>
            {
                110,
                310,
                600,
                1200,
                2400,
                4800,
                9600,
                14400,
                19200,
                38400,
                56000,
                57600,
                115200,
                128000,
                256000
            };
        public ObservableCollection<int> BaudrateCollectionInt
        {
            get { return _baudrateCollectionInt; }
            //set { _baudrateCollectionInt = value; }
        }



        #endregion BaudrateCollectionInt

        #region DataBitsCollectionInt
        private readonly ObservableCollection<int> _dataBitsCollectionInt = new ObservableCollection<int>()
            {
                5,
                6,
                7,
                8
            };
        public ObservableCollection<int> DataBitsCollectionInt
        {
            get { return _dataBitsCollectionInt; }
            //set { _dataBitsCollectionInt = value; }
        }
        #endregion DataCollectionInt;

        #region StopBitsDictionaryStopBits

        private readonly Dictionary<double, StopBits> _stopBitsDictionaryStopBits = new Dictionary<double, StopBits>()
            {
                {0, StopBits.None},
                {1, StopBits.One},
                {1.5, StopBits.OnePointFive},
                {2, StopBits.Two}
            };
        public Dictionary<double, StopBits> StopBitsDictionaryStopBits
        {
            get { return _stopBitsDictionaryStopBits; }
            //set { _stopBitsDictionaryStopBits = value; }
        }
        #endregion StopBitsDictionaryStopBits

        #region ParityDictionaryParity

        private readonly Dictionary<string, Parity> _parityDictionaryParity = new Dictionary<string, Parity>()
            {
                {"无校验", Parity.None},
                {"奇校验", Parity.Odd},
                {"偶校验", Parity.Even},
                {"1校验", Parity.Mark},
                {"0校验", Parity.Space}
            };

        public Dictionary<string, Parity> ParityDictionaryParity
        {
            get { return _parityDictionaryParity; }
            //set { _parityDictionaryParity = value; }
        }
        #endregion ParityDictionaryParity

        //////////////////////////////////////////////////////////////////////////////////
         
        #region SwitchNameString
        /// <summary>
        /// The <see cref="SwitchNameString" /> property's name.
        /// </summary>
        public const string SwitchNameStringPropertyName = "SwitchNameString";

        private string _switchNameString;

        /// <summary>
        /// Sets and gets the SocketSwitchNam property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SwitchNameString
        {
            get
            {
                return _switchNameString;
            }

            set
            {
                if (_switchNameString == value)
                {
                    return;
                }

                _switchNameString = value;
                RaisePropertyChanged(SwitchNameStringPropertyName);
            }
        }
        #endregion SwitchNameString

        /////////////////////////////////////////////////////////////////////////////////

        #region SelectedPortNumString
        /// <summary>
        /// The <see cref="SelectedPortNumString" /> property's name.
        /// </summary>
        public const string SelectedPortNumStringPropertyName = "SelectedPortNumString";

        private string _selectedPortNumString;

        /// <summary>
        /// Sets and gets the SelectedPortNumString property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SelectedPortNumString
        {
            get
            {
                return _selectedPortNumString;
            }

            set
            {
                if (_selectedPortNumString == value)
                {
                    return;
                }

                _selectedPortNumString = value;
                RaisePropertyChanged(SelectedPortNumStringPropertyName);
            }
        }
        #endregion SelectedPortNumString

        #region SelectedBaudrateInt
        /// <summary>
        /// The <see cref="SelectedBaudrateInt" /> property's name.
        /// </summary>
        public const string SelectedBaudrateIntPropertyName = "SelectedBaudrateInt";

        private int _selectedBaudrateInt;

        /// <summary>
        /// Sets and gets the SelectedBaudrateInt property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int SelectedBaudrateInt
        {
            get
            {
                return _selectedBaudrateInt;
            }

            set
            {
                if (_selectedBaudrateInt == value)
                {
                    return;
                }

                _selectedBaudrateInt = value;
                RaisePropertyChanged(SelectedBaudrateIntPropertyName);
            }
        }
        #endregion SelectedBaudrateInt

        #region SelectedDataBitsInt
        /// <summary>
        /// The <see cref="SelectedDataBitsInt" /> property's name.
        /// </summary>
        public const string SelectedDataBitsIntPropertyName = "SelectedDataBitsInt";

        private int _selectedDataBitsInt;

        /// <summary>
        /// Sets and gets the SelectedDataBitsInt property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int SelectedDataBitsInt
        {
            get
            {
                return _selectedDataBitsInt;
            }

            set
            {
                if (_selectedDataBitsInt == value)
                {
                    return;
                }

                _selectedDataBitsInt = value;
                RaisePropertyChanged(SelectedDataBitsIntPropertyName);
            }
        }
        #endregion SelectedDataBitsInt

        #region SelectedStopBitsStopBits
        /// <summary>
        /// The <see cref="SelectedStopBitsStopBits" /> property's name.
        /// </summary>
        public const string SelectedStopBitsStopBitsPropertyName = "SelectedStopBitsStopBits";

        private StopBits _selectedStopBitsStopBits;

        /// <summary>
        /// Sets and gets the SelectedStopBitsStopBits property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public StopBits SelectedStopBitsStopBits
        {
            get
            {
                return _selectedStopBitsStopBits;
            }

            set
            {
                if (_selectedStopBitsStopBits == value)
                {
                    return;
                }

                _selectedStopBitsStopBits = value;
                RaisePropertyChanged(SelectedStopBitsStopBitsPropertyName);
            }
        }
        #endregion SelectedStopBitsStopBits

        #region SelectedParityParity
        /// <summary>
        /// The <see cref="SelectedParityParity" /> property's name.
        /// </summary>
        public const string SelectedParityParityPropertyName = "SelectedParityParity";

        private Parity _selectedParitypParity;

        /// <summary>
        /// Sets and gets the SelectedParityParity property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Parity SelectedParityParity
        {
            get
            {
                return _selectedParitypParity;
            }

            set
            {
                if (_selectedParitypParity == value)
                {
                    return;
                }

                _selectedParitypParity = value;
                RaisePropertyChanged(SelectedParityParityPropertyName);
            }
        }
        #endregion SelectedParityParity

        ////////////////////////////////////////////////////////////////////////////////////
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged

        #endregion prop
    }
}
