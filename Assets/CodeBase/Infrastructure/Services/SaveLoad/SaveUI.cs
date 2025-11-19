namespace CodeBase.Infrastructure.Services.SaveLoad
{
    public class SaveUI : SaveLoadUI
    {
        private ISaveLoadService _saveLoadService;

        protected override void Start()
        {
            base.Start();
            _saveLoadService = AllServices.Container.Single<ISaveLoadService>();
        }

        protected void Update()
        {
            if (_inputService.IsSaveButtonUp())
                _saveLoadService.SaveProgress();
        }
    }
}
