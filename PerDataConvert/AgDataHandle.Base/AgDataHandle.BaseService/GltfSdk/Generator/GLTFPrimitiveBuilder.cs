using AgDataHandle.Maths;
using AgDataHandle.BaseService.GltfSdk.BuildingContext;
using AgDataHandle.BaseService.GltfSdk.Def;

namespace AgDataHandle.BaseService.GltfSdk.Generator
{
    internal class GLTFPrimitiveBuilder : GLTFBuilder
    {
        /// <summary>
        /// 对 目标 GLTF模型对象 使用 上下文 GLTFPrimitiveBuildingContext 对象 创建 Primitive
        /// 注意，GLTF对象只能有一个 GLTFBuffer, 且其 DataStream 属性必须是 MemoryStream
        /// </summary>
        /// <param name="primitiveContext">创建 GLTFPrimitive 所需数据的上下文对象</param>
        /// <param name="model">待添加 Primitive 的 GLTF 对象 </param>
        public GLTFPrimitiveBuilder(GLTFPrimitiveBuildingContext primitiveContext, GLTF model)
        {
            prmtCtx = primitiveContext;
            prmt = new GLTFPrimitive { GLTF = model };
            this.model = model;
            var bf0DataStream = model.Buffers[0].DataStream;
            if (!(bf0DataStream is MemoryStream))
                throw new Exception("[GLTFPrimitiveBuilder Error] Buffers[0].DataStream 必须是内存流");
            dataStream = model.Buffers[0].DataStream as MemoryStream;
        }

        internal GLTFPrimitiveBuildingContext prmtCtx;
        internal GLTF model;
        internal MemoryStream dataStream;
        internal GLTFPrimitive prmt;

        protected virtual void AddIndicesData(GLTFPrimitive prmt, List<int> indicesArr)
        {
            GLTFBufferView bv;
            GLTFAccessor acc;
            int startPosition;
            if (indicesArr != null && indicesArr.Count != 0)
            {
                startPosition = (int)dataStream.Position;
                foreach (var id in indicesArr)
                    dataStream.Write(BitConverter.GetBytes((ushort)id), 0, 2);
                int bound = indicesArr.Count * 2 % 4;
                if (bound != 0) // bound 只可能是2或者0
                {
                    dataStream.WriteByte(0x20);
                    dataStream.WriteByte(0x20);
                }
                // bv
                bv = CreateBufferView(model, indicesArr.Count * 2, 34963, startPosition);
                model.BufferViews.Add(bv);
                // acc
                acc = CreateAccessor(bv, "SCALAR", 5123, indicesArr.Count);
                acc.Min = new List<float> { indicesArr.Min() };
                acc.Max = new List<float> { indicesArr.Max() };
                model.Accessors.Add(acc);
                // pri
                prmt.Indices = model.Accessors.IndexOf(acc);
            }
        }

        protected virtual void AddVertexAttrData(GLTFPrimitive prmt, string attrName, List<Vector2> data)
        {
            // 逐4字节写入，内存占用小
            int startPosition = (int)dataStream.Position;
            byte[] pEle;
            foreach (var v in data)
            {
                pEle = BitConverter.GetBytes(v.X);
                dataStream.Write(pEle, 0, 4);
                pEle = BitConverter.GetBytes(v.Y);
                dataStream.Write(pEle, 0, 4);
            }
            // bv
            var bv = CreateBufferView(model, data.Count * 8, 34962, startPosition);
            model.BufferViews.Add(bv);
            // acc
            var acc = CreateAccessor(bv, "VEC2", 5126, data.Count);
            model.Accessors.Add(acc);
            // pri
            prmt.Attributes.Add(attrName, model.Accessors.IndexOf(acc));
        }

        protected virtual void AddVertexAttrData(GLTFPrimitive prmt, string attrName, List<Vector3> data)
        {
            // 逐4字节写入，内存占用小
            int startPosition = (int)dataStream.Position;
            byte[] pEle;
            foreach (var v in data)
            {
                pEle = BitConverter.GetBytes(v.X);
                dataStream.Write(pEle, 0, 4);
                pEle = BitConverter.GetBytes(v.Y);
                dataStream.Write(pEle, 0, 4);
                pEle = BitConverter.GetBytes(v.Z);
                dataStream.Write(pEle, 0, 4);
            }
            // bv
            var bv = CreateBufferView(model, data.Count * 12, 34962, startPosition);
            model.BufferViews.Add(bv);
            // acc
            var acc = CreateAccessor(bv, "VEC3", 5126, data.Count,
                new List<float> { data.Max(p => p.X), data.Max(p => p.Y), data.Max(p => p.Z) },
                new List<float> { data.Min(p => p.X), data.Min(p => p.Y), data.Min(p => p.Z) });
            model.Accessors.Add(acc);
            // pri
            prmt.Attributes.Add(attrName, model.Accessors.IndexOf(acc));
        }

        public GLTFPrimitive Build()
        {
            bool positionCountNotDivideBy3 = prmtCtx.Position.Count % 3 != 0;
            bool indicesArrNull = prmtCtx.Indices == null;
            bool indicesArrEmpty = true;
            if (!indicesArrNull)
                indicesArrEmpty = prmtCtx.Indices.Count == 0;
            if (positionCountNotDivideBy3 && (indicesArrNull || indicesArrEmpty))
                throw new Exception("[GLTFSimpleFactory Create Error] 顶点数不是3的倍数且索引为空");

            if (!indicesArrNull && !indicesArrEmpty)
                AddIndicesData(prmt, prmtCtx.Indices);
            AddVertexAttrData(prmt, "POSITION", prmtCtx.Position);
            if (prmtCtx.Normal != null && prmtCtx.Normal.Count == prmtCtx.Position.Count)
                AddVertexAttrData(prmt, "NORMAL", prmtCtx.Normal);
            if (prmtCtx.UV0 != null && prmtCtx.UV0.Count == prmtCtx.Position.Count)
                AddVertexAttrData(prmt, "TEXCOORD_0", prmtCtx.UV0);

            model.Meshes[0].Primitives.Add(prmt);

            if (prmtCtx.MaterialContext != null)
                new GLTFMaterialBuilder(prmtCtx.MaterialContext, prmt, model).Build();

            return prmt;
        }
    }
}
