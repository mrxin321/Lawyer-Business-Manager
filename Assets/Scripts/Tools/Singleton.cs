using System;
using UnityEngine;

public static class Clearer
{
    public static Action instanc;
}
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    #region Fields

    protected static T InstanceField;
    private static bool applicationIsQuitting = false;

    #endregion

    #region Properties

    public static T Instance
    {
        get
        {
            if (InstanceField == null)
            {
                if (applicationIsQuitting)
                    return null;
                InstanceField = (T)FindObjectOfType(typeof(T));
            }
            if (InstanceField != null)
            {
                return InstanceField;
            }
            var obj = new GameObject(typeof(T).ToString());
            InstanceField = obj.AddComponent<T>();
            Clearer.instanc += SetInstanceNull;
            return InstanceField;
        }
    }

    #endregion

    #region Methods

    protected virtual void Awake()
    {
        DontDestroyOnLoad(transform.root.gameObject);
    }

    protected virtual void OnDestroy()
    {
        //Debug.Log("Singleton OnDestroy:" + typeof(T).ToString());
        //if (applicationIsQuitting)
            //Debug.Log("Singleton was destroyed!!!!:" + typeof(T).ToString());
        InstanceField = null;
    }

    protected virtual void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }

    protected virtual void OnDisable()
    {
        //InstanceField = null;
    }

    public static void SetInstanceNull()
    {
        InstanceField = null;
    }
    protected virtual void Reset(){}
    #endregion
}
