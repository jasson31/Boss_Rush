using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class SelectUIManager : UIManager
{
    private static bool[] con = new bool[10];
    public Button B0, B1, B2, B3, B4, B5, B6, B7, B8;
    public Text tutorialText, dungeonText;
    public GameObject dungeonUI;

    ClearChecker clearChecker;

    public void alpha()
    {
        dungeonUI.gameObject.SetActive(true);
        if (!con[0])
        {
            B1.gameObject.SetActive(false);
            B2.gameObject.SetActive(false);
            B3.gameObject.SetActive(false);
        }
        else
        {
            B1.gameObject.SetActive(true);
            B2.gameObject.SetActive(true);
            B3.gameObject.SetActive(true);
        }
        if (!con[1] && !con[2] && !con[3])
        {
            B4.gameObject.SetActive(false);
            B5.gameObject.SetActive(false);
        }

        if(!con[4] && !con[5])
        {
            B6.gameObject.SetActive(false);
            B7.gameObject.SetActive(false);
            B8.gameObject.SetActive(false);
        }

        //if (!con[2])
        //{
        //    D0.gameObject.SetActive(false);
        //    D1.gameObject.SetActive(false);
        //}
    }

    public void tutorialNo()
    {
        
        tutorialText.gameObject.SetActive(false);
        alpha();
        
}
    public void tutorialYes()
    {
        tutorialText.gameObject.SetActive(false);
        Debug.Log("튜토리얼 보스");
        clearChecker.clearIndex = 0;
        SceneManager.LoadScene("Boss");
    }

    public void ClearTutorial()
    {
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
            clearChecker.clearIndex = 1;
            SceneManager.LoadScene("BossTower");
        }
    }

    public void Clearb1()
    {
        B1.GetComponentInChildren<Text>().text = "클리어";
        con[1] = true;

        B4.gameObject.SetActive(true);
        B5.gameObject.SetActive(true);

        B2.interactable = false;
        B3.interactable = false;
    }

    public void b2()
    {
        if (!con[2])
        {
            Debug.Log("보스2");
            clearChecker.clearIndex = 2;
            SceneManager.LoadScene("BossTower");

        }
    }

    public void Clearb2()
    {
        B2.GetComponentInChildren<Text>().text = "클리어";
        con[2] = true;

        B4.gameObject.SetActive(true);
        B5.gameObject.SetActive(true);

        B1.interactable = false;
        B3.interactable = false;
    }

    //private void d01()
    //{
    //    float rand = Random.value;

    //    if (rand > 0.5f)
    //    {
    //        D0.gameObject.SetActive(true);
    //    }
    //    else
    //    {
    //        D1.gameObject.SetActive(true);
    //    }
    //}

    //public void d0()
    //{
    //    Debug.Log("폐품상");
    //} 

    //public void d1()
    //{
    //    Debug.Log("대장간");
    //}

    public void b3()
    {
        if (!con[3])
        {
            Debug.Log("보스3");
            clearChecker.clearIndex = 3;
            SceneManager.LoadScene("BossTower");
        }
    }

    public void Clearb3()
    {
        B3.GetComponentInChildren<Text>().text = "클리어";
        con[3] = true;

        B4.gameObject.SetActive(true);
        B5.gameObject.SetActive(true);

        B1.interactable = false;
        B2.interactable = false;
    }

    public void b4()
    {
        if (!con[4])
        {
            Debug.Log("보스4");
            clearChecker.clearIndex = 4;
            SceneManager.LoadScene("BossBurang");
        }
    }

    public void Clearb4()
    {
        B4.GetComponentInChildren<Text>().text = "클리어";
        con[4] = true;

        B6.gameObject.SetActive(true);
        B7.gameObject.SetActive(true);
        B8.gameObject.SetActive(true);

        B5.interactable = false;
    }

    public void b5()
    {
        if (!con[5])
        {
            Debug.Log("보스5");
            clearChecker.clearIndex = 5;
            SceneManager.LoadScene("BossBurang");
        }
    }
    public void Clearb5()
    {
        B5.GetComponentInChildren<Text>().text = "클리어";
        con[5] = true;

        B6.gameObject.SetActive(true);
        B7.gameObject.SetActive(true);
        B8.gameObject.SetActive(true);

        B4.interactable = false;
    }

    public void b6()
    {
        if (!con[6])
        {
            Debug.Log("보스6");
            clearChecker.clearIndex = 6;
            SceneManager.LoadScene("BossSpider");
        }
    }
    public void Clearb6()
    {
        B5.GetComponentInChildren<Text>().text = "클리어";
        con[6] = true;

        B7.interactable = false;
        B8.interactable = false;
    }
    public void b7()
    {
        if (!con[7])
        {
            Debug.Log("보스7");
            clearChecker.clearIndex = 7;
            SceneManager.LoadScene("BossSpider");
        }
    }
    public void Clearb7()
    {
        B5.GetComponentInChildren<Text>().text = "클리어";
        con[7] = true;

        B6.interactable = false;
        B8.interactable = false;
    }
    public void b8()
    {
        if (!con[8])
        {
            Debug.Log("보스8");
            clearChecker.clearIndex = 8;
            SceneManager.LoadScene("BossSpider");
        }
    }
    public void Clearb8()
    {
        B5.GetComponentInChildren<Text>().text = "클리어";
        con[8] = true;

        B6.interactable = false;
        B7.interactable = false;
    }


    protected override void OnMount()
    {
        //Debug.Log("Select On");
    }

    protected override void OnUnmount()
    {
        //Debug.Log("Select Off");
    }

    private void Awake()
    {
        alpha();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(B0);
        clearChecker = FindObjectOfType<ClearChecker>();
        if (clearChecker != null)
        {
            switch(clearChecker.clearIndex)
            {
                case 0: ClearTutorial(); break;

                case 1: Clearb1(); break;
                case 2: Clearb2(); break;
                case 3: Clearb3(); break;

                case 4: Clearb4(); break;
                case 5: Clearb5(); break;

                case 6: Clearb6(); break;
                case 7: Clearb7(); break;
                case 8: Clearb8(); break;
            }
            Destroy(clearChecker.gameObject);
        }
        GameObject clearCheckerObject = new GameObject();
        clearCheckerObject.AddComponent<ClearChecker>();
        DontDestroyOnLoad(clearCheckerObject);
        clearChecker = clearCheckerObject.GetComponent<ClearChecker>();
    }
}

public class ClearChecker : MonoBehaviour
{
    public int clearIndex;
}
