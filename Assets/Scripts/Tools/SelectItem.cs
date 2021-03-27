using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectItem : MonoBehaviour
{
	public Action<bool> SelectAction;
    [SerializeField] GameObject SelectObj;

	public void SetData(Action<bool> action,bool choose)
	{
		SelectAction = action;
		SelectObj.SetActive(choose);
	}

	public void Choose()
	{
		SelectObj.SetActive(!SelectObj.activeSelf);
		Utility.SafePostEvent(SelectAction,SelectObj.activeSelf);
	}
}
