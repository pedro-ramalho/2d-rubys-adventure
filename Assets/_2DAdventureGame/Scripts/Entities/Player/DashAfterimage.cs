using System.Collections;
using UnityEngine;

namespace AdventureGame.Entities.Player
{
    public class DashAfterimage : MonoBehaviour
    {
        public void Initialize(Sprite sprite, Vector3 scale, bool flipX, Color tint, float duration)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = tint;
            sr.flipX = flipX;
            transform.localScale = scale;

            StartCoroutine(Fade(sr, duration));    
        }

        IEnumerator Fade(SpriteRenderer sr, float duration)
        {
            Color start = sr.color;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                sr.color = Color.Lerp(start, Color.clear, elapsed / duration);
                elapsed += Time.deltaTime;

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
