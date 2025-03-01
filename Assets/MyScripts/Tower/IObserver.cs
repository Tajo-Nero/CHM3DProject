using System.Collections.Generic;
using UnityEngine;

public interface IObserver
{
    void OnNotify(GameObject obj, string eventMessage);
}

public interface ISubject
{
    void AddObserver(IObserver observer);
    void RemoveObserver(IObserver observer);
    void NotifyObservers(GameObject obj, string eventMessage);
}

// ��ü Ŭ����
public class Subject : ISubject
{
    private List<IObserver> observers = new List<IObserver>();

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


