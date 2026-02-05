using GameFramework.Core;

namespace Hunting.Events
{
    /// <summary>
    /// 隐藏地图事件
    /// </summary>
    public static class HiddenMapEvents
    {
        /// <summary>
        /// 隐藏地图进入事件
        /// </summary>
        public static readonly EventKey HiddenMapEntered = new EventKey();
        
        /// <summary>
        /// 隐藏地图开始事件
        /// </summary>
        public static readonly EventKey HiddenMapPlayStart = new EventKey();
    }
}