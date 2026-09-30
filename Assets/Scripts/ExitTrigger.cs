using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    public string playerTag = "Player";
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        GameManager.Instance.Win();
    }
}
