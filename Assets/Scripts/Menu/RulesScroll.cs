using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class RulesScroll : MonoBehaviour
{
    Vector3 initialPosition = new Vector3(0,0,0);

    float initialClickPoint;

    private void OnMouseDown()
    {
        initialPosition = transform.localPosition;
        initialClickPoint = GetCurrentMouseZPosition();
    }

    private void OnMouseDrag()
    {
        float nextPositionY = initialPosition.y + (GetCurrentMouseZPosition() - initialClickPoint);

        if (nextPositionY >= 3.57 && nextPositionY <= 41)
        {
            transform.localPosition = new Vector3(initialPosition.x, nextPositionY, initialPosition.z);
        }
    }

    private float GetCurrentMouseZPosition()
    {
        Vector3 origPosition = Camera.main.ScreenToWorldPoint(GetPointerScreenPosition());
        return origPosition.z;
    }

    private Vector3 GetPointerScreenPosition()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();
        }

        if (Touchscreen.current != null)
        {
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }

        return Vector3.zero;
#else
        return Input.mousePosition;
#endif
    }
}
