using Jint;
using Jint.Native;
using Jint.Runtime.Interop;
using Microsoft.Extensions.Logging;

namespace Grindless;

internal static class JSLibrary
{
    private static void ExportEnum<T>(Engine engine, string name)
        where T : struct, Enum
    {
        engine.SetValue(name, IDExtension.GetAllSoGIDs<T>().ToDictionary(x => x.ToString()));
    }

    public static void LoadSoGTypes(Engine engine)
    {
        // TODO Capture global variable and set placeholder to undefined

        engine.AddModule("grindless", builder =>
        {
            builder.ExportType(typeof(Globals));
        });

        ExportEnum<EnemyCodex.EnemyTypes>(engine, "__enemyTypes");
        engine.Execute(
            """
            const Enums = {
                EnemyTypes: __enemyTypes
            };
            Object.freeze(Enums);
            """);
    }

    public static void LoadConsoleAPI(Engine engine, Func<ILogger> loggerSource)
    {
        // https://gist.github.com/bennettmcelwee/06f0cadd6a41847f848b4bd2a351b6bc
        engine.Execute("""
            // Similar to JSON.stringify but limited to a specified depth (default 1)
            // The approach is to prune the object first, then just call JSON.stringify to do the formatting

            const __prune = (obj, depth = 1) => {
              if (Array.isArray(obj) && obj.length > 0) {
                return (depth === 0) ? ['???'] : obj.map(e => __prune(e, depth - 1))
              } else if (obj && typeof obj === 'object' && Object.keys(obj).length > 0) {
                return (depth === 0) ? {'???':''} : Object.keys(obj).reduce((acc, key) => ({ ...acc, [key]: __prune(obj[key], depth - 1)}), {})
              } else {
                return obj
              }
            }

            const __stringify = (obj, depth = 1, space) => JSON.stringify(__prune(obj, depth), null, space)
            """);

        // TODO Capture global variable and set placeholder to undefined
        // TODO Investigate crash related to printing certain .NET objects (see Globals.Game.xWeaponAssetLibrary)
        engine.SetValue("__writeLine", new Action<string>((str) => loggerSource().LogInformation(str)));
        engine.Execute(
            """
            const console = {};
            console.log = (...args) => {
                let message = '';
                for (const arg of args) {
                    if (typeof arg === 'string')
                        message += arg + ' ';
                    else if (typeof arg === 'function')
                        message += '<function> '
                    else if (typeof arg === 'symbol')
                        message += '<symbol> '
                    else if (typeof arg === 'undefined')
                        message += '<undefined> '
                    else 
                        message += __stringify(arg, 3, 2) + ' ';
                }
                __writeLine(message);
            };
            Object.freeze(console);
            """);
    }
}
