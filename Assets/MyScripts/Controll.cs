using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controll : MonoBehaviour
{
    public enum PlayerControllerState
    {
        Idle,
        Walk,
        Jump
    }

    public class UnrefactoredPlayerController : MonoBehaviour
    {
        private PlayerControllerState state;

        private void Update()
        {
            GetInput();

            switch (state)
            {
                case PlayerControllerState.Idle:
                    Idle();
                    break;
                case PlayerControllerState.Walk:
                    Walk();
                    break;
                case PlayerControllerState.Jump:
                    Jump();
                    break;
            }
        }

        private void GetInput()
        {
            // process walk and jump controls
        }

        private void Walk()
        {
            // walk logic
        }

        private void Idle()
        {
            // idle logic
        }

        private void Jump()
        {
            // jump logic
        }
    }
}
