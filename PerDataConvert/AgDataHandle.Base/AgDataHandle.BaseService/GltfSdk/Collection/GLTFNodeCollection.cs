using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.Collection
{
    [JsonArray(false)]
    public class GLTFNodeCollection : GLTFCollection<GLTFNode>
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

        internal static GLTFNodeCollection Deserialize(JsonReader reader, GLTF model)
        {
            var col = new GLTFNodeCollection();
            reader.Read(); // token: propertyName -> startArray
            while (reader.TokenType != JsonToken.EndArray)
            {
                GLTFNode node = GLTFNode.Deserialize(reader, model);
                col.Add(node);
            }
            col.GLTF = model;

            return col;
        }
    }
}
