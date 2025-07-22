using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WaveProgressBar : MonoBehaviour
{
    [Header("UI ���")]
    public Text waveText; // ���� ���̺� ���� ��Ȳ (1/12)
    public Text statusText; // ���̺� ���� ǥ�� (���� ��� ��, ���� ��, �Ϸ� ��)

    [Header("���̺� ����")]
    public int totalWaves = 12;
    private int currentWave = 0;

    [Header("���� ��Ȳ")]
    private int maxEnemiesInWave = 0;
    private int remainingEnemies = 0;

    // ����
    private WaveManager waveManager;

    void Start()
    {
        // WaveManager ���� ã��
        StartCoroutine(InitializeAfterFrame());
    }

    IEnumerator InitializeAfterFrame()
    {
        // �� ������ ���
        yield return null;

        waveManager = FindObjectOfType<WaveManager>();

        if (waveManager != null)
        {
            // �̺�Ʈ ���
            waveManager.OnWaveCompleted += OnWaveCompleted;

            // �� ���̺� �� ����ȭ
            totalWaves = waveManager.GetTotalWaves();
        }

        // �ʱ�ȭ
        UpdateWaveText();
        UpdateStatusText("");
    }

    void Update()
    {
        if (waveManager != null)
        {
            // ���� ���̺� ����ȭ
            int waveManagerWave = waveManager.GetCurrentWave();

            if (waveManagerWave > 0 && waveManagerWave - 1 != currentWave)
            {
                currentWave = waveManagerWave - 1;

                if (currentWave >= 0 && currentWave < totalWaves)
                {
                    UpdateWaveText();
                }
            }

            // ���̺� ���� ��Ȳ ������Ʈ
            if (waveManager.isWaveActive && maxEnemiesInWave > 0)
            {
                float progress = waveManager.GetWaveProgress();
                UpdateProgress(progress);
            }
        }
    }

    // �÷��̾ �غ�Ǿ��� �� ȣ��
    public void OnPlayerReady()
    {
        UpdateStatusText("G Ű�� ���� ���̺� ����");
    }

    // ���̺� ���� �� ȣ��
    public void OnWaveStarted()
    {
        if (currentWave < totalWaves)
        {
            // ���� �ؽ�Ʈ ����
            UpdateStatusText("");

            // ���� ��Ȳ �ʱ�ȭ
            if (waveManager != null && currentWave < waveManager.waves.Length)
            {
                maxEnemiesInWave = waveManager.waves[currentWave].wave_enemyCount;
                remainingEnemies = maxEnemiesInWave;
            }
        }
    }

    public void OnWaveCompleted(int waveNumber)
    {
        // waveNumber�� 1���� �����ϹǷ� -1
        currentWave = waveNumber - 1;

        if (currentWave >= 0 && currentWave < totalWaves)
        {
            UpdateWaveText();

            // ���� ���̺갡 �ִٸ� ���� �ȳ�
            if (currentWave + 1 < totalWaves)
            {
                UpdateStatusText("G Ű�� ���� ���� ���̺� ����");
            }
            else
            {
                UpdateStatusText("��� ���̺� �Ϸ�!");
            }
        }
    }

    // ���̺� �ؽ�Ʈ ������Ʈ
    private void UpdateWaveText()
    {
        if (waveText != null)
        {
            waveText.text = $"Wave {currentWave + 1}/{totalWaves}";
        }
    }

    // ���� �ؽ�Ʈ ������Ʈ
    private void UpdateStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    // ���̺� �� ���� ��Ȳ ������Ʈ
    public void UpdateProgress(float progress)
    {
        // �ʿ��� ��� ���� ��Ȳ ǥ�� �߰� ����
    }

    // �ִ� �� �� ����
    public void SetMaxValue(int maxValue)
    {
        maxEnemiesInWave = maxValue;
    }

    // ���� óġ�� �� �� ����
    public void SetValue(int value)
    {
        remainingEnemies = maxEnemiesInWave - value;

        if (maxEnemiesInWave > 0)
        {
            float progress = (float)value / maxEnemiesInWave;
            UpdateProgress(progress);
        }
    }

    public void StartWave()
    {
        if (currentWave >= 0 && currentWave < totalWaves)
        {
            OnWaveStarted();
        }
    }

    // ���̺� ����
    public void EndWave()
    {
        // ����� Ư���� ó�� ����
    }

    // ����
    public void ResetProgressBar()
    {
        currentWave = 0;
        UpdateWaveText();
        UpdateStatusText("");
    }

    // ����
    private void OnDestroy()
    {
        // �̺�Ʈ ���� ����
        if (waveManager != null)
        {
            waveManager.OnWaveCompleted -= OnWaveCompleted;
        }
    }
}