using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSlide : MonoBehaviour
{
    //Process touch for panel display on if the touch is less than this threshold.
    private float leftEdge = Screen.width * 0.25f;

    //Minimum swipe distance for showing/hiding the panel.
    float swipeDistance = 10f;


    float startXPos;
    bool processTouch = false;
    bool isExpanded = false;

    [SerializeField]  Animator sliderAnimator;

    void Update()
    {
        if (Input.touches.Length > 0)
            Panel(Input.GetTouch(0));
    }

    void Panel(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                //Get the start position of touch.

                startXPos = touch.position.x;
                Debug.Log(startXPos);
                //Check if we need to process this touch for showing panel.
                if (startXPos < leftEdge)
                {
                    processTouch = true;
                }
                break;
            case TouchPhase.Ended:
                if (processTouch)
                {
                    //Determine how far the finger was swiped.
                    float deltaX = touch.position.x - startXPos;


                    if (isExpanded && deltaX < (-swipeDistance))
                    {
                        isExpanded = false;
                        sliderAnimator.Play("Slide");
                    }
                    else if (!isExpanded && deltaX > swipeDistance)
                    {
                        isExpanded = true;
                        sliderAnimator.Play("Entry");
                    }

                    startXPos = 0f;
                    processTouch = false;
                }
                break;
            default:
                return;
        }
    }
}
