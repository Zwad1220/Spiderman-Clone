using UnityEngine;

[CreateAssetMenu(fileName = "WallCrawlMovementDataSO", menuName = "Scriptable Objects/WallCrawlMovementDataSO")]
public class WallCrawlMovementDataSO : ScriptableObject
{
    public float climbSpeed = 6f;          // terminal vertical speed
    public float acceleration = 8f;   // how quickly horizontal speed catches up to the glide-ratio target
    public float turnRate = 90f;   
}
