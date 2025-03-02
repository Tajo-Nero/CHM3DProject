using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour, ISubject
{
    public GameObject mainMenuPanel; // 메인 메뉴 패널 (버튼 4개가 있는 패널)
    public GameObject settingsPanel; // 설정 패널 (슬라이더가 있는 패널)
    public GameObject[] menuButtons; // 메인 메뉴 패널에 있는 버튼들
    public GameObject settingsMenu; // 설정 메뉴
    public Slider backgroundMusicSlider; // 배경 음악 볼륨 슬라이더
    public Slider soundEffectsSlider; // 사운드 효과 볼륨 슬라이더
    public Button backButton; // 되돌아가기 버튼

    private List<IObserver> observers = new List<IObserver>();

    void Start()
    {
        InitializeMenu();
        InitializeSliders();
        InitializeBackButton();
    }

    void InitializeMenu()
    {
        // 초기 패널과 버튼 상태 비활성화
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false); // 설정 패널 비활성화
        foreach (GameObject button in menuButtons)
        {
            button.SetActive(false);
        }
        settingsMenu.SetActive(false); // 설정 메뉴 비활성화
        backgroundMusicSlider.gameObject.SetActive(false); // 배경 음악 슬라이더 비활성화
        soundEffectsSlider.gameObject.SetActive(false); // 사운드 효과 슬라이더 비활성화
        backButton.gameObject.SetActive(false); // 되돌아가기 버튼 비활성화
        gameObject.SetActive(false); // PauseMenuController 자체 비활성화
    }

    void InitializeSliders()
    {
        // 슬라이더 이벤트 설정
        backgroundMusicSlider.onValueChanged.AddListener(SetBackgroundMusicVolume);
        soundEffectsSlider.onValueChanged.AddListener(SetSoundEffectsVolume);
    }

    void InitializeBackButton()
    {
        // 되돌아가기 버튼 이벤트 설정
        backButton.onClick.AddListener(CloseSettings);
    }

    public void OpenMenu()
    {
        SetMenuActive(true);
        NotifyObservers(null, "OpenMenu"); // 관찰자들에게 알림 전송
    }

    public void CloseMenu()
    {
        SetMenuActive(false);
        NotifyObservers(null, "CloseMenu"); // 관찰자들에게 알림 전송
    }

    void SetMenuActive(bool isActive)
    {
        mainMenuPanel.SetActive(isActive);
        settingsPanel.SetActive(!isActive);
        foreach (GameObject button in menuButtons)
        {
            button.SetActive(isActive);
        }
        gameObject.SetActive(isActive);
    }

    public void OpenSettings()
    {
        SetMenuActive(false);
        SetSettingsActive(true);
    }

    public void CloseSettings()
    {
        SetSettingsActive(false);
        SetMenuActive(true);
    }

    void SetSettingsActive(bool isActive)
    {
        settingsPanel.SetActive(isActive);
        settingsMenu.SetActive(isActive);
        backgroundMusicSlider.gameObject.SetActive(isActive);
        soundEffectsSlider.gameObject.SetActive(isActive);
        backButton.gameObject.SetActive(isActive);
    }

    public void AddObserver(IObserver observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void NotifyObservers(GameObject obj, string eventMessage)
    {
        foreach (var observer in observers)
        {
            observer.OnNotify(obj, eventMessage);
        }
    }

    // 슬라이더 이벤트 핸들러
    void SetBackgroundMusicVolume(float volume)
    {
        AudioManager.Instance.SetBackgroundMusicVolume(volume);
    }

    void SetSoundEffectsVolume(float volume)
    {
        AudioManager.Instance.SetSoundEffectsVolume(volume);
    }
}
