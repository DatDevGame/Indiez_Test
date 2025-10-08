using System.Collections;
using System.Collections.Generic;
using Premium.Monetization;
using UnityEngine;
using UnityEngine.UI;

public class InterstitialAdButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            AdsManager.Instance.ShowInterstitialAd(AdsLocation.None);
        });
    }
}
