using System.Data.Common;
using UnityEngine;

public class GlideMovement : IMovementStrategy
{

    // Glide variables
    readonly GlideMovementDataSO data;
    public GlideMovement(GlideMovementDataSO data){
        this.data = data;
    }

    public void Move(MovementContext ctx)
    {
        // if (ctx.Grounded) return;

        Rigidbody rb = ctx.Rb;
        Vector3 velocity = rb.linearVelocity;

        // Steer facing directly from input, independent of current velocity —
        // this avoids the LookRotation(up = velocity) issue entirely.
        if (ctx.InputDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(ctx.InputDirection.normalized, Vector3.up);
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRot, data.turnRate * ctx.DeltaTime));
        }

        Vector3 facingForward = rb.transform.forward;

        // Vertical: gravity-driven descent, clamped to a terminal fall speed
        float verticalVelocity = velocity.y + Physics.gravity.y * data.glideGravityScale * ctx.DeltaTime;
        verticalVelocity = Mathf.Max(verticalVelocity, -data.maxFallSpeed);

        // Horizontal: how fast you're falling determines how fast you glide forward
        float fallSpeed = Mathf.Max(0f, -verticalVelocity);
        float targetForwardSpeed = fallSpeed * data.glideRatio;

        Vector3 currentHorizontal = Vector3.ProjectOnPlane(velocity, Vector3.up);
        float currentForwardSpeed = Vector3.Dot(currentHorizontal, facingForward);
        float newForwardSpeed = Mathf.MoveTowards(currentForwardSpeed, targetForwardSpeed, data.forwardAcceleration * ctx.DeltaTime);

        Vector3 horizontalVelocity = facingForward * newForwardSpeed;

        rb.linearVelocity = new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);
    }
}