using UnityEngine;
using System.Collections.Generic;

public class CannonController : MonoBehaviour
{
    [Header("Disparo")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 15f;
    public int poolSize = 10;
    public float fireRate = 0.5f;

    [Header("Rotación")]
    public float rotationSpeed = 5f;

    [Header("Visual Effects")]
    public GameObject muzzleFlashPrefab; // Efecto de disparo
    public float muzzleFlashDuration = 0.1f;

    [Header("Screen Shake")]
    public bool enableScreenShake = true;
    public float shakeMagnitude = 0.1f;
    public float shakeDuration = 0.1f;

    private List<BulletMove> bulletPool = new List<BulletMove>();
    private float nextFireTime = 0f;

    void Start()
    {
        // Inicializar pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject b = Instantiate(bulletPrefab);
            b.SetActive(false);
            BulletMove bm = b.AddComponent<BulletMove>();
            bulletPool.Add(bm);
        }

        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    void Update()
    {
        AimAtMouse();

        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            ShootToMouse();
            nextFireTime = Time.time + fireRate;
        }
    }

    void AimAtMouse()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = Mathf.Abs(Camera.main.transform.position.z);

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;

        Vector2 dir = mouseWorld - transform.position;

        float targetAngle;

        if (dir.y < 0)
        {
            targetAngle = dir.x >= 0 ? 0f : 180f;
        }
        else
        {
            targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (targetAngle < 0) targetAngle += 360f;
            targetAngle = Mathf.Clamp(targetAngle, 0f, 180f);
        }

        float currentAngle = transform.localEulerAngles.z;
        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * rotationSpeed);
        transform.localRotation = Quaternion.Euler(0f, 0f, newAngle);
    }

    void ShootToMouse()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(mouseScreen);
        targetPos.z = 0f;

        Vector3 minScreen = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Mathf.Abs(Camera.main.transform.position.z)));
        Vector3 maxScreen = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Mathf.Abs(Camera.main.transform.position.z)));
        targetPos.x = Mathf.Clamp(targetPos.x, minScreen.x, maxScreen.x);
        targetPos.y = Mathf.Clamp(targetPos.y, minScreen.y, maxScreen.y);

        // Obtener bala del pool
        foreach (BulletMove bm in bulletPool)
        {
            if (!bm.gameObject.activeInHierarchy)
            {
                bm.transform.position = firePoint.position;
                bm.Init(targetPos, bulletSpeed);
                bm.gameObject.SetActive(true);
                break;
            }
        }

        // Reproducir sonido de disparo
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPlayerShoot();
        }

        // Spawn muzzle flash
        if (muzzleFlashPrefab != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);
            Destroy(flash, muzzleFlashDuration);
        }

        // Screen shake
        if (enableScreenShake && CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(shakeMagnitude, shakeDuration);
        }
    }
}
