using System;
using UnityEngine;

public class GlideMovement : IMovementStrategy
{
    float glideGravityScale = 0.25f;
    float dragCoefficient = 0.02f;
    float liftCoefficient = 0.15f;
    float bankTurnRate = 60f; // degrees/sec at full input

    public void Move(MovementContext ctx){
        Vector3 velocity = ctx.Rb.linearVelocity;

        // reducted gravity while gliding
        Vector3 gravity = Physics.gravity * glideGravityScale;

        //drag oppose current velocity
        var drag = -velocity.normalized * velocity.sqrMagnitude * dragCoefficient;

        // lift acts opposite gravity, proportional to forward speed
        float forwardSpeed = Vector3.Dot(velocity, ctx.Rb.transform.forward);
        Vector3 lift = Vector3.up * Mathf.Max(0, forwardSpeed) * liftCoefficient;

        ctx.Rb.AddForce(gravity + drag + lift, ForceMode.Acceleration);

        // banking: rotate the body towards input direction, force follows facing   
        if(ctx.InputDirection.sqrMagnitude > .01f){
            Quaternion targetRot = Quaternion.LookRotation(
                Vector3.ProjectOnPlane(ctx.InputDirection, velocity.normalized), velocity.normalized);
            ctx.Rb.MoveRotation(Quaternion.RotateTowards(ctx.Rb.rotation, targetRot, bankTurnRate * ctx.DeltaTime));
        }
    }
}
