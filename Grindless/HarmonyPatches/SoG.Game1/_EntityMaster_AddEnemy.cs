using Microsoft.Xna.Framework;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1._EntityMaster_AddEnemy), typeof(ushort), typeof(EnemyCodex.EnemyTypes), typeof(Vector2), typeof(int), typeof(float), typeof(Enemy.SpawnEffectType), typeof(bool), typeof(bool), typeof(float[]))]
    static class _EntityMaster_AddEnemy
    {
        static void Prefix(ref EnemyCodex.EnemyTypes __state, ref ushort iEnemyID, ref EnemyCodex.EnemyTypes enEnemyType, ref Vector2 p_v2Pos, ref int ibitLayer, ref float fVirtualHeight, ref Enemy.SpawnEffectType enSpawnEffect, ref bool bAsElite, ref bool bDropsLoot, float[] afBehaviourVariables)
        {
            __state = enEnemyType;
            foreach (Mod mod in ModManager.Mods)
                mod.OnEnemySpawn(ref enEnemyType, ref p_v2Pos, ref bAsElite, ref bDropsLoot, ref ibitLayer, ref fVirtualHeight, afBehaviourVariables);
        }

        static void Postfix(ref EnemyCodex.EnemyTypes __state, Enemy __result, ushort iEnemyID, EnemyCodex.EnemyTypes enEnemyType, Vector2 p_v2Pos, int ibitLayer, float fVirtualHeight, Enemy.SpawnEffectType enSpawnEffect, bool bAsElite, bool bDropsLoot, float[] afBehaviourVariables)
        {
            foreach (Mod mod in ModManager.Mods)
                mod.PostEnemySpawn(__result, enEnemyType, __state, p_v2Pos, bAsElite, bDropsLoot, ibitLayer, fVirtualHeight, afBehaviourVariables);
        }
    }
}
