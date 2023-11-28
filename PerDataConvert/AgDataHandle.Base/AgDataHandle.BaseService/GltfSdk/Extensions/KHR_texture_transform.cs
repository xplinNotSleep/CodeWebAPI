using Newtonsoft.Json.Linq;
using System.Numerics;

namespace AgDataHandle.BaseService.GltfSdk.Extensions
{
    /// Class:  KHR_texture_transform
    ///
    /// Summary:    此拓展用于支持纹理图集
    ///
    /// Author:蔡周峻
    ///
    /// Date:   2022/1/13

    public class KHR_texture_transform
    {
        public float[] Offset { get; set; }
        public float Rotation { get; set; }
        public float[] Scale { get; set; }

        public KHR_texture_transform()
        {
            Offset = new float[] { 0.0f, 0.0f };
            Rotation = 0.0f;
            Scale = new float[] { 1.0f, 1.0f };
        }

        public KHR_texture_transform(float[] offset, float rotation, float[] scale)
        {
            Offset = offset;
            Rotation = rotation;
            Scale = scale;
        }

        public JProperty Serialize()
        {
            JProperty jProperty =
                new JProperty(KHR_texture_transformFactory.EXTENSION_NAME,
                    new JObject(
                        new JProperty(KHR_texture_transformFactory.OFFSET, new JArray(Offset[0], Offset[1])),
                        new JProperty(KHR_texture_transformFactory.ROTATION, Rotation),
                        new JProperty(KHR_texture_transformFactory.SCALE, new JArray(Scale[0], Scale[1]))
                        )
                    );
            return jProperty;
        }

        public Dictionary<string, JObject> ToDictionary()
        {
            var dic = new Dictionary<string, JObject>();
            dic.Add(KHR_texture_transformFactory.EXTENSION_NAME,
               new JObject(
                  new JProperty(KHR_texture_transformFactory.OFFSET, new JArray(Offset[0], Offset[1])),
                  new JProperty(KHR_texture_transformFactory.ROTATION, Rotation),
                  new JProperty(KHR_texture_transformFactory.SCALE, new JArray(Scale[0], Scale[1]))
                   )
               );
            return dic;
        }

        #region 静态创建方法
        public static KHR_texture_transform CreateFromTransform(Vector2 offset, Vector2 sourceImageSize, int atlaSize)
        {
            var kHR_Texture_Transform = new KHR_texture_transform();
            kHR_Texture_Transform.Offset = new float[] { offset.X / atlaSize, offset.Y / atlaSize };
            kHR_Texture_Transform.Scale = new float[] { sourceImageSize.X / atlaSize, sourceImageSize.Y / atlaSize };
            return kHR_Texture_Transform;
        }
        #endregion
    }

    public class KHR_texture_transformFactory
    {
        public const string EXTENSION_NAME = "KHR_texture_transform";
        public const string OFFSET = "offset";
        public const string ROTATION = "rotation";
        public const string SCALE = "scale";
    }

}
