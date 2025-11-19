using CodeBase.Data;
using CodeBase.Enemy;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.StaticData;
using UnityEngine;

namespace CodeBase.Logic
{
    public class EnemySpawner : MonoBehaviour, ISavedProgress
    {
        public MonsterTypeID MonsterTypeID;
        private string _id;

        [SerializeField] private bool _slain;
        private IGameFactory _factory;
        private EnemyDeath _enemyDeath;
        public bool Slain => _slain;

        private void Awake()
        {
            _id = GetComponent<UniqueId>().Id;
            _factory = AllServices.Container.Single<IGameFactory>();
        }

        public void LoadProgress(PlayerProgress progress)
        {
            if (progress.KillData.ClaeredSpawners.Contains(_id))
                _slain = true;
            else
                Spawn();
        }

        private void Spawn()
        {
            GameObject monster = _factory.CreateMonster(MonsterTypeID, transform);
            _enemyDeath = monster.GetComponent<EnemyDeath>();
            _enemyDeath.Happened += Slay; 
        }

        private void Slay()
        {
            if( _enemyDeath != null ) 
            _enemyDeath.Happened -= Slay;

            _slain = true;
        }

        public void UpdateProgress(PlayerProgress progress)
        {
            if (_slain)
            {
                Debug.Log("Регестрируем выключеный спавнер");
                progress.KillData.ClaeredSpawners.Add(_id); 
            }
        }
    }
}
