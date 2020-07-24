using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectUIManager : UIManager
{

    public void t()
    {
        GameObject.Find("T1").SetActive(false);
    }
    protected override void OnMount()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnUnmount()
    {
        throw new System.NotImplementedException();
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
