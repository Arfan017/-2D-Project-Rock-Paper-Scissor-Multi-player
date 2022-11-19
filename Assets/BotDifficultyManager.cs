using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System;
using Unity.Services.RemoteConfig;

public class BotDifficultyManager : MonoBehaviour
{
    [SerializeField] Bot bot;
    [SerializeField] int selectedDifficulity;
    [SerializeField] BotStats[] botDifficulities;

    [Header("Remote Config Parameter")]
    [SerializeField] bool enableRemoteConfig = false;
    [SerializeField] string difficultyKey = "Difficulty";
    struct userAttributes{};
    struct appAttributes{};


    IEnumerator Start()
    {
        //tunggu selesai set up
        yield return new WaitUntil(() => bot.IsReady);

        //set stats default dari difficulty manager
        //seuai selectedDifficulity dari inspector
        var newStats = botDifficulities[selectedDifficulity];
        bot.SetStats(newStats, true);

        // ambil difficulty dari unity config kalau enable
        if (enableRemoteConfig == false)
        {
            yield break;
        }

        //tapi tunggu dulu sampai unity service siap
        yield return new WaitUntil(
            () => 
            UnityServices.State == ServicesInitializationState.Initialized
            &&
            AuthenticationService.Instance.IsSignedIn
            );
        
        //Daftar dulu untuk event fetch completed
        RemoteConfigService.Instance.FetchCompleted += OnRemoteConfigFetched;
        
        // lalu fetch di sini. cukup sekali di awal permainan
        RemoteConfigService.Instance.FetchConfigs(
            new userAttributes(), new appAttributes());
    }

    private void OnDestroy()
    {
        // jangan lupa untuk unregister event untuk menghindari memory leak
        RemoteConfigService.Instance.FetchCompleted -= OnRemoteConfigFetched;
    }

    //setiap kali data baru didapatkan(melalui fetch) fungsi ini kan dipanggil
    private void OnRemoteConfigFetched(ConfigResponse response)
    {
        if (RemoteConfigService.Instance.appConfig.HasKey(difficultyKey) == false)
        {
            Debug.LogWarning($"Difficulty Key:{difficultyKey} not found on remote config server");
            return;
        }

        switch (response.requestOrigin)
        {
            case ConfigOrigin.Default:
            case ConfigOrigin.Cached:
                break;
            case ConfigOrigin.Remote:
                selectedDifficulity = RemoteConfigService.Instance.appConfig.GetInt(difficultyKey);
                selectedDifficulity = Mathf.Clamp(selectedDifficulity, 0, botDifficulities.Length-1);
                var newStats = botDifficulities[selectedDifficulity];
                bot.SetStats(newStats, true);
                break;
            
        }
    }
}