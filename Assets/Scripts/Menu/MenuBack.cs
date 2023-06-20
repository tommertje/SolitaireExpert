using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBack : MonoBehaviour
{

    [SerializeField] Animator menuSlider;
    [SerializeField] Animator highlightSlider;

    private void OnMouseDown()
    {
        highlightSlider.Play("SlideOut");
        menuSlider.Play("SlideIn");
    }
}
