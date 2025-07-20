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
        // ���̺� �Ŵ��� ����
        if (waveManager != null)
        {
            // ���� ���̺� ����ȭ
            int waveManagerWave = waveManager.GetCurrentWave();
            if (waveManagerWave != currentWave + 1)
            {
                currentWave = waveManagerWave - 1;
                UpdateWaveText();
                UpdateWaveTexture();
            }

            // ���̺� ���� ��Ȳ ������Ʈ (�� óġ ���൵)
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

    // ���̺� ���� �� ȣ�� (�̺�Ʈ �ݹ�)
    public void OnWaveCompleted(int waveNumber)
    {
        // UI ������Ʈ
        currentWave = waveNumber;
        UpdateScrollbar();
        UpdateWaveText();

        // ���� ���̺갡 �ִٸ� �ȳ� �޽��� ǥ��
        if (currentWave < totalWaves)
        {
            UpdateStatusText($"���̺� {currentWave} �Ϸ�! G Ű�� ���� ���� ���̺� ����");
        }
        else
        {
            UpdateStatusText("��� ���̺� �Ϸ�!");
        }

        // ���� ���̺� �ؽ�ó ��Ȱ��ȭ
        if (waveNumber - 1 < waveTextures.Length)
        {
            waveTextures[waveNumber - 1].SetActive(false);
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

        // ���� ���̺� �ؽ�ó Ȱ��ȭ
        if (currentWave < waveTextures.Length && waveTextures[currentWave] != null)
        {
            waveTextures[currentWave].SetActive(true);
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

    // ���̺� ���� - WaveManager���� ���� ȣ���ϱ� ���� �Լ�
    public void StartWave()
    {
        // �̹� ������ OnWaveStarted �Լ� Ȱ��
        OnWaveStarted();
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