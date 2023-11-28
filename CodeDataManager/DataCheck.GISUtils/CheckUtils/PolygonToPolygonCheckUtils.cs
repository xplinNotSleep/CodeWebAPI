using AGSpatialDataCheck.GISUtils.CheckParams;

namespace AGSpatialDataCheck.GISUtils.CheckUtils
{
    public class PolygonToPolygonCheckUtils
    {
        /// <summary>
        /// 判断两个面要素类中是否有相互重叠的要素（包括包含与被包含，不包括接边）
        /// </summary>
        /// <param name="multiLayer">两个图层,第一个为关注图层</param>
        /// <returns>相交的要素</returns>
        public BaseMessage IsOverlap(MultiSdeLayer multiLayer)
        {
            return null;
        }

        /// <summary>
        /// 判断两个面要素类是否具有空隙
        /// </summary>
        /// <param name="multiLayer">两个图层,第一个为关注图层</param>
        /// <returns>相交的要素</returns>
        public BaseMessage HasGap(MultiSdeLayer multiLayer)
        {
            return null;
        }

        /// <summary>
        /// 判断两个面要素类是否是相互覆盖的要素
        /// </summary>
        /// <param name="multiLayer">两个图层,第一个为关注图层</param>
        /// <returns>相交的要素</returns>
        public BaseMessage IsCovered(MultiSdeLayer multiLayer)
        {
            return null;
        }

        /// <summary>
        /// 多个面合并后是否与某个面重合
        /// </summary>
        /// <param name="multiLayer">两个图层,第一个为关注图层</param>
        /// <returns>相交的要素</returns>
        public BaseMessage IsCoincided(MultiSdeLayer multiLayer)
        {
            return null;
        }
    }
}
