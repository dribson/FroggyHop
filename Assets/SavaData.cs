using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SavaData : MonoBehaviour
{
    int tokensToSave;
    BinaryFormatter bf;
    FileStream file;
    SaveGameData data;
    [SerializeField] GameController GC;
    [SerializeField] UIController UIC;
    string SaveDataLoc, dest;

    List<bool> SkinsPurchased, HatsPurchased, TrailsPurchased;
    int SkinEquipped, HatEquipped, TrailEquipped;

    
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void OnApplicationPause(bool pause)
    {
#if UNITY_EDITOR
        // why the fuck does unity editor start paused? don't want to save there anyway
#else
        SaveGame();
#endif
    }

    private void Start()
    {
        SaveDataLoc = "FroggyData.frog";
        dest = Path.Combine(Application.persistentDataPath, SaveDataLoc);

#if UNITY_EDITOR_WIN
        dest = Application.persistentDataPath + "/" + SaveDataLoc;
#endif
        LoadGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DeleteSaveData();
        }
    }

    public void SaveGame()
    {
        try
        {
            bf = new BinaryFormatter();
            file = File.Create(dest);
            data = new SaveGameData();

            tokensToSave = GC.GetTokenCount();
            data.savedTokens = tokensToSave;

            data.Skins = UIC.GetPurchasedSkins();
            data.Hats = UIC.GetPurchasedHats();
            data.Trails = UIC.GetPurchasedTrails();

            data.SkinEquipped = UIC.GetEquippedSkin();
            data.HatEquipped = UIC.GetEquippedHat();
            data.TrailEquipped = UIC.GetEquippedTrail();

            bf.Serialize(file, data);
            file.Close();
            Debug.Log("Game Data Saved!");
        }
        catch (Exception e)
        {
            Debug.LogError("ERROR: Save Failed! | " + e);
        }
    }

    public void LoadGame()
    {
        if(File.Exists(dest))
        {
            bf = new BinaryFormatter();
            file = File.Open(dest, FileMode.Open);
            data = (SaveGameData)bf.Deserialize(file);
            file.Close();

            tokensToSave = data.savedTokens;

            SkinsPurchased = data.Skins;
            HatsPurchased = data.Hats;
            TrailsPurchased = data.Trails;

            SkinEquipped = data.SkinEquipped;
            HatEquipped = data.HatEquipped;
            TrailEquipped = data.TrailEquipped;

            GC.UpdateSaveData(tokensToSave);
            UIC.LoadPurchasedCustomize(SkinsPurchased, HatsPurchased, TrailsPurchased);
            UIC.LoadEquippedItems(SkinEquipped, HatEquipped, TrailEquipped);
        }
        else
        {
            Debug.LogWarning("No Save Data Present!");
        }
    }

    public void DeleteSaveData()
    {
        if (File.Exists(dest))
        {
            File.Delete(dest);
            tokensToSave = 0;
            CleanseLists();
            SkinEquipped = HatEquipped = TrailEquipped = 0;
            GC.UpdateSaveData(tokensToSave);
            UIC.LoadPurchasedCustomize(SkinsPurchased, HatsPurchased, TrailsPurchased);
            UIC.LoadEquippedItems(SkinEquipped, HatEquipped, TrailEquipped);
            Debug.Log("Data Erased!");
        }
        else
        {
            Debug.LogWarning("No Save Data Found!");
        }
    }

    void CleanseLists()
    {
        SkinsPurchased = UIC.GetPurchasedSkins();
        HatsPurchased = UIC.GetPurchasedHats();
        TrailsPurchased = UIC.GetPurchasedTrails();

        for (int i = 0; i < SkinsPurchased.Count; i++) { SkinsPurchased[i] = false; }
        for (int i = 0; i < HatsPurchased.Count; i++) { HatsPurchased[i] = false; }
        for (int i = 0; i < TrailsPurchased.Count; i++) { TrailsPurchased[i] = false; }
    }

}

[Serializable]
class SaveGameData
{
    public int savedTokens;
    public List<bool> Skins, Hats, Trails;
    public int SkinEquipped, HatEquipped, TrailEquipped;
}

