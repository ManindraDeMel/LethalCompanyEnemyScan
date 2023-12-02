using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using TerminalApi;
using EnemyScan;

namespace EnemyScan.Helper
{
    public static class ScanEnemies
    {
        public static string enemyString = "No Enemies Found.";

        [HarmonyPatch(typeof(RoundManager), "RefreshEnemiesList()")]
        [HarmonyPostfix]
        public static void GetEnemies(List<EnemyAI> ___spawnedAIs)
        {
            enemyString = BuildEnemyCountString(___spawnedAIs);
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

            return sb.ToString();
        }
    }
    [HarmonyPatch(typeof(StartOfRound))]
    public static class MoonManager
    {
        public static bool isOnMoon = true;

        [HarmonyPatch("StartGame")]
        [HarmonyPostfix]
        public static void StartingGame()
        {
            isOnMoon = true;
            EnemyScan.UpdateEnemyCommand();
        }

        [HarmonyPatch("ShipHasLeft")]
        [HarmonyPostfix]
        public static void EndingGame()
        {
            isOnMoon = false;
            EnemyScan.UpdateEnemyCommand();
        }

    }
}
