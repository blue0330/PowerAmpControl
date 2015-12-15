using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using PowerAmpControl.ViewModel;

namespace PowerAmpControl.Model
{
    public class PowerAmplifierModel : INotifyPropertyChanged
    {
        #region Value
        /// <summary>
        /// The <see cref="Value" /> property's name.
        /// </summary>
        public const string ValuePropertyName = "Value";

        private ushort _value = 300;

        /// <summary>
        /// Sets and gets the Number property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ushort Value
        {
            get { return _value; }

            set
            {
                if (_value == value)
                {
                    return;
                }

                _value = value;
                RaisePropertyChanged(ValuePropertyName);
            }
        }

        #endregion Value

        #region Unit

        /// <summary>
        /// The <see cref="Unit" /> property's name.
        /// </summary>
        public const string UnitPropertyName = "Unit";

        private string _unit;

        /// <summary>
        /// Sets and gets the Name property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Unit
        {
            get { return _unit; }

            set
            {
                if (_unit == value)
                {
                    return;
                }

                _unit = value;
                RaisePropertyChanged(UnitPropertyName);
            }
        }

        #endregion Unit

        #region Terminal

        /// <summary>
        /// The <see cref="Terminal" /> property's name.
        /// </summary>
        public const string TerminalPropertyName = "Terminal";

        private string _terminal = "";

        /// <summary>
        /// Sets and gets the Terminal property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Terminal
        {
            get { return _terminal; }

            set
            {
                if (_terminal == value)
                {
                    return;
                }

                _terminal = value;
                RaisePropertyChanged(TerminalPropertyName);
            }
        }

        #endregion Terminal

        #region CurrentSetCommand

        private RelayCommand _currentSetCommand;

        /// <summary>
        /// Gets the CurrentSetCommand.
        /// </summary>
        public RelayCommand CurrentSetCommand
        {
            get
            {
                return _currentSetCommand ?? (_currentSetCommand = new RelayCommand(
                    ExecuteCurrentSetCommand,
                    CanExecuteCurrentSetCommand));
            }
        }

        private void ExecuteCurrentSetCommand()
        {
            if (!CurrentSetCommand.CanExecute(null))
            {
                return;
            }

            if (((SocketViewModel.SocketServerModel.SwitchNameString == "关闭") && (SocketViewModel.SocketServer.Active)) 
                || ((SocketViewModel.SerialServerModel.SwitchNameString == "关闭")))
            {
                var powerAmplifierSet = new PowerAmplifierSet(0x06, 0xff, PowerAmplifierMessage.Registers.CurrentControl, 
                    PowerAmplifierViewModel.PowerAmplifierModels[0].Value);
                if (PowerAmplifierViewModel.SelectedTerminal.Terminal != null)
                    PowerAmplifierViewModel.Write(SocketViewModel.SelectedClient.TransportEndPoint,
                        powerAmplifierSet);
            }
            else
            {
                MessageBox.Show("请打开服务器");
            }


        }

        private bool CanExecuteCurrentSetCommand()
        {
            return true;
        }

        #endregion CurrentSetCommand


        #region CurrentGetCommand

        private RelayCommand _currentGetCommand;

        /// <summary>
        /// Gets the CurrentGetCommand.
        /// </summary>
        public RelayCommand CurrentGetCommand
        {
            get
            {
                return _currentGetCommand ?? (_currentGetCommand = new RelayCommand(
                    ExecuteCurrentGetCommand,
                    CanExecuteCurrentGetCommand));
            }
        }

        private void ExecuteCurrentGetCommand()
        {
            if (!CurrentGetCommand.CanExecute(null))
            {
                return;
            }

            if (((SocketViewModel.SocketServerModel.SwitchNameString == "关闭") && (SocketViewModel.SocketServer.Active))
                || ((SocketViewModel.SerialServerModel.SwitchNameString == "关闭")))
            {
                var powerAmplifierSet = new PowerAmplifierSet(0x03, 0xff, PowerAmplifierMessage.Registers.CurrentRead,
                    PowerAmplifierViewModel.PowerAmplifierModels[0].Value);
                if (PowerAmplifierViewModel.SelectedTerminal.Terminal != null)
                    PowerAmplifierViewModel.Write(SocketViewModel.SelectedClient.TransportEndPoint,
                        powerAmplifierSet);

            }
            else
            {
                MessageBox.Show("请打开服务器");
            }

        }

        private bool CanExecuteCurrentGetCommand()
        {
            return true;
        }

        #endregion CurrentGetCommand


        #region SwitchNameString

        /// <summary>
        /// The <see cref="SwitchNameString" /> property's name.
        /// </summary>
        public const string SwitchNameStringPropertyName = "SwitchNameString";

        private string _switchNameString = "打开";

        /// <summary>
        /// Sets and gets the SwitchNameString property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SwitchNameString
        {
            get { return _switchNameString; }

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

    public class SwitchNameModel : INotifyPropertyChanged
    {
        #region SwitchNameString

        /// <summary>
        /// The <see cref="SwitchNameString" /> property's name.
        /// </summary>
        public const string SwitchNameStringPropertyName = "SwitchNameString";

        private string _switchNameString;

        /// <summary>
        /// Sets and gets the SwitchNameString property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SwitchNameString
        {
            get { return _switchNameString; }

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
    public class PrintModel : INotifyPropertyChanged
    {

        #region Terminal

        /// <summary>
        /// The <see cref="Terminal" /> property's name.
        /// </summary>
        public const string TerminalPropertyName = "Terminal";

        private string _terminal;

        /// <summary>
        /// Sets and gets the Terminal property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Terminal
        {
            get { return _terminal; }

            set
            {
                if (_terminal == value)
                {
                    return;
                }

                _terminal = value;
                RaisePropertyChanged(TerminalPropertyName);
            }
        }

        #endregion Terminal

        #region Info

        /// <summary>
        /// The <see cref="Info" /> property's name.
        /// </summary>
        public const string InfoPropertyName = "Info";

        private string _info;

        /// <summary>
        /// Sets and gets the Info property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Info
        {
            get { return _info; }

            set
            {
                if (_info == value)
                {
                    return;
                }

                _info = value;
                RaisePropertyChanged(InfoPropertyName);
            }
        }

        #endregion Info

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
}

