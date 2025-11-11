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
    [UpdateSystemBefore(typeof(BallMovementSystem))]
    public partial class BallCarrySystem : PGDSystemEnhanced
    {
        static readonly float3 CarryOffset = new float3(0, 2, 0);
        [BurstCompile]
        protected override void OnCreate(ref PGDSystemState state)
        {
            state.RequireForUpdate<BallCarry>();
        }

        [BurstCompile]
        protected override void OnUpdate(ref PGDSystemState state)
        {
            var config = PGDGameContext.GetSingleton<Config>();
            // move carried balls
            foreach (var(ballTransform, carrier)in PGDGameContext.QueryForeach<PGDRefRW<PGDLocalTransform>, PGDRefRO<Carry>>().WithAll<Ball>())
            {
                var playerTransform = carrier.ValueRO.Target.GetComponent<PGDLocalTransform>();
                ballTransform.ValueRW.Position = playerTransform.Position + CarryOffset;
            }

            if (!Input.GetKeyDown(KeyCode.C))
            {
                return;
            }

            // foreach (var(playerTransform, playerEntity)in PGDGameContext.QueryForeach<PGDRefRW<PGDLocalTransform>>().WithAll<Player>().WithEntityAccess())
            // {
            //     if (state.World.IsComponentEnabled<Carry>(playerEntity))
            //     {
            //         // put down ball
            //         var carried = playerEntity.GetComponent<Carry>();
            //         var ballTransform = carried.Target.GetComponent<PGDLocalTransform>();
            //         ballTransform.Position = playerTransform.ValueRO.Position;
            //         carried.Target.Set(ballTransform);
            //         state.World.SetComponentEnabled<Carry>(carried.Target, false);
            //         state.World.SetComponentEnabled<Carry>(playerEntity, false);
            //         carried.Target.Set(new Carry());
            //         playerEntity.Set(new Carry());
            //     }
            //     else
            //     {
            //         // pick up first ball in range
            //         foreach (var(ballTransform, ballEntity)in PGDGameContext.QueryForeach<PGDRefRO<PGDLocalTransform>>().WithAll<Ball>().WithDisabled<Carry>().WithEntityAccess())
            //         {
            //             float distSQ = math.distancesq(playerTransform.ValueRO.Position, ballTransform.ValueRO.Position);
            //             if (distSQ <= config.BallKickingRangeSQ)
            //             {
            //                 ballEntity.Set(new Velocity());
            //                 playerEntity.Set(new Carry { Target = ballEntity });
            //                 ballEntity.Set(new Carry { Target = playerEntity });
            //                 state.World.SetComponentEnabled<Carry>(playerEntity, true);
            //                 state.World.SetComponentEnabled<Carry>(ballEntity, true);
            //                 break;
            //             }
            //         }
            //     }
            // }
        }
    }
}