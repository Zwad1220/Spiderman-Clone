using UnityEngine;

public struct MovementContext {
    public Rigidbody Rb;
    public Vector3 InputDirection;
    public float DeltaTime;
    public bool Grounded;

    public bool TouchingWall;
    public Vector3 WallNormal;   // points away from the wall surface
}
