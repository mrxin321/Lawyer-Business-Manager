using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseView : MonoBehaviour
{
    // Start is called before the first frame update
    public void onClose()
    {
    	GameObject.Destroy(gameObject);
    }
}
