using CodeBase.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.Infrastructure.Assetmanagement
{
    public interface IAssets: IService
    {
        GameObject Instantiate(string path);
        GameObject Instantiate(string path, Vector3 at);
    }
}