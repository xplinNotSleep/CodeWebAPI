using AgDataHandle.BaseService.CommonHelper;
using AgDataHandle.BaseService.GltfSdk.ENUM;
using AgDataHandle.BaseService.Utils;
using Newtonsoft.Json;
using System.Drawing.Imaging;

namespace AgDataHandle.BaseService.GltfSdk.Def
{
    /// <summary>
    /// 解析图片数据
    /// 2021年2月2日 陈彪 ，解析数据感觉又错误。
    /// </summary>
    public class GLTFImage : GLTFResourceElement
    {
        public GLTFImage()
        {
            BufferViewIndex = -1;
            UUID = Guid.NewGuid().ToString();
        }

        public GLTFImage(GLTFImageType imageType)
        {
            ImageType = imageType;
            UUID = Guid.NewGuid().ToString();
        }


        #region Properties

        [JsonIgnore]
        public GLTFImageType ImageType { get; set; }


        [JsonProperty("mimeType", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string MimeType { get; set; }

        [JsonProperty("name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// Uri的类型下BufferViewIndex需要为null
        /// </summary>
        [JsonProperty("bufferView", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public int? BufferViewIndex { get; set; }
        private int _ImageSize { get; set; } = 0;
        [JsonIgnore]
        public int ImageSize
        {
            get
            {
                if (ImageType == GLTFImageType.Base64)
                {
                    if (!Uri.StartsWith("data:"))
                        throw new Exception(string.Format("数据有误，uri:{0}", Uri));
                    return Convert.FromBase64String(Uri.Substring(Uri.IndexOf(",") + 1)).Length;
                }
                else if (ImageType == GLTFImageType.Bytes)
                {
                    return BinaryData.Count;
                }
                else if (ImageType == GLTFImageType.BufferView)
                {
                    var bv = GetBufferView();
                    return bv.ByteLength;
                }
                else if (ImageType == GLTFImageType.Uri)
                {
                    if (string.IsNullOrEmpty(Uri))
                        throw new Exception(string.Format("此 image 引用的路径不正确：{0}", Uri));
                    string absUri = string.Empty;
                    if (GLTF != null && !Path.IsPathRooted(Uri))
                    {
                        absUri = PathLunix.Combine(GLTF.DirectoryName, Uri);
                    }
                    else
                    {
                        absUri = Uri;
                    }
                    return (int)new FileInfo(absUri).Length;
                }
                else
                {
                    return _ImageSize;
                }
            }
            set
            {
                _ImageSize = value;
            }
        }
        /// <summary>
        /// 用来排重而已，new的时候自动生成
        /// </summary>
        [JsonIgnore]
        public string UUID { get; set; }
        #endregion

        #region Methods

        public GLTFBufferView GetBufferView()
        {
            if (ImageType != GLTFImageType.BufferView)
                return null;
            else if (BufferViewIndex < 0)
            {
                return null;
                //throw new Exception("[GLTFImage GetBufferView() Error]此 image 数据类型是嵌入 buffer 中的，但 此image 的 bufferView 索引号 却不存在，请检查数据");
            }
            return GLTF.BufferViews[(int)BufferViewIndex];
        }

        /// <summary>
        /// 获得图片的比特位数据。
        /// -1、支持linux环境
        /// -2、如果没有图片，则返回空。这种情况比如在共享材质情况下。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public byte[] GetImageData()
        {
            if (ImageType == GLTFImageType.Base64)
            {
                if (!Uri.StartsWith("data:"))
                    throw new Exception(string.Format("数据有误，uri:{0}", Uri));
                return Convert.FromBase64String(Uri.Substring(Uri.IndexOf(",") + 1));
            }
            else if (ImageType == GLTFImageType.BufferView)
            {
                var bv = GetBufferView();
                var bf = bv.GetBuffer();
                lock (bf)
                {
                    byte[] imgData = bf.ReadData(bv.ByteOffset, bv.ByteLength);
                    return imgData;
                }
            }
            else if (ImageType == GLTFImageType.Bytes)
            {
                return BinaryData.ToArray();
            }
            else
            {
                if (Path.IsPathRooted(Uri))
                {
                    return File.ReadAllBytes(Uri);
                }
                if (string.IsNullOrEmpty(Uri))
                    throw new Exception(string.Format("此 image 引用的路径不正确：{0}", Uri));
                string absUri = PathLunix.Combine(GLTF.SourceDirectoryName, Uri);
                if (absUri != null && File.Exists(absUri))
                {
                    return File.ReadAllBytes(absUri);
                }
                return new byte[] { };
            }
        }

        /// <summary>
        /// 复制现有的url图片
        /// </summary>
        public void CopyImage()
        {
            if (ImageType == GLTFImageType.Uri && GLTF.SourceDirectoryName.IsNotNullOrEmpty() && GLTF.DirectoryName != GLTF.SourceDirectoryName)
            {
                string imageAbsPath = PathLunix.Combine(GLTF.DirectoryName, Uri);
                if (Uri.ContainsAny("\\", "/"))
                {
                    var imageDir = Path.GetDirectoryName(imageAbsPath);
                    if (!Directory.Exists(imageDir))
                    {
                        Directory.CreateDirectory(imageDir);
                    }
                }
                if (!File.Exists(imageAbsPath))
                {
                    File.Copy(PathLunix.Combine(GLTF.SourceDirectoryName, Uri), imageAbsPath);
                }
            }
            else if (ImageType == GLTFImageType.Bytes && !string.IsNullOrEmpty(Uri))
            {
                string imageAbsPath = PathLunix.Combine(GLTF.DirectoryName, Uri);
                if (Uri.ContainsAny("\\", "/"))
                {
                    var imageDir = Path.GetDirectoryName(imageAbsPath);
                    if (!Directory.Exists(imageDir))
                    {
                        Directory.CreateDirectory(imageDir);
                    }
                }
                if (!File.Exists(imageAbsPath))
                {
                    File.WriteAllBytes(imageAbsPath, BinaryData.ToArray());
                }
            }
        }

        private ImageFormat GetImageType(string mimeType)
        {
            ImageFormat imageFormat = ImageFormat.Png;
            string extension = Path.GetExtension(Uri).ToLower().Replace(".", "");
            AgImageFormat format = (AgImageFormat)Enum.Parse(typeof(AgImageFormat), extension);
            switch (format)
            {
                case AgImageFormat.jpg:
                    imageFormat = ImageFormat.Jpeg;
                    break;
                default:
                    imageFormat = ImageFormat.Png;
                    break;
            }

            return imageFormat;
        }

        public void SetMimeType()
        {
            if (string.IsNullOrWhiteSpace(MimeType) && Uri.IsNotNullOrWhiteSpace())
            {
                string extension = Path.GetExtension(Uri).ToLower().Replace(".", "");
                AgImageFormat format = (AgImageFormat)Enum.Parse(typeof(AgImageFormat), extension);
                switch (format)
                {
                    case AgImageFormat.png:
                        MimeType = "image/png";
                        break;
                    case AgImageFormat.jpg:
                        MimeType = "image/jpeg";
                        break;
                    case AgImageFormat.webp:
                        MimeType = "image/webp";
                        break;
                    case AgImageFormat.gif:
                        MimeType = "image/gif";
                        break;
                    case AgImageFormat.ktx2:
                        MimeType = "image/ktx2";
                        break;
                    case AgImageFormat.dds:
                        MimeType = "image/dds";
                        break;
                }
            }
        }

        #endregion


        #region 解析过程
        internal static GLTFImage Deserialize(JsonReader reader, GLTF model)
        {
            if (reader.TokenType == JsonToken.StartArray)
                reader.Read(); // token : startArray -> startObject
            if (reader.TokenType == JsonToken.EndArray) //rainer 20210106
                return null;
            bool hasData = false;
            GLTFImage img = new GLTFImage();
            while (reader.TokenType == JsonToken.PropertyName || reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                string curPropName = reader.Value.ToString();
                hasData = true;
                switch (curPropName)
                {
                    case "name":
                        img.Name = reader.ReadAsString();
                        break;
                    case "mimeType":
                        img.MimeType = reader.ReadAsString();
                        break;
                    case "uri":
                    case "url":
                        img.Uri = reader.ReadAsString();
                        img.ImageType = GLTFImageType.Uri;
                        break;
                    case "bufferView":

                        img.BufferViewIndex = C73.GetInt32(reader);
                        img.ImageType = GLTFImageType.BufferView;
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            img.GLTF = model;
            return hasData ? img : null;
        }
        #endregion
    }
}
