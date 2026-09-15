using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace AdventureGame.Entities.Player
{
    public class PlayerFootstep : MonoBehaviour
    {
        [Serializable]
        private class SurfaceProfile
        {
            public string Tag;

            public AudioClip[] Clips;
        }

        [SerializeField]
        private AudioSource m_AudioSource;

        [SerializeField]
        private SurfaceProfile[] m_Surfaces;

        [SerializeField]
        private Vector2 m_FeetOffset = new Vector2(0f, -0.3f);

        [Tooltip("Random pitch variation in either direction")]
        [SerializeField, Range(0f, 0.3f)]
        private float m_PitchVariation = 0.05f;

        private readonly List<(Tilemap tilemap, AudioClip[] clips)> m_Resolved = new();

        void Awake() => SceneManager.sceneLoaded += OnSceneLoaded;

        void Start() => ResolveSurfaces();

        void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

        void OnSceneLoaded(Scene scene, LoadSceneMode mode) => ResolveSurfaces();

        public void PlayFootstep()
        {
            if (m_AudioSource == null)
                return;

            AudioClip[] clips = SelectClips();
            if (clips == null || clips.Length == 0)
                return;

            m_AudioSource.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
            m_AudioSource.pitch =
                1f + UnityEngine.Random.Range(-m_PitchVariation, m_PitchVariation);
            m_AudioSource.Play();
        }

        private AudioClip[] SelectClips()
        {
            Vector3 feet = transform.position + (Vector3)m_FeetOffset;

            foreach ((Tilemap tilemap, AudioClip[] clips) in m_Resolved)
            {
                if (tilemap == null)
                    continue;

                Vector3Int cell = tilemap.WorldToCell(feet);
                if (tilemap.GetTile(cell) != null)
                    return clips;
            }

            return null;
        }

        private void ResolveSurfaces()
        {
            m_Resolved.Clear();
            if (m_Surfaces == null)
                return;

            foreach (SurfaceProfile p in m_Surfaces)
            {
                if (string.IsNullOrEmpty(p.Tag))
                    continue;

                GameObject[] tagged = GameObject.FindGameObjectsWithTag(p.Tag);
                foreach (GameObject g in tagged)
                    if (g.TryGetComponent(out Tilemap tm))
                        m_Resolved.Add((tm, p.Clips));
            }
        }
    }
}
