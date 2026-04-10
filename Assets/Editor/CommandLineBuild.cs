using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

public static class CommandLineBuild
{
    public static void BuildWindows()
    {
        var enabledScenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        if (enabledScenes.Length == 0)
        {
            throw new InvalidOperationException("No enabled scenes found in Build Settings.");
        }

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
}
