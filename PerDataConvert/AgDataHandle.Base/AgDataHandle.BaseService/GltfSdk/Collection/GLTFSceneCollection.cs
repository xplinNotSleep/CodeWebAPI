using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.Collection
{
    [JsonArray(false)]
    public class GLTFSceneCollection : GLTFCollection<GLTFScene>
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

        internal static GLTFSceneCollection Deserialize(JsonReader reader, GLTF model)
        {
            var col = new GLTFSceneCollection();
            reader.Read(); // token: propertyName -> startArray
            while (reader.TokenType != JsonToken.EndArray)
            {
                GLTFScene sce = GLTFScene.Deserialize(reader, model);
                col.Add(sce);
            }
            col.GLTF = model;

            return col;
        }
    }
}
