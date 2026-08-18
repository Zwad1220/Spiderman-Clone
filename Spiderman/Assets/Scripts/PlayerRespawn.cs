using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public Transform respawn;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("DeathPoint"))
        {
            transform.position = respawn.position;
        }
    }
}
