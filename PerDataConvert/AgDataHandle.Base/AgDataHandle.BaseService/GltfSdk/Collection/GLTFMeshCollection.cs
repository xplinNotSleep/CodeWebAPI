using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.Collection
{
    [JsonArray(false)] // means this jarray's count can not be 0 
    public class GLTFMeshCollection : GLTFCollection<GLTFMesh>
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

        internal static GLTFMeshCollection Deserialize(JsonReader reader, GLTF model)
        {
            var col = new GLTFMeshCollection();
            reader.Read(); // token -> startArray
            while (reader.TokenType != JsonToken.EndArray)
            {
                GLTFMesh mesh = GLTFMesh.Deserialize(reader, model);
                col.Add(mesh);
            }
            col.GLTF = model;
            return col;
        }
    }
}
