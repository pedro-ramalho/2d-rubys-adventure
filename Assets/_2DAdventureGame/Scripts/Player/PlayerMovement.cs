using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputAction moveAction;
    [SerializeField] private float speed = 3.0f;

    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;

    [SerializeField] private AudioClip playerWalkClip;

    private Vector2 move;
    private Vector2 moveDirection = new(1, 0);

    static readonly int LookXHash = Animator.StringToHash("Look X");
    static readonly int LookYHash = Animator.StringToHash("Look Y");
    static readonly int SpeedHash = Animator.StringToHash("Speed");

    public Vector2 MoveDirection => moveDirection;

    void Start()
    {
        moveAction.Enable();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        move = moveAction.ReadValue<Vector2>();

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }

        animator.SetFloat(LookXHash, moveDirection.x);
        animator.SetFloat(LookYHash, moveDirection.y);
        animator.SetFloat(SpeedHash, move.magnitude);

        if (move.magnitude > 0)
        {
            if (audioSource.clip != playerWalkClip || !audioSource.isPlaying)
            {
                audioSource.clip = playerWalkClip;
                audioSource.Play();
            }
        }
        else if (audioSource.clip == playerWalkClip && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + move * (speed * Time.deltaTime));
    }
}