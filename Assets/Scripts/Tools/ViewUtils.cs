using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ViewUtils
{
    public static void Print(string message,params object[] args)
    {
        string msg = string.Format(message, args);
        Debug.Log(msg);
    }

    public static void MessageTips(string tips)
    {
    	UIManager.Instance.OpenWindow("MessageTipsView",tips);
    }

    public static void SetPayTypeDropdown(Dropdown dropdown,List<PayTypeData> dataList,int index)
   	{
   		dropdown.options.Clear();
	   	for (int i = 0; i < dataList.Count; i++)
	    {
	       	var temoData = new Dropdown.OptionData();
	       	temoData.text = dataList[i].Name;
	        dropdown.options.Add(temoData);

	    }
	    dropdown.value = index;
   	} 
}