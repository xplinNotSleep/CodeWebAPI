using AgDataHandle.BaseService.GltfSdk.Collection;
using AgDataHandle.BaseService.GltfSdk.Def;
using AgDataHandle.BaseService.GltfSdk.ENUM;

namespace AgDataHandle.BaseService.GltfSdk.Append
{
    /// <summary>
    /// 这个类是把GLTFImage填充到gltf的buffer中，是共同提出的类。
    /// 需要多次测试。-陈彪 2023年3月28日
    /// </summary>
    public class ImageToGLTFBuffer
    {
        public bool copyImageNameAndUrl = false;
        public bool copyDirectlyWhenImageIsUri = false;
        public int bufferIndex = 0;

        /// <summary>
        /// 将多个GLTFImage添加到glTF中，并指定对应的buffer
        /// </summary>
        /// <param name="glTF"></param>
        /// <param name="Images"></param>
        /// <param name="buffer"></param>
        public void AppendTo(GLTF glTF, GLTFBuffer buffer, IEnumerable<GLTFImage> Images)
        {
            glTF.Images = new GLTFImageCollection();
            glTF.Textures = new GLTFTextureCollection();

            foreach (GLTFImage image in Images)
            {
                AppendTo(image, buffer, glTF);
            }
        }
        /// <summary>
        /// 把image的数据填充到glTF的缓存中
        /// </summary>
        /// <param name="image"></param>
        /// <param name="buffer"></param>
        /// <param name="gLTF"></param>
        /// <param name="copyImageNameAndUrl">是否复制name&Url，这个可能会导致url和数据同时存在，一般不要选</param>
        /// <param name="copyDirectlyWhenImageIsUri">如果传入的image是url类型，那么直接复杂，默认false</param>
        public void AppendTo(GLTFImage image, GLTFBuffer buffer, GLTF gLTF)
        {
            if (copyDirectlyWhenImageIsUri && image.ImageType == GLTFImageType.Uri)
            {
                image.GLTF = gLTF;
                gLTF.Images.Add(image);

                CreateTexture(gLTF);
                return;
            }

            GLTFBufferView bfv = new GLTFBufferView();
            bfv.BufferIndex = bufferIndex;
            bfv.ByteOffset = buffer.ByteLength;
            bfv.ByteLength = image.ImageSize;
            gLTF.BufferViews.Add(bfv);

            GLTFImage gltfImageNew = new GLTFImage(GLTFImageType.BufferView);
            gltfImageNew.BufferViewIndex = gLTF.BufferViews.Count - 1;
            gltfImageNew.MimeType = image.MimeType;
            gLTF.Images.Add(gltfImageNew);
            gltfImageNew.GLTF = gLTF;
            gLTF.Buffers[bufferIndex].AddRange(image.GetImageData());

            //补充，存在共享材质时需读取的参数Name、Uri
            if (copyImageNameAndUrl)
            {
                gltfImageNew.Name = image.Name;
                gltfImageNew.Uri = image.Uri;
            }

            CreateTexture(gLTF);
        }



        /// <summary>
        /// 贴图信息,使用source和ssampler指向图片和采样器
        /// </summary>
        /// <param name="gLTF"></param>
        private void CreateTexture(GLTF gLTF)
        {
            GLTFTexture texture = new GLTFTexture();
            texture.ImageIndex = gLTF.Images.Count - 1;
            texture.SamplerIndex = 0;
            gLTF.Textures.Add(texture);
        }
    }
}
