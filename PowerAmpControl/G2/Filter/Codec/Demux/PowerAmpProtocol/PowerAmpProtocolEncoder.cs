using System;
using Mina.Core.Buffer;
using Mina.Filter.Codec.Demux;
using PowerAmpControl.ViewModel;

namespace PowerAmpControl.G2.Filter.Codec.Demux.PowerAmpProtocol
{
    class PowerAmpProtocolEncoder : IMessageEncoder<PowerAmplifierMessage>
    {
        public void Encode(Mina.Core.Session.IoSession session, PowerAmplifierMessage message, Mina.Filter.Codec.IProtocolEncoderOutput output)
        {
            var buffer = IoBuffer.Allocate(9);
            buffer.Put(((IMessageEncodable) message).ToBytes());
            buffer.Flip();
            output.Write(buffer);
        }

        public void Encode(Mina.Core.Session.IoSession session, object message, Mina.Filter.Codec.IProtocolEncoderOutput output)
        {
            Encode(session,(PowerAmplifierMessage)message,output);
        }
    }
}
