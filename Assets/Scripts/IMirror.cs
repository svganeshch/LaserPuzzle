using UnityEngine;
using UnityEngine.InputSystem;

public interface IMirror
{
    void Hold(Transform holdPoint);
    void Release();
    void Rotate(InputAction.CallbackContext ctx);
    void RotateComplete(InputAction.CallbackContext ctx);
}
