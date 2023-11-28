using AgDataHandle.BaseService.GltfSdk.Def;
using System.Text;

namespace AgDataHandle.BaseService
{
    /// Class:  GLBHelper
    ///
    /// Summary:    glb帮助类，提供gltf转glb字节.
    ///
    /// Author:蔡周峻
    ///
    /// Date:   2022/1/6

    public class GLBHelper
    {
        public static void Save(GLTF gLTFRoot, List<byte> binaryBuffer, string path)
        {
            var bytes = ToBytes(gLTFRoot, binaryBuffer);
            File.WriteAllBytes(path, bytes.ToArray());
        }
        public static List<byte> ToBytes(GLTF gLTFRoot, List<byte> binaryBuffer)
        {
            var jsonBuffer = GetJsonBufferPadded(gLTFRoot);
            // Allocate buffer (Global header) + (JSON chunk header) + (JSON chunk) + (Binary chunk header) + (Binary chunk)
            var glbLength = 12 + 8 + jsonBuffer.Length + 8 + binaryBuffer.Count;

            var glb = new List<byte>(glbLength);

            // Write binary glTF header (magic, version, length)
            var byteOffset = 0;
            glb.AddRange(BitConverter.GetBytes(0x46546C67));
            byteOffset += 4;
            glb.AddRange(BitConverter.GetBytes(2));
            byteOffset += 4;
            glb.AddRange(BitConverter.GetBytes(glbLength));
            byteOffset += 4;

            // Write JSON Chunk header (length, type)
            glb.AddRange(BitConverter.GetBytes(jsonBuffer.Length));
            byteOffset += 4;
            glb.AddRange(BitConverter.GetBytes(0x4E4F534A)); // Json
            byteOffset += 4;
            // Write JSON Chunk
            glb.AddRange(jsonBuffer);
            byteOffset += jsonBuffer.Length;

            // Write Binary Chunk header (length, type)
            glb.AddRange(BitConverter.GetBytes(binaryBuffer.Count));
            byteOffset += 4;
            glb.AddRange(BitConverter.GetBytes(0x004E4942)); // BIN
            byteOffset += 4;
            // Write Binary Chunk
            glb.AddRange(binaryBuffer);
            return glb;
        }

        /// <summary>
        /// padding json buffer
        /// </summary>
        /// <param name="model"></param>
        /// <param name="boundary"></param>
        /// <param name="offset">The byte offset on which the buffer starts.</param>
        /// <returns></returns>
        private static byte[] GetJsonBufferPadded(GLTF model, int boundary = 4, int offset = 0)
        {
            var json = model.GetJSONStr();
            var bs = Encoding.UTF8.GetBytes(json);
            var remainder = (offset + bs.Length) % boundary;
            var padding = remainder == 0 ? 0 : boundary - remainder;
            for (var i = 0; i < padding; i++)
            {
                json += " ";
            }
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
