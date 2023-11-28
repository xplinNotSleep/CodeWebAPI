namespace AgDataHandle.BaseService.IOJson
{
    public class SelfJSONItem
    {
        public string Key;
        public string Value;
        public string StartChar;
        public JSONTree ValueTree = null;
        public Dictionary<string, SelfJSONItem> KeyChildren = new Dictionary<string, SelfJSONItem>();
        public List<JSONTree> GroupJSON = null;

        #region 构造函数
        public SelfJSONItem()
        {

        }
        public SelfJSONItem(string p1)
        {
            Key = p1;
        }
        public SelfJSONItem(string p1, string p2)
        {
            Key = p1;
            Value = p2;
        }

        public SelfJSONItem(string key, JSONTree paramsJSON)
        {
            Key = key;
            ValueTree = paramsJSON;
        }
        #endregion

        #region 子集很简单
        public void AddArrayItem(string[] p)
        {
            JSONTree jsonList = new JSONTree();
            for (var i = 0; i < p.Length; i++)
            {
                var kv = p[i].Split(':');
                SelfJSONItem jsonItem = new SelfJSONItem();
                jsonItem.Key = kv[0];
                jsonItem.Value = kv[1];
                jsonList.Items.Add(jsonItem);
            }
            if (GroupJSON == null)
            {
                GroupJSON = new List<JSONTree>();
            }
            GroupJSON.Add(jsonList);
        }

        public void AddArrayItem(string p)
        {
            JSONTree jsonList = new JSONTree();
            var kv = p.Split(':');
            SelfJSONItem jsonItem = new SelfJSONItem();
            jsonItem.Key = kv[0];
            jsonItem.Value = kv[1];
            jsonList.Items.Add(jsonItem);

            if (GroupJSON == null)
            {
                GroupJSON = new List<JSONTree>();
            }
            GroupJSON.Add(jsonList);
        }

        public void AddArrayItem(JSONTree jsonTree)
        {
            if (GroupJSON == null)
            {
                GroupJSON = new List<JSONTree>();
            }
            GroupJSON.Add(jsonTree);
        }
        #endregion

        #region 添加行
        public void AddRow(string p)
        {
            if (ValueTree == null)
            {
                ValueTree = new JSONTree();
            }
            var kv = p.Split(':');
            SelfJSONItem jsonItem = new SelfJSONItem();
            jsonItem.Key = kv[0];
            jsonItem.Value = kv[1];
            ValueTree.Items.Add(jsonItem);
        }
        public void AddRow(SelfJSONItem parentCenter)
        {
            if (ValueTree == null)
            {
                ValueTree = new JSONTree();
            }
            ValueTree.Items.Add(parentCenter);
        }
        #endregion
    }

    public class JSONTree
    {
        public List<SelfJSONItem> Items = new List<SelfJSONItem>();

        #region 属性
        public int Count
        {
            get
            {
                return Items.Count;
            }
        }
        public SelfJSONItem this[int index]
        {
            get
            {
                return Items[index];
            }
        }
        public SelfJSONItem this[string index]
        {
            get
            {
                for (var i = 0; i < Items.Count; i++)
                {
                    if (Items[i].Key == index)
                    {
                        return Items[i];
                    }
                }
                return null;
            }
        }
        #endregion

        #region 添加项
        public void AddItem(string keyValue)
        {
            AddItem(keyValue, false);
        }
        public void AddItem(string keyValue, bool valueIsStr)
        {
            var t1 = keyValue.IndexOf(':');
            var key = keyValue.Substring(0, t1);
            var value = keyValue.Substring(t1 + 1).Trim();
            SelfJSONItem jsonItem = new SelfJSONItem();
            jsonItem.Key = key;
            jsonItem.Value = valueIsStr ? "\"" + value + "\"" : value;
            Items.Add(jsonItem);
        }
        public void AddItem(SelfJSONItem item)
        {
            Items.Add(item);
        }

        public void AddItem(string key, List<JSONTree> childItems)
        {
            SelfJSONItem jsonItem = new SelfJSONItem(key);
            jsonItem.GroupJSON = childItems;
            Items.Add(jsonItem);
        }

        public void AddItem(string key, JSONTree paramsJSON)
        {
            SelfJSONItem jsonItem = new SelfJSONItem(key, paramsJSON);
            Items.Add(jsonItem);
        }


        #endregion


    }
}
