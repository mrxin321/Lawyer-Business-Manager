using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class LoadingItemBase : MonoBehaviour
{   
    //loading占比
    public float LoadingRate = 0f;
    public float OneFrameProgress = 0f;
    protected float LoadingProgress;           //当前加载进度
    public float ExpectTime = 0f;

    public bool LoadingFinish = false;

    public virtual float GetLoadingProgress()
    {
        return 0f;
    }

    public virtual void StartProgress()
    {}

    public virtual void EndProgress()
    {}

    public virtual bool IsLoadingFinish()
    {
        return false;
    }

    public virtual string GetLoadingTips()
    {
        return "";
    }
}
