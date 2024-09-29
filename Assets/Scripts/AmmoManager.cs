using UnityEngine;
public class AmmoManager : MonoBehaviour
{

    private int ammoCount;
    public GameObject bulletHolder;
    private GameObject[] bulletModels;

    private void Start()
    {
        bulletModels = new GameObject[bulletHolder.transform.childCount];
        for (int i = 0; i < bulletHolder.transform.childCount; i++)
        {
            bulletModels[i] = bulletHolder.transform.GetChild(i).gameObject;
        }
        UpdateAmmoUI();
    }

    public void UseBullet()
    {
        if (ammoCount > 0)
        {
            ammoCount--;
            UpdateAmmoUI();
        }
    }

    private void UpdateAmmoUI()
    {
        for (int i = 0; i < bulletModels.Length; i++)
        {
            bulletModels[i].SetActive(i < ammoCount);
        }
    }

    public void setAmmoCount(int ammoCount)
    {
        this.ammoCount = ammoCount;
    }
}
