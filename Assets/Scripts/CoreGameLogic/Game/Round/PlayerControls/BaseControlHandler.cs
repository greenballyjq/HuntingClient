/// <summary>
/// 控制处理器基类
/// </summary>
public abstract class BaseControlHandler : IControlHandler
{
    /// <summary>
    /// 控制阶段
    /// </summary>
    public ControlHandlerPhase ControlPhase { get; private set; }

    /// <summary>
    /// 玩家武器
    /// </summary>
    protected PlayerWeapon PlayerWeapon;

    protected InputManager InputManager;
    protected CameraManager CameraManager;
    protected WeaponManager WeaponManager;

    public void Init()
    {
        InputManager = GameServiceLocator.GetAppManager<InputManager>();
        CameraManager = GameServiceLocator.GetAppManager<CameraManager>();
        WeaponManager = GameServiceLocator.GetRoundManager<WeaponManager>();

        PlayerWeapon = WeaponManager.PlayerWeapon;

        OnInit();
    }

    public void StartControl()
    {
        ControlPhase = ControlHandlerPhase.Preparing;

        OnControlStart();

        ControlPhase = ControlHandlerPhase.Running;
    }

    public void DoUpdate(float dt)
    {
        if (ControlPhase != ControlHandlerPhase.Running)
            return;

        OnControlUpdate(dt);
    }

    public void EndControl()
    {
        OnControlEnd();

        ControlPhase = ControlHandlerPhase.None;
    }

    /// <summary>
    /// 初始化钩子
    /// </summary>
    protected abstract void OnInit();

    /// <summary>
    /// 控制开始钩子
    /// </summary>
    protected abstract void OnControlStart();

    /// <summary>
    /// 控制更新钩子
    /// </summary>
    /// <param name="dt">时间增量</param>
    protected abstract void OnControlUpdate(float dt);

    /// <summary>
    /// 控制结束钩子
    /// </summary>
    protected abstract void OnControlEnd();
}