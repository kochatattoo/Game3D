using CodeBase.Infrastructure.Services.SaveLoad;
using CodeBase.Data;
using CodeBase.Infrastructure.Services.PersistentProgress;

namespace CodeBase.Infrastructure
{
    public class LoadProgressState : IState
    {
        private readonly GameStateMachine _gameStateMachine;
        private readonly IPersistentProgressService _progressService;
        private readonly ISaveLoadService _saveLoadService;

        public LoadProgressState(GameStateMachine gameStateMachine, IPersistentProgressService persistentProgressService, ISaveLoadService saveLoadService)
        {
            _gameStateMachine = gameStateMachine;
            _progressService = persistentProgressService;
            _saveLoadService = saveLoadService;
        }

        public void Enter()
        {
            LoadProgressOrInitNew();

            _gameStateMachine.Enter<LoadSceneState, string>(_progressService.Progress.WorldData.PositionOnLevel.Level);
        }

        public void Exit()
        {

        }
        private void LoadProgressOrInitNew() =>
            _progressService.Progress =
                _saveLoadService.LoadProgress()
                ?? NewProgress();

        private PlayerProgress NewProgress()
        {
            var progress = new PlayerProgress();
            progress.HeroState.MaxHP = 50;
            progress.HeroStats.Damage = 1;
            progress.HeroStats.DamageRadius = 0.5f;
            progress.HeroState.ResetHP();

            return progress; 
        }
    }
}