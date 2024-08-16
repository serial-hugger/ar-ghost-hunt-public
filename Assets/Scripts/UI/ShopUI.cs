using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public SaveManager saveScript;
    public List<ShopItem> shopItems = new List<ShopItem> {
        new ShopItem("premiumshop","category","PREMIUM SHOP",-1,-1,"none",23,"",false,false),
        new ShopItem("app","category","APPS",-1,-1,"none",0,"",false,false),
        new ShopItem("equipment", "category", "EQUIPMENT", -1, -1, "none", 1,"",false,false),
        new ShopItem("upgrade", "category", "UPGRADES", -1, -1, "none", 2,"",false,false),
        new ShopItem("skin", "category", "SKINS", -1, -1, "none", 3,"",false,false),
        new ShopItem("category","equipment","Return to categories",-1,-1,"none",-1,"",false,false),
        new ShopItem("emf","equipment","EMF",200,-1,"none",4,"Meter that measures electromagnetic fields, abnormal readings are common at haunted locations.",false,false),
        new ShopItem("compass","equipment","Compass",150,2,"none",11,"Normally points in one direction but can sometimes point toward unnatural phenomena.",false,false),
        new ShopItem("candles","equipment","Candles",100,-1,"none",8,"Set of three candles and a lighter.",false,false),
        new ShopItem("bell","equipment","Bell",50,6,"none",12,"A simple bell that makes a loud noise at the slightest touch.",false,false),
        new ShopItem("dowsingrod","equipment","Dowsing Rod",750,8,"none",5,"Two rods that can be used to find haunted locations.",false,false),
        new ShopItem("clipboard","equipment","Clipboard",150,-1,"none",6,"A piece of paper and bottle of ink that can become evidence of the paranormal.",false,false),
        new ShopItem("uv","equipment","UV Flashlight",500,-1,"none",13,"Reveals what can't be seen by the naked eye.",false,false),
        new ShopItem("motionsensor","equipment","Motion Sensor",500,0,"none",7,"Device that alerts you when it detects motion.",false,false),
        new ShopItem("livecamera","equipment","Live Camera",1000,-1,"none",14,"Streams video to the monitor.",false,false),
        new ShopItem("category","app","Return to categories",-1,-1,"none",-1,"",false,false),
        new ShopItem("video","app","Video Camera",1000,10,"none",9,"Video recording app that captures potato quality footage.",false,false),
        new ShopItem("sound","app","Sound Recorder",3000,20,"none",10,"Sound recording app that captures input from devices microphone.",false,false),
        new ShopItem("category","upgrade","Return to categories",-1,-1,"none",-1,"",false,false),
        new ShopItem("clipboardupgrade1","upgrade","2 Clipboards",250,-1,"clipboard",18,"Have a total of 2 clipboards.",false,false),
        new ShopItem("uvupgrade1","upgrade","2 UV Flashlights",1000,12,"uv",22,"Have a total of 2 UV flashlights.",false,false),
        new ShopItem("motionsensorupgrade1","upgrade","2 Motion Sensors",1500,17,"motionsensor",19,"Have a total of 2 motion sensors.",false,false),
        new ShopItem("motionsensorupgrade2","upgrade","3 Motion Sensors",2500,18,"motionsensorupgrade1",20,"Have a total of 3 motion sensors.",false,false),
        new ShopItem("motionsensorupgrade3","upgrade","4 Motion Sensors",3500,19,"motionsensorupgrade2",21,"Have a total of 4 motion sensors.",false,false),
        new ShopItem("cameraupgrade1","upgrade","2 Live Cameras",2000,21,"livecamera",15,"Have a total of 2 live cameras.",false,false),
        new ShopItem("cameraupgrade2","upgrade","3 Live Cameras",3000,22,"cameraupgrade1",16,"Have a total of 3 live cameras.",false,false),
        new ShopItem("cameraupgrade3","upgrade","4 Live Cameras",4000,23,"cameraupgrade2",17,"Have a total of 4 live cameras.",false,false),
        new ShopItem("category","skin","Return to categories",-1,-1,"none",-1,"",false,false),
        new ShopItem("category","premiumshop","Return to categories",-1,-1,"none",-1,"",false,false),
        new ShopItem("freeticket","premiumshop","Daily Skip Ticket",0,-1,"none",25,"Premium Account daily free skip ticket that can be used to completely skip the analysis wait time.\n[Affected by 5 ticket cap]",false,false),
        new ShopItem("premium","premiumshop","Premium Account",-1,-1,"none",24,"Upgrade your account to premium status. Premium removes banner ads and you will receive 2 extra desk storage, 1 free skip ticket per day and Analysis will only take 4 hours instead of 8.\n[One time payment, NOT a subscription]",true,false),
        new ShopItem("tickets1","premiumshop","1 Skip Ticket",-1,-1,"none",25,"1 skip ticket that can be used to completely skip the analysis wait time.\n[Not affected by 5 ticket cap]",true,false),
        new ShopItem("tickets5","premiumshop","5 Skip Tickets",-1,-1,"none",26,"5 skip tickets that can be used to completely skip the analysis wait time.\n[Not affected by 5 ticket cap]",true,false),
        new ShopItem("time30","premiumshop","30min. Skip Time",-1,-1,"none",27,"Watch an ad to get 30 minutes of analysis wait skip time.",false,true)

    };
    public GameObject content;
    public RectTransform contentRect;
    public GameObject shopButton;
    public string page = "category";
    public List<Sprite> shopIcons = new List<Sprite>();

    public GameObject itemView;
    public TextMeshProUGUI itemViewTitle;
    public TextMeshProUGUI itemViewRequirement;
    public TextMeshProUGUI itemViewDescription;
    public Image itemViewImage;

    public GameObject itemViewBuy;
    public GameObject itemViewIapBuy;
    public GameObject itemViewBought;
    public GameObject itemViewUnable;

    public TextMeshProUGUI buyButtonText;
    public TextMeshProUGUI iapBuyButtonText;

    public CodelessIAPButton iapScript;

    public ShopItem itemViewing;
    public Product productViewing;

    public GameObject closeSound;
    public GameObject purchaseSound;

    public Ads adScript;

    public GameObject adBubble;
    
    private void Awake()
    {
        saveScript = GameObject.Find("GameManager").GetComponent<SaveManager>();
        adScript = GameObject.Find("GameManager").GetComponent<Ads>();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (itemViewing!=null&&itemViewing.ad)
        {
            if (saveScript.gameData.timeClaimedToday > 0)
            {
                itemViewDescription.text = itemViewing.description + "\n[LEFT TODAY:" + (10-(saveScript.gameData.timeClaimedToday/(System.TimeSpan.TicksPerMinute*30))).ToString() + "/10]";
            }
            else
            {
                itemViewDescription.text = itemViewing.description + "\n[LEFT TODAY:10/10]";
            }
        }
    }
    public void OpenShopPage()
    {
        if (TimeManager.validTime && saveScript.gameData.prevDayPlayed < TimeManager.GetTime()/System.TimeSpan.TicksPerDay)
        {
            saveScript.gameData.prevDayPlayed = TimeManager.GetTime() / System.TimeSpan.TicksPerDay;
            saveScript.gameData.ticketsClaimedToday = 0;
            saveScript.gameData.timeClaimedToday = 0;
        }
        foreach (Transform child in content.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        int buttonAmount = 0;
        if (page!="category")
        {

        }
        for (int i = 0; i< shopItems.Count;i++)
        {
            if (shopItems[i].type == page)
            {
                if (shopItems[i].id!="freeticket"||(saveScript.gameData.premium&&saveScript.gameData.ticketsClaimedToday==0)) {
                    if (shopItems[i].id != "premium" || !saveScript.gameData.premium) {
                        CreateShopButton(shopItems[i]);
                        buttonAmount++;
                    }
                }
            }
        }
        contentRect.sizeDelta = new Vector2(600f,100f+(buttonAmount*110f));

    }
    public void CreateShopButton(ShopItem item)
    {
        ShopButton buttonScript = Instantiate(shopButton,content.transform).GetComponent<ShopButton>();
        buttonScript.titleText.text = item.text;
        buttonScript.item = item;
        buttonScript.shopScript = this;
        if (item.price==-1&&!item.iap&&!item.ad) {
            buttonScript.priceText.text = "";
            buttonScript.titleText.fontStyle &= ~FontStyles.Underline;
            buttonScript.titleText.rectTransform.localPosition = new Vector3(0f,-16f,0f);
        }
        else
        {
            if (!saveScript.CheckIfBought(item.id)) {
                if (item.requiredId=="none"&&item.requiredLevel==-1) {
                    if (item.price>0) {
                        buttonScript.priceText.text = "Price: " + item.price;
                    }
                    else
                    {
                        buttonScript.priceText.text = "Price: FREE";
                    }
                }
                else
                {
                    if ((saveScript.CheckIfBought(item.requiredId)|| item.requiredId=="none") && saveScript.GetPlayerLevel()>=item.requiredLevel)
                    {
                        buttonScript.priceText.text = "Price: " + item.price + " [UNLOCKED]";
                    }
                    else
                    {
                        buttonScript.priceText.text = "Price: " + item.price + " [LOCKED]";
                    }
                }
            }
            else
            {
                buttonScript.priceText.text = "BOUGHT";
            }
        }
        if (item.icon!=-1) {
            buttonScript.iconLeft.sprite = shopIcons[item.icon];
            buttonScript.iconRight.sprite = shopIcons[item.icon];
        }
        else
        {
            buttonScript.iconLeft.gameObject.SetActive(false);
            buttonScript.iconRight.gameObject.SetActive(false);
        }
    }
    public void CloseItemView()
    {
        iapScript.productId = "none";
        itemViewing = null;
        itemView.SetActive(false);
        Instantiate(closeSound, Camera.main.transform.position, transform.rotation);
    }
    public void OpenItemView(ShopItem item)
    {
        itemViewBuy.SetActive(false);
        itemViewBought.SetActive(false);
        itemViewUnable.SetActive(false);
        itemViewIapBuy.SetActive(false);
        itemViewing = item;
        itemViewImage.sprite = shopIcons[item.icon];
        itemViewTitle.text = item.text;
        string requirementText = "";
        if (item.price>=0)
        {
            if (saveScript.gameData.money >= item.price)
            {
                requirementText += "<color=#000000>";
            }
            else
            {
                requirementText += "<color=#800000>";
            }
            if (item.price>0) {
                requirementText += "Price: " + item.price + "</color>";
            }
            else
            {
                requirementText += "Price: " + "FREE" + "</color>";
            }
        }
        if (item.requiredLevel>0|| item.requiredId != "none")
        {
            if (saveScript.GetPlayerLevel()>=item.requiredLevel && (item.requiredId=="none" || saveScript.CheckIfBought(item.requiredId)))
            {
                requirementText += "<color=#000000>";
            }
            else
            {
                requirementText += "<color=#800000>";
            }
            requirementText += "\n" + "Needs:(";
        }
        if (item.requiredLevel > 0)
        {
            requirementText += "level:" + item.requiredLevel;
            if (item.requiredId != "none")
            {
                requirementText += ",";
            }
        }
        if (item.requiredId !="none")
        {
            requirementText += "item:" + item.requiredId + ")";
        }
        else if (item.requiredLevel > 0)
        {
            requirementText += ")";
        }
        itemViewRequirement.text = requirementText;
        itemViewDescription.text = item.description;
        if (item.iap)
        {
            itemViewRequirement.text = "<color=#000000>Price: " + productViewing.metadata.localizedPriceString + "</color>";
            itemViewDescription.text = item.description;
            itemViewBuy.SetActive(false);
            itemViewIapBuy.SetActive(true);
            iapBuyButtonText.text = productViewing.metadata.localizedPriceString;
        }
        if (item.ad)
        {
            itemViewRequirement.text = "<color=#000000>Price: " + "AD" + "</color>";

            buyButtonText.text = "WATCH AD";
            adBubble.SetActive(true);
        }
        else
        {
            adBubble.SetActive(false);
        }
        if (item.price == 0)
        {
            buyButtonText.text = "FREE";
        }
        if (!item.ad && item.price>0)
        {
            buyButtonText.text = "PURCHASE";
        }
        if (!item.iap) {
            if (saveScript.gameData.money >= item.price && saveScript.GetPlayerLevel() >= item.requiredLevel && (item.requiredId=="none"|| saveScript.CheckIfBought(item.requiredId)))
            {
                itemViewBuy.SetActive(true);
                itemViewBought.SetActive(false);
                itemViewUnable.SetActive(false);
            }
            else
            {
                itemViewBuy.SetActive(false);
                itemViewBought.SetActive(false);
                itemViewUnable.SetActive(true);
            }
            if (saveScript.CheckIfBought(item.id))
            {
                itemViewBuy.SetActive(false);
                itemViewBought.SetActive(true);
                itemViewUnable.SetActive(false);
            }
        }
        itemView.SetActive(true);
    }
    public void PurchaseItem()
    {
        if (!itemViewing.iap) {
            if (!itemViewing.ad) {
                if (saveScript.gameData.money >= itemViewing.price) {
                    Tutorial.AdvanceTutorial();
                    saveScript.gameData.money -= itemViewing.price;
                    if (itemViewing.id == "freeticket") {
                        if (saveScript.gameData.skipTickets < 5) {
                            saveScript.gameData.skipTickets++;
                            saveScript.gameData.ticketsClaimedToday++;
                            CloseItemView();
                            OpenShopPage();
                            Instantiate(purchaseSound, Camera.main.transform.position, transform.rotation);
                        }
                        else
                        {
                            saveScript.controlScript.popupScript.DisplayError("YOU HAVE TOO MANY TICKETS");

                        }
                    }
                    else
                    {
                        saveScript.gameData.purchasedShopItems.Add(itemViewing.id);
                        CloseItemView();
                        OpenShopPage();
                        Instantiate(purchaseSound, Camera.main.transform.position, transform.rotation);
                    }
                }
                else
                {
                    saveScript.controlScript.popupScript.DisplayError("NOT ENOUGH MONEY");
                }
            }
            else
            {
                if ((saveScript.gameData.timeClaimedToday / (System.TimeSpan.TicksPerMinute * 30))<10) {
                    adScript.ShowRewardedAd(itemViewing.id);
                }
                else
                {
                    saveScript.controlScript.popupScript.DisplayError("TODAYS LIMIT REACHED");
                }
            }
        }
        saveScript.WriteFile();
    }
    public void PurchaseIAPItem(Product product)
    {
        if (product.definition.id == "premium")
        {
            saveScript.gameData.premium = true;
        }
        if (product.definition.id == "tickets1")
        {
            saveScript.gameData.skipTickets += 1;
        }
        if (product.definition.id == "tickets5")
        {
            saveScript.gameData.skipTickets += 5;
        }
        saveScript.WriteFile();
        CloseItemView();
        OpenShopPage();
        Instantiate(purchaseSound, Camera.main.transform.position, transform.rotation);
    }
    public void PurchaseFailed()
    {
        saveScript.controlScript.popupScript.DisplayError("PURCHASE FAILED");
    }
    public void PremiumRestore(bool restored,string text)
    {
        if (restored)
        {
            saveScript.gameData.premium = true;
        }
        else
        {
            saveScript.controlScript.popupScript.DisplayError(text);
        }
    }
    private void OnEnable()
    {
        page = "category";
        OpenShopPage();
        itemView.SetActive(false);
    }
}
public class ShopItem
{
    public string id;
    public string type;
    public string text;
    public int price;
    public int requiredLevel;
    public string requiredId;
    public int icon;
    public string description;
    public bool iap;
    public bool ad;
    public ShopItem(string nId,string nType,string nText,int nPrice,int nRequiredLevel, string nRequiredId,int nIcon,string nDescription,bool nIap,bool nAd)
    {
        id = nId;
        type = nType;
        text = nText;
        price = nPrice;
        requiredLevel = nRequiredLevel;
        requiredId = nRequiredId;
        icon = nIcon;
        description = nDescription;
        iap = nIap;
        ad = nAd;
    }
}
