using AgDataHandle.BaseService.GltfSdk.BuildingContext;
using AgDataHandle.BaseService.GltfSdk.Def;

namespace AgDataHandle.BaseService.GltfSdk.Generator
{
    public class GLTFMultiNodeFactory : GLTFSimpleFactory
    {
        public GLTFMultiNodeFactory(string dirName) : base(dirName)
        {
            NodeContexts = new List<GLTFNodeBuildingContext>();
        }

        public List<GLTFNodeBuildingContext> NodeContexts { get; set; }

        public override GLTF Create(string gltfJSONFileName)
        {
            model = base.Create(gltfJSONFileName);

            for (int i = 0; i < NodeContexts.Count; i++)
            {
                var ndCtx = NodeContexts[i];
                var nd = new GLTFNodeBuilder(ndCtx, model).Build();
                nd.MeshIndex = 0;

                if (i == 0)
                    model.Nodes[0] = nd;
                else
                {
                    model.Nodes.Add(nd);
                    model.Scenes[0].Nodes.Add(model.Nodes.IndexOf(nd));
                }
            }

            return model;
        }
    }
}
