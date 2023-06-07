using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected GameObject highLightGO;
    [SerializeField] string KbText = "Press E to interact", ControllerText = "Press O to interact";
    TMP_Text highlightText;
    protected virtual void Start()
    {
        highlightText = highLightGO.GetComponent<TMP_Text>();
        StopHighLight();
    }
    private void Update()
    {
        if (highlightText == null) return;

        if (GameManager.Instance.currentScheme == Controllers.KbMouse) highlightText.text = KbText;
        else highlightText.text = ControllerText;
        
    }
    public virtual void StartHighLight()
    {
        highLightGO.SetActive(true);
    }
    public virtual void StopHighLight()
    {
        highLightGO.SetActive(false);
    }
    public abstract void Interact(Transform player);
}
