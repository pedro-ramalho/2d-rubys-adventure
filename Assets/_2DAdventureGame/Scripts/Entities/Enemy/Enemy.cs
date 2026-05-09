using UnityEngine;

public class Enemy : MonoBehaviour
{  
    // Components
    public Rigidbody2D Rigidbody { get; private set; }
    public Animator Animator { get; private set; }
    public AudioSource AudioSource { get; private set; }

    // Enemy Data
    [Header("Enemy Data")]
    [SerializeField] EnemyData data;
    public EnemyData Data => data;

    // Health
    public int CurrentHealth { get; set; }

    // State
    public EnemyState CurrentState { get; private set; }

    // State instances 
    public PatrollingState PatrollingState { get; private set; }
    public FixedState FixedState { get; private set; }

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        AudioSource = GetComponent<AudioSource>();

        CurrentHealth = data.maxHealth;

        PatrollingState = new PatrollingState();
        FixedState = new FixedState();

        // Set the initial state to PatrollingState
        CurrentState = PatrollingState;
        CurrentState.Enter(this);        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
