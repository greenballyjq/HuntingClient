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
        /// 当前临时状态是否可以被新临时状态打断
        /// </summary>
        private bool _canCurrentTempBeInterrupted;

        /// <summary>
        /// 初始化状态机
        /// </summary>
        /// <param name="initState">初始状态</param>
        public void Init(IState initState)
        {
            _stateBeforeTemp = null;
            _canCurrentTempBeInterrupted = true;
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
            _canCurrentTempBeInterrupted = true;

            _currentState?.Exit();
            _currentState = nextState;
            _currentState?.Enter();
        }

        /// <summary>
        /// 进入临时状态
        /// </summary>
        /// <param name="tempState">要进入的临时状态</param>
        /// <param name="ignoreSameState">是否忽略相同状态</param>
        /// <param name="canBeInterrupted">该临时状态是否可以被其他临时状态打断</param>
        public void EnterTempState(IState tempState, bool ignoreSameState = false, bool canBeInterrupted = true)
        {
            if (!ignoreSameState && _currentState == tempState)
                return;

            bool isSameState = _currentState == tempState;
            bool isEnteringSelf = _stateBeforeTemp != null && isSameState;

            if (_stateBeforeTemp != null && !_canCurrentTempBeInterrupted && !isEnteringSelf)
                return;

            if (_stateBeforeTemp == null)
                _stateBeforeTemp = _currentState;

            _currentState?.Pause();
            _currentState = tempState;
            _canCurrentTempBeInterrupted = canBeInterrupted;
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
            _canCurrentTempBeInterrupted = true;

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
