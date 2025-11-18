using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Scenes;
using Unity.Transforms;
using PGD;
using PGD.Jobs;

namespace Tutorials.Tornado
{
    [UpdateSystemAfter(typeof(SceneSystemGroup))]
    public partial class TornadoSpawnSystem : PGDSystemEnhanced
    {
        [BurstCompile]
        protected override void OnCreate(ref PGDSystemState state)
        {
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        protected override void OnUpdate(ref PGDSystemState state)
        {
            var config = PGDGameContext.GetSingleton<Config>();
            var entities = state.World.Instantiate(config.ParticlePrefab, 1000, Allocator.Temp);
            var random = Random.CreateFromIndex(1234);
            foreach (var entity in entities)
            {
                var particle = PGDGameContext.GetComponentRW<Particle>(entity);
                var transform = PGDGameContext.GetComponentRW<PGDLocalTransform>(entity);
                var color = PGDGameContext.GetComponentRW<URPMaterialPropertyBaseColor>(entity);
                transform.ValueRW.Position = new float3(random.NextFloat(-50f, 50f), random.NextFloat(0f, 50f), random.NextFloat(-50f, 50f));
                transform.ValueRW.Scale = random.NextFloat(.2f, .7f);
                particle.ValueRW.radiusMult = random.NextFloat();
                color.ValueRW.Value = new float4(new float3(random.NextFloat(.3f, .7f)), 1f);
            }

            state.Enabled = false;
        }
    }
}