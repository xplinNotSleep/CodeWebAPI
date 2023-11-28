using AGSpatialDataCheck.GISUtils.CheckParams;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGSpatialDataCheck.GISUtils.CheckUtils
{
    public class PropertyCheckUtils:BaseMessageUtils
    {
        /// <summary>
        /// 检查标识字段值是否存在空值
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        public BaseMessage IsUniqueValueExist(CheckField field)
        {
            try
            {
                if (string.IsNullOrEmpty(field.UniqueFieldName.Trim()))
                {
                    return Fail("规则配置异常，缺少必要标识字段");
                }
                //获取某要素类数据中各字段及其对应字段值
                IList<IDictionary<string, object>> layerAttr = LayerHelper.GetAttritubes(field.Layer);
                if (layerAttr == null)
                {
                    return Fail("要素类数据属性表查询出错!");
                }
                if (layerAttr.Count == 0)
                {
                    return Fail("待质检的要素类数据属性表为空!");
                }
                IDictionary<string, object> featureAttr = layerAttr[0];
                //判断核查字段是否在核查的图层中都存在
                StringBuilder errorField = CheckHelper.CheckFieldExist(featureAttr, field.UniqueFieldName);
                if (!string.IsNullOrEmpty(errorField.ToString()))
                {
                    return Fail($"设置的标识字段不存在数据表中");
                }

                bool hasEmpty = CheckHelper.hasEmptyUniqueFieldValue(layerAttr, field.UniqueFieldName);
                //Dictionary<string, List<string>> dicEmptyRecords = CheckHelper.GetEmptyFieldValues(layerAttr, checkFields, field.UniqueFieldName);
                if(hasEmpty)
                {
                    return Fail($"数据表中的标识字段唯一值存在空值！");
                }
                return Success(true);

            }
            catch (Exception e)
            {
                return Fail(e);
            }
        }

        /// <summary>
        /// 检查标识字段值是否唯一
        /// </summary>
        /// <param name="field">核查字段</param>
        /// <returns></returns>
        public BaseMessage IsGuidUnique(CheckField field)
        {
            try
            {
                if (string.IsNullOrEmpty(field.UniqueFieldName.Trim()))
                {
                    return Fail("规则配置异常，缺少必要标识字段");
                }

                //获取某要素类数据中各字段及其对应字段值
                IList<IDictionary<string, object>> layerAttr = LayerHelper.GetAttritubes(field.Layer);
                if (layerAttr == null)
                {
                    return Fail("要素类数据属性表查询出错!");
                }
                if (layerAttr.Count == 0)
                {
                    return Fail("待质检的要素类数据属性表为空!");
                }

                Dictionary<string, int> dicGuid = CheckHelper.GetRepeatGuid(layerAttr, field.UniqueFieldName);
                List<string> RepeatValues = new List<string>();
                foreach (string strValRepeat in dicGuid.Keys)
                {
                    if (dicGuid[strValRepeat]>1)
                    {
                        RepeatValues.Add(strValRepeat);
                    }
                }

                return Success(RepeatValues);

            }
            catch (Exception e)
            {
                return Fail(e);
            }
        }

        /// <summary>
        /// 检查字段值是否唯一
        /// </summary>
        /// <param name="field">核查字段</param>
        /// <returns></returns>
        public BaseMessage IsValueUnique(CheckField field)
        {
            try
            {
                if (string.IsNullOrEmpty(field.UniqueFieldName.Trim()))
                {
                    return Fail("规则配置异常，缺少必要标识字段");
                }
                if (string.IsNullOrEmpty(field.FieldName.Trim()))
                {
                    return Fail("规则配置异常，缺少必要核查字段");
                }

                //获取某要素类数据中各字段及其对应字段值
                IList<IDictionary<string, object>> layerAttr = LayerHelper.GetAttritubes(field.Layer);
                if (layerAttr == null)
                {
                    return Fail("要素类数据属性表查询出错!");
                }
                if (layerAttr.Count == 0)
                {
                    return Fail("待质检的要素类数据属性表为空!");
                }
                
                Dictionary<string, List<string>> dicFieldValues = CheckHelper.GetRepeatFieldValues(layerAttr, field.FieldName, field.UniqueFieldName);
                Dictionary<string, List<string>> dicRepeatValues = new Dictionary<string, List<string>>();
                foreach (string strValRepeat in dicFieldValues.Keys)
                {
                    if (dicFieldValues[strValRepeat].Count != 1)
                    {
                        dicRepeatValues.Add(strValRepeat, dicFieldValues[strValRepeat]);
                    }
                }

                return Success(dicRepeatValues);

            }
            catch(Exception e)
            {
                return Fail(e);
            }
        }

        /// <summary>
        /// 检查字段值是否为空
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        public BaseMessage IsValueExist(CheckField field)
        {
            try
            {
                if (string.IsNullOrEmpty(field.UniqueFieldName.Trim()))
                {
                    return Fail("规则配置异常，缺少必要标识字段");
                }
                //获取核查字段
                GetCheckFields(field, out string[] backFields, out List<string> checkFields);
                if (checkFields.Count==0 || backFields.Length==0)
                {
                    return Fail("未设置核查字段");
                }
                if(field.Layer.LayerType=="sde")
                {
                    field.Layer.BackFieldNames+=$",{field.UniqueFieldName}";
                }
                //获取某要素类数据中各字段及其对应字段值
                IList<IDictionary<string, object>> layerAttr = LayerHelper.GetAttritubes(field.Layer);
                if (layerAttr == null)
                {
                    return Fail("要素类数据属性表查询出错!");
                }
                if (layerAttr.Count == 0)
                {
                    return Fail("待质检的要素类数据属性表为空!");
                }
                IDictionary<string, object> featureAttr = layerAttr[0];
                //判断核查字段是否在核查的图层中都存在
                StringBuilder errorFields = CheckHelper.CheckFieldsExist(featureAttr, checkFields);
                if (!string.IsNullOrEmpty(errorFields.ToString()))
                {
                    return Fail($"{errorFields}字段不存在数据表中");
                }
                Dictionary<string, List<string>> dicEmptyRecords = CheckHelper.GetEmptyFieldValues(layerAttr, checkFields, field.UniqueFieldName);

                return Success(dicEmptyRecords);

            }
            catch (Exception e)
            {
                return Fail(e);
            }
        }

        /// <summary>
        /// 检查字段值是否不为空（字段值必须为空，不为空则为错误）
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        public BaseMessage IsValueNotExist(CheckField field)
        {
            try
            {
                if (string.IsNullOrEmpty(field.UniqueFieldName.Trim()))
                {
                    return Fail("规则配置异常，缺少必要标识字段");
                }
                //获取核查字段
                GetCheckFields(field, out string[] backFields, out List<string> checkFields);
                if (checkFields.Count == 0 || backFields.Length == 0)
                {
                    return Fail("未设置核查字段");
                }
                if (field.Layer.LayerType == "sde")
                {
                    field.Layer.BackFieldNames += $",{field.UniqueFieldName}";
                }
                //获取某要素类数据中各字段及其对应字段值
                IList<IDictionary<string, object>> layerAttr = LayerHelper.GetAttritubes(field.Layer);
                if (layerAttr == null)
                {
                    return Fail("要素类数据属性表查询出错!");
                }
                if (layerAttr.Count == 0)
                {
                    return Fail("待质检的要素类数据属性表为空!");
                }
                IDictionary<string, object> featureAttr = layerAttr[0];
                //判断核查字段是否在核查的图层中都存在
                StringBuilder errorFields = CheckHelper.CheckFieldExist(featureAttr, field.FieldName);
                if (!string.IsNullOrEmpty(errorFields.ToString()))
                {
                    return Fail($"{errorFields}字段不存在数据表中");
                }
                //获取核查字段中属性值不为空的记录
                Dictionary<string, List<string>> dicNotEmptyRecords = CheckHelper.GetNotEmptyFieldValues(layerAttr, checkFields, field.UniqueFieldName);

                return Success(dicNotEmptyRecords);

            }
            catch (Exception e)
            {
                return Fail(e);
            }
        }

        /// <summary>
        /// 检查属性值是否符合规则
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        public BaseMessage IsEffectiveCoding(CheckField field)
        {
            if (string.IsNullOrEmpty(field.UniqueFieldName.Trim()))
            {
                return Fail("规则配置异常，缺少必要标识字段");
            }

            if (string.IsNullOrEmpty(field.FieldName) || string.IsNullOrEmpty(field.FieldName2))
            {
                return Fail("规则配置异常，缺少必要参数字段");
            }

            //获取某要素类数据中各字段及其对应字段值
            IList<IDictionary<string, object>> layerAttr = LayerHelper.GetAttritubes(field.Layer);
            if (layerAttr == null)
            {
                return Fail("要素类数据属性表查询出错!");
            }
            if (layerAttr.Count == 0)
            {
                return Fail("待质检的要素类数据属性表为空!");
            }

            IDictionary<string, object> featureAttr = layerAttr[0];
            StringBuilder errorFields = CheckHelper.CheckFieldsExist(featureAttr, new List<string>() {field.FieldName,
            field.FieldName2});
            if (!string.IsNullOrEmpty(errorFields.ToString()))
            {
                return Fail($"{errorFields}字段不存在数据表中");
            }

            Dictionary<string, string> dicOutCoding = new Dictionary<string, string>();
            foreach (IDictionary<string, object> dicAttr in layerAttr)
            {
                if (string.IsNullOrEmpty(dicAttr[field.FieldName].ToString())|| string.IsNullOrEmpty(dicAttr[field.FieldName2].ToString()))
                {
                    continue;
                }
                string fieldValue = dicAttr[field.FieldName].ToString();
                string codeXZQ = dicAttr[field.FieldName2].ToString();
                string guid = dicAttr[field.UniqueFieldName].ToString();
                //string feaId = dicAttr[CheckHelper.PK_NAME].ToString();

                if (!CheckHelper.CheckBeginWithXZQ(fieldValue, codeXZQ))
                {
                   dicOutCoding.Add(guid, fieldValue);
                }
            }
            return Success(dicOutCoding);
        }

        /// <summary>
        /// 检查数值型属性值是否在区间范围内
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        public BaseMessage IsNumValueInRange(CheckField field)
        {
            if (string.IsNullOrEmpty(field.UniqueFieldName.Trim()))
            {
                return Fail("规则配置异常，缺少必要标识字段");
            }

            if (string.IsNullOrEmpty(field.FieldName))
            {
                return Fail("规则配置异常，缺少必要参数字段");
            }

            //获取某要素类数据中各字段及其对应字段值
            IList<IDictionary<string, object>> layerAttr = LayerHelper.GetAttritubes(field.Layer);
            if (layerAttr == null)
            {
                return Fail("要素类数据属性表查询出错!");
            }
            if (layerAttr.Count == 0)
            {
                return Fail("待质检的要素类数据属性表为空!");
            }

            IDictionary<string, object> featureAttr = layerAttr[0];
            StringBuilder errorField = CheckHelper.CheckFieldExist(featureAttr, field.FieldName);
            if (!string.IsNullOrEmpty(errorField.ToString()))
            {
                return Fail($"{errorField}字段不存在数据表中");
            }
            //if (!double.TryParse(field.MinValue, out double minSize) || !double.TryParse(field.param2, out double maxSize))
            //{
            //    Toolset.TinyLogger.WriteLog("范围参数配置异常");
            //    return Error("范围参数配置异常");
            //}
            Dictionary<string, double> errors = new Dictionary<string, double>();
            foreach (IDictionary<string, object> dicAttr in layerAttr)
            {
                //string feaId = dicAttr[CheckHelper.PK_NAME].ToString();
                string guid = dicAttr[field.UniqueFieldName].ToString();
                if (dicAttr[field.FieldName] == null)
                {
                    continue;
                }
                string fieldValue = dicAttr[field.FieldName].ToString();
                if (string.IsNullOrEmpty(fieldValue))
                {
                    continue;
                }

                if (double.TryParse(fieldValue, out double size))
                {
                    if (CheckHelper.IsErrorData(field.MinValue, field.MaxValue, "0", size))
                    {
                        errors.Add(guid, size);
                    }
                }
                else
                {
                    return Fail("所核查字段非数值类型字段!");
                }
            }
            return Success(errors);
        }

        /// <summary>
        /// 检查字符型属性值是否在区间范围内
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        public BaseMessage IsStrValueInRange(CheckField field)
        {
            if (string.IsNullOrEmpty(field.UniqueFieldName))
            {
                return Fail("规则配置异常，缺少必要标识字段");
            }

            if (string.IsNullOrEmpty(field.FieldName))
            {
                return Fail("规则配置异常，缺少必要参数字段");
            }

            //获取某要素类数据中各字段及其对应字段值
            IList<IDictionary<string, object>> layerAttr = LayerHelper.GetAttritubes(field.Layer);
            if (layerAttr == null)
            {
                return Fail("要素类数据属性表查询出错!");
            }
            if (layerAttr.Count == 0)
            {
                return Fail("待质检的要素类数据属性表为空!");
            }

            IDictionary<string, object> featureAttr = layerAttr[0];
            StringBuilder errorField = CheckHelper.CheckFieldExist(featureAttr, field.FieldName);
            if (!string.IsNullOrEmpty(errorField.ToString()))
            {
                return Fail($"{errorField}字段不存在数据表中");
            }

            if(string.IsNullOrEmpty(field.Values) || field.Values.Split(',').Length==0)
            {
                return Fail($"未设置核查字段的取值范围!");
            }
            string[] range = field.Values.Split(',');
            Dictionary<string, string> errors = new Dictionary<string, string>();
            foreach (IDictionary<string, object> dicAttr in layerAttr)
            {
                //string feaId = dicAttr[CheckHelper.PK_NAME].ToString();
                string guid = dicAttr[field.UniqueFieldName].ToString();
                if (dicAttr[field.FieldName] == null)
                {
                    continue;
                }
                string content = dicAttr[field.FieldName].ToString();
                if (string.IsNullOrEmpty(content))
                {
                    continue;
                }

                if (CheckHelper.IsErrorValue(content, range))
                {
                    errors.Add(guid, content);
                }
            }
            return Success(errors);
        }

        /// <summary>
        /// 数据库表中字符型日期字段(年月日)是否为合法的日期（如2019-01-01)
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        public BaseMessage IsValidDate(CheckField field)
        {
            if (string.IsNullOrEmpty(field.UniqueFieldName))
            {
                return Fail("规则配置异常，缺少必要标识字段");
            }

            GetCheckFields(field, out string[] backFields, out List<string> checkFields);
            if (checkFields.Count == 0 || backFields.Length == 0)
            {
                return Fail("未设置核查字段");
            }
            if (field.Layer.LayerType == "sde")
            {
                field.Layer.BackFieldNames += $",{field.UniqueFieldName}";
            }
            IList<IDictionary<string, object>> layerAttr = LayerHelper.GetAttritubes(field.Layer);
            if (layerAttr == null)
            {
                return Fail("要素类数据属性表查询出错!");
            }
            if (layerAttr.Count == 0)
            {
                return Fail("待质检的要素类数据属性表为空!");
            }

            IDictionary<string, object> featureAttr = layerAttr[0];
            StringBuilder errorFields = CheckHelper.CheckFieldsExist(featureAttr, checkFields);
            if (!string.IsNullOrEmpty(errorFields.ToString()))
            {
                return Fail($"{errorFields}字段不存在数据表中");
            }
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
            foreach (IDictionary<string, object> dicAttr in layerAttr)
            {
                string guid = dicAttr[field.UniqueFieldName].ToString();

                foreach(string checkField in checkFields)
                {
                    string fieldValue = dicAttr[checkField].ToString();
                    if (string.IsNullOrEmpty(fieldValue))
                    {
                        continue;
                    }
                    var b = DateTime.TryParse(fieldValue, out DateTime resultDateTime);
                    if (!b)
                    {
                        if(!errors.ContainsKey(checkField))
                        {
                            errors.Add(checkField, new List<string>() { guid });
                        }
                        else
                        {
                            errors[checkField].Add(guid);
                        }
                    }
                }
            }
            return Success(errors);
        }

        /// <summary>
        /// 是否为合法的年份（如2019),暂无年份范围
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        public BaseMessage IsValidYear(CheckField field)
        {
            return null;
        }

        /// <summary>
        /// 与其他日期比较后，判断逻辑上是否为合法的年份（如2019),可为逗号分隔的多个年份
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        public BaseMessage IsValidYearCompared(CheckField field)
        {
            return null;
        }

        /// <summary>
        /// 字符型年月是否为合法的年月
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        public BaseMessage IsValidYearMonth(CheckField field)
        {
            return null;
        }

        /// <summary>
        /// 日期(年月日)字段值与其他日期比较后，逻辑上是否为合法的日期（如桥梁建成日期不能超过调查时间),还未完全实现
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        public BaseMessage IsValidDateCompared(CheckField field)
        {
            return null;
        }

        /// <summary>
        /// 检查身份证是否有效，返回不合法的身份证记录信息
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        public BaseMessage IsEffectiveIDCard(CheckField field)
        {
            return null;
        }

        /// <summary>
        /// 检查是否为有效的字段值,返回的是与所给值（这些值实际是不合符要求的）相同的记录信息
        /// </summary>
        /// <param name="field">图层属性</param>
        /// <returns></returns>
        public BaseMessage IsEffectiveValue(CheckField field)
        {
            return null;
        }

        /// <summary>
        /// 获取质检图层需要质检的字段
        /// </summary>
        /// <param name="field"></param>
        /// <param name="backFields"></param>
        /// <param name="checkFields"></param>
        /// <param name="selectFields"></param>
        /// <param name="whereClause"></param>
        /// <param name="fieldName"></param>
        private void GetCheckFields(CheckField field, out string[] backFields, out List<string> checkFields)
        {
            backFields = field.Layer.BackFieldNames.Split(CheckHelper.BACK_FIELD_SPLIT);
            checkFields = new List<string>(backFields);

        }
    }
}
