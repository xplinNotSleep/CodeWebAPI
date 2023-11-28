using Newtonsoft.Json.Linq;

namespace AgDataHandle.BaseService.CommonHelper
{
    public class ArrayHelper
    {
        #region Array Convertion

        /// <summary>
        /// 合并数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arrs">待合并的数组，传递>=2个同类型的数组</param>
        /// <returns>合并后的数组</returns>
        public static T[] MergeArray<T>(params T[][] arrs)
        {
            List<T> result = new List<T>();
            foreach (T[] arr in arrs)
            {
                result.AddRange(arr);
            }
            return result.ToArray();
        }

        public static T[] SubArray<T>(T[] arr, int from, int to)
        {
            if (from <= 0)
                from = 0;
            return arr.Skip(from).Take(to - from).ToArray();
        }

        public static T[] SubArrayByLength<T>(T[] arr, int from, int step)
        {
            if (from <= 0)
                from = 0;
            return arr.Skip(from).Take(step).ToArray();
        }

        #endregion


        internal static Dictionary<string, int> CloneDict(Dictionary<string, int> attributes)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>(attributes.Count);

            foreach (var attr in attributes)
            {
                dict.Add(attr.Key.Clone() as string, attr.Value);
            }

            return dict;
        }

        internal static Dictionary<string, JObject> CloneDict(Dictionary<string, JObject> extensions)
        {
            Dictionary<string, JObject> dict = new Dictionary<string, JObject>(extensions.Count);

            foreach (var attr in extensions)
            {
                dict.Add(attr.Key.Clone() as string, JObject.Parse(attr.Value.ToString()));
            }

            return dict;
        }
    }
}
