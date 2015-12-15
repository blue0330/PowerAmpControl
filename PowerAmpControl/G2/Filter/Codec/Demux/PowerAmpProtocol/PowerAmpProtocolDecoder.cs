using System;
using Mina.Core.Buffer;
using Mina.Core.Session;
using Mina.Filter.Codec;
using Mina.Filter.Codec.Demux;
using PowerAmpControl.ViewModel;

namespace PowerAmpControl.G2.Filter.Codec.Demux.PowerAmpProtocol
{
    class ProtocolDecoder : IMessageDecoder
    {
        public MessageDecoderResult Decodable(IoSession session, IoBuffer input)
        {
            var flag1 = input.Get();
            while (flag1 != 0xEF)
            {
                if (input.Remaining < 1)
                {
                    return MessageDecoderResult.NeedData;
                }
                flag1 = input.Get();
            }
            if (input.Remaining < 1)
            {
                return MessageDecoderResult.NeedData;
            }
            var reg = input.Get();
            while (reg == 0xEF)
            {
                if (input.Remaining < 2)
                {
                    return MessageDecoderResult.NeedData;
                }
                reg = input.Get();
            }


            //if (input.Remaining < 2)
            //{
            //    return MessageDecoderResult.NeedData;
            //}


            var register = (PowerAmplifierMessage.Registers)reg;
            //var register = (PowerAmplifierMessage.Registers)input.Get();
            if (flag1 != PowerAmplifierMessage.Flag1 ||
                !PowerAmplifierMessage.IsValidRegister(register))
            {
                return MessageDecoderResult.NotOK;
            }
            var desiredRemaining = register == PowerAmplifierMessage.Registers.CurrentRead ? 3 : 2;
            if (input.Remaining < desiredRemaining)
            {
                return MessageDecoderResult.NeedData;
            }
            input.Skip(desiredRemaining - 1);
            var flag2 = input.Get();
            if (flag2 != PowerAmplifierMessage.Flag2)
            {
                return MessageDecoderResult.NotOK;
            }
            return MessageDecoderResult.OK;
        }

        public MessageDecoderResult Decode(IoSession session, IoBuffer input, IProtocolDecoderOutput output)
        {
            var message = new PowerAmplifierBack();
            var flag1 = input.Get();
            while ( flag1 != 0xEF )
            {
                flag1 = input.Get();
            }
            var reg = input.Get();
            while (reg == 0xEF)
            {
                reg = input.Get();
            }
            //input.Skip(1);//Flag1
            message.Register = (PowerAmplifierMessage.Registers)reg;
            switch (message.Register)
            {
                case PowerAmplifierMessage.Registers.CurrentControl:
                    message.State = input.Get();
                    break;
                case PowerAmplifierMessage.Registers.CurrentRead:
                    message.Current = CurrentLittleEndian.FromLittleEndian((ushort)input.GetInt16());
                    break;
                case PowerAmplifierMessage.Registers.LaserEnable:
                case PowerAmplifierMessage.Registers.LaserShutdown:
                    message.Ldsw = input.Get();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            input.Skip(1);  //Flag2
            output.Write(message);
            return MessageDecoderResult.OK;
        }

        public void FinishDecode(IoSession session, IProtocolDecoderOutput output)
        {

            throw new NotImplementedException();
        }
    }
}
