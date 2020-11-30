using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var prefab = Resources.Load("hh/TestPrefab");
        var go = GameObject.Instantiate(prefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
