using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Input/Action Response")]
public class InputResponseSO : ScriptableObject
{
    public InputActionReference inputAction;

    public event Action<InputAction.CallbackContext> OnPerformed;
    public event Action<InputAction.CallbackContext> OnStarted;
    public event Action<InputAction.CallbackContext> OnCanceled;

    private bool isInitialized = false;

    public void Initialize()
    {

        if (isInitialized || inputAction == null) return;

        Debug.Log("Initializing input: " + inputAction.name);

        inputAction.action.started += HandleStarted;
        inputAction.action.performed += HandlePerformed;
        inputAction.action.canceled += HandleCanceled;

        inputAction.action.Enable();

        isInitialized = true;
    }

    public void Cleanup()
    {
        if (!isInitialized || inputAction == null) return;

        inputAction.action.started -= HandleStarted;
        inputAction.action.performed -= HandlePerformed;
        inputAction.action.canceled -= HandleCanceled;

        inputAction.action.Disable();
        isInitialized = false;
    }

    private void HandleStarted(InputAction.CallbackContext ctx)
    {
        OnStarted?.Invoke(ctx);
    }

    private void HandlePerformed(InputAction.CallbackContext ctx)
    {
        OnPerformed?.Invoke(ctx);
    }

    private void HandleCanceled(InputAction.CallbackContext ctx)
    {
        OnCanceled?.Invoke(ctx);
    }
    public void SetupFromRuntimeAsset(InputActionAsset runtimeAsset)
    {
        if (inputAction == null || runtimeAsset == null) return;
        var runtimeAction = runtimeAsset.FindAction(inputAction.action.id);
        if (runtimeAction == null)
        {
            Debug.LogWarning($"Action {inputAction.name} not found in runtime asset");
            return;
        }

        runtimeAction.started += HandleStarted;
        runtimeAction.performed += HandlePerformed;
        runtimeAction.canceled += HandleCanceled;
        runtimeAction.Enable();
        isInitialized = true;
    }

}