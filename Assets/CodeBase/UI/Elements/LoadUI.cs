using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Services.SaveLoad;

namespace CodeBase.UI
{
    public class LoadUI : SaveLoadUI
    {
        private IReloadService _reloadService;

        protected override void Start()
        {
            base.Start();
            _reloadService = AllServices.Container.Single<IReloadService>();

        }
        protected void Update()
        {
            if (_inputService.IsLoadButtonUp())
               _reloadService.Reload();
        }
    }
}
