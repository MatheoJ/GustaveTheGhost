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
        if (CollisionHandled)
        {
            return;
        }
        Destroy_box.SetActive(true);
        if (HealOrEnergy != null)
            HealOrEnergy.SetActive(true);
        Box.SetActive(false);
        AudioManager.Instance.PlaySound("broken_wood", 0.1f);
        // fade out
        Destroy(gameObject, 5f);
        CollisionHandled = true;
    }

}

