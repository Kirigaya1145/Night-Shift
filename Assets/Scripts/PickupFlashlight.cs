using UnityEngine;

public class PickupFlashlight : MonoBehaviour
{
    public string playerTag = "Player";
    public float activeDuration = 60f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        PlayerFlashlight pf = other.GetComponent<PlayerFlashlight>();
        if (pf != null)
        {
            pf.PickUp(activeDuration);
            FlashlighSpawn.Instance?.NotifyPickUp();
            Destroy(gameObject);
        }
    }
}
