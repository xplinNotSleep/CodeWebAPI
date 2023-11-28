using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json.Linq;

namespace AgDataHandle.BaseService.GltfSdk.Extensions.Primitive
{
    public class DracoHandler
    {
        public DracoHandler(GLTFPrimitive pri)
        {
            if (pri.Extensions == null || !pri.Extensions.ContainsKey("KHR_draco_mesh_compression"))
                throw new Exception("[DracoHandler Error] primitive 无draco扩展项");
            Parse(pri.Extensions["KHR_draco_mesh_compression"]);
            model = pri.GLTF;
        }

        private GLTF model { get; set; }
        private int bufferViewIndex { get; set; }
        private Dictionary<string, int> vertexAttrs { get; set; }
        private void Parse(JObject dracoExt)
        {
            bufferViewIndex = dracoExt["bufferView"].Value<int>();
            var attrs = dracoExt["attributes"] as JObject;
            if (!attrs.ContainsKey("POSITION"))
                throw new Exception("[DracoHandler Error] 顶点属性没有POSITION");
            vertexAttrs = vertexAttrs ?? new Dictionary<string, int>(attrs.Properties().Count());
            foreach (var attr in attrs)
            {
                vertexAttrs.Add(attr.Key, attr.Value.Value<int>());
            }
        }

        public MemoryStream GetDRACOBuffer()
        {
            MemoryStream ms = new MemoryStream();

            var bv = model.BufferViews[bufferViewIndex];
            var bf = bv.GetBuffer();
            bf.ResetPosition();
            var dataStream = bf.DataStream;
            dataStream.Seek(bv.ByteOffset, SeekOrigin.Current);

            int bt, i = 0;
            while ((bt = dataStream.ReadByte()) != -1 && i < bv.ByteLength)
            {
                ms.WriteByte((byte)bt);
                i++;
            }

            return ms;
        }
    }
}
