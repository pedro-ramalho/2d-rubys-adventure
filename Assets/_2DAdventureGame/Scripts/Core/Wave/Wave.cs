using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Core.Wave
{
    [Serializable]
    public class Wave
    {
        [FormerlySerializedAs("enemyPrefab")]
        public GameObject EnemyPrefab;

        [FormerlySerializedAs("enemyCount")]
        public int EnemyCount;
    }
}
