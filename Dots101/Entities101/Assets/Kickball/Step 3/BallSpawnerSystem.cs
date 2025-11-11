using Tutorials.Kickball.Execute;
using Tutorials.Kickball.Step1;
using Tutorials.Kickball.Step2;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using PGD;
using PGD.Jobs;

namespace Tutorials.Kickball.Step3
{
    // This UpdateBefore is necessary to ensure the balls get rendered in
    // the correct position for the frame in which they're spawned.
    [UpdateSystemBefore(typeof(TransformSystemGroup))]
    public partial class BallSpawnerSystem : PGDSystemEnhanced
    {
        [BurstCompile]
        protected override void OnCreate(ref PGDSystemState state)
        {
            state.RequireForUpdate<BallSpawner>();
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        protected override void OnUpdate(ref PGDSystemState state)
        {
            var config = PGDGameContext.GetSingleton<Config>();
            if (!Input.GetKeyDown(KeyCode.Return))
            {
                return;
            }

            var rand = new Random(123);
            // For every player, spawn a ball, position it at the player's location, and give it a random velocity.
            foreach (var transform in PGDGameContext.QueryForeach<PGDRefRO<PGDLocalTransform>>().WithAll<Player>())
            {
                var ball = state.World.Instantiate(config.BallPrefab);
                ball.Set(new PGDLocalTransform { Position = transform.ValueRO.Position, Rotation = quaternion.identity, Scale = 1 });
                ball.Set(new Velocity { // NextFloat2Direction() returns a random 2d unit vector.
                Value = rand.NextFloat2Direction() * config.BallStartVelocity });
            }
        }
    }
}