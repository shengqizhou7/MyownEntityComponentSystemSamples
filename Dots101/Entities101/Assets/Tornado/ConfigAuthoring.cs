using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using PGD;
using PGD.Jobs;

namespace Tutorials.Tornado
{
    public class ConfigAuthoring : MonoBehaviour
    {
        public GameObject BarPrefab;
        [Range(0f, 1f)]
        public float BarDamping;
        [Range(0f, 1f)]
        public float BarFriction;
        public float BarBreakResistance;
        [Range(0f, 1f)]
        public float TornadoForce;
        public float TornadoMaxForceDist;
        public float TornadoHeight;
        public float TornadoUpForce;
        public float TornadoInwardForce;
        public GameObject ParticlePrefab;
        public float ParticleSpinRate;
        public float ParticleUpwardSpeed;
        class Baker : PGDHybrid<ConfigAuthoring>
        {
            public override void Handle(ConfigAuthoring authoring)
            {
                var entity = GetHybridEntity();
                AddComponent(entity, new Config { BarPrefab = GetHybridEntity(authoring.BarPrefab), BarDamping = authoring.BarDamping, BarFriction = authoring.BarFriction, BarBreakResistance = authoring.BarBreakResistance, TornadoForce = authoring.TornadoForce, TornadoMaxForceDist = authoring.TornadoMaxForceDist, TornadoHeight = authoring.TornadoHeight, TornadoUpForce = authoring.TornadoUpForce, TornadoInwardForce = authoring.TornadoInwardForce, ParticlePrefab = GetHybridEntity(authoring.ParticlePrefab), ParticleSpinRate = authoring.ParticleSpinRate, ParticleUpwardSpeed = authoring.ParticleUpwardSpeed });
            }
        }
    }

    public struct Config : IComponent
    {
        public IEntity BarPrefab;
        public float BarDamping;
        public float BarFriction;
        public float BarBreakResistance;
        public float TornadoForce;
        public float TornadoMaxForceDist;
        public float TornadoHeight;
        public float TornadoUpForce;
        public float TornadoInwardForce;
        public IEntity ParticlePrefab;
        public float ParticleSpinRate;
        public float ParticleUpwardSpeed;
    }
}