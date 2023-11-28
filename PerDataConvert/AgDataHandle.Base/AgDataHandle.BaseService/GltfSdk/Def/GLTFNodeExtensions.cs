using System.Text.RegularExpressions;

namespace AgDataHandle.BaseService.GltfSdk.Def
{
    /// <summary>
    /// 反序列化/序列化 通过（通过 GLTFNodeConvertor 实现）2020年8月8日
    /// 
    /// 本来专门处理glTF节点(node）的属性
    /// </summary>
    partial class GLTFNode
    {
        private string m_catagory = null;
        private string m_catagoryWithoutNormalModel = null;

        /// <summary>
        /// 获得其中的ObjectId,这个方法会自动从属性中读取。
        /// </summary>
        public string ProperityOfCatagory
        {
            get
            {
                if (m_catagory != null)
                    return m_catagory;
                if (this.Extensions.ContainsKey("catagory"))
                {
                    m_catagory = this.Extensions.GetGLTFValue("catagory");
                }
                if (m_catagory == null)
                {
                    m_catagory = "";
                }
                return m_catagory;
            }
        }
        /// <summary>
        /// 如果Catagory是常规模型，则使用Family
        /// </summary>
        public string ProperityOfCatagoryWithoutNormalModel
        {
            get
            {
                if (this.Extensions == null)
                {
                    return null;
                }
                if (m_catagoryWithoutNormalModel != null)
                    return m_catagoryWithoutNormalModel;
                m_catagoryWithoutNormalModel = ProperityOfCatagory;
                if (m_catagoryWithoutNormalModel == "常规模型")
                {
                    if (this.Extensions.ContainsKey("familytype"))
                    {
                        var familyname = this.Extensions.GetGLTFValue("familytype");
                        if(familyname != null)
                        {
                            m_catagoryWithoutNormalModel = familyname;
                        }
                    }
                }
                m_catagoryWithoutNormalModel = RemoveIrregularChar(m_catagoryWithoutNormalModel);


                return m_catagoryWithoutNormalModel;
            }
        }
        /// <summary>
        /// 通过正则方法，规范字符串
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        private string RemoveIrregularChar(string Str)
        {
            if (Str.IsNotNullOrEmpty())
                Str = Regex.Replace(Str, "[ \\[ \\] \\^ \\-_*×――(^)|'$%~!@#$…&%￥—+=<>《》!！??？:：•`·、。，；,.;\"‘’“”-]", "");
            return Str;
        }
    }
}
