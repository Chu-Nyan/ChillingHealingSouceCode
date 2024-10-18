using System;
using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    public event Action<UIBase> OnDeactivation;
    
    public virtual void Activate()
    {
        gameObject.SetActive(true);
    }

    public virtual void Deactivate()
    {
        gameObject.SetActive(false);
        OnDeactivation?.Invoke(this);
        OnDeactivation = null;
    }
}
