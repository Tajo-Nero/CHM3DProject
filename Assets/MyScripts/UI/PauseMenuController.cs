using UnityEngine;
using System.Collections.Generic;

public class PauseMenuController : MonoBehaviour, ISubject
{
    public GameObject[] panels; // 여러 개의 패널을 담을 배열
    private List<IObserver> observers = new List<IObserver>();

    void Start()
    {
        // 시작 시 모든 패널을 비활성화
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
        gameObject.SetActive(false); // PauseMenuController 자신도 비활성화
    }

    public void OpenMenu()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(true);
        }
        gameObject.SetActive(true); // PauseMenuController 자신도 활성화
        NotifyObservers(null, "OpenMenu"); // 옵저버들에게 신호 전달
    }

    public void CloseMenu()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
        gameObject.SetActive(false); // PauseMenuController 자신도 비활성화
        NotifyObservers(null, "CloseMenu"); // 옵저버들에게 신호 전달
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
}
