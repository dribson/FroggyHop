using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    Transform Froggy;
    [SerializeField] Froggy phorg;

    [SerializeField] Sector SectorBase;
    [SerializeField] List<Sector> SectorList = new List<Sector>();
    UIController UIC;
    Vector2 SectorPos;
    float dist, closest = 0, highestY = 0, lowestY = 0, rightmostX = 0, leftmostX = 0, vertCount, horizCount;

    int tokens = 0;

    private void Start()
    {
        Screen.autorotateToPortrait = true;
        UIC = GetComponent<UIController>();
    }

    public void BeginGame()
    {
        UIC.BeginGame();
        StartCoroutine(DelayStartGame());
    }

    IEnumerator DelayStartGame()
    {
        yield return new WaitForSeconds(0.5f);
        DestroyAllSectors();
        StartingSectors();
        StartCoroutine(InitFrog());
    }

    IEnumerator InitFrog()
    {
        yield return new WaitForSeconds(Time.deltaTime);
        phorg.StartGame();
    }

    public void EndGame(float points)
    {
        ChangeTokenCount((int) (Mathf.Floor(points)/10));
        GetComponent<SavaData>().SaveGame();
        UIC.EndGame(points);
    }

    void StartingSectors()
    {
        SectorPos = new Vector2(-6, 3.4f);
        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                SectorList.Add(GenerateSector(SectorPos));
                SectorPos.x += 6;
            }
            SectorPos = new Vector2(-6, SectorPos.y + 6);
        }
    }

    void GenerateNewSector(int How2Generate)
    {
        List<Sector> tempList = new List<Sector>();
        switch (How2Generate) 
        {
            // Generate new sections up above;
            case 0:
                foreach(Sector s in SectorList)
                {
                    if (s.transform.position.y == highestY)
                        tempList.Add(GenerateSector(s.transform.position + new Vector3(0, 6f)));
                }
            break;

            // Generate new sections to the left
            case 1:
                foreach(Sector s in SectorList)
                {
                    if (s.transform.position.x == leftmostX)
                        tempList.Add(GenerateSector(s.transform.position + new Vector3(-6, 0)));
                }
                break;

            // Generate new sections to the right
            case 2:
                foreach (Sector s in SectorList)
                {
                    if (s.transform.position.x == rightmostX)
                        tempList.Add(GenerateSector(s.transform.position + new Vector3(6, 0)));
                }
                break;
        }
        foreach (Sector s in tempList)
        {
            SectorList.Add(s);
        }
    }

    public void DestroyOldSectors(int How2Destroy)
    {
        switch (How2Destroy)
        {
            // Destroy sectors from bottom
            case 0:
                for (int i = 0; i < SectorList.Count; i++)
                {
                    if (SectorList[i].transform.position.y <= phorg.transform.position.y - 8)
                    {
                        Sector temp = SectorList[i];
                        SectorList.Remove(SectorList[i]);
                        Destroy(temp.gameObject);
                        i--;
                    }
                }
                break;

            // Destroy sectors from right
            case 1:
                for(int i = 0; i < SectorList.Count; i++)
                {
                    if(SectorList[i].transform.position.x >= phorg.transform.position.x + 12)
                    {
                        Sector temp = SectorList[i];
                        SectorList.Remove(SectorList[i]);
                        Destroy(temp.gameObject);
                        i--;
                    }
                }
                break;

            // Destroy sectors from left
            case 2:
                for (int i = 0; i < SectorList.Count; i++)
                {
                    if (SectorList[i].transform.position.x <= phorg.transform.position.x - 12)
                    {
                        Sector temp = SectorList[i];
                        SectorList.Remove(SectorList[i]);
                        Destroy(temp.gameObject);
                        i--;
                    }
                }
                break;
        }
    }

    public void CheckGenNewSectors()
    {
        dist = closest = 0;
        lowestY = leftmostX = int.MaxValue;
        rightmostX = highestY = int.MinValue;
        Sector closestSector;
        foreach(Sector s in SectorList)
        {
            dist = Vector3.Distance(s.transform.position, phorg.transform.position);
            if(dist < closest)
            {
                closest = dist;
                closestSector = s;
            }
            if(s.transform.position.y <= lowestY)
            {
                lowestY = s.transform.position.y;
                horizCount++;
            }
            else if(s.transform.position.y >= highestY)
            {
                highestY = s.transform.position.y;
            }
            if(s.transform.position.x >= rightmostX)
            {
                rightmostX = s.transform.position.x;
                vertCount++;
            }
            else if(s.transform.position.x <= leftmostX)
            {
                leftmostX = s.transform.position.x;
            }
        }

        if(phorg.transform.position.y > lowestY + 8)
        {
            DestroyOldSectors(0);
            GenerateNewSector(0);
        }
        if(phorg.transform.position.x < leftmostX + 12)
        {
            DestroyOldSectors(1);
            GenerateNewSector(1);
        }
        else if(phorg.transform.position.x > rightmostX - 12)
        {
            DestroyOldSectors(2);
            GenerateNewSector(2);
        }
    }

    Sector GenerateSector(Vector3 genPos)
    {
        return (Instantiate(SectorBase, genPos, Quaternion.identity, transform));
    }

    void DestroyAllSectors()
    {
        if(SectorList.Count >= 1)
        {
            for(int i = 0; i < SectorList.Count; i++)
            {
                Sector temp = SectorList[i];
                SectorList.Remove(temp);
                Destroy(temp.gameObject);
                i--;
                if (SectorList.Count == 0) break;
            }
            SectorList.Clear();
        }
    }

    public int GetTokenCount()
    {
        return tokens;
    }

    public void ChangeTokenCount(int amount)
    {
        tokens += amount;
    }

    public void UpdateSaveData(int tokenCount)
    {
        tokens = tokenCount;
    }

    public void ResetPosition()
    {
        DestroyAllSectors();
        phorg.ResetPositions();
    }

}

