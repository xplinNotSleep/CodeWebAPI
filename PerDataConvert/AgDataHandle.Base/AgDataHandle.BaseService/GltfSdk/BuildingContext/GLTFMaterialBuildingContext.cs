using AgDataHandle.BaseService.GltfSdk.Def;
using System.ComponentModel;

namespace AgDataHandle.BaseService.GltfSdk.BuildingContext
{
    public sealed class GLTFMaterialBuildingContext : GLTFBuildingContext
    {
        public bool IsDoubleSided { get; set; }

        public string AlphaMode { get; set; }

        public float AlphaCutOff { get; set; }

        [Description("基础颜色值，可空，[RGBA]范围是0~1")]
        public float[] BaseColor { get; set; }

        [Description("自发光色，可空，[RGB]范围是0~1")]
        public float[] EmissiveColor { get; set; }

        [Description("基本颜色贴图，如果设置此参数必须设置 UV，否则会抛异常")]
        public byte[] BaseTextureImage { get; set; }

        [Description("法线贴图，如果设置此参数必须设置 UV，否则会抛异常")]
        public byte[] NormalTextureImage { get; set; }

        [Description("高光贴图，如果设置此参数必须设置 UV，否则会抛异常")]
        public byte[] EmissiveTextureImage { get; set; }

        [Description("遮罩贴图，如果设置此参数必须设置 UV，否则会抛异常")]
        public byte[] OcclusionTextureImage { get; set; }

        [Description("金属光泽度")]
        public float Metallic { get; set; }

        [Description("粗糙度")]
        public float Roughness { get; set; }

        public string Name { get; set; }

        #region Methods

        public static GLTFMaterialBuildingContext LoadFromObject(GLTFMaterial material)
        {
            GLTFMaterialBuildingContext ctx = new GLTFMaterialBuildingContext
            {
                Name = material.Name.Clone() as string,
                IsDoubleSided = material.DoubleSided,
                AlphaMode = material.AlphaMode.ToString(),
                AlphaCutOff = (float)material.AlphaCutOff
            };
            if (material.EmissiveFactor != null)
            {
                ctx.EmissiveColor = material.EmissiveFactor.Clone() as float[];
            }
            if (material.PbrMetallicRoughness != null && !material.PbrMetallicRoughness.IsAllDefault)
            {
                ctx.BaseColor = material.PbrMetallicRoughness.BaseColorFactor;
                ctx.Roughness = (float)material.PbrMetallicRoughness.RoughnessFactor;
                ctx.Metallic = (float)material.PbrMetallicRoughness.MetallicFactor;
            }

            return ctx;
        }

        #endregion
    }
}
