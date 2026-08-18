using UnityEngine;

public class WalkMovement : IMovementStrategy
{
    public float groundDrag;
    public float groundSpeed = 16f;
    public float airSpeed = 3f;

    public void Move(MovementContext ctx){
        // if (!ctx.Grounded) return;
        ctx.Rb.useGravity = true;
        ctx.Rb.linearDamping = ctx.Grounded ? groundDrag : 0f;
        float speed = ctx.Grounded? groundSpeed : airSpeed;

        ctx.Rb.AddForce(ctx.InputDirection * speed, ForceMode.Force);
    }
}
