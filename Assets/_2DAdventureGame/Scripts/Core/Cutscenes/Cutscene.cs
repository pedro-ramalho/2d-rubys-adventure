using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineCamera))]
public abstract class Cutscene : MonoBehaviour
{
    [SerializeField] protected int activePriority = 100;

    protected CinemachineCamera cam;
    private int dormantPriority;

    protected virtual void Awake()
    {
        cam = GetComponent<CinemachineCamera>();
        dormantPriority = cam.Priority;
    }

    public IEnumerator Play()
    {
        if (CutsceneManager.Instance != null)
            CutsceneManager.Instance.IsActive = true;

        cam.Priority = activePriority;

        yield return Run();

        cam.Priority = dormantPriority;

        if (CutsceneManager.Instance != null)
            CutsceneManager.Instance.IsActive = false;
    }

    protected abstract IEnumerator Run();
}
