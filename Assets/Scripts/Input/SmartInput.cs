using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

[Serializable]
public class SmartInput
{
    public InputAction Action { get; set; }

    [Range(0,1)]
    public float cachedTime = 0.2f;
    [Range(0,1)]
    public float opportunityWindowTime = 0.2f;

    private float performedTime = -100f;
    private float opportunitySignalTime = -100f;

    [Header("Callback")]
    public UnityAction onPerformed;

    private bool initialised = false;

    public void Initialise()
    {
        if (initialised) return;

        if (IsActionValid)
        {
            initialised = true;
            Action.performed += OnActionPerformed;
        }
    }

    public void Uninitialise()
    {
        if (!initialised) return;

        if (IsActionValid)
        {
            Action.performed -= OnActionPerformed;
            initialised = false;
        }
    }

    private void OnActionPerformed(InputAction.CallbackContext context)
    {
        onPerformed?.Invoke();
        performedTime = Time.unscaledTime;
    }

    // Realtime button press
    public bool IsPressedNow()
    {
        return IsActionValid && Action.IsPressed();
    }

    public bool IsJustPressed()
    {
        return IsActionValid && Action.WasPressedThisFrame();
    }

    public bool IsJustReleased()
    {
        return IsActionValid && Action.WasReleasedThisFrame();
    }


    // Cached time
    public bool IsCached()
    {
        return (Time.unscaledTime - performedTime) <= cachedTime;
    }

    public void ConsumeCache()
    {
        performedTime = -100f;
    }


    // Oportunity time
    public void SignalOpportunityWindow()
    {
        opportunitySignalTime = Time.unscaledTime;
    }

    public bool IsInsideOpportunityWindow()
    {
        return (Time.unscaledTime - opportunitySignalTime) <= opportunityWindowTime;
    }

    public void ConsumeOpportunityWindow()
    {
        opportunitySignalTime = -100f;
    }

    private bool IsActionValid => Action != null;

    public static SmartInput Create(InputAction actionReference, float cachedTime = 0.0f, float opportunityWindow = 0.0f)
    {
        SmartInput smartInput = new()
        {
            Action = actionReference,
            cachedTime = cachedTime,
            opportunityWindowTime = opportunityWindow
        };

        smartInput.Initialise();

        return smartInput;
    }


    public bool IsPressed(bool consume = true)
    {
        bool isPressed = IsJustPressed();

        bool ret = isPressed || IsCached() || (IsInsideOpportunityWindow() && isPressed);

        if(ret && consume)
        {
            ConsumeCache();
            ConsumeOpportunityWindow();
        }

        return ret;
    }

}