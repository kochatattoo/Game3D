namespace CodeBase.Infrastructure.Services.SaveLoad
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
