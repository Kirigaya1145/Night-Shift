using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 localOffset = new Vector3(0, 6, -6);
    public float lookHeight = 1f;
    public float positionSmooth = 8f;
    public float yawSmooth = 4f;

    float currentYaw;

    void Start()
    {
        if (target != null) currentYaw = target.eulerAngles.y;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Karena Player.cs sekarang cuma mengubah rotasinya sendiri saat gerak maju,
        // target.eulerAngles.y di sini juga jadi "diam" saat mundur/menyamping,
        // jadi currentYaw ikut berhenti dan tidak muter terus.
        currentYaw = Mathf.LerpAngle(currentYaw, target.eulerAngles.y, yawSmooth * Time.deltaTime);

        Quaternion yawRotation = Quaternion.Euler(0, currentYaw, 0);
        Vector3 desiredPosition = target.position + yawRotation * localOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, positionSmooth * Time.deltaTime);

        Vector3 lookPoint = target.position + Vector3.up * lookHeight;
        transform.rotation = Quaternion.LookRotation(lookPoint - transform.position, Vector3.up);
    }
}