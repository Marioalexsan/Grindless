using Jint;
using Jint.Native;
using Microsoft.Extensions.Logging;

namespace Grindless;

internal static class JSLibrary
{
    public static void LoadConsoleAPI(Engine engine, Func<ILogger> loggerSource)
    {
        engine.SetValue("__writeLine", new Action<string>((str) => loggerSource().LogInformation(str)));
        engine.Execute(
            """
            const console = {};
            console.log = (...args) => {
                let message = '';
                for (const arg of args) {
                    if (typeof arg !== 'string')
                        message += JSON.stringify(arg) + ' ';
                    else
                        message += arg + ' ';
                }
                __writeLine(message);
            };
            Object.freeze(console);
            """);
    }
}
