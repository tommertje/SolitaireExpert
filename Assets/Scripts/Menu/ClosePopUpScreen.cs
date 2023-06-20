using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosePopUpScreen : MonoBehaviour
{
    [SerializeField] GameObject popUpScreen;
    [SerializeField] GameObject menuCamera;

    private void OnMouseDown()
    {
        for (int i = 1; i < popUpScreen.transform.childCount - 2; i++)
        {
            var child = popUpScreen.transform.GetChild(i).gameObject;
            if (child != null)
                child.SetActive(false);
        }

        popUpScreen.SetActive(false);
        menuCamera.SetActive(false);
    }
}
