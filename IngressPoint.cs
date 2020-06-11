using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

using Harmony;
using UnityEngine;


namespace IncreasedRange
{
    public static class IngressPoint
    {
        [HarmonyPatch(typeof(TechWeapon))]
        [HarmonyPatch("OnSpawn")]
        public static class PatchManualRange
        {
            public static void Postfix(ref TechWeapon __instance)
            {
                __instance.m_ManualTargetingSettingsGamepad.m_ManualTargetingRadiusSP = 3000.0f;
                __instance.m_ManualTargetingSettingsMAndKB.m_ManualTargetingRadiusSP = 3000.0f;
            }
        }

        [HarmonyPatch(typeof(ChargeAtVisible))]
        [HarmonyPatch("OnAwake")]
        public static class PatchAttackRange
        {
            public static void Postfix(ref ChargeAtVisible __instance)
            {
                __instance.m_AttackRange = 1500.0f;
            }
        }

        /* [HarmonyPatch(typeof(ModuleRadar))]
        [HarmonyPatch("OnPool")]
        public static class PatchModuleRadar
        {
            public static void Postfix(ref ModuleRadar __instance)
            {
                FieldInfo m_Range = typeof(ModuleRadar).GetField("m_Range", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                m_Range.SetValue(__instance, Mathf.Max(900.0f, (float)m_Range.GetValue(__instance)));
            }
        }

        [HarmonyPatch(typeof(ModuleVision))]
        [HarmonyPatch("OnPool")]
        public static class PatchModuleVision
        {
            public static void Postfix(ref ModuleVision __instance)
            {
                __instance.visionRange = Mathf.Max(__instance.visionRange, 900.0f);
                __instance.visionConeAngle = 360.0f;
                FieldInfo m_Range = typeof(ModuleVision).GetField("m_Range", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                m_Range.SetValue(__instance, Mathf.Max(900.0f, (float)m_Range.GetValue(__instance)));
            }
        } */

        public static void Main()
        {
            FieldInfo m_SleepRangeFromCamera = typeof(ManTechs).GetField("m_SleepRangeFromCamera", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            m_SleepRangeFromCamera.SetValue(Singleton.Manager<ManTechs>.inst, 3000.0f);
            HarmonyInstance.Create("flsoz.ttmm.kaizott.mod").PatchAll(Assembly.GetExecutingAssembly());

            /* try
            {
                WeaponAimMod.src.WrappedDataHolder.cheatDisabled = true;
            }
            catch
            {
                Console.WriteLine("Failed to enable enemy target lead");
            } */
        }
    }
}
