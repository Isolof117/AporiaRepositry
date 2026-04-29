using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponBase : MonoBehaviour
{

    // Assets
    public Camera playerCamera;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public QTE_MovingBox MovingBox;
    public SkillCheck KeyInputs;

    // Bullet Attributes
    public float bulletVelocity, bulletSpread, fireRate;
    private float lifeTime = 3;

    // Conditions
    private bool isShooting, readyToShoot, allowReset = true;
    private int currentBurstBullet; 
    public int bulletsPerBurst = 3;

    // Reloading
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;
    public AudioSource ReloadAudio;

    // UI
    //public TextMeshProUGUI QTEPopUp;

    // States
    public enum ShootingMode
    {
        Auto,
        Single,
        Burst
    }  
    public ShootingMode currentMode;

    // Constructors
    private void Awake()
    {
        readyToShoot = true;
        currentBurstBullet = bulletsPerBurst;
        bulletsLeft = magazineSize;
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentMode == ShootingMode.Auto && bulletsLeft > 0)
        {
            isShooting = Input.GetMouseButton(0);
        }
        else if (currentMode == ShootingMode.Burst || currentMode == ShootingMode.Single )
        {
            isShooting = Input.GetMouseButtonDown(0);
        }

        if (readyToShoot && isShooting)
        {
            currentBurstBullet = bulletsPerBurst;
            Fire();
        }

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !isReloading)
        {
            QTESelect();
        }

        if (bulletsLeft < 1)
        {
            readyToShoot = false;
        }

        if (bulletsLeft <= 0 && !isReloading && isShooting && !readyToShoot)
        {
            bulletsLeft = 0;
            isShooting = false;
            QTESelect();
        }

        if (AmmoManager.Instance.ammoCount != null)
        {
            AmmoManager.Instance.ammoCount.text = $"{bulletsLeft/bulletsPerBurst}/{magazineSize/bulletsPerBurst}";
        }
    }

    void QTESelect()
    {
        bulletsLeft = 0;
        int Choice = Random.Range(0, 2);
        switch (Choice)
        {
            case 0:
                {
                        if (!KeyInputs.isVisible && !MovingBox.QTEActive)
                        KeyInputs.StartQTE(this);
                        break;
                }
            case 1:
                {
                        if (!MovingBox.QTEActive && !KeyInputs.isVisible)
                        StartCoroutine(MovingBox.DoQTE(this));
                        break;
                }
            default:
                {
                        break;
                }
        };
    }

    public void Fire()
    {
        bulletsLeft--;

        readyToShoot = false;

        Vector3 shotDirection = CalculateDirectionAndSpread().normalized;

        // Fix Bullet Orientation
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        bullet.transform.forward = shotDirection;

        bullet.GetComponent<Rigidbody>().AddForce(shotDirection * bulletVelocity, ForceMode.Impulse);
        StartCoroutine(DestroyBullet(bullet, lifeTime));

        if (allowReset)
        {
            Invoke("ResetShot", fireRate);
            allowReset = false;
        }

        if (currentMode == ShootingMode.Burst && currentBurstBullet > 1)
        {
            currentBurstBullet--;
            Invoke("Fire", fireRate);
        }
    }

    public void Reload()
    {

        // Insert QTE
        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }


    private void ReloadCompleted()
    {
        isReloading = false;
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;


        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - firePoint.position;

        float x = UnityEngine.Random.Range(-bulletSpread, bulletSpread);
        float y = UnityEngine.Random.Range(-bulletSpread, bulletSpread);

        return direction + new Vector3(x, y, 0);

    }

    private IEnumerator DestroyBullet(GameObject bullet, float lifeTime)
    {

        yield return new WaitForSeconds (lifeTime);
            
        Destroy(bullet);

    }
}