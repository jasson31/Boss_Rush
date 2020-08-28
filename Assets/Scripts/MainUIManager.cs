using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIManager : UIManager
{
    public Button GS, S, I, ESLarge, ESMedium, ESSmall, BGMLarge, BGMMedium, BGMMute;
    public GameObject titleButtons, settingsMenu;
    GameObject audioObject1,audioObject2,audioObject3;
    GameObject text, backText;

    Rigidbody2D rgbd;

    int TitleInclineValue = 0;

    AudioSource audio1,audio2,audio3;//audio1:클릭소리, audio2:지지직소리, audio3:BGM

    void Awake()
    {
        audioObject1 = GameObject.Find("EffectSound1");
        audioObject2 = GameObject.Find("EffectSound2");
        audioObject3 = GameObject.Find("BGM");
        audio1 = audioObject1.GetComponent<AudioSource>();
        audio2 = audioObject2.GetComponent<AudioSource>();
        audio3 = audioObject3.GetComponent<AudioSource>();

        text = GameObject.Find("InclineTempText");
        backText = GameObject.Find("InclineBackTempText");
        rgbd = text.GetComponent<Rigidbody2D>();

        settingsMenu.SetActive(false);
    }

    protected override void OnMount()
    {
        Debug.Log("Main On");
    }

    protected override void OnUnmount()
    {
        Debug.Log("Main Off");
    }

    public void GameStart()
    {
        UnityEngine.Debug.Log("Game Start!");

        audio1.Play();
    }

    public void OpenSettings()
    {
        UnityEngine.Debug.Log("Open Settings!");

        titleButtons.SetActive(false);
        settingsMenu.SetActive(true);

        audio1.Play();
    }

    public void CloseSettings()
    {
        UnityEngine.Debug.Log("Close Settings!");

        titleButtons.SetActive(true);
        settingsMenu.SetActive(false);

        audio1.Play();
    }

    public void AdjustEffectSoundtoLarge()
    {
        UnityEngine.Debug.Log("Adjust Effect Sound to Large!");

        audio1.volume = 1.0f;
        audio2.volume = 1.0f;

        audio1.Play();
    }

    public void AdjustEffectSoundtoMedium()
    {
        UnityEngine.Debug.Log("Adjust Effect Sound to Medium!");

        audio1.volume = 0.6f;
        audio2.volume = 0.6f;

        audio1.Play();
    }

    public void AdjustEffectSoundtoSmall()
    {
        UnityEngine.Debug.Log("Adjust Effect Sound to Small!");

        audio1.volume = 0.2f;
        audio2.volume = 0.2f;

        audio1.Play();
    }

    public void AdjustBGMSoundtoLarge()
    {
        UnityEngine.Debug.Log("Adjust BGM Sound to Large!");

        audio3.mute = false;
        audio3.volume = 1.0f;

        audio1.Play();
    }

    public void AdjustBGMSoundtoMedium()
    {
        UnityEngine.Debug.Log("Adjust BGM Sound to Medium!");

        audio3.mute = false;
        audio3.volume = 0.4f;

        audio1.Play();
    }

    int audio3MuteVal = 0;
    public void MuteBGMSound()
    {
        if(audio3MuteVal==0)
        {
            UnityEngine.Debug.Log("Mute BGM Sound!");
            audio3.mute = true;
            audio3MuteVal = 1;
        }
        else
        {
            UnityEngine.Debug.Log("Unmute BGM Sound!");
            audio3.mute = false;
            audio3MuteVal = 0;
        }

        audio1.Play();
    }

    public void TitleIncline()
    {
        if (TitleInclineValue == 0)
        {
            rgbd.WakeUp();
            rgbd.bodyType = RigidbodyType2D.Dynamic;

            TitleInclineValue++;

            audio2.Play();
        }
        else
        {
            rgbd.bodyType = RigidbodyType2D.Static;

            text.transform.rotation = Quaternion.Euler(0, 0, 0);
            text.transform.position = backText.transform.position;

            TitleInclineValue--;

            audio2.Stop();
        }
    }

    //키 세팅
    public enum KeyAction {Left,Right,Roll,Jump};

    public static class KeySetting { public static Dictionary<KeyAction, KeyCode> keys = new Dictionary<KeyAction, KeyCode>();}//키 담을 곳

    public Text[] keytxt;

    void Start()//초기화
    {
        KeySetting.keys.Add(KeyAction.Left, KeyCode.A);
        KeySetting.keys.Add(KeyAction.Right, KeyCode.D);
        KeySetting.keys.Add(KeyAction.Roll, KeyCode.W);
        KeySetting.keys.Add(KeyAction.Jump, KeyCode.Space);

        for(int i=0;i<keytxt.Length;i++)
        {
            keytxt[i].text = KeySetting.keys[(KeyAction)i].ToString();
        }
    }

    void OnGUI()//키 변경
    {
        Event keyEvent = Event.current;
        if(keyEvent.isKey && key!=-1)
        {
            int isOverlap = 0;
            for(int i=0;i<4;i++)
            {
                if (keyEvent.keyCode == KeySetting.keys[(KeyAction)i])
                {
                    isOverlap = 1;
                }
            }
            if(isOverlap == 1)
            {
                UnityEngine.Debug.Log("There's same key already used!");
                key = -1;
            }
            else
            {
                KeySetting.keys[(KeyAction)key] = keyEvent.keyCode;
                key = -1;
            }
        }
    }

    int key = -1;
    public void ChangeKey(int num)//바꿀 키 지정
    {
        key = num;
    }

    //키 세팅 테스트
    void Update()
    {
        if(Input.GetKey(KeySetting.keys[KeyAction.Left]))
        {
            UnityEngine.Debug.Log("Left");
        }
        else if (Input.GetKey(KeySetting.keys[KeyAction.Right]))
        {
            UnityEngine.Debug.Log("Right");
        }
        if (Input.GetKey(KeySetting.keys[KeyAction.Roll]))
        {
            UnityEngine.Debug.Log("Roll");
        }
        else if (Input.GetKey(KeySetting.keys[KeyAction.Jump]))
        {
            UnityEngine.Debug.Log("Jump");
        }

        for (int i = 0; i < keytxt.Length; i++)
        {
            keytxt[i].text = KeySetting.keys[(KeyAction)i].ToString();
        }
    }
}
