using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }
    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        if (Instance == null) Instance = this;
        
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake(float amplitude = 1f) => impulseSource.GenerateImpulse(amplitude);
}
