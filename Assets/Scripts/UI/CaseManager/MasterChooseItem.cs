using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterChooseItem :MonoBehaviour
{
    [SerializeField] Text Name;
    [SerializeField] SelectItem SelectItem;

    private int MasterId;
    public static Action<int,bool> ChooseAction;
	public void SetData(int masterId,string name,bool choose,Action<int,bool> chooseAction)
	{
		ChooseAction = chooseAction; 
		Name.text = name;
		MasterId = masterId;
		SelectItem.SetData(ItemChoose,choose);
	}

	public void ItemChoose(bool choose)
	{
		Utility.SafePostEvent(ChooseAction,MasterId,choose);
	}
}
