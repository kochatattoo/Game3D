using CodeBase.Data;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Services.PersistentProgress;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace CodeBase.Enemy
{
    public class LootPiece : MonoBehaviour, ISavedProgress
    {
        public GameObject Skull;
        public GameObject PickupFxPrefab;
        public TextMeshPro LootText;
        public GameObject PickupPopup;

        private Loot _loot;
        private bool _picked;
        private WorldData _worldData;
        [SerializeField]private string _id;

        private IGameFactory _gameFactory;

        public void Construct(WorldData worldData, IGameFactory gameFactory)
        {
           _worldData = worldData;
            _gameFactory = gameFactory;
        }

        public void Initialize(Loot loot)
        {
            _id = $"{gameObject.scene.name}_{Guid.NewGuid().ToString()}";
            _loot = loot;
        }

        public void SetId(string id) => _id = id;

        public void UpdateProgress(PlayerProgress progress)
        {
            if (!progress.WorldData.LootData.LootsOnGround.Dict.ContainsKey(_id))
            {
                LootObject lootObject = new(_id, transform.position, _loot);
                progress.WorldData.LootData.LootsOnGround.Dict.Add(_id, lootObject);
            }

        }

        public void LoadProgress(PlayerProgress progress)
        {
            if (progress.WorldData.LootData.LootsOnGround.Dict.TryGetValue(_id, out LootObject lootPiece))
            {
                LoadData(lootPiece);

                progress.WorldData.LootData.LootsOnGround.Dict.Remove(_id);
            }
        }

        private void LoadData(LootObject lootObject)
        {
            _loot = lootObject.loot;
            transform.position = lootObject.positionData.AsUnityVector();
            _id = lootObject.id;
        }

        private void OnTriggerEnter(Collider other) => 
            PickUp();

        private void PickUp()
        {
            if (_picked)
                return;

            _picked = true;

            _worldData.LootData.LootsOnGround.Dict.Remove(_id);

            _gameFactory.ProgressWriters.Remove(this);
            _gameFactory.ProgressReaders.Remove(this);

            UpdateWorldData();
            HideSkull();
            PlayPickupFx();
            Showtext();

            StartCoroutine(StartDestroyTimer());
        }

        private void UpdateWorldData() => 
            _worldData.LootData.Collect(_loot);

        private void HideSkull() => 
            Skull.SetActive(false);

        private void PlayPickupFx() => 
            Instantiate(PickupFxPrefab, transform.position, Quaternion.identity);

        private void Showtext()
        {
            LootText.text = $"{_loot.Value}";
            PickupPopup.SetActive(true);
        }
        private IEnumerator StartDestroyTimer()
        {
            yield return new WaitForSeconds(1.5f);

            Destroy(gameObject);
        }

    }
}
