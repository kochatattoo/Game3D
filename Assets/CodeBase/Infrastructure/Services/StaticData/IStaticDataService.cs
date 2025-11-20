
using CodeBase.StaticData;

namespace CodeBase.Infrastructure.Services.StaticData
{
    public interface IStaticDataService: IService
    {
        LevelStaticData ForLevel(string sceneKey);
        MonsterStaticData ForMonster(MonsterTypeID typeId);
        void LoadMonsters();
    }
}