using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class Minimap : MonoBehaviour
{
    [SerializeField] private InputAction mapAction;
    [SerializeField] private GameObject mapOverlay;

    private Camera mapCamera;

    void Awake()
    {
        mapCamera = GetComponent<Camera>();
        mapCamera.enabled = false;

        if (mapOverlay != null)
            mapOverlay.SetActive(false);
    }

    void OnEnable() => mapAction.Enable();
    void OnDisable() => mapAction.Disable();

    void Update()
    {
        if (mapAction.WasPressedThisFrame())
            ToggleMap();
    }

    void ToggleMap()
    {
        bool show = !mapCamera.enabled;
        mapCamera.enabled = show;

        if (mapOverlay != null)
            mapOverlay.SetActive(show);
    }
}
