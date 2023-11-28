using AgDataHandle.BaseService.GltfSdk.BuildingContext;
using AgDataHandle.BaseService.GltfSdk.Def;
using AgDataHandle.BaseService.Utils;
using System.ComponentModel;

namespace AgDataHandle.BaseService.GltfSdk.Generator
{
    [Description("不推荐使用多Mesh模型")]
    public class GLTFMultiMeshFactory : GLTFFactory
    {
        public GLTFMultiMeshFactory(string dirName) : base(dirName)
        {
            MeshContexts = new List<GLTFMeshBuildingContext>();
        }

        public List<GLTFMeshBuildingContext> MeshContexts { get; set; }

        public override GLTF Create(string gltfJSONFileName)
        {
            model.FullName = PathLunix.Combine(model.DirectoryName, gltfJSONFileName);
            model.Nodes.Add(new GLTFNode { GLTF = model, MeshIndex = 0 });
            model.Scenes[0].Nodes.Add(0);
            model.Scene = 0;

            foreach (var meshCtx in MeshContexts)
            {
                new GLTFMeshBuilder(meshCtx, model).Build();
            }

            bf0.ByteLength = (int)dataStream.Length;

            ShakeEmptyGLTFElement();
            return null;
        }
    }
}
