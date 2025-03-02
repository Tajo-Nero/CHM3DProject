using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public Slider bgmSlider;
    public Slider sfxSlider;

    private void Start()
    {
        mainMenuPanel.SetActive(false); // 처음에 꺼진 상태로 시작합니다.

        bgmSlider.value = AudioManager.Instance.backgroundMusicSource.volume;
        sfxSlider.value = AudioManager.Instance.soundEffectsSource.volume;

        bgmSlider.onValueChanged.AddListener(delegate { AdjustBGMVolume(); });
        sfxSlider.onValueChanged.AddListener(delegate { AdjustSFXVolume(); });
    }

    void Update()
    {
        Debug.Log("Update method is running"); // 디버그 로그 추가

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key pressed"); // 디버그 로그 추가
            ToggleMainMenu();
        }
    }

    private void ToggleMainMenu()
    {
        mainMenuPanel.SetActive(!mainMenuPanel.activeSelf);
        Debug.Log("Toggling main menu panel. Current state: " + mainMenuPanel.activeSelf); // 디버그 로그 추가
    }

    public void OnReturnButton()
    {
        mainMenuPanel.SetActive(false);
    }

    public void OnOptionsButton()
    {
        mainMenuPanel.SetActive(false);
    }

    public void OnGiveUpButton()
    {
        // 포기하기 버튼에 대한 로직을 추가하세요
    }

    public void OnExitGameButton()
    {
        Application.Quit();
    }

    private void AdjustBGMVolume()
    {
        AudioManager.Instance.SetBackgroundMusicVolume(bgmSlider.value);
    }

    private void AdjustSFXVolume()
    {
        AudioManager.Instance.SetSoundEffectsVolume(sfxSlider.value);
    }
}
