﻿using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using System.Reflection.Emit;
using System.Reflection;
using Bagmen;
using Microsoft.Xna.Framework;
using SoG;

namespace Grindless.Patches
{
    [HarmonyPatch(typeof(MonsterSpawnInArenaTrials))]
    internal class Bagmen_MonsterSpawnInArenaTrials
    {
        [HarmonyTranspiler]
        [HarmonyPatch("SpawnEnemy", typeof(EnemyCodex.EnemyTypes), typeof(Vector2), typeof(Vector2))]
        public static IEnumerable<CodeInstruction> SpawnEnemy_Transpiler(IEnumerable<CodeInstruction> code, ILGenerator generator)
        {
            List<CodeInstruction> codeList = code.ToList();

            List<CodeInstruction> inserted = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Enemy), nameof(Enemy.enType))),
                new CodeInstruction(OpCodes.Starg, 1)
            };

            MethodInfo info = AccessTools.Method(typeof(Game1), nameof(Game1._EntityMaster_AddEnemy), new Type[] { typeof(EnemyCodex.EnemyTypes), typeof(Vector2), typeof(int), typeof(float), typeof(bool), typeof(Enemy.SpawnEffectType), typeof(float[]) });

            return codeList.InsertAfterMethod(info, inserted);
        }
    }
}
