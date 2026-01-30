using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Player
{
    // Requerimos el componente automáticamente
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovementController : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem stepParticles;

        [SerializeField]
        private float baseMovementSpeed = 5;

        public Vector2 InputDirection { get; private set; }
        public Vector2 MovementDirection { get; private set; }
        public Vector3 Velocity { get; private set; }


        private CharacterController characterController;

        private float verticalVelocity;
        private bool canMove = true;

        [Header("Ground Stick")]
        [SerializeField] private float stickToGroundForce = -10f; // Fuerza fuerte hacia abajo
        [SerializeField] LayerMask groundLayer;

        public bool CanMove
        {
            get => canMove;
            set
            {
                canMove = value;
                if (!value)
                {
                    MovementDirection = Vector2.zero;
                    verticalVelocity = 0;
                }
            }
        }

        private void Start()
        {
            characterController = GetComponent<CharacterController>();
        }


        private void Update()
        {
            if (!CanMove) return;

            ProcessInput();
            ProcessMovement();
        }

        private void ProcessInput()
        {
            InputDirection = Utils.AdjustToCamera(InputController.LeftStick);
        }

        private void ProcessMovement()
        {
            Vector2 targetMovement = InputDirection * baseMovementSpeed;

            MovementDirection = targetMovement;

            Vector3 finalMove = Utils.ProjectIn3D(MovementDirection) * Time.deltaTime;

            bool edgeDetected = EdgeDetection(finalMove);

            if (!edgeDetected)
            {
                finalMove = Vector3.zero;
            }

            characterController.Move(finalMove );

            Velocity = characterController.velocity;
        }


        private bool EdgeDetection(Vector3 desiredMovement)
        {
            Vector3 nextFramePosition = transform.position + desiredMovement;

            const float offset = .2f;
            nextFramePosition.y += offset;

            if (!Physics.Raycast(nextFramePosition, Vector3.down, 10f, groundLayer))
            {
                return false;
            }

            return true;
        }

    }
}