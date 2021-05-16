using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Permissioon : MonoBehaviour
{
	public List<PermissionType> PermissionList = new List<PermissionType>();
   	void Awake()
   	{
   		var canShow = false;
   		var myPermission = (PermissionType)PlayerDataManager.Instance.GetPermissin();
   		foreach(var item in PermissionList)
   		{
   			if(item == myPermission)canShow = true;
   		}

   		gameObject.SetActive(canShow);
   	}
}
