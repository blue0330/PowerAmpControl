using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace PowerAmpControl.Interface
{
    public interface ISocket
    {
        int SendData(Socket socket, byte[] buffer, int index, int count);
        int ReceiveData(Socket socket, byte[] buffer, int index, int count);
        void Close();
    }
}
