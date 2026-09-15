using AdventureGame.Core.Constants;
using AdventureGame.Core.Quests;
using AdventureGame.Entities.Enemy.Instances.Vending_Machine.States;
using UnityEngine;

namespace AdventureGame.Entities.Enemy.Instances.Vending_Machine
{
    public class VendingMachine : Enemy
    {
        public new VendingMachineData Data => (VendingMachineData)base.Data;

        public SpriteRenderer SpriteRenderer { get; private set; }
        public Color BaseColor { get; private set; }

        public Vector2 ChargeDirection { get; set; }
        public bool IsChargeHorizontal =>
            Mathf.Abs(ChargeDirection.x) > Mathf.Abs(ChargeDirection.y);

        public VendingMachineState CurrentState { get; private set; }
        public VendingMachineMovingState MovingState { get; private set; }
        public VendingMachineWindupState WindupState { get; private set; }
        public VendingMachineChargingState ChargingState { get; private set; }
        public VendingMachineStunnedState StunnedState { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            BaseColor = SpriteRenderer.color;

            MovingState = new VendingMachineMovingState();
            WindupState = new VendingMachineWindupState();
            ChargingState = new VendingMachineChargingState();
            StunnedState = new VendingMachineStunnedState();

            CurrentState = MovingState;
            CurrentState.Enter(this);
        }

        void Update() => CurrentState.Update(this);

        void FixedUpdate() => CurrentState.FixedUpdate(this);

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent(out VendingMachine _))
            {
                SpawnExplosion(SpriteRenderer.bounds.center);
                GetComponent<QuestReporter>()?.Report();
                Destroy(gameObject);

                return;
            }

            if (collision.gameObject.TryGetComponent(out Player.Player player))
            {
                player.ApplyDamage(Data.ContactDamage);

                if (CurrentState == ChargingState)
                {
                    SpawnExplosion(collision.GetContact(0).point);
                    ChangeState(StunnedState);
                }

                return;
            }

            if (CurrentState == ChargingState && collision.gameObject.CompareTag(Tags.Wall))
            {
                SpawnExplosion(collision.GetContact(0).point);
                ChangeState(StunnedState);
            }
        }

        void SpawnExplosion(Vector2 position)
        {
            if (Data.ExplosionPrefab != null)
                Instantiate(Data.ExplosionPrefab, position, Quaternion.identity);
        }

        public void ChangeState(VendingMachineState newState)
        {
            CurrentState.Exit(this);
            CurrentState = newState;
            CurrentState.Enter(this);
        }

        protected override void OnProjectileHit()
        {
            SpawnExplosion(SpriteRenderer.bounds.center);
            GetComponent<QuestReporter>()?.Report();
            Destroy(gameObject);
        }
    }
}
