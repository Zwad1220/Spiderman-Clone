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



        // Debug.Log("Player is climbing a wall");

        // Build a local coordinate system ON the wall surface, using its normal.
        // wallUp = "vertical" as far as the wall is concerned
        // wallRight = the tangent perpendicular to that
        Vector3 wallUp = Vector3.ProjectOnPlane(Vector3.up, ctx.WallNormal).normalized;
        Vector3 wallRight = Vector3.Cross(ctx.WallNormal, wallUp).normalized;

        // Rotate the player to face INTO the wall, "up" along the wall's surface
        Quaternion targetRot = Quaternion.LookRotation(-ctx.WallNormal, wallUp);
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRot, data.turnRate * ctx.DeltaTime));

        // Map 2D input onto the wall's plane instead of world XZ.
        // InputDirection.y here assumes you're feeding raw move.y/move.x into it upstream —
        // see note below, this is the part most likely to need adjusting for your input setup.
        Vector3 climbDir = (wallUp * ctx.InputDirection.z) + (wallRight * ctx.InputDirection.x);
        climbDir = Vector3.ClampMagnitude(climbDir, 1f);

        Vector3 targetVelocity = climbDir * data.climbSpeed;

        // Snap directly like GlideMovement does, no force accumulation
        rb.linearVelocity = Vector3.MoveTowards(rb.linearVelocity, targetVelocity, data.acceleration * ctx.DeltaTime);
    }
}