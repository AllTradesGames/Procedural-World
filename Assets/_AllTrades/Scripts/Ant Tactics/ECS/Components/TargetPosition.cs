using Unity.Entities;

namespace AntTactics.Components
{
    public struct TargetPosition : IComponentData
    {
        public bool destroyOnReachTarget;
        public float x;
        public float y;
        public float z;
    }
}
