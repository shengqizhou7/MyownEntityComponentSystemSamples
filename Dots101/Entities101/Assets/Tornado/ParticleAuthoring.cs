using UnityEngine;
using Unity.Entities;
using PGD;
using PGD.Jobs;

namespace Tutorials.Tornado
{
    public class ParticleAuthoring : MonoBehaviour
    {
        class Baker : PGDHybrid<ParticleAuthoring>
        {
            public override void Handle(ParticleAuthoring authoring)
            {
                var entity = GetHybridEntity();
                AddComponent<Particle>(entity);
            }
        }
    }

    public struct Particle : IComponent
    {
        public float radiusMult;
    }
}