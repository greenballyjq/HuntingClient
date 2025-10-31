using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hunting.Data
{
    [CreateAssetMenu(fileName = "_MapData", menuName = "Data/New HuntMap Data", order = 0)]
    public class HuntMapData : ScriptableObject
    {
        public List<HuntMapInfo> huntMapInfoList;

        public enum HuntMapType
        {
            // 草原
            Steppe,

            // 森林
            Forest,

            // 海滩
            Beach,

            // 花园
            Garden,
        }

        [Serializable]
        public class HuntMapInfo
        {
            public HuntMapType mapType;
            public Sprite mapSprite;
            public string mapName;
            [TextArea] public string mapDescription;
        }

        public HuntMapInfo GetMapInfoFromType(HuntMapType type)
        {
            HuntMapInfo result = default;
            foreach (var mapInfo in huntMapInfoList)
                if (mapInfo.mapType == type)
                    result = mapInfo;
            return result;
        }

        public HuntMapInfo GetRandomMapInfo()
        {
            return huntMapInfoList[UnityEngine.Random.Range(0, huntMapInfoList.Count)];
        }
    }
}