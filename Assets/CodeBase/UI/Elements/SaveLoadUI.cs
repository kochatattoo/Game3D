using CodeBase.Infrastructure.Services;
using CodeBase.Services.Input;
using UnityEngine;

namespace CodeBase.UI
{
    public abstract class SaveLoadUI : MonoBehaviour
    {
        protected IInputService _inputService;
        

        protected virtual void Start()
        {
            _inputService = AllServices.Container.Single<IInputService>();
        }
    }
}
