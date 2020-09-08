using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("IN Game UI")]
    [SerializeField] TextMeshProUGUI weaponName;
    [SerializeField] TextMeshProUGUI magazineSize;
    [SerializeField] Image weaponImage;
    [SerializeField] Transform bulletHolder;
    [SerializeField] GameObject bulletImage;
    [SerializeField] Image reloadingFill;
    [SerializeField] List<GameObject> healthFill;
    [SerializeField] List<GameObject> bullets;
    [SerializeField] TextMeshProUGUI timer;
    [SerializeField] TextMeshProUGUI msTimer;
    [SerializeField] TextMeshProUGUI enemyCounter;
    [SerializeField] Gradient gradient;

    [Header("End UI")]
    [SerializeField] GameObject endScreen;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI endTimer;
    [SerializeField] TextMeshProUGUI roomCounter;
    [SerializeField] TextMeshProUGUI killCounter;
    [SerializeField] TextMeshProUGUI weaponsCounter;
    [SerializeField] TextMeshProUGUI chestCounter;
    [SerializeField] TextMeshProUGUI healthKitCounter;

    [Header("Pause Screen")]
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject settingsScreen;


    public static UIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SetWeaponUI(string weaponNameText, int projectileLeft,int magazineSizeNumber, Sprite weaponImageSprite)
    {
        weaponName.text = weaponNameText;
        magazineSize.text = "/" + magazineSizeNumber;
        weaponImage.sprite = weaponImageSprite;

        SetBulletsCount(projectileLeft);
    }
    void SetBulletsCount(int projectileLeft)
    {
        for (int i = 0; i < bullets.Count; i++)
        {
            bullets[i].SetActive(i < projectileLeft);
        }
    }

    public void SetBulletsCount(int projectileLeft, int magazineSize)
    {
        for (int i = 0; i < magazineSize; i++)
        {
            bullets[i].SetActive(i < projectileLeft);
        }
    }
    public void SetHealthBar(int health, int maxHealth)
    {
        float amount = (float)health / (float)maxHealth;
        for (int i = 0; i < healthFill.Count; i++)
        {
            healthFill[i].SetActive(i < health);
            healthFill[i].GetComponent<Image>().color = gradient.Evaluate(amount);
        }
    }
    public void SetReloadingBar(float progress)
    {
        reloadingFill.fillAmount = progress;
    }
    public void SetEnemyCounter(int enemyNumber)
    {
        enemyCounter.text = enemyNumber + " Left";
    }
    public void SetTimer(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        float milliSeconds = (time % 1) * 1000;
        timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        msTimer.text = string.Format("{0:000}", milliSeconds);
    }

    public void ResartLevel()
    {
        endScreen.SetActive(false);
        GameManager.Instance.RestartLevel();
    }
    public void GoToMainMenu()
    {
        endScreen.SetActive(false);
        GameManager.Instance.GoToMainMenu();
    }

    public void SetEndScreen(float timer, int room, int kills, int weapons, int chest, int healthKit)
    {
        float minutes = Mathf.FloorToInt(timer / 60);
        float seconds = Mathf.FloorToInt(timer % 60);
        endTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        roomCounter.text = "Room . . " + room;
        killCounter.text = "Enemies killed . . " + kills;
        weaponsCounter.text = "Weapons crafted . . " + weapons;
        chestCounter.text = "Chest opened . . " + chest;
        healthKitCounter.text = "Health kit used . . " + healthKit;

        endScreen.SetActive(true);
    }

    public void TogglePauseScreen()
    {
        pauseScreen.SetActive(GameManager.gamePaused);
        settingsScreen.SetActive(false);
    }
}
