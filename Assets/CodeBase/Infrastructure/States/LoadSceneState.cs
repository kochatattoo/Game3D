using CodeBase.Logic;
using CodeBase.CameraLogic;
using UnityEngine;
using CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.UI;
using UnityEngine.SceneManagement;
using CodeBase.Infrastructure.Services.StaticData;
using CodeBase.StaticData;
using CodeBase.UI.Services;
using System.Threading.Tasks;


namespace CodeBase.Infrastructure
{
    public class LoadSceneState : IPayloadedState<string>
    {
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
            _gameFactory.CleanUp();
            _gameFactory.WarmUp();
            _sceneLoader.Load(sceneName, OnLoaded);
        }

        public void Exit() {}

        private async void OnLoaded()
        {
            await InitUIRoot();
            await InitGameWorld();
            InformProgressReaders();

            _curtain.Hide();
            _stateMachine.Enter<GameLoopState>();
        }

        private async Task InitUIRoot() => 
           await _uiFactory.CreateUIRoot();

        private void InformProgressReaders()
        {
            foreach (ISavedProgressReader progressReader in _gameFactory.ProgressReaders)
            {
                progressReader.LoadProgress(_progressService.Progress);
            }
        }

        private async Task InitGameWorld()
        {
            LevelStaticData levelData = LevelStaticData();

            // Camera camera = InitCamera();
            await InitSpawners(levelData);
            await InitLoots();
            await InitTransferToPoints(levelData);

            GameObject hero = await InitHero(levelData);
            await InitHud(hero);


            CameraFollow(hero);
        }

        private async Task<Camera> InitCamera() =>
            await _gameFactory.CreateCameraOnScene();

        private async Task InitTransferToPoints(LevelStaticData levelData) =>
           await _gameFactory.CreateTransferToPoint(levelData.LevelTransferData);

        private async Task InitLoots()
        {
            foreach (var id in _progressService.Progress.WorldData.LootData.LootsOnGround.Dict)
            {
              await _gameFactory.CreateLoot(id.Key);
            }
        }

        private async Task InitSpawners(LevelStaticData levelData)
        {
            foreach(EnemySpawnerData spawnerData in levelData.EnemySpawners)
            {
               await _gameFactory.CreateSpawner(spawnerData.position, spawnerData.Id, spawnerData.MonsterTypeID);
            }
        }

        private async Task<GameObject> InitHero(LevelStaticData levelData) =>
           await _gameFactory.CreateHero(at: levelData.InitialHeroPosition);

        private async Task InitHud(GameObject hero)
        {
           GameObject hud = await _gameFactory.CreateHud();

            hud.GetComponentInChildren<ActorUI>()
                .Construct(hero.GetComponent<IHealth>());
        }

        private LevelStaticData LevelStaticData() => 
            _staticData.ForLevel(SceneManager.GetActiveScene().name);

        private static void CameraFollow( GameObject hero) => 
            Camera.main.GetComponent<CameraFollow>()
            .Follow(hero);

        private static void CameraFollow(Camera camera,GameObject hero) =>
            camera.GetComponent<CameraFollow>()
            .Follow(hero);
    }
}