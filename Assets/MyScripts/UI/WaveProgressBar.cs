using UnityEngine;
using UnityEngine.UI;

public class WaveProgressBar : MonoBehaviour
{
    public Scrollbar scrollbar;
    public Text waveText;
    public GameObject[] waveTextures; // �ؽ�ó ������Ʈ �迭
    private int totalWaves = 12;
    private int currentWave = 0;

    void Start()
    {
        if (scrollbar != null)
        {
            UpdateScrollbar(); // �ʱ� ��ũ�ѹ� ������Ʈ
            UpdateWaveText();
        }

        // ��� �ؽ�ó ��Ȱ��ȭ
        foreach (var texture in waveTextures)
        {
            texture.SetActive(false);
        }
    }

    public void StartWave()
    {
        if (currentWave < totalWaves)
        {
            waveTextures[currentWave].SetActive(true); // ���� ���̺� �ؽ�ó Ȱ��ȭ
        }
    }

    public void EndWave()
    {
        if (currentWave < totalWaves)
        {
            waveTextures[currentWave].SetActive(false); // ���� ���̺� �ؽ�ó ��Ȱ��ȭ
            currentWave++;
            UpdateScrollbar();
            UpdateWaveText();
        }
    }

    private void UpdateScrollbar()
    {
        if (scrollbar != null)
        {
            float value = (float)currentWave / (totalWaves - 1); // ��ũ�ѹ� �� ������Ʈ
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
