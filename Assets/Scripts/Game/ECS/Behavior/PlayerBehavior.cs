using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Player behavior.
/// </summary>
public class PlayerBehavior : MonoBehaviour
{
    /// <summary>
    /// Current health of the Player.
    /// </summary>
    public float? health;
    
    /// <summary>
    /// Maximum health of the Player.
    /// </summary>
    public float? maxHealth;
    
    /// <summary>
    /// Player baker.
    /// </summary>
    public class PlayerBaker : Baker<PlayerBehavior>
    {
        /// <summary>
        /// Add all necessary components to the new entity.
        /// </summary>
        public override void Bake(PlayerBehavior authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            // Get the health from the main Component if not provided.
            var current = authoring.health ?? GetComponent<Player>().health;
            var max = authoring.maxHealth ?? GetComponent<Player>().maxHealth;
            
            // Make the entity a Player.
            AddComponent(entity, new TPlayer());
            AddComponent(entity, new CHealth { current = current, max = max });
        }
    }
}

