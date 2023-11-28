using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.Collection
{
    [JsonArray]
    public class GLTFMaterialCollection : GLTFCollection<GLTFMaterial>
    {
        [JsonIgnore]
        private GLTF gltf = null;
        [JsonIgnore]
        public GLTF GLTF
        {
            get
            {
                return gltf;
            }
            set
            {
                gltf = value;
                foreach (var el in ls)
                {
                    el.GLTF = value;
                }
            }
        }

        internal static GLTFMaterialCollection Deserialize(JsonReader reader, GLTF model)
        {
            var col = new GLTFMaterialCollection();
            reader.Read(); // token -> startArray
            while (reader.TokenType != JsonToken.EndArray)
            {
                GLTFMaterial mt = GLTFMaterial.Deserialize(reader, model);
                col.Add(mt);
            }
            col.GLTF = model;
            return col;
        }
    }
}
