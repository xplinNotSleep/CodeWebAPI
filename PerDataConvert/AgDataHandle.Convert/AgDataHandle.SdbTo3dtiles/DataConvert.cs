using AgDataHandle.BaseService;
using AgDataHandle.BaseService.B3dmModel;
using AgDataHandle.BaseService.GeometryHelper;
using AgDataHandle.BaseService.GeometryHelper.Collection;
using AgDataHandle.BaseService.GeometryHelper.Def;
using AgDataHandle.BaseService.GeometryHelper.Param;
using AgDataHandle.BaseService.GltfSdk.Def;
using AgDataHandle.BaseService.GltfSdk.ENUM;
using AgDataHandle.BaseService.I3dmModel;
using AgDataHandle.Maths;
using AgDataHandle.Maths.Numerics;
using AgDataHandle.Maths.SpatialAlternation;
using DotSpatial.Projections;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Pure.Data;
using ServiceCenter.Core;

namespace AgDataHandle.SdbTo3dtiles
{
    public class DataConvert
    {
        static List<string> sdbTables = new List<string>() { "MunicipalDesignParameters", "PipeSegments", "PipeFittings", "Wells" };
        public static void ToConvertGltf(string sdbPath, string savePath, string ExtraParam = null)
        {
            To3dtilesSetting setting;
            if (ExtraParam.IsNotNullOrEmpty())
            {
                setting = ExtraParam.FromJson<To3dtilesSetting>();
            }
            else
            {
                setting = new To3dtilesSetting();
            }
            var dbService = new Database("Data Source=" + sdbPath, DatabaseType.SQLite);
            var geometryModel = GetGeometryModel(dbService, setting.ModelUnitChange);
            var gltf=geometryModel.ToGltf(new GeometryConvertParam() { IsUseExtension = setting.IsUseExtension,IsInvertYZ=setting.InvertYZ });
            gltf.Save(savePath);
        }

        public static void ToConvert(string sdbPath, string savePath, string ExtraParam = null)
        {
            To3dtilesSetting setting;
            if (ExtraParam.IsNotNullOrEmpty())
            {
                setting = ExtraParam.FromJson<To3dtilesSetting>();
            }
            else
            {
                setting = new To3dtilesSetting();
            }
            var dbService = new Database("Data Source=" + sdbPath, DatabaseType.SQLite);
            var geometryModel = GetGeometryModel(dbService, setting.ModelUnitChange);
            var results = geometryModel.To3dtiles(new GeometryConvertParam() { IsUseExtension = setting.IsUseExtension, IsInvertYZ = setting.InvertYZ });
            if (setting.gisPosition == null)
            {
                var projectInfo = dbService.ExecuteModel<dynamic>("select * from ProjectInformations") as IDictionary<string, object>;
                var BasePointX = projectInfo["BasePointX"]?.ToDouble();
                var BasePointY = projectInfo["BasePointY"]?.ToDouble();
                if (BasePointX != null && BasePointY != null)
                {
                    var projection = ProjectionInfo.FromEsriString("PROJCS[\"SHAG2000\",GEOGCS[\"GCS_China_Geodetic_Coordinate_System_2000\",DATUM[\"D_China_2000\",SPHEROID[\"CGCS2000\",6378137.0,298.257222101]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",0.0],PARAMETER[\"False_Northing\",-3457147.81],PARAMETER[\"Central_Meridian\",121.4644444],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]");
                    var radius = new double[] { BasePointX.Value, BasePointY.Value };
                    Reproject.ReprojectPoints(radius, null, projection, KnownCoordinateSystems.Geographic.World.WGS1984, 0, 1);
                    setting.gisPosition = new GisPosition(radius[0], radius[1],0,0);
                }
                else
                {
                    setting.gisPosition = new GisPosition(121.778166f, 31.126223f, 0, 0);
                }
            }
            SaveTilesetToFile(results, savePath, setting);
        }

        /// <summary>
        /// 投影坐标获取后，进一步利用4参数转换坐标
        /// </summary>
        /// <param name="v">原始点</param>
        /// <param name="dx">四参数x偏移</param>
        /// <param name="dy">四参数y偏移</param>
        /// <param name="A">四参数旋转量</param>
        /// <param name="K">四参数</param>
        /// <param name="Dh"></param>
        static void Transform4Para(ref Vector3d v,double dx,double dy,double A,double K,double Dh)
        {
            var X1 = dx;
            var Y1 = dy;

            var cosAngle = Math.Cos(A);
            var sinAngle = Math.Sin(A);

            X1 += K * (cosAngle * v.X - sinAngle * v.Y);
            Y1 += K * (sinAngle * v.X + cosAngle * v.Y);

            v.X = X1;
            v.Y = Y1;
            // 固定改正差
            v.Z += Dh;
        }

        static GeometryModel GetGeometryModel(IDatabase dbService, double ModelUnitChange=1)
        {
            var lodrelations= dbService.ExecuteList<dynamic>("select * from LODRelations").Select(p => p as IDictionary<string, object>).ToList();
            var geometrys = dbService.ExecuteList<dynamic>("select * from Geometrys").Select(p => p as IDictionary<string, object>).ToList();
            var materials = dbService.ExecuteList<dynamic>("select * from Materials").Select(p => p as IDictionary<string, object>).ToList();
            var textures = dbService.ExecuteList<dynamic>("select * from Textures").Select(p => p as IDictionary<string, object>).ToList();
            var provider = dbService.CreateTransformationProvider();
            var alldatas = new List<IDictionary<string, object>>();
            for (int i = 0; i < sdbTables.Count; i++)
            {
                if (!provider.TableExists(sdbTables[i])) continue;
                var datas = dbService.ExecuteList<dynamic>("select * from " + sdbTables[i]).Select(p => p as IDictionary<string, object>).ToList();
                alldatas.AddRange(datas);
            }
            GeometryModel geometryModel = new GeometryModel();
            GeometryNode pNode = new GeometryNode();
            geometryModel.Nodes.Add(pNode);
            pNode.Children = new GeometryNodeCollection();
            Dictionary<string, int> imageMap = new Dictionary<string, int>();
            Dictionary<string, int> materialMap = new Dictionary<string, int>();
            foreach (var texture in textures)
            {
                GLTFImage gLTFImage = new GLTFImage(GLTFImageType.Bytes);
                gLTFImage.Name = texture["name"]?.ToString();
                gLTFImage.MimeType = texture["texturefiletype"]?.ToString() == "1" ? "image/jpeg" : "image/png";
                gLTFImage.BufferType = GLTFBufferType.BinaryData;
                gLTFImage.BinaryData = (texture["file"] as byte[]).ToList();
                gLTFImage.UUID = texture["id"].ToString();
                geometryModel.Images.Add(gLTFImage);
                imageMap.Add(gLTFImage.UUID, geometryModel.Images.Count - 1);
                GLTFTexture gLTFTexture = new GLTFTexture();
                gLTFTexture.Name = texture["name"]?.ToString();
                gLTFTexture.ImageIndex = geometryModel.Images.Count - 1;
                geometryModel.Textures.Add(gLTFTexture);
            }
            foreach (var material in materials)
            {
                GLTFMaterial gltfMaterial = GLTFMaterial.CreateDefault();
                gltfMaterial.UUID = material["id"].ToString();
                gltfMaterial.Name = material["userlabel"]?.ToString();
                gltfMaterial.PbrMetallicRoughness = new GLTFPbrMetallicRoughness();
                var color = new List<float>();
                var rgba = Convert.ToInt64(material["color"]);
                for (var i = rgba; i > 0;)
                {
                    color.Add((float)(i % 256) / 255);
                    i = i / 256;
                }
                gltfMaterial.PbrMetallicRoughness.BaseColorFactor = color.ToArray();
                if (material["textureid"]?.ToString() != null)
                {
                    gltfMaterial.PbrMetallicRoughness.BaseColorTexture = new GLTFTextureInfo();
                    gltfMaterial.PbrMetallicRoughness.BaseColorTexture.Index = imageMap[material["textureid"].ToString()];
                }
                geometryModel.Materials.Add(gltfMaterial);
                materialMap.Add(gltfMaterial.UUID, geometryModel.Materials.Count - 1);
            }
            foreach (var lodinfo in lodrelations)
            {
                var geometryId = lodinfo["geometryid"].ToString();
                var geometry = geometrys.FirstOrDefault(p => p["id"].ToString() == geometryId);
                if (geometry == null) continue;
                var attr=alldatas.FirstOrDefault(p => p["id"].ToString() == lodinfo["graphicelementid"].ToString());
                GeometryNode geometryNode = new GeometryNode();
                geometryNode.pNode = pNode;
                geometryNode.Extensions = attr?.ToDictionary(p => p.Key, p => new JObject { { "Value", p.Value?.ToString()??"" } });
                pNode.Children.Add(geometryNode);
                GeometryMesh geometryMesh = new GeometryMesh();
                geometryNode.Mesh = geometryMesh;
                geometryNode.MeshId = geometryModel.Meshes.Count;
                geometryModel.Meshes.Add(geometryMesh);

                var materialidsStr = geometry["materialids"].ToString();
                var materialids = StringToList<string>(materialidsStr);
                var vertices = StringToList<double>(geometry["vertices"].ToString());
                if(ModelUnitChange!=1f)
                {
                    for(int i = 0;i<vertices.Count;i++) {
                        vertices[i] *= (double)ModelUnitChange;
                    }
                }
                var vertexIndexes = StringToList<int>(geometry["vertexindexes"].ToString());
                var normalsStr = geometry["normals"]?.ToString();
                var normalIndexesStr = geometry["normalindexes"]?.ToString();
                var uvsStr = geometry["texturecoords"]?.ToString();
                var uvIndexesStr = geometry["texturecoordindexes"]?.ToString();
                var normals = StringToList<double>(normalsStr);
                var normalsIndexes = StringToList<int>(normalIndexesStr);
                var uvs = StringToList<double>(uvsStr);
                var uvsIndexes = StringToList<int>(uvIndexesStr);
                List<Vector3> vertexsTmp = new List<Vector3>();
                for (int i = 0; i < vertices.Count; i += 3)
                {
                    vertexsTmp.Add(new Vector3(vertices[i], vertices[i + 1], vertices[i + 2]));
                }
                List<Vector3> normalsTmp = new List<Vector3>();
                for (int i = 0; i < normals.Count; i += 3)
                {
                    normalsTmp.Add(new Vector3(normals[i], normals[i + 1], normals[i + 2]));
                }
                List<Vector2> uvsTmp = new List<Vector2>();
                for (int i = 0; i < uvs.Count; i += 2)
                {
                    uvsTmp.Add(new Vector2(uvs[i], uvs[i + 1]));
                }
                List<List<object>> faceRanges = new List<List<object>>();
                var materails = materialids.Distinct().ToList();
                if (materails.Count > 0)
                {
                    for (int k = 0; k < materails.Count; k++)
                    {
                        var index = materialids.FindIndex(a => a == materails[k]);
                        var endIndex = materialids.Count - 1;
                        if (k + 1 < materails.Count)
                        {
                            endIndex = materialids.FindIndex(a => a == materails[k + 1]);
                        }

                        faceRanges.Add(new List<object> { index, endIndex, materails[k] });
                    }
                }
                else
                {
                    faceRanges.Add(new List<object> { 0, vertexIndexes.Count / 3 - 1, materails[0] });
                }

                for (int k = 0; k < faceRanges.Count; k++)
                {
                    var start = (int)faceRanges[k][0];
                    var end = (int)faceRanges[k][1];

                    GeometryPrimitive primitive = new GeometryPrimitive();
                    int nTriIndex = 0;
                    Dictionary<string, int> dictVNU = new Dictionary<string, int>();
                    for (int j = start; j <= end; j++)
                    {
                        var tri = new Vector3Int();
                        for (int x = 0; x < 3; x++)
                        {
                            string vnu = string.Empty;
                            Vector3 arr = vertexsTmp[vertexIndexes[j * 3 + x]];
                            vnu += "v" + arr.ToString();
                            Vector3 arrNormal = null;
                            if (normals != null && normals.Count > 0)
                            {
                                if (normalsIndexes[j * 3 + x] != -1)
                                {
                                    arrNormal = normalsTmp[normalsIndexes[j * 3 + x]];
                                    vnu += "n" + arrNormal.ToString();
                                }
                            }
                            Vector2 arrUv = null;
                            if (uvs != null && uvs.Count > 0)
                            {
                                if (uvsIndexes[j * 3 + x] != -1)
                                {
                                    arrUv = uvsTmp[uvsIndexes[j * 3 + x]];
                                    vnu += "u" + arrUv.ToString();
                                }
                            }
                            if (dictVNU.ContainsKey(vnu))
                            {
                                tri[x] = dictVNU[vnu];
                            }
                            else
                            {
                                tri[x] = nTriIndex;
                                dictVNU.Add(vnu, nTriIndex);
                                var vertice = new Vector3(arr[0], arr[1], arr[2]);
                                if (primitive.Vertices == null)
                                {
                                    primitive.Vertices = new List<Vector3>();
                                }
                                primitive.Vertices.Add(vertice);
                                if (arrNormal != null)
                                {
                                    var normal = new Vector3(arrNormal[0], arrNormal[1], arrNormal[2]);
                                    if (primitive.Normals == null)
                                    {
                                        primitive.Normals = new List<Vector3>();
                                    }
                                    primitive.Normals.Add(normal);
                                }
                                if (arrUv != null)
                                {
                                    if (primitive.Uvs == null)
                                    {
                                        primitive.Uvs = new List<Vector2>();
                                    }
                                    primitive.Uvs.Add(new Vector2(arrUv[0], arrUv[1]));
                                }
                                nTriIndex++;
                            }
                        }
                        if (primitive.Triangles == null)
                        {
                            primitive.Triangles = new List<Vector3Int>();
                        }
                        primitive.Triangles.Add(tri);
                    }
                    var materialid = faceRanges[k][2].ToString();
                    primitive.Material = geometryModel.Materials[materialMap[materialid]];
                    primitive.MaterialIndex = materialMap[materialid];
                    if (primitive.Material.PbrMetallicRoughness.BaseColorTexture != null)
                    {
                        primitive.Image = new GeometryImage();
                        primitive.Image.PbrImage = geometryModel.Images[primitive.Material.PbrMetallicRoughness.BaseColorTexture.Index];
                    }
                    geometryMesh.Primitives.Add(primitive);
                }
            }
            return geometryModel;
        }

        static void SaveTilesetToFile(List<SplitGltfResult> results, string outputDirectoryPath, To3dtilesSetting setting)
        {
            Tileset tileset = new Tileset();
            tileset.GeometricError = 500;
            tileset.Asset = new TilesetAsset();
            tileset.Root = new Tile();
            tileset.Root.Children = new List<Tile>();
            tileset.Root.Refine = "REPLACE";
            tileset.Root.GeometricError = 500;
            int nIndex = 1;
            tileset.Root.BoundingVolume = new BoundingVolume();
            foreach (var result in results)
            {
                Tile tile = new Tile();
                if (result.I3dmMatrixs == null)
                {
                    B3dm b3dm = new B3dm(result.GLTF);
                    b3dm.featureTableJsonCount = result.BatchLength;
                    b3dm.BatchTableJson = JsonConvert.SerializeObject(result.Attrs, new KeyValuePairConverter());
                    b3dm.Export(Path.Combine(outputDirectoryPath, "model", nIndex.ToString() + ".b3dm"));
                    tile.Content = new TileContent();
                    tile.Content.Url = "model" + "/" + nIndex.ToString() + ".b3dm";//前端加载要求左斜杠
                }
                else
                {
                    I3dm i3dm = new I3dm(result.GLTF, result.I3dmMatrixs, result.Attrs);
                    i3dm.Export(Path.Combine(outputDirectoryPath, "model", nIndex.ToString() + ".i3dm"));
                    tile.Content = new TileContent();
                    tile.Content.Url = "model" + "/" + nIndex.ToString() + ".i3dm";//前端加载要求左斜杠
                }
                tile.Refine = "REPLACE";
                tile.GeometricError = 250;
                tile.BoundingVolume = new BoundingVolume();
                if (setting.InvertYZ)
                {
                    tile.BoundingVolume.Box = new double[] {
                (result.BoundingBox.MinX + result.BoundingBox.MaxX) / 2, -(result.BoundingBox.MinZ + result.BoundingBox.MaxZ) / 2,
                (result.BoundingBox.MinY + result.BoundingBox.MaxY) / 2, (result.BoundingBox.MaxX - result.BoundingBox.MinX) / 2,
                0, 0, 0, -(result.BoundingBox.MaxZ - result.BoundingBox.MinZ) / 2, 0, 0, 0, (result.BoundingBox.MaxY - result.BoundingBox.MinY) / 2 };
                }
                else
                {
                    tile.BoundingVolume.Box = new double[] {
                (result.BoundingBox.MinX + result.BoundingBox.MaxX) / 2, (result.BoundingBox.MinY + result.BoundingBox.MaxY) / 2,
                (result.BoundingBox.MinZ + result.BoundingBox.MaxZ) / 2, (result.BoundingBox.MaxX - result.BoundingBox.MinX) / 2,
                0, 0, 0, (result.BoundingBox.MaxY - result.BoundingBox.MinY) / 2, 0, 0, 0, (result.BoundingBox.MaxZ - result.BoundingBox.MinZ) / 2 };
                }

                tileset.Root.Children.Add(tile);
                nIndex++;
            }
            //var lonmeter = (double)LatLonHelper.LongitudeToMeters(setting.gisPosition.Longitude, setting.gisPosition.Latitude);
            //var latmeter = (double)LatLonHelper.LatitudeToMeters(setting.gisPosition.Latitude);
            //setting.gisPosition.CheckLatLonIsDegree();
            var matrix = GisUtilDouble.Wgs84Transform(setting.gisPosition.Longitude, setting.gisPosition.Latitude, 0);
            tileset.Root.Transform = matrix.ToDoubleArray();

            var minX = results.Min(p => p.BoundingBox.MinX);
            var maxX = results.Max(p => p.BoundingBox.MaxX);
            var minY = results.Min(p => p.BoundingBox.MinY);
            var maxY = results.Max(p => p.BoundingBox.MaxY);
            var minZ = results.Min(p => p.BoundingBox.MinZ);
            var maxZ = results.Max(p => p.BoundingBox.MaxZ);
            if (setting.InvertYZ) // 更新模型朝向时同步更新模型包围盒方向
            {
                tileset.Root.BoundingVolume.Box = new double[] {
                (minX + maxX) / 2, -(minZ + maxZ) / 2,
                (minY + maxY) / 2, (maxX - minX) / 2,
                0, 0, 0, -(maxZ - minZ) / 2, 0, 0, 0, (maxY - minY) / 2 };
                tileset.Asset.GltfUpAxis = "Y";
            }
            else
            {
                tileset.Root.BoundingVolume.Box = new double[] {
                (minX + maxX) / 2, (minY + maxY) / 2,
                (minZ + maxZ) / 2, (maxX - minX) / 2,
                0, 0, 0, (maxY - minY) / 2, 0, 0, 0, (maxZ - minZ) / 2 };
                tileset.Asset.GltfUpAxis = "Z";
            }


            File.WriteAllText(Path.Combine(outputDirectoryPath, "tileset.json"), JsonConvert.SerializeObject(tileset, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        private static List<T> StringToList<T>(string str)
        {
            if (string.IsNullOrEmpty(str)) return null;
            List<T> list = new List<T>();
            string[] array = str.Split(',');
            if (array.Length < 2)
            {
                return null;
            }

            list.Add((T)Convert.ChangeType(array[0].Replace("[", ""), typeof(T)));
            for (int i = 1; i < array.Length - 1; i++)
            {
                list.Add((T)Convert.ChangeType(array[i], typeof(T)));
            }

            list.Add((T)Convert.ChangeType(array[^1].Replace("]", ""), typeof(T)));
            return list;
        }
    }

    /// <summary>
    /// 转3dtiles设置，此配置针对webapi接口
    /// </summary>
    public class To3dtilesSetting
    {
        /// <summary>
        /// 模型单位转换，单位都要转换为米
        /// </summary>
        public double ModelUnitChange { get; set; } = 1;

        /// <summary>
        /// 翻转YZ,3DTiles朝向，true-Z轴朝上，false-Y轴朝上        
        /// </summary>
        public bool InvertYZ { get; set; } = false;
        /// <summary>
        /// gis位置
        /// </summary>
        public GisPosition gisPosition { get; set; }// = new GisPosition(121.778166f, 31.126223f, 0, 0);
        /// <summary>
        /// 是否使用属性
        /// </summary>
        public bool IsUseExtension { get; set; } = true;
    }
}