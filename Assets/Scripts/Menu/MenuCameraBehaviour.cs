using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        if (Camera.main.aspect < 1.7)
        {
            gameObject.GetComponent<Camera>().rect = new Rect(0.06f, 0.2f, 0.88f, 0.6f);
        }
    }
}
