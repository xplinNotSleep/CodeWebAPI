using AgDataHandle.Maths.Numerics;
using AgDataHandle.BaseService.GeometryHelper.Collection;
using AgDataHandle.BaseService.GeometryHelper.Param;
using AgDataHandle.BaseService.GltfSdk.Def;
using Newtonsoft.Json.Linq;

namespace AgDataHandle.BaseService.GeometryHelper.Def
{
    public class GeometryMesh
    {
        public GeometryMesh()
        {
            Primitives = new GeometryPrimitiveCollection();
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 图元集合
        /// </summary>
        public GeometryPrimitiveCollection Primitives { get; set; }
        /// <summary>
        /// 属性
        /// </summary>
        public Dictionary<string, JObject> Extensions { get; set; }
        /// <summary>
        /// 额外
        /// </summary>
        public JObject Extras { get; set; }
        /// <summary>
        /// 包围盒
        /// </summary>
        public BoundingBox BoundingBox
        {
            get
            {
                return Primitives?.BoundingBox;
            }
        }

        public GLTFMesh AddMeshToGltf(GLTF gLTF, GeometryConvertParam convertParam = null)
        {
            if (Primitives.Count == 0) return null;
            GLTFMesh gLTFMesh = new GLTFMesh();
            gLTFMesh.Name = Name;
            if (convertParam?.IsUseExtension ?? true)
            {
                gLTFMesh.Extensions = Extensions;
                gLTFMesh.Extras = Extras;
            }
            gLTF.Meshes.Add(gLTFMesh);
            Primitives.ForEach(x =>
            {
                gLTFMesh.Primitives.Add(x.AddPrimitiveToGltf(gLTF, convertParam));
            });
            return gLTFMesh;
        }

        /// <summary>
        /// 转成gltf
        /// </summary>
        /// <returns></returns>
        public GLTF ToGltf(GeometryConvertParam convertParam = null)
        {
            var gltf = GLTF.ConvertByAction(self =>
            {
                GLTFNode node = new GLTFNode();
                node.MeshIndex = 0;
                self.Nodes.Add(node);
                self.Scenes[0].Nodes.Add(0);
                AddMeshToGltf(self, convertParam);
            });
            return gltf;
        }
    }
}
