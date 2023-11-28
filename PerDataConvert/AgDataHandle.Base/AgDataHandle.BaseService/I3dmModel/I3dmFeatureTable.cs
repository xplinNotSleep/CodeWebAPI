using AgDataHandle.BaseService.I3dmModel;
using AgDataHandle.BaseService.Utils;
using AgDataHandle.Maths;
using Newtonsoft.Json;
using ServiceCenter.Core;

namespace AgDataHandle.BaseService.I3dmModel
{
    public class I3dmFeatureTable
    {
        public List<Vector3> Positions { get; set; }
        public List<Vector3> NormalUps { get; set; }
        public List<Vector3> NormalRights { get; set; }
        public List<Vector3> ScaleNonUniforms { get; set; }
        public List<float> Scales { get; set; }
        public List<ushort> BatchIds { get; set; }
        public Vector3 RtcCenter { get; set; }
        //增加Extensions字段
        public List<ExtensionsField> ExtensionsFields { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchIdSerializeType"></param>
        /// <param name="batchIdBytesLength"></param>
        /// <returns></returns>
        public string GetFeatureTableJson(string batchIdSerializeType = "UNSIGNED_SHORT", int batchIdBytesLength = 0)
        {
            I3dmFeatureTableJson i3DmFeatureTableJson = new I3dmFeatureTableJson();
            var offset = 0;
            i3DmFeatureTableJson.InstancesLength = Positions.Count;
            i3DmFeatureTableJson.PositionOffset = new I3dmByteOffset() { offset = offset };
            offset += Positions.ToBytes().Count();
            if (NormalUps != null)
            {
                i3DmFeatureTableJson.NormalUpOffset = new I3dmByteOffset() { offset = offset };
                offset += NormalUps.ToBytes().Count();
            }
            if (NormalRights != null)
            {
                i3DmFeatureTableJson.NormalRightOffset = new I3dmByteOffset() { offset = offset };
                offset += NormalRights.ToBytes().Count();
            }
            if (ScaleNonUniforms != null)
            {
                i3DmFeatureTableJson.ScaleNonUniformOffset = new I3dmByteOffset() { offset = offset };
                offset += ScaleNonUniforms.ToBytes().Count();
            }
            if (Scales != null)
            {
                i3DmFeatureTableJson.ScaleOffset = new I3dmByteOffset() { offset = offset };
                offset += Scales.ToBytes().Count();
            }
            if (RtcCenter != null)
            {
                i3DmFeatureTableJson.RtcCenter = RtcCenter;
                offset += RtcCenter.ToBytes().Count();
            }
            //增加Extensions字段
            if (ExtensionsFields != null)
            {
                i3DmFeatureTableJson.ExtensionsOffset = new I3dmByteOffset() { offset = offset };
                offset += ExtensionsFields.ToBytes().Count();
            }
            if (BatchIds != null)
            {
                i3DmFeatureTableJson.BatchIdOffset = new I3dmByteOffset() { offset = offset, componentType = batchIdSerializeType };
                // not needed beacuse last one: offset += batchIdBytesLength;
            }

            string FeatureTableJson = JsonConvert.SerializeObject(i3DmFeatureTableJson, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            return FeatureTableJson;
        }

        /// <summary>
        /// 获取FeatureTableBinary
        /// </summary>
        /// <returns></returns>
        public byte[] GetFeatureTableBinary()
        {
            List<byte> featureTableBinary = new List<byte>();

            featureTableBinary.AddRange(Positions.ToBytes());
            if (NormalUps != null)
            {
                featureTableBinary.AddRange(NormalUps.ToBytes());
            }
            if (NormalRights != null)
            {
                featureTableBinary.AddRange(NormalRights.ToBytes());
            }
            if (ScaleNonUniforms != null)
            {
                featureTableBinary.AddRange(ScaleNonUniforms.ToBytes());
            }
            if (Scales != null)
            {
                featureTableBinary.AddRange(Scales.ToBytes());
            }
            if (BatchIds != null)
            {
                BatchIds.ForEach(batchid =>
                {
                    featureTableBinary.AddRange(BitConverter.GetBytes(batchid));
                });
            }
            if (RtcCenter != null)
            {
                featureTableBinary.AddRange(RtcCenter.ToBytes());
            }
            //增加Extension字段
            if (ExtensionsFields != null)
            {
                featureTableBinary.AddRange(ExtensionsFields.ToBytes());
            }
            return featureTableBinary.ToArray();
        }


        public void FromBinaryAndJson(I3dmFeatureTableJson i3DmFeatureTableJson, byte[] Binary)
        {
            var instanceCount = i3DmFeatureTableJson.InstancesLength;

            if (i3DmFeatureTableJson.PositionOffset != null)
                Positions = Vector3Extensions.FromBytes(Binary, i3DmFeatureTableJson.PositionOffset.offset, instanceCount);

            if (i3DmFeatureTableJson.NormalUpOffset != null)
                NormalUps = Vector3Extensions.FromBytes(Binary, i3DmFeatureTableJson.NormalUpOffset.offset, instanceCount);

            if (i3DmFeatureTableJson.NormalRightOffset != null)
                NormalRights = Vector3Extensions.FromBytes(Binary, i3DmFeatureTableJson.NormalRightOffset.offset, instanceCount);

            if (i3DmFeatureTableJson.ScaleNonUniformOffset != null)
                ScaleNonUniforms = Vector3Extensions.FromBytes(Binary, i3DmFeatureTableJson.ScaleNonUniformOffset.offset, instanceCount);

            if (i3DmFeatureTableJson.ScaleOffset != null)
            {
                Scales = new List<float>();
                var bytes = Binary.Skip(i3DmFeatureTableJson.ScaleOffset.offset).Take(instanceCount * 4).ToArray();
                var reader = new BinaryReader(new MemoryStream(bytes));
                for (int i = 0; i < instanceCount; i++)
                {
                    Scales.Add(reader.ReadSingle());
                }
            }

            if (i3DmFeatureTableJson.RtcCenter != null)
                RtcCenter = i3DmFeatureTableJson.RtcCenter;

            if (i3DmFeatureTableJson.BatchIdOffset != null)
            {
                BatchIds = new List<ushort>();
                var bytes = Binary.Skip(i3DmFeatureTableJson.BatchIdOffset.offset).Take(instanceCount * 2).ToArray();
                var reader = new BinaryReader(new MemoryStream(bytes));
                for (int i = 0; i < instanceCount; i++)
                {
                    BatchIds.Add(reader.ReadUInt16());
                }
            }
        }
        public void FromBinaryAndJson(string i3DmFeatureTableStr, byte[] Binary)
        {
            var i3dmFeatureTableJson = i3DmFeatureTableStr.FromJson<I3dmFeatureTableJson>();
            FromBinaryAndJson(i3dmFeatureTableJson, Binary);
        }
    }

    public class I3dmFeatureTableJson
    {
        [JsonProperty("INSTANCES_LENGTH")]
        public int InstancesLength { get; set; }

        [JsonProperty("POSITION")]
        public I3dmByteOffset PositionOffset { get; set; }

        [JsonProperty("NORMAL_UP")]
        public I3dmByteOffset NormalUpOffset { get; set; }

        [JsonProperty("NORMAL_RIGHT")]
        public I3dmByteOffset NormalRightOffset { get; set; }

        [JsonProperty("SCALE_NON_UNIFORM")]
        public I3dmByteOffset ScaleNonUniformOffset { get; set; }

        [JsonProperty("SCALE")]
        public I3dmByteOffset ScaleOffset { get; set; }

        [JsonProperty("BATCH_ID")]
        public I3dmByteOffset BatchIdOffset { get; set; }

        [JsonProperty("EAST_NORTH_UP")]
        public bool IsEastNorthUp { get; set; } = true;

        [JsonProperty("RTC_CENTER")]
        public Vector3 RtcCenter { get; set; }
        //新增Extensions字段
        [JsonProperty("extensions")]
        public I3dmByteOffset ExtensionsOffset { get; set; }

    }
}
