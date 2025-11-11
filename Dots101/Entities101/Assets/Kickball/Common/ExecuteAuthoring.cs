using Unity.Entities;
using UnityEngine;
using PGD;
using PGD.Jobs;

namespace Tutorials.Kickball.Execute
{
    public class ExecuteAuthoring : MonoBehaviour
    {
        [Header("Step 1")]
        public bool ObstacleSpawner;
        [Header("Step 2")]
        public bool PlayerSpawner;
        public bool PlayerMovement;
        [Header("Step 3")]
        public bool BallSpawner;
        public bool BallMovement;
        [Header("Step 4")]
        public bool NewPlayerMovement;
        public bool NewBallMovement;
        [Header("Step 5")]
        public bool BallCarry;
        public bool BallKicking;
        class Baker : PGDHybrid<ExecuteAuthoring>
        {
            public override void Handle(ExecuteAuthoring authoring)
            {
                var entity = GetHybridEntity();
                if (authoring.ObstacleSpawner)
                    AddComponent<ObstacleSpawner>(entity);
                if (authoring.PlayerMovement)
                    AddComponent<PlayerMovement>(entity);
                if (authoring.PlayerSpawner)
                    AddComponent<PlayerSpawner>(entity);
                if (authoring.BallSpawner)
                    AddComponent<BallSpawner>(entity);
                if (authoring.BallMovement)
                    AddComponent<BallMovement>(entity);
                if (authoring.NewPlayerMovement)
                    AddComponent<NewPlayerMovement>(entity);
                if (authoring.NewBallMovement)
                    AddComponent<NewBallMovement>(entity);
                if (authoring.BallCarry)
                    AddComponent<BallCarry>(entity);
                if (authoring.BallKicking)
                    AddComponent<BallKicking>(entity);
            }
        }
    }

    public struct ObstacleSpawner : IComponent
    {
    }

    public struct PlayerMovement : IComponent
    {
    }

    public struct BallMovement : IComponent
    {
    }

    public struct NewPlayerMovement : IComponent
    {
    }

    public struct NewBallMovement : IComponent
    {
    }

    public struct PlayerSpawner : IComponent
    {
    }

    public struct BallSpawner : IComponent
    {
    }

    public struct BallCarry : IComponent
    {
    }

    public struct BallKicking : IComponent
    {
    }
}