using UnityEngine;
using Unity.Entities;
using PGD;
using PGD.Jobs;

namespace Tutorials.Tornado
{
    public class BarAuthoring : MonoBehaviour
    {
        class Baker : PGDHybrid<BarAuthoring>
        {
            public override void Handle(BarAuthoring authoring)
            {
                var entity = GetHybridEntity();
                AddComponent<Bar>(entity);
                AddComponent<BarThickness>(entity);
            }
        }
    }

    public struct Bar : IComponent
    {
        public int pointA;
        public int pointB;
        public float length;
    }

    public struct BarThickness : IComponent
    {
        public float Value;
    }
}