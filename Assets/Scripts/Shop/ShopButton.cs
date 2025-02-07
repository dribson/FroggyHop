using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    [SerializeField] bool isUnlocked;
    [SerializeField] CustomizeType Type;
    [SerializeField] string Name;
    [SerializeField] int CostToPurchase;
    [SerializeField] Sprite me;
    [SerializeField] CustomizeButton custom;
    [SerializeField] string Description;
    [SerializeField] ParticleSystem TrailEffect;


    public Sprite GetImage()
    {
        return me;
    }

    public string GetName()
    {
        return Name;
    }

    public int GetCost()
    {
        return CostToPurchase;
    }

    public bool HasBeenPurchased()
    {
        return isUnlocked;
    }

    public CustomizeButton GetCustom()
    {
        return custom;
    }

    public CustomizeType GetCustType()
    {
        return Type;
    }

    public string GetDescription()
    {
        return Description;
    }

    public ParticleSystem GetTrail()
    {
        return TrailEffect;
    }

    public void MakePurchased()
    {
        isUnlocked = true;
        custom.GetComponent<Button>().interactable = true;
        GetComponent<Button>().interactable = false;
        gameObject.SetActive(false);
    }

    public void MakeUnPurchased()
    {
        isUnlocked = false;
        custom.GetComponent<Button>().interactable = false;
        GetComponent<Button>().interactable = true;
        gameObject.SetActive(true);
    }
}
