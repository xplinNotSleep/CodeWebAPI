using AgDataHandle.Maths;
using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using AgDataHandle.BaseService.B3dmModel;

namespace AgDataHandle.BaseService.I3dmModel
{
    public class I3dm : BI3dmBase
    {
        public I3dmFeatureTable FeatureTable { get; set; }
        /// <summary>
        /// 传入相似矩阵
        /// </summary>
        List<IMatrix4x4> Matrixs { get; set; }
        /// <summary>
        /// 属性信息
        /// </summary>
        Dictionary<string, List<object>> BatchTable { get; set; }
        /// <summary>
        /// 如果不为nulll
        /// </summary>
        //public List<ushort> BatchIds { get; set; }
        #region 构造函数

        public I3dm() : base("i3dm") { }
        public static I3dm LoadFormFile(string path)
        {
            if (!File.Exists(path))
                throw new Exception("文件不存在：" + path);

            using (var fs = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(fs))
                {
                    var header = BI3dmHelper.ReadHead(reader);
                    var i3dm = new I3dm(reader, header);
                    i3dm.GetFeaturTableInfo();
                    return i3dm;
                }
            }
        }
        public void GetFeaturTableInfo()
        {
            FeatureTable = new I3dmFeatureTable();
            FeatureTable.FromBinaryAndJson(FeatureTableJson, FeatureTableBinary);
        }
        public void GetMatrixInfo()
        {
            if (FeatureTable == null)
                GetFeaturTableInfo();
            Matrixs = new List<IMatrix4x4>();
            for (int i = 0; i < FeatureTable.Positions.Count; i++)
            {
                //缩放矩阵
                var scaleMat = Matrix4x4.Identity;
                scaleMat.ScaleByVector<Matrix4x4>(FeatureTable.ScaleNonUniforms[i]);
                //旋转矩阵
                var row0 = FeatureTable.NormalRights[i];
                var row1 = FeatureTable.NormalUps[i];
                var rotateMat = new Matrix4x4
                (
                    row0.X, row0.Y, row0.Z, 0.0f,
                    row1.X, row1.Y, row1.Z, 0.0f,
                    0.0f, 0.0f, 1.0f, 0.0f,
                    0.0f, 0.0f, 0.0f, 1.0f
                );
                //平移矩阵
                var translationMat = Matrix4x4.Identity;
                translationMat.TranslationByVector<Matrix4x4>(FeatureTable.Positions[i]);

                var result = scaleMat.MultiplyToOther(rotateMat).MultiplyToOther(translationMat);
                Matrixs.Add(result);
            }
        }
        public I3dm(GLB gLB, List<IMatrix4x4> matrixs, Dictionary<string, List<object>> BatchTable = null) : base("i3dm")
        {
            glbData = gLB.GetAllBytes();
            Matrixs = matrixs;
            head.gltfFormat = 1;
            this.BatchTable = BatchTable;
            UpdateFeatureTable();
        }

        public I3dm(GLTF gLTF, List<IMatrix4x4> matrixs, Dictionary<string, List<object>> BatchTable = null) : base("i3dm")
        {
            GLB gLB = GLB.LoadFromGLTF(gLTF);
            glbData = gLB.GetAllBytes();
            Matrixs = matrixs;
            head.gltfFormat = 1;
            this.BatchTable = BatchTable;
            UpdateFeatureTable();
        }

        public I3dm(byte[] glbbyte, List<IMatrix4x4> matrixs, Dictionary<string, List<object>> BatchTable = null) : base("i3dm")
        {
            glbData = glbbyte;
            Matrixs = matrixs;
            head.gltfFormat = 1;
            this.BatchTable = BatchTable;
            UpdateFeatureTable();
        }

        public I3dm(BinaryReader reader, BI3dmHeader b3dmHeader) : base(reader, b3dmHeader)
        {
        }

        public void UpdateFeatureTable()
        {
            FeatureTable = new I3dmFeatureTable();
            FeatureTable.Positions = new List<Vector3>();
            FeatureTable.NormalRights = new List<Vector3>();
            FeatureTable.NormalUps = new List<Vector3>();
            FeatureTable.Scales = new List<float>();
            //变换矩阵写入
            Matrixs.ForEach(matrix =>
            {
                //FeatureTable.Positions.Add(MatrixHelper.GetPosition(matrix));
                //FeatureTable.NormalRights.Add(MatrixHelper.GetNormalRight(matrix));
                //FeatureTable.NormalUps.Add(MatrixHelper.GetNormalUp(matrix));
                FeatureTable.Scales.Add(1.0f);
                //FeatureTable.ScaleNonUniforms.Add(I3dmHelper.GetScale2(matrix));
            });
            var batchids = Enumerable.Range(0, Matrixs.Count).ToList();
            FeatureTable.BatchIds = new List<ushort>();
            foreach (var batchid in batchids)
            {
                FeatureTable.BatchIds.Add(batchid.ToUShort());
            }

            FeatureTableJson = FeatureTable.GetFeatureTableJson();
            FeatureTableBinary = FeatureTable.GetFeatureTableBinary();

            if (BatchTable != null && BatchTable.Count > 0)
            {
                BatchTableJson = JsonConvert.SerializeObject(BatchTable, new KeyValuePairConverter());
            }
        }
        #endregion

        #region 导出
        public override void Export(string path)
        {
            var bytes = GetBytes();
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllBytes(path, bytes);
        }
        #endregion
    }
}
