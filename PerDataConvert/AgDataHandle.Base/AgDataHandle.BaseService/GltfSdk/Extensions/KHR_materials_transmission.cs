using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json.Linq;

namespace AgDataHandle.BaseService.GltfSdk.Extensions
{
    /// Class:  KHR_texture_transform
    ///
    /// Summary:    此拓展用于支持纹理图集
    ///
	/// Author:zhengmx
    ///
    /// Date:   2022/7/19
    public class KHR_materials_transmission
    {
        public float TransmissionFactor { get; set; }

        public GLTFTextureInfo TransmissionTexture { get; set; }

        public KHR_materials_transmission()
        {
            TransmissionFactor = 0.0f;
        }

        public KHR_materials_transmission(JObject extension)
        {
            Deserialize(extension);
        }

        private void Deserialize(JObject extension)
        {
            foreach (var khrItem in extension)
            {
                switch (khrItem.Key)
                {
                    case "transmissionFactor":
                        if (float.TryParse(khrItem.Value.ToString(), out float factor))
                        {
                            TransmissionFactor = factor;
                        }
                        break;
                    case "transmissionTexture":
                        TransmissionTexture = new GLTFTextureInfo();
                        var texcoord = khrItem.Value["texcoord"];
                        var index = khrItem.Value["index"];
                        if (index != null)
                        {
                            TransmissionTexture.Index = int.Parse(index.ToString());
                        }
                        if (texcoord != null)
                        {
                            TransmissionTexture.TexCoord = int.Parse(texcoord.ToString());
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public JObject Serialize()
        {
            JObject jobject = new JObject();
            JObject extension = new JObject();
            JObject textureInfo = new JObject();

            extension.Add(KHR_texture_transmissionFactory.TRANSMISSIONFACTOR, TransmissionFactor);

            if (TransmissionTexture != null)
            {
                textureInfo.Add(KHR_texture_transmissionFactory.TEXTUREINFO_INDEX, TransmissionTexture.Index);
                textureInfo.Add(KHR_texture_transmissionFactory.TEXTUREINFO_TEXCOORD, TransmissionTexture.TexCoord);
                extension.Add(KHR_texture_transmissionFactory.TRANSMISSIONTEXTURE, textureInfo);
            }

            jobject.Add(KHR_texture_transmissionFactory.EXTENSION_NAME, extension);

            return jobject;
        }
    }

    public class KHR_texture_transmissionFactory
    {
        public const string EXTENSION_NAME = "KHR_materials_transmission";
        public const string TRANSMISSIONFACTOR = "transmissionFactor";
        public const string TRANSMISSIONTEXTURE = "transmissionTexture";

        public const string TEXTUREINFO_INDEX = "index";
        public const string TEXTUREINFO_TEXCOORD = "texcoord";
    }
}
