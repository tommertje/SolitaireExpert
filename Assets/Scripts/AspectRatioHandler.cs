using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatioHandler : MonoBehaviour
{
    [SerializeField] GameObject aspectRatio16_9;
    [SerializeField] GameObject aspectRatio18_9;
    [SerializeField] GameObject aspectRatio195_9;

    // Start is called before the first frame update
    void Awake()
    {
        if (Camera.main.aspect >= 2.1)
        {
            Instantiate(aspectRatio195_9, new Vector3(0, 0, 0), Quaternion.identity);
        }
        else if (Camera.main.aspect >= 1.9)
        {
            Instantiate(aspectRatio18_9, new Vector3(0, 0, 0), Quaternion.identity);
        }
        else if(Camera.main.aspect >= 1.7)
        {
            Instantiate(aspectRatio16_9, new Vector3(0, 0, 0), Quaternion.identity);
        }
        else
        {
            Camera.main.orthographicSize = 6.6f;
            Instantiate(aspectRatio16_9, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
}
