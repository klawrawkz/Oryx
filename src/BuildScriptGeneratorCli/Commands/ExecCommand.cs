﻿// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

using System;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Oryx.BuildScriptGenerator;
using Microsoft.Oryx.Common;
using Microsoft.Oryx.Common.Extensions;

namespace Microsoft.Oryx.BuildScriptGeneratorCli
{
    [Command(Name, Description = "Execute an arbitrary command in the default shell, in an environment " +
        "with the best-matching platform tool versions.")]
    internal class ExecCommand : CommandBase
    {
        public const string Name = "exec";

        [Option("-s|--src <dir>", CommandOptionType.SingleValue, Description = "Source directory.")]
        [DirectoryExists]
        public string SourceDir { get; set; }

        [Argument(1, Description = "The command to execute in an app-specific environment.")]
        public string Command { get; set; }

        internal override int Execute(IServiceProvider serviceProvider, IConsole console)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<ExecCommand>>();
            var env = serviceProvider.GetRequiredService<IEnvironment>();
            var generator = serviceProvider.GetRequiredService<IBuildScriptGenerator>();
            var opts = serviceProvider.GetRequiredService<IOptions<BuildScriptGeneratorOptions>>().Value;

            if (string.IsNullOrWhiteSpace(Command))
            {
                logger.LogDebug("Command is empty; exiting");
                return ProcessConstants.ExitSuccess;
            }

            var shellPath = env.GetEnvironmentVariable("BASH") ?? FilePaths.Bash;
            var ctx = BuildScriptGenerator.CreateContext(serviceProvider, operationId: null);
            ctx.DisableMultiPlatformBuild = false;
            var tools = generator.GetRequiredToolVersions(ctx);

            int exitCode;
            using (var timedEvent = logger.LogTimedEvent("ExecCommand"))
            {
                // Build envelope script
                var scriptBuilder = new ShellScriptBuilder("\n")
                    .AddShebang(shellPath)
                    .AddCommand("set -e");

                if (tools.Count > 0)
                {
                    scriptBuilder.Source($"{FilePaths.Benv} {StringExtensions.JoinKeyValuePairs(tools)}");
                }

                var script = scriptBuilder.AddCommand(Command).ToString();

                // Create temporary file to store script
                var tempScriptPath = Path.GetTempFileName();
                File.WriteAllText(tempScriptPath, script);
                timedEvent.AddProperty(nameof(tempScriptPath), tempScriptPath);

                if (DebugMode)
                {
                    console.WriteLine($"Temporary script @ {tempScriptPath}:");
                    console.WriteLine("---");
                    console.WriteLine(script);
                    console.WriteLine("---");
                }

                exitCode = ProcessHelper.RunProcess(
                    shellPath,
                    new[] { tempScriptPath },
                    opts.SourceDir,
                    (sender, args) => { if (args.Data != null) console.WriteLine(args.Data); },
                    (sender, args) => { if (args.Data != null) console.Error.WriteLine(args.Data); },
                    waitTimeForExit: null);
                timedEvent.AddProperty("exitCode", exitCode.ToString());
            }

            return exitCode;
        }

        internal override void ConfigureBuildScriptGeneratorOptions(BuildScriptGeneratorOptions options)
        {
            BuildScriptGeneratorOptionsHelper.ConfigureBuildScriptGeneratorOptions(
                options,
                SourceDir,
                null,
                null,
                null,
                null,
                null,
                shouldPackage: false,
                requiredOsPackages: null,
                scriptOnly: false,
                enableDynamicInstall: false,
                null);
        }
    }
}
