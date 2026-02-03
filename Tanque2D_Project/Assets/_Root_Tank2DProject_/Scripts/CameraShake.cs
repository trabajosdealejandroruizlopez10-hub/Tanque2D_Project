using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("Shake Settings")]
    public bool enableShake = true;

    private Vector3 originalPosition;
    private bool isShaking = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    public void Shake(float magnitude, float duration)
    {
        if (!enableShake) return;

        if (!isShaking)
        {
            StartCoroutine(ShakeCoroutine(magnitude, duration));
        }
    }

    IEnumerator ShakeCoroutine(float magnitude, float duration)
    {
        isShaking = true;
        originalPosition = transform.localPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        isShaking = false;
    }

    // Método para shake más intenso (por ejemplo al recibir daño)
    public void ShakeHeavy()
    {
        Shake(0.3f, 0.2f);
    }

    // Método para shake suave (por ejemplo al disparar)
    public void ShakeLight()
    {
        Shake(0.05f, 0.1f);
    }
}