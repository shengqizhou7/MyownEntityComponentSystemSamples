using Tutorials.Kickball.Execute;
using Tutorials.Kickball.Step1;
using Tutorials.Kickball.Step2;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using PGD;
using PGD.Jobs;

namespace Tutorials.Kickball.Step3
{
    [UpdateSystemBefore(typeof(TransformSystemGroup))]
    public partial class BallMovementSystem : PGDSystemEnhanced
    {
        [BurstCompile]
        protected override void OnCreate(ref PGDSystemState state)
        {
            state.RequireForUpdate<BallMovement>();
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        protected override void OnUpdate(ref PGDSystemState state)
        {
            var config = PGDGameContext.GetSingleton<Config>();
            // The world or its SystemGroups may sometimes override the current time or delta time,
            // for various reasons, so you should use SystemAPI.Time instead of UnityEngine.Time.
            var dt = PGDGameContext.Time.DeltaTime;
            var decayFactor = config.BallVelocityDecay * dt;
            var minDist = config.ObstacleRadius + 0.5f; // the ball radius is 0.5f
            var minDistSQ = minDist * minDist;
            // For every ball entity, we need to read and modify its LocalTransform and Velocity.
            foreach (var(ballTransform, velocity)in PGDGameContext.QueryForeach<PGDRefRW<PGDLocalTransform>, PGDRefRW<Velocity>>().WithAll<Ball>().WithAll<Carry>()) // Relevant in Step 5
            {
                // Skip the ball if it isn't moving!
                if (velocity.ValueRO.Value.Equals(float2.zero))
                {
                    continue;
                }

                var magnitude = math.length(velocity.ValueRO.Value);
                var newPosition = ballTransform.ValueRW.Position + new float3(velocity.ValueRO.Value.x, 0, velocity.ValueRO.Value.y) * dt;
                // Check if the ball intersects an obstacle. If so, reflect the ball's velocity vector.
                foreach (var obstacleTransform in PGDGameContext.QueryForeach<PGDRefRO<PGDLocalTransform>>().WithAll<Obstacle>())
                {
                    if (math.distancesq(newPosition, obstacleTransform.ValueRO.Position) <= minDistSQ)
                    {
                        newPosition = DeflectBall(ballTransform.ValueRO.Position, obstacleTransform.ValueRO.Position, velocity, magnitude, dt);
                        // As long as the obstacles are spaced apart, it's impossible
                        // for one ball to hit two obstacles in a single frame, so we can
                        // break after detecting collision with one obstacle.
                        break;
                    }
                }

                ballTransform.ValueRW.Position = newPosition;
                // Velocity decay.
                var newMagnitude = math.max(magnitude - decayFactor, 0);
                velocity.ValueRW.Value = math.normalizesafe(velocity.ValueRO.Value) * newMagnitude;
            }
        }

        private float3 DeflectBall(float3 ballPos, float3 obstaclePos, PGDRefRW<Velocity> velocity, float magnitude, float dt)
        {
            var obstacleToBallVector = math.normalize((ballPos - obstaclePos).xz);
            velocity.ValueRW.Value = math.reflect(math.normalize(velocity.ValueRO.Value), obstacleToBallVector) * magnitude;
            return ballPos + new float3(velocity.ValueRO.Value.x, 0, velocity.ValueRO.Value.y) * dt;
        }
    }
}