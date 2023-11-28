using AgDataHandle.Maths;
using AgDataHandle.BaseService.GltfSdk.Def;
using System.ComponentModel;

namespace AgDataHandle.BaseService.GltfSdk.BuildingContext
{
    public sealed class GLTFPrimitiveBuildingContext : GLTFBuildingContext
    {
        public GLTFPrimitiveBuildingContext(List<Vector3> position)
        {
            Position = position;
        }
        [Description("顶点的坐标列表")]
        public List<Vector3> Position { get; set; }

        [Description("顶点的法线列表")]
        public List<Vector3> Normal { get; set; }

        [Description("给 BaseColorTexture 使用")]
        public List<Vector2> UV0 { get; set; }

        public bool CheckData()
        {
            if (Position.Count % 3 != 0)
            {
                if (Indices == null || Indices.Count == 0)
                    return false;
            }
            if (UV0 != null && UV0.Count != Position.Count)
                return false;
            if (Normal != null && Normal.Count != Position.Count)
                return false;

            foreach (var attr in OtherVertexAttrData)
            {
                if (attr.Value.Count != Position.Count)
                    return false;
            }
            return true;
        }

        [Description("给 NormalTexture(法线纹理) 使用")]
        [Obsolete]
        public List<Vector2> UV1 { get; set; }

        [Description("给 EmissiveTexture(发光纹理) 使用")]
        [Obsolete]
        public List<Vector2> UV2 { get; set; }

        [Description("给 OcclusionTexture 使用")]
        [Obsolete]
        public List<Vector2> UV3 { get; set; }

        [Description("顶点的索引号列表")]
        public List<int> Indices { get; set; }

        internal Dictionary<string, List<float>> OtherVertexAttrData = new Dictionary<string, List<float>>();

        #region Methods

        public static GLTFPrimitiveBuildingContext LoadFromObject(GLTFPrimitive primitive)
        {
            var ctx = new GLTFPrimitiveBuildingContext(primitive.GetPositions());

            ctx.MaterialContext = primitive.MaterialIndex >= 0 ? null : GLTFMaterialBuildingContext.LoadFromObject(primitive.GetMaterial());
            ctx.Indices = primitive.Indices < 0 ? null : primitive.GetIndices();
            ctx.Normal = primitive.Attributes.ContainsKey("NORMAL") ? null : primitive.GetNormals();
            ctx.UV0 = primitive.Attributes.ContainsKey("TEXCOORD_0") ? null : primitive.GetUV0();

            if (primitive.Attributes.ContainsKey("_BATCHID"))
                ctx.OtherVertexAttrData.Add("_BATCHID", primitive.GetVetexAttributes("_BATCHID"));
            return ctx;
        }

        /// <summary>
        /// 比如可以设置 _BATCHID 数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public GLTFPrimitiveBuildingContext SetOther(string name, List<float> data)
        {
            // TODO 最好用正则
            // Regex regex = new Regex("^_[A-Z]{1,10}");
            if (!name.StartsWith("_"))
                throw new Exception("顶点的Attribute名称必须以下划线开始");

            if (Position.Count != data.Count)
                throw new Exception(string.Format("顶点个数：{0} 不等于传入数据的个数 {1}", Position.Count, data.Count));

            OtherVertexAttrData.Add(name, data);

            return this;
        }

        #endregion

        public GLTFMaterialBuildingContext MaterialContext { get; set; }
    }
}
