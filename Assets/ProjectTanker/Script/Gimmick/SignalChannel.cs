using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Gimmick/SignalChannel")]
public class SignalChannel : ScriptableObject
{
    public bool IsOn { get; private set; }
    public event Action<bool> OnSignalChanged;

    public void Send(bool value)
    {
        IsOn = value;
        OnSignalChanged?.Invoke(value);
    }

    private void OnDisable() => IsOn = false;
}
