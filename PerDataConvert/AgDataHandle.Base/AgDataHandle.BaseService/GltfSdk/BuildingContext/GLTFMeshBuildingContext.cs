using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace AgDataHandle.BaseService.GltfSdk.BuildingContext
{
    public class GLTFMeshBuildingContext : GLTFBuildingContext
    {
        public GLTFMeshBuildingContext(GLTFPrimitiveBuildingContext priCtx)
        {
            if (priCtx == null)
                throw new Exception("[GLTFMeshBuildingContext Constructor Error] 必须传入图元上下文对象");

            PrimitiveContexts = new List<GLTFPrimitiveBuildingContext>();
            Extensions = new Dictionary<string, JObject>();
            Extras = new JObject();
            PrimitiveContexts.Add(priCtx);
        }

        #region Properties

        public List<GLTFPrimitiveBuildingContext> PrimitiveContexts { get; set; }

        [Description("可选|Mesh的名称")]
        public string Name { get; set; }

        public Dictionary<string, JObject> Extensions { get; set; }

        public JObject Extras { get; set; }

        #endregion

        #region Methods

        public bool CheckData()
        {
            bool status = true;
            foreach (var priCtx in PrimitiveContexts)
            {
                if (priCtx.CheckData())
                {
                    status = false;
                    break;
                }
            }
            return status;
        }

        #endregion
    }
}
