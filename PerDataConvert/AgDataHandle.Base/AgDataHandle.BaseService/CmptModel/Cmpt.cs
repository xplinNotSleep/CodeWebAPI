using AgDataHandle.BaseService.B3dmModel;
using AgDataHandle.BaseService.I3dmModel;

namespace AgDataHandle.BaseService.CmptModel
{
    public class Cmpt
    {
        private CmptHeader m_header;

        public CmptHeader Header { get => m_header; set => m_header = value; }
        public List<byte> Data { get; set; } = new List<byte>();

        public Cmpt()
        {
            Header = new CmptHeader();
        }

        public void AddB3dm(B3dm b3dm)
        {
            b3dm.glbData = BufferPadding.AddPadding(b3dm.glbData);
            byte[] b3dmContent = b3dm.GetBytes();
            AddBytes(b3dmContent);
            Header.TilesLength += 1;
        }

        public void AddI3dm(I3dm i3dm)
        {
            byte[] i3dmContent = i3dm.GetBytes();
            AddBytes(i3dmContent);
            Header.TilesLength += 1;
        }


        public void AddBytes(byte[] bytes)
        {
            Data.AddRange(bytes);
        }

        public byte[] ToBytes()
        {
            var header_length = 16; // magic + version + bytelength + tileslenght
            Header.ByteLength = Data.Count + header_length;

            var memoryStream = new MemoryStream();
            var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(Header.ToBytes().ToArray());
            binaryWriter.Write(Data.ToArray());

            binaryWriter.Flush();
            binaryWriter.Close();

            return memoryStream.ToArray();
        }

        public void ExportCmpt(string path)
        {
            var bytes = ToBytes();
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllBytes(path, bytes);
        }
    }
}
