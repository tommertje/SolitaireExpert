using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

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
#if ENABLE_INPUT_SYSTEM
        if (Touchscreen.current == null)
        {
            return;
        }

        var primaryTouch = Touchscreen.current.primaryTouch;
        var phase = primaryTouch.phase.ReadValue();

        if (phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            HandleTouchStart(primaryTouch.position.ReadValue().x);
        }
        else if (phase == UnityEngine.InputSystem.TouchPhase.Ended || phase == UnityEngine.InputSystem.TouchPhase.Canceled)
        {
            HandleTouchEnd(primaryTouch.position.ReadValue().x);
        }
#else
        if (Input.touches.Length > 0)
            Panel(Input.GetTouch(0));
#endif
    }

    void Panel(Touch touch)
    {
        switch (touch.phase)
        {
            case UnityEngine.TouchPhase.Began:
                //Get the start position of touch.

                startXPos = touch.position.x;
                Debug.Log(startXPos);
                //Check if we need to process this touch for showing panel.
                if (startXPos < leftEdge)
                {
                    processTouch = true;
                }
                break;
            case UnityEngine.TouchPhase.Ended:
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

    private void HandleTouchStart(float xPosition)
    {
        startXPos = xPosition;
        Debug.Log(startXPos);
        processTouch = startXPos < leftEdge;
    }

    private void HandleTouchEnd(float xPosition)
    {
        if (!processTouch)
        {
            return;
        }

        float deltaX = xPosition - startXPos;

        if (isExpanded && deltaX < -swipeDistance)
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
}
