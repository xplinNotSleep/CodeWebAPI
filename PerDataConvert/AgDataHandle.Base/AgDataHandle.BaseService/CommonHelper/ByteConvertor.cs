﻿namespace AgDataHandle.BaseService.CommonHelper
{
    public static class ByteConvertor
    {
        public static byte[] ToBytes<T>(List<ushort> ids)
        {
            var res = new List<byte>();
            foreach (var id in ids)
            {
                if (typeof(T).Equals(typeof(byte)))
                {
                    res.Add((byte)id);
                }
                else if (typeof(T).Equals(typeof(ushort)))
                {
                    var p = Convert.ToUInt16(id);
                    res.AddRange(BitConverter.GetBytes(p));
                }
                else if (typeof(T).Equals(typeof(uint)))
                {
                    var p = Convert.ToUInt32(id);
                    res.AddRange(BitConverter.GetBytes(p));
                }

            }
            return res.ToArray();
        }
    }
}
