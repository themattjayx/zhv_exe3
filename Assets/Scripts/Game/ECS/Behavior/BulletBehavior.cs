using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Bullet behavior.
/// </summary>
public class BulletBehavior : MonoBehaviour
{
    /// <summary>
    /// The speed of the bullet.
    /// </summary>
    public float speed;
    
    /// <summary>
    /// The lifetime of the bullet.
    /// </summary>
    public float lifeTime;

    /// <summary>
    /// Bullet baker.
    /// </summary>
    public class BulletBaker : Baker<BulletBehavior>
    {
        /// <summary>
        /// Add all necessary components to the new entity.
        /// </summary>
        public override void Bake(BulletBehavior authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TBullet());
            AddComponent(entity, new TMoveForward());
            AddComponent(entity, new CMove { speed = authoring.speed });
            AddComponent(entity, new CTimed { lifeTime = authoring.lifeTime });
        }
    }
}
