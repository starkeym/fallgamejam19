using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;



public class PlayerScript : MonoBehaviour
{
    CharacterController characterController;

    GameObject Attack;

    public float speed = 6.0f;
    
    public float gravity = 20.0f;

    private Vector3 moveDirection = Vector3.zero;

    bool isMoving = false;

    bool BatteryCollected = false;

    public static float Health = 1;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Attack = GameObject.FindGameObjectWithTag("Attackablearea");
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

        if(Input.GetKey("d") || Input.GetKey("a") || Input.GetKey("w")|| Input.GetKey("s"))
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
        if(Health >= 1)
        {
            Health = 1;
        }
        if(isMoving==true)
        {

            Health = 0.4f;

        }
        if(BatteryCollected ==true)
        {
            Health += 0.1f;
            BatteryCollected = false;

        }
        if( Health <= 0)
        {
            SceneManager.LoadScene("Scene1");
        }
    }
    
    
}