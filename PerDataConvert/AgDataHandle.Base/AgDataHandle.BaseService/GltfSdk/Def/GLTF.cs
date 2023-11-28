using AgDataHandle.BaseService.GltfSdk.Collection;
using AgDataHandle.BaseService.GltfSdk.CustomConvertor;
using AgDataHandle.BaseService.GltfSdk.ENUM;
using AgDataHandle.BaseService.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgDataHandle.BaseService.GltfSdk.Def
{
    [JsonObject(ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
    public class GLTF : IDisposable
    {
        public GLTF(bool isReturnEmpty = false)
        {
            if (!isReturnEmpty)
            {
                Asset = new GLTFAsset();
                Buffers = new GLTFBufferCollection();
                BufferViews = new GLTFBufferViewCollection();
                Accessors = new GLTFAccessorCollection();
                Scenes = new GLTFSceneCollection();
                Nodes = new GLTFNodeCollection();
                Meshes = new GLTFMeshCollection();
                Materials = new GLTFMaterialCollection();
                Textures = new GLTFTextureCollection();
                ExtensionJsonInfo = new GLTFExtensionInfo();
            }
        }

        public void BindGLTF()
        {
            Asset.GLTF = this;
            Buffers.GLTF = this;
            BufferViews.GLTF = this;
            Accessors.GLTF = this;
            Scenes.GLTF = this;
            Nodes.GLTF = this;
            Meshes.GLTF = this;
            if (Materials != null)
                Materials.GLTF = this;
            if (Textures != null)
                Textures.GLTF = this;
            if (Images != null)
                Images.GLTF = this;
            if (Samplers != null)
                Samplers.GLTF = this;
        }


        #region Properties

        [JsonProperty("asset", Required = Required.Always, Order = 0)]
        public GLTFAsset Asset { get; set; }
        [JsonProperty("scene", Required = Required.DisallowNull, Order = 1)]
        public int Scene { get; set; }
        [JsonProperty("buffers", Required = Required.Always, Order = 2)]
        public GLTFBufferCollection Buffers { get; set; }
        [JsonProperty("bufferViews", Required = Required.Always, Order = 3)]
        public GLTFBufferViewCollection BufferViews { get; set; }
        [JsonProperty("accessors", Required = Required.Always, Order = 4)]
        public GLTFAccessorCollection Accessors { get; set; }
        [JsonProperty("scenes", Required = Required.Always, Order = 9)]
        public GLTFSceneCollection Scenes { get; set; }
        [JsonProperty("nodes", Required = Required.Always, Order = 8)]
        public GLTFNodeCollection Nodes { get; set; }
        [JsonProperty("meshes", Required = Required.Always, Order = 5)]
        public GLTFMeshCollection Meshes { get; set; }
        [JsonProperty("materials", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 6)]
        public GLTFMaterialCollection Materials { get; set; }
        [JsonProperty("textures", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 10)]
        public GLTFTextureCollection Textures { get; set; }
        [JsonProperty("images", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 7)]
        public GLTFImageCollection Images { get; set; }
        [JsonProperty("samplers", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 11)]
        public GLTFSamplerCollection Samplers { get; set; }
        [JsonProperty("extensions", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 12)]
        public Dictionary<string, JObject> Extensions { get; set; }
        [JsonProperty("extensionsUsed", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 13)]
        public List<string> ExtensionsUsed { get; set; }

        [JsonProperty("extensionsRequired", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore, Order = 14)]
        public List<string> ExtensionsRequired { get; set; }
        /// <summary>
        /// 原有的gltf的路径
        /// </summary>
        [JsonIgnore]
        public string SourceDirectoryName { get; set; }
        /// <summary>
        /// 当前的gltf的路径
        /// </summary>
        [JsonIgnore]
        public string DirectoryName { get; set; }
        [JsonIgnore]
        public string FullName { get; set; }
        [JsonIgnore]
        public GLTFExtensionInfo ExtensionJsonInfo { get; set; }
        /// <summary>
        /// 标记Mesh和对应的Primitive状态
        /// </summary>
        [JsonIgnore]
        public GLTFMeshPrimitiveState gLTFMeshPrimitiveState { get; set; } = GLTFMeshPrimitiveState.MeshWithMultiPrimitives;
        #endregion

        #region Methods
        public void ShakeEmptyGLTFElement()
        {
            if (Samplers != null && Samplers.Count == 0)
            {
                if (Textures != null && Textures.Count != 0)
                    foreach (var tex in Textures)
                        tex.SamplerIndex = -1;
                Samplers = null;
            }
            if (Images != null && Images.Count == 0)
                Images = null;
            if (Textures != null && Textures.Count == 0)
            {
                Samplers = null;
                Images = null;
                Textures = null;
            }
            if (Materials != null && Materials.Count == 0)
            {
                Samplers = null;
                Textures = null;
                Images = null;
                Materials = null;
            }
        }

        public void ShakeExt(bool extensions = true, bool extras = true)
        {
            foreach (var nd in Nodes)
            {
                nd.Extensions = null;
                nd.Extras = null;
            }
            foreach (var mesh in Nodes)
            {
                mesh.Extensions = null;
                mesh.Extras = null;
            }
            foreach (var mt in Materials)
            {
                mt.Extensions = null;
                mt.Extras = null;
            }
        }

        public string GetJSONStr(bool glb = false)
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new GLTFExtensionConvertor());
            settings.Converters.Add(new GLTFBufferViewConvertor());
            settings.Converters.Add(new GLTFAccessorConvertor());
            settings.Converters.Add(new GLTFNodeConvertor());
            settings.Converters.Add(new GLTFMeshConvertor());
            settings.Converters.Add(new GLTFMaterialConvertor());
            settings.Converters.Add(new GLTFImageConvertor());
            settings.Converters.Add(new GLTFTextureInfoConvertor());
            settings.Converters.Add(new GLTFPbrMetallicRoughnessConvertor());
            settings.Converters.Add(new GLTFOcclusionTextureInfoConvertor());
            settings.Converters.Add(new GLTFTextureConvertor());
            if (glb)
            {
                settings.Converters.Add(new GLTFBufferConvertor());
            }
            string gltfJSONStr = JsonConvert.SerializeObject(this, settings);
            return gltfJSONStr;
        }

        public GLTF Save(string jsonFilePath)
        {
            if (jsonFilePath == null)
                if (string.IsNullOrEmpty(FullName))
                    throw new Exception("请设置 GLTF.FullName 或 传入参数 path");
                else
                    jsonFilePath = FullName;

            var binFileName = Path.ChangeExtension(Path.GetFileName(jsonFilePath), ".bin");
            Buffers[0].Uri = binFileName;
            string dirName = Path.GetDirectoryName(jsonFilePath);
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
            DirectoryName = dirName;//要输出的gltf路径
            // merge and save bin
            for (int i = Buffers.Count - 1; i >= 0; i--)
            {
                if (Buffers[i].ByteLength <= 0)
                    Buffers.RemoveAt(i);
            }
            Buffers.MergeBufferIntoOne(dirName);
            string binFileFullName = PathLunix.Combine(dirName, binFileName);
            if (File.Exists(binFileFullName))
            {
                binFileFullName = PathLunix.Combine(dirName, Path.GetFileName(jsonFilePath));
                binFileFullName = Path.ChangeExtension(binFileFullName, "bin");
                Buffers[0].Uri = Path.GetFileName(binFileFullName);
            }
            // serializer settings
            string gltfJSONStr = GetJSONStr();
            File.WriteAllText(jsonFilePath, gltfJSONStr);
            if (Images != null && Images.Count > 0)
            {
                foreach (var image in Images)
                {
                    image.CopyImage();
                }
            }

            if (ExtensionJsonInfo.Count > 0)
            {
                string JsonPath = Path.ChangeExtension(jsonFilePath, "json");
                JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                });
                //savePath为保存路径
                StreamWriter sw = new StreamWriter(JsonPath, true);
                //json为一个对象
                serializer.Serialize(new JsonTextWriter(sw), ExtensionJsonInfo);
                //清理当前编写器的所有缓冲区，使所有缓冲数据写入基础设备
                sw.Flush();
                //关闭当前的 StringWriter 和基础流
                sw.Close();
            }

            using (FileStream fStream = new FileStream(binFileFullName, FileMode.Create, FileAccess.Write))
            {
                if (Buffers[0].BufferType == GLTFBufferType.BinaryData)
                {
                    foreach (byte b in Buffers[0].BinaryData)
                    {
                        fStream.WriteByte(b);
                    }
                }
                else
                {
                    Buffers[0].ResetPosition();
                    var bf0DataStream = Buffers[0].DataStream;
                    bf0DataStream.CopyTo(fStream);

                    int bt;
                    while ((bt = bf0DataStream.ReadByte()) != -1)
                        fStream.WriteByte((byte)bt);
                }
            }
            return this;
        }

        /// <summary>
        /// 转换gltf时，按照对应类型转换Json文件，入库时会审核Json数据类型
        /// </summary>
        /// <param name="jsonFilePath"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public GLTF Save(string jsonFilePath, string dataType)
        {
            if (jsonFilePath == null)
                if (string.IsNullOrEmpty(FullName))
                    throw new Exception("请设置 GLTF.FullName 或 传入参数 path");
                else
                    jsonFilePath = FullName;

            var binFileName = Path.ChangeExtension(Path.GetFileName(jsonFilePath), ".bin");
            Buffers[0].Uri = binFileName;
            string dirName = Path.GetDirectoryName(jsonFilePath);
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
            if (DirectoryName == null)
            {
                DirectoryName = dirName;
            }
            // merge and save bin
            Buffers.MergeBufferIntoOne(dirName);
            string binFileFullName = PathLunix.Combine(dirName, binFileName);
            if (File.Exists(binFileFullName))
            {
                binFileFullName = PathLunix.Combine(dirName, Path.GetFileName(jsonFilePath));
                binFileFullName = Path.ChangeExtension(binFileFullName, "bin");
                Buffers[0].Uri = Path.GetFileName(binFileFullName);
            }
            // serializer settings
            string gltfJSONStr = GetJSONStr();
            File.WriteAllText(jsonFilePath, gltfJSONStr);
            if (Images != null && Images.Count > 0)
            {
                foreach (var image in Images)
                {
                    image.CopyImage();
                }
            }

            if (ExtensionJsonInfo.Count > 0)
            {
                if (dataType == "IFC")
                {
                    //重新赋值IFC入库标识，后续入库判断此标识
                    ExtensionJsonInfo.gltfsorce = "IFCConvert";
                }
                if (dataType == "OBJ")
                {
                    //重新赋值OBJ入库标识，后续入库判断此标识
                    ExtensionJsonInfo.gltfsorce = "OBJConvert";
                }

                string eleJson = JsonConvert.SerializeObject(ExtensionJsonInfo, new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                });
                string JsonPath = Path.ChangeExtension(jsonFilePath, "json");
                File.WriteAllText(JsonPath, eleJson);
            }

            using (FileStream fStream = new FileStream(binFileFullName, FileMode.Create, FileAccess.Write))
            {
                if (Buffers[0].BufferType == GLTFBufferType.BinaryData)
                {
                    foreach (byte b in Buffers[0].BinaryData)
                    {
                        fStream.WriteByte(b);
                    }
                }
                else
                {
                    Buffers[0].ResetPosition();
                    var bf0DataStream = Buffers[0].DataStream;
                    bf0DataStream.CopyTo(fStream);

                    int bt;
                    while ((bt = bf0DataStream.ReadByte()) != -1)
                        fStream.WriteByte((byte)bt);
                }
            }
            return this;
        }


        public GLTF Update()
        {
            var jsonFilePath = FullName;
            var binFileName = Path.ChangeExtension(Path.GetFileName(jsonFilePath), ".bin");
            Buffers[0].Uri = binFileName;
            string dirName = Path.GetDirectoryName(jsonFilePath);
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
            if (DirectoryName == null)
            {
                DirectoryName = dirName;
            }
            // merge and save bin
            Buffers.MergeBufferIntoOne(dirName);
            string binFileFullName = PathLunix.Combine(dirName, binFileName);
            if (File.Exists(binFileFullName))
            {
                binFileFullName = PathLunix.Combine(dirName, Path.GetFileName(jsonFilePath));
                binFileFullName = Path.ChangeExtension(binFileFullName, "bin");
                Buffers[0].Uri = Path.GetFileName(binFileFullName);
            }
            // serializer settings
            string gltfJSONStr = GetJSONStr();
            File.WriteAllText(jsonFilePath, gltfJSONStr);
            if (Images != null && Images.Count > 0)
            {
                foreach (var image in Images)
                {
                    image.CopyImage();
                }
            }

            if (ExtensionJsonInfo.Count > 0)
            {
                string eleJson = JsonConvert.SerializeObject(ExtensionJsonInfo, new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                });
                string JsonPath = Path.ChangeExtension(jsonFilePath, "json");
                File.WriteAllText(JsonPath, eleJson);
            }
            return this;
        }
        public GLTF UpdateGltfBin()
        {
            var jsonFilePath = FullName;
            var binFileName = Path.ChangeExtension(Path.GetFileName(jsonFilePath), ".bin");
            Buffers[0].Uri = binFileName;
            string dirName = Path.GetDirectoryName(jsonFilePath);
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
            if (DirectoryName == null)
            {
                DirectoryName = dirName;
            }
            // merge and save bin
            Buffers.MergeBufferIntoOne(dirName);
            string binFileFullName = PathLunix.Combine(dirName, binFileName);
            if (File.Exists(binFileFullName))
            {
                binFileFullName = PathLunix.Combine(dirName, Path.GetFileName(jsonFilePath));
                binFileFullName = Path.ChangeExtension(binFileFullName, "bin");
                Buffers[0].Uri = Path.GetFileName(binFileFullName);
            }
            // serializer settings
            string gltfJSONStr = GetJSONStr();
            File.WriteAllText(jsonFilePath, gltfJSONStr);
            if (Images != null && Images.Count > 0)
            {
                foreach (var image in Images)
                {
                    image.CopyImage();
                }
            }

            if (ExtensionJsonInfo.Count > 0)
            {
                string eleJson = JsonConvert.SerializeObject(ExtensionJsonInfo, new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                });
                string JsonPath = Path.ChangeExtension(jsonFilePath, "json");
                File.WriteAllText(JsonPath, eleJson);
            }
            //var testbin = Path.GetDirectoryName(binFileFullName) + "//test.bin";
            using (FileStream fStream = new FileStream(binFileFullName, FileMode.Create, FileAccess.Write))
            //using (FileStream fStream = new FileStream(testbin, FileMode.Create, FileAccess.Write))
            {
                foreach (byte b in Buffers[0].BinaryData)
                {
                    fStream.WriteByte(b);
                }
                //if (Buffers[0].BufferType == GLTFBufferType.BinaryData)
                //{
                //    foreach (byte b in Buffers[0].BinaryData)
                //    {
                //        fStream.WriteByte(b);
                //    }
                //}
                //else
                //{
                //    Buffers[0].ResetPosition();
                //    var bf0DataStream = Buffers[0].DataStream;
                //    bf0DataStream.CopyTo(fStream);

                //    int bt;
                //    while ((bt = bf0DataStream.ReadByte()) != -1)
                //        fStream.WriteByte((byte)bt);
                //}
            }
            return this;
        }


        /// <summary>
        /// 通过注入生成nodes跟meshes等信息
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static GLTF ConvertByAction(Action<GLTF> action)
        {
            GLTF gltf = new GLTF();
            var buffer = new GLTFBuffer();
            gltf.Buffers.Add(buffer);
            buffer.DataStream = new MemoryStream();
            gltf.Scenes.Add(new GLTFScene());
            action(gltf);
            if (gltf.Images != null && gltf.Images.Count > 0)
            {
                foreach (var image in gltf.Images)
                {
                    GLTFBufferView bfv = new GLTFBufferView();
                    bfv.BufferIndex = 0;
                    bfv.ByteOffset = buffer.ByteLength;
                    bfv.ByteLength = image.ImageSize;
                    gltf.BufferViews.Add(bfv);
                    buffer.AddRange(image.GetImageData());
                    image.ImageType = GLTFImageType.BufferView;
                    image.BufferViewIndex = gltf.BufferViews.Count - 1;
                }
                if (gltf.Samplers == null)
                {
                    //取样器,定义图片的采样和滤波方式
                    GLTFSampler sampler = new GLTFSampler();
                    sampler.MagFilter = GLTFMagFilter.Linear.GetHashCode();
                    sampler.MinFilter = GLTFMinFilter.LinearMipmapLinear.GetHashCode();
                    sampler.WrapS = GLTFWrappingMode.Repeat.GetHashCode();
                    sampler.WrapT = GLTFWrappingMode.Repeat.GetHashCode();
                    gltf.Samplers = new GLTFSamplerCollection
                {
                    sampler
                };
                }
            }
            else
            {
                gltf.Textures = null;
                gltf.Images = null;
            }
            gltf.BindGLTF();
            return gltf;
        }

        /// <summary>
        /// 释放引用的二进制文件；如果此 GLTF 由 GLB/B3dm 引用，请慎重调用此方法
        /// </summary>
        public void Dispose()
        {
            foreach (var bf in Buffers)
            {
                bf.DataStream.Dispose();
                bf.DataStream.Close();
            }
        }

        #endregion

        [JsonIgnore]
        public static bool IgnoreExtAndExtras { get; set; }

        public static GLTF LoadFromJsonReader(JsonReader reader)
        {
            GLTF model = new GLTF();

            if (reader.Read() && reader.TokenType != JsonToken.StartObject)
            {
                throw new Exception("gltf json must be an object");
            }
            while (reader.TokenType == JsonToken.PropertyName || reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                string curProp = reader.Value.ToString();
                switch (curProp)
                {
                    case "extensionsUsed":
                        model.ExtensionsUsed = model.ExtensionsUsed ?? new List<string>();
                        reader.Read();
                        while (reader.TokenType != JsonToken.EndArray)
                        {
                            var q = reader.ReadAsString();
                            if (q == null)
                                break;
                            model.ExtensionsUsed.Add(q);
                        }
                        break;
                    case "extensionsRequired":
                        model.ExtensionsRequired = model.ExtensionsRequired ?? new List<string>();
                        reader.Read();
                        while (reader.TokenType != JsonToken.EndArray)
                        {
                            string ss = reader.ReadAsString();
                            if (ss == null)
                                break;
                            model.ExtensionsRequired.Add(ss);
                        }
                        break;
                    case "extensions":
                        if (IgnoreExtAndExtras)
                        {
                            reader.Skip();
                            break;
                        }
                        if (reader.Read() && reader.TokenType != JsonToken.StartObject)
                        {
                            throw new Exception("GLTF extensions must be an object");
                        }

                        model.Extensions = new Dictionary<string, JObject>();
                        JObject extensions = (JObject)JToken.ReadFrom(reader);
                        foreach (JToken child in extensions.Children())
                        {
                            if (child.Type != JTokenType.Property)
                            {
                                throw new Exception("Children token of extensions should be properties");
                            }
                            JProperty prop = (JProperty)child;
                            model.Extensions.Add(prop.Name, prop.Value as JObject);
                        }
                        break;
                    case "accessors":
                        var accCol = GLTFAccessorCollection.Deserialize(reader, model);
                        model.Accessors = accCol;
                        break;
                    case "asset":
                        GLTFAsset asset = GLTFAsset.Deserialize(reader, model);
                        model.Asset = asset;
                        break;
                    case "buffers":
                        var bfCol = GLTFBufferCollection.Deserialize(reader, model);
                        model.Buffers = bfCol;
                        break;
                    case "bufferViews":
                        var bvCol = GLTFBufferViewCollection.Deserialize(reader, model);
                        model.BufferViews = bvCol;
                        break;
                    case "scene":
                        model.Scene = (int)reader.ReadAsInt32();
                        break;
                    case "scenes":
                        var sceCol = GLTFSceneCollection.Deserialize(reader, model);
                        model.Scenes = sceCol;
                        break;
                    case "nodes":
                        var ndCol = GLTFNodeCollection.Deserialize(reader, model);
                        model.Nodes = ndCol;
                        break;
                    case "meshes":
                        var meshCol = GLTFMeshCollection.Deserialize(reader, model);
                        model.Meshes = meshCol;
                        break;
                    case "materials":
                        var mtCol = GLTFMaterialCollection.Deserialize(reader, model);
                        model.Materials = mtCol;
                        break;
                    case "textures":
                        var texCol = GLTFTextureCollection.Deserialize(reader, model);
                        model.Textures = texCol;
                        break;
                    case "samplers":
                        var splCol = GLTFSamplerCollection.Deserialize(reader, model);
                        model.Samplers = splCol;
                        break;
                    case "images":
                        var imgCol = GLTFImageCollection.Deserialize(reader, model);
                        model.Images = imgCol;
                        break;
                    default:
                        reader.Skip();
                        break;
                }
                if (reader.TokenType == JsonToken.EndObject)
                {
                    //break;
                }
            }
            model.BindGLTF();
            return model;
        }

        #region 从某处加载
        public static GLTF LoadFromFile(string filePath, bool updateImage = true)
        {
            #region 兼容glb
            if (Path.GetExtension(filePath).ToLower() == ".glb")
            {
                var glb = GLB.LoadFromFile(filePath);
                var gltf = glb?.GLTF;
                if (gltf != null)
                {
                    if (gltf.Images != null && gltf.Images.Count > 0 && updateImage)
                        gltf.Images.UpdateStatus();
                    return gltf;
                }
            }
            #endregion
            FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            if (!stream.CanRead)
                return null;
            GLTF model;

            TextReader sr = new StreamReader(stream);
            JsonReader jr = new JsonTextReader(sr);
            IgnoreExtAndExtras = false;
            model = LoadFromJsonReader(jr);
            model.DirectoryName = Path.GetDirectoryName(filePath);
            model.SourceDirectoryName = Path.GetDirectoryName(filePath);
            model.FullName = filePath;
            model.Buffers.UpdateStatus();
            if (model.Images != null && model.Images.Count > 0 && updateImage)
                model.Images.UpdateStatus();
            stream.Close();
            return model;
        }

        public static GLTF LoadFromFileForWrite(string filePath)
        {
            FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            if (!stream.CanRead)
                return null;
            GLTF model;

            TextReader sr = new StreamReader(stream);
            JsonReader jr = new JsonTextReader(sr);
            IgnoreExtAndExtras = false;
            model = LoadFromJsonReader(jr);
            model.DirectoryName = Path.GetDirectoryName(filePath);
            model.SourceDirectoryName = Path.GetDirectoryName(filePath);
            model.FullName = filePath;
            model.Buffers.UpdateStatusForWrite();
            if (model.Images != null && model.Images.Count > 0)
                model.Images.UpdateStatusForWrite();
            stream.Close();
            return model;
        }
        #endregion
    }



    #region Extensions
    /// <summary>
    /// 节点扩展
    /// </summary>
    //public class GltfNodeExtensions
    //{
    //    public string elementId { get; set; }
    //    public string objectId { get; set; }
    //    public string level { get; set; }
    //}
    /// <summary>
    /// 网格扩展
    /// </summary>
    //public class GltfMeshExtensions
    //{
    //    public string uuid { get; set; }
    //    public string name { get; set; }

    //    public string familytype { get; set; }
    //    public string infotype { get; set; }
    //    public string category { get; set; }
    //    public string familyName { get; set; }

    //    public string topologiesinfo { get; set; }
    //    public string elementattributes { get; set; }
    //    public string host { get; set; }
    //}
    /// <summary>
    /// 材质扩展
    /// </summary>
    /// 
    public class GltfMaterialExtensions
    {

        public string uuid { get; set; }
        public string materialid { get; set; }
        public string materialname
        { get; set; }
        public string textruesource { get; set; }

    }

    /// <summary>
    /// json中存储的 rvt的ElementAttribute
    /// </summary>
    //public class RVTElementAttribute
    //{
    //    public string No { get; set; }

    //    public string AttrName { get; set; }
    //    public string AttrValue { get; set; }
    //    public string Unit { get; set; }
    //    public string BuiltInParameter { get; set; }
    //}


    #endregion
}
