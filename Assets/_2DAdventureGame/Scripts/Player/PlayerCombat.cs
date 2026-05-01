using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private InputAction launchAction;
    [SerializeField] private GameObject projectile;
    [SerializeField] private AudioClip throwProjectileClip;

    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;
    private PlayerMovement playerMovement;

    static readonly int LaunchHash = Animator.StringToHash("Launch");

    void Start()
    {
        launchAction.Enable();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (launchAction.WasPressedThisFrame())
            Launch();
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectile, rb.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile p = projectileObject.GetComponent<Projectile>();
        p.Launch(playerMovement.MoveDirection, 300);
        animator.SetTrigger(LaunchHash);
        audioSource.PlayOneShot(throwProjectileClip);
    }
}