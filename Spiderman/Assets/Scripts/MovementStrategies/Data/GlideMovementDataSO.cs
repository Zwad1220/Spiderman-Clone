using UnityEngine;

[CreateAssetMenu(fileName = "GlideMovementDataSO", menuName = "Scriptable Objects/GlideMovementDataSO")]
public class GlideMovementDataSO : ScriptableObject
{
    public float glideGravityScale = 0.3f;   // how strongly gravity pulls you down while gliding
    public float maxFallSpeed = 6f;          // terminal vertical speed
    public float glideRatio = 2.2f;          // horizontal distance gained per unit of fall (tune this — higher = flatter, faster glide)
    public float forwardAcceleration = 8f;   // how quickly horizontal speed catches up to the glide-ratio target
    public float turnRate = 90f;             // deg/sec steering responsiveness
}
