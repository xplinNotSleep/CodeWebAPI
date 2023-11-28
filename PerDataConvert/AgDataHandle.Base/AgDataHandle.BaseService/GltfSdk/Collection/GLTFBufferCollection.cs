using AgDataHandle.BaseService.GltfSdk.Def;
using AgDataHandle.BaseService.Utils;
using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.Collection
{
    [JsonArray(false)]
    public class GLTFBufferCollection : GLTFCollection<GLTFBuffer>
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

        public GLTFBuffer MergeBufferIntoOne(string DirectoryName)
        {
            if (Count <= 1)
                return this[0];

            GLTFBuffer firstBuffer = this[0];
            firstBuffer.ResetPosition();
            Stream newStream;
            if (firstBuffer.BufferType == GLTFBufferType.FileStream)
            {
                string absUri = PathLunix.Combine(DirectoryName, firstBuffer.Uri);
                if (!File.Exists(absUri))
                    throw new Exception(string.Format("文件 {0} 不存在", absUri));
                // newStream = new BufferedStream();
                newStream = new FileStream(absUri, FileMode.Append, FileAccess.ReadWrite);
            }
            else
            {
                newStream = new MemoryStream(firstBuffer.ByteLength);
                int bt;
                firstBuffer.IsReadOnly = false;
                while ((bt = firstBuffer.DataStream.ReadByte()) != -1)
                    newStream.WriteByte((byte)bt);
            }
            for (int i = 1; i < Count; i++)
            {
                var nextBuffer = this[i];
                firstBuffer.IsReadOnly = false;
                int bt;
                while ((bt = nextBuffer.DataStream.ReadByte()) != -1)
                    newStream.WriteByte((byte)bt);
            }
            firstBuffer.DataStream = newStream;
            return firstBuffer;
        }

        public void UpdateStatus()
        {
            for (int i = 0; i < Count; i++)
            {
                var item = this[i];
                if (item.BufferType == GLTFBufferType.FileStream)
                {
                    if (item.DataStream == null && string.IsNullOrEmpty(this[i].Uri))
                        throw new Exception(string.Format("buffers[{0}] 引用了空的文件", i));
                    string absUri = PathLunix.Combine(GLTF.DirectoryName, this[i].Uri);
                    if (item.Uri != null)
                    {
                        if (item.Uri.StartsWith("data:"))
                        {
                            var base64 = System.Text.RegularExpressions.Regex.Split(item.Uri, "base64,");
                            byte[] bt = Convert.FromBase64String(base64[1]);
                            var fileLength = bt.Length;
                            MemoryStream stream = new MemoryStream(bt);
                            item.DataStream = stream;
                        }
                        else
                        {
                            var stream = new FileStream(absUri, FileMode.Open, FileAccess.Read);
                            item.DataStream = new MemoryStream(stream.ReadAllBytes());
                            stream.Close();
                        }
                        item.IsReadOnly = true;
                        item.ResetPosition();
                    }
                }
                else // MemoryStream
                {
                    if (item.DataStream != null)
                        item.IsReadOnly = true;
                    else
                        throw new Exception(string.Format("buffers[{0}] 引用了空的内存流", i));
                }
            }
        }

        public void UpdateStatusForWrite()
        {
            for (int i = 0; i < Count; i++)
            {
                var item = this[i];
                if (item.BufferType == GLTFBufferType.FileStream)
                {
                    if (item.DataStream == null && string.IsNullOrEmpty(this[i].Uri))
                        throw new Exception(string.Format("buffers[{0}] 引用了空的文件", i));
                    string absUri = PathLunix.Combine(GLTF.DirectoryName, this[i].Uri);
                    if (item.Uri != null)
                    {
                        if (item.Uri.StartsWith("data:"))
                        {
                            var base64 = System.Text.RegularExpressions.Regex.Split(item.Uri, "base64,");
                            byte[] bt = Convert.FromBase64String(base64[1]);
                            var fileLength = bt.Length;
                            MemoryStream stream = new MemoryStream(bt);
                            item.DataStream = stream;
                            item.IsReadOnly = true;
                            item.ResetPosition();
                        }
                        else
                        {
                            //item.DataStream = new FileStream(absUri, FileMode.Open, FileAccess.ReadWrite);
                            FileStream fs = new FileStream(absUri, FileMode.Open, FileAccess.ReadWrite);
                            int byteLength = (int)fs.Length;
                            byte[] fileBytes = new byte[byteLength];
                            fs.Read(fileBytes, 0, byteLength);
                            fs.Close();
                            MemoryStream mStream = new MemoryStream(fileBytes);
                            item.DataStream = mStream;
                            item.IsReadOnly = true;
                            item.ResetPosition();
                            //fs.Close();
                        }
                        //item.IsReadOnly = true;
                        //item.ResetPosition();
                    }
                }
                else // MemoryStream
                {
                    if (item.DataStream != null)
                        item.IsReadOnly = true;
                    else
                        throw new Exception(string.Format("buffers[{0}] 引用了空的内存流", i));
                }
            }
        }

        internal static GLTFBufferCollection Deserialize(JsonReader reader, GLTF model)
        {
            var col = new GLTFBufferCollection();
            reader.Read(); // token -> startArray
            while (reader.TokenType != JsonToken.EndArray)
            {
                GLTFBuffer bf = GLTFBuffer.Deserialize(reader, model);
                col.Add(bf);
            }
            col.GLTF = model;
            return col;
        }
    }
}
