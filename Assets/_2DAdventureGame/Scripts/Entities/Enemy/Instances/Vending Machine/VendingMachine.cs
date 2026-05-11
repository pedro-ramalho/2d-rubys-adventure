using UnityEngine;

public class VendingMachine : Enemy
{  
    public static readonly int MoveXHash = Animator.StringToHash("Move X");
    public static readonly int MoveYHash = Animator.StringToHash("Move Y");

    [Header("Vending Machine Assets")]
    [SerializeField] private ParticleSystem smokeEffect;
    public ParticleSystem SmokeEffect => smokeEffect;

    public int Direction { get; set; }

    public VendingMachineState CurrentState { get; private set; }
    public VendingMachineMovingState MovingState { get; private set; }
    public VendingMachineChargingState ChargingState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        MovingState = new VendingMachineMovingState();
        ChargingState = new VendingMachineChargingState();

        CurrentState = MovingState;
        CurrentState.Enter(this);
    }

    void Update() => CurrentState.Update(this);

    void FixedUpdate() => CurrentState.Update(this);
    
    public void ChangeState(VendingMachineState newState)
    {
        CurrentState.Exit(this);
        CurrentState = newState;
        CurrentState.Enter(this);
    }

    protected override void OnProjectileHit() => Destroy(gameObject);
}
