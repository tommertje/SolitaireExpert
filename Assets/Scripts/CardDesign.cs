using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDesign : MonoBehaviour
{
    [SerializeField] List<Material> cardDesigns;
    [SerializeField] private int deckNumber = 0;

    private Transform child;
    private MeshRenderer cardMesh;
    
    private void Start()
    {
        child = gameObject.transform.GetChild(0);
        cardMesh = child.Find("pPlane2").GetComponent<MeshRenderer>();
        cardMesh.material = cardDesigns[deckNumber];
    }

    public void ChangeDesign(int designNumber)
    {
        cardMesh.material = cardDesigns[designNumber];
    }
}
