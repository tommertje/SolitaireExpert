using UnityEngine;

public class MenuButton : MonoBehaviour
{
    GameObject menuSlider;
    Animator menuSliderAnim;
    [SerializeField] GameObject menuCloser;

    void Start()
    {
        menuSlider = transform.GetChild(0).gameObject;
        menuSliderAnim = menuSlider.GetComponent<Animator>();
        menuCloser.SetActive(false);
        menuSlider.SetActive(false);
    }

    private void OnMouseDown()
    {
        OpenMenu();
    }

    public void OpenMenu()
    {
        bool menuIsVisible = menuCloser.activeSelf;
        if (!menuIsVisible)
        {
            if (!menuSlider.activeSelf)
            {
                menuSlider.SetActive(true);
            }
            menuSliderAnim.Play("SlideIn", 0, 0f);
            menuCloser.SetActive(true);
        }
        else
        {
            menuSliderAnim.Play("SlideOut", 0, 0f);
            menuCloser.SetActive(false);
        }
    }
}
