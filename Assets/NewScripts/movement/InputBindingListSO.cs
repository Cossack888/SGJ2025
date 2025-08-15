using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Input/Input Binding List")]
public class InputBindingListSO : ScriptableObject
{
    [Serializable]
    public class BindingEntry
    {
        public InputActionType actionType;
        public InputResponseSO response;
    }

    public BindingEntry[] bindings;
    public InputResponseSO GetResponse(InputActionType actionType)
    {
        foreach (var binding in bindings)
        {
            if (binding.actionType == actionType)
                return binding.response;
        }

        return null;
    }
}