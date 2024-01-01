using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.Logging;
using TerminalApi;
using TerminalApi.Events;
using static TerminalApi.TerminalApi;

namespace EnemyScan;

[BepInPlugin(modGUID, modName, modVersion)]
[BepInDependency("atomic.terminalapi")]
public class EnemyScan : BaseUnityPlugin
{
    private const string modGUID = "299792458.EnemyScan";
    private const string modName = "EnemyScan";
    private const string modVersion = "1.2.1";
    private const string DefaultString = "No Enemies Found.\n\n";
    private static TerminalNode _triggerNode = null!;
    private static ManualLogSource _log = null!;

    private void Awake()
    {
        _log = Logger;

        var verbKeyword = CreateTerminalKeyword("list", true);
        var nounKeyword = CreateTerminalKeyword("enemies");
        nounKeyword.defaultVerb = verbKeyword;

        _triggerNode = CreateTerminalNode(DefaultString, true);
        verbKeyword.AddCompatibleNoun(nounKeyword, _triggerNode);

        AddTerminalKeyword(verbKeyword);
        AddTerminalKeyword(nounKeyword);

        Events.TerminalParsedSentence += OnTerminalParsedSentence;
    }

    private static void OnTerminalParsedSentence(object sender, Events.TerminalParseSentenceEventArgs e)
    {
        if (e.ReturnedNode != _triggerNode)
            return;

        _triggerNode.displayText = BuildEnemyCountString();
    }

    private static string BuildEnemyCountString()
    {
        var enemies = FindObjectsOfType<EnemyAI>()
            .Where(ai => ai.GetComponentInChildren<ScanNodeProperties>() is not null) // do not show what we cannot scan
            .GroupBy(ai => ai.enemyType.enemyName)
            .OrderBy(g => g.Key)
            .ToList();

        if (enemies.Count == 0)
            return DefaultString;

        StringBuilder sb = new();
        foreach (var group in enemies)
        {
            sb.Append(group.Key);
            sb.Append(": ");
            sb.AppendLine(group.Count().ToString());
        }
        
        sb.AppendLine();
        return sb.ToString();
    }
}