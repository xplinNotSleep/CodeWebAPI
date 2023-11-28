namespace AgDataHandle.BaseService.GltfSdk.ENUM
{
    public enum GLTFImageType
    {
        BufferView,
        Uri,
        Base64,
        Bytes
    }

    public enum GLTFTargets
    {
        ARRAY_BUFFER = 34962, // 代表顶点数据
        ELEMENT_ARRAY_BUFFER = 34963 // 代表顶点索引数据
    }

    public enum GLTFAccessorType
    {
        SCALAR,
        VEC2,
        VEC3,
        VEC4,
        MAT2,
        MAT3,
        MAT4
    }

    public enum GLTFAccessorComponentType
    {
        BYTE = 5120,
        UNSIGNED_BYTE = 5121,
        SHORT = 5122,
        UNSIGNED_SHORT = 5123,
        UNSIGNED_INT = 5125,
        FLOAT = 5126
    }

    public enum GLTFModeEnum
    {
        POINTS = 0,
        LINES = 1,
        LINE_LOOP = 2,
        LINE_STRIP = 3,
        TRIANGLES = 4,
        TRIANGLE_STRIP = 5,
        TRIANGLE_FAN = 6
    }

    /// <summary>
    /// The alpha rendering mode of a material.
    /// </summary>
    public enum GLTFAlphaMode
    {
        /// The alpha value is ignored and the rendered output is fully opaque.
        OPAQUE = 1,

        /// The rendered output is either fully opaque or fully transparent depending on
        /// the alpha value and the specified alpha cutoff value.
        MASK,

        /// The rendered output is either fully opaque or fully transparent depending on
        /// the alpha value and the specified alpha cutoff value.
        BLEND,
    }

    /// <summary>
    /// Minification filter.
    /// </summary>
    public enum GLTFMinFilter
    {
        None = 0,
        Nearest = 9728,
        Linear = 9729,
        NearestMipmapNearest = 9984,
        LinearMipmapNearest = 9985,
        NearestMipmapLinear = 9986,
        LinearMipmapLinear = 9987
    }
    /// <summary>
    /// Magnification filter.
    /// </summary>
    public enum GLTFMagFilter
    {
        None = 0,
        Nearest = 9728,
        Linear = 9729,
    }

    /// <summary>
    /// Texture co-ordinate wrapping mode.
    /// </summary>
    public enum GLTFWrappingMode
    {
        None = 0,
        /// Corresponds to `GL_CLAMP_TO_EDGE`.
        ClampToEdge = 33071,

        /// Corresponds to `GL_MIRRORED_REPEAT`.
        MirroredRepeat = 33648,

        /// Corresponds to `GL_REPEAT`.
        Repeat = 10497,
    }

    /// <summary>
    /// 几何数据类型（可扩充）,1-是内置构件，2-不是是内置构件,3-全部构件
    /// </summary>
    public enum NodeTypeEnum
    {
        InnerComponent = 1,
        CommonComponent = 2,
        All = 3
    }

    /// <summary>
    /// 本定义仅作为说明使用
    /// </summary>
    public enum GLTFMeshPrimitiveState
    {
        MeshWithMultiPrimitives,
        MeshWithSinglePrimitive
    }

    public enum AgImageFormat
    {
        defaultFormat,//即原始数据图片格式
        jpg,
        png,
        webp,
        gif,
        ktx2,
        dds
    }

    public enum GLTFCollectionType
    {
        Accessor,
        Buffer,
        BufferView,
        Scene,
        Node,
        Mesh,
        Primitive,
        Material,
        Sampler,
        Texture,
        Image,
        Extension
    }
}
