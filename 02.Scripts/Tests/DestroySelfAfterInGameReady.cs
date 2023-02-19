using HTH.Assets;
using HTH.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelfAfterInGameReady : MonoBehaviour
{    
    // Update is called once per frame
    void Update()
    {
        if (AssetsLoader.instance.isLoaded &&
            LoadingUI.instance.gameObject.activeSelf == false)
        {
            Destroy(gameObject);
        }
    }
}
