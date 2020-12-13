using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyTaskItem : MonoBehaviour
{
    [SerializeField] Text TaskContent;
	[SerializeField] Dropdown Dropdown;

	private TaskData TaskData;

    public void SetData(TaskData taskData)
    {
        TaskData = taskData;
        
        if(TaskData != null)
        {        
    		TaskContent.text = taskData.Content;
            Dropdown.value = taskData.State;
    	}

        //状态

        // d1.options.Clear();
        // Dropdown.OptionData temoData;
        // var names = Enum.GetNames(typeof(GuideFuncType));
        // EDebug.LogFormat("wtf ffffffffffffffff====================== {0}",names);
        // for (int i = 0; i < names.Length; i++)
        // {
        //     temoData = new Dropdown.OptionData();
        //     temoData.text = names[i];
        //     d1.options.Add(temoData);
        // }
    }

    public void SelectChange()
    {
        var hashtable = new Hashtable();

        hashtable.Add(0,TaskData.Id);
        hashtable.Add(1,Dropdown.value);
        var calNames1 = new string[]{"id","state"};
        
        var reader = SqliteManager.Instance.UpateValue("task",calNames1,hashtable);
        reader.Close();
    }
}
