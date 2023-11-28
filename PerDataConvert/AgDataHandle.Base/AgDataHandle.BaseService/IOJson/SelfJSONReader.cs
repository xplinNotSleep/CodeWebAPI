namespace AgDataHandle.BaseService.IOJson
{
    public class SelfJSONReader
    {
        public List<SelfJSONItem> Items = new List<SelfJSONItem>();
        public SelfJSONReader(string str)
        {
            Items = Read(str);
        }
        private List<SelfJSONItem> Read(string str)
        {
            str = str.Trim();
            str = str.TrimStart('{');
            str = StringEx.TrimEndWithFirst(str, new char[] { '}' });
            str = str.Trim();

            int endIndex = 0;
            List<SelfJSONItem> items = new List<SelfJSONItem>();
            while (str.Length > 2)
            {
                SelfJSONItem item = ReadStringJSON(str, out endIndex);
                items.Add(item);
                if (endIndex >= str.Length)
                    break;
                str = str.Substring(endIndex);
                str = str.Trim();
                str = str.TrimStart(',');
                str = str.Trim();
            }

            return items;
        }

        #region 切割数组
        public List<string> SplitArr(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            var firstCharIndex = getNextCharIndex(str, 0);
            if (firstCharIndex == -1 || firstCharIndex >= str.Length)
            {
                if (str[firstCharIndex] != '[')
                {
                    throw new Exception("isNot Arr");
                }
            }
            str = str.Trim();
            str = str.TrimStart('[');
            str = StringEx.TrimEndWithFirst(str, new char[] { ']' });
            str = str.Trim();
            List<string> childItems = new List<string>();
            while (true)
            {
                if (str.Length < 2)
                {
                    break;
                }
                var k1 = GetEndArr(str, '{', '}');
                var childItem = str.Substring(0, k1 + 1);
                childItem = childItem.TrimStart(new char[] { ',', ' ', '\r', '\n' });
                childItems.Add(childItem);
                str = str.Substring(k1 + 1);
                str = str.TrimStart(new char[] { ',', ' ', '\r', '\n' });
                str = str.TrimStart();
            }
            return childItems;
        }
        public List<string> SplitArrByFirstKey(string str)
        {
            int t1 = str.IndexOf('"');
            int t2 = str.IndexOf('"', t1 + 1);
            var firstKey = str.Substring(t1 + 1, t2 - t1 - 1);
            List<string> rs = new List<string>();
            int fromIndex = 0;
            while (true)
            {
                var t3 = str.IndexOf(firstKey, fromIndex);
                var t4 = str.IndexOf(firstKey, t3 + firstKey.Length);
                if (t3 < 0 || t4 < 0)
                {
                    if (t3 > 0 && t4 < 0)
                    {
                        t4 = str.Length;
                        var strEnd = str.Substring(t3, t4 - t3);
                        int t5End = strEnd.LastIndexOf('}');
                        rs.Add("{\"" + strEnd.Substring(0, t5End + 1));
                    }
                    break;
                }

                var str1 = str.Substring(t3, t4 - t3);
                int t5 = str1.LastIndexOf('}');
                rs.Add("{\"" + str1.Substring(0, t5 + 1));
                fromIndex = t4;
            }

            return rs;
        }
        #endregion

        public string GetValue(string key, bool withCode = false)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                if (Items[i].Key == key)
                {
                    if (withCode)
                    {
                        if (Items[i].StartChar.Trim() == "\"")
                        {
                            return "\"" + Items[i].Value + "\"";
                        }
                    }
                    return Items[i].Value;
                }
            }
            return null;
        }

        public SelfJSONItem GetItem(string key)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                if (Items[i].Key == key)
                {
                    return Items[i];
                }
            }
            return null;
        }

        private SelfJSONItem ReadStringJSON(string str, out int endIndex)
        {
            var t = str.IndexOf(':');
            var t2 = getNextCharIndex(str, t + 1);
            var nextChar = str[t2];
            int t3 = -1;
            if (nextChar == '"')
            {
                t2 = t2 + 1;
                t3 = GetNextYinHaoIndex(str, t2);
            }
            else if (nextChar == '[')
            {
                t3 = GetEndArr(str, '[', ']') + 1;
            }
            else if (nextChar == '{')
            {
                t3 = GetEndArr(str, '{', '}') + 1;
            }
            else
            {
                t3 = GetNextSignedChar(str, t2);
            }

            if (t3 != -1)
            {
                SelfJSONItem item = new SelfJSONItem();
                item.Key = str.Substring(1, t - 2).Trim().Trim('\"');
                item.Value = str.Substring(t2, t3 - t2);
                item.StartChar = str.Substring(t + 1, t2 - t - 1);
                endIndex = t3 + 1;
                return item;
            }
            throw new Exception("ddef");

        }

        private int getNextCharIndex(string str, int t)
        {
            while (char.IsWhiteSpace(str, t))
            {
                t++;
            }
            return t;
        }

        private int GetNextYinHaoIndex(string str, int t)
        {
            while (t < str.Length)
            {
                if (str[t] == '"')
                {
                    if (t > 0 && str[t - 1] == '\\')
                    {

                    }
                    else
                    {
                        return t;
                    }
                }
                t++;
            }
            return -1;
        }

        private int GetEndArr(string str, char startChar, char endChar)
        {
            int t = 0;
            int saveCount = -1;
            while (true)
            {
                if (str[t] == '"')
                {
                    t++;
                    do
                    {
                        var kk = str[t] != '"';
                        if (kk == false)
                        {
                            if (t > 0 && str[t - 1] == '\\')
                            {
                                if (t > 1 && str[t - 2] == '\\')
                                {
                                    kk = false;
                                }
                                else
                                {
                                    kk = true;
                                }
                            }
                        }
                        if (kk)
                        {
                            t++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    while (true);
                }
                if (str[t] == startChar)
                {
                    if (saveCount == -1)
                    {
                        saveCount = 1;
                    }
                    else
                    {
                        saveCount++;
                    }
                }
                if (str[t] == endChar)
                {
                    saveCount--;
                }
                if (saveCount == 0)
                {
                    return t;
                }
                t++;
            }
            throw new Exception("error");
        }


        private int GetNextSignedChar(string str, int fromIndex)
        {
            fromIndex = fromIndex + 1;
            while (str[fromIndex] != ',' && str[fromIndex] != ']' && str[fromIndex] != '}')
            {
                fromIndex++;
                if (fromIndex >= str.Length)
                {
                    break;
                }
            }
            return fromIndex;
        }
    }



    public class StringEx
    {
        public static string TrimEndWithFirst(string str, char[] s)
        {
            int lastIndex = str.Length - 1;
            for (lastIndex = str.Length - 1; lastIndex > -1; lastIndex--)
            {
                if (str[lastIndex] == ' ' || str[lastIndex] == '\t' || str[lastIndex] == '\r' || str[lastIndex] == '\n')
                    continue;
                var f = false;
                for (var j = 0; j < s.Length; j++)
                {
                    if (s[j] == str[lastIndex])
                    {
                        return str.Substring(0, lastIndex);
                    }
                }
                if (f == false)
                {
                    return str.Substring(0, lastIndex + 1);
                }

            }
            return str.Substring(0, lastIndex + 1);
        }
    }
}
