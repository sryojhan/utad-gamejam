using UnityEngine;

namespace Player
{
    //TODO: a lo mejor no hace falta que sea un singleton. Permitiria ademas tener soporte para varios jugadores...

    public class PlayerController : Singleton<PlayerController>
    {
        //private PlayerStateMachine _stateMachine;
        private PlayerMovementController _movementController;
        //private PlayerCombatController _combatController;
        //private PlayerCombatTargetSelector _targetSelector;
        //private PlayerInteractor _playerInteractor;

        //private AnimationCoordinator _animationCoordinator;

        private Animator _animator;
        private StopMotionAnimator _stopMotionAnimator;


        //public static PlayerStateMachine StateMachine => instance._stateMachine;
        public static PlayerMovementController Movement => instance._movementController;
        //public static PlayerCombatController Combat => instance._combatController;
        //public static PlayerCombatTargetSelector TargetSelector => instance._targetSelector;
        //public static PlayerInteractor Interactor => instance._playerInteractor;
        public static Animator Animator => instance._animator;
        //public static StopMotionAnimator StopMotion => instance._stopMotionAnimator;

        //public static AnimationCoordinator AnimationCoordinator => instance._animationCoordinator;

        public static Vector3 Position => instance.transform.position;


        private void Awake()
        {
            EnsureInitialised();
            InitialiseReferences();
        }

        private void Start()
        {
            if (Progress.IsInitialised())
            {
                if (!string.IsNullOrEmpty(Progress.instance.last_door_id))
                {
                    Door[] roomdoors = FindObjectsByType<Door>(FindObjectsSortMode.None);
                
                    foreach(Door door in roomdoors)
                    {
                        if(door.id == Progress.instance.last_door_id)
                        {
                            transform.position = Utils.Flatten(door.transform.position);
                            return;
                        }
                    }
                }
            }    
        }

        private void InitialiseReferences()
        {
            _animator = GetComponentInChildren<Animator>();

            //_stateMachine = GetComponentInChildren<PlayerStateMachine>();
            _movementController = GetComponentInChildren<PlayerMovementController>();
            //_combatController = GetComponentInChildren<PlayerCombatController>();
            //_targetSelector = GetComponentInChildren<PlayerCombatTargetSelector>();
            //_playerInteractor = GetComponentInChildren<PlayerInteractor>();

            //_stopMotionAnimator = GetComponentInChildren<StopMotionAnimator>();
            //_animationCoordinator = GetComponentInChildren<AnimationCoordinator>();
        }

    }
}
