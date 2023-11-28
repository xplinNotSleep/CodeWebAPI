using AgDataHandle.Maths;
using AgDataHandle.Maths.SpatialAlternation;
using AgDataHandle.SdbTo3dtiles;
using AgDataHandle.Web.Base;
using DotSpatial.Projections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using ServiceCenter.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AgDataHandle.Web.Controllers
{

    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ConvertController : ApiControllerBase
    {        //定义两个信号量
        static SemaphoreSlim m_semaphore = new SemaphoreSlim(1);
        string _3dtilesPath { get; set; }
        string _3dtilesHost { get; set; }
        public ConvertController()
        {
            _3dtilesPath = Toolset.AppConfigHelper["3dtilesPath"];
            _3dtilesHost = WebRequestHelper.GetHost(Toolset.HttpContext)+ "/3dtiles/";
        }

        /// <summary>
        /// 3d数据格式(sdb等)转成3Dtiles
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("To3DTiles")]
        public async Task<BaseMessage> To3DTiles([FromBody] RequestParam param)
        {
            await m_semaphore.WaitAsync();
            try
            {
                param.inputfile = param.inputfile.Trim('"');
                param.outputpath = param.outputpath.Trim('"');

                Toolset.TinyLogger.WriteLog("To3DTiles开始处理：" + param.inputfile);
                string exportSetting = string.Empty;
                if (param.exportSetting != null)
                {
                    if (param.exportSetting is string)
                    {
                        exportSetting = param.exportSetting.ToString();
                    }
                    else if (param.exportSetting is JsonElement)
                    {
                        var jsonEL = (JsonElement)param.exportSetting;
                        if (jsonEL.ValueKind.ToString() == "String")
                        {
                            exportSetting = param.exportSetting.ToString();
                        }
                        else
                        {
                            exportSetting = jsonEL.GetRawText();
                        }
                    }
                }
                DataConvert.ToConvert(param.inputfile, param.outputpath, exportSetting);
                return Success("转换成功");
            }
            catch (Exception ex)
            {
                Toolset.TinyLogger.WriteLog(ex.ToFriendlyString());
                return Fail(null, "失败", ex);
            }
            finally
            {
                m_semaphore.Release();
            }
        }

        /// <summary>
        /// 3d数据格式(sdb等)转成3Dtiles
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("To3DTilesServer")]
        public async Task<BaseMessage<dynamic>> To3DTilesServer([FromBody] RequestParam3 param)
        {
            await m_semaphore.WaitAsync();
            try
            {
                param.inputfile = param.inputfile.Trim('"');

                Toolset.TinyLogger.WriteLog("To3DTiles开始处理：" + param.inputfile);
                string exportSetting = string.Empty;
                if (param.exportSetting != null)
                {
                    if (param.exportSetting is string)
                    {
                        exportSetting = param.exportSetting.ToString();
                    }
                    else if (param.exportSetting is JsonElement)
                    {
                        var jsonEL = (JsonElement)param.exportSetting;
                        if (jsonEL.ValueKind.ToString() == "String")
                        {
                            exportSetting = param.exportSetting.ToString();
                        }
                        else
                        {
                            exportSetting = jsonEL.GetRawText();
                        }
                    }
                }
                var serverPath = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
                var outputpath = Path.Combine(_3dtilesPath, serverPath);
                DataConvert.ToConvert(param.inputfile, outputpath, exportSetting);
                return Success<dynamic>(new { Server = _3dtilesHost + serverPath + "/tileset.json" }, "转换成功");
            }
            catch (Exception ex)
            {
                Toolset.TinyLogger.WriteLog(ex.ToFriendlyString());
                return Fail<dynamic>(null, "失败", ex);
            }
            finally
            {
                m_semaphore.Release();
            }
        }

        /// <summary>
        /// 3d数据格式(sdb等)转成3Dtiles
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("ToGltf")]
        public async Task<BaseMessage> ToGltf([FromBody] RequestParam2 param)
        {
            await m_semaphore.WaitAsync();
            try
            {
                param.inputfile = param.inputfile.Trim('"');
                param.outputfile = param.outputfile.Trim('"');

                Toolset.TinyLogger.WriteLog("ToGLTF开始处理：" + param.inputfile);
                string exportSetting = string.Empty;
                if (param.exportSetting != null)
                {
                    if (param.exportSetting is string)
                    {
                        exportSetting = param.exportSetting.ToString();
                    }
                    else if (param.exportSetting is JsonElement)
                    {
                        var jsonEL = (JsonElement)param.exportSetting;
                        if (jsonEL.ValueKind.ToString() == "String")
                        {
                            exportSetting = param.exportSetting.ToString();
                        }
                        else
                        {
                            exportSetting = jsonEL.GetRawText();
                        }
                    }
                }
                DataConvert.ToConvertGltf(param.inputfile, param.outputfile, exportSetting);
                return Success("转换成功");
            }
            catch (Exception ex)
            {
                Toolset.TinyLogger.WriteLog(ex.ToFriendlyString());
                return Fail(null, "失败", ex);
            }
            finally
            {
                m_semaphore.Release();
            }
        }

        /// <summary>
        /// 转换国家2000大地坐标为经纬度坐标,中央经线空时默认120
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("UnProject")]
        public async Task<BaseMessage<Vector2d>> UnProject([FromBody] ProjectParam param)
        {
            try
            {
                var coord = new CGCS_2000();
                coord.CentralLongitude = param.CentralLongitude;
                var radius = coord.UnProject(new Vector2d(param.Y, param.X));
                var wgs84 = new Vector2d(radius[1] * 180 / Math.PI, radius[0] * 180 / Math.PI);
                return Success(wgs84, "转换成功");
            }
            catch (Exception ex)
            {
                Toolset.TinyLogger.WriteLog(ex.ToFriendlyString());
                return Fail<Vector2d>(null, "失败", ex);
            }
        }

        /// <summary>
        /// 根据esri投影字符串转换坐标，目标投影为空时默认经纬度
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("UnProjectByPrjString")]
        public async Task<BaseMessage<Vector2d>> UnProjectWgs84ByPrjString([FromBody] ProjectParam2 param)
        {
            try
            {
                var projection = GetProjectionInfo(param.Proj);
                if (projection == null)
                {
                    return Fail<Vector2d>(null, "原始点投影信息输入有误");
                }
                ProjectionInfo destProjection = null;
                if(param.TargetProj.IsNullOrEmpty())
                {
                    destProjection = KnownCoordinateSystems.Geographic.World.WGS1984;
                }
                else
                {
                    destProjection= GetProjectionInfo(param.TargetProj);
                }
                if (destProjection == null)
                {
                    return Fail<Vector2d>(null, "原始点投影信息输入有误");
                }
                var radius = new double[] { param.X, param.Y };
                Reproject.ReprojectPoints(radius, null, projection, destProjection, 0, 1);
                var wgs84 = new Vector2d(radius[0], radius[1]);
                return Success(wgs84, "转换成功");
            }
            catch (Exception ex)
            {
                Toolset.TinyLogger.WriteLog(ex.ToFriendlyString());
                return Fail<Vector2d>(null, "失败", ex);
            }
        }

        /// <summary>
        /// 批量根据esri投影字符串转换坐标，目标投影为空时默认经纬度
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("UnProjectByPrjStringBatch")]
        public async Task<BaseMessage<List<Vector2d>>> UnProjectByPrjStringBatch([FromBody] ProjectParam3 param)
        {
            try
            {
                var projection = ProjectionInfo.FromEsriString(param.Proj);
                if(projection == null)
                {
                    return Fail<List<Vector2d>>( null,"原始点投影信息输入有误");
                }
                ProjectionInfo destProjection = null;
                if (param.TargetProj.IsNullOrEmpty())
                {
                    destProjection = KnownCoordinateSystems.Geographic.World.WGS1984;
                }
                else
                {
                    destProjection = ProjectionInfo.FromEsriString(param.TargetProj);
                }
                if (destProjection == null)
                {
                    return Fail<List<Vector2d>>(null, "原始点投影信息输入有误");
                }
                var radius = param.Points.SelectMany(p => new double[] { p.X, p.Y }).ToArray();
                Reproject.ReprojectPoints(radius, null, projection, KnownCoordinateSystems.Geographic.World.WGS1984, 0, param.Points.Count());
                List<Vector2d> list = new List<Vector2d>();
                for (int i = 0; i < radius.Length / 2; i++)
                {
                    list.Add(new Vector2d() { X = radius[2 * i], Y = radius[2 * i + 1] });
                }
                return Success(list, "转换成功");
            }
            catch (Exception ex)
            {
                Toolset.TinyLogger.WriteLog(ex.ToFriendlyString());
                return Fail<List<Vector2d>>(null, "失败", ex);
            }
        }

        /// <summary>
        /// 转换Shanghaicity为经纬度
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("UnProjectShanghaiCity")]
        public async Task<BaseMessage<Vector2d>> UnProjectShanghaiCity([FromBody] Vector2d param)
        {
            try
            {
                var projection = ProjectionInfo.FromEsriString("PROJCS[\"SHAG2000\",GEOGCS[\"GCS_China_Geodetic_Coordinate_System_2000\",DATUM[\"D_China_2000\",SPHEROID[\"CGCS2000\",6378137.0,298.257222101]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",0.0],PARAMETER[\"False_Northing\",-3457147.81],PARAMETER[\"Central_Meridian\",121.4644444],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]");
                var radius = new double[] { param.X, param.Y };
                Reproject.ReprojectPoints(radius, null, projection, KnownCoordinateSystems.Geographic.World.WGS1984, 0, 1);
                var wgs84 = new Vector2d(radius[0] * 180 / Math.PI, radius[1] * 180 / Math.PI);
                return Success(wgs84, "转换成功");
            }
            catch (Exception ex)
            {
                Toolset.TinyLogger.WriteLog(ex.ToFriendlyString());
                return Fail<Vector2d>(null, "失败", ex);
            }
        }

        /// <summary>
        /// 批量转换Shanghaicity为经纬度
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("UnProjectShanghaiCityBatch")]
        public async Task<BaseMessage<List<Vector2d>>> UnProjectShanghaiCityBatch([FromBody] IEnumerable<Vector2d> param)
        {
            try
            {
                var projection = ProjectionInfo.FromEsriString("PROJCS[\"SHAG2000\",GEOGCS[\"GCS_China_Geodetic_Coordinate_System_2000\",DATUM[\"D_China_2000\",SPHEROID[\"CGCS2000\",6378137.0,298.257222101]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",0.0],PARAMETER[\"False_Northing\",-3457147.81],PARAMETER[\"Central_Meridian\",121.4644444],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]");
                var radius = param.SelectMany(p=>new double[] {p.X,p.Y }).ToArray();
                Reproject.ReprojectPoints(radius, null, projection, KnownCoordinateSystems.Geographic.World.WGS1984, 0, param.Count());
                List<Vector2d> list = new List<Vector2d>();
                for(int i=0;i< radius.Length/2;i++)
                {
                    list.Add(new Vector2d() { X = radius[2 * i], Y = radius[2 * i + 1] });
                }
                return Success(list, "转换成功");
            }
            catch (Exception ex)
            {
                Toolset.TinyLogger.WriteLog(ex.ToFriendlyString());
                return Fail<List<Vector2d>>(null, "失败", ex);
            }
        }

        ProjectionInfo GetProjectionInfo(string projinfo)
        {
            try
            {
                ProjectionInfo projection = null;
                if (projinfo.StartsWithIgnoreCase("PROJCS"))
                {
                    projection = ProjectionInfo.FromEsriString(projinfo);
                }
                else if (projinfo.All(char.IsDigit))
                {
                    projection = ProjectionInfo.FromEpsgCode(int.Parse(projinfo));
                }
                else if (Regex.IsMatch(projinfo, @"[\S]+:[\d]+"))
                {
                    var arr = projinfo.Split(':');
                    projection = ProjectionInfo.FromAuthorityCode(arr[0], int.Parse(arr[1]));
                }
                else
                {
                    projection = ProjectionInfo.FromProj4String(projinfo);
                }
                return projection;
            }
            catch
            {
                return null;
            }
        }
    }
}
