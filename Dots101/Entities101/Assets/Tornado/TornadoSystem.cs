using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using PGD;
using PGD.Jobs;

namespace Tutorials.Tornado
{
    public partial class TornadoSystem : PGDJobSystemBase
    {
        [BurstCompile]
        protected override void OnCreate(ref PGDSystemState state)
        {
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        protected override void OnUpdate(ref PGDSystemState state)
        {
            var elapsedTime = (float)PGDGameContext.Time.ElapsedTime;
            var config = PGDGameContext.GetSingleton<Config>();
            new TornadoParticleJob
            {
                ParticleSpinRate = config.ParticleSpinRate,
                ParticleUpwardSpeed = config.ParticleUpwardSpeed,
                ElapsedTime = elapsedTime,
                Tornado = BuildingSystem.Position(elapsedTime),
                DeltaTime = PGDGameContext.Time.DeltaTime
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct TornadoParticleJob : IJobParallel
    {
        public float ElapsedTime;
        public float2 Tornado;
        public float DeltaTime;
        public float ParticleSpinRate;
        public float ParticleUpwardSpeed;
        public void Execute(ref PGDLocalTransform transform, in Particle particle)
        {
            var tornadoPos = new float3(Tornado.x + BuildingSystem.TornadoSway(transform.Position.y, ElapsedTime), transform.Position.y, Tornado.y);
            var delta = tornadoPos - transform.Position;
            float dist = math.length(delta);
            delta /= dist;
            float inForce = dist - math.saturate(tornadoPos.y / 50f) * 30f * particle.radiusMult + 2f;
            transform.Position += new float3(-delta.z * ParticleSpinRate + delta.x * inForce, ParticleUpwardSpeed, delta.x * ParticleSpinRate + delta.z * inForce) * DeltaTime;
            if (transform.Position.y > 50f)
            {
                transform.Position.y = 0f;
            }
        }
    }
}