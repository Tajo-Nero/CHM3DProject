using UnityEngine;
using System.Collections.Generic;

public class PauseMenuController : MonoBehaviour, ISubject
{
    public GameObject[] panels; // ���� ���� �г��� ���� �迭
    private List<IObserver> observers = new List<IObserver>();

    void Start()
    {
        // ���� �� ��� �г��� ��Ȱ��ȭ
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
        gameObject.SetActive(false); // PauseMenuController �ڽŵ� ��Ȱ��ȭ
    }

    public void OpenMenu()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(true);
        }
        gameObject.SetActive(true); // PauseMenuController �ڽŵ� Ȱ��ȭ
        NotifyObservers(null, "OpenMenu"); // �������鿡�� ��ȣ ����
    }

    public void CloseMenu()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
        gameObject.SetActive(false); // PauseMenuController �ڽŵ� ��Ȱ��ȭ
        NotifyObservers(null, "CloseMenu"); // �������鿡�� ��ȣ ����
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
