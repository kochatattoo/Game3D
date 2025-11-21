using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.Infrastructure.Services.SaveLoad;
using CodeBase.Infrastructure.Services.StaticData;
using CodeBase.Logic;
using CodeBase.UI.Services;
using System;
using System.Collections.Generic;

namespace CodeBase.Infrastructure
{
    /// <summary>
    /// Класс изменяющий состояние игры
    /// </summary>
    public class GameStateMachine
    {
        private readonly Dictionary<Type, IExitableState> _states;
        private IExitableState _activeState;

        public GameStateMachine(SceneLoader sceneLoader, LoadingCurtain curtain, AllServices services)
        {
            _states = new Dictionary<Type, IExitableState>() // Словарь наших стейтов ключ по типу
            {
                [typeof(BootstrapState)] = new BootstrapState(this, sceneLoader, services),
                [typeof(LoadSceneState)] = new LoadSceneState(this, sceneLoader, curtain,
                                                              services.Single<IGameFactory>(), 
                                                              services.Single<IPersistentProgressService>(),
                                                              services.Single<IStaticDataService>(), 
                                                              services.Single<IUIFactory>()),
                [typeof(LoadProgressState)] = new LoadProgressState(this, 
                                                              services.Single<IPersistentProgressService>(), 
                                                              services.Single<ISaveLoadService>()),
                [typeof(GameLoopState)] = new GameLoopState(this)
            };
            
        }

        /// <summary>
        /// Входим в новое состояние через обобщеные типы (ограничение интерфейс IState)
        /// </summary>
        public void Enter<TState>() where TState : class, IState
        {
            IState state = ChangeState<TState>();
            state.Enter(); // Запускаем его
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
        {
            TState state = ChangeState<TState>();
            state.Enter(payload); // Запускаем его
        }

        private TState ChangeState<TState>() where TState : class, IExitableState
        {
            _activeState?.Exit();

            TState state = GetState<TState>();
            _activeState = state;

            return state;
        }

        private TState GetState<TState>() where TState : class, IExitableState =>
            _states[typeof(TState)] as TState;
        
    }
}
