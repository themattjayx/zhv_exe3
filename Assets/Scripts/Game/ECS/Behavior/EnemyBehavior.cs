using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

/// <summary>
/// Enemy behavior.
/// </summary>
public class EnemyBehavior : MonoBehaviour
{
    /// <summary>
    /// Current health of the Enemy.
    /// </summary>
    public float health = 10.0f;

    /// <summary>
    /// Current speed of the Enemy.
    /// </summary>
    public float speed = 1.0f;

    /// <summary>
    /// Enemy baker.
    /// </summary>
    public class EnemyBaker : Baker<EnemyBehavior>
    {
        /// <summary>
        /// Add all necessary components to the new entity.
        /// </summary>
        public override void Bake(EnemyBehavior authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TEnemy());
            AddComponent(entity, new TMoveForward());
            AddComponent(entity, new CHealth { current = authoring.health, max = authoring.health });
            AddComponent(entity, new CMove { speed = authoring.speed });
        }
    }
}
