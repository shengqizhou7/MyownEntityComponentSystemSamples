using Tutorials.Kickball.Execute;
using Tutorials.Kickball.Step1;
using Tutorials.Kickball.Step2;
using Tutorials.Kickball.Step3;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using PGD;
using PGD.Jobs;

namespace Tutorials.Kickball.Step5
{
    // UpdateBefore BallMovementSystem so that the ball movement is affected by a kick in the same frame.
    [UpdateSystemBefore(typeof(BallMovementSystem))]
    [UpdateSystemBefore(typeof(TransformSystemGroup))]
    public partial class BallKickingSystem : PGDSystemEnhanced
    {
        [BurstCompile]
        protected override void OnCreate(ref PGDSystemState state)
        {
            state.RequireForUpdate<BallKicking>();
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        protected override void OnUpdate(ref PGDSystemState state)
        {
            var config = PGDGameContext.GetSingleton<Config>();
            if (!Input.GetKeyDown(KeyCode.Space))
            {
                return;
            }

            // For every player, add an impact velocity to every ball in kicking range.
            foreach (var playerTransform in PGDGameContext.QueryForeach<PGDRefRO<PGDLocalTransform>>().WithAll<Player>())
            {
                foreach (var(ballTransform, velocity)in PGDGameContext.QueryForeach<PGDRefRO<PGDLocalTransform>, PGDRefRW<Velocity>>().WithAll<Ball>())
                {
                    float distSQ = math.distancesq(playerTransform.ValueRO.Position, ballTransform.ValueRO.Position);
                    if (distSQ <= config.BallKickingRangeSQ)
                    {
                        var playerToBall = ballTransform.ValueRO.Position.xz - playerTransform.ValueRO.Position.xz;
                        // Use normalizesafe() in case the ball and player are exactly on top of each other
                        // (which isn't very likely but not impossible).
                        velocity.ValueRW.Value += math.normalizesafe(playerToBall) * config.BallKickForce;
                    }
                }
            }
        }
    }
}