using UnityEngine;

public class PauseMenuObserver : MonoBehaviour, IObserver
{
    public PauseMenuController pauseMenuController;

    private void Start()
    {
        // 옵저버 등록
        if (pauseMenuController != null)
        {
            pauseMenuController.AddObserver(this);
        }
    }

    private void OnDestroy()
    {
        // 옵저버 해제
        if (pauseMenuController != null)
        {
            pauseMenuController.RemoveObserver(this);
        }
    }

    public void OnNotify(GameObject obj, string eventMessage)
    {
        if (eventMessage == "OpenMenu")
        {
            Debug.Log("Pause menu opened!");
            // 여기에 필요한 추가 작업을 수행
        }
        else if (eventMessage == "CloseMenu")
        {
            Debug.Log("Pause menu closed!");
            // 여기에 필요한 추가 작업을 수행
        }
    }
}
