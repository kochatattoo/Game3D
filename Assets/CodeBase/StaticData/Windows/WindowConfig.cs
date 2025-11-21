using CodeBase.UI;
using CodeBase.UI.Services;
using System;

namespace CodeBase.StaticData.Windows
{
    [Serializable]
    public class WindowConfig
    {
        public WindowId WindowId;
        public WindowBase prefab;
    }
}