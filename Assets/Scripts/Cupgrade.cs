using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cupgrade : MonoBehaviour
{
    [SerializeField] Craft[] crafts;
    [SerializeField] GameObject[] guns = new GameObject[2];
    [SerializeField] Transform[] places = new Transform[2];
    [SerializeField] Transform[] results = new Transform[2];
    [SerializeField] GameObject canvas;

    private void Start()
    {
        foreach (Craft craft in crafts)
        {
            craft.Setup();
        }
    }
    private void Update()
    {
        if(canvas)
        canvas.SetActive(Vector3.Distance(PlayerController.Instance.GetPosition(), transform.position) < 1.5f);
    }
    public void AddItem(GameObject itemGo)
    {
        for (int i = 0; i < guns.Length; i++)
        {
            if (guns[i] == null)
            {
                guns[i] = itemGo;
                itemGo.transform.parent = transform;
                itemGo.transform.position = places[i].position;
                if (i == guns.Length - 1)
                    Craft();
                return;
            }
        }
    }

    public void RemoveItem(GameObject item)
    {
        for (int i = 0; i < guns.Length; i++)
        {
            if (guns[i] == item)
                guns[i] = null;
        }
    }

    void Craft()
    {
        if (guns[1].name == guns[0].name)
        {
            for (int i = 0; i < guns.Length; i++)
            {
                GameObject newGun = Instantiate(guns[i], results[i].position, Quaternion.identity);
                newGun.name = guns[i].name;
                newGun.GetComponent<GunController>().DroppedDown();
            }
            ClearGO();
            return;
        }

        foreach (Craft craft in crafts)
        {
            if (craft.ingredients.Contains(guns[1].name) && craft.ingredients.Contains(guns[0].name))
            {
                GameObject newGun = Instantiate(craft.gun, results[0].position, Quaternion.identity);
                newGun.name = craft.gun.name;
                newGun.GetComponent<GunController>().DroppedDown();
                GameManager.Instance.AddWeaponCraft();
                AudioManager.Instance.Play("Anvil");
                ClearGO();
                return;
            }
        }


        //if (guns[1].GetComponent<Ingredient>().level != guns[0].GetComponent<Ingredient>().level)
        //{
        //    int gunOneLevel = guns[0].GetComponent<Ingredient>().level;
        //    int gunTwoLevel = guns[1].GetComponent<Ingredient>().level;
        //    int maxLevel = Mathf.Max(gunOneLevel, gunTwoLevel);
        //    if (maxLevel == gunTwoLevel)
        //    {
        //        GameObject newGun = Instantiate(guns[1], results[0].position, Quaternion.identity);
        //        newGun.name = guns[1].name;
        //        newGun.GetComponent<GunController>().DroppedDown();
        //        newGun.GetComponent<GunController>().Upgrade();
        //        ClearGO();
        //    }
        //    else
        //    {
        //        GameObject newGun = Instantiate(guns[0], results[0].position, Quaternion.identity);
        //        newGun.name = guns[0].name;
        //        newGun.GetComponent<GunController>().DroppedDown();
        //        newGun.GetComponent<GunController>().Upgrade();
        //        ClearGO();
        //    }
        //    return;
        //}

        // If we don't find a recipe
        for (int i = 0; i < guns.Length; i++)
        {
            GameObject newGun = Instantiate(guns[i], results[i].position, Quaternion.identity);
            newGun.name = guns[i].name;
            newGun.GetComponent<GunController>().DroppedDown();
        }
        ClearGO();
    }

    void ClearGO()
    {
        for (int i = 0; i < guns.Length; i++)
        {
            Destroy(guns[i]);
            guns[i] = null;
        }
    }
    public string CraftWithNames(string[] items)
    {
        foreach (Craft craft in crafts)
        {
            if (craft.ingredients.Contains(items[1]) && craft.ingredients.Contains(items[0]))
                return craft.gun.name;
            
        }
        return null;
    }
}

[System.Serializable]
public class Craft
{
    public List<GameObject> go;

    [HideInInspector] public List<string> ingredients = new List<string>();

    public GameObject gun;

    public void Setup()
    {
        foreach (GameObject g in go)
        {
            ingredients.Add(g.name);
        }
    }
}
