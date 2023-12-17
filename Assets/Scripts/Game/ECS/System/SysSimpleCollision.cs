using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

/// <summary>
/// System used for the simplified radius-based collision detection.
/// </summary>
public partial class SysSimpleCollision : SystemBase
{
    /// <summary>
    /// Group of all enemy entities.
    /// </summary>
    private EntityQuery mEnemyQuery;
    
    /// <summary>
    /// Group of all bullet entities.
    /// </summary>
    private EntityQuery mBulletQuery;
    
    /// <summary>
    /// Initialization code for the system.
    /// </summary>
    protected override void OnCreate()
    {
        base.OnCreate();
        mEnemyQuery = GetEntityQuery(
            typeof(CHealth), 
            ComponentType.ReadOnly<LocalTransform>(),
            ComponentType.ReadOnly<TEnemy>()
        );
        mBulletQuery = GetEntityQuery(
            typeof(CTimed), 
            ComponentType.ReadOnly<LocalTransform>()
        );
    }
    
    /// <summary>
    /// Main system update code.
    /// </summary>
    protected override void OnUpdate()
    {
        // This system implements the simplified radius-based physics collisions, skip it if we use "real" physics.
        if (Settings.Instance.useECSPhysics)
        { return; }
        
        // Prepare a list of current bullets.
        var bulletLocations = mBulletQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
        // Prepare a list of all living players.
        var livingPlayers = GameManager.Instance.LivingPlayers();
        var livingPlayerPositions = new NativeArray<float3>(livingPlayers.Count, Allocator.TempJob);
        for (int iii = 0; iii < livingPlayers.Count; ++iii)
        { livingPlayerPositions[iii] = livingPlayers[iii].transform.position; }
        // Using a simplified collision detection system.
        var radius = Settings.Instance.ecsSimplePhysicsRadius;
        
        // Detect collisions with the enemies and set their health to zero.
        Entities
            .WithAll<TEnemy>()
            .WithReadOnly(bulletLocations)
            .WithDisposeOnCompletion(bulletLocations)
            .ForEach((ref CHealth health, in LocalTransform transform) => {
                for (int iii = 0; iii < bulletLocations.Length; ++iii)
                {
                    if (CheckCollision(transform.Position, bulletLocations[iii].Position, radius))
                    { health.current = 0.0f; }
                }
        }).ScheduleParallel();
        
        // Detect collisions with the player and perform the damage.
        // This is a simplified version, which runs on the main thread!
        Entities
            .WithAll<TEnemy>()
            .WithoutBurst()
            .WithReadOnly(livingPlayerPositions)
            .WithDisposeOnCompletion(livingPlayerPositions)
            .ForEach((in LocalTransform transform) => {
                for (var playerIdx = 0; playerIdx < livingPlayerPositions.Length; ++playerIdx)
                { // Search for the player to attack.
                    var playerDistance = math.length(livingPlayerPositions[playerIdx] - transform.Position);

                    if (playerDistance < radius)
                    { GameManager.Instance.DamagePlayer(playerIdx, 1.0f); }
                }
        }).Run();
    }

    /// <summary>
    /// Check whether two positions collide.
    /// </summary>
    static bool CheckCollision(float3 pos1, float3 pos2, float radius)
    { return math.length(pos1 - pos2) < radius; }
}
