using UnityEngine;

namespace Hunting.Manager
{
    /// <summary>
    /// 游戏业务管理器基类
    /// </summary>
    public abstract class BaseGameManager : MonoBehaviour, IGameManager
    {
        public abstract void Init();
        public abstract void Update();
        public abstract void Release();
    }
}