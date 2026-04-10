using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraBehaviour : MonoBehaviour
{
    [SerializeField] private Rect cameraViewport = new Rect(0f, 0f, 1f, 1f);

    // Start is called before the first frame update
    void Awake()
    {
        // Keep a tiny safe margin while maximizing visible play area on all screens.
        gameObject.GetComponent<Camera>().rect = cameraViewport;
    }
}
