using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using AntTactics.Components;

namespace AntTactics.Systems
{
    public class MoveTowardTargetSystem : JobComponentSystem
    {
        private struct MoveTowardTargetJob : IJobProcessComponentData<Speed, Position, TargetPosition>
        {
            public float deltaTime;

            public void Execute(ref Speed speed, ref Position position, ref TargetPosition targetPosition)
            {
                // TODO: move stuff
                //position.Value.x += speed.value * deltaTime;
            }
        }

        protected override JobHandle OnUpdate(JobHandle dependsOn)
        {
            MoveTowardTargetJob job = new MoveTowardTargetJob
            {
                deltaTime = Time.deltaTime
            };
            return job.Schedule(this, dependsOn);
        }


    }
}
