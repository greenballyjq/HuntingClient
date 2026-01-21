
///// <summary>
///// 状态机
///// </summary>
//public class StateMachine
//{
//    /// <summary>
//    /// 当前状态
//    /// </summary>
//    public IState CurrentState { get; private set; }

//    /// <summary>
//    /// 初始化状态机
//    /// </summary>
//    /// <param name="startState"></param>
//    public void Init(IState startState)
//    {
//        CurrentState = startState;
//        startState.Enter();
//    }

//    /// <summary>
//    /// 切换状态
//    /// </summary>
//    /// <param name="newState"></param>
//    public void ChangeState(IState newState)
//    {
//        CurrentState.Exit();
//        CurrentState = newState;
//        CurrentState.Enter();
//    }

//    /// <summary>
//    /// 每帧更新
//    /// </summary>
//    public void DoUpdate()
//    {
//        CurrentState?.Tick();
//    }
//}
