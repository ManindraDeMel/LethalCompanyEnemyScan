using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using EnemyScan;
using System.Linq;

namespace EnemyScan.Helper
{
    [HarmonyPatch(typeof(RoundManager))]
    public static class ScanEnemies
    {
        public static string enemyString = "No Enemies Found.";

        [HarmonyPatch("AdvanceHourAndSpawnNewBatchOfEnemies")]
        [HarmonyPostfix]
        public static void GetEnemies()
        {
            UpdateEnemyCount();
        }
        [HarmonyPatch("BeginEnemySpawning")]
        [HarmonyPostfix]
        public static void GetInitialEnemies()
        {
            UpdateEnemyCount();
        }
        private static void UpdateEnemyCount()
        {
            EnemyAI[] enemyAIs = UnityEngine.Object.FindObjectsOfType<EnemyAI>();
            enemyString = BuildEnemyCountString(enemyAIs.ToList());
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
                return "No Enemies Found..";
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
