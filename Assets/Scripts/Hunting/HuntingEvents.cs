using System.Collections;
using System.Collections.Generic;
using GameFramework.Core;
using Hunting.Game.Bullet;
using UnityEngine;

namespace Hunting
{
    /// <summary>
    /// 打猎游戏事件键集中管理类
    /// </summary>
    public static class HuntingEvents
    {
        #region Hunting游戏类事件
        public static readonly EventKey HuntingGameStarted = new EventKey();
        public static readonly EventKey HuntingGameEnded = new EventKey();
        #endregion
    }
}

