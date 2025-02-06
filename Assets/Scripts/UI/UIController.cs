using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DigitalRuby.Tween;

public class UIController : MonoBehaviour
{

    #region Visible Variables

    [SerializeField] Froggy player;

    [Header("Canvas Groups")]

    [SerializeField] [Tooltip("UI During Gameplay")] Canvas GameplayCanvas;
    [SerializeField] [Tooltip("UI In Menu")] Canvas MenuCanvas;
    [SerializeField] [Tooltip("UI When Dead")] Canvas FailCanvas;

    [Header("Player Customization")]
    [SerializeField] SpriteRenderer PlayerSkin;
    [SerializeField] SpriteRenderer PlayerHat;
    [SerializeField] ParticleSystem PlayerTrail;

    [Header("Text Elements")]

    [SerializeField] [Tooltip("Score Total in FailCanvas")] Text scoreText;
    [SerializeField] [Tooltip("Tokens Earned in FailCanvas")] Text gameplayTokenText;
    [SerializeField] [Tooltip("Tokens Total in Shop")] Text shopCurrentTokenText;
    [SerializeField] [Tooltip("Shop Item Name When Purchasing")] Text shopTitleText;
    [SerializeField] [Tooltip("Shop Item Description When Purchasing")] Text shopDescText;
    [SerializeField] [Tooltip("Item Cost When Purchasing")] Text shopCostText;
    [SerializeField] [Tooltip("Text When Purchasing & Too Poor")] Text noMoneyText;

    [Header("Transforms")]

    [SerializeField] [Tooltip("Moves All Menus Simultaneously")] Transform Menus;
    [SerializeField] [Tooltip("Pops Up When Purchasing Something")] Transform ShopShowcase;

    [SerializeField] [Tooltip("Scaling MenuArea Transform")] RectTransform MenuArea;
    [SerializeField] [Tooltip("Scaling CustomizeArea Transform")] RectTransform CustomizeArea;
    [SerializeField] [Tooltip("Scaling ShopArea Transform")] RectTransform ShopArea;

    [Header("Images")]

    [SerializeField] [Tooltip("Skin Preview Image")] Image SkinPreview;
    [SerializeField] [Tooltip("Hat Preview Image")] Image HatPreview;
    [SerializeField] [Tooltip("Trail Preview Image")] ParticleSystem TrailPreview;
    [SerializeField] [Tooltip("Pop Up Preview When Purchasing")] Image ShopPreview;
    [SerializeField] [Tooltip("Show Equipped Froggy Skin When Showcasing Hat/Trail")] Image SecretHiddenFroggy;

    // TODO Replace this with a script that auto populates based off of the children of the gameobjects in scene
    [Header("Button Lists")]
    [SerializeField] [Tooltip("Skin Customize Viewport Content")] RectTransform SkinCustom;
    [SerializeField] [Tooltip("Hat Customize Viewport Content")] RectTransform HatCustom;
    [SerializeField] [Tooltip("Trail Customize Viewport Content")] RectTransform TrailCustom;

    [SerializeField] [Tooltip("Skin Shop Viewport Content")] RectTransform SkinShop;
    [SerializeField] [Tooltip("Hat Shop Viewport Content")] RectTransform HatShop;
    [SerializeField] [Tooltip("Trail Shop Viewport Content")] RectTransform TrailShop;

    [Header("Customize Showcases")]
    [SerializeField] [Tooltip("CustomizeArea/SkinShowcase")] RectTransform CustomSkinShowcase;
    [SerializeField] [Tooltip("CustomizeArea/HatShowcase")] RectTransform CustomHatShowcase;
    [SerializeField] [Tooltip("CustomizeArea/TrailShowcase")] RectTransform CustomTrailShowcase;

    [Header("Shop Showcases")]
    [SerializeField] [Tooltip("ShopArea/SkinShowcase")] RectTransform ShopSkinShowcase;
    [SerializeField] [Tooltip("ShopArea/HatShowcase")] RectTransform ShopHatShowcase;
    [SerializeField] [Tooltip("ShopArea/TrailShowcase")] RectTransform ShopTrailShowcase;

    #endregion
    #region Private Variables

    ShopButton loadedShopItem;

    bool allowMenuing;

    System.Action<ITween<Vector2>> TweenRight;
    System.Action<ITween<Vector2>> TweenLeft;

    Vector2 tweenStartPos, tweenEndPos;

    GameController GC;

    int equippedSkin, equippedHat, equippedTrail, screenWidth = Screen.width;

    List<CustomizeButton> SkinCustomList = new List<CustomizeButton>(), HatCustomList = new List<CustomizeButton>(), TrailCustomList = new List<CustomizeButton>();

    List<ShopButton> SkinShopList = new List<ShopButton>(), HatShopList = new List<ShopButton>(), TrailShopList = new List<ShopButton>();

    #endregion


    // Start is called before the first frame update
    void Start()
    {
        InitializeShopButtons();
        GC = GetComponent<GameController>();
        allowMenuing = true;
        GameplayCanvas.enabled = false;
        MenuCanvas.enabled = true;
        FailCanvas.enabled = false;
        noMoneyText.enabled = false;
        ShopShowcase.gameObject.SetActive(false);
        TrailPreview.Play();
        ChangeCustomTab(0);
        ChangeShopTab(0);
        UpdateUIScalePerScreen(screenWidth);
    }

    /// <summary>
    /// Initializes shop UI from buttons that are children of a gameobject instead of lists in editor
    /// </summary>
    void InitializeShopButtons()
    {
        SkinCustomList.AddRange(SkinCustom.GetComponentsInChildren<CustomizeButton>());
        HatCustomList.AddRange(HatCustom.GetComponentsInChildren<CustomizeButton>());
        TrailCustomList.AddRange(TrailCustom.GetComponentsInChildren<CustomizeButton>());
        SkinShopList.AddRange(SkinShop.GetComponentsInChildren<ShopButton>());
        HatShopList.AddRange(HatShop.GetComponentsInChildren<ShopButton>());
        TrailShopList.AddRange(TrailShop.GetComponentsInChildren<ShopButton>());
    }

    /// <summary>
    /// Update the width of the Menu UI to match the width of the screen, and adjust Customization/Shop tab positions to smoothly tween positions from updated widths
    /// </summary>
    void UpdateUIScalePerScreen(float width)
    {
        // TODO update the UI to scale appropriately with different screen widths/heights
        Vector2 ScreenUIScale = new Vector2(width, Screen.height);
        MenuArea.sizeDelta = ScreenUIScale;
        CustomizeArea.sizeDelta = ScreenUIScale;
        ShopArea.sizeDelta = ScreenUIScale;
        MenuArea.localPosition = new Vector2(0, 0);
        CustomizeArea.localPosition = new Vector2(width, 0);
        ShopArea.localPosition = new Vector2(width * 2, 0);
    }

    public void BeginGame()
    {
        GameplayCanvas.enabled = true;
        MenuCanvas.enabled = false;
        FailCanvas.enabled = false;
    }

    public void EndGame(float points)
    {
        GameplayCanvas.enabled = false;
        MenuCanvas.enabled = false;
        FailCanvas.enabled = true;
        scoreText.text = "You scored " + points.ToString("F0") + " points!";
        gameplayTokenText.text = "Earned " + (Mathf.Floor(points / 10)) + " RibBitCoins!";
        UpdateTokenCount();
    }

    public void MenuButton()
    {
        GameplayCanvas.enabled = false;
        MenuCanvas.enabled = true;
        FailCanvas.enabled = false;
    }

    public void GoToCustomizeFromMain()
    {
        if (allowMenuing)
        {
            StartCoroutine(AllowMenu(0.75f));
            TweenLeft = (t) =>
            {
                Menus.position = t.CurrentValue;
            };
            tweenStartPos = Menus.position;
            tweenEndPos = tweenStartPos - new Vector2(screenWidth, 0);
            Menus.gameObject.Tween("TweenMenuLeft", tweenStartPos, tweenEndPos, 0.75f, TweenScaleFunctions.CubicEaseInOut, TweenLeft);
            UpdateTokenCount();
            ChangeCustomTab(0);
            ChangeShopTab(0);
            TrailPreview.Play();
        }
    }

    public void GoToMain()
    {
        if (allowMenuing)
        {
            GC.ResetPosition();
            StartCoroutine(AllowMenu(0.75f));
            TweenRight = (t) =>
            {
                Menus.position = t.CurrentValue;
            };
            tweenStartPos = Menus.position;
            tweenEndPos = tweenStartPos + new Vector2(screenWidth, 0);
            Menus.gameObject.Tween("TweenMenuRight", tweenStartPos, tweenEndPos, 0.75f, TweenScaleFunctions.CubicEaseInOut, TweenRight);
            PlayerTrail.Clear();
            PlayerTrail.Stop();
            UpdateTokenCount();
        }
    }

    public void GoToShop()
    {
        if (allowMenuing)
        {
            StartCoroutine(AllowMenu(0.75f));
            TweenLeft = (t) =>
            {
                Menus.position = t.CurrentValue;
            };
            tweenStartPos = Menus.position;
            tweenEndPos = tweenStartPos - new Vector2(screenWidth, 0);
            Menus.gameObject.Tween("TweenMenuLeft", tweenStartPos, tweenEndPos, 0.75f, TweenScaleFunctions.CubicEaseInOut, TweenLeft);
            UpdateTokenCount();
        }
    }

    public void GoToCustomizeFromShop()
    {
        if (allowMenuing)
        {
            StartCoroutine(AllowMenu(0.75f));
            TweenRight = (t) =>
            {
                Menus.position = t.CurrentValue;
            };
            tweenStartPos = Menus.position;
            tweenEndPos = tweenStartPos + new Vector2(screenWidth, 0);
            Menus.gameObject.Tween("TweenMenuRight", tweenStartPos, tweenEndPos, 0.75f, TweenScaleFunctions.CubicEaseInOut, TweenRight);
            UpdateTokenCount();
        }
    }

    IEnumerator AllowMenu(float timer)
    {
        allowMenuing = false;
        yield return new WaitForSeconds(timer);
        allowMenuing = true;
    }

    public void UpdateTokenCount()
    {
        shopCurrentTokenText.text = "RibBitCoins: " + GC.GetTokenCount();
    }

    public void OpenShop(ShopButton who)
    {
        ShopShowcase.gameObject.SetActive(true);
        loadedShopItem = who;
        ShopPreview.sprite = loadedShopItem.GetImage();
        shopCostText.text = "Cost: " + loadedShopItem.GetCost();
        shopTitleText.text = loadedShopItem.GetName();
        shopDescText.text = loadedShopItem.GetDescription();
        if(who.GetCustType() != CustomizeType.Skin)
        {
            TrailPreview.Clear();
            SecretHiddenFroggy.sprite = player.GetComponent<SpriteRenderer>().sprite;
            SecretHiddenFroggy.gameObject.SetActive(true);
            //TrailPreview.Play();
            
        }
    }

    public void CloseShop()
    {
        loadedShopItem = null;
        noMoneyText.enabled = false;
        ShopShowcase.gameObject.SetActive(false);
        SecretHiddenFroggy.gameObject.SetActive(false);
    }

    public void PurchaseItem()
    {
        if(GC.GetTokenCount() >= loadedShopItem.GetCost())
        {
            GC.ChangeTokenCount(-loadedShopItem.GetCost());
            UpdateTokenCount();
            loadedShopItem.MakePurchased();
            GetComponent<SavaData>().SaveGame();
            CloseShop();
        }
        else
        {
            noMoneyText.enabled = true;
            noMoneyText.text = "Need " + (loadedShopItem.GetCost() - GC.GetTokenCount()) + " more RibBitCoins!";
        }
    }

    public void ChangeCustomize(CustomizeButton CSB)
    {
        if(CSB.GetCustomType() == CustomizeType.Skin)
        {
            PlayerSkin.sprite = CSB.GetImage();
            SkinPreview.GetComponent<Image>().sprite = CSB.GetImage();
            equippedSkin = SkinCustomList.IndexOf(CSB);
        }
        else if(CSB.GetCustomType() == CustomizeType.Hat)
        {
            PlayerHat.sprite = CSB.GetImage();
            HatPreview.GetComponent<Image>().sprite = CSB.GetImage();
            equippedHat = HatCustomList.IndexOf(CSB);
        }
        else if(CSB.GetCustomType() == CustomizeType.Trail)
        {
            ParticleSystem tempPS = CSB.GetParticle();
            UpdateTrailVariables(tempPS);
            PlayerTrail.Clear();
            TrailPreview.Clear();
            equippedTrail = TrailCustomList.IndexOf(CSB);
        }
    }

    public void LoadPurchasedCustomize(List<bool> Skins, List<bool> Hats, List<bool> Trails)
    {
        int iterator = 0;
        foreach (bool b in Skins)
        {
            if (b)
            {
                SkinShopList[iterator].MakePurchased();
                SkinCustomList[iterator].MakePurchased();
            }
            else
            {
                SkinShopList[iterator].MakeUnPurchased();
                SkinCustomList[iterator].MakeUnPurchased();
            }
            iterator++;
        }
        iterator = 0;
        foreach (bool b in Hats)
        {
            iterator = 0;
            if (b)
            {
                HatShopList[iterator].MakePurchased();
                HatCustomList[iterator].MakePurchased();
            }
            else
            {
                HatShopList[iterator].MakeUnPurchased();
                HatCustomList[iterator].MakeUnPurchased();
            }
            iterator++;
        }
        iterator = 0;
        foreach (bool b in Trails)
        {
            iterator = 0;
            if (b)
            {
                TrailShopList[iterator].MakePurchased();
                TrailCustomList[iterator].MakePurchased();
            }
            else
            {
                TrailShopList[iterator].MakeUnPurchased();
                TrailCustomList[iterator].MakeUnPurchased();
            }
            iterator++;
        }
    }

    public void LoadEquippedItems(int Skin, int Hat, int Trail)
    {
        if (Skin > 0)
        {
            if (SkinShopList[Skin - 1].HasBeenPurchased())
            {
                ChangeCustomize(SkinCustomList[Skin]);
            }
        }
        else if(SkinCustomList.Count > 0) ChangeCustomize(SkinCustomList[0]);

        if (Hat > 0)
        {
            ChangeCustomize(HatCustomList[Hat]);
        }
        else if (HatCustomList.Count > 0) ChangeCustomize(HatCustomList[0]);

        if (Trail > 0)
        {
            ChangeCustomize(TrailCustomList[Trail]);
        }
        else if (TrailCustomList.Count > 0)ChangeCustomize(TrailCustomList[0]);
        PlayerTrail.Clear();
        PlayerTrail.Stop();
        TrailPreview.Clear();
        TrailPreview.Stop();
    }

    public List<bool> GetPurchasedSkins()
    {
        List<bool> P_Skins = new List<bool>();

        foreach(ShopButton s in SkinShopList)
        {
            P_Skins.Add(s.HasBeenPurchased());
        }

        return P_Skins;
    }
    public List<bool> GetPurchasedHats()
    {
        List<bool> P_Hats = new List<bool>();

        foreach (ShopButton s in HatShopList)
        {
            P_Hats.Add(s.HasBeenPurchased());
        }

        return P_Hats;
    }
    public List<bool> GetPurchasedTrails()
    {
        List<bool> P_Trails = new List<bool>();

        foreach (ShopButton s in TrailShopList)
        {
            P_Trails.Add(s.HasBeenPurchased());
        }

        return P_Trails;
    }

    public int GetEquippedSkin()
    {
        return equippedSkin;
    }
    public int GetEquippedHat()
    {
        return equippedHat;
    }
    public int GetEquippedTrail()
    {
        return equippedTrail;
    }

    public void ChangeCustomTab(int which)
    {
        switch (which)
        {
            case 0:
                CustomSkinShowcase.gameObject.SetActive(true);
                CustomHatShowcase.gameObject.SetActive(false);
                CustomTrailShowcase.gameObject.SetActive(false);
                break;
            case 1:
                CustomSkinShowcase.gameObject.SetActive(false);
                CustomHatShowcase.gameObject.SetActive(true);
                CustomTrailShowcase.gameObject.SetActive(false);
                break;
            case 2:
                CustomSkinShowcase.gameObject.SetActive(false);
                CustomHatShowcase.gameObject.SetActive(false);
                CustomTrailShowcase.gameObject.SetActive(true);
                break;
            default: break;
        }
    }

    public void ChangeShopTab(int which)
    {
        switch (which)
        {
            case 0:
                ShopSkinShowcase.gameObject.SetActive(true);
                ShopHatShowcase.gameObject.SetActive(false);
                ShopTrailShowcase.gameObject.SetActive(false);
                break;
            case 1:
                ShopSkinShowcase.gameObject.SetActive(false);
                ShopHatShowcase.gameObject.SetActive(true);
                ShopTrailShowcase.gameObject.SetActive(false);
                break;
            case 2:
                ShopSkinShowcase.gameObject.SetActive(false);
                ShopHatShowcase.gameObject.SetActive(false);
                ShopTrailShowcase.gameObject.SetActive(true);
                break;
            default: break;
        }
    }

    void UpdateTrailVariables(ParticleSystem tempPS)
    {
        // TODO pass in generic particle system so don't have to copy code here
        // TODO because unity particle systems FUCKIGN SUCK in code have to MANUALLY edit EACH PART of EACH NFUCKING COMPNENT I HATE THIS
        // Manual changes needed, confirm changes for new trails:
        //  Emission - Rate over Time
        //  Shape - Radius, Arc, Rotation
        //  Size over Liftime - Size
        //  Renderer - Shared Material

        ParticleSystem.EmissionModule playerEmission = PlayerTrail.emission, customEmission = TrailPreview.emission;
        playerEmission.rateOverTime = tempPS.emission.rateOverTime;
        customEmission.rateOverTime = tempPS.emission.rateOverTime;
        ParticleSystem.ShapeModule playerShape = PlayerTrail.shape, customShape = TrailPreview.shape;
        playerShape.radius = tempPS.shape.radius;
        playerShape.arc = tempPS.shape.arc;
        playerShape.rotation = tempPS.shape.rotation;
        customShape.radius = tempPS.shape.radius;
        customShape.arc = tempPS.shape.arc;
        customShape.rotation = tempPS.shape.rotation;
        ParticleSystem.SizeOverLifetimeModule playerSizeOverLifetime = PlayerTrail.sizeOverLifetime, customSizeOverLifetime = TrailPreview.sizeOverLifetime;
        playerSizeOverLifetime.size = tempPS.sizeOverLifetime.size;
        customSizeOverLifetime.size = tempPS.sizeOverLifetime.size;
        ParticleSystemRenderer playerRenderer = PlayerTrail.GetComponent<ParticleSystemRenderer>(), customRenderer = TrailPreview.GetComponent<ParticleSystemRenderer>();
        playerRenderer.sharedMaterial = tempPS.GetComponent<ParticleSystemRenderer>().sharedMaterial;
        customRenderer.sharedMaterial = tempPS.GetComponent<ParticleSystemRenderer>().sharedMaterial;
    }
}
