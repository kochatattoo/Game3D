using CodeBase.StaticData;
using CodeBase.StaticData.Windows;
using CodeBase.UI.Services;

namespace CodeBase.Infrastructure.Services.StaticData
{
    public interface IStaticDataService: IService
    {
        LevelStaticData ForLevel(string sceneKey);
        MonsterStaticData ForMonster(MonsterTypeID typeId);
        WindowConfig ForWindow(WindowId shop);
        void LoadMonsters();
    }
}