using CodeBase.Infrastructure;

namespace CodeBase.Infrastructure.Services
{
    public class AllServices
    {
        private static AllServices _instance;
        public static AllServices Container => _instance ?? (_instance = new AllServices());

        public void RegisterSingle<TService>(TService implemintation) where TService : IService =>
            Implemintation<TService>.ServiceInstance = implemintation;
      
        public TService Single<TService>() where TService : IService =>
            Implemintation<TService>.ServiceInstance;

        private static class Implemintation<TService> where TService : IService
        {
            public static TService ServiceInstance;
        }
    }
}
