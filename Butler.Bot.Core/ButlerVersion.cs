using System.Reflection;

namespace Butler.Bot.Core;

class ButlerVersion
{
    public static Version GetCurrent()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        return assembly.GetName().Version!;
    }
}
