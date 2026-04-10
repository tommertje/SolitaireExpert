using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

public static class CommandLineBuild
{
    private static string[] GetEnabledScenes()
    {
        var enabledScenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        if (enabledScenes.Length == 0)
        {
            throw new InvalidOperationException("No enabled scenes found in Build Settings.");
        }

        return enabledScenes;
    }

    public static void BuildWindows()
    {
        var enabledScenes = GetEnabledScenes();

        var outputDir = Path.GetFullPath("build/StandaloneWindows64");
        Directory.CreateDirectory(outputDir);
        var outputPath = Path.Combine(outputDir, "SolitaireExpert.exe");

        var options = new BuildPlayerOptions
        {
            scenes = enabledScenes,
            locationPathName = outputPath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new Exception($"Windows build failed: {report.summary.result}");
        }

        Console.WriteLine($"Windows build succeeded: {outputPath}");
        Console.WriteLine($"Build size: {report.summary.totalSize} bytes");
    }

    public static void BuildWebGL()
    {
        var enabledScenes = GetEnabledScenes();

        var outputDir = Path.GetFullPath("build/WebGL");
        Directory.CreateDirectory(outputDir);

        var options = new BuildPlayerOptions
        {
            scenes = enabledScenes,
            locationPathName = outputDir,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new Exception($"WebGL build failed: {report.summary.result}");
        }

        Console.WriteLine($"WebGL build succeeded: {outputDir}");
        Console.WriteLine($"Build size: {report.summary.totalSize} bytes");
    }
}
