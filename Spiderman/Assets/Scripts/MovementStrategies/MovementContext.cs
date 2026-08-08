using UnityEngine;

public struct MovementContext{
    public Rigidbody Rb;
    public Vector3 InputDirection;   // world-space, from orientation-relative input
    public float DeltaTime;
    public bool Grounded;
}
