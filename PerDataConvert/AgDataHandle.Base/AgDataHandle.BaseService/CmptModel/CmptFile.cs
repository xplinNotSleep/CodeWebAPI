using AgDataHandle.BaseService.B3dmModel;
using AgDataHandle.BaseService.I3dmModel;
using System.Text;

namespace AgDataHandle.BaseService.CmptModel
{
    /// <summary>
    /// 从文件读取Cmpt文件，然后解析获得BI3DM的过程
    /// </summary>
    public class CmptFile
    {
        public List<BI3dmBase> BI3dms { get; set; }
        public CmptHeader Header { get; set; }

        public CmptFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new Exception("文件不存在：" + filePath);

            BI3dms = new List<BI3dmBase>();
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                using (var reader = new BinaryReader(fs))
                {
                    Header = ReadHead(reader);
                    for (var i = 0; i < Header.TilesLength; i++)
                    {
                        var header = BI3dmHelper.ReadHead(reader);
                        if (header.magic == "b3dm")
                        {
                            var b3dm = new B3dm(reader, header);
                            BI3dms.Add(b3dm);
                        }
                        else
                        {
                            var i3dm = new I3dm(reader, header);
                            BI3dms.Add(i3dm);
                        }

                    }
                }
            }
        }

        private CmptHeader ReadHead(BinaryReader br)
        {
            var head = new CmptHeader();
            var mag = br.ReadBytes(4);
            head.Magic = Encoding.UTF8.GetString(mag);
            head.Version = (int)br.ReadUInt32();
            head.ByteLength = (int)br.ReadUInt32();
            head.TilesLength = (int)br.ReadUInt32();
            return head;
        }
    }


    //public class BatchTable
    //{
    //    /// <summary>
    //    /// The number of distinguishable models, also called features, in the batch.
    //    /// If the Binary glTF does not have a batchId attribute, this field must be 0.
    //    /// </summary>
    //    [JsonProperty("BATCH_LENGTH")]
    //    public uint BatchLength { get; set; }
    //}


}
