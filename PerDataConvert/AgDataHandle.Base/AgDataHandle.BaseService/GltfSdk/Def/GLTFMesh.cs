using AgDataHandle.Maths;
using AgDataHandle.BaseService.CommonHelper;
using AgDataHandle.BaseService.GltfSdk.Collection;
using AgDataHandle.BaseService.GltfSdk.ENUM;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgDataHandle.BaseService.GltfSdk.Def
{
    /*
     * 反序列化/序列化测试通过 2020年8月7日
     */


    public class GLTFMesh : GLTFElement
    {
        private GLTF gltf;
        public GLTFMesh()
        {
            Primitives = new GLTFPrimitiveCollection(GLTF);
            UUID = Guid.NewGuid().ToString();
        }

        #region Properties

        [JsonProperty(
            "name",
            Required = Required.DisallowNull,
            NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        [JsonProperty(
            "primitives",
            Required = Required.Always)]
        public GLTFPrimitiveCollection Primitives { get; set; }

        [JsonProperty("extensions", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, JObject> Extensions { get; set; } = new Dictionary<string, JObject>();

        [JsonProperty("extras", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public JObject Extras { get; set; }
        /// <summary>
        /// 用来排重
        /// </summary>
        [JsonIgnore]
        public string UUID { get; set; }

        public override GLTF GLTF
        {
            get { return gltf; }
            set
            {
                gltf = value;
                foreach (var pri in Primitives)
                {
                    pri.GLTF = value;
                }
            }
        }

        #endregion

        #region Methods

        public GLTFMesh Clone()
        {
            GLTFMesh newMesh = new GLTFMesh
            {
                Name = Name == null ? null : Name.Clone() as string,
                GLTF = null,
                Primitives = Primitives.Clone(),
                Extensions = ArrayHelper.CloneDict(Extensions)
            };

            return newMesh;
        }

        internal static GLTFMesh Deserialize(JsonReader reader, GLTF model)
        {
            if (reader.TokenType == JsonToken.StartArray)
                reader.Read();

            GLTFMesh mesh = new GLTFMesh();

            while (reader.TokenType == JsonToken.PropertyName || reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                string curProp = reader.Value.ToString();
                switch (curProp)
                {
                    case "name":
                        mesh.Name = reader.ReadAsString();
                        break;
                    case "extensions":
                        if (GLTF.IgnoreExtAndExtras)
                        {
                            reader.Skip();
                            break;
                        }
                        var jpExts = JToken.ReadFrom(reader) as JProperty;
                        mesh.Extensions = mesh.Extensions ?? new Dictionary<string, JObject>();
                        mesh.LoadExtension(jpExts, mesh.Extensions);
                        break;
                    case "extras":
                        if (GLTF.IgnoreExtAndExtras)
                        {
                            reader.Skip();
                            break;
                        }
                        var jpExtras = JToken.ReadFrom(reader) as JObject;
                        mesh.Extras = jpExtras;
                        break;
                    case "primitives":
                        reader.Read();

                        while (reader.TokenType != JsonToken.EndArray)
                        {
                            GLTFPrimitive pri = GLTFPrimitive.Deserialize(reader, model);
                            mesh.Primitives.Add(pri);
                        }
                        break;
                    default:
                        reader.Skip();
                        break;
                }

            }
            if (reader.TokenType != JsonToken.StartObject && reader.TokenType != JsonToken.EndArray)
                reader.Read();

            mesh.GLTF = model;
            return mesh;
        }

        #endregion
        public List<MeshData> GetGeoDatasFromPrim(/*GLTFMesh originalMesh*/)
        {
            List<MeshData> meshDatas = new List<MeshData>();
            foreach (var p in Primitives)
            {
                MeshData meshdata = new MeshData();
                meshdata.Vectors = p.GetPositions() /*MeshBoundingBoxHelper.GetNodeActualCoord(p.GetPositions(), mat)*/;
                var uvs = p.GetUV0();
                meshdata.Uvs = uvs == null ? new List<Vector2>() : uvs;
                var uvs1 = p.GetUV0("TEXCOORD_1");
                meshdata.Uvs1 = uvs1 == null ? new List<Vector2>() : uvs1;
                var nomarl = p.GetNormals();
                meshdata.Nomarls = nomarl == null ? new List<Vector3>() : nomarl;
                meshdata.Indices = p.GetIndices();

                #region 处理材质信息
                meshdata.MaterialIndex = p.MaterialIndex;
                if (meshdata.MaterialIndex != -1)
                {
                    var mgLTFMaterial = GLTF.Materials[meshdata.MaterialIndex];
                    meshdata.gLTFMaterial = mgLTFMaterial;
                    meshdata.ImageIndex = mgLTFMaterial.ImageIndex;
                    //由于gltf数据可能会变化，如果是bufferView的情况直接换取字节流
                    if (mgLTFMaterial.Image != null && mgLTFMaterial.Image.ImageType == GLTFImageType.BufferView)
                    {
                        if (GLTF.Images.Count() < meshdata.ImageIndex)
                        {
                            var gLTFImage = new GLTFImage();
                            gLTFImage.ImageType = GLTFImageType.Bytes;
                            gLTFImage.BinaryData = mgLTFMaterial.Image.GetImageData().ToList();
                            gLTFImage.Name = mgLTFMaterial.Name;
                            meshdata.gLTFImage = gLTFImage;
                        }
                        else
                        {
                            meshdata.gLTFImage = GLTF.Images[meshdata.ImageIndex];
                        }
                    }
                    else
                    {
                        meshdata.gLTFImage = mgLTFMaterial.Image;
                    }

                    if (mgLTFMaterial.EmissiveTexture != null)
                    {
                        int emissiveImageIndex = GLTF.Textures[mgLTFMaterial.EmissiveTexture.Index].ImageIndex;
                        if (GLTF.Images[emissiveImageIndex].ImageType == GLTFImageType.BufferView)
                        {
                            if (GLTF.Images.Count() < emissiveImageIndex)
                            {
                                var gLTFImage = new GLTFImage();
                                gLTFImage.ImageType = GLTFImageType.Bytes;
                                gLTFImage.BinaryData = GLTF.Images[emissiveImageIndex].GetImageData().ToList();
                                meshdata.EmissiveImage = gLTFImage;
                            }
                            else
                            {
                                meshdata.EmissiveImage = GLTF.Images[emissiveImageIndex];
                            }
                        }
                        else
                        {
                            meshdata.EmissiveImage = GLTF.Images[emissiveImageIndex];
                        }
                    }
                }
                #endregion

                meshDatas.Add(meshdata);
            }
            return meshDatas;
        }
    }

    /// <summary>
    /// 记录Mesh几何信息
    /// </summary>
    public class MeshData
    {
        public List<Vector3> Vectors { get; set; }
        public List<Vector2> Uvs { get; set; }
        public List<Vector2> Uvs1 { get; set; }
        public List<Vector3> Nomarls { get; set; }
        public int ImageIndex { get; set; } = -1;
        public int MaterialIndex { get; set; } = -1;
        public GLTFImage gLTFImage { get; set; }
        public int emissiveImageIndex { get; set; } = -1;
        public GLTFImage EmissiveImage { get; set; }
        public List<int> Indices { get; set; }
        public GLTFMaterial gLTFMaterial { get; set; }
    }
}
