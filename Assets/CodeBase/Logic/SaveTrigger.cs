using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Services.SaveLoad;
using UnityEngine;

namespace CodeBase.Logic
{
    public class SaveTrigger: MonoBehaviour
    {
        private ISaveLoadService _saveLoadService;

        public BoxCollider Collider;
        private bool _isSaved = false;

        private void Awake()
        {
            _saveLoadService = AllServices.Container.Single<ISaveLoadService>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(_isSaved == false)
            {
                _isSaved = true;
                _saveLoadService.SaveProgress();
                Debug.Log("Progress Saved");
                gameObject.SetActive(false);
            }
        }

        private void OnDrawGizmos()
        {
            if(Collider == null)
                return;

            Gizmos.color = new Color32(30, 200, 30, 130);
            Gizmos.DrawCube(transform.position+Collider.center, Collider.size);
        }
    }
}
