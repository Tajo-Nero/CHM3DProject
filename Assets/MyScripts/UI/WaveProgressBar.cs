using UnityEngine;
using UnityEngine.UI;

public class WaveProgressBar : MonoBehaviour
{
    public Scrollbar scrollbar;
    public Text waveText;
    public GameObject[] waveTextures; // 텍스처 오브젝트 배열
    private int totalWaves = 12;
    private int currentWave = 0;

    void Start()
    {
        if (scrollbar != null)
        {
            UpdateScrollbar(); // 초기 스크롤바 업데이트
            UpdateWaveText();
        }

        // 모든 텍스처 비활성화
        foreach (var texture in waveTextures)
        {
            texture.SetActive(false);
        }
    }

    public void StartWave()
    {
        if (currentWave < totalWaves)
        {
            waveTextures[currentWave].SetActive(true); // 현재 웨이브 텍스처 활성화
        }
    }

    public void EndWave()
    {
        if (currentWave < totalWaves)
        {
            waveTextures[currentWave].SetActive(false); // 현재 웨이브 텍스처 비활성화
            currentWave++;
            UpdateScrollbar();
            UpdateWaveText();
        }
    }

    private void UpdateScrollbar()
    {
        if (scrollbar != null)
        {
            float value = (float)currentWave / (totalWaves - 1); // 스크롤바 값 업데이트
            scrollbar.value = value;
        }
    }

    private void UpdateWaveText()
    {
        if (waveText != null)
        {
            waveText.text = $"Wave {currentWave + 1}/{totalWaves}";
        }
    }
}
