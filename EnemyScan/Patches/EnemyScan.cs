using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using System.Linq;

namespace EnemyScan.Helper
{
    [HarmonyPatch(typeof(RoundManager))]
    public static class ScanEnemies
    {
        public static string enemyString = "No Enemies Found";
        public static RoundManager roundManager;
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake(ref RoundManager __instance)
        {
            roundManager = __instance;
        }
        public static void UpdateEnemyCount()
        {
            enemyString = BuildEnemyCountString(roundManager.SpawnedEnemies);
            EnemyScan.UpdateEnemyCommand();
        }
        private static string BuildEnemyCountString(List<EnemyAI> enemies)
        {
            var enemyCount = new Dictionary<string, int>();

            // Counting occurrences
            foreach (var enemy in enemies)
            {
                string enemyName = enemy.enemyType.enemyName;

                if (enemyCount.ContainsKey(enemyName))
                {
                    enemyCount[enemyName]++;
                }
                else
                {
                    enemyCount[enemyName] = 1;
                }
            }

            // Building the string
            StringBuilder sb = new StringBuilder();
            foreach (var pair in enemyCount)
            {
                sb.AppendLine($"{pair.Key}: {pair.Value}");
            }
            if (sb.ToString() == "")
            {
                return "No Enemies Found";
            }
            return sb.ToString();
        }
    }
    [HarmonyPatch(typeof(Terminal))]
    public static class TerminalPatch
    {
        [HarmonyPatch("BeginUsingTerminal")]
        [HarmonyPostfix]
        public static void BeginUsingTerminal() {
            ScanEnemies.UpdateEnemyCount();
        }
    }
}
