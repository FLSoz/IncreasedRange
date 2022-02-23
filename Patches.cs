using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using HarmonyLib;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;


namespace IncreasedRange
{
    public static class Patches
    {
        private static FieldInfo behaviorTreeMap = typeof(BehaviorManager).GetField("behaviorTreeMap", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        private static bool ranOnce = false;

        /* [HarmonyPatch(typeof(Weapons))]
        [HarmonyPatch("AimAtTarget")]
        public static class PatchAlwaysFire
        {
            public static void Postfix(ref Weapons __instance, ref Tank tank)
            {
                __instance.FireWeapons(tank);
            }
        } */

        /* [HarmonyPatch(typeof(ModuleWeapon))]
        [HarmonyPatch("UpdateAutoAimBehaviour")]
        public static class PatchAlwaysFire
        {
            private static FieldInfo m_HasTargetInFiringCone = typeof(ModuleWeapon).GetField("m_HasTargetInFiringCone", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            private static FieldInfo m_CurrentAITreeType = typeof(TechAI).GetField("m_CurrentAITreeType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            public static void Postfix(ref ModuleWeapon __instance)
            {
                if ((bool) m_HasTargetInFiringCone.GetValue(__instance) && __instance.block.tank.IsAIControlled())
                {
                    AITreeType currentTreeType = (AITreeType)m_CurrentAITreeType.GetValue(__instance.block.tank.AI);
                    if (currentTreeType != null && currentTreeType.IsType(AITreeType.AITypes.Guard))
                    {
                        __instance.FireControl = true;
                    }
                }
            }
        } */

        public static class PatchLog
        {
            /*
            [HarmonyPatch(typeof(BehaviorManager.BehaviorTree))]
            [HarmonyPatch("Initialize")]
            public static class PatchInitialize
            {
                public static void Postfix(ref BehaviorManager.BehaviorTree __instance)
                {
                    Console.WriteLine($"Initializing Behavior Tree {__instance.behavior}");
                    __instance.behavior.LogTaskChanges = true;
                }
            }
            */

            /* [HarmonyPatch(typeof(BehaviorManager))]
            [HarmonyPatch("EnableBehavior")]
            public static class PatchEnableBehavior
            {
                private static FieldInfo behaviorTreeMap = typeof(BehaviorManager).GetField("behaviorTreeMap", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                private static FieldInfo pausedBehaviorTrees = typeof(BehaviorManager).GetField("pausedBehaviorTrees", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                public static bool Prefix(ref BehaviorManager __instance, ref Behavior behavior)
                {
                    Dictionary<Behavior, BehaviorManager.BehaviorTree> behaviorTreeMap = (Dictionary<Behavior, BehaviorManager.BehaviorTree>)PatchEnableBehavior.behaviorTreeMap.GetValue(__instance);
                    if (__instance.IsBehaviorEnabled(behavior) && behaviorTreeMap.TryGetValue(behavior, out BehaviorManager.BehaviorTree tree))
                    {
                        Console.WriteLine($"Attempt to enable already enabled Behavior Tree {tree.behavior.BehaviorName} : {tree.behavior.BehaviorDescription}");
                        return false;
                    }
                    Dictionary<Behavior, BehaviorManager.BehaviorTree> pausedBehaviorTrees = (Dictionary<Behavior, BehaviorManager.BehaviorTree>)PatchEnableBehavior.pausedBehaviorTrees.GetValue(__instance);

                    if (behavior != null && BehaviorManager.instance != null && pausedBehaviorTrees.TryGetValue(behavior, out tree))
                    {
                        Console.WriteLine($"Enabling Paused Behavior Tree {tree.behavior.BehaviorName} : {tree.behavior.BehaviorDescription}");
                        tree.behavior.LogTaskChanges = true;
                    }
                    else
                    {
                        Console.WriteLine("Enabling Uninitialized Behavior Tree");
                    }
                    return true;
                }

                private static string NodeString(BehaviorDesigner.Runtime.Tasks.Task task)
                {
                    return $"({task.FriendlyName}:{task.GetHashCode()})";
                }

                public static void Postfix(ref BehaviorManager __instance, ref Behavior behavior)
                {
                    Dictionary<Behavior, BehaviorManager.BehaviorTree> behaviorTreeMap = (Dictionary<Behavior, BehaviorManager.BehaviorTree>)PatchEnableBehavior.behaviorTreeMap.GetValue(__instance);

                    if (behavior != null && BehaviorManager.instance != null)
                    {
                        BehaviorManager.BehaviorTree tree = behaviorTreeMap[behavior];
                        Console.WriteLine("MY DEBUG:");
                        Console.WriteLine($"Task List: [{(tree.taskList == null ? "" : string.Join(", ", tree.taskList.Select(task => NodeString(task))))}]");
                        Console.WriteLine($"Parent Index: [{(tree.parentIndex == null ? "" : string.Join(", ", tree.parentIndex.Select(ind => ind < 0 ? "ROOT" : NodeString(tree.taskList[ind]))))}]");
                        Console.WriteLine($"Children Index: [\n{(tree.childrenIndex == null ? "" : string.Join(",\n", tree.childrenIndex.Select(list => list == null ? "\t[]" : "\t[" + string.Join(", ", list.Select(ind => ind < 0 ? "ROOT" : NodeString(tree.taskList[ind]))) + "]")))}]");
                        Console.WriteLine($"Relative Child Index: [{(tree.relativeChildIndex == null ? "" : string.Join(", ", tree.relativeChildIndex.Select(ind => ind < 0 ? "ROOT" : NodeString(tree.taskList[ind]))))}]");
                        Console.WriteLine($"Parent Composite Index: [{(tree.parentCompositeIndex == null ? "" : string.Join(", ", tree.parentCompositeIndex.Select(ind => ind < 0 ? "ROOT" : NodeString(tree.taskList[ind]))))}]");
                        Console.WriteLine($"Children Conditional Index: [\n{(tree.childConditionalIndex == null ? "" : string.Join(",\n", tree.childConditionalIndex.Select(list => list == null ? "\t[]" : "\t[" + string.Join(", ", list.Select(ind => ind < 0 ? "ROOT" : NodeString(tree.taskList[ind]))) + "]")))}]");
                    }
                }
            } */
        }

        [HarmonyPatch(typeof(TechWeapon), "ManualTargetingRadius", MethodType.Getter)]
        public static class PatchManualRange
        {
            [HarmonyPostfix]
            public static void Postfix(ref float __result)
            {
                __result = Math.Max(IngressPoint.ManualTargetingRange, __result);
            }
        }

        /* [HarmonyPatch(typeof(EnemyInRange))]
        [HarmonyPatch("OnUpdate")]
        public static class PatchNeverRange
        {
            public static void Postfix(ref TaskStatus __result)
            {
                __result = TaskStatus.Failure;
            }
        } */

        // For standard AI enemies
        [HarmonyPatch(typeof(ChargeAtVisible), "OnUpdate")]
        public static class PatchAttackRange
        {
            [HarmonyPrefix]
            public static void Prefix(ref ChargeAtVisible __instance, out float __state)
            {
                __state = __instance.m_AttackRange;
                __instance.m_AttackRange = Mathf.Max(IngressPoint.EnemyChargeRange, __state);
            }

            [HarmonyPostfix]
            public static void Postfix(ref ChargeAtVisible __instance, float __state)
            {
                __instance.m_AttackRange = __state;
            }
        }

        [HarmonyPatch(typeof(GetNearestPlayerOnTeam), "OnUpdate")]
        public static class PatchGetNearestPlayerOnTeam
        {
            [HarmonyPrefix]
            public static void Prefix(ref GetNearestPlayerOnTeam __instance, out float __state)
            {
                __state = __instance.m_MaxRange.Value;
                __instance.m_MaxRange.SetValue(Mathf.Max(IngressPoint.AllySearchRange, __state));
            }

            [HarmonyPostfix]
            public static void Postfix(ref GetNearestPlayerOnTeam __instance, float __state)
            {
                __instance.m_MaxRange.SetValue(__state);
            }
        }

        /* [HarmonyPatch(typeof(VisibleInRange))]
        [HarmonyPatch("OnAwake")]
        public static class PatchVisibleInRange
        {
            public static void Postfix(ref VisibleInRange __instance)
            {
                __instance.m_Range.SetValue(900.0f);
            }
        }

        [HarmonyPatch(typeof(TechVision))]
        [HarmonyPatch("OnSpawn")]
        public static class PatchTechVision
        {
            private static FieldInfo m_SearchRadius = typeof(TechVision).GetField("m_SearchRadius", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            public static void Postfix(ref TechVision __instance)
            {
                m_SearchRadius.SetValue(__instance, Mathf.Max(900.0f, (float)m_SearchRadius.GetValue(__instance)));
            }
        } */

        [HarmonyPatch(typeof(ModuleRadar), "Range", MethodType.Getter)]
        public static class PatchModuleRadar
        {
            [HarmonyPostfix]
            public static void Postfix(ref float __result)
            {
                __result = Math.Max(IngressPoint.IncreasedRadarRange, __result);
            }
        }

        [HarmonyPatch(typeof(ModuleVision), "Range", MethodType.Getter)]
        public static class PatchModuleVision
        {
            [HarmonyPostfix]
            public static void Postfix(ref float __result)
            {
                __result = Math.Max(IngressPoint.IncreasedVisionRange, __result);
            }
        }

        /* [HarmonyPatch(typeof(TechAI))]
        [HarmonyPatch("OnSpawn")]
        public static class PatchLogIn
        {
            private static MethodInfo EnableCurrentTree = typeof(TechAI).GetMethod("EnableCurrentTree", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            private static MethodInfo DisableCurrentTree = typeof(TechAI).GetMethod("DisableCurrentTree", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            private static FieldInfo m_BehaviorTree = typeof(TechAI).GetField("m_BehaviorTree", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            private static FieldInfo m_CurrentAITreeType = typeof(TechAI).GetField("m_CurrentAITreeType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            private static string NodeString(BehaviorDesigner.Runtime.Tasks.Task task)
            {
                return $"({task.FriendlyName}:{task.GetHashCode()})";
            }

            public static void Postfix(ref TechAI __instance)
            {
                BehaviorTree behavior = (BehaviorTree)m_BehaviorTree.GetValue(__instance);
                if (!IngressPoint.ranOnce && behavior != null)
                {
                    IngressPoint.ranOnce = true;
                    // AITreeType.AITypes originalTreeType = ((AITreeType)m_CurrentAITreeType.GetValue(__instance)).GetAIType();

                    // For every AI Type, force enable
                    List<AITreeType.AITypes> treeTypes = Enum.GetValues(typeof(AITreeType.AITypes)).Cast<AITreeType.AITypes>().ToList();

                    Behavior.CreateBehaviorManager();

                    Dictionary<Behavior, BehaviorManager.BehaviorTree> behaviorTreeMap = (Dictionary<Behavior, BehaviorManager.BehaviorTree>)IngressPoint.behaviorTreeMap.GetValue(BehaviorManager.instance);

                    // FollowPassive is null
                    // Specific is null

                    List<AITreeType.AITypes> exclusionList = new List<AITreeType.AITypes>() {
                       AITreeType.AITypes.Escort,
                       AITreeType.AITypes.Idle,
                       AITreeType.AITypes.Guard,
                       AITreeType.AITypes.Harvest,
                       AITreeType.AITypes.FollowPassive
                    };

                    foreach (AITreeType.AITypes aiType in treeTypes.Where(item => !exclusionList.Contains(item)))
                    {
                        Console.WriteLine($"AI TYPE: {aiType}");
                        __instance.SetBehaviorType(new AITreeType(aiType), true);
                        EnableCurrentTree.Invoke(__instance, null);

                        d.Assert(behavior != null);

                        BehaviorManager.BehaviorTree tree = behaviorTreeMap[behavior];
                        Console.WriteLine("MY DEBUG:");
                        Console.WriteLine($"Task List: [{(tree.taskList == null ? "" : string.Join(", ", tree.taskList.Select(task => NodeString(task))))}]");
                        Console.WriteLine($"Parent Index: [{(tree.parentIndex == null ? "" : string.Join(", ", tree.parentIndex.Select(ind => ind < 0 ? "ROOT" : NodeString(tree.taskList[ind]))))}]");
                        Console.WriteLine($"Children Index: [\n{(tree.childrenIndex == null ? "" : string.Join(",\n", tree.childrenIndex.Select(list => list == null ? "\t[]" : "\t[" + string.Join(", ", list.Select(ind => ind < 0 ? "ROOT" : NodeString(tree.taskList[ind]))) + "]")))}]");
                        Console.WriteLine($"Relative Child Index: [{(tree.relativeChildIndex == null ? "" : string.Join(", ", tree.relativeChildIndex.Select(ind => ind < 0 ? "ROOT" : NodeString(tree.taskList[ind]))))}]");
                        Console.WriteLine($"Parent Composite Index: [{(tree.parentCompositeIndex == null ? "" : string.Join(", ", tree.parentCompositeIndex.Select(ind => ind < 0 ? "ROOT" : NodeString(tree.taskList[ind]))))}]");
                        Console.WriteLine($"Children Conditional Index: [\n{(tree.childConditionalIndex == null ? "" : string.Join(",\n", tree.childConditionalIndex.Select(list => list == null ? "\t[]" : "\t[" + string.Join(", ", list.Select(ind => ind < 0 ? "ROOT" : NodeString(tree.taskList[ind]))) + "]")))}]");

                        DisableCurrentTree.Invoke(__instance, null);
                    }

                    // __instance.SetBehaviorType(new AITreeType(originalTreeType), true);
                    EnableCurrentTree.Invoke(__instance, null);
                }
            }
        } */
    }
}
