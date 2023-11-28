using NetTopologySuite.Mathematics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace AGSpatialDataCheck.GISUtils
{
    public static class CheckHelper
    {
        /// <summary>
        /// 返回字段字符串分隔符
        /// </summary>
        public static readonly char BACK_FIELD_SPLIT = ',';
        /// <summary>
        /// 图层检查主键
        /// </summary>
        public static readonly string PK_NAME = "objectid";

        /// <summary>
        /// 获取需要检查的字段
        /// </summary>
        /// <param name="fieldNames"></param>
        /// <param name="otherFields"></param>
        /// <returns></returns>
        public static List<string> GetCheckFields(string[] fieldNames, params string[] otherFields)
        {
            List<string> checkFields = new List<string>(fieldNames);
            foreach (var field in otherFields)
            {
                if (!checkFields.Contains(field))
                {
                    checkFields.Add(field);
                }
            }

            return checkFields;
        }

        /// <summary>
        /// 核实待质检字段是否存在于指定属性表中
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="checkFields"></param>
        /// <returns></returns>
        public static StringBuilder CheckFieldExist(DataTable dt, List<string> checkFields)
        {
            StringBuilder errorFields = new StringBuilder();
            foreach (var fieldName in checkFields)
            {
                if (!string.IsNullOrEmpty(fieldName.Trim()) && !dt.Columns.Contains(fieldName))
                {
                    errorFields.Append($"{fieldName},");
                }
            }
            return errorFields.TrimEnd(',');
        }

        /// <summary>
        /// 核实待质检多个字段是否存在于指定属性表中
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="checkFields"></param>
        /// <returns></returns>
        public static StringBuilder CheckFieldsExist(IDictionary<string, object> fieldAttr, List<string> checkFields)
        {
            StringBuilder errorFields = new StringBuilder();
            foreach (var fieldName in checkFields)
            {
                if (!string.IsNullOrEmpty(fieldName.Trim()) && !fieldAttr.ContainsKey(fieldName))
                {
                    errorFields.Append($"{fieldName},");
                }
            }
            return errorFields.TrimEnd(',');
        }

        /// <summary>
        /// 核实待质检某个字段是否存在于指定属性表中
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="checkFields"></param>
        /// <returns></returns>
        public static StringBuilder CheckFieldExist(IDictionary<string, object> fieldAttr, string checkField)
        {
            StringBuilder errorFields = new StringBuilder();
            if (!string.IsNullOrEmpty(checkField.Trim()) && !fieldAttr.ContainsKey(checkField))
            {
                errorFields.Append($"{checkField}");
            }
            return errorFields;
        }

        /// <summary>
        /// 根据字段值构造字典
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static Dictionary<string, List<DataRow>> GetFieldValueDic(DataTable dt, string fieldName)
        {
            Dictionary<string, List<DataRow>> pDictionary = new Dictionary<string, List<DataRow>>();
            foreach (DataRow row in dt.Rows)
            {
                string fieldValue = row[fieldName].ToString();
                if (string.IsNullOrEmpty(fieldValue))
                {
                    continue;
                }
                if (pDictionary.ContainsKey(fieldValue))
                {
                    pDictionary[fieldValue].Add(row);
                }
                else
                {
                    pDictionary.Add(fieldValue, new List<DataRow>() { row });
                }
            }

            return pDictionary;
        }

        /// <summary>
        /// 获取每个唯一值对应的记录
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool hasEmptyUniqueFieldValue(IList<IDictionary<string, object>> layerAttr,
            string uniqueFieldName)
        {
            foreach (IDictionary<string, object> featureAttr in layerAttr)
            {
                string fieldValue = featureAttr[uniqueFieldName].ToString();
                if (string.IsNullOrEmpty(fieldValue))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取每个唯一值对应的记录
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static Dictionary<string, int> GetRepeatGuid(IList<IDictionary<string, object>> layerAttr,
            string UniqueFieldName)
        {
            Dictionary<string, int> dicGuid = new Dictionary<string, int>();
            foreach (IDictionary<string, object> featureAttr in layerAttr)
            {
                string fieldGuid = featureAttr[UniqueFieldName].ToString();
              
                if (!dicGuid.ContainsKey(fieldGuid))
                {
                    dicGuid.Add(fieldGuid, 1);
                }
                else
                {
                    dicGuid[fieldGuid]++;
                }
            }

            return dicGuid;
        }

        /// <summary>
        /// 获取每个唯一值对应的记录
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> GetRepeatFieldValues(IList<IDictionary<string, object>> layerAttr, 
            string fieldName, string uniqueFieldName)
        {
            Dictionary<string, List<string>> dicRepeats = new Dictionary<string, List<string>>();
            foreach (IDictionary<string, object> featureAttr in layerAttr)
            {
                string guid = featureAttr[uniqueFieldName].ToString();
                string fieldValue = featureAttr[fieldName].ToString();
                if (string.IsNullOrEmpty(fieldValue))
                {
                    continue;
                }
                if (dicRepeats.ContainsKey(fieldValue))
                {
                    dicRepeats[fieldValue].Add(guid);
                }
                else
                {
                    dicRepeats.Add(fieldValue, new List<string>() { guid });
                }
            }

            return dicRepeats;
        }

        /// <summary>
        /// 获取数据中属性存在空值的字段以及相应的记录
        /// </summary>
        /// <param name="layerAttr"></param>
        /// <param name="checkFields"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> GetEmptyFieldValues(IList<IDictionary<string, object>> layerAttr,
            List<string> checkFields, string UniqueFieldName)
        {
            Dictionary<string, List<string>> dicEmptyField = new Dictionary<string, List<string>>();
            
            foreach(IDictionary<string, object> feaAttr in layerAttr)
            {
                string guid = feaAttr[UniqueFieldName].ToString();
                foreach(string checkField in checkFields)
                {
                    //string fieldValue = feaAttr[checkField].ToString();
                    if (feaAttr[checkField]==null || feaAttr[checkField].ToString().Trim()=="")
                    {
                        if (!dicEmptyField.ContainsKey(checkField)) dicEmptyField.Add(checkField, new List<string>() { guid });
                        else
                        {
                            dicEmptyField[checkField].Add(guid);
                        }
                    }
                }

            }

            return dicEmptyField;


        }

        /// <summary>
        /// 获取数据中属性存在非空值的字段以及相应的记录
        /// </summary>
        /// <param name="layerAttr"></param>
        /// <param name="checkFields"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> GetNotEmptyFieldValues(IList<IDictionary<string, object>> layerAttr,
            List<string> checkFields, string UniqueFieldName)
        {
            Dictionary<string, List<string>> dicEmptyField = new Dictionary<string, List<string>>();

            foreach (IDictionary<string, object> feaAttr in layerAttr)
            {
                //string feaId = feaAttr[CheckHelper.PK_NAME].ToString();
                string guid = feaAttr[UniqueFieldName].ToString();
                foreach (string checkField in checkFields)
                {
                    string fieldValue = feaAttr[checkField].ToString();
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        if (!dicEmptyField.ContainsKey(checkField)) dicEmptyField.Add(checkField, new List<string>() { guid });
                        else
                        {
                            dicEmptyField[checkField].Add(guid);
                        }
                    }
                }

            }

            return dicEmptyField;


        }

        /// <summary>
        /// 获取数值区间范围检查错误记录(用flag来判定是否将最大或最小值算进去)
        /// 
        public static bool IsErrorData(double minSize, double maxSize, string flag, double length)
        {
            if (string.IsNullOrEmpty(flag))
            {
                if (length >= maxSize || length <= minSize)
                {
                    return true;
                }
            }
            else if (flag == "-1")
            {
                if (length >= maxSize || length < minSize)
                {
                    return true;
                }
            }
            else if (flag == "1")
            {
                if (length > maxSize || length <= minSize)
                {
                    return true;
                }
            }
            else
            {
                if (length > maxSize || length < minSize)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取字符型区间范围内检查超限记录
        /// </summary>
        /// <param name="checkValue"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool IsErrorValue(string checkValue, string[] range)
        {
            List<string> rangeValues = new List<string>(range);
            if(!rangeValues.Contains(checkValue))
            {
                return true;
            }
            return false;

        }

        /// <summary>
        /// 检查编码是否以行政区编码开始的12位编码
        /// </summary>
        /// <param name="value"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool CheckBeginWithXZQ(string value, string code)
        {
            return Regex.IsMatch(value, "^" + code + @"\d{6}");
        }

        /// <summary>
        /// 计算两点之间的空间距离
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="z0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="z1"></param>
        /// <returns></returns>
        public static double Calculate(double x0, double y0, double? z0, double x1, double y1, double? z1)
        {
            double dSquareSum = 0;
            bool bHasZ = z0.HasValue && z1.HasValue;
            dSquareSum = Math.Pow(x0 - x1, 2) + Math.Pow(y0 - y1, 2);
            dSquareSum += bHasZ ? Math.Pow(z0.Value - z1.Value, 2) : 0;
            return Math.Sqrt(dSquareSum);
        }

        //判断点是否在直线上 
        public static bool GetPointIsInLine(Vector2D pf, Vector2D p1, Vector2D p2, double range)
        {

            //range 判断的的允许误差，不需要误差则赋值0
            double dll = Calculate(pf.X, pf.Y, 0, p1.X, p1.Y, 0);
            double dl2 = Calculate(pf.X, pf.Y, 0, p2.X, p2.Y, 0);
            if (dll <= range || dl2 <= range)
                return true;
            //点在线段首尾两端之外则return false
            double cross = (p2.X - p1.X) * (pf.X - p1.X) + (p2.Y - p1.Y) * (pf.Y - p1.Y);
            if (cross <= 0) return false;
            double d2 = (p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y);
            if (cross >= d2) return false;

            double r = cross / d2;
            double px = p1.X + (p2.X - p1.X) * r;
            double py = p1.Y + (p2.Y - p1.Y) * r;

            //判断距离是否小于误差
            return Math.Sqrt((pf.X - px) * (pf.X - px) + (py - pf.Y) * (py - pf.Y)) <= range;
        }


        #region 扩展方法

        public static StringBuilder TrimEnd(this StringBuilder builder, char trimChar)
        {
            int length = builder.Length;
            for (int i = builder.Length - 1; i >= 0; i--)
            {
                if (builder[i] == trimChar)
                {
                    builder = builder.Remove(i, 1);
                }
                else
                {
                    break;
                }
            }

            return builder;
        }

        #endregion

    }
}
