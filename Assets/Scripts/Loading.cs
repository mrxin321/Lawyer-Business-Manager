using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IFix.Core;
using System.IO;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public List<LoadingItemBase> LoadingItems;
    public static Action<float> UpdateProgress;

    [SerializeField] Text ProgressText;
    [SerializeField] Image ProgressImage;
    [SerializeField] Transform ProgressRoot;

    private bool AllDone = false;
    private LoadingItemBase CurrentLoadingItemBase;

    private float TotalLoadingRate = 0f;

    void Awake()
    {
    }

    void Destory()
    {
    }

    private LoadingItemBase GetNextLoadingItem()
    {
        if(LoadingItems.Count > 0)
        {
            var item = LoadingItems[0];
            LoadingItems.RemoveAt(0);
            return item;
        }
        return null;
    }

    private void LateUpdate()
    {
        if(AllDone)return;

        if(CurrentLoadingItemBase == null)
        {
            CurrentLoadingItemBase = GetNextLoadingItem();

            if(CurrentLoadingItemBase != null)
            {
                CurrentLoadingItemBase.gameObject.SetActive(true);
                CurrentLoadingItemBase.StartProgress();
            }
            
        }

        LoadingUpdate();

        if(CurrentLoadingItemBase != null && CurrentLoadingItemBase.IsLoadingFinish())
        {
            CurrentLoadingItemBase.EndProgress();
            CurrentLoadingItemBase.gameObject.SetActive(false);
            CurrentLoadingItemBase = null;
        }

        //loading结束
        if(CurrentLoadingItemBase == null && LoadingItems.Count <= 0)
        {
            LoadingFinish();
        }


    }

    private void LoadingUpdate()
    {
        if(CurrentLoadingItemBase == null)return;

        var progress = CurrentLoadingItemBase.GetLoadingProgress();
        ProgressRoot.gameObject.SetActive(progress >= 0);

        ProgressText.text = string.Format(CurrentLoadingItemBase.GetLoadingTips(),string.Format("{0:F2}",progress));
        ProgressImage.fillAmount = progress / 100f;
    }

    private void LoadingFinish()
    {
        // 卸载当前场景
        Utility.DoWait(()=>{
            SceneManager.UnloadScene("Loading");
        },1f,this);
        // 加载下一个场景
        SceneManager.LoadScene("StartScene", LoadSceneMode.Additive);

        AllDone = true;
    }

    private void OnUpdateProgress(float progress)
    {}
}
