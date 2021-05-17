using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToDoListView : BaseView
{
    public static Action UpdateView;

    private void OnEnable()
    {
        UpdateView += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        UpdateView -= Refresh;
    }

    [SerializeField] Transform ItemRoot;

    public override void Refresh()
    {

        Utility.DestroyAllChildren(ItemRoot);

        //获取当前没有完结的案子的未完成任务
        var todoList = TaskReader.GetToDoList(PlayerDataManager.Instance.GetUserId());
        Debug.LogFormat("wtf Refresh ================={0}",todoList.Count);

        foreach(var taskData in todoList)
        {
            var copyItem = AssetManager.CreatePrefab("TodoItem",ItemRoot);

            var item = copyItem.GetComponent<TodoItem>();
            if(item != null)
            {
                item.SetData(taskData);
            }

        }
    }
}
