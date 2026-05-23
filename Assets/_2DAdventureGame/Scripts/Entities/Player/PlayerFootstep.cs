using UnityEngine;

public class PlayerFootstep : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip[] clips;

    [Tooltip("Random pitch variation in either direction")]
    [SerializeField, Range(0f, 0.3f)] private float pitchVariation = 0.05f;

    public void PlayFootstep()
    {
        if (source == null || clips == null || clips.Length == 0) return;

        source.clip = clips[Random.Range(0, clips.Length)];
        source.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        source.Play();
    }
}
