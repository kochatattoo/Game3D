using CodeBase.Infrastructure.Services.Ads;
using CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.UI.Windows.Shop;
using TMPro;

namespace CodeBase.UI
{
    public class ShopWindow : WindowBase
    {
        public TextMeshProUGUI Skulltext;
        public RewardedAdItem AdItem;

        public void Construct(IAdsService adsService, IPersistentProgressService progressService) 
        {
            base.Construct(progressService);
            AdItem.Construct(adsService, progressService);
        }
        protected override void Initialize()
        {
            AdItem.Initialize();
            RefreshSkullText();
        }
        protected override void SubscribeUpdates() 
        {
            AdItem.Subscribe();
            Progress.WorldData.LootData.Changed += RefreshSkullText; 
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            AdItem.CleanUp();
            Progress.WorldData.LootData.Changed -= RefreshSkullText;
        }

        private void RefreshSkullText() => 
            Skulltext.text = Progress.WorldData.LootData.Collected.ToString();
    }
}
