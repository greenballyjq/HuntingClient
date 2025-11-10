using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hunting.Data
{
    [CreateAssetMenu(fileName = "_Data", menuName = "Data/New Character Data", order = 0)]
    public class CharacterData : ScriptableObject
    {
        public enum CharacterType
        {
            // 基础角色-男
            Base_Mail,

            // 基础角色-女
            Base_Femail,

            // 亚克东
            YKD,

            // 紫薇
            ZW,

            // 金状元
            JZY,

            // 大美丽
            DML
        }

        [Serializable]
        public class CharacterInfo
        {
            public CharacterType characterType;
            public string characterName;
            public Sprite characterSprite;
            [TextArea] public string characterDescription;
        }

        public List<CharacterInfo> characterInfoList;

        public CharacterInfo GetCharacterInfoFromType(CharacterType characterType)
        {
            CharacterInfo result = default;
            foreach (var characterInfo in characterInfoList)
                if (characterInfo.characterType == characterType)
                    result = characterInfo;
            return result;
        }

        public CharacterInfo GetRandomCharacterInfo()
        {
            return characterInfoList[UnityEngine.Random.Range(0, characterInfoList.Count)];
        }
    }
}