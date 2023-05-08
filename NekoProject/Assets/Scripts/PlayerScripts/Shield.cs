using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float rotationSpeed, distanceFromPlayer;

    // Update is called once per frame
    void Update()
    {
        MoveToCursor();
    }

    void RotateAroundPlayer()
    {
        transform.RotateAround(target.position, Vector3.forward, rotationSpeed * Time.deltaTime);
    }

    void MoveToCursor()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 position = new Vector3(mousePos.x - target.position.x, mousePos.y - target.position.y, target.position.z);
        position = position.normalized * distanceFromPlayer;

        transform.right = position.normalized;
        transform.position = target.position + position;
    }
}
