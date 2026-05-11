public abstract class VendingMachineState : State<VendingMachine>
{
    public abstract override string ID { get; }
    public override void Enter(VendingMachine owner) { }
    public override void Exit(VendingMachine owner) { }
    public override void FixedUpdate(VendingMachine owner) { }
    public override void Update(VendingMachine owner) { }
}
