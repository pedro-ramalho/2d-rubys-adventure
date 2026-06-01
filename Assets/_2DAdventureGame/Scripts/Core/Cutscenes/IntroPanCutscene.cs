using System.Collections;
using UnityEngine;

public class IntroPanCutscene : Cutscene
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private float duration = 3f;
    [SerializeField] private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private bool playOnStart = true;

    void Start()
    {
        if (playOnStart) StartCoroutine(Play());
    }

    protected override IEnumerator Run()
    {
        if (startPoint == null || endPoint == null) yield break;

        Vector3 from = startPoint.position;
        Vector3 to = endPoint.position;

        transform.position = from;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = curve.Evaluate(Mathf.Clamp01(timer / duration));
            transform.position = Vector3.Lerp(from, to, t);
            yield return null;
        }

        transform.position = to;
    }
}
