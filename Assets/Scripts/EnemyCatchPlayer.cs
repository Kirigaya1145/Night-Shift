using UnityEngine;

public class EnemyCatchPlayer : MonoBehaviour
{
    public string playerTag = "Player";
    EnemyFSM fsm;

    void Awake() 
    { 
        fsm = GetComponentInParent<EnemyFSM>(); 
    }
    void OnTriggerEnter(Collider other) 
    { 
        TryCatch(other); 
    }
    void OnTriggerStay(Collider other) 
    { 
        TryCatch(other); 
    }

    void TryCatch(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (fsm != null && fsm.currentState == EnemyFSM.State.Flee) return;
        if (GameManager.Instance != null) GameManager.Instance.Lose();
    }
}
