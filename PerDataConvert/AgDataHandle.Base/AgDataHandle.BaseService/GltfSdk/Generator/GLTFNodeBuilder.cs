using AgDataHandle.BaseService.GltfSdk.BuildingContext;
using AgDataHandle.BaseService.GltfSdk.Def;

namespace AgDataHandle.BaseService.GltfSdk.Generator
{
    internal class GLTFNodeBuilder : GLTFBuilder
    {
        public GLTFNodeBuilder(GLTFNodeBuildingContext nodeCtx, GLTF model)
        {
            this.model = model;
            this.nodeCtx = nodeCtx;
        }

        private readonly GLTF model;
        private readonly GLTFNodeBuildingContext nodeCtx;

        public GLTFNode Build()
        {
            GLTFNode node = new GLTFNode { GLTF = model };
            node.Matrix = nodeCtx.TransformMatrix;
            node.Name = string.IsNullOrEmpty(nodeCtx.Name) ? null : nodeCtx.Name;
            node.Extensions = nodeCtx.Extensions.Count == 0 ? null : nodeCtx.Extensions;
            return node;
        }
    }
}
