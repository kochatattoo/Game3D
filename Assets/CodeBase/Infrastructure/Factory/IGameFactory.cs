using CodeBase.Data;
using CodeBase.Enemy;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.StaticData;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public interface IGameFactory: IService
    {
        List<ISavedProgressReader> ProgressReaders { get; }
        List<ISavedProgress> ProgressWriters { get; }
        GameObject CreateHero(Vector3 at);
        GameObject CreateHud();
        GameObject CreateMonster(MonsterTypeID monsterTypeID, Transform parent);
        void Cleanup();
        void Register(ISavedProgressReader progressReader);
        LootPiece CreateLoot();
        LootPiece CreateLoot(string id);
    }
}