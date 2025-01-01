using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Purchasing;

public class ShopButton : MonoBehaviour
{
    public ShopItem item;
    public ShopUI shopScript;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI priceText;
    public Image iconLeft;
    public Image iconRight;
    public Button button;

    public ButtonBlocker blockerScript;

    public GameObject clickSound;

    public CodelessIAPButton iapScript;
    public Product myProduct;

    // Start is called before the first frame update
    void Start()
    {
        blockerScript.myAction = "shop" + item.id;
        if (item.iap)
        {
            iapScript.productId = item.id;
            iapScript.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (item.iap && myProduct!=null)
        {
            titleText.text = myProduct.metadata.localizedTitle;
            priceText.text = myProduct.metadata.localizedPriceString;
            if (shopScript.itemView.activeSelf == true)
            {
                iapScript.enabled = false;
            }
            else
            {
                iapScript.enabled = true;
            }
        }
        if (item.ad)
        {
            titleText.text = item.text;
            priceText.text = "WATCH AD";
        }
    }
    public void ButtonPress()
    {
        if (shopScript.itemView.activeSelf == false) {
            Tutorial.AdvanceTutorial();
            Instantiate(clickSound, Camera.main.transform.position, transform.rotation);
            shopScript.productViewing = myProduct;
            if (item.price != -1) {
                shopScript.iapScript.enabled = false;
                shopScript.OpenItemView(item);
            }
            else
            {
                if (item.iap)
                {
                    shopScript.iapScript.productId = item.id;
                    shopScript.iapScript.enabled = true;
                    shopScript.OpenItemView(item);
                }
                else
                {
                    if (item.ad)
                    {
                        shopScript.iapScript.enabled = false;
                        shopScript.OpenItemView(item);
                    }
                    else {
                        shopScript.iapScript.enabled = false;
                        shopScript.page = item.id;
                        shopScript.OpenShopPage();
                    }
                }
            }
        }
    }
    public void ProductPurchased()
    {

    }
    public void ProductFetched(Product product)
    {
        myProduct = product;
        iapScript.enabled = false;
    }
}
