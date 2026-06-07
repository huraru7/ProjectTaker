using UnityEngine;
using UnityEngine.InputSystem;

public class DebugController : MonoBehaviour
{
    [SerializeField] private TankModuleManager tankModuleManager;

    void Update()
    {
        if (Keyboard.current.mKey.wasPressedThisFrame)
            tankModuleManager.ModuleEarn();
    }
}
