using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// System moving entities forward.
/// </summary>
public partial class SysMoveForward : SystemBase
{
    /// <summary>
    /// Main system update code.
    /// </summary>
    protected override void OnUpdate()
    {
        var deltaTime = World.Time.DeltaTime;

        Entities
            .WithAll<TMoveForward>()
            .ForEach((ref LocalTransform transform, in CMove move) => {
            transform.Position += math.mul(transform.Rotation, new float3(0.0f, 0.0f, move.speed)) * deltaTime;
        }).ScheduleParallel();
    }
}
