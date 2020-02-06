﻿// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Oryx.BuildScriptGenerator.Python;
using Microsoft.Oryx.Common;
using Microsoft.Oryx.Tests.Common;
using Xunit;

namespace Microsoft.Oryx.BuildScriptGenerator.Tests.Python
{
    public class PythonPlatformTests
    {
        [Fact]
        public void GeneratedSnippet_DoesNotHaveInstallScript_IfDynamicInstallIsDisabled_AndUseLatestVersionIsTrue()
        {
            // Arrange
            var options = new BuildScriptGeneratorOptions() { EnableDynamicInstall = false };
            var environment = new TestEnvironment();
            environment.Variables[SdkStorageConstants.UseLatestVersion] = "true";
            var installerScriptSnippet = "##INSTALLER_SCRIPT##";
            var versionProvider = new TestPythonVersionProvider(new[] { "3.7.5", "3.8.0" }, defaultVersion: "3.7.5");
            var platformInstaller = new TestPythonPlatformInstaller(
                isVersionAlreadyInstalled: false,
                installerScript: installerScriptSnippet,
                Options.Create(options),
                environment);
            var platform = CreatePlatform(environment, versionProvider, platformInstaller, options);
            var repo = new MemorySourceRepo();
            repo.AddFile("", PythonConstants.RequirementsFileName);
            repo.AddFile("print(1)", "bla.py");
            var context = new BuildScriptGeneratorContext { SourceRepo = repo, PythonVersion = "3.7.5" };

            // Act
            var snippet = platform.GenerateBashBuildScriptSnippet(context);

            // Assert
            Assert.NotNull(snippet);
            Assert.Null(snippet.InstallationScriptSnippet);
            Assert.Contains(ManifestFilePropertyKeys.PythonVersion, snippet.BuildProperties.Keys);
            Assert.Equal("3.7.5", snippet.BuildProperties[ManifestFilePropertyKeys.PythonVersion]);
        }

        [Fact]
        public void GeneratedSnippet_DoesNotHaveInstallScript_IfDynamicInstallIsEnabled_AndUseLatestVersionIsFalse()
        {
            // Arrange
            var options = new BuildScriptGeneratorOptions() { EnableDynamicInstall = false };
            var environment = new TestEnvironment();
            var installerScriptSnippet = "##INSTALLER_SCRIPT##";
            var versionProvider = new TestPythonVersionProvider(new[] { "3.7.5", "3.8.0" }, defaultVersion: "3.7.5");
            var platformInstaller = new TestPythonPlatformInstaller(
                isVersionAlreadyInstalled: false,
                installerScript: installerScriptSnippet,
                Options.Create(options),
                environment);
            var platform = CreatePlatform(environment, versionProvider, platformInstaller, options);
            var repo = new MemorySourceRepo();
            repo.AddFile("", PythonConstants.RequirementsFileName);
            repo.AddFile("print(1)", "bla.py");
            var context = new BuildScriptGeneratorContext { SourceRepo = repo, PythonVersion = "3.7.5" };

            // Act
            var snippet = platform.GenerateBashBuildScriptSnippet(context);

            // Assert
            Assert.NotNull(snippet);
            Assert.Null(snippet.InstallationScriptSnippet);
            Assert.Contains(ManifestFilePropertyKeys.PythonVersion, snippet.BuildProperties.Keys);
            Assert.Equal("3.7.5", snippet.BuildProperties[ManifestFilePropertyKeys.PythonVersion]);
        }

        [Fact]
        public void GeneratedSnippet_HasInstallationScript_IfDynamicInstallIsEnabled_AndLatestVersionIsTrue()
        {
            // Arrange
            var options = new BuildScriptGeneratorOptions() { EnableDynamicInstall = true };
            var environment = new TestEnvironment();
            environment.Variables[SdkStorageConstants.UseLatestVersion] = "true";
            var installerScriptSnippet = "##INSTALLER_SCRIPT##";
            var versionProvider = new TestPythonVersionProvider(new[] { "3.7.5", "3.8.0" }, defaultVersion: "3.7.5");
            var platformInstaller = new TestPythonPlatformInstaller(
                isVersionAlreadyInstalled: false,
                installerScript: installerScriptSnippet,
                Options.Create(options),
                environment);
            var platform = CreatePlatform(environment, versionProvider, platformInstaller, options);
            var repo = new MemorySourceRepo();
            repo.AddFile("", PythonConstants.RequirementsFileName);
            repo.AddFile("print(1)", "bla.py");
            var context = new BuildScriptGeneratorContext { SourceRepo = repo, PythonVersion = "3.7.5" };

            // Act
            var snippet = platform.GenerateBashBuildScriptSnippet(context);

            // Assert
            Assert.NotNull(snippet);
            Assert.NotNull(snippet.InstallationScriptSnippet);
            Assert.Equal(installerScriptSnippet, snippet.InstallationScriptSnippet);
            Assert.Contains(ManifestFilePropertyKeys.PythonVersion, snippet.BuildProperties.Keys);
            Assert.Equal("3.7.5", snippet.BuildProperties[ManifestFilePropertyKeys.PythonVersion]);
        }

        [Fact]
        public void GeneratedSnippet_DoesNotHaveInstallScript_IfVersionIsAlreadyPresentOnDisk()
        {
            // Arrange
            var options = new BuildScriptGeneratorOptions() { EnableDynamicInstall = true };
            var environment = new TestEnvironment();
            environment.Variables[SdkStorageConstants.UseLatestVersion] = "true";
            var installerScriptSnippet = "##INSTALLER_SCRIPT##";
            var versionProvider = new TestPythonVersionProvider(new[] { "3.7.5", "3.8.0" }, defaultVersion: "3.7.5");
            var platformInstaller = new TestPythonPlatformInstaller(
                isVersionAlreadyInstalled: true,
                installerScript: installerScriptSnippet,
                Options.Create(options),
                environment);
            var platform = CreatePlatform(environment, versionProvider, platformInstaller, options);
            var repo = new MemorySourceRepo();
            repo.AddFile("", PythonConstants.RequirementsFileName);
            repo.AddFile("print(1)", "bla.py");
            var context = new BuildScriptGeneratorContext { SourceRepo = repo, PythonVersion = "3.7.5" };

            // Act
            var snippet = platform.GenerateBashBuildScriptSnippet(context);

            // Assert
            Assert.NotNull(snippet);
            Assert.Null(snippet.InstallationScriptSnippet);
            Assert.Contains(ManifestFilePropertyKeys.PythonVersion, snippet.BuildProperties.Keys);
            Assert.Equal("3.7.5", snippet.BuildProperties[ManifestFilePropertyKeys.PythonVersion]);
        }

        [Fact]
        public void GeneratedScript_DoesNotUseVenv()
        {
            // Arrange
            var scriptGenerator = CreatePlatform();
            var repo = new MemorySourceRepo();
            repo.AddFile("", PythonConstants.RequirementsFileName);
            repo.AddFile("print(1)", "bla.py");
            var context = new BuildScriptGeneratorContext { SourceRepo = repo };

            // Act
            var snippet = scriptGenerator.GenerateBashBuildScriptSnippet(context);

            // Assert
            Assert.NotNull(snippet);
            Assert.Contains("Python Virtual Environment", snippet.BashBuildScriptSnippet);
            Assert.True(scriptGenerator.IsCleanRepo(repo));
        }

        [Theory]
        [InlineData(null, "bla.tar.gz")]
        [InlineData("tar-gz", "bla.tar.gz")]
        [InlineData("zip", "bla.zip")]
        public void ExlcudedDirs_DoesNotContainVirtualEnvDir_IfCompressVirtualEnv_IsEnabled(
            string compressOption,
            string compressedVirtualEnvFileName)
        {
            // Arrange
            var scriptGenerator = CreatePlatform();
            var repo = new MemorySourceRepo();
            repo.AddFile("", PythonConstants.RequirementsFileName);
            var venvName = "bla";
            var context = new BuildScriptGeneratorContext
            {
                SourceRepo = repo,
                Properties = new Dictionary<string, string> {
                    { "virtualenv_name", venvName },
                    { "compress_virtualenv", compressOption }
                }
            };

            // Act
            var excludedDirs = scriptGenerator.GetDirectoriesToExcludeFromCopyToBuildOutputDir(context);

            // Assert
            Assert.NotNull(excludedDirs);
            Assert.Contains(venvName, excludedDirs);
            Assert.DoesNotContain(compressedVirtualEnvFileName, excludedDirs);
        }

        private PythonPlatform CreatePlatform(
            IEnvironment environment,
            IPythonVersionProvider pythonVersionProvider,
            PythonPlatformInstaller platformInstaller,
            BuildScriptGeneratorOptions options)
        {
            var commonOptions = Options.Create(options);

            return new PythonPlatform(
                commonOptions,
                pythonVersionProvider,
                environment,
                NullLogger<PythonPlatform>.Instance,
                detector: null,
                platformInstaller);
        }

        private PythonPlatform CreatePlatform(string defaultVersion = null)
        {
            var testEnv = new TestEnvironment();
            var versionProvider = new TestPythonVersionProvider(
                supportedVersions: new[] { Common.PythonVersions.Python37Version },
                defaultVersion: defaultVersion);
            var commonOptions = Options.Create(new BuildScriptGeneratorOptions());

            return new PythonPlatform(
                commonOptions,
                versionProvider,
                testEnv,
                NullLogger<PythonPlatform>.Instance,
                detector: null,
                new PythonPlatformInstaller(commonOptions, testEnv));
        }

        private class TestPythonVersionProvider : IPythonVersionProvider
        {
            private readonly IEnumerable<string> _supportedVersions;
            private readonly string _defaultVersion;

            public TestPythonVersionProvider(IEnumerable<string> supportedVersions, string defaultVersion)
            {
                _supportedVersions = supportedVersions;
                _defaultVersion = defaultVersion;
            }

            public PlatformVersionInfo GetVersionInfo()
            {
                return PlatformVersionInfo.CreateOnDiskVersionInfo(_supportedVersions, _defaultVersion);
            }
        }

        private class TestPythonPlatformInstaller : PythonPlatformInstaller
        {
            private readonly bool _isVersionAlreadyInstalled;
            private readonly string _installerScript;

            public TestPythonPlatformInstaller(
                bool isVersionAlreadyInstalled,
                string installerScript,
                IOptions<BuildScriptGeneratorOptions> commonOptions,
                IEnvironment environment)
                : base(commonOptions, environment)
            {
                _isVersionAlreadyInstalled = isVersionAlreadyInstalled;
                _installerScript = installerScript;
            }

            public override bool IsVersionAlreadyInstalled(string version)
            {
                return _isVersionAlreadyInstalled;
            }

            public override string GetInstallerScriptSnippet(string version)
            {
                return _installerScript;
            }
        }
    }
}