using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailEffect : MonoBehaviour
{
    public GameObject trail;
    public Animator an;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            an.SetBool("IsAttacking",true);
            trail.SetActive(true);
            StartCoroutine(timer());
        }
    }
    IEnumerator timer() {
        yield return new WaitForSeconds(0.3f);

        an.SetBool("IsAttacking", false);
        
        trail.SetActive(false);
    }
}
