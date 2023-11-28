using AGSpatialDataCheck.GISUtils.CheckParams;

namespace AGSpatialDataCheck.GISUtils.CheckUtils
{
    public class LineToLineCheckUtils
    {
        /// <summary>
        /// 线要素类与其他线要素类中的要素相交检查
        /// </summary>
        /// <param name="multiLayer">两个图层,第一个为关注图层</param>
        /// <returns>相交的要素</returns>
        public BaseMessage IsIntersected(MultiSdeLayer multiLayer)
        {
            return null;
        }

        /// <summary>
        /// 线要素是否被线要素覆盖
        /// </summary>
        /// <param name="multiLayer">两个图层,第一个为关注图层</param>
        /// <returns>相交的要素</returns>
        public BaseMessage IsCovered(MultiSdeLayer multiLayer)
        {
            return null;
        }
    }
}
