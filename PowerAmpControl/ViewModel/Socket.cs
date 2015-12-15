using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using PowerAmpControl.Model;
using System.Web;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using Mina.Core.Future;
using Mina.Core.Service;
using Mina.Core.Session;
using Mina.Filter.Codec;
using Mina.Transport.Serial;
using Mina.Transport.Socket;
using PowerAmpControl.Converter;
using PowerAmpControl.G2.Filter.Codec.Demux;
using PowerAmpControl.Interface;

namespace PowerAmpControl.ViewModel
{
    public class SocketViewModel
    {

        public static SocketServer SocketServer;
        public static SerialServer SerialServer;
        public SocketViewModel()
        {
            SocketServerModel = new ServerModel
            {
                // 获得本机局域网IP地址
                //IpAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[9].ToString(),
                //IpAddress = Dns.GetHostAddresses(Dns.GetHostName()),
                IpAddress = GetIp(),
                PortNum = 5004,
                //TranspotrEndPoint = new IPEndPoint(GetIp(),5004),
                SwitchNameString = "打开"
            };

            SerialServerModel = new SerialModel()
            {
                SwitchNameString = "打开"
            };

            SocketServer = new SocketServer();

            SerialServer = new SerialServer();

            ClientModels = new ObservableCollection<TransportModel>();

            SelectedClient = new TransportModel();





        }

        private string GetIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ipAddress in host.AddressList)
            {
                if (ipAddress.AddressFamily.ToString() == "InterNetwork")
                    return ipAddress.ToString();
            }
            return "0.0.0.0";
        }

        #region SocketServerModel;
        public static ServerModel SocketServerModel { get; set; }

        #endregion SocketServerModel;

        #region SerialServerModel;
        public static SerialModel SerialServerModel { get; set; }

        #endregion SerialServerModel;

        #region SocketServerSwitchCommand

        private RelayCommand _socketServerSwitchCommand;

        /// <summary>
        /// Gets the SocketServerSwitchCommand.
        /// </summary>
        public RelayCommand SocketServerSwitchCommand
        {
            get
            {
                return _socketServerSwitchCommand ?? (_socketServerSwitchCommand = new RelayCommand(
                    ExecuteSocketServerSwitchCommand,
                    CanExecuteSocketServerSwitchCommand));
            }
        }

        private void ExecuteSocketServerSwitchCommand()
        {
            if (!SocketServerSwitchCommand.CanExecute(null))
            {
                return;
            }

            if ((SocketServerModel.SwitchNameString == "打开") && (!SocketServer.IsServerOpen))
            {
                SocketServer.OpenServer(IPAddress.Parse(SocketServerModel.IpAddress), SocketServerModel.PortNum);
                if (SocketServer.IsServerOpen)
                {
                    SocketServerModel.SwitchNameString = "关闭";
                }

            }
            else
            {
                SocketServer.Unbind();
                //ClientModels.Clear();
                Client c = new SocketClientClear();
                c.Clear();
                if (!SocketServer.IsServerOpen)
                {
                    SocketServerModel.SwitchNameString = "打开";
                }

            }


        }
        private bool CanExecuteSocketServerSwitchCommand()
        {

            return true;
        }

        #endregion SocketServerSwitchCommand

        #region SerialServerSwitchCommand

        private RelayCommand _serialServerSwitchCommand;

        /// <summary>
        /// Gets the SerialServerSwitchCommand.
        /// </summary>
        public RelayCommand SerialServerSwitchCommand
        {
            get
            {
                return _serialServerSwitchCommand ?? (_serialServerSwitchCommand = new RelayCommand(
                    ExecuteSerialServerSwitchCommand,
                    CanExecuteSerialServerSwitchCommand));
            }
        }

        private void ExecuteSerialServerSwitchCommand()
        {
            if (!SerialServerSwitchCommand.CanExecute(null))
            {
                return;
            }


            if ((SerialServerModel.SwitchNameString == "打开") && (!SerialServer.IsServerOpen))
            //if ((SerialServerModel.SwitchNameString == "打开"))
            {
                SerialServer.OpenServer(SerialServerModel.SelectedPortNumString, SerialServerModel.SelectedBaudrateInt, SerialServerModel.SelectedParityParity,
                    SerialServerModel.SelectedDataBitsInt, SerialServerModel.SelectedStopBitsStopBits);
                SerialServer.IsServerOpen = true;
                if (SerialServer.IsServerOpen)
                {
                    SerialServerModel.SwitchNameString = "关闭";
                }

            }
            else
            {
                //SerialServer.R;
                SerialServer.SerialConnectFuture.Session.Close(true);
                SerialServer.IsServerOpen = false;
                Client c = new SerialClientClear();
                c.Clear();
                
                //ClientModels.Clear();
                if (!SerialServer.IsServerOpen)
                {
                    SerialServerModel.SwitchNameString = "打开";
                }

            }


        }
        private bool CanExecuteSerialServerSwitchCommand()
        {

            return true;
        }

        #endregion SerialServerSwitchCommand


         


        #region ClientModels
        public static ObservableCollection<TransportModel> ClientModels { get; set; }

        #endregion ClientModels

        #region SelectedClient

        private static TransportModel _selectedClient;
        public static TransportModel SelectedClient
        {
            get
            {

                return _selectedClient;
            }
            set
            {
                if (_selectedClient == value)
                {
                    return;
                }

                _selectedClient = value;
                if (value != null)
                    if (value.TransportEndPoint != null)
                        PowerAmplifierViewModel.Client(value.TransportEndPoint);
            }

        }

        #endregion SelectedClient



        public static void ClientsModelsAdd(EndPoint endPoint)
        {
            //ThreadPool.QueueUserWorkItem(delegate
            //{
            //    Application.Current.Dispatcher.Invoke(new Action(() =>
            //    {
            //        SocketClientModels.Add(new SocketModel() {IpEndPoint = ipEndPoint});
            //    }),null);

            //});
            //SocketClientModels.Add(new SocketModel { IpEndPoint = ipEndPoint });

            foreach (var client in ClientModels)
            {
                if (client.TransportEndPoint.Equals(endPoint))
                    return;
            }
            Application.Current.Dispatcher.Invoke(
                () => ClientModels.Add(new TransportModel { TransportEndPoint = endPoint }));
        }

    }


    public abstract class Client
    {
        public abstract void Clear();
    }

    public class SocketClientClear : Client
    {
        public override void Clear()
        {
            var i = 0;
            while (i < SocketViewModel.ClientModels.Count)
            //for (int i = 0; i < SocketViewModel.ClientModels.Count; i++)
            {
                if ((SocketViewModel.ClientModels[i].TransportEndPoint as IPEndPoint) != null)
                {
                    SocketViewModel.ClientModels.RemoveAt(i);
                    i = 0;
                }
                else
                {
                    i++;
                }
            }
        }
    }

    public class SerialClientClear : Client
    {
        public override void Clear()
        {
            var i = 0;
            while (i < SocketViewModel.ClientModels.Count)
            //for (int i = 0; i < SocketViewModel.ClientModels.Count; i++)
            {
                if ((SocketViewModel.ClientModels[i].TransportEndPoint as SerialEndPoint) != null)
                {
                    SocketViewModel.ClientModels.RemoveAt(i);
                    i = 0;
                }
                else
                {
                    i++;
                }
            }
        }
    }

    public class SocketServer : AsyncSocketAcceptor
    {
        public SocketServer()
        {
            //FilterChain.AddLast("logger", new LoggingFilter());
            FilterChain.AddLast("codec", new ProtocolCodecFilter(new PowerAmpProtocolCodecFactory()));
            //this.ExceptionCaught += SocketClient_ExceptionCaught;
            this.SessionConfig.SendBufferSize = 50;//Byte
            this.SessionConfig.ReadBufferSize = 50;
            this.SessionConfig.ReceiveBufferSize = 50;//50Byte
            this.ReuseBuffer = false;
            this.SessionConfig.WriteTimeout = 10;
            this.SessionCreated += ServerSessionCreated;
            this.MessageReceived += BackProcess.PowerAmpBackProcess;

        }
        private void ServerSessionCreated(object sender, IoSessionEventArgs e)
        {
            //Application.Current.Dispatcher.Invoke(() => SocketViewModel.SocketClientModels.Clear());
            foreach (var session in ManagedSessions)
            {
                var t = (EndPoint)session.Value.RemoteEndPoint;
                //var n = ManagedSessions.Count;
                SocketViewModel.ClientsModelsAdd(t);

            }
        }
        public void OpenServer(IPAddress ipAddress, int port)
        {
            try
            {
                base.Bind(new CreatEndPoint().SetEndPoint(ipAddress, port));
            }
            catch (Exception)
            {

                MessageBox.Show("SOCKET服务器打开失败");
            }


        }

        public void Bind(int port)
        {
            base.Bind(new IPEndPoint(IPAddress.Any, port));
        }
        public bool HasSessions
        {
            get { return ManagedSessions.Count > 0; }
        }

        public void ReadSession(IoSessionMessageEventArgs e)
        {
            var endpoint = e.Session.RemoteEndPoint;

        }
        //public void BackProcess(object sender, IoSessionMessageEventArgs e)
        //{
        //    var message = (PowerAmplifierMessage)e.Message;
        //    var powerAmpBack = new PowerAmplifierBack();
        //    powerAmpBack.Process(message);
        //}

        public void Write(EndPoint client, object message)
        {
            foreach (var session in ManagedSessions.Values)
            {
                var c = session.RemoteEndPoint;
                if (c.Equals(client))
                {
                    session.Write(message);
                    return;
                }
            }
        }

        //public bool IsServerOpen { get; set; }
        public bool IsServerOpen
        {
            get { return Active; }
        }

    }

    public class SerialServer : SerialConnector
    {
        //public static SerialConnector SerialConnector;
        public IConnectFuture SerialConnectFuture;
        public SerialServer()
        {

            //FilterChain.AddLast("logger", new LoggingFilter());
            FilterChain.AddLast("codec", new ProtocolCodecFilter(new PowerAmpProtocolCodecFactory()));
            //this.ExceptionCaught += SocketClient_ExceptionCaught;
            this.SessionConfig.WriteBufferSize = 50;//Byte
            this.SessionConfig.ReadBufferSize = 50;
            //this.SessionConfig.ReceivedBytesThreshold = 50;//50Byte
            this.SessionConfig.WriteTimeout = 10;
            this.SessionCreated += ServerSessionCreated;
            this.MessageReceived += BackProcess.PowerAmpBackProcess;

            SerialConnectFuture = new DefaultConnectFuture();

            //SerialConnector = new SerialConnector();
        }

        private void ServerSessionCreated(object sender, IoSessionEventArgs e)
        {
            //Application.Current.Dispatcher.Invoke(() => SocketViewModel.SocketClientModels.Clear());
            foreach (var session in ManagedSessions)
            {
                var t = (EndPoint)session.Value.RemoteEndPoint;
                //var n = ManagedSessions.Count;
                SocketViewModel.ClientsModelsAdd(t);

            }
        }

        public void OpenServer(string portNum, int baudrate, Parity parity, int dataBits, StopBits stopBits)
        {
            try
            {

                SerialConnectFuture = Connect(new CreatEndPoint().SetEndPoint(portNum, baudrate, parity, dataBits, stopBits));
                SerialConnectFuture.Await();

            }
            catch (Exception)
            {
                _isServerOpen = false;
                MessageBox.Show("串口打开失败");
            }


        }

        public void Write(EndPoint client, object message)
        {
            foreach (var session in ManagedSessions.Values)
            {
                var c = session.RemoteEndPoint;
                if (c.Equals(client))
                {
                    session.Write(message);
                    return;
                }
            }
        }

        private static bool _isServerOpen = false;
        public static bool IsServerOpen
        {
            get { return _isServerOpen; }
            set { _isServerOpen = value; }
        }
        
    }


    public abstract class BackProcess
    {
        public static void PowerAmpBackProcess(object sender, IoSessionMessageEventArgs e)
        {
            var message = (PowerAmplifierMessage)e.Message;
            var powerAmpBack = new PowerAmplifierBack();
            powerAmpBack.Process(message);
        }

    }

    public class CreatEndPoint
    {
        public EndPoint SetEndPoint(IPAddress ipAddress, int port)
        {
            return new IPEndPoint(ipAddress, port);
        }

        public EndPoint SetEndPoint(string portNum, int baudrate, Parity parity, int dataBits, StopBits stopBits)
        {
            return new SerialEndPoint(portNum, baudrate, parity, dataBits, stopBits);
        }

    }
}
