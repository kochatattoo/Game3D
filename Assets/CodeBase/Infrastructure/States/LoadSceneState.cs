using CodeBase.Logic;
using CodeBase.CameraLogic;
using UnityEngine;
using CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.UI;
using UnityEngine.SceneManagement;
using CodeBase.Infrastructure.Services.StaticData;
using CodeBase.StaticData;
using System;
using CodeBase.UI.Services;


namespace CodeBase.Infrastructure
{
    public class LoadSceneState : IPayloadedState<string>
    {
        private const string InitialPointTag = "InitialPoint";
        private const string EnemySpawnerTag = "EnemySpawner";
        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly LoadingCurtain _curtain;
        private readonly IGameFactory _gameFactory;
        private readonly IPersistentProgressService _progressService;
        private readonly IStaticDataService _staticData;
        private readonly IUIFactory _uiFactory;

        public LoadSceneState(GameStateMachine gameStateMachine, SceneLoader sceneLoader,
            LoadingCurtain curtain, IGameFactory gameFactory, IPersistentProgressService progressService,
            IStaticDataService staticDataService, IUIFactory uiFactory)
        {
            _stateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _curtain = curtain;
            _gameFactory = gameFactory;
            _progressService = progressService;
            _staticData = staticDataService;
            _uiFactory = uiFactory;
        }

        public void Enter(string sceneName)
        {
            _curtain.Show();
            _gameFactory.Cleanup();
            _sceneLoader.Load(sceneName, OnLoaded);
        }

        public void Exit() {}
        

        private void OnLoaded()
        {
            InitUIRoot();
            InitGameWorld();
            InformProgressReaders();

            _curtain.Hide();
            _stateMachine.Enter<GameLoopState>();
        }

        private void InitUIRoot() => 
            _uiFactory.CreateUIRoot();

        private void InformProgressReaders()
        {
            foreach (ISavedProgressReader progressReader in _gameFactory.ProgressReaders)
            {
                progressReader.LoadProgress(_progressService.Progress);
            }
        }

        private void InitGameWorld()
        {
            InitSpawners();
            InitLoots();

            GameObject hero = InitHero();
            InitHud(hero);

            CameraFollow(hero);
        }

        private void InitLoots()
        {
            foreach (var id in _progressService.Progress.WorldData.LootData.LootsOnGround.Dict)
            {
                _gameFactory.CreateLoot(id.Key);
            }
        }

        private void InitSpawners()
        {
            string sceneKey = SceneManager.GetActiveScene().name;
            LevelStaticData levelData = _staticData.ForLevel(sceneKey);
            foreach(EnemySpawnerData spawnerData in levelData.EnemySpawners)
            {
                _gameFactory.CreateSpawner(spawnerData.position, spawnerData.Id, spawnerData.MonsterTypeID);
            }
        }

        private GameObject InitHero()
        {
            return _gameFactory.CreateHero(at: GameObject
                .FindGameObjectWithTag(InitialPointTag)
                .transform.position);
        }

        private void InitHud(GameObject hero)
        {
           GameObject hud = _gameFactory.CreateHud();

            hud.GetComponentInChildren<ActorUI>()
                .Construct(hero.GetComponent<IHealth>());
        }

        private static void CameraFollow(GameObject hero) => 
            Camera.main.GetComponent<CameraFollow>()
            .Follow(hero);
    }
}