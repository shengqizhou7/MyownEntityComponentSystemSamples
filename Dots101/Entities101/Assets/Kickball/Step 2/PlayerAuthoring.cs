using Unity.Entities;
using UnityEngine;
using PGD;
using PGD.Jobs;

namespace Tutorials.Kickball.Step2
{
    // Same pattern as ObstacleAuthoring.cs in Step 1.
    public class PlayerAuthoring : MonoBehaviour
    {
        class Baker : PGDHybrid<PlayerAuthoring>
        {
            public override void Handle(PlayerAuthoring authoring)
            {
                var entity = GetHybridEntity();
                AddComponent<Player>(entity);
                // Used in Step 5
                AddComponent<Carry>(entity);
                // SetComponentEnabled<Carry>(entity, false);
            }
        }
    }

    public struct Player : IComponent
    {
    }

    // Used in Step 5
    public struct Carry : IComponent, IEnableableComponent
    {
        // on a ball, this denotes the player carrying the ball; on a player, this denotes the ball being carried
        public IEntity Target;
    }
}