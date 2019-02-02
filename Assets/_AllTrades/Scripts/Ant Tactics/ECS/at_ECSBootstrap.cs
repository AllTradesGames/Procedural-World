using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using AntTactics.Components;

public class at_ECSBootstrap : MonoBehaviour
{
    public float antSpeed;
    public Mesh antMesh;
    public Material antMaterial;

    private EntityManager entityManager;

    void Start()
    {
        entityManager = World.Active.GetOrCreateManager<EntityManager>();
    }


    public void SpawnAnts(int amount, Vector3 start, Vector3 target)
    {
        Entity antEntity;
        for (int ii = 0; ii < amount; ii++)
        {
            antEntity = entityManager.CreateEntity(
                ComponentType.Create<Position>(),
                ComponentType.Create<TargetPosition>(),
                ComponentType.Create<Speed>(),
                ComponentType.Create<MeshInstanceRenderer>()
            );
            
            entityManager.SetComponentData(antEntity, new Speed{
                value = antSpeed
            });
            entityManager.SetComponentData(antEntity, new TargetPosition{
                x = target.x,
                y = target.y,
                z = target.z,
                destroyOnReachTarget = true
            });
            entityManager.SetSharedComponentData(antEntity, new MeshInstanceRenderer{
                mesh = antMesh,
                material = antMaterial
            });
        }
    }





}
