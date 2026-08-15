using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public Transform respawn;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DeathPoint"))
        {
            transform.position = respawn.position;
        }
    }
}
