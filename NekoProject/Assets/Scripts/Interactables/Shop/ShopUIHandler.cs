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

    public void HighLight(ShopSlot shopSlot)
    {
        lastSelectedSlot = shopSlot;
        highLightBorder.transform.position = shopSlot.transform.position;
        descriptionText.text = shopSlot.Description;
        costText.text = shopSlot.Cost.ToString();
    }

    private void OnEnable()
    {
        GameManager.Instance.ControllerConected += ControllerConected;
        if(GameManager.Instance.currentScheme == Controllers.Controller) EventSystem.current.SetSelectedGameObject(shopSlots[0].gameObject);
        lastSelectedSlot = shopSlots[0];
    }

    private void OnDisable()
    {
        GameManager.Instance.ControllerConected -= ControllerConected;
    }

    void ControllerConected()
    {
        if (EventSystem.current.alreadySelecting) return;

        EventSystem.current.SetSelectedGameObject(shopSlots[0].gameObject);
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
