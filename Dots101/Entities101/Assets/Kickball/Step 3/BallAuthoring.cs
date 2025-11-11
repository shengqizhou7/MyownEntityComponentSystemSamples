using Tutorials.Kickball.Step2;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using PGD;
using PGD.Jobs;

namespace Tutorials.Kickball.Step3
{
    public class BallAuthoring : MonoBehaviour
    {
        class Baker : PGDHybrid<BallAuthoring>
        {
            public override void Handle(BallAuthoring authoring)
            {
                var entity = GetHybridEntity();
                // A single authoring component can add multiple components to the entity.
                AddComponent<Ball>(entity);
                AddComponent<Velocity>(entity);
                // Used in Step 5
                AddComponent<Carry>(entity);
                // SetComponentEnabled<Carry>(entity, false);
            }
        }
    }

    // A tag component for ball entities.
    public struct Ball : IComponent
    {
    }

    // A 2d velocity vector for the ball entities.
    public struct Velocity : IComponent
    {
        public float2 Value;
    }
}