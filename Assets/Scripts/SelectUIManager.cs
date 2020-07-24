using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SelectUIManager : UIManager
{
    public bool tutorial = false;
    public Button B0, B1, B2, B3;
 
    public void t1()
    {
        GameObject.Find("T1").SetActive(false);
        B0.gameObject.SetActive(true);
        
}
    public void t2()
    {
        GameObject.Find("T1").SetActive(false);
        tutorial = true;
        B0.GetComponentInChildren<Text>().text = "클리어";
        b0();
        
}

    public void b0()
    {
        Button[] arr = { B0, B1, B2, B3 };
        foreach(Button a in arr)
        {
            a.gameObject.SetActive(true);
        }
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
        B0.gameObject.SetActive(false);
        B1.gameObject.SetActive(false);
        B2.gameObject.SetActive(false);
        B3.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
