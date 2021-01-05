using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rc6_final
{
    class Mode: MainWindow
    {
        private ICoderBlok _algorithm;
        public static List<byte> original_text = new List<byte>();
        public static List<byte> encrypted_text = new List<byte>();

        public Mode(ICoderBlok algorithm)
        {
            _algorithm = algorithm;

        }


        private static byte[] Get_Iv()
        {
            Random rnd = new Random();
            Byte[] b = new Byte[16];
            rnd.NextBytes(b);
            return b;
        }
        private byte[] iv = Get_Iv();

        private byte[] ExpandData(ref byte[] data)
        {
            if (data.Length % 16 != 0)
            {
                var addByte = new List<byte>();
                for (int i = 0; i < 16 - data.Length % 16; i++)
                    addByte.Add(0);

                data = data.Concat(addByte.ToArray()).ToArray();
            }
            return data;
        }

        public byte[] EncodeEBC(byte[] data)
        {
            ExpandData(ref data);
            var result = new List<byte>();
            for (int i = 0; i < data.Length; i += _algorithm.Size())
            {
                result.AddRange(_algorithm.EncodeBlok(data.Skip(i).Take(_algorithm.Size()).ToArray()));
            }
            encrypted_text = result;
            return result.ToArray();
        }

        public byte[] DecodeEBC(byte[] bloks)
        {
            ExpandData(ref bloks);
            var result = new List<byte>();
            for (int i = 0; i < bloks.Length; i += _algorithm.Size())
               result.AddRange(_algorithm.DecodeBlok(bloks.Skip(i).Take(_algorithm.Size()).ToArray()));
            original_text = result;
            return result.ToArray();
        }


        public async Task<byte[]> EncodeCBC(byte[] message)
        {
            var message_copy = ExpandData(ref message);
            var result = new List<byte>();
            var prev = iv;

            for (int i = 0; i < message_copy.Length; i += _algorithm.Size())
            {
                for (int j = 0; j < _algorithm.Size(); j++)
                    message_copy[j] ^= prev[j];

                await Task.Run(() => result.AddRange(_algorithm.EncodeBlok(message.Skip(i).Take(_algorithm.Size()).ToArray())));
                // берем блок
                prev = result.Skip(i).Take(_algorithm.Size()).ToArray();
            }
            encrypted_text = result;
            return result.ToArray();
        }

        public async Task<byte[]> DecodeCBC(byte[] code)
        {
            var message_copy = (byte[])code.Clone();
            var result = new List<byte>();
            var prev = iv;

            for (int i = 0; i < message_copy.Length; i += _algorithm.Size())
            {
                for (int j = 0; j < _algorithm.Size(); j++)
                    message_copy[i + j] ^= prev[j];
                await Task.Run(() => result.AddRange(_algorithm.DecodeBlok(code.Skip(i).Take(_algorithm.Size()).ToArray())));
                prev = result.Skip(i).Take(_algorithm.Size()).ToArray();
            }
            original_text = result;
            return result.ToArray();
        }

        public async Task<byte[]> EncodeCFB(byte[] message)
        {
            var message_copy = ExpandData(ref message);

            var result = new List<byte>();

            byte[] prev = iv;

            for (int i = 0; i < message_copy.Length; i += _algorithm.Size())
            {
                await Task.Run(() => result.AddRange(_algorithm.EncodeBlok(prev)));
                for (int j = 0; j < _algorithm.Size(); j++)
                    result[i + j] ^= message_copy[j];

                prev = result.Skip(i).Take(_algorithm.Size()).ToArray();
            }
            encrypted_text = result;
            return result.ToArray();
        }

        public async Task<byte[]> DecodeCFB(byte[] code)
        {
            var message_copy = ExpandData(ref code);
            var result = new List<byte>();

            byte[] prev = iv;
            for (int i = 0; i < code.Length; i += _algorithm.Size())
            {
                await Task.Run(() => result.AddRange(_algorithm.DecodeBlok(prev)));
                for (int j = 0; j < _algorithm.Size(); j++)
                    result[i + j] ^= message_copy[j];

                prev = message_copy.Skip(i).Take(_algorithm.Size()).ToArray();
            }

            original_text = result;
            return result.ToArray();
        }

        public async Task<byte[]> EncodeOFB(byte[] message)
        {
            var message_copy = ExpandData(ref message);
            var result = new List<byte>();

            byte[] prev = iv;

            for (int i = 0; i < message.Length; i += _algorithm.Size())
            {
                await Task.Run(() => result.AddRange(_algorithm.EncodeBlok(prev)));
                prev = result.Skip(i).Take(_algorithm.Size()).ToArray();
                for (int j = 0; j < _algorithm.Size(); j++)
                    result[i + j] ^= message_copy[i + j];
            }
            encrypted_text = result;
            return result.ToArray();
        }

        public async Task<byte[]> DecodeOFB(byte[] code)
        {
            return await Task.Run(() => EncodeOFB(code));
        }
    }
}
