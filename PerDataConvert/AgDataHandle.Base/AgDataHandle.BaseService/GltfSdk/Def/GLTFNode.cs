using AgDataHandle.Maths;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgDataHandle.BaseService.GltfSdk.Def
{
    /*
     * 反序列化/序列化 通过（通过 GLTFNodeConvertor 实现）2020年8月8日
     */

    public partial class GLTFNode: GLTFElement
    {
        public GLTFNode()
        {
            MeshIndex = -1;
            CameraIndex = -1;
        }

        #region Properties

        [JsonProperty(
            "name",
            Required = Required.DisallowNull,
            NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("camera")]
        public int CameraIndex { get; set; }

        [JsonProperty("mesh")]
        public int MeshIndex { get; set; }

        [JsonProperty(
            "matrix", 
            Required = Required.DisallowNull, 
            NullValueHandling = NullValueHandling.Ignore)]
        public IMatrix4x4 Matrix { get; set; }

        [JsonProperty(
            "rotation", 
            Required = Required.DisallowNull, 
            NullValueHandling = NullValueHandling.Ignore)]
        public float[] Rotation { get; set; }

        [JsonProperty(
            "scale", 
            Required = Required.DisallowNull, 
            NullValueHandling = NullValueHandling.Ignore)]
        public float[] Scale { get; set; }

        [JsonProperty(
            "translation", 
            Required = Required.DisallowNull, 
            NullValueHandling = NullValueHandling.Ignore)]
        public float[] Translation { get; set; }

        [JsonProperty(
            "children",
            Required = Required.DisallowNull,
            NullValueHandling = NullValueHandling.Ignore)]
        public List<int> ChildrenID { get; set; }

        [JsonProperty("extensions", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, JObject> Extensions { get; set; } = new Dictionary<string, JObject>();

        [JsonProperty("extras", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public JObject Extras { get; set; }
        /// <summary>
        /// TODO??????
        /// </summary>
        [JsonIgnore]
        public bool IsMeshNode
        {
            get
            {
                if (MeshIndex >= 0 && CameraIndex >= 0)
                    throw new Exception("MeshIndex & CameraIndex 数值错误");
                //if (ChildrenID != null && ChildrenID.Count != 0)
                //    return false;
                if (MeshIndex >= 0)
                    return true;
                else
                    return false;
            }
        }

        #region 获得模型
        public GLTFMesh GetMesh(bool isDeepClone = false)
        {
            if (!IsMeshNode)
                return null;
            GLTFMesh mesh;
            if (isDeepClone)
            {
                mesh = GLTF.Meshes[MeshIndex].Clone();
            }
            else
                mesh = GLTF.Meshes[MeshIndex];
            return mesh;
        }
        #endregion

        #region 获取父节点
        public GLTFNode GetParentNode()
        {
            int index = GLTF.Nodes.IndexOf(this);
            foreach (GLTFNode node in GLTF.Nodes)
            {
                if (node.ChildrenID.IsNotNullOrEmpty()&&node.ChildrenID.Contains(index))
                {
                    return node;
                }
            }
            return null;
        }
        #endregion

        #region 获取node的局部变换矩阵
        public IMatrix4x4 GetMatrix()
        {
            //存在变换矩阵则直接返回
            if (Matrix != null)
                return Matrix;

            Vector3 T = new Vector3(0, 0, 0);
            var Q = Quaternion.Identity;
            Vector3 S = Vector3.One;

            if (Translation != null && Translation.Length == 3)
            {
                T = new Vector3(Translation[0], Translation[1], Translation[2]);

            }
            if (Rotation != null && Rotation.Length == 4)
            {
                Q = new Quaternion(Rotation[0], Rotation[1], Rotation[2], Rotation[3]);
            }
            if (Scale != null && Scale.Length == 3)
            {
                S = new Vector3(Scale[0], Scale[1], Scale[2]);
            }

            return Matrix4x4.FromTranslationQuaternionRotationScale<Matrix4x4>(T, Q, S);
        }
        #endregion

        #region 获取node基于全局的变换矩阵（即从当前节点遍历计算到根节点）
        public IMatrix4x4 GetGlobalMatrix()
        {
            IMatrix4x4 result = GetMatrix();
            GLTFNode parent = this.GetParentNode();
            while (parent.IsNotNull())
            {
                result = result.MultiplyToOther(parent.GetMatrix());
                parent = parent.GetParentNode();
            }
            return result;
        }
        #endregion

        internal static GLTFNode Deserialize(JsonReader reader, GLTF model)
        {
            if (reader.TokenType == JsonToken.StartArray)
                reader.Read(); // startArray -> startObject
            GLTFNode nd = new GLTFNode();
            while (reader.TokenType == JsonToken.PropertyName || (reader.Read() && reader.TokenType == JsonToken.PropertyName))
            {
                string curProp = reader.Value.ToString();

                switch (curProp)
                {
                    case "camera":
                        nd.CameraIndex = (int)reader.ReadAsInt32();
                        break;
                    case "mesh":
                        nd.MeshIndex = (int)reader.ReadAsInt32();
                        break;
                    case "matrix":
                        List<float> mat4 = new List<float>(16);
                        reader.Read(); // token -> startArray
                        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            mat4.Add(float.Parse(reader.Value.ToString()));
                        nd.Matrix = new Matrix4x4(mat4.ToArray());
                        break;
                    case "rotation":
                        List<float> rotation = new List<float>(4);
                        reader.Read(); // token -> startArray
                        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            rotation.Add(float.Parse(reader.Value.ToString()));
                        nd.Rotation = rotation.ToArray();
                        break;
                    case "scale":
                        List<float> scale = new List<float>(3);
                        reader.Read(); // token -> startArray
                        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            scale.Add(float.Parse(reader.Value.ToString()));
                        nd.Scale = scale.ToArray();
                        break;
                    case "translation":
                        List<float> translation = new List<float>(16);
                        reader.Read(); // token -> startArray
                        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            translation.Add(float.Parse(reader.Value.ToString()));
                        nd.Translation = translation.ToArray();
                        break;
                    case "children":
                        List<int> children = new List<int>();
                        reader.Read();
                        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            children.Add(int.Parse(reader.Value.ToString()));
                        nd.ChildrenID = children;
                        break;
                    case "name":
                        nd.Name = reader.ReadAsString();
                        break;
                    case "extensions":
                        if (GLTF.IgnoreExtAndExtras)
                        {
                            reader.Skip();
                            break;
                        }
                        var jpExts = JToken.ReadFrom(reader) as JProperty;
                        nd.Extensions = nd.Extensions ?? new Dictionary<string, JObject>();
                        nd.LoadExtension(jpExts, nd.Extensions);
                        break;
                    case "extras":
                        if (GLTF.IgnoreExtAndExtras)
                        {
                            reader.Skip();
                            break;
                        }
                        var jpExtras = JToken.ReadFrom(reader) as JObject;
                        nd.Extras = jpExtras;
                        break;
                    default:
                        reader.Skip();
                        break;
                }
                // if (reader.Path.Contains("ext")) // 如果是endObject，说明上一步是 extensions 或 extras
            }
            if (reader.TokenType != JsonToken.StartObject && reader.TokenType != JsonToken.EndArray)
                reader.Read();
            
            nd.GLTF = model;
            return nd;
        }

        #endregion
    }
}
