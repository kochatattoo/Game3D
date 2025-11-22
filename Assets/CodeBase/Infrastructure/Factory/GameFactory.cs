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
using UnityEngine.AddressableAssets;
using UnityEngine.AI;
using UnityEngine.ResourceManagement.AsyncOperations;
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

        public GameObject CreateHero(Vector3 at)
        {
            HeroGameObject = InstantiateRegistered(AssetPath.HeroParh, at);

            return HeroGameObject;
        }

        public GameObject CreateHud() 
        {
            GameObject hud = InstantiateRegistered(AssetPath.HUDPath);
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

            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(monsterData.PrefabReference);

            GameObject prefab = await handle
                .Task;

            Addressables.Release(handle);

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

        public LootPiece CreateLoot()
        {
            LootPiece lootPiece = InstantiateRegistered(AssetPath.Loot)
                .GetComponent<LootPiece>();

            lootPiece.Construct(_progressService.Progress.WorldData, this);

           return lootPiece;
        }

        public LootPiece CreateLoot(string id)
        {
            LootPiece lootPiece = CreateLoot();

            lootPiece.SetId(id);

            return lootPiece;
        }

        public void CreateTransferToPoint(LevelTransferData levelTransferData) =>
            InstantiateRegistered(AssetPath.TransferToPoint, levelTransferData.TransferToPosition)
                .GetComponent<LevelTransferTrigger>()
                .Construct(_gameStateMachine, levelTransferData.LevelTo);

        public void CreateSpawner(Vector3 at, string spawnerId, MonsterTypeID monsterTypeID)
        {
            var spawner = InstantiateRegistered(AssetPath.Spawner, at)
                .GetComponent<SpawnPoint>();

            spawner.Construct(this);
            spawner.Id = spawnerId;
            spawner.MonsterTypeID = monsterTypeID;

        }

        public Camera CreateCameraOnScene() =>
            _assets.Instantiate(AssetPath.Camera).GetComponent<Camera>();

        public void Cleanup()
        {
            ProgressReaders.Clear();
            ProgressWriters.Clear();
        }

        private void Register(ISavedProgressReader progressReader)
        {
            if (progressReader is ISavedProgress progressWriter)
            ProgressWriters.Add(progressWriter);

            ProgressReaders.Add(progressReader);
        }

        private GameObject InstantiateRegistered(string prefabPath, Vector3 at)
        {
            GameObject gameObject = _assets.Instantiate(prefabPath, at);
            RegisterProggressWatchers(gameObject);
            return gameObject;
        }   
        
        private GameObject InstantiateRegistered(string prefabPath)
        {
            GameObject gameObject = _assets.Instantiate(prefabPath);
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
