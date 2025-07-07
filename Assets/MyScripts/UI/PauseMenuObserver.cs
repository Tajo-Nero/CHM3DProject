using UnityEngine;

public class PauseMenuObserver : MonoBehaviour, IObserver
{
    public PauseMenuController pauseMenuController;

    private void Start()
    {
        // 可历滚 殿废
        if (pauseMenuController != null)
        {
            pauseMenuController.AddObserver(this);
        }
    }

    private void OnDestroy()
    {
        // 可历滚 秦力
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
        }
        else if (eventMessage == "CloseMenu")
        {
            Debug.Log("Pause menu closed!");
        }
    }
}
