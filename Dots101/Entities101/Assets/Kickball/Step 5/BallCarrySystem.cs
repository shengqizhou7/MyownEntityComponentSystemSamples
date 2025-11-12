using Tutorials.Kickball.Execute;
using Tutorials.Kickball.Step1;
using Tutorials.Kickball.Step2;
using Tutorials.Kickball.Step3;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Tutorials.Kickball.Step5
{
    [UpdateBefore(typeof(BallMovementSystem))]
    public partial struct BallCarrySystem : ISystem
    {
        static readonly float3 CarryOffset = new float3(0, 2, 0);

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BallCarry>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();

            // move carried balls
            foreach (var (ballTransform, carrier) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<Carry>>()
                         .WithAll<Ball>())
            {
                if (carrier.ValueRO.IsEnabled)
                {
                    var playerTransform = state.EntityManager.GetComponentData<LocalTransform>(carrier.ValueRO.Target);
                    ballTransform.ValueRW.Position = playerTransform.Position + CarryOffset;
                }
            }

            if (!Input.GetKeyDown(KeyCode.C))
            {
                return;
            }

            foreach (var (playerTransform, carry, playerEntity) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<Carry>>()
                         .WithAll<Player>()
                         .WithEntityAccess())
            {
                if (carry.ValueRO.IsEnabled)
                {
                    // put down ball
                    var carried = carry.ValueRO;

                    var ballTransform = state.EntityManager.GetComponentData<LocalTransform>(carried.Target);
                    ballTransform.Position = playerTransform.ValueRO.Position;
                    state.EntityManager.SetComponentData(carried.Target, ballTransform);

                    state.EntityManager.SetComponentData(carried.Target, new Carry { IsEnabled = false });
                    state.EntityManager.SetComponentData(playerEntity, new Carry { IsEnabled = false });
                }
                else
                {
                    // pick up first ball in range
                    foreach (var (ballTransform, ballCarry, ballEntity) in
                             SystemAPI.Query<RefRO<LocalTransform>, RefRO<Carry>>()
                                 .WithAll<Ball>()
                                 .WithEntityAccess())
                    {
                        if (ballCarry.ValueRO.IsEnabled)
                            continue;

                        float distSQ = math.distancesq(playerTransform.ValueRO.Position,
                            ballTransform.ValueRO.Position);

                        if (distSQ <= config.BallKickingRangeSQ)
                        {
                            state.EntityManager.SetComponentData(ballEntity, new Velocity());

                            state.EntityManager.SetComponentData(playerEntity, new Carry { Target = ballEntity, IsEnabled = true });
                            state.EntityManager.SetComponentData(ballEntity, new Carry { Target = playerEntity, IsEnabled = true });
                            break;
                        }
                    }
                }
            }
        }
    }
}
