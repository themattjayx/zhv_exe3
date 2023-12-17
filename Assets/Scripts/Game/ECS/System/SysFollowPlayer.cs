using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

/// <summary>
/// System tracking the closest player.
/// </summary>
public partial class SysFollowPlayer : SystemBase
{
    /// <summary>
    /// Main system update code.
    /// </summary>
    protected override void OnUpdate()
    {
        // Prepare a list of all living players.
        var livingPlayers = GameManager.Instance.LivingPlayers();
        var livingPlayerPositions = new NativeArray<float3>(livingPlayers.Count, Allocator.TempJob);
        for (int iii = 0; iii < livingPlayers.Count; ++iii)
        { livingPlayerPositions[iii] = livingPlayers[iii].transform.position; }

        // Run on the main thread.
        Entities
            .WithAll<TEnemy>()
            .WithReadOnly(livingPlayerPositions)
            .WithDisposeOnCompletion(livingPlayerPositions)
            .ForEach((ref LocalTransform transform) =>
        {
            // Find nearest player
            var nearestPosition = float3.zero;
            var nearestDistance = float.MaxValue;
            
            foreach (var playerPosition in livingPlayerPositions)
            { // Search for the nearest player.
                var playerDistance = math.length(playerPosition - transform.Position);
                
                if (playerDistance < nearestDistance)
                { nearestDistance = playerDistance; nearestPosition = playerPosition; }
            }

            if (nearestDistance < float.MaxValue)
            { // If we have any players -> Follow them!
                // Prepare the player's direction.
                var direction = new float3(nearestPosition) - transform.Position;
                direction.y = 0.0f;

                // Look in that direction.
                transform.Rotation = quaternion.LookRotation(
                    direction, 
                    new float3{ x = 0.0f, y = -1.0f, z = 0.0f }
                );
            }
        }).ScheduleParallel();
    }
}
