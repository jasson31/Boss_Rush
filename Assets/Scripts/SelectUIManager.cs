using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SelectUIManager : UIManager
{
    private static bool[] con = new bool[10];
    public Button B0, B1, B2, B3, B4, B5;
    public Text text;
  

    public void t1()
    {
        GameObject.Find("T1").SetActive(false);
        text.gameObject.SetActive(true);
        l1Enable();
        
}
    public void t2()
    {
        GameObject.Find("T1").SetActive(false);
        text.gameObject.SetActive(true);
        Debug.Log("튜토리얼 보스");
        con[0] = true;
        B0.GetComponentInChildren<Text>().text = "클리어";
        l1Enable();
        
}

    private void l1Enable()
    {
        Button[] arr = { B0, B1, B2, B3 };
        foreach (Button a in arr)
        {
            a.gameObject.SetActive(true);
        }
    }

    public void b0()
    {
        if (!con[0])
        {
            Debug.Log("튜토리얼 보스");
            B0.GetComponentInChildren<Text>().text = "클리어";
            con[0] = true;
        }
    } 
    
    public void b1()
    {
        if (!con[1])
        {
            Debug.Log("보스1");
            B1.GetComponentInChildren<Text>().text = "클리어";
            con[1] = true;

            B4.gameObject.SetActive(true);
            B5.gameObject.SetActive(true);
        }
    }

    public void b2()
    {
        if (!con[2])
        {
            Debug.Log("보스2");
            B2.GetComponentInChildren<Text>().text = "클리어";
            con[2] = true;
        }
    }
    
    public void b3()
    {
        if (!con[3])
        {
            Debug.Log("보스3");
            B3.GetComponentInChildren<Text>().text = "클리어";
            con[3] = true;
        }
    }
    
    public void b4()
    {
        if (!con[4])
        {
            Debug.Log("보스4");
            B4.GetComponentInChildren<Text>().text = "클리어";
            con[4] = true;
        }
    }
    
    public void b5()
    {
        if (!con[5])
        {
            Debug.Log("보스5");
            B5.GetComponentInChildren<Text>().text = "클리어";
            con[5] = true;
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
        text.gameObject.SetActive(false);
        B0.gameObject.SetActive(false);
        B1.gameObject.SetActive(false);
        B2.gameObject.SetActive(false);
        B3.gameObject.SetActive(false);
        B4.gameObject.SetActive(false);
        B5.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
