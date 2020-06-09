using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class PlayerInputSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        // ref for writing to, in for reading from -- Must list refs, then ins
        Entities.ForEach((ref PaddleMovementData moveData, in PaddleInputData inputData) =>
        {
            moveData.direction = Input.GetKey(inputData.upKey) ? 1 : Input.GetKey(inputData.downKey) ? -1 : 0;
        }).Run(); // .Run() for main thread, .Schedule() for threading

        return default;
    }
}