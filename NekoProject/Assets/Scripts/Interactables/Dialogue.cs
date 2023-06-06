using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : Interactable
{
    [SerializeField] TMP_Text dialogueText;
    [SerializeField] string[] phrases;
    int phraseIndex;
    public override void Interact(Transform player)
    {
        if(phraseIndex >= phrases.Length) 
        {
            StopHighLight();
        }
        else
        {
            dialogueText.gameObject.SetActive(true);
            highLightGO.SetActive(false);
            int _index = phraseIndex;
            dialogueText.text = phrases[_index];
            phraseIndex++;
        }
    }

    public override void StopHighLight()
    {
        base.StopHighLight();

        dialogueText.gameObject.SetActive(false);
        phraseIndex = 0;
    }
}
