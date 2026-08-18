using UnityEngine;

public class VerticalMovementStrategy : IMovementStrategy
{
    readonly WallCrawlMovementDataSO data;

    public VerticalMovementStrategy(WallCrawlMovementDataSO data)
    {
        this.data = data;
    }

    public void Move(MovementContext ctx)
    {
        Rigidbody rb = ctx.Rb;
        rb.linearDamping = 0f;
        rb.useGravity = false;

        Vector3 wallUp = Vector3.ProjectOnPlane(Vector3.up, ctx.WallNormal).normalized;
        Vector3 wallRight = Vector3.Cross(ctx.WallNormal, wallUp).normalized;

        Quaternion targetRot = Quaternion.LookRotation(-ctx.WallNormal, wallUp);
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRot, data.turnRate * ctx.DeltaTime));

        // Raw input axes mapped onto the wall plane — not the world-projected
        // InputDirection, which has no defined relationship to the wall basis.
        Vector3 climbDir = (wallUp * ctx.RawMove.y) + (wallRight * ctx.RawMove.x);
        climbDir = Vector3.ClampMagnitude(climbDir, 1f);

        Vector3 targetVelocity = climbDir * data.climbSpeed;

        rb.linearVelocity = Vector3.MoveTowards(rb.linearVelocity, targetVelocity, data.acceleration * ctx.DeltaTime);
        Debug.Log($"climbDir:{climbDir} targetVelocity:{targetVelocity} rb.velocity:{rb.linearVelocity} damping:{rb.linearDamping}");
    }
}