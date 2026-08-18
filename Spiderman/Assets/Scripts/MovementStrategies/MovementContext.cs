using UnityEngine;

public struct MovementContext
{
    public Rigidbody Rb;
    public Vector3 InputDirection;
    public Vector2 RawMove;   // new — raw 2D input, unprojected
    public float DeltaTime;
    public bool Grounded;
    public bool TouchingWall;
    public Vector3 WallNormal;
}