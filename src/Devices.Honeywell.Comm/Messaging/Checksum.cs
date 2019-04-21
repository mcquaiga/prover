using System;

namespace Devices.Honeywell.Comm.Messaging
{
    //public static class Checksum
    //{
    //    #region Methods

    // public static string CalcCRC(string body) { int x; int y; long inChar; long crc = 0; try { for
    // (x = 1; x <= body.Length; x++) { inChar = Convert.ToChar(body.Substring(x, 1)); crc ^= (inChar
    // * 0x100); for (y = 1; y <= 8; y++) { if (crc & 0x32768) crc = (crc * 2) ^ 0x1021; else crc =
    // crc * 2; } crc = crc & 65535; } var crcHex = crc.ToString("X"); return Left("0000", 4 - crcHex
    // + crcHex); } catch (Exception ex) { throw new Exception(ex.Message, ex); } }

    //    #endregion
    //}

    public static class CrcChecksum
    {
        #region Methods

        public static string Calculate(string input)
        {
            var bytes = HexToBytes(input);
            return Crc16.ComputeChecksum(bytes).ToString("x2");
        }

        #endregion

        #region Methods

        private static byte[] HexToBytes(string input)
        {
            byte[] result = new byte[input.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Convert.ToByte(input.Substring(2 * i, 2), 16);
            }
            return result;
        }

        #endregion
    }

    internal static class Crc16
    {
        #region Constructors

        static Crc16()
        {
            ushort value;
            ushort temp;
            for (ushort i = 0; i < table.Length; ++i)
            {
                value = 0;
                temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                table[i] = value;
            }
        }

        #endregion

        #region Methods

        public static ushort ComputeChecksum(byte[] bytes)
        {
            ushort crc = 0;
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte index = (byte)(crc ^ bytes[i]);
                crc = (ushort)((crc >> 8) ^ table[index]);
            }
            return crc;
        }

        #endregion

        #region Fields

        const ushort polynomial = 0xA001;

        static readonly ushort[] table = new ushort[256];

        #endregion
    }
}