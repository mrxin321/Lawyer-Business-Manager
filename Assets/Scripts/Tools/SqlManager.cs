using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SqlManager : MonoBehaviour
{
	void OnDestroy()
	{
		SqliteManager.Instance.CloseDB();
	}
}
