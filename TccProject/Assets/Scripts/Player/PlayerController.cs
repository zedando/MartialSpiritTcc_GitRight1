using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 20f;
    public float xRange = 16f;
    public float zRange1 = 17f;
    public float zRange2 = -1f;

    public GameObject projectilePrefab;

    private bool isInvisible = false;

    private Vector2 movementInput;
    public GameObject Player;



    //Send Menssages

    /*private void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    private void OnFire()
    {
       Instantiate(projectilePrefab, transform.position, projectilePrefab.transform.rotation);
    }*/

    //Unity events

    public void OnMoveEvent(InputAction.CallbackContext value)
    {
        movementInput = value.ReadValue<Vector2>();
    }

    

    private void Update()
    {
        //Movimenta o player
        Vector3 movement = new Vector3(movementInput.x, 0f, movementInput.y) * speed * Time.deltaTime;
        transform.Translate(movement);

        //Limites(eixo x) do  player 
        if (transform.position.x < -xRange)
        {
            transform.position = new Vector3(-xRange, transform.position.y, transform.position.z);
        }
        else if (transform.position.x > xRange)
        {
            transform.position = new Vector3(xRange, transform.position.y, transform.position.z);
        }

        //Limites(eixo z) do  player 
        if (transform.position.z < zRange2) // Ajuste aqui
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zRange2);
        }
        else if (transform.position.z > zRange1)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zRange1);
        }
    }


    
}

   
