using CodeBase.Enemy;
using CodeBase.Infrastructure.Assetmanagement;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.Infrastructure.Services.StaticData;
using CodeBase.Logic;
using CodeBase.StaticData;
using CodeBase.UI;
using CodeBase.UI.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace CodeBase.Infrastructure
{
    public class GameFactory : IGameFactory
    {
        private readonly IAssets _assets;
        private readonly IStaticDataService _staticData;
        private readonly IRandomService _randomService;
        private readonly IPersistentProgressService _progressService;
        private readonly IWindowService _windowService;
        private readonly IGameStateMachine _gameStateMachine;

        private GameObject HeroGameObject { get; set; }
        public List<ISavedProgressReader> ProgressReaders { get; } = new List<ISavedProgressReader>();
        public List<ISavedProgress> ProgressWriters { get; } = new List<ISavedProgress> { };

        public GameFactory(IAssets assets, IStaticDataService staticData, IPersistentProgressService progressService, IRandomService randomService, IWindowService windowService, IGameStateMachine gameStateMachine)
        {
            _assets = assets;
            _staticData = staticData;
            _randomService = randomService;
            _progressService = progressService;
            _windowService = windowService;
            _gameStateMachine = gameStateMachine;
        }

        public async Task WarmUp()
        {
           await _assets.Load<GameObject>(AssetAddress.Loot);
           await _assets.Load<GameObject>(AssetAddress.Spawner);
        }

        public async Task<GameObject> CreateHero(Vector3 at)
        {
            HeroGameObject = await InstantiateRegisteredAsync(AssetAddress.HeroParh, at);
            return HeroGameObject;
        }

        public async Task<GameObject> CreateHud() 
        {
            GameObject hud = await InstantiateRegisteredAsync(AssetAddress.HUDPath);
            hud.GetComponentInChildren<LootCounter>()
                .Construct(_progressService.Progress.WorldData);

            foreach(OpenWindowButton openWindowButton in hud.GetComponentsInChildren<OpenWindowButton>())
            {
                openWindowButton.Construct(_windowService);
            }

            return hud;
        }

        public async Task<GameObject> CreateMonster(MonsterTypeID typeId, Transform parent)
        {
            MonsterStaticData monsterData = _staticData.ForMonster(typeId);

            GameObject prefab = await _assets.Load<GameObject>(monsterData.PrefabReference);
            GameObject monster = Object.Instantiate(prefab, parent.position, Quaternion.identity, parent);

            IHealth health = monster.GetComponent<EnemyHealth>();
            health.Current = monsterData.Hp;
            health.Max = monsterData.Hp;

            monster.GetComponent<ActorUI>().Construct(health);
            monster.GetComponent<AgentMoveToHero>().Construct(HeroGameObject.transform);
            monster.GetComponent<NavMeshAgent>().speed = monsterData.MoveSpeed;

            LootSpawner lootSpawner = monster.GetComponentInChildren<LootSpawner>();
            lootSpawner.Setloot(monsterData.MinLoot, monsterData.MaxLoot);
            lootSpawner.Construct(this, _randomService);

            Attack attack = monster.GetComponent<Attack>();
            attack.Construct(HeroGameObject.transform);
            attack.Damage = monsterData.Damage;
            attack.Cleavage = monsterData.Cleavage;
            attack.EffectiveDistance = monsterData.EffectiveDistance;

            monster.GetComponent<RotateToHero>()?.Consturct(HeroGameObject.transform);

            return monster;
        }

        public async Task<LootPiece> CreateLoot()
        {
            GameObject prefab = await _assets.Load<GameObject>(AssetAddress.Loot);
            LootPiece lootPiece = InstantiateRegistered(prefab)
                .GetComponent<LootPiece>();

            lootPiece.Construct(_progressService.Progress.WorldData, this);

           return lootPiece;
        }

        public async Task<LootPiece> CreateLoot(string id)
        {
            LootPiece lootPiece = await CreateLoot();

            lootPiece.SetId(id);

            return lootPiece;
        }

        public async Task CreateSpawner(Vector3 at, string spawnerId, MonsterTypeID monsterTypeID)
        {
            GameObject prefab = await _assets.Load<GameObject>(AssetAddress.Spawner);
            SpawnPoint spawner = InstantiateRegistered(prefab, at)
                .GetComponent<SpawnPoint>();

            spawner.Construct(this);
            spawner.Id = spawnerId;
            spawner.MonsterTypeID = monsterTypeID;
        }

        public async Task CreateTransferToPoint(LevelTransferData levelTransferData)
        {
            GameObject levelTransferTriffer = await InstantiateRegisteredAsync(AssetAddress.TransferToPoint, levelTransferData.TransferToPosition);
              levelTransferTriffer.GetComponent<LevelTransferTrigger>()
                .Construct(_gameStateMachine, levelTransferData.LevelTo);
        }

        public async Task<Camera> CreateCameraOnScene()
        {
           GameObject pref = await _assets.Load<GameObject>(AssetAddress.Camera);
           Camera camera = InstantiateRegistered(pref).GetComponent<Camera>(); 
           return camera;
        }

        public void CleanUp()
        {
            ProgressReaders.Clear();
            ProgressWriters.Clear();

            _assets.CleanUp();
        }

        private void Register(ISavedProgressReader progressReader)
        {
            if (progressReader is ISavedProgress progressWriter)
            ProgressWriters.Add(progressWriter);

            ProgressReaders.Add(progressReader);
        }

        private async Task <GameObject> InstantiateRegisteredAsync(string path, Vector3 at)
        {
            GameObject gameObject = await _assets.Instantiate(path, at);
            RegisterProggressWatchers(gameObject);
            return gameObject;
        }

        private async Task<GameObject> InstantiateRegisteredAsync(string path)
        {
            GameObject gameObject = await _assets.Instantiate(path);
            RegisterProggressWatchers(gameObject);
            return gameObject;
        }

        private GameObject InstantiateRegistered(GameObject prefab, Vector3 at)
        {
            GameObject gameObject = Object.Instantiate(prefab, at, Quaternion.identity);
            RegisterProggressWatchers(gameObject);
            return gameObject;
        }

        private GameObject InstantiateRegistered(GameObject prefab)
        {
            GameObject gameObject = Object.Instantiate(prefab);
            RegisterProggressWatchers(gameObject);
            return gameObject;
        }

        private void RegisterProggressWatchers(GameObject gameObject)
        {
            foreach (ISavedProgressReader progressReader in gameObject.GetComponentsInChildren<ISavedProgressReader>())
            {
                Register(progressReader);
            }
        }
    }
}
