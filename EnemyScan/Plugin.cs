using BepInEx;
using HarmonyLib;
using TerminalApi;
using static TerminalApi.TerminalApi;
using EnemyScan.Helper;
using System.Reflection;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace EnemyScan
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency("atomic.terminalapi")]
    public class EnemyScan : BaseUnityPlugin
    {
        private const string modGUID = "299792458.EnemyScan";
        private const string modName = "EnemyScan";
        private const string modVersion = "1.2.0";


        private ConfigEntry<float> cooldown;
        private ConfigEntry<float> cost;

        private static float lastScanTime = 0f;

        public ManualLogSource logSource;
        void Awake()
        {
            // Initialize the configuration entries
            cooldown = Config.Bind("General", "Cooldown", 0f, "Cooldown time for the enemy scan command.");
            cost = Config.Bind("General", "Cost", 0f, "Cost to execute the enemy scan command.");
            logSource = BepInEx.Logging.Logger.CreateLogSource("logSource");
            TerminalKeyword verbKeyword = CreateTerminalKeyword("list", true);
            TerminalKeyword nounKeyword = CreateTerminalKeyword("enemies");
            TerminalNode triggerNode = CreateTerminalNode($"No Enemies Found.\n", true);

            verbKeyword = verbKeyword.AddCompatibleNoun(nounKeyword, triggerNode);
            nounKeyword.defaultVerb = verbKeyword;

            AddTerminalKeyword(verbKeyword);
            AddTerminalKeyword(nounKeyword);

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        public static void UpdateEnemyCommand()
        {
            UpdateKeywordCompatibleNoun("list", "enemies", CreateTerminalNode($"{ScanEnemies.enemyString}\n", true));
        }
    }
}