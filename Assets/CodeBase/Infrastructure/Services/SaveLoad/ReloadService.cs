namespace CodeBase.Infrastructure.Services.SaveLoad
{
    public class ReloadService: IReloadService
    {
        private readonly GameStateMachine gameStateMachine;

        public ReloadService(GameStateMachine gameStateMachine)
        {
            this.gameStateMachine = gameStateMachine;
        }

        public void Reload()
        {
            gameStateMachine.Enter<BootstrapState>();
        }
    }
}
