using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputController : Singleton<InputController>
{
    private InputMap inputMap;

    [SerializeField]
    private SmartInput southButton;
    [SerializeField]
    private SmartInput northButton;
    [SerializeField]
    private SmartInput eastButton;
    [SerializeField]
    private SmartInput westButton;


    public static SmartInput South => instance.southButton;
    public static SmartInput North => instance.northButton;
    public static SmartInput East => instance.eastButton;
    public static SmartInput West => instance.westButton;


    [SerializeField]
    private Rumble rumble;
    public static Rumble Rumble => instance.rumble;


    private void Awake()
    {
        EnsureInitialised();

        inputMap = new();

        southButton.Action = inputMap.ControllerLayout.South;
        eastButton.Action = inputMap.ControllerLayout.East;
        westButton.Action = inputMap.ControllerLayout.West;
        northButton.Action = inputMap.ControllerLayout.North;
    }

    private void InitialiseSmartInput()
    {
        southButton.Initialise();
        eastButton.Initialise();
        westButton.Initialise();
        northButton.Initialise();
    }

    private void UninitialiseSmartInput()
    {
        southButton.Uninitialise();
        eastButton.Uninitialise();
        westButton.Uninitialise();
        northButton.Uninitialise();
    }


    private void OnEnable()
    {
        inputMap.Enable();
        InitialiseSmartInput();
    }

    private void OnDisable()
    {
        UninitialiseSmartInput();
        inputMap.Disable();
    }


    public static Vector2 LeftStick => instance.inputMap.ControllerLayout.LeftStick.ReadValue<Vector2>();
    public static Vector2 RightStick => instance.inputMap.ControllerLayout.RightStick.ReadValue<Vector2>();


    public static InputMap Map => instance.inputMap;


    // Smart input
    //TODO: hacer una clase para gestionar todo el rumble facilmente
    //TODO: hacer un mapa facilmente accesible de smartInputs


    //public static void RumbleAdvanced(float duration, AnimationCurve low, AnimationCurve high)
    //{
    //    if (Gamepad.current == null) return;

    //    Motion.Lerp((i) =>
    //    {
    //        Gamepad.current.SetMotorSpeeds(low.Evaluate(i), high.Evaluate(i));

    //    }).EndCallback(() =>
    //    {
    //        Gamepad.current.SetMotorSpeeds(0, 0);
    //    }).Duration(duration).Play();
    //}

    public static void StopRumble()
    {

    }

}
