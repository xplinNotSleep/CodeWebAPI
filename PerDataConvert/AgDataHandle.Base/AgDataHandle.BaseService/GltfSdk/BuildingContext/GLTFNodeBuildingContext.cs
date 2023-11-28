using AgDataHandle.BaseService.GltfSdk.Def;
using AgDataHandle.Maths;
using Newtonsoft.Json.Linq;

namespace AgDataHandle.BaseService.GltfSdk.BuildingContext
{
    public sealed class GLTFNodeBuildingContext : GLTFBuildingContext
    {
        public GLTFNodeBuildingContext()
        {
            Extensions = new Dictionary<string, JObject>();
            Extras = new JObject();
        }

        public string Name { get; set; }

        public Matrix4x4 TransformMatrix { get; set; }

        public Dictionary<string, JObject> Extensions { get; set; }
        public JObject Extras { get; set; }

        #region Methods

        public static GLTFNodeBuildingContext LoadFromObject(GLTFNode node)
        {
            GLTFNodeBuildingContext ctx = new GLTFNodeBuildingContext
            {
                Name = node.Name.Clone() as string,
                TransformMatrix = node.Matrix == null ? null : new Matrix4x4(node.Matrix as Matrix4x4)
            };

            return ctx;
        }

        #endregion
    }
}
