﻿using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class RewardAdmob : MonoBehaviour
{
    RewardBasedVideoAd ad;
    [SerializeField] string appId;
    [SerializeField] string unitId;
    [SerializeField] bool isTest;
    [SerializeField] string deviceId;
    void Start()
    {
        MobileAds.Initialize(appId);
        ad = RewardBasedVideoAd.Instance;

        //광고요청이 성공적으로 로드되면 호출됩니다.
        ad.OnAdLoaded += OnAdLoaded;
        //광고요청을 로드하지 못했을 때 호출됩니다.
        ad.OnAdFailedToLoad += OnAdFailedToLoad;
        //광고가 표시될 때 호출됩니다.
        ad.OnAdOpening += OnAdOpening;
        //광고가 재생되기 시작하면 호출됩니다.
        ad.OnAdStarted += OnAdStarted;
        //사용자가 비디오 시청을 통해 보상을 받을 때 호출됩니다.
        ad.OnAdRewarded += OnAdRewarded;
        //광고가 닫힐 때 호출됩니다.
        ad.OnAdClosed += OnAdClosed;
        //광고클릭으로 인해 사용자가 애플리케이션을 종료한 경우 호출됩니다.
        ad.OnAdLeavingApplication += OnAdLeavingApplication;

        LoadAd();
    }

    void LoadAd()
    {
        AdRequest request = new AdRequest.Builder().Build();
        if (isTest)
        {
            if (deviceId.Length > 0)
                request = new AdRequest.Builder().AddTestDevice(AdRequest.TestDeviceSimulator).AddTestDevice(deviceId).Build();
            else
                unitId = "ca-app-pub-3940256099942544/5224354917"; //테스트 유닛 ID

        }
        ad.LoadAd(request, unitId);
    }

    public void OnBtnViewAdClicked()
    {
        if (ad.IsLoaded())
        {
            Debug.Log("View Ad");
            ad.Show();
        }
        else
        {
            Debug.Log("Ad is Not Loaded");
            LoadAd();
        }
    }

    void OnAdLoaded(object sender, EventArgs args) { Debug.Log("OnAdLoaded"); }
    void OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e) { Debug.Log("OnAdFailedToLoad"); }
    void OnAdOpening(object sender, EventArgs e) { Debug.Log("OnAdOpening"); }
    void OnAdStarted(object sender, EventArgs e) { Debug.Log("OnAdStarted"); }
    void OnAdRewarded(object sender, Reward e)
    {
        Game_Manager.Instance.ui_manager.ads = true;
        Game_Manager.Instance.ui_manager.x2_Btn_Text.text = Game_Manager.Instance.ui_manager.money * 2 + "";
        Game_Manager.Instance.ui_manager.AD_Btn.gameObject.SetActive(false);
        Game_Manager.Instance.ui_manager.Reward_Btn.gameObject.SetActive(false);
        Game_Manager.Instance.ui_manager.x2_Btn.gameObject.SetActive(true);
        Debug.Log("OnAdRewarded");
    }
    void OnAdClosed(object sender, EventArgs e)
    {
        Debug.Log("OnAdClosed");
        LoadAd();
    }
    void OnAdLeavingApplication(object sender, EventArgs e) { Debug.Log("OnAdLeavingApplication"); }
}