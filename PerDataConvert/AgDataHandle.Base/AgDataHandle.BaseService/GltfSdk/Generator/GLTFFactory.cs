using AgDataHandle.BaseService.GltfSdk.Collection;
using AgDataHandle.BaseService.GltfSdk.Def;

namespace AgDataHandle.BaseService.GltfSdk.Generator
{
    public abstract class GLTFFactory
    {
        public GLTFFactory(string dirName)
        {
            dataStream = new MemoryStream();
            model = new GLTF
            {
                DirectoryName = dirName,
                Materials = new GLTFMaterialCollection(),
                Samplers = new GLTFSamplerCollection(),
                Textures = new GLTFTextureCollection(),
                Images = new GLTFImageCollection()
            };
            bf0 = new GLTFBuffer(GLTFBufferType.Memory)
            {
                DataStream = dataStream,
                GLTF = model,
                IsReadOnly = false
            };
            model.Buffers.Add(bf0);
            model.Scene = 0;
            model.Scenes.Add(new GLTFScene { GLTF = model, Nodes = new List<int> { } });
            model.Meshes.Add(new GLTFMesh { GLTF = model });
        }

        protected MemoryStream dataStream;
        protected GLTF model;
        protected GLTFBuffer bf0;

        public abstract GLTF Create(string gltfJSONFileName);

        public virtual void ShakeEmptyGLTFElement()
        {
            if (model.Samplers != null && model.Samplers.Count == 0)
            {
                if (model.Textures != null && model.Textures.Count != 0)
                    foreach (var tex in model.Textures)
                        tex.SamplerIndex = -1;
                model.Samplers = null;
            }
            if (model.Images != null && model.Images.Count == 0)
                model.Images = null;
            if (model.Textures != null && model.Textures.Count == 0)
            {
                model.Samplers = null;
                model.Images = null;
                model.Textures = null;
            }
            if (model.Materials != null && model.Materials.Count == 0)
            {
                model.Samplers = null;
                model.Textures = null;
                model.Images = null;
                model.Materials = null;
            }
        }
    }
}
