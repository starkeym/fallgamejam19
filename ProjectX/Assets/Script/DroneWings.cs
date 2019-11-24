using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneWings : MonoBehaviour
{
    Rigidbody rg;
    // Start is called before the first frame update
    void Start()
    {
        rg = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        gameObject.transform.Rotate(transform.right*3);
    }
}
