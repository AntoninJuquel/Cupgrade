using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;

    private void Awake()
    {
        string jsonstring = PlayerPrefs.GetString("weaponsDiscovered");
        WeaponsDiscoveredJSON weaponsDiscoveredJSON = JsonUtility.FromJson<WeaponsDiscoveredJSON>(jsonstring);
        if (weaponsDiscoveredJSON == null)
        {
            WeaponsDiscoveredJSON weaponsDiscoveredJSONn = new WeaponsDiscoveredJSON { list = new List<string>() { "Pistol" } };
            string json = JsonUtility.ToJson(weaponsDiscoveredJSONn);
            PlayerPrefs.SetString("weaponsDiscovered", json);
            PlayerPrefs.Save();
        }
    }
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Time.timeScale = 1f;
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void SetMasterVolume(float amount)
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(amount) * 20);
    }
    public void SetMusicVolume(float amount)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(amount) * 20);
    }
    public void SetEffectsVolume(float amount)
    {
        mixer.SetFloat("EffectsVolume", Mathf.Log10(amount) * 20);
    }
    public void ToggleFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
    }
    public void GoToCraftScreen()
    {
        SceneManager.LoadScene(2);
    }
    public void GoToBestScoreScreen()
    {
        SceneManager.LoadScene(3);
    }
}
