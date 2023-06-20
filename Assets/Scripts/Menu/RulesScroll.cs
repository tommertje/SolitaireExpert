using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Vector3 origPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return origPosition.z;
    }
}
