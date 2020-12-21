using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TodoDeleteEvent : MonoBehaviour
{
    public void DeleteSelf()
    {
    	GameObject.Destroy(gameObject);
    }
}
