using AgDataHandle.BaseService.GltfSdk.BuildingContext;
using AgDataHandle.BaseService.GltfSdk.Def;
using AgDataHandle.BaseService.Utils;

namespace AgDataHandle.BaseService.GltfSdk.Generator
{
    public class GLTFSimpleFactory : GLTFFactory
    {
        public GLTFSimpleFactory(string dirName) : base(dirName)
        {
            PrimitiveContexts = new List<GLTFPrimitiveBuildingContext>();
        }

        public List<GLTFPrimitiveBuildingContext> PrimitiveContexts { get; set; }

        public override GLTF Create(string gltfJSONFileName)
        {
            model.FullName = PathLunix.Combine(model.DirectoryName, gltfJSONFileName);
            model.Nodes.Add(new GLTFNode { GLTF = model, MeshIndex = 0 });
            model.Scenes[0].Nodes.Add(0);
            model.Scene = 0;

            if (PrimitiveContexts.Count == 0)
                return null;
            foreach (var priCtx in PrimitiveContexts)
            {
                if (!priCtx.CheckData())
                    throw new Exception("[GLTFSimpleBuilder Create() Error] 数据不合规");

                new GLTFPrimitiveBuilder(priCtx, model).Build();
            }
            bf0.ByteLength = (int)dataStream.Length;

            ShakeEmptyGLTFElement();
            return model;
        }
    }
}
