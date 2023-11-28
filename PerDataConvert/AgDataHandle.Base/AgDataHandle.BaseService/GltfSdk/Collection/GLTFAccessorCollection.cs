using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.Collection
{
    [JsonArray(false)]
    public class GLTFAccessorCollection : GLTFCollection<GLTFAccessor>
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

        internal static GLTFAccessorCollection Deserialize(JsonReader reader, GLTF model)
        {
            var col = new GLTFAccessorCollection();
            reader.Read(); // token -> startArray
            while (reader.TokenType != JsonToken.EndArray)
            {
                GLTFAccessor acc = GLTFAccessor.Deserialize(reader, model);
                col.Add(acc);
            }
            col.GLTF = model;
            return col;
        }
    }
}
