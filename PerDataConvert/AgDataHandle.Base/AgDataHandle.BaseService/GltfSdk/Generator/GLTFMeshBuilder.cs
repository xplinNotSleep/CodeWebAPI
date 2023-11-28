using AgDataHandle.BaseService.GltfSdk.BuildingContext;
using AgDataHandle.BaseService.GltfSdk.Def;

namespace AgDataHandle.BaseService.GltfSdk.Generator
{
    internal class GLTFMeshBuilder : GLTFBuilder
    {
        public GLTFMeshBuilder(GLTFMeshBuildingContext meshCtx, GLTF model)
        {
            this.model = model;
            this.meshCtx = meshCtx;
        }

        private readonly GLTF model;
        private readonly GLTFMeshBuildingContext meshCtx;

        public GLTFMesh Build()
        {
            if (!meshCtx.CheckData())
                return null;

            GLTFMesh mesh = new GLTFMesh { GLTF = model };
            mesh.Name = meshCtx.Name;
            mesh.Extensions = meshCtx.Extensions.Count == 0 ? null : meshCtx.Extensions;
            mesh.Extras = meshCtx.Extras;

            GLTFPrimitiveBuilder priBuilder = null;
            foreach (var priCtx in meshCtx.PrimitiveContexts)
            {
                if (meshCtx.PrimitiveContexts.IndexOf(priCtx) == 0)
                {
                    priBuilder = new GLTFPrimitiveBuilder(priCtx, model);
                    priBuilder.Build();
                }
                else
                {
                    priBuilder.prmtCtx = priCtx;
                    priBuilder.Build();
                }
            }

            var nd = new GLTFNode
            {
                GLTF = model,
                MeshIndex = model.Meshes.IndexOf(mesh)
            };
            model.Meshes.Add(mesh);
            model.Nodes.Add(nd);
            model.Scenes[0].Nodes.Add(model.Nodes.IndexOf(nd));
            return mesh;
        }
    }
}
