using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WeaponMenu : MonoBehaviour
{
    [System.Serializable]
    class Tier
    {
        public GameObject tierPage;
        public GameObject[] firstItems;
        public GameObject[] secondItems;
        public List<GameObject> results;
    }
    [SerializeField] List<string> weaponsDiscovered;
    [SerializeField] Tier[] tiers;
    [SerializeField] int tierIndex;

    [SerializeField] string[] itemsSelected = new string[2];
    [SerializeField] string result;
    
    Cupgrade cupgrade;
    private void Awake()
    {
        cupgrade = GetComponent<Cupgrade>();

        string jsonstring = PlayerPrefs.GetString("weaponsDiscovered");
        WeaponsDiscoveredJSON weaponsDiscoveredJSON = JsonUtility.FromJson<WeaponsDiscoveredJSON>(jsonstring);
        weaponsDiscovered = weaponsDiscoveredJSON.list;

        PreviousTier();
    }
    public void SelectFirstItem(int index)
    {
        itemsSelected[0] = tiers[tierIndex].firstItems[index].name;
        for (int i = 0; i < tiers[tierIndex].secondItems.Length; i++)
        {
            tiers[tierIndex].secondItems[i].SetActive(i != index);
        }
        itemsSelected[1] = null;
        foreach (GameObject resultGo in tiers[tierIndex].results)
        {
            resultGo.SetActive(false);
        }
    }
    public void SelectSecondItem(int index)
    {
        itemsSelected[1] = tiers[tierIndex].secondItems[index].name;
        result = cupgrade.CraftWithNames(itemsSelected);
        foreach (GameObject resultGo in tiers[tierIndex].results)
        {
            resultGo.SetActive(resultGo.name == result);
        }
    }
    public void NextTier()
    {
        tiers[tierIndex].tierPage.SetActive(false);
        tierIndex = Mathf.Clamp(tierIndex + 1, 0, tiers.Length-1);
        tiers[tierIndex].tierPage.SetActive(true);

        foreach (GameObject firstItem in tiers[tierIndex].firstItems)
        {
            firstItem.transform.GetChild(0).GetComponent<Image>().color = weaponsDiscovered.Contains(firstItem.name) ? Color.white : Color.black;
        }
        foreach (GameObject secondItem in tiers[tierIndex].secondItems)
        {
            secondItem.transform.GetChild(0).GetComponent<Image>().color = weaponsDiscovered.Contains(secondItem.name) ? Color.white : Color.black;
            secondItem.SetActive(false);
        }
        foreach (GameObject result in tiers[tierIndex].results)
        {
            result.transform.GetChild(0).GetComponent<Image>().color = weaponsDiscovered.Contains(result.name) ? Color.white : Color.black;
            result.SetActive(false);
        }
    }
    public void PreviousTier()
    {
        tiers[tierIndex].tierPage.SetActive(false);
        tierIndex = Mathf.Clamp(tierIndex - 1, 0, tiers.Length-1);
        tiers[tierIndex].tierPage.SetActive(true);
        foreach (GameObject firstItem in tiers[tierIndex].firstItems)
        {
            firstItem.transform.GetChild(0).GetComponent<Image>().color = weaponsDiscovered.Contains(firstItem.name) ? Color.white : Color.black;
        }
        foreach (GameObject secondItem in tiers[tierIndex].secondItems)
        {
            secondItem.transform.GetChild(0).GetComponent<Image>().color = weaponsDiscovered.Contains(secondItem.name) ? Color.white : Color.black;
            secondItem.SetActive(false);
        }
        foreach (GameObject result in tiers[tierIndex].results)
        {
            result.transform.GetChild(0).GetComponent<Image>().color = weaponsDiscovered.Contains(result.name) ? Color.white : Color.black;
            result.SetActive(false);
        }
    }
    public void BackBtn()
    {
        SceneManager.LoadScene(0);
    }
    public void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteKey("weaponsDiscovered");
    }
}
