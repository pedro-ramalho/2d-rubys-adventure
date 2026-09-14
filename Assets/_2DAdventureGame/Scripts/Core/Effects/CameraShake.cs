using Unity.Cinemachine;
using UnityEngine;

namespace AdventureGame.Core.Effects
{
    public class CameraShake : SceneSingleton<CameraShake>
    {
        private CinemachineImpulseSource m_ImpulseSource;

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;

            m_ImpulseSource = GetComponent<CinemachineImpulseSource>();
        }

        public void Shake(float amplitude = 1f) => m_ImpulseSource.GenerateImpulse(amplitude);
    }
}
