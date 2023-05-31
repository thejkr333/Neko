using UnityEngine;
using UnityEngine.Events;

public class MyButton : MonoBehaviour, IClickable
{
    public UnityEvent OnClick;
    public void OnSelected()
    {
        OnClick?.Invoke();
    }
}
