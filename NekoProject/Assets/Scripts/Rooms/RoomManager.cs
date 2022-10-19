using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    PlayerController player;

    [SerializeField] Image blackFadeTransitionImage;

    CinemachineVirtualCamera cam;

    public enum Direction { right, left, up, down }
    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();

        cam = FindObjectOfType<CinemachineVirtualCamera>();
    }

    // Devuelve true si el jugador esta moviendose entre salas, sin el control del jugador
    bool playerTransitioning;

    // La sala a la que se esta accediendo
    RoomLogic newRoom;

    // Este metodo se llama cuando el jugador ha collisionado con el trigger de salida de una sala
    public void RoomExited(RoomLogic roomExitTriggered, RoomManager.Direction dir)
    { StartCoroutine(RoomExited_IEnumerator(roomExitTriggered, dir)); }
    public IEnumerator RoomExited_IEnumerator(RoomLogic roomExitTriggered, RoomManager.Direction dir)
    {
        // Si el jugador se acerca al borde de una sala y toca el trigger de salida de la sala,
        // empezar proceso de transicion
        if (!playerTransitioning)
            playerTransitioning = true;
        // Si la varible de "playerTransitioning" ya estaba a true, significa que el jugador
        // estaba transicionando entre salas y ha llegado a la nueva sala
        else
        {
            newRoom = roomExitTriggered;
            playerTransitioning = false;
            yield break;
        }

        // Controlar al jugador para que vaya a la siguiente sala
        if (dir == Direction.left)
            player.ControlPlayer(-1);
        else if (dir == Direction.right)
            player.ControlPlayer(1);
        else if (dir == Direction.down)
            player.ControlPlayer(0);
        else if (dir == Direction.up)
        {
            player.ControlPlayer(.8f);
            player.AddVelocityToRB(new Vector2(30, 40));
        }

        // Esperar hasta que el jugador llege al trigger de la nueva sala
        yield return new WaitUntil(() => playerTransitioning == false);

        // Cambio de camara, pasar de la sala anterior a la nueva
        cam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = newRoom.cameraConfiner.GetComponent<PolygonCollider2D>();


        // AQUI SPAWNEAR ENEMIGOS DE LA NUEVA SALA Y DEMAS CAMBIOS

        if (dir == Direction.left || dir == Direction.right)
            // Alejar al jugador del borde de la sala nueva
            yield return new WaitForSeconds(.2f);
        else if (dir == Direction.up || dir == Direction.down)
            yield return new WaitForSeconds(.6f);

        // Volver a controlar al jugador
        player.UnControl();


        // Version alternativa
        #region

        //if (playerCannotActivateTriggers)
        //{
        //    newRoom = triggerActivated_Tr.GetComponentInParent<RoomLogic>();
        //    yield break;
        //}

        //// Si el jugador se acerca al borde de una sala y toca el trigger de salida de la sala,
        //// empezar proceso de transicion
        //if (!playerTransitioning)
        //{
        //    playerTransitioning = true;
        //    // No dejar al jugador collisionar con triggers de este tipo
        //    playerCannotActivateTriggers = true;
        //    // Desactivar el trigger antiguo
        //    triggerActivated_Tr.gameObject.SetActive(false);
        //}
        //// Si la varible de "playerTransitioning" ya estaba a true, significa que el jugador
        //// estaba transicionando entre salas y ha llegado a la nueva sala
        //else
        //{
        //    playerArrived = true;
        //    playerTransitioning = false;
        //    yield break;
        //}

        //float transitionTime = .5f;

        //TransitionIn(transitionTime);

        //// Mover el jugador a la direccion de la siguiente sala
        //player.ControlPlayer(1);

        //yield return new WaitForSeconds(2);

        //// Mover el jugador al trigger de la sala anterior
        //player.transform.position = triggerActivated_Tr.position;
        //// A partir de ahora, dejar que collisione con los triggers
        //// Para saber cuando ha llegado a la siguiente sala
        //playerCannotActivateTriggers = false;

        //// Cambio de camara, pasar de la sala anterior a la nueva
        //cam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = newRoom.cameraConfiner.GetComponent<PolygonCollider2D>();

        //TransitionOut(transitionTime);

        //// Esperar hasta que el jugador llege al trigger de la nueva sala
        //yield return new WaitUntil(() => playerTransitioning == false);

        //// Devolver control al jugador
        //player.UnControl();

        #endregion
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
