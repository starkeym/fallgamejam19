using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    GameObject Attack;
    public GameObject RobotSignal;
    float timer = 3;
    // Start is called before the first frame update
    void Start()
    {
        Attack = GameObject.FindGameObjectWithTag("Attackablearea");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if((other.gameObject.tag=="Enemy" && FieldOfView.canbeKilled==true && Input.GetMouseButtonDown(0)) || (other.gameObject.tag =="Standingenemy" && FieldOfView.canbeKilled == true))
        {
            Destroy(other.gameObject);
            
        }
        if ((other.gameObject.tag == "Robot" && FieldOfView.canbeKilled == true && Input.GetMouseButtonDown(0)) || (other.gameObject.tag == "Standingrobot" && FieldOfView.canbeKilled == true))
        {
            
            Destroy(other.gameObject);
            Instantiate(RobotSignal, gameObject.transform.position, Quaternion.identity);
            RobotSignal = GameObject.FindGameObjectWithTag("Robotsignal");
           
           


        }
    }
   
}
