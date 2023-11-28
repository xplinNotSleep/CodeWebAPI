using AgDataHandle.BaseService.GltfSdk.Append;
using AgDataHandle.BaseService.GltfSdk.Collection;
using AgDataHandle.BaseService.GltfSdk.Def;
using AgDataHandle.BaseService.GltfSdk.ENUM;

namespace AgDataHandle.BaseService.GltfSdk.Result
{
    public class GLTFResultCollection : GLTFCollection<GLTFResult>
    {
        /// <summary>
        /// 把结果集搞成树，方便生成lod
        /// </summary>
        public List<GLTFResultCollection> Children { get; set; }

        /// <summary>
        /// 把父节点转成gltf，暂时不管children
        /// </summary>
        /// <returns></returns>
        public GLTF ToGltf()
        {
            GLTF gLTF = new GLTF();
            if (this.Contains(p => p.HasLineIndex))
            {
                gLTF.ExtensionsUsed = new List<string>() { "CESIUM_primitive_outline" };
            }
            var buffer = new GLTFBuffer();

            gLTF.Buffers.Add(buffer);
            gLTF.Scenes.Add(new GLTFScene());
            gLTF.Scenes[0].Nodes.Add(0);
            GLTFNode firstNode = new GLTFNode();
            firstNode.ChildrenID = new List<int>();
            gLTF.Nodes.Add(firstNode);
            buffer.DataStream = new MemoryStream();
            int nBufferViewIndex = 0;

            var Images = this.Where(p => p.Images != null).SelectMany(p => p.Images).Distinct();

            this.ForEach(p =>
            {
                for (int i = 0; i < p.GeometryResults.Count; i++)
                {
                    GLTFMesh gLTFMesh = new GLTFMesh();
                    gLTF.Meshes.Add(gLTFMesh);

                    //数组是空的则设置为null,不写出到gltf
                    if (gLTFMesh.Extensions.Count == 0)
                        gLTFMesh.Extensions = null;

                    CreateGLTFNode(gLTF, p.GeometryResults[i], firstNode);

                    p.GeometryResults[i].gLTFPrimitives.ForEach(p1 =>
                    {
                        p1.Attributes.ForEach(kv =>
                        {
                            p1.Attributes[kv.Key] = kv.Value + nBufferViewIndex;
                        });
                        p1.Indices += nBufferViewIndex;

                        //UpdatePrimititveMaterialIndexF1(p1, gLTF, p, Images);
                    });

                    p.GeometryResults[i].gLTFAccessors.ForEach(a =>
                    {
                        a.BufferViewIndex += nBufferViewIndex;
                    });

                    p.GeometryResults[i].gLTFBufferViews.ForEach(v =>
                    {
                        v.ByteOffset += buffer.ByteLength;
                    });

                    //CreateGLTFNodeConsiderSimilar(gLTF, p.GeometryResults[i], firstNode);

                    gLTFMesh.Primitives.AddRange(p.GeometryResults[i].gLTFPrimitives.ToArray());
                    gLTF.Accessors.AddRange(p.GeometryResults[i].gLTFAccessors.ToArray());
                    gLTF.BufferViews.AddRange(p.GeometryResults[i].gLTFBufferViews.ToArray());
                }
                nBufferViewIndex += p.GeometryResults.Sum(p1 => p1.gLTFAccessors.Count);
                var bs = p.DataStream.GetBuffer();
                buffer.DataStream.Write(bs, 0, (int)p.DataStream.Length);
                buffer.ByteLength += p.ByteLength;
            });

            CopyMaterial(gLTF);

            SaveImageData(gLTF, Images, buffer);

            gLTF.BindGLTF();
            return gLTF;
        }

        private void SaveImageData(GLTF gLTF, IEnumerable<GLTFImage> Images, GLTFBuffer buffer)
        {
            if (Images != null && Images.Count() > 0)
            {
                //图片的处理放在最后，加在bufferView数组的最后
                new ImageToGLTFBuffer()
                {
                    copyImageNameAndUrl = true,
                    copyDirectlyWhenImageIsUri = true
                }.AppendTo(gLTF, buffer, Images);

                if (gLTF.Samplers == null)
                {
                    //取样器,定义图片的采样和滤波方式
                    GLTFSampler sampler = new GLTFSampler();
                    sampler.MagFilter = GLTFMagFilter.Linear.GetHashCode();
                    sampler.MinFilter = GLTFMinFilter.LinearMipmapLinear.GetHashCode();
                    sampler.WrapS = GLTFWrappingMode.Repeat.GetHashCode();
                    sampler.WrapT = GLTFWrappingMode.Repeat.GetHashCode();
                    gLTF.Samplers = new GLTFSamplerCollection();
                    gLTF.Samplers.Add(sampler);
                }
            }
        }

        private void CreateGLTFNode(GLTF gLTF, GLTFNodeResult p, GLTFNode firstNode)
        {
            GLTFNode gLTFNode = new GLTFNode()
            {
                Extensions = p.Extensions
            };
            gLTF.Nodes.Add(gLTFNode);

            gLTFNode.MeshIndex = gLTF.Meshes.Count - 1;
            gLTFNode.Extensions = p.Extensions;
            firstNode.ChildrenID.Add(gLTF.Nodes.Count - 1);
        }

        private void CreateGLTFNodeConsiderSimilar(GLTF gLTF, GLTFNodeResult p, GLTFNode firstNode)
        {
            if (p.IsSimlilar)
            {
                p.SimlilarMatrixs.ForEach(matrix =>
                {
                    GLTFNode gLTFNode = new GLTFNode()
                    {
                        Extensions = p.Extensions,
                        MeshIndex = gLTF.Meshes.Count - 1,
                        Matrix = matrix
                    };
                    gLTF.Nodes.Add(gLTFNode);
                    firstNode.ChildrenID.Add(gLTF.Nodes.Count - 1);
                });
            }
            else
            {
                GLTFNode gLTFNode = new GLTFNode()
                {
                    Extensions = p.Extensions
                };
                gLTF.Nodes.Add(gLTFNode);
                gLTFNode.MeshIndex = gLTF.Meshes.Count - 1;
                firstNode.ChildrenID.Add(gLTF.Nodes.Count - 1);
            }
        }

        private void CopyMaterial(GLTF gLTF)
        {
            this.Where(p => p.Materials != null).SelectMany(p => p.Materials)?.ForEach(m =>
            {
                if (m != null && !gLTF.Materials.Contains(m))
                {
                    gLTF.Materials.Add(m);
                }
            });
        }

        private void UpdatePrimititveMaterialIndexF1(GLTFPrimitive gltfPrimitive, GLTF gLTF, GLTFResult gltfResult, IEnumerable<GLTFImage> Images)
        {
            if (gltfPrimitive.MaterialIndex == 1)
                return;

            var m = gltfResult.Materials[gltfPrimitive.MaterialIndex];
            if (gLTF.Materials.IndexOf(m) == -1)
            {
                gLTF.Materials.Add(m);
                if (m.ImageIndex != -1)
                {
                    var image = gltfResult.Images[m.ImageIndex];
                    m.PbrMetallicRoughness.BaseColorTexture.Index = gLTF.Images.IndexOf(image);
                }
            }
            else
            {
                gltfPrimitive.MaterialIndex = gLTF.Materials.IndexOf(m);
            }

        }
        private void UpdatePrimititveMaterialIndexF2(GLTFPrimitive p1, GLTF gLTF, GLTFResult p, IEnumerable<GLTFImage> Images)
        {
            if (p1.MaterialIndex == 1)
                return;

            var m = p.Materials[p1.MaterialIndex];
            if (gLTF.Materials.IndexOf(m) == -1)
            {
                gLTF.Materials.Add(m);
                if (m.ImageIndex != -1)
                {
                    var image = p.Images[m.ImageIndex];
                    Images.ForEachWithIndex((ii, index) =>
                    {
                        if (ii == image)
                        {
                            m.PbrMetallicRoughness.BaseColorTexture.Index = index;
                            return;
                        }
                    });
                }
            }
            else
            {
                p1.MaterialIndex = gLTF.Materials.IndexOf(m);
            }
        }

    }
}
