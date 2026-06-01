using System.Collections;
using UnityEngine;

public class FollowTargetCutscene : Cutscene
{
    [SerializeField] private Transform target;

    private bool finished;

    protected override void Awake()
    {
        base.Awake();
        if (target != null) cam.Follow = target;
    }

    public void Begin()
    {
        if (CutsceneManager.Active) return;
        finished = false;
        StartCoroutine(Play());
    }

    public void Finish() => finished = true;

    protected override IEnumerator Run()
    {
        yield return new WaitUntil(() => finished);
    }
}
