using Jint;
using Jint.Native;
using Jint.Native.Object;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace Grindless;

internal class JavaScriptMod : Mod, IDisposable
{
    private readonly Engine _engine;
    private readonly ObjectInstance _mod;
    private readonly JsValue _jsThis;

    private readonly string _name;
    private readonly Version _version;
    private bool disposedValue;

    public override string Name => _name;
    public override Version Version => _version;

    public JavaScriptMod(Dictionary<string, string> sources)
    {
        _engine = new Engine(options =>
        {
            options.Strict = true;
        });

        JSLibrary.LoadConsoleAPI(_engine, () => Logger);

        foreach (var pair in sources)
        {
            _engine.AddModule(pair.Key, pair.Value);
            Program.Logger.LogInformation($"Loading {pair.Key}");
        }

        _mod = _engine.ImportModule("index.js");
        _name = _mod.GetProperty("name").Value.AsString();
        _version = Version.Parse(_mod.GetProperty("version").Value.AsString());

        if (Name == null || Version == null)
            throw new InvalidOperationException("Name or version was not specified.");

        _jsThis = JsValue.FromObject(_engine, this);
        Logger = Program.LogFactory.CreateLogger(Name);
    }

    private void WrapCall(Action action, [CallerMemberName] string methodName = "")
    {
        try
        {
            action();
        }
        catch (Exception e)
        {
            Program.Logger.LogError(e, $"An error occurred when executing method {methodName}!");
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _engine.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public override void Load()
    {
        WrapCall(() =>
        {
            _mod.GetProperty("load").Value.Call(_jsThis);
        });
    }

    public override void Unload()
    {
        WrapCall(() =>
        {
            _mod.GetProperty("unload").Value.Call(_jsThis);
        });
    }

    public override void PostLoad()
    {
        WrapCall(() =>
        {
            _mod.GetProperty("postLoad").Value.Call();
        });
    }

    public override void SaveCharacterData(BinaryWriter stream)
    {
        // TODO
    }

    public override void LoadCharacterData(BinaryReader stream, Version saveVersion)
    {
        // TODO
    }

    public override void SaveWorldData(BinaryWriter stream)
    {
        // TODO
    }

    public override void LoadWorldData(BinaryReader stream, Version saveVersion)
    {
        // TODO
    }

    public override void SaveArcadeData(BinaryWriter stream)
    {
        // TODO
    }

    public override void LoadArcadeData(BinaryReader stream, Version saveVersion)
    {
        // TODO
    }

    private class PlayerDamagedEvent
    {
        public PlayerView player;
        public int damage;
        public byte type;
    }

    public override void OnPlayerDamaged(PlayerView player, ref int damage, ref byte type)
    {
        var gameEvent = new PlayerDamagedEvent
        {
            player = player,
            damage = damage,
            type = type
        };

        WrapCall(() =>
        {
            _mod.GetProperty("onPlayerDamaged").Value.Call(_jsThis, JsValue.FromObject(_engine, gameEvent));
        });

        damage = gameEvent.damage;
        type = gameEvent.type;
    }

    public override void OnPlayerKilled(PlayerView player)
    {
    }

    public override void PostPlayerLevelUp(PlayerView player)
    {
        base.PostPlayerLevelUp(player);
    }

    public override void OnEnemyDamaged(Enemy enemy, ref int damage, ref byte type)
    {
        base.OnEnemyDamaged(enemy, ref damage, ref type);
    }

    public override void PostEnemyKilled(Enemy enemy, AttackPhase killer)
    {
        base.PostEnemyKilled(enemy, killer);
    }

    public override void OnNPCDamaged(NPC enemy, ref int damage, ref byte type)
    {
        base.OnNPCDamaged(enemy, ref damage, ref type);
    }

    public override void OnNPCInteraction(NPC npc)
    {
        base.OnNPCInteraction(npc);
    }

    public override void OnArcadiaLoad()
    {
        base.OnArcadiaLoad();
    }

    public override void PostArcadeRoomStart()
    {
        base.PostArcadeRoomStart();
    }

    public override void PostArcadeRoomComplete()
    {
        base.PostArcadeRoomComplete();
    }

    public override void PostArcadeGauntletEnemySpawned(Enemy enemy)
    {
        base.PostArcadeGauntletEnemySpawned(enemy);
    }

    public override void OnItemUse(ItemCodex.ItemTypes enItem, PlayerView xView, ref bool bSend)
    {
        base.OnItemUse(enItem, xView, ref bSend);
    }

    public override void PostSpellActivation(PlayerView xView, ISpellActivation xact, SpellCodex.SpellTypes enType, int iBoostState)
    {
        base.PostSpellActivation(xView, xact, enType, iBoostState);
    }

    public override void PostLevelLoad(Level.ZoneEnum level, Level.WorldRegion region, bool staticOnly)
    {
        base.PostLevelLoad(level, region, staticOnly);
    }

    public override void OnEnemySpawn(ref EnemyCodex.EnemyTypes enemy, ref Vector2 position, ref bool isElite, ref bool dropsLoot, ref int bitLayer, ref float virtualHeight, float[] behaviourVariables)
    {
        base.OnEnemySpawn(ref enemy, ref position, ref isElite, ref dropsLoot, ref bitLayer, ref virtualHeight, behaviourVariables);
    }

    public override void PostEnemySpawn(Enemy entity, EnemyCodex.EnemyTypes enemy, EnemyCodex.EnemyTypes original, Vector2 position, bool isElite, bool dropsLoot, int bitLayer, float virtualHeight, float[] behaviourVariables)
    {
        base.PostEnemySpawn(entity, enemy, original, position, isElite, dropsLoot, bitLayer, virtualHeight, behaviourVariables);
    }

    public override void OnBaseStatsUpdate(BaseStats stats)
    {
        base.OnBaseStatsUpdate(stats);
    }

    public override void PostBaseStatsUpdate(BaseStats stats)
    {
        base.PostBaseStatsUpdate(stats);
    }
}
