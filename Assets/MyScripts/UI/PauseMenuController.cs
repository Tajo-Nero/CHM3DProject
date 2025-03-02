using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour, ISubject
{
    public GameObject mainMenuPanel; // ���� �޴� �г� (��ư 4���� �ִ� �г�)
    public GameObject settingsPanel; // ���� �г� (�����̴��� �ִ� �г�)
    public GameObject[] menuButtons; // ���� �޴� �гο� �ִ� ��ư��
    public GameObject settingsMenu; // ���� �޴�
    public Slider backgroundMusicSlider; // ��� ���� ���� �����̴�
    public Slider soundEffectsSlider; // ���� ȿ�� ���� �����̴�
    public Button backButton; // �ǵ��ư��� ��ư

    private List<IObserver> observers = new List<IObserver>();

    void Start()
    {
        InitializeMenu();
        InitializeSliders();
        InitializeBackButton();
    }

    void InitializeMenu()
    {
        // �ʱ� �гΰ� ��ư ���� ��Ȱ��ȭ
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false); // ���� �г� ��Ȱ��ȭ
        foreach (GameObject button in menuButtons)
        {
            button.SetActive(false);
        }
        settingsMenu.SetActive(false); // ���� �޴� ��Ȱ��ȭ
        backgroundMusicSlider.gameObject.SetActive(false); // ��� ���� �����̴� ��Ȱ��ȭ
        soundEffectsSlider.gameObject.SetActive(false); // ���� ȿ�� �����̴� ��Ȱ��ȭ
        backButton.gameObject.SetActive(false); // �ǵ��ư��� ��ư ��Ȱ��ȭ
        gameObject.SetActive(false); // PauseMenuController ��ü ��Ȱ��ȭ
    }

    void InitializeSliders()
    {
        // �����̴� �̺�Ʈ ����
        backgroundMusicSlider.onValueChanged.AddListener(SetBackgroundMusicVolume);
        soundEffectsSlider.onValueChanged.AddListener(SetSoundEffectsVolume);
    }

    void InitializeBackButton()
    {
        // �ǵ��ư��� ��ư �̺�Ʈ ����
        backButton.onClick.AddListener(CloseSettings);
    }

    public void OpenMenu()
    {
        SetMenuActive(true);
        NotifyObservers(null, "OpenMenu"); // �����ڵ鿡�� �˸� ����
    }

    public void CloseMenu()
    {
        SetMenuActive(false);
        NotifyObservers(null, "CloseMenu"); // �����ڵ鿡�� �˸� ����
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

    // �����̴� �̺�Ʈ �ڵ鷯
    void SetBackgroundMusicVolume(float volume)
    {
        AudioManager.Instance.SetBackgroundMusicVolume(volume);
    }

    void SetSoundEffectsVolume(float volume)
    {
        AudioManager.Instance.SetSoundEffectsVolume(volume);
    }
}
