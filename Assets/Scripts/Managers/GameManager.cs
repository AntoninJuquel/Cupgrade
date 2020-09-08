using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class WeaponsDiscoveredJSON
{
    public List<string> list;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] GameObject crosshair;
    [SerializeField] List<Level> levels;
    GameObject endLevel;
    Vector3 spawnPointPosition;
    List<GameObject> enemies = new List<GameObject>();
    int totalEnemyNumber;
    public static bool gamePaused;
    float timer;
    int roomCounter, killCounter, weaponsCounter, chestCounter, healthKitCounter, previousPreset;

    List<string> weaponsDiscovered;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
        Cursor.visible = false;
        crosshair.SetActive(true);

        string jsonstring = PlayerPrefs.GetString("weaponsDiscovered");
        WeaponsDiscoveredJSON weaponsDiscoveredJSON = JsonUtility.FromJson<WeaponsDiscoveredJSON>(jsonstring);
        weaponsDiscovered = weaponsDiscoveredJSON.list;
    }
    private void Start()
    {
        LevelGenerator.Instance.LoadLevel();
    }
    private void Update()
    {
        timer += Time.deltaTime;
        UIManager.Instance.SetTimer(timer);

        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }
    public void SetSpawnPointPosition(Vector3 position)
    {
        spawnPointPosition = position;
        GameObject.FindGameObjectWithTag("Player").transform.position = position;
    }
    public void SetEndLevelGO(GameObject _endLevel)
    {
        endLevel = _endLevel;
        endLevel.SetActive(false);
    }

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
        totalEnemyNumber = enemies.Count;
        UIManager.Instance.SetEnemyCounter(totalEnemyNumber);
    }
    public void RemoveEnemy(GameObject enemy, bool killedByPlayer)
    {
        if (enemies.Count > 0)
        {
            enemies.Remove(enemy);
            UIManager.Instance.SetEnemyCounter(enemies.Count);
            if (killedByPlayer)
                killCounter++;
            if (enemies.Count == 0)
            {
                endLevel.SetActive(true);
            }
        }
    }
    public void AddWeaponCraft()
    {
        weaponsCounter++;
    }
    public void AddChestOpened()
    {
        chestCounter++;
    }
    public void AddKitUsed()
    {
        healthKitCounter++;
    }
    public void EndScreen()
    {
        UIManager.Instance.SetEndScreen(timer, roomCounter, killCounter, weaponsCounter, chestCounter, healthKitCounter);
        Time.timeScale = 0f;
        Cursor.visible = true;
        crosshair.SetActive(false);

        WeaponsDiscoveredJSON weaponsDiscoveredJSON = new WeaponsDiscoveredJSON { list = weaponsDiscovered };
        string json = JsonUtility.ToJson(weaponsDiscoveredJSON);
        PlayerPrefs.SetString("weaponsDiscovered", json);
        PlayerPrefs.Save();
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1f;

        crosshair.SetActive(true);
        Cursor.visible = false;

        timer = 0;
        roomCounter = 0;
        killCounter = 0;
        weaponsCounter = 0;
        chestCounter = 0;
        healthKitCounter = 0;

        Destroy(PlayerController.Instance.gameObject);
        Destroy(Camera.main.transform.parent.gameObject);
        Destroy(gameObject);
    }
    public void FinishLevel()
    {
        
        roomCounter++;
        int presetIndex = UnityEngine.Random.Range(0, levels.Count);
        if(presetIndex == previousPreset)
        {
            if (presetIndex == levels.Count - 1)
                presetIndex = 0;
            else
                presetIndex++;
        }
        LevelGenerator.Instance.LoadLevel(levels[presetIndex]);
        previousPreset = presetIndex;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
        Cursor.visible = true;
        crosshair.SetActive(false);

        Destroy(PlayerController.Instance.gameObject);
        Destroy(Camera.main.transform.parent.gameObject);
        Destroy(gameObject);
    }

    public void TogglePause()
    {
        gamePaused = !gamePaused;

        if (gamePaused)
            AudioManager.Instance.Transition("Theme", "Theme_waffled");
        else
            AudioManager.Instance.Transition("Theme_waffled", "Theme");

        Time.timeScale = gamePaused ? 0f : 1f;
        UIManager.Instance.TogglePauseScreen();

        Cursor.visible = gamePaused;
        crosshair.SetActive(!gamePaused);
    }
    public void AddWeapon(string weaponName)
    {
        if (!weaponsDiscovered.Contains(weaponName))
        {
            weaponsDiscovered.Add(weaponName);
        }
    }
    public void GenerationComplete()
    {
        if(enemies.Count == 0)
        {
            endLevel.SetActive(true);
            UIManager.Instance.SetEnemyCounter(enemies.Count);
        }

    }
}
