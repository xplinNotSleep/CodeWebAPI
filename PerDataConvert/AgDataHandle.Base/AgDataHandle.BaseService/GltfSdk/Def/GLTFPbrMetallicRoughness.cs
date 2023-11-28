using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.Def
{
    [JsonObject(IsReference = true)]
    public class GLTFPbrMetallicRoughness : GLTFElement
    {
        public GLTFPbrMetallicRoughness()
        {
            BaseColorFactor = new float[4] { 1.0f, 1.0f, 1.0f, 1.0f };
            MetallicFactor = 1.0f;
            RoughnessFactor = 1.0f;
        }


        #region Properties

        [JsonProperty("baseColorFactor", Required = Required.DisallowNull)]
        public float[] BaseColorFactor { get; set; }

        [JsonProperty("metallicFactor", Required = Required.DisallowNull)]
        public float MetallicFactor { get; set; }

        [JsonProperty("roughnessFactor", Required = Required.DisallowNull)]
        public float RoughnessFactor { get; set; }

        [JsonProperty("baseColorTexture", NullValueHandling = NullValueHandling.Ignore)]
        public GLTFTextureInfo BaseColorTexture { get; set; }

        [JsonProperty("metallicRoughnessTexture", NullValueHandling = NullValueHandling.Ignore)]
        public GLTFTextureInfo MetallicRoughnessTexture { get; set; }

        [JsonIgnore]
        public bool IsAllDefault
        {
            get
            {
                bool status = true;

                if (BaseColorFactor == null || !BaseColorFactor.All(v => v == 1.0))
                    status = false;
                if (MetallicFactor != 1.0)
                    status = false;
                if (RoughnessFactor != 1.0)
                    status = false;
                if (BaseColorTexture != null)
                    status = false;
                if (MetallicRoughnessTexture != null)
                    status = false;
                return status;
            }
        }

        #endregion

        #region Methods

        public GLTFPbrMetallicRoughness Clone()
        {
            var pbr = new GLTFPbrMetallicRoughness() { GLTF = null };

            pbr.BaseColorFactor = BaseColorFactor.Clone() as float[];
            pbr.BaseColorTexture = BaseColorTexture == null ? null : BaseColorTexture.Clone();
            pbr.MetallicFactor = MetallicFactor;
            pbr.MetallicRoughnessTexture = MetallicRoughnessTexture == null ? null : MetallicRoughnessTexture.Clone();
            pbr.RoughnessFactor = RoughnessFactor;

            return pbr;
        }

        internal static GLTFPbrMetallicRoughness Deserialize(JsonReader reader, GLTF model)
        {
            var pbr = new GLTFPbrMetallicRoughness();

            reader.Read(); // propertyName -> startObject

            while (reader.TokenType == JsonToken.PropertyName || reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                string propName = reader.Value.ToString();

                switch (propName)
                {
                    case "roughnessFactor":
                        pbr.RoughnessFactor = (float)reader.ReadAsDouble();
                        break;
                    case "metallicFactor":
                        pbr.MetallicFactor = (float)reader.ReadAsDouble();
                        break;
                    case "metallicRoughnessTexture":
                        pbr.MetallicRoughnessTexture = GLTFTextureInfo.Deserialize(reader, model);
                        break;
                    case "baseColorTexture":
                        pbr.BaseColorTexture = GLTFTextureInfo.Deserialize(reader, model);
                        break;
                    case "baseColorFactor":
                        List<float> bcF = new List<float>(4);
                        //double[] bcF = new double[4];
                        reader.Read(); // token -> startArray
                        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            bcF.Add(float.Parse(reader.Value.ToString()));
                        pbr.BaseColorFactor = bcF.ToArray();
                        break;
                    default:
                        break;
                }

            }
            if (pbr.IsAllDefault)
                return null;

            pbr.GLTF = model;
            return pbr;
        }

        #endregion
    }


}
