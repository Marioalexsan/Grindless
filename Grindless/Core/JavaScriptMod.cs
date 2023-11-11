using Jint;
using Jint.Native;
using Jint.Native.Object;
using Jint.Runtime.Interop;
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
            options.Interop.AllowGetType = true;
            options.SetTypeResolver(new TypeResolver
            {
                MemberFilter = member =>
                {
                    if (typeof(Type).IsAssignableFrom(member.DeclaringType))
                    {
                        if (member.Name == nameof(Type.Name))
                            return true;

                        if (member.Name == nameof(Type.FullName))
                            return true;

                        return false;
                    }

                    return true;
                }
            });
        });

        JSLibrary.LoadSoGTypes(_engine);
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

        ListedDependencies = ((IDictionary<string, object>)GetJSProperty("dependencies").ToObject())
            .ToDictionary(x => x.Key, x => (string)x.Value);
    }

    internal Dictionary<string, string> ListedDependencies { get; } = new();

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

    private readonly Dictionary<string, JsValue> _propertyCache = new();

    private JsValue GetJSProperty([CallerMemberName] string name = null)
    {
        if (_propertyCache.TryGetValue(name, out var value))
            return value;

        var method = _mod.GetProperty(name).Value;

        if (method == null || method.IsUndefined() || method.IsNull())
        {
            _propertyCache[name] = null;
            Program.Logger.LogInformation($"No property {name} found for mod.");
            return null;
        }

        _propertyCache[name] = method;
        return method;
    }

    private JsValue Wrap(object obj)
    {
        return JsValue.FromObject(_engine, obj);
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
            GetJSProperty()?.Call(_jsThis);
        });
    }

    public override void Unload()
    {
        WrapCall(() =>
        {
            GetJSProperty()?.Call(_jsThis);
        });
    }

    public override void PostLoad()
    {
        WrapCall(() =>
        {
            GetJSProperty()?.Call(_jsThis);
        });
    }

    public override void OnEntityDamage(OnEntityDamageData data)
    {
        WrapCall(() =>
        {
            GetJSProperty()?.Call(_jsThis, Wrap(data));
        });
    }

    public override void PostEntityDamage(PostEntityDamageData data)
    {
        WrapCall(() =>
        {
            GetJSProperty()?.Call(_jsThis, Wrap(data));
        });
    }

    public override void PostLevelLoad(PostLevelLoadData data)
    {
        WrapCall(() =>
        {
            GetJSProperty()?.Call(_jsThis, Wrap(data));
        });
    }

    public override void OnEntityDeath(OnEntityDeathData data)
    {
        WrapCall(() =>
        {
            GetJSProperty()?.Call(_jsThis, Wrap(data));
        });
    }

    public override void PostRenderTopGUI(PostRenderTopGUIData data)
    {
        WrapCall(() =>
        {
            GetJSProperty()?.Call(_jsThis, Wrap(data));
        });
    }
}
