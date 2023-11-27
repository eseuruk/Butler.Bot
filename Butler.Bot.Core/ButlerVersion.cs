using System.Reflection;

namespace Butler.Bot.Core;

public static class ButlerVersion
{
    public static Version GetCurrent()
    {
        // Semantic versining with three part version number. Revision component is not used for now.
        // Version is provided as a built parameter durig CI. Please see .github/worflows/DraftRelease.yml

        Assembly assembly = Assembly.GetExecutingAssembly();
        var assemblyVersion = assembly.GetName().Version!;

        return new Version(assemblyVersion.Major, assemblyVersion.Minor, assemblyVersion.Build);
    }
}
