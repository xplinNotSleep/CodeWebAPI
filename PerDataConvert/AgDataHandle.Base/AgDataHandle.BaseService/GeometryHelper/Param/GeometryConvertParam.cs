namespace AgDataHandle.BaseService.GeometryHelper.Param
{
    public class GeometryConvertParam
    {
        /// <summary>
        /// 参数为0时默认不拆分，生成一个gltf
        /// </summary>
        public double dMaxSize { get; set; } = 0;
        /// <summary>
        /// 是否使用扩展属性
        /// </summary>
        public bool IsUseExtension { get; set; } = false;
        /// <summary>
        /// 是否翻转YZ
        /// </summary>
        public bool IsInvertYZ { get; set; } = false;

    }
}
