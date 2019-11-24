using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class Slice : MonoBehaviour
{
    public GameObject pos;
    public LayerMask m_LayerMask;
    public Material CutMat;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = pos.transform.position;
        gameObject.transform.rotation = pos.transform.rotation;
        if (Input.GetMouseButtonDown(0))
        {
            Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale , Quaternion.identity, m_LayerMask);
            foreach (var item in hitColliders)
            {
                SlicedHull SlicedObj = kes(item.gameObject,CutMat);
                GameObject sliced1 = SlicedObj.CreateUpperHull(item.gameObject,CutMat);
                GameObject sliced2 = SlicedObj.CreateLowerHull(item.gameObject,CutMat);
                addComponent(sliced1);
                addComponent(sliced2);
                Destroy(item.gameObject);
            }
        }

    }

    SlicedHull kes(GameObject obj, Material mat=null)
    {

        return obj.Slice(transform.position, transform.up, mat);
    }

    void addComponent(GameObject obj)
    {

        obj.AddComponent<MeshCollider>().convex = true;
        obj.AddComponent<Rigidbody>();
        obj.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
        obj.GetComponent<Rigidbody>().AddExplosionForce(100, gameObject.transform.position, 10);

    }

}
