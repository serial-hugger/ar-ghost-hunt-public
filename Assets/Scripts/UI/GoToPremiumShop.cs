using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToPremiumShop : MonoBehaviour
{

    public ShopUI shopScript;
    public MainUI uiScript;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenPremiumShop()
    {
        if (!Tutorial.inTutorial) {
            uiScript.SetCurrentMenu("shop");
            uiScript.shopUI.SetActive(true);
            shopScript.page = "premiumshop";
            shopScript.OpenShopPage();
        }
    }
}
