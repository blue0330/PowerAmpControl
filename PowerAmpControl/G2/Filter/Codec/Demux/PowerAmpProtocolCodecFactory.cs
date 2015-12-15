using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mina.Filter.Codec.Demux;
using PowerAmpControl.G2.Filter.Codec.Demux.PowerAmpProtocol;
using PowerAmpControl.ViewModel;

namespace PowerAmpControl.G2.Filter.Codec.Demux
{
    public class PowerAmpProtocolCodecFactory : DemuxingProtocolCodecFactory
    {
        public PowerAmpProtocolCodecFactory()
        {
            AddMessageDecoder<ProtocolDecoder>();
            AddMessageEncoder<PowerAmplifierMessage, PowerAmpProtocolEncoder>();
        }
    }
}
