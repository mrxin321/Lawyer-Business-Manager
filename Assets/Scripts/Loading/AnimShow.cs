using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class AnimShow : LoadingItemBase
{
    public override string GetLoadingTips()
    {
        return "";
    }

    public override float GetLoadingProgress()
    {
        return -1.0f;   
    }

    public override void StartProgress()
    {
        Utility.DoWait(()=>{
            LoadingFinish = true;
        },4,this);
    }

    public override bool IsLoadingFinish()
    {
        return LoadingFinish;
    }

}
