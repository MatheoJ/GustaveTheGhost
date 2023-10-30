using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class HealScript : MonoBehaviour
{
    [SerializeField]
    private int healAmount = 10;
    public int HealAmount
    {
        get { return healAmount; }
        set { healAmount = value; }
    }
    [SerializeField]
    private int energyAmount = 10;
    public int EnergyAmount
    {
        get { return energyAmount; }
        set { energyAmount = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //get layer of "character"
        if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
                Destroy(gameObject);
                other.GetComponent<AbstractCharacter>().Heal(healAmount);
        }
    }
}
