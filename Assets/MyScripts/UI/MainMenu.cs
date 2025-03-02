using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public Slider bgmSlider;
    public Slider sfxSlider;

    private void Start()
    {
        mainMenuPanel.SetActive(false); // ó���� ���� ���·� �����մϴ�.

        bgmSlider.value = AudioManager.Instance.backgroundMusicSource.volume;
        sfxSlider.value = AudioManager.Instance.soundEffectsSource.volume;

        bgmSlider.onValueChanged.AddListener(delegate { AdjustBGMVolume(); });
        sfxSlider.onValueChanged.AddListener(delegate { AdjustSFXVolume(); });
    }

    void Update()
    {
        Debug.Log("Update method is running"); // ����� �α� �߰�

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key pressed"); // ����� �α� �߰�
            ToggleMainMenu();
        }
    }

    private void ToggleMainMenu()
    {
        mainMenuPanel.SetActive(!mainMenuPanel.activeSelf);
        Debug.Log("Toggling main menu panel. Current state: " + mainMenuPanel.activeSelf); // ����� �α� �߰�
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
        // �����ϱ� ��ư�� ���� ������ �߰��ϼ���
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
