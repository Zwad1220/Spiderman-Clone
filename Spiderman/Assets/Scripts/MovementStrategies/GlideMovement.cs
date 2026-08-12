using UnityEngine;

public class GlideMovement : IMovementStrategy
{
    //Descent
    float glideGravityScale = 0.3f;   // how strongly gravity pulls you down while gliding
    float maxFallSpeed = 6f;          // terminal vertical speed

    //forward glide
    float glideRatio = 2.2f;          // horizontal distance gained per unit of fall (tune this — higher = flatter, faster glide)
    float forwardAcceleration = 8f;   // how quickly horizontal speed catches up to the glide-ratio target
    float turnRate = 90f;             // deg/sec steering responsiveness

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
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRot, turnRate * ctx.DeltaTime));
        }

        Vector3 facingForward = rb.transform.forward;

        // Vertical: gravity-driven descent, clamped to a terminal fall speed
        float verticalVelocity = velocity.y + Physics.gravity.y * glideGravityScale * ctx.DeltaTime;
        verticalVelocity = Mathf.Max(verticalVelocity, -maxFallSpeed);

        // Horizontal: how fast you're falling determines how fast you glide forward
        float fallSpeed = Mathf.Max(0f, -verticalVelocity);
        float targetForwardSpeed = fallSpeed * glideRatio;

        Vector3 currentHorizontal = Vector3.ProjectOnPlane(velocity, Vector3.up);
        float currentForwardSpeed = Vector3.Dot(currentHorizontal, facingForward);
        float newForwardSpeed = Mathf.MoveTowards(currentForwardSpeed, targetForwardSpeed, forwardAcceleration * ctx.DeltaTime);

        Vector3 horizontalVelocity = facingForward * newForwardSpeed;

        rb.linearVelocity = new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);
    }
}