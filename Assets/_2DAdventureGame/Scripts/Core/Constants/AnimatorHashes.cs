using UnityEngine;

namespace AdventureGame.Core.Constants
{
    public static class AnimatorHashes
    {
        public static readonly int MoveX = Animator.StringToHash("Move X");
        public static readonly int MoveY = Animator.StringToHash("Move Y");

        public static readonly int LookX = Animator.StringToHash("Look X");
        public static readonly int LookY = Animator.StringToHash("Look Y");
        public static readonly int Speed = Animator.StringToHash("Speed");
        public static readonly int Hit = Animator.StringToHash("Hit");
        public static readonly int Launch = Animator.StringToHash("Launch");

        public static readonly int Fixed = Animator.StringToHash("Fixed");

        public static readonly int ChargingHorizontal = Animator.StringToHash("ChargingHorizontal");
    }
}
