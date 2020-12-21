using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyTaskItem : MonoBehaviour
{
    [SerializeField] Text TaskContent;
    [SerializeField] Dropdown Dropdown;
    [SerializeField] Transform TodoRoot;
	[SerializeField] Transform TodoTag;

	private TaskData TaskData;
    private List<InputField> InputFieldList = new List<InputField>();

    public void SetData(TaskData taskData)
    {
        TaskData = taskData;
        
        if(TaskData != null)
        {        
    		TaskContent.text = taskData.Content;
            Dropdown.value = taskData.State;
    	}

        TodoTag.gameObject.SetActive(taskData.TodoCount > 0);

        if(taskData.TodoCount > 0)
        {
            for(int i =0;i<taskData.TodoCount;i++)
            {
                var copyItem = AssetManager.CreatePrefab("TodoInputField",TodoRoot);
                var inputField = copyItem.GetComponent<InputField>();
                if(inputField != null)
                {
                    inputField.text = i == 0?taskData.Todo1:(i == 1?taskData.Todo2:taskData.Todo3);
                    InputFieldList.Add(inputField);
                    var index = i;
                    inputField.onEndEdit.AddListener((str)=>{
                        InputFieldEndEdit(index,str);
                    }); 
                }
            }
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

    private void InputFieldEndEdit(int todoIndex,string endStr)
    {
        var hashtable = new Hashtable();

        hashtable.Add(0,TaskData.Id);
        hashtable.Add(1,endStr);
        var paramStr = "todo1";
        if(todoIndex == 1)paramStr = "todo2";
        if(todoIndex == 2)paramStr = "todo3";
        var calNames1 = new string[]{"id",paramStr};
        
        SqliteManager.Instance.UpateValue("task",calNames1,hashtable);
    }   

    public void OpenOrCloseTodoList()
    {
        if(TaskData.TodoCount <= 0)return;
        TodoRoot.gameObject.SetActive(!TodoRoot.gameObject.activeSelf);
        TodoTag.localRotation = Quaternion.Euler(0.0f,0.0f,TodoRoot.gameObject.activeSelf?-90f:90f);
    }

    public void SelectChange()
    {
        var hashtable = new Hashtable();

        hashtable.Add(0,TaskData.Id);
        hashtable.Add(1,Dropdown.value);
        var calNames1 = new string[]{"id","state"};
        
        SqliteManager.Instance.UpateValue("task",calNames1,hashtable);
    }
}
