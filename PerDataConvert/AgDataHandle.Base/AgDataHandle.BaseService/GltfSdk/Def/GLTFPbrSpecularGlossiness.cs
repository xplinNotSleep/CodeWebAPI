using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.Def
{
    [JsonObject(IsReference = true)]
    public class GLTFPbrSpecularGlossiness : GLTFElement
    {
        public static readonly float[] SPEC_FACTOR_DEFAULT = new float[] { 0.2f, 0.2f, 0.2f };
        public static readonly double GLOSS_FACTOR_DEFAULT = 0.5d;

        /// <summary>
        /// The RGBA components of the reflected diffuse color of the material. 
        /// Metals have a diffuse value of [0.0, 0.0, 0.0]. 
        /// The fourth component (A) is the alpha coverage of the material. 
        /// The <see cref="GLTFMaterial.AlphaMode"/> property specifies how alpha is interpreted. 
        /// The values are linear.
        /// </summary>
        public float[] DiffuseFactor = new float[] { 0.0f, 0.0f, 0.0f, 1.0f };

        /// <summary>
        /// The diffuse texture. 
        /// This texture contains RGB(A) components of the reflected diffuse color of the material in sRGB color space. 
        /// If the fourth component (A) is present, it represents the alpha coverage of the 
        /// material. Otherwise, an alpha of 1.0 is assumed. 
        /// The <see cref="GLTFMaterial.AlphaMode"/> property specifies how alpha is interpreted. 
        /// The stored texels must not be premultiplied.
        /// </summary>
        public GLTFTextureInfo DiffuseTexture;

        /// <summary>
        /// The specular RGB color of the material. This value is linear
        /// </summary>
        public float[] SpecularFactor = SPEC_FACTOR_DEFAULT;

        /// <summary>
        /// The glossiness or smoothness of the material. 
        /// A value of 1.0 means the material has full glossiness or is perfectly smooth. 
        /// A value of 0.0 means the material has no glossiness or is completely rough. 
        /// This value is linear.
        /// </summary>
        public double GlossinessFactor = GLOSS_FACTOR_DEFAULT;

        /// <summary>
        /// The specular-glossiness texture is RGBA texture, containing the specular color of the material (RGB components) and its glossiness (A component). 
        /// The values are in sRGB space.
        /// </summary>
        public GLTFTextureInfo SpecularGlossinessTexture;

        public GLTFPbrSpecularGlossiness()
        {
        }


        #region Methods

        public GLTFPbrSpecularGlossiness Clone()
        {
            var pbr = new GLTFPbrSpecularGlossiness() { GLTF = null };

            pbr.DiffuseFactor = DiffuseFactor.Clone() as float[];
            pbr.DiffuseTexture = DiffuseTexture.Clone();
            pbr.SpecularFactor = SpecularFactor.Clone() as float[];
            pbr.GlossinessFactor = GlossinessFactor;
            pbr.SpecularGlossinessTexture = SpecularGlossinessTexture.Clone();

            return pbr;
        }

        /// <summary>
        /// 暂时未实现
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        internal static GLTFPbrSpecularGlossiness Deserialize(JsonReader reader, GLTF model)
        {
            var pbr = new GLTFPbrSpecularGlossiness();

            reader.Read(); // propertyName -> startObject

            while (reader.TokenType == JsonToken.PropertyName || reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                string propName = reader.Value.ToString();

                switch (propName)
                {
                    default:
                        break;
                }
            }

            pbr.GLTF = model;
            return pbr;
        }

        #endregion
    }
}
