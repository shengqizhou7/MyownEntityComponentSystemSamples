using Tutorials.Kickball.Execute;
using Tutorials.Kickball.Step1;
using Tutorials.Kickball.Step2;
using Tutorials.Kickball.Step3;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using PGD;
using PGD.Jobs;

namespace Tutorials.Kickball.Step4
{
    [UpdateSystemBefore(typeof(TransformSystemGroup))]
    public partial class NewBallMovementSystem : PGDJobSystemBase
    {
        [BurstCompile]
        protected override void OnCreate(ref PGDSystemState state)
        {
            state.RequireForUpdate<NewBallMovement>();
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        protected override void OnUpdate(ref PGDSystemState state)
        {
            var config = PGDGameContext.GetSingleton<Config>();
            var obstacleQuery = PGDGameContext.BuildQuery().WithAllComponents(IComponents.Get<PGDLocalTransform, Obstacle>());
            var minDist = config.ObstacleRadius + 0.5f; // the ball radius is 0.5f
            var job = new BallMovementJob
            {
                ObstacleTransforms = obstacleQuery.ToComponentDataArray<PGDLocalTransform>(state.WorldUpdateAllocator),
                DecayFactor = config.BallVelocityDecay * PGDGameContext.Time.DeltaTime,
                DeltaTime = PGDGameContext.Time.DeltaTime,
                MinDistToObstacleSQ = minDist * minDist
            };
            job.ScheduleParallel();
        }
    }

    // The implicit query of this IJobEntity matches all entities having LocalTransform, Velocity, and Ball components.
    [WithAllComponents(typeof(Ball))]
    [WithDisabled(typeof(Carry))] // Relevant in Step 5
    [BurstCompile]
    public partial struct BallMovementJob : IJobParallel
    {
        [ReadOnly]
        public NativeArray<PGDLocalTransform> ObstacleTransforms;
        public float DecayFactor;
        public float DeltaTime;
        public float MinDistToObstacleSQ;
        public void Execute(ref PGDLocalTransform transform, ref Velocity velocity)
        {
            if (velocity.Value.Equals(float2.zero))
            {
                return;
            }

            var magnitude = math.length(velocity.Value);
            var newPosition = transform.Position + new float3(velocity.Value.x, 0, velocity.Value.y) * DeltaTime;
            foreach (var obstacleTransform in ObstacleTransforms)
            {
                if (math.distancesq(newPosition, obstacleTransform.Position) <= MinDistToObstacleSQ)
                {
                    newPosition = DeflectBall(transform.Position, obstacleTransform.Position, ref velocity, magnitude, DeltaTime);
                    break;
                }
            }

            transform.Position = newPosition;
            var newMagnitude = math.max(magnitude - DecayFactor, 0);
            velocity.Value = math.normalizesafe(velocity.Value) * newMagnitude;
        }

        private float3 DeflectBall(float3 ballPos, float3 obstaclePos, ref Velocity velocity, float magnitude, float dt)
        {
            var obstacleToBallVector = math.normalize((ballPos - obstaclePos).xz);
            velocity.Value = math.reflect(math.normalize(velocity.Value), obstacleToBallVector) * magnitude;
            return ballPos + new float3(velocity.Value.x, 0, velocity.Value.y) * dt;
        }
    }
}