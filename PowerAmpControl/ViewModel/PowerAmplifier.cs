using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Mina.Transport.Serial;
using PowerAmpControl.Model;
//using PowerAmpControl.Converter;

namespace PowerAmpControl.ViewModel
{
    public interface IMessageEncodable
    {
        byte[] ToBytes();
    }
    public interface IMessageDecodable
    {
        void Process(PowerAmplifierMessage message);
    }

    [StructLayout(LayoutKind.Explicit, Size = 2)]
    public struct CurrentLittleEndian
    {
        [FieldOffset(0)]
        public byte High;
        [FieldOffset(1)]
        public byte Low;

        public ushort LittleEndian
        {
            get
            {
                return BitConverter.ToUInt16(new[] { High, Low }, 0);
            }
            set
            {
                var littleEndian = BitConverter.GetBytes(value);
                High = littleEndian[1];
                Low = littleEndian[0];
            }
        }

        //public ushort CurrentBigEndian
        //{
        //    get
        //    {
        //        return BitConverter.ToUInt16(new[] { Low, High }, 0);
        //    }
        //    set
        //    {
        //        var littleEndian = BitConverter.GetBytes(value);
        //        High = littleEndian[0];
        //        Low = littleEndian[1];
        //    }
        //}

        public static CurrentLittleEndian FromBigEndian(ushort bigEndian)
        {
            var littleEndian = BitConverter.GetBytes(bigEndian);
            return new CurrentLittleEndian
            {
                High = littleEndian[1],
                Low = littleEndian[0]
            };

        }

        public static CurrentLittleEndian FromLittleEndian(ushort littleEndian)
        {
            var bigEndian = BitConverter.GetBytes(littleEndian);
            return new CurrentLittleEndian
            {
                High = bigEndian[0],
                Low = bigEndian[1]
            };

        }
    }
    public abstract class PowerAmplifierMessage
    {
        public enum Registers : byte
        {
            CurrentControl = 0x00,
            CurrentRead = 0x01,
            LaserEnable = 0x04,
            LaserShutdown = 0x05
        }

        public const byte Flag1 = 0xEF;
        public const byte Flag2 = 0xEF;
        public const byte Flag = 0x80;
        public byte Len { get; set; }
        public byte Addr { get; set; }
        public Registers Register { get; set; }
        public CurrentLittleEndian Current { get; set; }
        public byte Ldsw { get; set; }
        public byte State { get; set; }

        //public static bool IsValidRegister(Registers reg)
        //{
        //    return Enum.GetValues(typeof(Registers)).Cast<Registers>().Any(register => register == reg);
        //}

        public static bool IsValidRegister(Registers reg)
        {

            foreach (var register in Enum.GetValues(typeof(Registers)))
            {
                if ((Registers)register == reg)
                    return true;
            }
            return false;
        }
    }
    public class PowerAmplifierSet : PowerAmplifierMessage, IMessageEncodable
    {
        public PowerAmplifierSet(byte len, byte addr, Registers register, ushort current)
        {
            Len = len;
            Addr = addr;
            Register = register;
            Current = CurrentLittleEndian.FromBigEndian(current);
        }

        public PowerAmplifierSet(byte len, byte addr, Registers register)
        {
            Len = len;
            Addr = addr;
            Register = register;
        }
        public byte[] ToBytes()
        {
            switch (Register)
            {
                case Registers.CurrentControl:
                    return ToBytes_PumpCurrentControlSet();
                case Registers.CurrentRead:
                    return ToBytes_PumpCurrentReadSet();
                case Registers.LaserEnable:
                    return ToBytes_PumpEnableSet();
                case Registers.LaserShutdown:
                    return ToBytes_PumpDisableSet();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private byte[] ToBytes_PumpCurrentControlSet()
        {
            var sum = (byte)(Flag1 + Flag2 + Len + Addr + (byte)Register + Flag + Current.High + Current.Low);
            return new[]
            {
                Flag1,Flag2,Len,Addr,(byte)Register,Flag,Current.High,Current.Low,sum
            };
        }

        private byte[] ToBytes_PumpCurrentReadSet()
        {
            var sum = (byte)(Flag1 + Flag2 + Len + Addr + (byte)Register);
            return new[]
            {
                Flag1,Flag2,Len,Addr,(byte)Register,sum
            };
        }

        private byte[] ToBytes_PumpEnableSet()
        {
            var sum = (byte)(Flag1 + Flag2 + Len + Addr + (byte)Register);
            return new[]
            {
                Flag1,Flag2,Len,Addr,(byte)Register,sum
            };
        }

        private byte[] ToBytes_PumpDisableSet()
        {
            var sum = (byte)(Flag1 + Flag2 + Len + Addr + (byte)Register);
            return new[]
            {
                Flag1,Flag2,Len,Addr,(byte)Register,sum
            };
        }

    }

    public class PowerAmplifierBack : PowerAmplifierMessage, IMessageDecodable
    {

        public void Process(PowerAmplifierMessage message)
        {

            switch (message.Register)
            {
                case Registers.CurrentControl:
                    if (message.State == 0x00)
                        Application.Current.Dispatcher.Invoke(() =>
                        PowerAmplifierViewModel.PrintModels.Add(new PrintModel { Info = "CurrentSet successful！" }));
                    else
                        Application.Current.Dispatcher.Invoke(() =>
                        PowerAmplifierViewModel.PrintModels.Add(new PrintModel { Info = "CurrentValue upper！" }));
                    break;
                case Registers.CurrentRead:
                    Application.Current.Dispatcher.Invoke(() =>
                    PowerAmplifierViewModel.PowerAmplifierModels[0].Value = message.Current.LittleEndian);
                    Application.Current.Dispatcher.Invoke(() =>
                    PowerAmplifierViewModel.PrintModels.Add(new PrintModel { Info = "CurrentRead successful！" }));
                    break;
                case Registers.LaserEnable:
                    if (message.Ldsw == 0x01)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
PowerAmplifierViewModel.PrintModels.Add(new PrintModel { Info = "LaserEnable successful！" }));
                        PowerAmplifierViewModel.IsLaserEnable = true;
                        Application.Current.Dispatcher.Invoke(() =>
PowerAmplifierViewModel.LaserSwitchName.SwitchNameString = "关闭");
                    }

                    break;
                case Registers.LaserShutdown:
                    if (message.Ldsw == 0x00)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
PowerAmplifierViewModel.PrintModels.Add(new PrintModel { Info = "LaserShutdown successful！" }));
                        PowerAmplifierViewModel.IsLaserEnable = false;
                        Application.Current.Dispatcher.Invoke(() =>
PowerAmplifierViewModel.LaserSwitchName.SwitchNameString = "打开");
                    }

                    break;
            }
            if (PowerAmplifierViewModel.PrintModels.Count >= 6)
            {
                Application.Current.Dispatcher.Invoke(() =>
                PowerAmplifierViewModel.PrintModels.RemoveAt(0));
            }

        }
    }


    public class PowerAmplifierViewModel : INotifyPropertyChanged
    {

        public PowerAmplifierViewModel()
        {

            PowerAmplifierModels = new ObservableCollection<PowerAmplifierModel>
            {
                new PowerAmplifierModel {Value = 30, Unit = "mA"}
            };

            PrintModels = new ObservableCollection<PrintModel>
            {

            };
            SelectedTerminal = new PowerAmplifierModel();
            LaserSwitchName = new SwitchNameModel { SwitchNameString = "关闭" };
        }

        #region PowerAmplifierModels
        public static ObservableCollection<PowerAmplifierModel> PowerAmplifierModels { get; set; }

        #endregion PowerAmplifierModels


        #region SelectedTerminal
        public static PowerAmplifierModel SelectedTerminal { get; set; }

        #endregion SelectedTerminal

        public static void Client(EndPoint ep)
        {
            string t = null;
            var endpoint = ep as IPEndPoint;
            if (endpoint != null)
            {
                t = string.Format("{0}:{1}", endpoint.Address, endpoint.Port);
            }
            var endpoint2 = ep as SerialEndPoint;
            if (endpoint2 != null)
            {
                t = string.Format("{0}:{1}", endpoint2.PortName, endpoint2.BaudRate);
            }
            SelectedTerminal.Terminal = t;
        }

        public static void Write(EndPoint ep, object message)
        {

            var endpoint = ep as IPEndPoint;
            if (endpoint != null)
            {
                SocketViewModel.SocketServer.Write(ep, message);
            }
            var endpoint2 = ep as SerialEndPoint;
            if (endpoint2 != null)
            {
                SocketViewModel.SerialServer.Write(ep, message);
            }
        }

        #region SwitchCommand

        private RelayCommand _switchCommand;

        /// <summary>
        /// Gets the SwitchCommand.
        /// </summary>
        public RelayCommand SwitchCommand
        {
            get
            {
                return _switchCommand ?? (_switchCommand = new RelayCommand(
                    ExecuteSwitchCommand,
                    CanExecuteSwitchCommand));
            }
        }

        private void ExecuteSwitchCommand()
        {
            if (!SwitchCommand.CanExecute(null))
            {
                return;
            }
            LaserSwitch();
        }

        private bool CanExecuteSwitchCommand()
        {
            return true;
        }


        private void LaserSwitch()
        {
            if (((SocketViewModel.SocketServerModel.SwitchNameString == "关闭") && (SocketViewModel.SocketServer.Active))
    || ((SocketViewModel.SerialServerModel.SwitchNameString == "关闭")))
            {
                PowerAmplifierSet powerAmplifierSet;
                if (LaserSwitchName.SwitchNameString == "打开")
                {
                    powerAmplifierSet = new PowerAmplifierSet(0x03, 0xff, PowerAmplifierMessage.Registers.LaserEnable);
                    //if (IsLaserEnable)
                    //    LaserSwitchName.SwitchNameString = "关闭";
                }
                else
                {
                    powerAmplifierSet = new PowerAmplifierSet(0x03, 0xff, PowerAmplifierMessage.Registers.LaserShutdown);
                    //if (!IsLaserEnable)
                    //    LaserSwitchName.SwitchNameString = "打开"; 
                }
                if (SelectedTerminal.Terminal != null)
                    Write(SocketViewModel.SelectedClient.TransportEndPoint,
                        powerAmplifierSet);
            }
            else
            {
                MessageBox.Show("请打开服务器");
            }
        }


        #endregion SwitchCommand





        public static bool IsLaserEnable;

        public void SwitchNameStringChang(bool isEnable)
        {

        }

        #region LaserSwitchName
        public static SwitchNameModel LaserSwitchName { get; set; }

        #endregion LaserSwitchName

        #region PrintModels
        public static ObservableCollection<PrintModel> PrintModels { get; set; }

        #endregion PrintModels


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }

    //public class CurrentReadMonitor
    //{
    //    public CurrentReadMonitor()
    //    {
    //        CurrentRead = new PowerAmplifierModel()
    //        {
    //            Number = 0,
    //            Name = "获取电流"

    //        };

    //    }
    //    public PowerAmplifierModel CurrentRead { get; set; }

    //    public void Encode(IoSession session, object message, IProtocolEncoderOutput output)
    //    {
    //        var buffer = IoBuffer.Allocate(50);
    //        buffer.AutoExpand = true;

    //        buffer.Put(0xEF);
    //        buffer.Put(0xEF);
    //        buffer.Put(0x03);
    //        buffer.Put(0xFF);
    //        buffer.Put(0x01);
    //        var sum = 0xEF + 0xEF + 0x06 + 0xFF + 0x01;
    //        buffer.Put((byte)sum);

    //        buffer.Flip();

    //        //lock (EncoderLockHelper.EncoderLocker)
    //        {
    //            output.Write(buffer);
    //        }
    //    }

    //    public MessageDecoderResult Decodablie(IoSession session, IoBuffer input)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public MessageDecoderResult Decode(IoSession session, IoBuffer input, IProtocolDecoderOutput output)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Process()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    //public class LaserEnableMonitor : AbstractMonitor
    //{
    //    public void Encode(IoSession session, object message, IProtocolEncoderOutput output)
    //    {
    //        var buffer = IoBuffer.Allocate(50);
    //        buffer.AutoExpand = true;

    //        buffer.Put(0xEF);
    //        buffer.Put(0xEF);
    //        buffer.Put(0x03);
    //        buffer.Put(0xFF);
    //        buffer.Put(0x04);
    //        var sum = 0xEF + 0xEF + 0x06 + 0xFF + 0x01;
    //        buffer.Put((byte)sum);

    //        buffer.Flip();

    //        //lock (EncoderLockHelper.EncoderLocker)
    //        {
    //            output.Write(buffer);
    //        }
    //    }

    //    public MessageDecoderResult Decodablie(IoSession session, IoBuffer input)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public MessageDecoderResult Decode(IoSession session, IoBuffer input, IProtocolDecoderOutput output)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Process()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    //public class LaserShutdownMonitor : AbstractMonitor
    //{
    //    public void Encode(IoSession session, object message, IProtocolEncoderOutput output)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public MessageDecoderResult Decodablie(IoSession session, IoBuffer input)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public MessageDecoderResult Decode(IoSession session, IoBuffer input, IProtocolDecoderOutput output)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Process()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
