using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatioHandler : MonoBehaviour
{
    [SerializeField] GameObject aspectRatio16_9;
    [SerializeField] GameObject aspectRatio18_9;
    [SerializeField] GameObject aspectRatio195_9;
    [SerializeField] private float gameplayZoomMultiplier = 0.88f;
    [SerializeField] private float baseOrthographicSize = 5f;
    [SerializeField] private float widthFitReferenceAspect = 16f / 9f;
    [SerializeField] private bool fitCameraToVisibleContent = true;
    [SerializeField] private float horizontalContentPadding = 0.15f;
    [SerializeField] private float verticalContentPadding = 0.2f;

    private Camera mainCamera;
    private int lastScreenWidth;
    private int lastScreenHeight;

    void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            return;
        }

        CreateLayoutForCurrentAspect();
        ApplyCameraForCurrentAspect();
        CacheScreenSize();
    }

    void Update()
    {
        if (Screen.width == lastScreenWidth && Screen.height == lastScreenHeight)
        {
            return;
        }

        ApplyCameraForCurrentAspect();
        CacheScreenSize();
    }

    private void CreateLayoutForCurrentAspect()
    {
        float aspect = mainCamera.aspect;

        if (aspect >= 2.1f)
        {
            Instantiate(aspectRatio195_9, Vector3.zero, Quaternion.identity);
        }
        else if (aspect >= 1.9f)
        {
            Instantiate(aspectRatio18_9, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Instantiate(aspectRatio16_9, Vector3.zero, Quaternion.identity);
        }
    }

    private void ApplyCameraForCurrentAspect()
    {
        float aspect = (float)Screen.width / Screen.height;
        float widthFitMultiplier = Mathf.Max(1f, widthFitReferenceAspect / aspect);

        // Keep the board visible on narrower screens, while still filling larger displays.
        float responsiveSize = baseOrthographicSize * widthFitMultiplier * gameplayZoomMultiplier;
        float fittedSize = fitCameraToVisibleContent ? GetSizeRequiredForVisibleContent(aspect) : 0f;
        mainCamera.orthographicSize = Mathf.Max(responsiveSize, fittedSize);
    }

    private void CacheScreenSize()
    {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }

    private float GetSizeRequiredForVisibleContent(float aspect)
    {
        if (mainCamera == null)
        {
            return 0f;
        }

        Card[] cards = FindObjectsOfType<Card>();
        Bounds contentBounds = new Bounds(Vector3.zero, Vector3.zero);
        bool hasBounds = false;

        for (int i = 0; i < cards.Length; i++)
        {
            Card card = cards[i];
            if (card == null || !card.gameObject.activeInHierarchy)
            {
                continue;
            }

            Renderer renderer = card.GetComponent<Renderer>();
            if (renderer == null || !renderer.enabled || !renderer.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (!hasBounds)
            {
                contentBounds = renderer.bounds;
                hasBounds = true;
                continue;
            }

            contentBounds.Encapsulate(renderer.bounds);
        }

        if (!hasBounds)
        {
            return GetFallbackRendererFit(aspect);
        }

        float halfHeight = contentBounds.extents.y + verticalContentPadding;
        float halfWidthAsHeight = (contentBounds.extents.x + horizontalContentPadding) / Mathf.Max(0.01f, aspect);
        return Mathf.Max(halfHeight, halfWidthAsHeight);
    }

    private float GetFallbackRendererFit(float aspect)
    {
        Renderer[] renderers = FindObjectsOfType<Renderer>();
        Bounds contentBounds = new Bounds(Vector3.zero, Vector3.zero);
        bool hasBounds = false;

        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null || !renderer.enabled || !renderer.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (!hasBounds)
            {
                contentBounds = renderer.bounds;
                hasBounds = true;
                continue;
            }

            contentBounds.Encapsulate(renderer.bounds);
        }

        if (!hasBounds)
        {
            return 0f;
        }

        float halfHeight = contentBounds.extents.y + verticalContentPadding;
        float halfWidthAsHeight = (contentBounds.extents.x + horizontalContentPadding) / Mathf.Max(0.01f, aspect);
        return Mathf.Max(halfHeight, halfWidthAsHeight);
    }
}
