//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Shoot : MonoBehaviour
//{
//    public GameObject bulletPrefab;

//    private PlayerController playerController;
//    protected AbstractCharacter _Character { get; set; }

//    private AmmoManager ammoManager;

//    private void Awake()
//    {
//        playerController = GetComponent<PlayerController>();
//        _Character = GetComponentInChildren<AbstractCharacter>();
//        ammoManager = GameObject.Find("BulletContainer").GetComponent<AmmoManager>();
//        ammoManager.setAmmoCount(_Character.GetCurrentAmmo());
//    }
//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        CheckShoot();
//    }

//    public void CheckShoot()
//    {
//        if (Input.GetButtonDown("Fire1"))
//        {
//            if (_Character.canShoot())
//            {
//                Debug.Log("Shoot");
//                _Character.SetCurrentAmmo(_Character.GetCurrentAmmo() - 1);

//                Vector3 bulletPosition = _Character.GetEyePosition();
//                Vector3 bulletDirection = _Character.GetForward();
//                bulletPosition.y -= 0.5f;
//                bulletPosition += bulletDirection * 0.5f;
//                // 90 degree or -90 degree according to the direction
//                Quaternion quaternion = Quaternion.Euler(90, 0, 0);
//                if (bulletDirection.x < 0)
//                {
//                    quaternion = Quaternion.Euler(-90, 0, 0);
//                }
//                GameObject bullet = Instantiate(bulletPrefab, bulletPosition, quaternion);
//                bullet.GetComponent<Bullet>().SetDirection(bulletDirection);

//                // Update the ammo UI
//                ammoManager.UseBullet();
//            }
            
//        }
//    }
//}
