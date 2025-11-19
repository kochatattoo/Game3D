using CodeBase.Enemy;
using CodeBase.Utility;
using System;
using System.Collections.Generic;

namespace CodeBase.Data
{
    [Serializable]
    public class LootData
    {
        public int Collected;
        public event Action Changed;

        public SerializableDictionary<string , LootObject> LootsOnGround;

        public LootData() 
        {
            LootsOnGround = new SerializableDictionary<string, LootObject>();
        }

        public void Collect(Loot loot)
        {
            Collected += loot.Value;
            Changed?.Invoke();
        }
    }
}