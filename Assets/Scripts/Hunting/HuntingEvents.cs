using System.Collections;
using System.Collections.Generic;
using GameFramework.Core;
using Hunting.Game.Bullet;
using UnityEngine;

namespace Hunting
{
    /// <summary>
    /// ������Ϸ�¼������й�����
    /// </summary>
    public static class HuntingEvents
    {
        #region ��Ϸ�߼����¼�
        public static readonly EventKey GameStarted = new EventKey();
        public static readonly EventKey GamePaused = new EventKey();
        public static readonly EventKey GameResumed = new EventKey();
        public static readonly EventKey GameEnded = new EventKey();

        public static readonly EventKey HuntingGameStarted = new EventKey();
        public static readonly EventKey HuntingGameEnded = new EventKey();
        #endregion
    }
}

