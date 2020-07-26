using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUIManager : UIManager
{
    protected override void OnMount()
    {
        Debug.Log("Main On");
    }

    protected override void OnUnmount()
    {
        Debug.Log("Main Off");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
