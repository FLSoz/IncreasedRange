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
    public static class IngressPoint
    {
        internal static float SleepRangeFromCamera = 600.0f;   // What we modify it to
        internal static float IncreasedRadarRange = 900.0f;
        internal static float IncreasedVisionRange = 900.0f;
        internal static float ManualTargetingRange = 900.0f;
        internal static float DefaultSleepRangeFromCamera = 900.0f;
        internal static float EnemyChargeRange = 900.0f;
        internal static float AllySearchRange = 1800.0f;

        internal static FieldInfo m_SleepRangeFromCamera = typeof(ManTechs).GetField("m_SleepRangeFromCamera", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        internal const string HarmonyID = "flsoz.ttmm.increasedrange.mod";
        internal static Harmony harmony = new Harmony(HarmonyID);

        public static void Main()
        {
            IngressPoint.m_SleepRangeFromCamera.SetValue(Singleton.Manager<ManTechs>.inst, IngressPoint.SleepRangeFromCamera);
            IngressPoint.harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    public class IncreasedRangeMod : ModBase {
        public override void EarlyInit()
        {
            IngressPoint.DefaultSleepRangeFromCamera = (float)IngressPoint.m_SleepRangeFromCamera.GetValue(Singleton.Manager<ManTechs>.inst);
        }

        public override bool HasEarlyInit()
        {
            return true;
        }

        public override void Init()
        {
            IngressPoint.Main();
        }

        public override void DeInit()
        {
            IngressPoint.m_SleepRangeFromCamera.SetValue(Singleton.Manager<ManTechs>.inst, IngressPoint.DefaultSleepRangeFromCamera);
            Console.WriteLine($"Reset sleep range to : {IngressPoint.m_SleepRangeFromCamera.GetValue(Singleton.Manager<ManTechs>.inst)}");
            IngressPoint.harmony.UnpatchAll(IngressPoint.HarmonyID);
        }
    }
}
