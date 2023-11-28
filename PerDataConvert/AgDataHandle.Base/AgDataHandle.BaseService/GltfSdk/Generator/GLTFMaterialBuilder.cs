using AgDataHandle.BaseService.CommonHelper;
using AgDataHandle.BaseService.GltfSdk.BuildingContext;
using AgDataHandle.BaseService.GltfSdk.Def;
using AgDataHandle.BaseService.GltfSdk.ENUM;

namespace AgDataHandle.BaseService.GltfSdk.Generator
{
    internal class GLTFMaterialBuilder : GLTFBuilder
    {
        public GLTFMaterialBuilder(GLTFMaterialBuildingContext materialContext, GLTFPrimitive targetPrmt, GLTF model)
        {
            materialCtx = materialContext;
            material = new GLTFMaterial { GLTF = model };
            material.Name = materialContext.Name;
            this.model = model;
            this.targetPrmt = targetPrmt;

            var bf0DataStream = model.Buffers[0].DataStream;
            if (!(bf0DataStream is MemoryStream))
                throw new Exception("[GLTFPrimitiveBuilder Error] Buffers[0].DataStream 必须是内存流");
            dataStream = model.Buffers[0].DataStream as MemoryStream;
        }

        private GLTFMaterialBuildingContext materialCtx;
        private GLTF model;
        private GLTFPrimitive targetPrmt;
        private MemoryStream dataStream;
        private GLTFMaterial material;

        public void Build()
        {
            GLTFPbrMetallicRoughness pbr = new GLTFPbrMetallicRoughness();
            GLTFImage img = null;
            GLTFTexture baseColorTex = null;

            if (materialCtx.BaseColor != null)
                pbr.BaseColorFactor = materialCtx.BaseColor;
            if (materialCtx.BaseTextureImage != null)
            {
                if (targetPrmt == null || !targetPrmt.Attributes.ContainsKey("TEXCOORD_0"))
                    throw new Exception("[GLTFSimpleBuilder.BuildMaterial() Error] 颜色贴图必须设定UVO");

                // 填入img
                int beforeLength = materialCtx.BaseTextureImage.Length;
                int startPosition = (int)dataStream.Position;
                materialCtx.BaseTextureImage = EncodingHelper.FillBlank(materialCtx.BaseTextureImage);
                dataStream.Write(materialCtx.BaseTextureImage, 0, materialCtx.BaseTextureImage.Length);
                // 创建BufferView
                GLTFBufferView bv = CreateBufferView(model, beforeLength, -1);
                bv.ByteOffset = startPosition;
                model.BufferViews.Add(bv);
                // 修改img
                img = img ?? new GLTFImage { GLTF = model };
                img.ImageType = GLTFImageType.BufferView;
                img.MimeType = "image/jpeg";
                img.DataStream = dataStream;
                img.BufferViewIndex = model.BufferViews.IndexOf(bv);
                model.Images.Add(img);
                // 创建texture
                baseColorTex = new GLTFTexture
                {
                    GLTF = model,
                    ImageIndex = model.Images.IndexOf(img)
                };
                model.Textures.Add(baseColorTex);
                // 修改pbr中的材质定义
                pbr.BaseColorTexture = new GLTFTextureInfo
                {
                    GLTF = model,
                    Index = model.Textures.IndexOf(baseColorTex),
                    TexCoord = 0
                };
            }

            // 金属光泽度、粗糙度
            if (materialCtx.Metallic >= 0 && materialCtx.Metallic < 1)
                pbr.MetallicFactor = materialCtx.Metallic;
            if (materialCtx.Roughness >= 0 && materialCtx.Roughness < 1)
                pbr.RoughnessFactor = materialCtx.Roughness;

            if (!pbr.IsAllDefault)
            {
                if (materialCtx.AlphaMode == null)
                {
                    materialCtx.AlphaCutOff = 0.5f;
                    material.AlphaMode = GLTFAlphaMode.OPAQUE.ToString();
                }
                else if (!materialCtx.AlphaMode.Equals("BLEND") &&
                        !materialCtx.AlphaMode.Equals("MASK") &&
                        !materialCtx.AlphaMode.Equals("OPAQUE"))
                {
                    if (materialCtx.AlphaCutOff > 0 && materialCtx.AlphaCutOff != 0.5)
                        material.AlphaCutOff = materialCtx.AlphaCutOff;
                    else
                        throw new Exception("AlphaCutOff 参数异常");
                    material.AlphaMode = materialCtx.AlphaMode.ToString();
                }
                if (materialCtx.IsDoubleSided)
                    material.DoubleSided = materialCtx.IsDoubleSided;

                material.PbrMetallicRoughness = pbr;
            }

            // 最好加一层判断material是否为默认值
            model.Materials.Add(material);
            targetPrmt.MaterialIndex = model.Materials.IndexOf(material);
        }
    }
}
