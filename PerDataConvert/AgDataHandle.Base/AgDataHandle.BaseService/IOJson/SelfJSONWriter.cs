//using System.IO;

//namespace AgDataHandle.IO.JSON
//{
//    public class SelfJSONWriter
//    {
//        public void WriteString(StreamWriter sw, JSONTree jsonTree)
//        {
//            sw.Write("{");
//            int index = 0;
//            for (var i = 0; i < jsonTree.Count; i++)
//            {
//                WriteString(sw, jsonTree[i].Key, jsonTree[i], i != jsonTree.Count - 1);

//                index++;
//            }
//            sw.Write("}");
//        }
//        public void WriteString(StreamWriter sw, string key, SelfJSONItem jSONItem, bool withDh = true)
//        {
//            sw.Write("\"" + key + "\":");
//            if (jSONItem.KeyChildren != null && jSONItem.KeyChildren.Count > 0)
//            {
//                sw.Write("{");
//                int index = 0;
//                foreach (var subKey in jSONItem.KeyChildren.Keys)
//                {
//                    WriteString(sw, subKey, jSONItem.KeyChildren[subKey], index != jSONItem.KeyChildren.Keys.Count - 1);

//                    index++;
//                }
//                sw.Write("}");
//            }
//            if (jSONItem.ValueTree != null && jSONItem.ValueTree.Count > 0)
//            {
//                sw.Write("{");
//                int index = 0;
//                for (var i = 0; i < jSONItem.ValueTree.Count; i++)
//                {
//                    WriteString(sw, jSONItem.ValueTree[i].Key, jSONItem.ValueTree[i], i != jSONItem.ValueTree.Count - 1);

//                    index++;
//                }
//                sw.Write("}");
//            }
//            else if (jSONItem.GroupJSON != null)
//            {
//                sw.Write("[");
//                for (var i = 0; i < jSONItem.GroupJSON.Count; i++)
//                {
//                    if (i != 0)
//                    {
//                        sw.Write(',');
//                    }
//                    sw.Write("{");
//                    var items = jSONItem.GroupJSON[i].Items;
//                    for (var j = 0; j < items.Count; j++)
//                    {
//                        WriteString(sw, items[j].Key, items[j], j != items.Count - 1);
//                    }
//                    sw.Write("}");
//                }
//                sw.Write("]");
//            }
//            else if (string.IsNullOrEmpty(jSONItem.Value) == false)
//            {
//                sw.Write(jSONItem.Value);
//            }

//            if (withDh)
//                sw.Write(",");
//        }

//        public void WriteString(StreamWriter sw, string key, string value, bool withDh = true)
//        {
//            sw.Write("\"" + key + "\": \"" + value + "\"");
//            if (withDh)
//                sw.Write(",");
//        }
//        public void WriteString(StreamWriter sw, string key, int value, bool withDh = true)
//        {
//            sw.Write("\"" + key + "\": " + value + "");
//            if (withDh)
//                sw.Write(",");
//        }
//        public void WriteString(StreamWriter sw, string key, float[] value, bool withDh = true)
//        {
//            string vs = "";
//            for (var i = 0; i < value.Length; i++)
//            {
//                if (i != 0)
//                {
//                    vs += ",";
//                }
//                vs += value[i];
//            }

//            sw.Write("\"" + key + "\": [" + vs + "]");
//            if (withDh)
//                sw.Write(",");
//        }

//        public StreamWriter CreateMemoryWriter()
//        {
//            MemoryStream ms = new MemoryStream();
//            StreamWriter sw = new StreamWriter(ms);
//            return sw;
//        }
//        public string ReadMemoryWriter(StreamWriter sw)
//        {
//            MemoryStream ms = sw.BaseStream as MemoryStream;
//            sw.Flush();
//            ms.Seek(0, SeekOrigin.Begin);
//            StreamReader sr = new StreamReader(ms);
//            var str = sr.ReadToEnd();
//            ms.Dispose();
//            sr.Dispose();
//            sw.Dispose();

//            return str;
//        }
//    }
//}
