using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIManager : UIManager
{
    public Button GS, S, I, ESLarge, ESMedium, ESSmall, BGMLarge, BGMMedium, BGMMute;
    public GameObject titleButtons, settingsMenu;
    GameObject audioObject1,audioObject2;
    GameObject text, backText;

    Rigidbody2D rgbd;

    int TitleInclineValue = 0;

    AudioSource audio1,audio2;//audio1:클릭소리, audio2:지지직소리

    void Awake()
    {
        audioObject1 = GameObject.Find("EffectSound1");
        audioObject2 = GameObject.Find("EffectSound2");
        audio1 = audioObject1.GetComponent<AudioSource>();
        audio2 = audioObject2.GetComponent<AudioSource>();

        text = GameObject.Find("InclineTempText");
        backText = GameObject.Find("InclineBackTempText");
        rgbd = text.GetComponent<Rigidbody2D>();

        settingsMenu.SetActive(false);
    }

    protected override void OnMount()
    {
        
    }

    protected override void OnUnmount()
    {
        
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

        audio1.Play();
    }

    public void AdjustEffectSoundtoMedium()
    {
        UnityEngine.Debug.Log("Adjust Effect Sound to Medium!");

        audio1.Play();
    }

    public void AdjustEffectSoundtoSmall()
    {
        UnityEngine.Debug.Log("Adjust Effect Sound to Small!");

        audio1.Play();
    }

    public void AdjustBGMSoundtoLarge()
    {
        UnityEngine.Debug.Log("Adjust BGM Sound to Large!");

        audio1.Play();
    }

    public void AdjustBGMSoundtoMedium()
    {
        UnityEngine.Debug.Log("Adjust BGM Sound to Medium!");

        audio1.Play();
    }

    public void MuteBGMSound()
    {
        UnityEngine.Debug.Log("Mute BGM Sound!");

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
}
