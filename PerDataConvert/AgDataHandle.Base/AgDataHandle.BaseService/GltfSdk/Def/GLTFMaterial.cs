using AgDataHandle.BaseService.CommonHelper;
using AgDataHandle.BaseService.GltfSdk.ENUM;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace AgDataHandle.BaseService.GltfSdk.Def
{
    public class GLTFMaterial : GLTFElement
    {
        public GLTFMaterial()
        {
            DoubleSided = true;
            AlphaCutOff = 0.5f;
            AlphaMode = GLTFAlphaMode.OPAQUE.ToString();
            EmissiveFactor = new float[3] { 0.0f, 0.0f, 0.0f };
            Extensions = new Dictionary<string, JObject>();
            UUID = Guid.NewGuid().ToString();
        }

        #region Properties

        [Description("是否双面。默认使用 true.")]
        [JsonProperty("doubleSided", Required = Required.DisallowNull)]
        public bool DoubleSided { get; set; }

        [JsonProperty("alphaCutOff", Required = Required.DisallowNull)]
        public float AlphaCutOff { get; set; }

        [JsonProperty("alphaMode", Required = Required.DisallowNull)]
        public string AlphaMode { get; set; }

        [JsonProperty("emissiveFactor", Required = Required.DisallowNull)]
        public float[] EmissiveFactor { get; set; }

        [JsonProperty("emissiveTexture", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public GLTFTextureInfo EmissiveTexture { get; set; }

        [JsonProperty("occlusionTexture", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public GLTFOcclusionTextureInfo OcclusionTexture { get; set; }

        [JsonProperty("normalTexture", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public GLTFNormalTextureInfo NormalTexture { get; set; }

        [JsonProperty("pbrMetallicRoughness", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public GLTFPbrMetallicRoughness PbrMetallicRoughness { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore, Required = Required.DisallowNull)]
        public string Name { get; set; }

        [JsonProperty("extensions", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, JObject> Extensions { get; set; } = new Dictionary<string, JObject>();

        [JsonProperty("extras", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, JObject> Extras { get; set; }
        [JsonIgnore]
        public int ImageIndex
        {
            get
            {

                if (PbrMetallicRoughness != null && PbrMetallicRoughness.BaseColorTexture != null)
                {
                    int textureIndex = PbrMetallicRoughness.BaseColorTexture.Index;
                    if (GLTF != null)
                    {
                        if (GLTF.Textures != null)
                        {
                            int imageIndex = GLTF.Textures[textureIndex].ImageIndex;
                            return imageIndex;
                        }
                    }
                }
                return -1;

            }
            set
            {
                if (PbrMetallicRoughness != null && PbrMetallicRoughness.BaseColorTexture != null)
                {
                    PbrMetallicRoughness.BaseColorTexture.Index = value;
                }
            }
        }

        [JsonIgnore]
        public GLTFImage Image
        {
            get
            {
                if (ImageIndex == -1) return null;
                return GLTF.Images[ImageIndex];

            }
        }
        /// <summary>
        /// 用来排重而已，new的时候自动生成
        /// </summary>
        [JsonIgnore]
        public string UUID { get; set; }
        #endregion

        #region MyRegion

        public GLTFMaterial Clone()
        {
            GLTFMaterial material = new GLTFMaterial() { GLTF = null };

            if (Extensions != null || Extensions.Count == 0)
                material.Extensions = ArrayHelper.CloneDict(Extensions as Dictionary<string, JObject>);

            material.PbrMetallicRoughness = PbrMetallicRoughness.IsNullOrEmpty() || PbrMetallicRoughness.IsAllDefault ? null : PbrMetallicRoughness.Clone();
            material.AlphaCutOff = AlphaCutOff;
            material.DoubleSided = DoubleSided;
            material.AlphaMode = AlphaMode;
            material.Name = Name == null ? null : Name.Clone() as string;
            material.NormalTexture = NormalTexture == null ? null : NormalTexture.Clone();
            material.EmissiveFactor = EmissiveFactor == null ? null : EmissiveFactor.Clone() as float[];
            material.EmissiveTexture = EmissiveTexture == null ? null : EmissiveTexture.Clone();
            material.OcclusionTexture = OcclusionTexture == null ? null : OcclusionTexture.Clone();
            material.UUID = UUID;
            return material;
        }

        internal static GLTFMaterial Deserialize(JsonReader reader, GLTF model)
        {
            if (reader.TokenType == JsonToken.StartArray)
                reader.Read();
            GLTFMaterial mt = new GLTFMaterial();
            while (reader.TokenType == JsonToken.PropertyName || reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                string curPropName = reader.Value.ToString().ToLower();
                switch (curPropName)
                {
                    case "doublesided":
                        mt.DoubleSided = (bool)reader.ReadAsBoolean();
                        break;
                    case "alphamode":
                        mt.AlphaMode = reader.ReadAsString();
                        break;
                    case "alphacutoff":
                        mt.AlphaCutOff = (float)reader.ReadAsDouble();
                        break;
                    case "name":
                        mt.Name = reader.ReadAsString();
                        break;
                    case "normaltexture":
                        mt.NormalTexture = GLTFNormalTextureInfo.Deserialize(reader, model);// token -> PropertyName
                        break;
                    case "occlusiontexture":
                        mt.OcclusionTexture = GLTFOcclusionTextureInfo.Deserialize(reader, model);
                        break;
                    case "pbrmetallicroughness":
                        mt.PbrMetallicRoughness = GLTFPbrMetallicRoughness.Deserialize(reader, model);
                        break;
                    case "emissivefactor":
                        List<float> ef = new List<float>(3);
                        reader.Read(); // token -> startArray
                        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            ef.Add(float.Parse(reader.Value.ToString()));
                        mt.EmissiveFactor = ef.ToArray();
                        break;
                    case "emissivetexture":
                        mt.EmissiveTexture = GLTFTextureInfo.Deserialize(reader, model);
                        break;
                    case "extensions":
                        if (GLTF.IgnoreExtAndExtras)
                        {
                            reader.Skip();
                            break;
                        }
                        var jpExts = JToken.ReadFrom(reader) as JProperty;
                        mt.Extensions = mt.Extensions ?? new Dictionary<string, JObject>();
                        mt.LoadExtension(jpExts, mt.Extensions);
                        break;
                    case "extras":
                        if (GLTF.IgnoreExtAndExtras)
                        {
                            reader.Skip();
                            break;
                        }
                        var jpExtras = JToken.ReadFrom(reader) as JProperty;
                        mt.Extras = mt.Extras ?? new Dictionary<string, JObject>();
                        mt.LoadExtension(jpExtras, mt.Extras);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
                if (reader.TokenType == JsonToken.PropertyName)
                    continue;
            }
            if (reader.TokenType != JsonToken.StartObject && reader.TokenType != JsonToken.EndArray)
                reader.Read();

            mt.GLTF = model;
            return mt;
        }

        #endregion

        /// <summary>
        /// 获取默认的材质
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static GLTFMaterial CreateDefault(string name = "default", GLTFAlphaMode mode = GLTFAlphaMode.OPAQUE, bool doubleSided = true)
        {
            return new GLTFMaterial
            {
                AlphaMode = mode.ToString(),
                Name = name,
                PbrMetallicRoughness = new GLTFPbrMetallicRoughness
                {
                    BaseColorFactor = new float[] { 1.0f, 1.0f, 1.0f, 1.0f },
                    MetallicFactor = 0.0f,
                    RoughnessFactor = 0.7f
                },
                DoubleSided = doubleSided,
            };
        }
    }
}
