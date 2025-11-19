using CodeBase.Data;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.Enemy
{
    public class LootSpawner : MonoBehaviour
    {
        public EnemyDeath EnemyDeath;
        private IGameFactory _factory;
        private IRandomService _random;
        private int _lootMin;
        private int _lootMax;

        public void Construct(IGameFactory gameFactory, IRandomService random)
        {
            _factory = gameFactory;
            _random = random;
        }

        private void Start()
        {
            EnemyDeath.Happened += SpawnLoot;
        }

        private void SpawnLoot()
        {
            LootPiece loot = _factory.CreateLoot();
            loot.transform.position = transform.position;

            Loot lootItem = GenerateLoot();
            loot.Initialize(lootItem);
        }

        private Loot GenerateLoot()
        {
            return new Loot()
            {
                Value = _random.Next(_lootMin, _lootMax)
            };
        }

        public void Setloot(int min, int max)
        {
            _lootMin = min;
            _lootMax = max;
        }
    }
}
