using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxScript : MonoBehaviour
{
    public GameObject HealOrEnergy;
    private GameObject Destroy_box;
    private GameObject Box;
    private bool CollisionHandled = false;
    // Start is called before the first frame update
    void Start()
    {
        // get chidren object
        Destroy_box = transform.Find("Destroy_box").gameObject;
        Box = transform.Find("Box001").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void HandleCollision()
    {
        print("handle collision");
        if (CollisionHandled)
        {
            return;
        }
        print("collision");
        Destroy_box.SetActive(true);
        if (HealOrEnergy != null)
            HealOrEnergy.SetActive(true);
        Box.SetActive(false);

        // fade out
        Destroy(gameObject, 5f);
        CollisionHandled = true;
    }

}

