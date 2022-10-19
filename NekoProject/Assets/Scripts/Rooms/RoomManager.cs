using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    PlayerController player;

    [SerializeField] Image blackFadeTransitionImage;

    public enum Direction { right, left, up, down }
    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }


    // Devuelve true si el jugador esta moviendose entre salas, sin el control del jugador
    bool playerTransitioning;

    bool playerArrived;

    // Este metodo se llama cuando el jugador ha collisionado con el trigger de salida de una sala
    public void RoomExited()
    { StartCoroutine(RoomExited_IEnumerator()); }
    public IEnumerator RoomExited_IEnumerator()
    {
        // Si el jugador se acerca al borde de una sala y toca el trigger de salida de la sala,
        // empezar proceso de transicion
        if (!playerTransitioning)
            playerTransitioning = true;
        // Si la varible de "playerTransitioning" ya estaba a true, significa que el jugador
        // estaba transicionando entre salas y ha llegado a la nueva sala
        else
        {
            playerArrived = true;
            playerTransitioning = false;
            yield break;
        }

        float transitionTime = .5f;

        TransitionIn(transitionTime);

        // Mover el jugador a la direccion de la siguiente sala


        yield return new WaitForSeconds(transitionTime);

        // Mover el jugador al trigger de la sala anterior

        TransitionOut(transitionTime);

        // Esperar hasta que el jugador llege al trigger de la nueva sala
        yield return new WaitUntil(() => playerTransitioning == false);


        // Devolver control al jugador
    }

    void TransitionIn(float time)
    {
        blackFadeTransitionImage.DOFade(1, time);
    }

    void TransitionOut(float time)
    {
        blackFadeTransitionImage.DOFade(0, time);
    }

}
