using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SelectUIManager : UIManager
{
    private static bool[] con = new bool[10];
    public Button B0, B1, B2, B3, B4, B5, D0, D1;
    public Text tutorialText, dungeonText;
    public GameObject G1, G2, G3, dungeonUI;
  
    public void alpha()
    {
        dungeonUI.gameObject.SetActive(true);
        if (!con[1])
        {
            B4.gameObject.SetActive(false);
            B5.gameObject.SetActive(false);
        }

        if (!con[2])
        {
            D0.gameObject.SetActive(false);
            D1.gameObject.SetActive(false);
        }
    }

    public void t1()
    {
        
        tutorialText.gameObject.SetActive(false);
        alpha();
        
}
    public void t2()
    {
        tutorialText.gameObject.SetActive(false);
        Debug.Log("튜토리얼 보스");
        con[0] = true;
        B0.GetComponentInChildren<Text>().text = "클리어";
        alpha();
        
}

    public void b0()
    {
        
        if (!con[0])
        {
            tutorialText.gameObject.SetActive(true);
            dungeonUI.gameObject.SetActive(false);
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

            B2.interactable = false;
            B3.interactable = false;
        }
    }

    public void b2()
    {
        if (!con[2])
        {
            Debug.Log("보스2");
            B2.GetComponentInChildren<Text>().text = "클리어";
            con[2] = true;

            //G1.gameObject.SetActive(false);
            //G3.gameObject.SetActive(false);

            B1.interactable = false;
            B3.interactable = false;

            d01();
        }
    }

    private void d01()
    {
        float rand = Random.value;

        if (rand > 0.5f)
        {
            D0.gameObject.SetActive(true);
        }
        else
        {
            D1.gameObject.SetActive(true);
        }
    }

    public void d0()
    {
        Debug.Log("폐품상");
    } 
    
    public void d1()
    {
        Debug.Log("대장간");
    }
    
    public void b3()
    {
        if (!con[3])
        {
            Debug.Log("보스3");
            B3.GetComponentInChildren<Text>().text = "클리어";
            con[3] = true;

            //G1.gameObject.SetActive(false);
            //G2.gameObject.SetActive(false);

            B1.interactable = false;
            B2.interactable = false;
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
        //throw new System.NotImplementedException();
        //Debug.Log("test2\n");
    }

    protected override void OnUnmount()
    {
        //throw new System.NotImplementedException();
        //Debug.Log("test\n"); 
    }

    // Start is called before the first frame update
    void Start()
    {
        alpha();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
