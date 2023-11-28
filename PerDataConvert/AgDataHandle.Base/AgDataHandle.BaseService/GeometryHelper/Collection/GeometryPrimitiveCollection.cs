using AgDataHandle.Maths.Numerics;
using AgDataHandle.BaseService.GeometryHelper.Def;
using AgDataHandle.BaseService.GeometryHelper.Param;
using AgDataHandle.BaseService.GltfSdk.Collection;
using AgDataHandle.BaseService.GltfSdk.Def;

namespace AgDataHandle.BaseService.GeometryHelper.Collection
{
    public class GeometryPrimitiveCollection : GLTFCollection<GeometryPrimitive>
    {
        BoundingBox _BoundingBox;
        /// <summary>
        /// 包围盒
        /// </summary>
        public BoundingBox BoundingBox
        {
            get
            {
                if (_BoundingBox == null)
                {
                    _BoundingBox = new BoundingBox();
                    this.ForEach(x =>
                    {
                        _BoundingBox.Update(x.BoundingBox);
                    });
                }
                return _BoundingBox;
            }
        }

        /// <summary>
        /// 转成gltf
        /// </summary>
        /// <returns></returns>
        public GLTF ToGltf(GeometryConvertParam convertParam = null)
        {
            var gltf = GLTF.ConvertByAction(self =>
            {
                self.Scenes[0].Nodes.Add(0);
                GLTFNode node = new GLTFNode();
                node.MeshIndex = 0;
                self.Nodes.Add(node);
                GLTFMesh gLTFMesh = new GLTFMesh();
                self.Meshes.Add(gLTFMesh);
                this.ForEach(p => gLTFMesh.Primitives.Add(p.AddPrimitiveToGltf(self, convertParam)));
            });
            return gltf;
        }
    }
}
