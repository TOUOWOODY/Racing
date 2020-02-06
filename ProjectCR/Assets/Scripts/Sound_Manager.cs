using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sound_Manager : MonoBehaviour
{
    public AudioClip button, Ready, Racing;
    public AudioSource audioSource;

    void Start()
    {
    }

    public void Initialized()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 1.0f;      //0.0f ~ 1.0f사이의 숫자로 볼륨을 조절
        audioSource.mute = false;       //오디오 음소거
        audioSource.playOnAwake = false;
    }
    void Update()
    {
        
    }


    public void Button_Sound()
    {
        audioSource.volume = 1;
        audioSource.loop = false;       //반복 여부
        audioSource.clip = button;      //오디오에 bgm이라는 파일 연결
        audioSource.Play();             //오디오 재생
    }

    public void Racing_Ready()
    {
        audioSource.loop = true;        //반복 여부
        audioSource.clip = Ready;       //오디오에 bgm이라는 파일 연결
        audioSource.Play();             //오디오 재생
        audioSource.volume = 0.15f;
    }
    public void Racing_Start()
    {
        audioSource.clip = Racing;       //오디오에 bgm이라는 파일 연결
        audioSource.Play();             //오디오 재생
        audioSource.volume = 0.15f;
    }
}
