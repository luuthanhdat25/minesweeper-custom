using System.Collections.Generic;
using UnityEngine;

public class RepeatMonoBehaviour : MonoBehaviour
{
    protected virtual void Reset()
    {
        this.LoadComponents();
        this.ResetValue();
    }
    
    protected virtual void Awake()
    {
        this.LoadComponents();
        this.ResetValue();
    }
    
    protected virtual void ResetValue()
    {
        // For override
    }

    protected virtual void LoadComponents()
    {
        // For override
    }

    protected T FindComponentInParent<T>() where T : Component
    {
        Transform parent = transform.parent;
        while (parent != null)
        {
            T foundComponent = parent.GetComponent<T>();
            if (foundComponent != null) return foundComponent;
            parent = parent.parent;
        }
        return null;
    }
    
    //Find component in all children(all children of children) of this transform 
    protected T FindComponentInChildren<T>() where T : Component
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(transform);

        while (queue.Count > 0)
        {
            Transform current = queue.Dequeue();
            T foundComponent = current.GetComponent<T>();
            if (foundComponent != null) return foundComponent;

            int childCount = current.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = current.GetChild(i);
                queue.Enqueue(child);
            }
        }
        return null;
    }
}
