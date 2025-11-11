using Tutorials.Kickball.Execute;
using Tutorials.Kickball.Step1;
using Tutorials.Kickball.Step2;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using PGD;
using PGD.Jobs;

namespace Tutorials.Kickball.Step4
{
    [UpdateSystemBefore(typeof(TransformSystemGroup))]
    public partial class NewPlayerMovementSystem : PGDJobSystemBase
    {
        [BurstCompile]
        protected override void OnCreate(ref PGDSystemState state)
        {
            state.RequireForUpdate<NewPlayerMovement>();
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        protected override void OnUpdate(ref PGDSystemState state)
        {
            var config = PGDGameContext.GetSingleton<Config>();
            var obstacleQuery = PGDGameContext.BuildQuery().WithAllComponents(IComponents.Get<PGDLocalTransform, Obstacle>());
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            var input = new float3(horizontal, 0, vertical) * PGDGameContext.Time.DeltaTime * config.PlayerSpeed;
            // Only move if the user has directional input.
            if (input.Equals(float3.zero))
            {
                return;
            }

            var minDist = config.ObstacleRadius + 0.5f; // the player capsule radius is 0.5f
            var job = new PlayerMovementJob
            {
                Input = input,
                MinDistSQ = minDist * minDist,
                ObstacleTransforms = obstacleQuery.ToComponentDataArray<PGDLocalTransform>(state.WorldUpdateAllocator)
            };
            job.ScheduleParallel();
        }
    }

    // The implicit query of this IJobEntity matches all entities having LocalTransform and Player components.
    [WithAllComponents(typeof(Player))]
    [BurstCompile]
    public partial struct PlayerMovementJob : IJobParallel
    {
        [ReadOnly]
        public NativeArray<PGDLocalTransform> ObstacleTransforms;
        public float3 Input;
        public float MinDistSQ;
        public void Execute(ref PGDLocalTransform transform)
        {
            var newPos = transform.Position + Input;
            foreach (var obstacleTransform in ObstacleTransforms)
            {
                if (math.distancesq(newPos, obstacleTransform.Position) <= MinDistSQ)
                {
                    newPos = transform.Position;
                    break;
                }
            }

            transform.Position = newPos;
        }
    }
}