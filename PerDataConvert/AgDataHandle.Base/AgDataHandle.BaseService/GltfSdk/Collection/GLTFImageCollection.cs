using AgDataHandle.BaseService.GltfSdk.Def;
using AgDataHandle.BaseService.GltfSdk.ENUM;
using AgDataHandle.BaseService.Utils;
using Newtonsoft.Json;

namespace AgDataHandle.BaseService.GltfSdk.Collection
{
    /// <summary>
    /// 图片组
    /// </summary>
    [JsonArray(true)]
    public class GLTFImageCollection : GLTFCollection<GLTFImage>
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

        public void UpdateStatus()
        {
            for (int i = 0; i < Count; i++)
            {
                var img = this[i];
                if (img.ImageType == GLTFImageType.Uri) // file
                {
                    if (GLTF.DirectoryName == null)
                        throw new Exception("[GLTFImageCollection UpdateStatus Error]图片引用了文件，但是未找到绝对路径");

                    string absUri = PathLunix.Combine(GLTF.DirectoryName, img.Uri);
                    FileStream stream = new FileStream(absUri, FileMode.Open, FileAccess.Read);
                    img.DataStream = new MemoryStream(stream.ReadAllBytes());
                    img.IsReadOnly = true;
                    img.ResetPosition();
                    stream.Close();
                }
                else if (img.ImageType == GLTFImageType.BufferView) // buffer
                {
                    var bv = img.GetBufferView();
                    if (bv != null)
                    {
                        var bf = bv.GetBuffer();
                        if (bf != null && bf.DataStream != null)
                        {
                            img.DataStream = bf.DataStream;
                            img.StreamStartPosition = bv.ByteOffset;
                            img.IsReadOnly = bf.IsReadOnly;
                        }
                    }
                }
            }
        }

        public void UpdateStatusForWrite()
        {
            for (int i = 0; i < Count; i++)
            {
                var img = this[i];
                if (img.ImageType == GLTFImageType.Uri) // file
                {
                    if (GLTF.DirectoryName == null)
                        throw new Exception("[GLTFImageCollection UpdateStatus Error]图片引用了文件，但是未找到绝对路径");

                    string absUri = PathLunix.Combine(GLTF.DirectoryName, img.Uri);
                    //img.DataStream = new FileStream(absUri, FileMode.Open, FileAccess.ReadWrite);
                    FileStream fs = new FileStream(absUri, FileMode.Open, FileAccess.ReadWrite);
                    img.DataStream = fs;
                    img.IsReadOnly = true;
                    img.ResetPosition();
                    fs.Close();
                }
                else if (img.ImageType == GLTFImageType.BufferView) // buffer
                {
                    var bv = img.GetBufferView();
                    if (bv != null)
                    {
                        var bf = bv.GetBuffer();
                        if (bf != null && bf.DataStream != null)
                        {
                            img.DataStream = bf.DataStream;
                            img.StreamStartPosition = bv.ByteOffset;
                            img.IsReadOnly = bf.IsReadOnly;
                        }
                    }
                }
            }
        }

        [Obsolete]
        public void MergeImagesIntoBuffer()
        {

        }

        internal static GLTFImageCollection Deserialize(JsonReader reader, GLTF model)
        {
            var col = new GLTFImageCollection();
            reader.Read(); // token -> startArray
            while (reader.TokenType != JsonToken.EndArray)
            {
                GLTFImage img = GLTFImage.Deserialize(reader, model);
                if (img != null)
                    col.Add(img);
            }
            col.GLTF = model;
            return col;
        }
    }
}
