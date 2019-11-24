using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using EzySlice;



public class PlayerScriptFinal : MonoBehaviour
{
    public GameObject trail;
    public Animator an;


    ////////Slicer
    public GameObject SlicerPlane;
    public GameObject SlicerPos;
    public LayerMask m_LayerMask;
    public Material CutMat;

    /////////////



    CharacterController characterController;
    public float speed = 6.0f;
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
        Attack();
    }

    void Attack()
    {

        SlicerPlane.transform.position = SlicerPos.transform.position;
        SlicerPlane.transform.rotation = SlicerPos.transform.rotation;
        
        if (Input.GetMouseButtonDown(0))
        {
            //Attack animation and effects
            an.SetBool("IsAttacking", true);
            trail.SetActive(true);
            StartCoroutine(timer());
            StartCoroutine(Trailtimer());




            //////////////Slicer
            Collider[] hitColliders = Physics.OverlapBox(SlicerPlane.transform.position, SlicerPlane.transform.localScale, Quaternion.identity, m_LayerMask);
            foreach (var item in hitColliders)
            {
                SlicedHull SlicedObj = slc(item.gameObject, CutMat);
                GameObject sliced1 = SlicedObj.CreateUpperHull(item.gameObject, CutMat);
                GameObject sliced2 = SlicedObj.CreateLowerHull(item.gameObject, CutMat);
                addComponent(sliced1);
                addComponent(sliced2);
                Destroy(item.gameObject);
            }
        }



    }


    void Movement()
    {


            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            moveDirection *= speed;

            characterController.Move(moveDirection * Time.deltaTime);
        if (Input.GetKey("a")|| Input.GetKey("s") || Input.GetKey("d") || Input.GetKey("w"))
        {

            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
        if (Input.GetAxis("Horizontal")==0&& Input.GetAxis("Vertical")==0)
        {
            isMoving = false;
            an.SetBool("IsWalking",false);
        }
        else
        {
            isMoving = true;
            an.SetBool("IsWalking", true);

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

    
    /////////VFX AND ANIMATION
   
    IEnumerator timer()
    {
        yield return new WaitForSeconds(0.1f);

        an.SetBool("IsAttacking", false);

    }
    IEnumerator Trailtimer()
    {
        yield return new WaitForSeconds(0.22f);

        trail.SetActive(false);
    }



    /////////Slicer
    SlicedHull slc(GameObject obj, Material mat = null)
    {

        return obj.Slice(SlicerPlane.transform.position,  SlicerPlane.transform.up,  mat);
    }

    void addComponent(GameObject obj)
    {

        obj.AddComponent<MeshCollider>().convex = true;
        obj.AddComponent<Rigidbody>();
        obj.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
        obj.GetComponent<Rigidbody>().AddExplosionForce(100, SlicerPlane.transform.position, 10);

    }

}