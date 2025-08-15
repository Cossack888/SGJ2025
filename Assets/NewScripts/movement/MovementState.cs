using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class MovementState2D
{
    protected readonly PlayerMovementController2D controller;
    protected readonly Rigidbody2D rb;
    protected readonly InputBindingListSO inputBindings;

    private readonly Dictionary<InputActionType, InputResponseSO> inputCache = new();
    private readonly HashSet<InputActionType> initializedInputs = new();

    protected Vector2 MoveInput => controller.CurrentMoveInput;

    public MovementState2D(PlayerMovementController2D controller)
    {
        this.controller = controller;
        this.rb = controller.Rigidbody;
        this.inputBindings = controller.InputBindings;
    }

    public virtual void Enter() { }

    public virtual void Exit()
    {
        foreach (var type in initializedInputs)
            inputCache[type]?.Cleanup();

        initializedInputs.Clear();
    }

    public virtual void Tick() { }

    public virtual void FixedTick() { }

    protected bool IsPressed(InputActionType type)
    {
        return GetInput(type)?.inputAction?.action?.IsPressed() ?? false;
    }

    protected bool IsSprinting()
    {
        return IsPressed(InputActionType.Player_Sprint);
    }

    protected float ReadSprintModifier(float sprintMultiplier = 2f)
    {
        return IsSprinting() ? sprintMultiplier : 1f;
    }

    protected void MoveX(float inputX, float baseSpeed = 5f, float sprintMultiplier = 2f)
    {
        var velocity = rb.linearVelocity;
        float finalSpeed = baseSpeed * ReadSprintModifier(sprintMultiplier);
        velocity.x = inputX * finalSpeed;
        rb.linearVelocity = velocity;
    }

    #region Input Bindings

    protected void BindStarted(InputActionType type, Action<InputAction.CallbackContext> callback)
    {
        var input = GetOrInitializeInput(type);
        if (input != null) input.OnStarted += callback;
    }

    protected void BindPerformed(InputActionType type, Action<InputAction.CallbackContext> callback)
    {
        var input = GetOrInitializeInput(type);
        if (input != null) input.OnPerformed += callback;
    }

    protected void BindCanceled(InputActionType type, Action<InputAction.CallbackContext> callback)
    {
        var input = GetOrInitializeInput(type);
        if (input != null) input.OnCanceled += callback;
    }

    protected void UnbindStarted(InputActionType type, Action<InputAction.CallbackContext> callback)
    {
        var input = GetInput(type);
        if (input != null) input.OnStarted -= callback;
    }

    protected void UnbindPerformed(InputActionType type, Action<InputAction.CallbackContext> callback)
    {
        var input = GetInput(type);
        if (input != null) input.OnPerformed -= callback;
    }

    protected void UnbindCanceled(InputActionType type, Action<InputAction.CallbackContext> callback)
    {
        var input = GetInput(type);
        if (input != null) input.OnCanceled -= callback;
    }

    #endregion

    #region Helpers

    protected InputResponseSO GetOrInitializeInput(InputActionType type)
    {
        var input = GetInput(type);
        if (input == null) return null;

        if (!initializedInputs.Contains(type))
        {
            input.Initialize();
            initializedInputs.Add(type);
        }

        return input;
    }

    protected InputResponseSO GetInput(InputActionType type)
    {
        if (inputCache.TryGetValue(type, out var cached))
            return cached;

        var found = Array.Find(inputBindings.bindings, b => b.actionType == type)?.response;
        if (found != null)
            inputCache[type] = found;

        return found;
    }

    #endregion
}
