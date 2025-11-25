using CodeBase.Enemy;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.StaticData;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public interface IGameFactory: IService
    {
        List<ISavedProgressReader> ProgressReaders { get; }
        List<ISavedProgress> ProgressWriters { get; }
        Task<GameObject> CreateHero(Vector3 at);
        Task<GameObject> CreateHud();
        Task<GameObject> CreateMonster(MonsterTypeID typeId, Transform parent);
        void CleanUp();
        Task<LootPiece> CreateLoot();
        Task<LootPiece> CreateLoot(string id);
        Task CreateSpawner(Vector3 at, string spawnerId, MonsterTypeID monsterTypeID);
        Task CreateTransferToPoint(LevelTransferData levelTransferData);
        Task<Camera> CreateCameraOnScene();
        Task WarmUp();
    }
}