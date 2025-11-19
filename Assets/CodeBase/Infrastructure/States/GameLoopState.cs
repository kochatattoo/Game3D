namespace CodeBase.Infrastructure
{
    internal class GameLoopState : IState
    {
        private GameStateMachine _stateMachine;

        public GameLoopState(GameStateMachine gameStateMachine)
        {
            _stateMachine = gameStateMachine;
        }

        public void Enter()
        {
            
        }

        public void Exit()
        {
            
        }
    }
}