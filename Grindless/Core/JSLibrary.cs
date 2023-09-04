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

    public static void LoadSoGEnums(Engine engine)
    {
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
                        message += JSON.stringify(arg) + ' ';
                }
                __writeLine(message);
            };
            Object.freeze(console);
            """);
    }
}
