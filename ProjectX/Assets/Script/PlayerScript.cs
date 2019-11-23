using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;



public class PlayerScript : MonoBehaviour
{
    CharacterController characterController;

    public float speed = 6.0f;
    
    public float gravity = 20.0f;

    private Vector3 moveDirection = Vector3.zero;

    bool isMoving = false;

    bool BatteryCollected = false;

    float Health = 100;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Movement();
    }
    void Movement()
    {
        if (characterController.isGrounded)
        {


            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            moveDirection *= speed;


        }

        moveDirection.y -= gravity * Time.deltaTime;

        if(Input.GetKeyDown("d") || Input.GetKeyDown("a") || Input.GetKeyDown("w")|| Input.GetKeyDown("s"))
        {
            characterController.Move(moveDirection * Time.deltaTime);
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
        
    }
    void Battery()
    {
        if(Health >= 100)
        {
            Health = 100;
        }
        if(isMoving==true)
        {

            Health -= Time.deltaTime + 0.5f;

        }
        if(BatteryCollected ==true)
        {
            Health += 10;
            BatteryCollected = false;

        }
        if( Health <= 0)
        {
            SceneManager.LoadScene("Scene1");
        }
    }
    
    
}