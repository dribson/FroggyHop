using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CustomizeType
{
    Skin,
    Hat,
    Trail
}

public class CustomizeButton : MonoBehaviour
{
    [SerializeField] Froggy Player;
    [SerializeField] GameObject CustomizePreview;
    [SerializeField] bool isUnlocked;
    [SerializeField] CustomizeType Type;
    [SerializeField] Sprite me;
    [SerializeField] ParticleSystem meParticle;
    Button b;

    private void Start()
    {
        b = GetComponent<Button>();

        CheckUpdateAvailability();
    }

    public Sprite GetImage()
    {
        return me;
    }

    public ParticleSystem GetParticle()
    {
        return meParticle;
    }

    public CustomizeType GetCustomType()
    {
        return Type;
    }

    public void CheckUpdateAvailability()
    {
        if (isUnlocked)
            b.interactable = true;
        else b.interactable = false;
    }

    public void MakePurchased()
    {

    }

    public void MakeUnPurchased()
    {

    }
}
