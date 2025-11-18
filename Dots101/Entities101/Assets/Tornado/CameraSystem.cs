using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using PGD;
using PGD.Jobs;

namespace Tutorials.Tornado
{
    //[UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]
    public partial class CameraSystem : PGDSystemEnhanced
    {
        [BurstCompile]
        protected override void OnCreate(ref PGDSystemState state)
        {
            state.RequireForUpdate<Config>();
        }

        protected override void OnUpdate(ref PGDSystemState state)
        {
            var tornadoPosition = BuildingSystem.Position((float)PGDGameContext.Time.ElapsedTime);
            var cam = Camera.main.transform;
            cam.position = new Vector3(tornadoPosition.x, 10f, tornadoPosition.y) - cam.forward * 60f;
        }
    }
}