using CodeBase.Services.Input;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.SaveLoad
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
