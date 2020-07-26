using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectUIManager : UIManager
{
    protected override void OnMount()
    {
        Debug.Log("Select On");
    }

    protected override void OnUnmount()
    {
        Debug.Log("Select Off");
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
