using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using PGD;
using PGD.Jobs;

namespace Tutorials.Tornado
{
    /*
     * Updates the transforms of the bars.
     */
    public partial class BuildingRenderSystem : PGDJobSystemBase
    {
        [BurstCompile]
        protected override void OnCreate(ref PGDSystemState state)
        {
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        protected override void OnUpdate(ref PGDSystemState state)
        {
            var job1 = new PointRenderJob
            {
                CurrentPoints = PGDGameContext.GetSingleton<PointArrays>().current
            };
            job1.ScheduleParallel();
        }

        [BurstCompile]
        public partial struct PointRenderJob : IJobParallel
        {
            [ReadOnly]
            public NativeArray<float3> CurrentPoints;
            public void Execute(ref PGDLocalToWorld ltw, in Bar bar, in BarThickness thickness)
            {
                var a = CurrentPoints[bar.pointA];
                var b = CurrentPoints[bar.pointB];
                var d = math.distance(a, b);
                var norm = (a - b) / d;
                var t = (a + b) / 2;
                var r = quaternion.LookRotationSafe(norm, norm.yzx);
                var s = new float3(new float2(thickness.Value), d);
                ltw.Value = float4x4.TRS(t, r, s);
            }
        }
    }
}