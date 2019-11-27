///* 
//    ------------------- Code Monkey -------------------
//
//    Thank you for downloading this package
//    I hope you find it useful in your projects
//    If you have any questions let me know
//    Cheers!
//
//               unitycodemonkey.com
//    --------------------------------------------------
// */
//
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Unity.Transforms;
//using Unity.Entities;
//using Unity.Mathematics;
//
//public class FindTargetSystem : ComponentSystem {
//
//    protected override void OnUpdate() {
//        Entities.WithNone<HasTarget>().WithAll<Unit>().ForEach((Entity entity, ref Translation unitTranslation) => {
//            // Code running on all entities with "Unit" Tag
//            //Debug.Log("Unit: " + entity);
//
//            float3 unitPosition = unitTranslation.Value;
//            Entity closestTargetEntity = Entity.Null;
//            float3 closestTargetPosition = float3.zero;
//
//            Entities.WithAll<Target>().ForEach((Entity targetEntity, ref Translation targetTranslation) => { 
//                // Cycling through all entities with "Target" Tag
//                //Debug.Log("Target: " + targetEntity);
//
//                if (closestTargetEntity == Entity.Null) {
//                    // No target
//                    closestTargetEntity = targetEntity;
//                    closestTargetPosition = targetTranslation.Value;
//                } else {
//                    if (math.distance(unitPosition, targetTranslation.Value) < math.distance(unitPosition, closestTargetPosition)) {
//                        // This target is closer
//                        closestTargetEntity = targetEntity;
//                        closestTargetPosition = targetTranslation.Value;
//                    }
//                }
//            });
//
//            // Closest Target
//            if (closestTargetEntity != Entity.Null) {
//                //Debug.DrawLine(unitPosition, closestTargetPosition);
//                PostUpdateCommands.AddComponent(entity, new HasTarget { targetEntity = closestTargetEntity });
//            }
//        });
//    }
//
//}
