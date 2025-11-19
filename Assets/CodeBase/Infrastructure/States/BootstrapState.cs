using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Assetmanagement;
using CodeBase.Services.Input;
using UnityEngine;
using CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.Infrastructure.Services.SaveLoad;
using CodeBase.Infrastructure.Services.StaticData;

namespace CodeBase.Infrastructure
{
    /// <summary>
    /// Класс сосотяния бутстрапера
    /// </summary>
    public class BootstrapState : IState
    {
        private const string Initial = "Initial";
        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly AllServices _services;

        public BootstrapState(GameStateMachine stateMachine, SceneLoader sceneLoader, AllServices services)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _services = services;

            RegisterServices();
        }

        public void Enter()
        {
            _sceneLoader.Load(Initial, EnterLoadLevel);
        }
        public void Exit()
        {

        }

        private void EnterLoadLevel() =>
            _stateMachine.Enter<LoadProgressState>();
    
        private void RegisterServices()
        {
            RegisterStaticData();
            IRandomService randomService = new UnityRandomService();

            _services.RegisterSingle<IInputService>(InputService());
            _services.RegisterSingle<IAssets>(new AssetProvider()); // Тут по курсу что то с AdressableAssets
            _services.RegisterSingle<IPersistentProgressService>(new PersistentProgressService());
            RegisterGameFactory(randomService);
            _services.RegisterSingle<ISaveLoadService>(new SaveLoadService(_services.Single<IPersistentProgressService>(), _services.Single<IGameFactory>()));
            _services.RegisterSingle<IReloadService>(new ReloadService(_stateMachine));

        }

        private void RegisterGameFactory(IRandomService randomService)
        {
            _services.RegisterSingle<IGameFactory>(new GameFactory
                (_services.Single<IAssets>(), 
                _services.Single<IStaticDataService>(),
                _services.Single<IPersistentProgressService>(),
                randomService));
        }

        private void RegisterStaticData()
        {
            IStaticDataService staticData = new StaticDataService();
            staticData.LoadMonsters();
            _services.RegisterSingle(staticData);
        }

        private static IInputService InputService()
        {
            if (Application.isEditor)
                return new StandaloneInputService();
            else
            {
                return new MobileInputService();
            }
        }
    }
}
