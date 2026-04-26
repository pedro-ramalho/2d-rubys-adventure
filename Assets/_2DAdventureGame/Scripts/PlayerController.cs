using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction moveAction;
    private Rigidbody2D rb;
    private Vector2 move;

    // Player health
    public int maxHealth = 5;
    public int health { get { return currentHealth; }}
    int currentHealth = 4;

    // Player speed
    public float speed = 3.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction.Enable();
        rb = GetComponent<Rigidbody2D>();
        //currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        move = moveAction.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        Vector2 position = rb.position + move * speed * Time.deltaTime;
        rb.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);
    }
}
