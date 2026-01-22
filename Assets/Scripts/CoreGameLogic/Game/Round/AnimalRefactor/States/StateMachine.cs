namespace Hunting.Game.Animal
{
    public class StateMachine
    {
        /// <summary>
        /// 临时状态的上一状态
        /// </summary>
        private IState _stateBeforeTemp;

        /// <summary>
        /// 当前状态
        /// </summary>
        private IState _currentState;
        public IState CurrentState => _currentState;

        /// <summary>
        /// 初始化状态机
        /// </summary>
        /// <param name="initState">初始状态</param>
        public void Init(IState initState)
        {
            _stateBeforeTemp = null;
            _currentState = initState;
            initState?.Enter();
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="nextState">下一状态</param>
        public void ChangeState(IState nextState)
        {
            if (_currentState == nextState) return;

            _stateBeforeTemp = null;

            _currentState?.Exit();
            _currentState = nextState;
            _currentState?.Enter();
        }

        /// <summary>
        /// 进入临时状态
        /// </summary>
        /// <param name="tempState">临时状态</param>
        public void EnterTempState(IState tempState)
        {
            if (_currentState == tempState)
                return;

            if (_stateBeforeTemp == null)
                _stateBeforeTemp = _currentState;

            _currentState?.Pause(); 
            _currentState = tempState;
            _currentState?.Enter();
        }

        /// <summary>
        /// 退出临时状态
        /// </summary>
        public void ExitTempState()
        {
            if (_stateBeforeTemp == null) return;

            _currentState?.Exit();
            _currentState = _stateBeforeTemp;
            _stateBeforeTemp = null;
            
            _currentState?.Resume(); 
        }

        /// <summary>
        /// 每帧更新
        /// </summary>
        /// <param name="dt">时间增量</param>
        public void DoUpdate(float dt)
        {
            _currentState?.DoUpdate(dt);
        }
    }
}
