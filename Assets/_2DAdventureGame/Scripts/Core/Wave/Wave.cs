using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Core.Wave
{
    [Serializable]
    public class Wave
    {
        public GameObject EnemyPrefab;

        public int EnemyCount;
    }
}
