using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCloser : MonoBehaviour
{
    [SerializeField] GameObject menuSlider;

    [SerializeField] GameObject highlightSlider;

    [SerializeField] GameObject menuCloseArea;

    Animator menuSliderAnim;
    Animator highlightSliderAnim;

    // Start is called before the first frame update
    void Start()
    {
        menuSliderAnim = menuSlider.GetComponent<Animator>();
        highlightSliderAnim = highlightSlider.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        if (menuSliderAnim.GetCurrentAnimatorStateInfo(0).IsName("SlideIn"))
        {
            menuSliderAnim.Play("SlideOut");
            menuCloseArea.SetActive(false);
        }
        else if (highlightSliderAnim.GetCurrentAnimatorStateInfo(0).IsName("SlideIn"))
        {
            highlightSliderAnim.Play("SlideOut");
            menuCloseArea.SetActive(false);
            
        }
    }    
}
