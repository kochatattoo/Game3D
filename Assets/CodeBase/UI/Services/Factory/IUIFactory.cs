using CodeBase.Infrastructure.Services;
using System.Threading.Tasks;

namespace CodeBase.UI.Services
{
    public interface IUIFactory: IService
    {
        void CreateShop();
        Task CreateUIRoot();
    }
}
