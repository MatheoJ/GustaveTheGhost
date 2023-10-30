using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubBoxScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
       /* //if not parent object
        if(transform.parent == collision.transform)
        {
            return;
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet") || collision.gameObject.layer == LayerMask.NameToLayer("Sword"))
        {
            // Notify the parent object (BoxOfPandora) about the collision.
            transform.parent.GetComponent<BoxScript>().HandleCollision();
        }*/
    }
}
