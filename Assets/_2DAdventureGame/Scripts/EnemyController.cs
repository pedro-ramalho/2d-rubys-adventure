using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed = 1.0f;

    public bool vertical;

    public float changeTime = 3.0f;
    float timer;
    int direction = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        timer = changeTime;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rb.position;

        if (vertical)
        {
            position.y += speed * direction * Time.deltaTime;
        }
        else
        {
            position.x += speed * direction * Time.deltaTime;
        }

        rb.MovePosition(position);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();
        if (controller == null) return;

        controller.ChangeHealth(-1);
    }
}
