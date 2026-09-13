using Unity.Cinemachine;
using UnityEngine;

namespace AdventureGame.Core.Effects
{
    public class CameraShake : SceneSingleton<CameraShake>
    {
        private CinemachineImpulseSource impulseSource;

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;

            impulseSource = GetComponent<CinemachineImpulseSource>();
        }

        public void Shake(float amplitude = 1f) => impulseSource.GenerateImpulse(amplitude);
    }
}
