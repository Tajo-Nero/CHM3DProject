using UnityEngine;
using UnityEngine.UI;

public class WaveProgressBar : MonoBehaviour
{
    [Header("UI ���")]
    public Scrollbar scrollbar;
    public Text waveText;
    public GameObject[] waveTextures; // �ؽ�ó ������Ʈ �迭

    [Header("���̺� ����")]
    public int totalWaves = 12;
    private int currentWave = 0;

    [Header("���� ��Ȳ")]
    private int maxEnemiesInWave = 0;
    private int remainingEnemies = 0;

    [Header("���� �ؽ�Ʈ")]
    public Text statusText; // ���̺� ���� ǥ�� (���� ��� ��, ���� ��, �Ϸ� ��)

    // ����
    private WaveManager waveManager;

    void Start()
    {
        // WaveManager ���� ã��
        waveManager = FindObjectOfType<WaveManager>();

        if (waveManager != null)
        {
            // �̺�Ʈ ���
            waveManager.OnWaveCompleted += OnWaveCompleted;

            // �� ���̺� �� ����ȭ
            totalWaves = waveManager.GetTotalWaves();
        }

        // �ʱ�ȭ
        if (scrollbar != null)
        {
            scrollbar.value = 0;
            UpdateWaveText();
        }

        // ��� �ؽ�ó ��Ȱ��ȭ
        foreach (var texture in waveTextures)
        {
            if (texture != null)
                texture.SetActive(false);
        }

        // �ʱ� ���� �ؽ�Ʈ ����
        UpdateStatusText("G Ű�� ���� ���̺� ����");
    }

    void Update()
    {
        if (waveManager != null)
        {
            // ���� ���̺� ����ȭ
            int waveManagerWave = waveManager.GetCurrentWave();

            // ���� üũ �߰�
            if (waveManagerWave > 0 && waveManagerWave - 1 != currentWave)
            {
                currentWave = waveManagerWave - 1;

                // ���� üũ
                if (currentWave >= 0 && currentWave < totalWaves)
                {
                    UpdateWaveText();
                    UpdateWaveTexture();
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

    // ���̺� ���� �� ȣ��
    public void OnWaveStarted()
    {
        if (currentWave < totalWaves && currentWave < waveTextures.Length)
        {
            // ���� ���̺� �ؽ�ó Ȱ��ȭ
            UpdateWaveTexture();

            // ���� �ؽ�Ʈ ������Ʈ
            UpdateStatusText($"���̺� {currentWave + 1} ���� ��");

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

        // ���� üũ
        if (currentWave >= 0 && currentWave < totalWaves)
        {
            UpdateScrollbar();
            UpdateWaveText();

            // ���� ���̺� �ؽ�ó ��Ȱ��ȭ
            if (currentWave < waveTextures.Length && waveTextures[currentWave] != null)
            {
                waveTextures[currentWave].SetActive(false);
            }

            // ���� ���̺갡 �ִٸ� �ȳ� �޽���
            if (currentWave + 1 < totalWaves)
            {
                UpdateStatusText($"���̺� {currentWave + 1} �Ϸ�! G Ű�� ���� ���� ���̺� ����");
            }
            else
            {
                UpdateStatusText("��� ���̺� �Ϸ�!");
            }
        }
    }

    // ���̺� �ؽ�ó ������Ʈ
    private void UpdateWaveTexture()
    {
        // ���� ��� �ؽ�ó ��Ȱ��ȭ
        foreach (var texture in waveTextures)
        {
            if (texture != null)
                texture.SetActive(false);
        }

        // ���� üũ �߰�
        if (currentWave >= 0 && currentWave < waveTextures.Length && waveTextures[currentWave] != null)
        {
            waveTextures[currentWave].SetActive(true);
            Debug.Log($"���̺� �ؽ�ó Ȱ��ȭ: {currentWave}");
        }
        else
        {
            Debug.LogWarning($"���̺� �ؽ�ó ���� �ʰ�: currentWave={currentWave}, �ؽ�ó ��={waveTextures.Length}");
        }
    }

    // ��ũ�ѹ� ������Ʈ
    private void UpdateScrollbar()
    {
        if (scrollbar != null && totalWaves > 1)
        {
            float value = (float)currentWave / (totalWaves - 1);
            scrollbar.value = value;
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

    // ���̺� �� ���� ��Ȳ ������Ʈ (�� óġ��)
    public void UpdateProgress(float progress)
    {
        if (scrollbar != null)
        {
            // ���� ���̺� �� ���� ��Ȳ�� ��Ÿ���� ���� ǥ�� ����
            // ��: ������ ���� �ٳ� �ؽ�Ʈ�� ǥ��
        }
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

        // ���� ��Ȳ UI ������Ʈ (�ʿ��� ���)
        if (maxEnemiesInWave > 0)
        {
            float progress = (float)value / maxEnemiesInWave;
            UpdateProgress(progress);
        }
    }

    public void StartWave()
    {
        // ���� ���̺갡 ��ȿ�� �������� Ȯ��
        if (currentWave >= 0 && currentWave < totalWaves)
        {
            OnWaveStarted();
        }
        else
        {
            Debug.LogError($"�߸��� ���̺� �ε���: {currentWave}");
        }
    }

    // ���̺� ���� - WaveManager���� ���� ȣ���ϱ� ���� �Լ�
    public void EndWave()
    {
        // ���� ���̺� �ؽ�ó ��Ȱ��ȭ
        if (currentWave < waveTextures.Length && waveTextures[currentWave] != null)
        {
            waveTextures[currentWave].SetActive(false);
        }

        // ���� ���̺� ����
        currentWave++;

        // UI ������Ʈ
        UpdateScrollbar();
        UpdateWaveText();

        // ���� �ؽ�Ʈ ������Ʈ
        if (currentWave < totalWaves)
        {
            UpdateStatusText($"���̺� {currentWave} �Ϸ�! G Ű�� ���� ���� ���̺� ����");
        }
        else
        {
            UpdateStatusText("��� ���̺� �Ϸ�!");
        }
    }

    // ����
    public void ResetProgressBar()
    {
        currentWave = 0;
        UpdateScrollbar();
        UpdateWaveText();
        UpdateWaveTexture();
        UpdateStatusText("G Ű�� ���� ���̺� ����");
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