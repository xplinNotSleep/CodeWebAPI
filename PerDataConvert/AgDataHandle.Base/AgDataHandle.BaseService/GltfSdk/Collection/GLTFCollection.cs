using AgDataHandle.BaseService.GltfSdk.ENUM;
using Newtonsoft.Json;
using System.Collections;

namespace AgDataHandle.BaseService.GltfSdk.Collection
{
    [JsonArray(false)]
    public abstract class GLTFCollection<GLTFElement> : ICollection<GLTFElement>, IEnumerable<GLTFElement>, IEnumerable, IList<GLTFElement>, IReadOnlyCollection<GLTFElement>, IReadOnlyList<GLTFElement>
    {
        public GLTFCollection()
        {
        }

        public GLTFCollection(List<GLTFElement> ls)
        {
            this.ls = ls;
        }

        [JsonProperty]
        protected List<GLTFElement> ls = new List<GLTFElement>();

        [JsonIgnore]
        public GLTFElement this[int index]
        {
            get { return ls[index]; }
            set { ls[index] = value; }
        }

        [JsonIgnore]
        public int Count { get { return ls.Count; } }

        [JsonIgnore]
        public bool IsReadOnly { get { return false; } }

        [JsonIgnore]
        public GLTFCollectionType CollectionType { get; set; }

        public void Add(GLTFElement item) { ls.Add(item); }

        public void Clear() { ls.Clear(); }

        public bool Contains(GLTFElement item) { return ls.Contains(item); }

        public void CopyTo(GLTFElement[] array, int arrayIndex) { ls.CopyTo(array, arrayIndex); }

        public IEnumerator<GLTFElement> GetEnumerator() { return ls.GetEnumerator(); }

        public bool Remove(GLTFElement item) { return ls.Remove(item); }

        IEnumerator IEnumerable.GetEnumerator() { return ls.GetEnumerator(); }

        public int IndexOf(GLTFElement el) { return ls.IndexOf(el); }

        public string ToString(bool isCompressJSONStr = true)
        {
            Formatting fmt = isCompressJSONStr ? Formatting.Indented : Formatting.None;
            string jsonStr = JsonConvert.SerializeObject(this, fmt);
            return jsonStr;
        }

        public void Insert(int index, GLTFElement item)
        {
            ls.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ls.RemoveAt(index);
        }

        public bool Exists(Predicate<GLTFElement> predicate)
        {
            return ls.Exists(predicate);
        }
    }
}
