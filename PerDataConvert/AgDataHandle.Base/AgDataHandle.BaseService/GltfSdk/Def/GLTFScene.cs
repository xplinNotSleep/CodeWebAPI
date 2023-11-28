using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.Def
{
    public class GLTFScene : GLTFElement
    {
        public GLTFScene()
        {
            Nodes = new List<int>();
        }

        #region Properties

        [JsonProperty(
            "name",
            Required = Required.DisallowNull,
            NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(
            "nodes",
            Required = Required.DisallowNull,
            NullValueHandling = NullValueHandling.Ignore)]
        public List<int> Nodes { get; set; }

        internal static GLTFScene Deserialize(JsonReader reader, GLTF model)
        {
            if (reader.TokenType == JsonToken.StartArray)
                reader.Read(); // token: startArray -> startObject

            GLTFScene sce = new GLTFScene();

            while (reader.TokenType == JsonToken.PropertyName || reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                string propName = reader.Value.ToString();

                switch (propName)
                {
                    case "name":
                        sce.Name = reader.ReadAsString();
                        break;
                    case "nodes":
                        List<int> nds = new List<int>();
                        reader.Read(); // token -> startArray
                        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            nds.Add(int.Parse(reader.Value.ToString()));
                        sce.Nodes = nds;
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
            reader.Read(); // token: endObject -> endArray
            sce.GLTF = model;
            return sce;
        }

        #endregion
    }
}
