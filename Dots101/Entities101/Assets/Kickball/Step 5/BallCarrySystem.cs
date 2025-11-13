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
            foreach (var (ballTransform, carrier, carryEnabled) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<Carry>, RefRO<CarryEnabled>>()
                         .WithAll<Ball>())
            {
                if (carryEnabled.ValueRO.Value)
                {
                    var playerTransform = state.EntityManager.GetComponentData<LocalTransform>(carrier.ValueRO.Target);
                    ballTransform.ValueRW.Position = playerTransform.Position + CarryOffset;
                }
            }

            if (!Input.GetKeyDown(KeyCode.C))
            {
                return;
            }

            foreach (var (playerTransform, carry, carryEnabled, playerEntity) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<Carry>, RefRO<CarryEnabled>>()
                         .WithAll<Player>()
                         .WithEntityAccess())
            {
                if (carryEnabled.ValueRO.Value)
                {
                    // put down ball
                    var carried = carry.ValueRO;

                    var ballTransform = state.EntityManager.GetComponentData<LocalTransform>(carried.Target);
                    ballTransform.Position = playerTransform.ValueRO.Position;
                    state.EntityManager.SetComponentData(carried.Target, ballTransform);

                    state.EntityManager.SetComponentData(carried.Target, new CarryEnabled { Value = false });
                    state.EntityManager.SetComponentData(playerEntity, new CarryEnabled { Value = false });
                }
                else
                {
                    // pick up first ball in range
                    foreach (var (ballTransform, ballCarry, ballCarryEnabled, ballEntity) in
                             SystemAPI.Query<RefRO<LocalTransform>, RefRO<Carry>, RefRO<CarryEnabled>>()
                                 .WithAll<Ball>()
                                 .WithEntityAccess())
                    {
                        if (ballCarryEnabled.ValueRO.Value)
                            continue;

                        float distSQ = math.distancesq(playerTransform.ValueRO.Position,
                            ballTransform.ValueRO.Position);

                        if (distSQ <= config.BallKickingRangeSQ)
                        {
                            state.EntityManager.SetComponentData(ballEntity, new Velocity());

                            state.EntityManager.SetComponentData(playerEntity, new Carry { Target = ballEntity });
                            state.EntityManager.SetComponentData(ballEntity, new Carry { Target = playerEntity });
                            
                            state.EntityManager.SetComponentData(playerEntity, new CarryEnabled { Value = true });
                            state.EntityManager.SetComponentData(ballEntity, new CarryEnabled { Value = true });
                            break;
                        }
                    }
                }
            }
        }
    }
}
