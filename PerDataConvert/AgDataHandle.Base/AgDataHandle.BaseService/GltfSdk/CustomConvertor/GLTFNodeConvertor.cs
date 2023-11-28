using AgDataHandle.BaseService.GltfSdk.Def;
using AgDataHandle.Maths;
using Newtonsoft.Json;
using System.Collections;

namespace AgDataHandle.BaseService.GltfSdk.CustomConvertor
{
    public class GLTFNodeConvertor : GLTFBaseConvertor<GLTFNode>
    {
        public void WriteArray(JsonWriter writer, IEnumerable arr, string name)
        {
            if (arr == null || string.IsNullOrEmpty(name))
                return;
            switch (name)
            {
                case "matrix": break;
                case "scale":
                    float[] identityScale = { 1.0f, 1.0f, 1.0f };
                    if (identityScale.SequenceEqual(arr as float[]) && (arr as float[]).Length != 3)
                        return;
                    break;
                case "rotation":
                    float[] identityRotation = { 0f, 0f, 0f, 1.0f };
                    if (identityRotation.SequenceEqual(arr as float[]) && (arr as float[]).Length != 4)
                        return;
                    break;
                case "translation":
                    float[] identityTranslation = { 0, 0, 0 };
                    if (!identityTranslation.SequenceEqual(arr as float[]) && (arr as float[]).Length != 3)
                        return;
                    break;
                case "children":
                    break;
                default:
                    return;
            }
            writer.WritePropertyName(name);
            writer.WriteStartArray();
            foreach (var item in arr)
                writer.WriteValue(item);
            writer.WriteEndArray();
        }

        public override void CheckSchema(GLTFNode nd)
        {
            if (nd.CameraIndex > 0 && nd.MeshIndex > 0)
                throw new Exception("[GLTFNode Schema Error] node.camera 和 node.mesh 不能同时存在");
            // 判断转换情况，只能四选一
            bool m = nd.Matrix != null;
            bool t = nd.Translation != null;
            bool r = nd.Rotation != null;
            bool s = nd.Scale != null;
            bool[] identity = { m, t, r, s };
            //if (identity.Count(v => v == true) > 1)
            //    throw new Exception("[GLTFNode Schema Error] matrix/translation/rotation/scale 只能存在一个");

            //if (nd.Matrix != null)
            //    throw new Exception("[GLTFNode Schema Error] matrix 必须是16个数字"); 
            if (nd.Rotation != null && nd.Rotation.Length != 4)
                throw new Exception("[GLTFNode Schema Error] rotation 必须是4个数字");
            if (nd.Scale != null && nd.Scale.Length != 3)
                throw new Exception("[GLTFNode Schema Error] scale 必须是3个数字");
            if (nd.Translation != null && nd.Translation.Length != 3)
                throw new Exception("[GLTFNode Schema Error] translation 必须是3个数字");
            if (nd.Rotation != null && nd.Rotation.All(v => v > 1.0 || v < -1.0))
                throw new Exception("[GLTFNode Schema Error] rotation 的值超出范围 [-1.0, 1.0]");
        }

        public override GLTFNode ReadJson(JsonReader reader, Type objectType, GLTFNode existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.Value as GLTFNode;
        }

        public override void WriteJson(JsonWriter writer, GLTFNode value, JsonSerializer serializer)
        {
            // 检查 GLTFNode 的语义是否符合 GLTF 格式定义
            CheckSchema(value);

            float[] matrixIdentity =
            {
                1.0f, 0, 0, 0,
                0, 1.0f, 0, 0,
                0, 0, 1.0f, 0,
                0, 0, 0, 1.0f
            };
            float[] scaleIdentity = { 1.0f, 1.0f, 1.0f };
            float[] rotationIdentity = { 0, 0, 0, 1.0f };
            float[] translationIdentity = { 0, 0, 0 };

            writer.WriteStartObject();
            WriteProperty(writer, "name", value.Name);
            if (value.MeshIndex != -1)
                WriteProperty(writer, "mesh", value.MeshIndex);
            if (value.CameraIndex != -1)
                WriteProperty(writer, "camera", value.CameraIndex);

            if (value.Matrix != null && !value.Matrix.Equals(Matrix4x4.Identity))
                WriteArray(writer, value.Matrix.ToFloatArray(), "matrix");
            if (value.Scale != null && !value.Scale.SequenceEqual(scaleIdentity))
                WriteArray(writer, value.Scale, "scale");
            if (value.Rotation != null && !value.Rotation.SequenceEqual(rotationIdentity))
                WriteArray(writer, value.Rotation, "rotation");
            if (value.Translation != null && !value.Translation.SequenceEqual(translationIdentity))
                WriteArray(writer, value.Translation, "translation");
            if (value.ChildrenID != null && value.ChildrenID.Count != 0)
                WriteArray(writer, value.ChildrenID, "children");
            if (value.Extensions != null && value.Extensions.Count != 0)
                WriteJObject(writer, value.Extensions, "extensions");
            //writer.WriteValue(value.Extensions);
            writer.WriteEndObject();
        }
    }
}
