using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace AdventureGame.Entities.Player
{
    public class PlayerFootstep : MonoBehaviour
    {
        [Serializable]
        private class SurfaceProfile
        {
            public string tag;
            public AudioClip[] clips;
        }

        [SerializeField] private AudioSource source;
        [SerializeField] private SurfaceProfile[] surfaces;

        [SerializeField] private Vector2 feetOffset = new Vector2(0f, -0.3f);

        [Tooltip("Random pitch variation in either direction")]
        [SerializeField, Range(0f, 0.3f)] private float pitchVariation = 0.05f;

        private readonly List<(Tilemap tilemap, AudioClip[] clips)> resolved = new();

        void Awake() => SceneManager.sceneLoaded += OnSceneLoaded;

        void Start() => ResolveSurfaces();

        void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

        void OnSceneLoaded(Scene scene, LoadSceneMode mode) => ResolveSurfaces();

        public void PlayFootstep()
        {
            if (source == null) 
                return;

            AudioClip[] clips = SelectClips();
            if (clips == null || clips.Length == 0) 
                return;

            source.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
            source.pitch = 1f + UnityEngine.Random.Range(-pitchVariation, pitchVariation);
            source.Play();
        }

        private AudioClip[] SelectClips()
        {
            Vector3 feet = transform.position + (Vector3)feetOffset;
        
            foreach((Tilemap tilemap, AudioClip[] clips) in resolved)
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
            resolved.Clear();
            if (surfaces == null) 
                return;

            foreach (SurfaceProfile p in surfaces)
            {
                if (string.IsNullOrEmpty(p.tag)) 
                    continue;

                GameObject[] tagged = GameObject.FindGameObjectsWithTag(p.tag);
                foreach (GameObject g in tagged)
                    if (g.TryGetComponent(out Tilemap tm))
                        resolved.Add((tm, p.clips));
            }
        }
    }
}
