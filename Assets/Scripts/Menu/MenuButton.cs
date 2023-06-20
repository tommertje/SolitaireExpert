using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    GameObject menuSlider;

    Animator menuSliderAnim;

    [SerializeField] GameObject menuCloser;

    // Start is called before the first frame update
    void Start()
    {
        menuSlider = transform.GetChild(0).gameObject;
        menuSliderAnim = menuSlider.GetComponent<Animator>();
    }

    private void OnMouseDown()
    {
        OpenMenu();
    }

    public void OpenMenu()
    {
        if (menuSliderAnim.GetCurrentAnimatorStateInfo(0).IsName("SlideOut"))
        {
            menuSliderAnim.Play("SlideIn");
            menuCloser.SetActive(true);
        }
        else if (menuSliderAnim.GetCurrentAnimatorStateInfo(0).IsName("SlideIn"))
        {
            menuSliderAnim.Play("SlideOut");
            menuCloser.SetActive(false);
        }
        else //this is the first button press case; when state is the default "New State"
        {
            menuSliderAnim.Play("SlideIn");
            menuCloser.SetActive(true);
        }
    }
}
