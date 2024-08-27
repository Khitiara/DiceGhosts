using System.Runtime.CompilerServices;
using Argon;

namespace tests;

public static class Initializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.AddExtraSettings(s => s.DefaultValueHandling = DefaultValueHandling.Include);
        VerifierSettings.HashParameters();
    }
}