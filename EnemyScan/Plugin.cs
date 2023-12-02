using BepInEx;
using HarmonyLib;
using TerminalApi;
using static TerminalApi.TerminalApi;
using EnemyScan.Helper;
using System.Reflection;
namespace EnemyScan
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency("atomic.terminalapi")]
    public class EnemyScan : BaseUnityPlugin
    {
        private const string modGUID = "299792458.EnemyScan";
        private const string modName = "EnemyScan";
        private const string modVersion = "1.0.0";

        void Awake()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            
            TerminalKeyword verbKeyword = CreateTerminalKeyword("list", true);
            TerminalKeyword nounKeyword = CreateTerminalKeyword("enemies");
            TerminalNode triggerNode = CreateTerminalNode($"ERROR! No Enemies in space\n", true);

            verbKeyword = verbKeyword.AddCompatibleNoun(nounKeyword, triggerNode);
            nounKeyword.defaultVerb = verbKeyword;

            AddTerminalKeyword(verbKeyword);
            AddTerminalKeyword(nounKeyword);
        }

        public static void UpdateEnemyCommand()
        {
            if (MoonManager.isOnMoon)
            {
                UpdateKeywordCompatibleNoun("list", "enemies", CreateTerminalNode($"{ScanEnemies.enemyString}\n", true));
            }
            else
            {
                UpdateKeywordCompatibleNoun("list", "enemies", CreateTerminalNode($"ERROR! No Enemies in space\n", true));
            }
        }
    }
}