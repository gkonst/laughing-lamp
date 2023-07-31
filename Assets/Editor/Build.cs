using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using static System.Environment;

namespace Editor
{
    public class Build
    {
        private const string BUILD_DIR = "Build";
        private const string MASTER_CLIENT = "MasterClient_qa";

        [MenuItem("Sunday/Build/Linux/Build Master Client")]
        public static void BuildLinuxMasterClient()
        {
            var targetDir = $"{BUILD_DIR}/Master";

            var buildOptions = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/SampleScene.unity" },
                locationPathName = $"{targetDir}/{MASTER_CLIENT}.x86_64",
                target = BuildTarget.StandaloneLinux64,
                subtarget = (int)StandaloneBuildSubtarget.Server
            };
            var buildSummary = BuildSupernova("Master Linux Client", targetDir, buildOptions);
            ReportSummary(buildSummary);
            ExitWithResult(buildSummary.result);
        }

        private static void Clean(string targetDir)
        {
            Debug.Log($"Cleaning build dir {targetDir}");
            Directory.CreateDirectory(BUILD_DIR);
            if (Directory.Exists(targetDir))
            {
                Directory.Delete(targetDir, true);
            }

            Directory.CreateDirectory(targetDir);
        }

        private static BuildSummary BuildSupernova(string buildName, string targetDir, BuildPlayerOptions buildOptions)
        {
            Clean(targetDir);
            var currentTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var currentTarget = EditorUserBuildSettings.activeBuildTarget;
            var currentSubTarget = EditorUserBuildSettings.standaloneBuildSubtarget;
            SwitchTarget(buildOptions.targetGroup, buildOptions.target, ToSubtarget(buildOptions.subtarget));
            Debug.Log($"Building {buildName}...");
            var report = BuildPipeline.BuildPlayer(buildOptions);
            Debug.Log($"{buildName} Build {report.summary.result}.");
            SwitchTarget(currentTargetGroup, currentTarget, currentSubTarget);
            return report.summary;
        }

        private static void SwitchTarget(BuildTargetGroup targetGroup, BuildTarget target, StandaloneBuildSubtarget subtarget)
        {
            EditorUserBuildSettings.standaloneBuildSubtarget = subtarget;
            EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, target);
        }

        private static StandaloneBuildSubtarget ToSubtarget(int subtarget) => subtarget switch
        {
            0 => StandaloneBuildSubtarget.Player,
            1 => StandaloneBuildSubtarget.Server,
            _ => StandaloneBuildSubtarget.NoSubtarget
        };
        
        private static void ReportSummary(BuildSummary summary)
        {
            Console.WriteLine(
                $"{Environment.NewLine}" +
                $"###########################{NewLine}" +
                $"#      Build results      #{NewLine}" +
                $"###########################{NewLine}" +
                $"{NewLine}" +
                $"Duration: {summary.totalTime.ToString()}{NewLine}" +
                $"Warnings: {summary.totalWarnings.ToString()}{NewLine}" +
                $"Errors: {summary.totalErrors.ToString()}{NewLine}" +
                $"Size: {summary.totalSize.ToString()} bytes{NewLine}" +
                $"{NewLine}"
            );
        }

        private static void ExitWithResult(BuildResult result)
        {
            switch (result)
            {
                case BuildResult.Succeeded:
                    Console.WriteLine("Build succeeded!");
                    EditorApplication.Exit(0);
                    break;
                case BuildResult.Failed:
                    Console.WriteLine("Build failed!");
                    EditorApplication.Exit(101);
                    break;
                case BuildResult.Cancelled:
                    Console.WriteLine("Build cancelled!");
                    EditorApplication.Exit(102);
                    break;
                case BuildResult.Unknown:
                default:
                    Console.WriteLine("Build result is unknown!");
                    EditorApplication.Exit(103);
                    break;
            }
        }
    }
}