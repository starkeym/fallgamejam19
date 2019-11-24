using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    GameObject Attack;
    public GameObject RobotSignal;
    float timer = 3;
    bool swordactive = false;
    // Start is called before the first frame update
    void Start()
    {
        Attack = GameObject.FindGameObjectWithTag("Attackablearea");
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            StartCoroutine(countdown());
        }
        
       
    }
    private void OnTriggerStay(Collider other)
    {
        if((other.gameObject.tag=="Enemy" && FieldOfView.canbeKilled==true ) || (other.gameObject.tag =="Standingenemy" && FieldOfView.canbeKilled == true ))
        {
            Destroy(other.gameObject);
            
        }
        if ((other.gameObject.tag == "Robot" && FieldOfView.canbeKilled == true) || (other.gameObject.tag == "Standingrobot" && FieldOfView.canbeKilled == true ))
        {
            
            Destroy(other.gameObject);
            Instantiate(RobotSignal, gameObject.transform.position, Quaternion.identity);
            RobotSignal = GameObject.FindGameObjectWithTag("Robotsignal");
           
           


        }
    }
    private void OnTriggerEnter(Collider other)
    {
       
        if ((other.gameObject.tag == "Enemy" && FieldOfView.canbeKilled == true )  || (other.gameObject.tag == "Standingenemy" && FieldOfView.canbeKilled == true ))
        {
            Destroy(other.gameObject);

        }
        if ((other.gameObject.tag == "Robot" && FieldOfView.canbeKilled == true ) || (other.gameObject.tag == "Standingrobot" && FieldOfView.canbeKilled == true ))
        {

            Destroy(other.gameObject);
            Instantiate(RobotSignal, gameObject.transform.position, Quaternion.identity);
            RobotSignal = GameObject.FindGameObjectWithTag("Robotsignal");




        }
    }
    IEnumerator countdown()
    {
        gameObject.GetComponent<BoxCollider>().enabled = true;
        yield return new WaitForSeconds(0.1f);
         gameObject.GetComponent<BoxCollider>().enabled = false;
    }

}
