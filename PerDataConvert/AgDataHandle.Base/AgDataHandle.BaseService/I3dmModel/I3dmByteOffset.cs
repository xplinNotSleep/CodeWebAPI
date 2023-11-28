using Newtonsoft.Json;

namespace AgDataHandle.BaseService.I3dmModel
{
    public class I3dmByteOffset
    {
        [JsonProperty("byteOffset")]

        public int offset { get; set; }

        [JsonProperty("componentType")]
        public string componentType { get; set; }
    }
}
