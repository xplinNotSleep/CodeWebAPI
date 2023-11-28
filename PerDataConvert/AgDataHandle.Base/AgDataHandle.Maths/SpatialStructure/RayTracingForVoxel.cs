using AgDataHandle.Maths.Geometry;
using AgDataHandle.Maths.Numerics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgDataHandle.Maths.SpatialStructure
{
    /// <summary>
    /// 基于体素的光线追踪
    /// </summary>
    public class RayTracingForVoxel
    {
        private int m_xSize;
        private int m_ySize;
        private int m_zSize;
        private Vector3[,,] m_dataSource;
        private VoxelLuminanceInput[,,] m_voxelLuminanceInput;
        private int m_maxRefNum = 0;
        private int m_currentRefNum = 0;
        private float m_currentIl;
        
        /// <summary>
        /// 被光线击中的体素坐标
        /// </summary>
        private List<Vector3> m_result;

        /// <summary>
        /// 最大步进次数
        /// </summary>
        public int MaxStep { get; set; } = 1000;
        /// <summary>
        /// 光线路径
        /// </summary>
        public List<Vector3> LightPath { get; set; } = new List<Vector3>();
        /// <summary>
        /// 体素光照输出
        /// </summary>
        public VoxelLuminanceOutput[,,] LuminanceOutput { get; set; }
        /// <summary>
        /// 原点坐标
        /// </summary>
        public Vector3 Origin { get; set; }
        /// <summary>
        /// 体素大小
        /// </summary>
        public float Unit { get; set; }
        /// <summary>
        /// 是否保存光线路径
        /// </summary>
        public bool SavePath { get; set; } = false;
        /// <summary>
        /// 最大反射次数
        /// </summary>
        public int MaxRefNum { get; set; }
        public RayTracingForVoxel(Vector3[,,] vs)
        {
            m_dataSource = vs;
            m_xSize = m_dataSource.GetLength(0);
            m_ySize = m_dataSource.GetLength(1);
            m_zSize = m_dataSource.GetLength(2);
            m_result = new List<Vector3>();
        }
        public RayTracingForVoxel(VoxelLuminanceInput[,,] data)
        {
            m_voxelLuminanceInput = data;
            m_xSize = m_voxelLuminanceInput.GetLength(0);
            m_ySize = m_voxelLuminanceInput.GetLength(1);
            m_zSize = m_voxelLuminanceInput.GetLength(2);
            m_result = new List<Vector3>();
            m_currentRefNum = 0;
            m_currentIl = 1;
        }

        /// <summary>
        /// 执行光线追踪
        /// </summary>
        /// <param name="start">光源位置</param>
        /// <param name="dir">方向</param>
        /// <param name="reflectionNum">反射次数</param>
        /// <returns></returns>
        public List<Vector3> Tracing(Vector3 start, Vector3 dir, int reflectionNum)
        {
            m_currentRefNum = 0;
            if (reflectionNum == 0)
                return m_result;
            float step = 1f;
            Vector3 reDir = null;
            for (int i = 1; i < MaxStep; i++)
            {
                var p = start + dir * step * i;
                var x = (int)Math.Round(p.X);
                var y = (int)Math.Round(p.Y);
                var z = (int)Math.Round(p.Z);
                if(SavePath)
                {
                    LightPath.Add(new Vector3(x, y, z));
                }
                
                var refP = Reflect(dir, x, y, z);
                if (refP == null)
                    continue;
                m_currentRefNum++;
                m_result.Add(new Vector3(x, y, z));
                reDir = refP;
                break;
            }
            if (m_result.Count == 0 || reDir == null)
                return m_result;
            Tracing(m_result[m_result.Count - 1], reDir, reflectionNum - 1);
            return m_result;
        }
        
        /// <summary>
        /// 执行光线追踪
        /// </summary>
        /// <param name="lightPosition">光源位置</param>
        /// <param name="dir">方向</param>
        /// <param name="reflectionNum">反射次数</param>
        /// <returns></returns>
        public List<Vector3> TracingForVoxel(Vector3 lightPosition, Vector3 dir, int reflectionNum)
        {
            if (reflectionNum == 0)
                return m_result;
            float step = Unit/2;
            
            for (int i = 1; i < MaxStep; i++)
            {
                var p = lightPosition + dir * step * i;
                var vp = WorldPToVP(p);
                var x = vp.X;
                var y = vp.Y;
                var z = vp.Z;
                if(SavePath)
                    LightPath.Add(p);
                if (x < 0 || x >= m_xSize || y < 0 || y >= m_ySize || z < 0 || z >= m_zSize)
                    continue;
                if (m_voxelLuminanceInput[x, y, z] == null)
                    continue;
                
                var refP = ComputeReflectForVoxel(dir, x, y, z, lightPosition, ref p);
                if (refP == null)
                    continue;
                m_currentRefNum++;
                m_result.Add(new Vector3(x, y, z));
                TracingForVoxel(p, refP, reflectionNum - 1);
                break;
            }
            return m_result;
        }
        
        /// <summary>
        /// 世界坐标转体素坐标
        /// </summary>
        /// <param name="wp"></param>
        /// <returns></returns>
        private Vector3Int WorldPToVP(Vector3 wp)
        {
            var x = (int)((wp.X - Origin.X) / Unit);
            var y = (int)((wp.Y - Origin.Y) / Unit);
            var z = (int)((wp.Z - Origin.Z) / Unit);
            return new Vector3Int(x, y, z);
        }
        private Vector3 Reflect(Vector3 dir,int x,int y,int z)
        {
            if (x < 0 || x >= m_xSize || y < 0 || y >= m_ySize || z < 0 || z >= m_zSize)
                return null;
            if (m_dataSource[x, y, z] == null)
                return null;
            return GetReflectedDir(dir,m_dataSource[x,y,z]);
        }
      
        /// <summary>
        /// 计算反射信息
        /// </summary>
        /// <param name="dir">方向</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="startP">光源位置</param>
        /// <param name="crosspoint">交点</param>
        /// <returns></returns>
        private Vector3 ComputeReflectForVoxel(Vector3 dir, int x, int y, int z,Vector3 startP,ref Vector3 crosspoint)
        {
            if (x < 0 || x >= m_xSize || y < 0 || y >= m_ySize || z < 0 || z >= m_zSize)
                return null;
            if (m_voxelLuminanceInput[x, y, z] == null)
                return null;
            if (LuminanceOutput[x, y, z] == null)
                LuminanceOutput[x, y, z] = new VoxelLuminanceOutput();
           
            var result = LuminanceOutput[x, y, z].ComputeCrosspoint(m_voxelLuminanceInput[x, y, z], startP, dir,ref m_currentIl);
            if (result == null)
                return -1 * dir;
            
            crosspoint = result.Item1;
            return result.Item2;
        }
        private  Vector3 GetReflectedDir(Vector3 v1, Vector3 n)
        {
            return v1 - 2 * v1.Dot(n) * n;
        }

    }

    /// <summary>
    /// 体素光照输入信息
    /// </summary>
    public class VoxelLuminanceInput
    {
        public Vector3 Position { get; set; }
        public float Unit;
        public BoundingBox Box { get; set; }
        public List<Vector3> DirList { get; set; }
        
        /// <summary>
        /// 是否可以反射
        /// </summary>
        public bool LeftCanReflect { get; set; } = false;
        public bool RightCanReflect { get; set; } = false;
        public bool FrontCanReflect { get; set; } = false;
        public bool BackCanReflect { get; set; } = false;
        public bool TopCanReflect { get; set; } = false;
        
        /// <summary>
        /// 反射平面
        /// </summary>
        public Plane3 TopPlane { get; set; }
        public Plane3 LeftPlane { get; set; }
        public Plane3 RightPlane { get; set; }
        public Plane3 FrontPlane { get; set; }
        public Plane3 BackPlane { get; set; }

        /// <summary>
        /// 是否镜面反射
        /// </summary>
        public bool SpecularReflection { get; set; } = false;

        public VoxelLuminanceInput(Vector3 position,float unit)
        {
            Position = position;
            Unit = unit;
            DirList = new List<Vector3>();
        }
        
        /// <summary>
        /// 初始化反射平面
        /// </summary>
        public void Init()
        {
            if(TopCanReflect)
            {
                TopPlane = new Plane3(new Vector3(0,0,1),new Vector3(Position.X,Position.Y,Box.MaxZ));
            }
            if (LeftCanReflect)
            {
                LeftPlane = new Plane3(new Vector3(-1, 0, 0), new Vector3(Box.MinX, Position.Y, Position.Z));
            }
            if (RightCanReflect)
            {
                RightPlane = new Plane3(new Vector3(1, 0, 0), new Vector3(Box.MaxX, Position.Y, Position.Z));
            }
            if (FrontCanReflect)
            {
                FrontPlane = new Plane3(new Vector3(0, -1, 0), new Vector3(Position.X, Box.MinY, Position.Z));
            }
            if (BackCanReflect)
            {
                BackPlane = new Plane3(new Vector3(0, 1, 0), new Vector3(Position.X, Box.MaxY, Position.Z));
            }
        }

        /// <summary>
        /// 判断点是否在平面上
        /// </summary>
        /// <param name="crosspoint"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public bool InTopPlane(Vector3 crosspoint,float buffer=0)
        {
            return crosspoint.X >= Box.MinX-buffer && crosspoint.X <= Box.MaxX+buffer
                && crosspoint.Y >= Box.MinY-buffer && crosspoint.Y >= Box.MaxY+buffer
                && crosspoint.Z > Position.Z;
        }
        public bool InLeftPlane(Vector3 crosspoint,float buffer=0)
        {
            return crosspoint.Z >= Box.MinZ-buffer && crosspoint.Z <= Box.MaxZ+buffer
                && crosspoint.Y >= Box.MinY-buffer && crosspoint.Y <= Box.MaxY+buffer
                && crosspoint.X < Position.X;
        }
        public bool InRightPlane(Vector3 crosspoint, float buffer = 0)
        {
            return crosspoint.Z >= Box.MinZ-buffer && crosspoint.Z <= Box.MaxZ+buffer
                && crosspoint.Y >= Box.MinY-buffer && crosspoint.Y <= Box.MaxY+buffer
                && crosspoint.X > Position.X;
        }
        public bool InFrontPlane(Vector3 crosspoint, float buffer = 0)
        {
            return crosspoint.Z >= Box.MinZ-buffer && crosspoint.Z <= Box.MaxZ+buffer
                && crosspoint.X >= Box.MinX-buffer && crosspoint.X <= Box.MaxX+buffer
                && crosspoint.Y < Position.Y;
        }
        public bool InBackPlane(Vector3 crosspoint, float buffer = 0)
        {
            return crosspoint.Z >= Box.MinZ-buffer && crosspoint.Z <= Box.MaxZ+buffer
                && crosspoint.X >= Box.MinX -buffer&& crosspoint.X <= Box.MaxX+buffer
                && crosspoint.Y > Position.Y;
        }

        /// <summary>
        /// 获取顶部反射方向
        /// </summary>
        /// <returns></returns>
        public Vector3 GetTopReflectDir()
        {
            if (SpecularReflection)
                return new Vector3(0,0,1);
            return RayTracingSampling.GetSamplingDirForDiffuseReflection();
        }
        /// <summary>
        /// 获取左侧反射方向
        /// </summary>
        /// <returns></returns>
        public Vector3 GetLeftReflectDir()
        {
            if (SpecularReflection)
                return new Vector3(-1, 0,0);
            var dir = RayTracingSampling.GetSamplingDirForDiffuseReflection();
            return MathAlgorithm.RoateY(dir,90);
        }
        /// <summary>
        /// 获取右侧反射方向
        /// </summary>
        /// <returns></returns>
        public Vector3 GetRightReflectDir()
        {
            if (SpecularReflection)
                return new Vector3(1, 0, 0);
            var dir = RayTracingSampling.GetSamplingDirForDiffuseReflection();
            return MathAlgorithm.RoateY(dir, -90);
        }
        /// <summary>
        /// 获取前面反射方向
        /// </summary>
        /// <returns></returns>
        public Vector3 GetFrontReflectDir()
        {
            if (SpecularReflection)
                return new Vector3(0, -1, 0);
            var dir = RayTracingSampling.GetSamplingDirForDiffuseReflection();
            return MathAlgorithm.RoateX(dir, -90);
        }
        /// <summary>
        /// 获取后面反射方向
        /// </summary>
        /// <returns></returns>
        public Vector3 GetBackReflectDir()
        {
            if (SpecularReflection)
                return new Vector3(0, 1, 0);
            var dir = RayTracingSampling.GetSamplingDirForDiffuseReflection();
            return MathAlgorithm.RoateX(dir, 90);
        }
    }
    
    /// <summary>
    /// 存储体素的光照输出信息
    /// </summary>
    public class VoxelLuminanceOutput
    {
        /// <summary>
        /// 是否接收到光照
        /// </summary>
        public bool LeftReceiveLight { get; set; } = false;
        public bool RightReceiveLight { get; set; }=false;
        public bool FrontReceiveLight { get; set; } = false;
        public bool BackReceiveLight { get; set; } = false;
        public bool TopReceiveLight { get; set; } = false;

        /// <summary>
        /// 存储亮度信息
        /// </summary>
        public float TopLuminance { get; set; }
        public float LeftLuminance { get;  set; }
        public float RightLuminance { get;  set; }
        public float FrontLuminance { get;  set; }
        public float BackLuminance { get;  set; }

        public VoxelLuminanceOutput()
        {
            TopLuminance = 0;
            LeftLuminance = 0;
            RightLuminance = 0;
            FrontLuminance = 0;
            BackLuminance = 0;
        }
      

        public Tuple<Vector3,Vector3> ComputeCrosspoint(VoxelLuminanceInput voxelReflectedDir,Vector3 point,Vector3 dir,ref float luminance)
        {
            Vector3 crosspoint = null;
            Vector3 noraml = null;
            var mind = float.MaxValue;
            var refLuminance = 0f;
            if (voxelReflectedDir.TopCanReflect)
            {
                var ccrosspoint = MathAlgorithm.CalculateIntersectPoint(voxelReflectedDir.TopPlane,dir,point);
                if(ccrosspoint!=null)
                {
                    var d = (ccrosspoint - point).LengthSquare();
                    if (d < mind)
                    {
                        var nl = (dir * -1).Dot(new Vector3(0, 0, 1));
                        var canTop = nl >= 0;
                        if ((voxelReflectedDir.InTopPlane(ccrosspoint, 0.03f) || crosspoint == null) && canTop)
                        {
                            mind = d;
                            TopReceiveLight = true;
                            refLuminance = nl * luminance;
                            TopLuminance += refLuminance;
                            crosspoint = ccrosspoint;
                            noraml = voxelReflectedDir.GetTopReflectDir();
                        }
                    }
                }
              
            }
            if(voxelReflectedDir.LeftCanReflect)
            {
                var ccrosspoint = MathAlgorithm.CalculateIntersectPoint(voxelReflectedDir.LeftPlane, dir, point);
                if(ccrosspoint!=null)
                {
                    var d = (ccrosspoint - point).LengthSquare();
                    if (d < mind)
                    {
                        var nl = (dir * -1).Dot(new Vector3(-1, 0, 0));
                        var canLeft = nl >= 0;
                        if ((voxelReflectedDir.InLeftPlane(ccrosspoint, 0.03f) || crosspoint == null) && canLeft)
                        {
                            mind = d;
                            LeftReceiveLight = true;
                            refLuminance = nl * luminance;
                            LeftLuminance += refLuminance;
                            crosspoint = ccrosspoint;
                            noraml = voxelReflectedDir.GetLeftReflectDir();
                        }
                    }
                }
                
            }
            if (voxelReflectedDir.RightCanReflect)
            {
                var ccrosspoint = MathAlgorithm.CalculateIntersectPoint(voxelReflectedDir.RightPlane, dir, point);
                if(ccrosspoint!=null)
                {
                    var d = (ccrosspoint - point).LengthSquare();
                    if (d < mind)
                    {
                        var nl = (dir * -1).Dot(new Vector3(1, 0, 0));
                        var canRight = nl >= 0;
                        if ((voxelReflectedDir.InRightPlane(ccrosspoint, 0.03f) || crosspoint == null) && canRight)
                        {
                            mind = d;
                            RightReceiveLight = true;
                            refLuminance = nl * luminance;
                            RightLuminance+= refLuminance;
                            crosspoint = ccrosspoint;
                            noraml = voxelReflectedDir.GetRightReflectDir();
                        }
                    }
                }
               
                
            }
            if (voxelReflectedDir.FrontCanReflect)
            {
                var ccrosspoint = MathAlgorithm.CalculateIntersectPoint(voxelReflectedDir.FrontPlane, dir, point);
                if(ccrosspoint!=null)
                {
                    var d = (ccrosspoint - point).LengthSquare();
                    if (d < mind)
                    {
                        var nl = (dir * -1).Dot(new Vector3(0, -1, 0));
                        var canFront = nl >= 0;
                        if ((voxelReflectedDir.InFrontPlane(ccrosspoint, 0.03f) || crosspoint == null) && canFront)
                        {
                            mind = d;
                            FrontReceiveLight = true;
                            refLuminance = nl * luminance;
                            FrontLuminance += refLuminance;
                            crosspoint = ccrosspoint;
                            noraml = voxelReflectedDir.GetFrontReflectDir();

                        }
                    }
                }
               
            }
            if (voxelReflectedDir.BackCanReflect)
            {
                var ccrosspoint = MathAlgorithm.CalculateIntersectPoint(voxelReflectedDir.BackPlane, dir, point);
                if(ccrosspoint!=null)
                {
                    var d = (ccrosspoint - point).LengthSquare();
                    if (d < mind)
                    {
                        var nl = (dir * -1).Dot(new Vector3(0, 1, 0));
                        var canBack = nl >= 0;
                        if ((voxelReflectedDir.InBackPlane(ccrosspoint, 0.03f) || crosspoint == null) && canBack)
                        {
                            mind = d;
                            BackReceiveLight = true;
                            refLuminance = nl * luminance;
                            BackLuminance += refLuminance;
                            crosspoint = ccrosspoint;
                            noraml = voxelReflectedDir.GetBackReflectDir();
                        }
                    }
                }
               
            }
            if (crosspoint == null)
                return null;
            luminance = refLuminance;
            return new Tuple<Vector3, Vector3>(crosspoint,noraml);
        }
    }
}
