using UnityEngine;

public class CannonRotationDebug : MonoBehaviour
{
    void Update()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector2 dir = mouseWorld - transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        Debug.Log("Angle raw: " + angle);

        // Convertimos a 0–360
        if (angle < 0) angle += 360f;

        Debug.Log("Angle 0-360: " + angle);

        // SIN CLAMP, rotación libre
        transform.localRotation = Quaternion.Euler(0, 0, angle);

        Debug.Log("Applied rotation Z: " + transform.localEulerAngles.z);
    }
}

