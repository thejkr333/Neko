using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopUIHandler : MonoBehaviour
{
    [SerializeField] GameObject highLightBorder;
    [SerializeField] ShopSlot[] shopSlots;
    [SerializeField] TMP_Text descriptionText, costText;

    ShopSlot lastSelectedSlot;
    void Awake()
    {
        for (int i = 0; i < shopSlots.Length; i++) 
        {
            shopSlots[i].Clicked += HighLight;
        }
        gameObject.SetActive(false);
    }

    void HighLight(ShopSlot shopSlot)
    {
        lastSelectedSlot = shopSlot;
        highLightBorder.transform.position = shopSlot.transform.position;
        descriptionText.text = shopSlot.Description;
        costText.text = shopSlot.Cost.ToString();
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(shopSlots[0].gameObject);
        lastSelectedSlot = shopSlots[0];
    }

    public void Buy()
    {
        PlayerStorage playerStorage = FindObjectOfType<PlayerStorage>();
        if(playerStorage.Coins >= lastSelectedSlot.Cost)
        {
            playerStorage.UnlockBooster(lastSelectedSlot.Booster);
        }
    }

    public void Back()
    {
        GameManager.Instance.EnablePlayerInputs();
        GameManager.Instance.DisableUIInputs();
        gameObject.SetActive(false);
    }
}
