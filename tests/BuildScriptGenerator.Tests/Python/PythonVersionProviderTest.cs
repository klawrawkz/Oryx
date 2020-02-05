// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

using System.Net.Http;
using Microsoft.Extensions.Options;
using Microsoft.Oryx.BuildScriptGenerator.Python;
using Microsoft.Oryx.Common;
using Microsoft.Oryx.Tests.Common;
using Xunit;

namespace Microsoft.Oryx.BuildScriptGenerator.Tests.Python
{
    public class PythonVersionProviderTest
    {
        [Fact]
        public void GetsVersions_FromStorage_WhenDynamicInstall_IsTrue_AndUseLatestVersionIsTrue()
        {
            // Arrange
            var (versionProvider, onDiskVersionProvider, storageVersionProvider) = CreateVersionProvider(
                enableDynamicInstall: true, useLatestVersion: true);

            // Act
            var versionInfo = versionProvider.GetVersionInfo();

            // Assert
            Assert.True(storageVersionProvider.GetVersionInfoCalled);
            Assert.False(onDiskVersionProvider.GetVersionInfoCalled);
        }

        [Fact]
        public void GetsVersions_DoesNotGetVersionsFromStorage_WhenDynamicInstall_IsFalse()
        {
            // Arrange
            var (versionProvider, onDiskVersionProvider, storageVersionProvider) = CreateVersionProvider(
                enableDynamicInstall: false, useLatestVersion: true);

            // Act
            var versionInfo = versionProvider.GetVersionInfo();

            // Assert
            Assert.False(storageVersionProvider.GetVersionInfoCalled);
            Assert.True(onDiskVersionProvider.GetVersionInfoCalled);
        }

        [Fact]
        public void GetsVersions_DoesNotGetVersionsFromStorage_WhenDynamicInstall_IsTrue_AndUseLatestVersion_IsFalse()
        {
            // Arrange
            var (versionProvider, onDiskVersionProvider, storageVersionProvider) = CreateVersionProvider(
                enableDynamicInstall: true, useLatestVersion: false);

            // Act
            var versionInfo = versionProvider.GetVersionInfo();

            // Assert
            Assert.False(storageVersionProvider.GetVersionInfoCalled);
            Assert.True(onDiskVersionProvider.GetVersionInfoCalled);
        }

        [Fact]
        public void GetsVersions_DoesNotGetVersionsFromStorage_ByDefault()
        {
            // Arrange
            var (versionProvider, onDiskVersionProvider, storageVersionProvider) = CreateVersionProvider(
                enableDynamicInstall: false, useLatestVersion: false);

            // Act
            var versionInfo = versionProvider.GetVersionInfo();

            // Assert
            Assert.False(storageVersionProvider.GetVersionInfoCalled);
            Assert.True(onDiskVersionProvider.GetVersionInfoCalled);
        }

        private class TestPythonSdkStorageVersionProvider : PythonSdkStorageVersionProvider
        {
            public TestPythonSdkStorageVersionProvider(
                IEnvironment environment, IHttpClientFactory httpClientFactory)
                : base(environment, httpClientFactory)
            {
            }

            public bool GetVersionInfoCalled { get; private set; }

            public override PlatformVersionInfo GetVersionInfo()
            {
                GetVersionInfoCalled = true;

                return null;
            }
        }

        private (IPythonVersionProvider, TestPythonOnDiskVersionProvider, TestPythonSdkStorageVersionProvider)
            CreateVersionProvider(bool enableDynamicInstall, bool useLatestVersion)
        {
            var commonOptions = Options.Create(new BuildScriptGeneratorOptions()
            {
                EnableDynamicInstall = enableDynamicInstall
            });
            var pythonOptions = Options.Create(new PythonScriptGeneratorOptions());
            var environment = new TestEnvironment();
            environment.Variables[SdkStorageConstants.UseLatestVersion] = useLatestVersion.ToString();

            var onDiskProvider = new TestPythonOnDiskVersionProvider(pythonOptions);
            var storageProvider = new TestPythonSdkStorageVersionProvider(environment, new TestHttpClientFactory());
            var versionProvider = new PythonVersionProvider(
                commonOptions,
                environment,
                onDiskProvider,
                storageProvider);
            return (versionProvider, onDiskProvider, storageProvider);
        }


        private class TestPythonOnDiskVersionProvider : PythonOnDiskVersionProvider
        {
            public TestPythonOnDiskVersionProvider(IOptions<PythonScriptGeneratorOptions> options) : base(options)
            {
            }

            public bool GetVersionInfoCalled { get; private set; }

            public override PlatformVersionInfo GetVersionInfo()
            {
                GetVersionInfoCalled = true;

                return null;
            }
        }
    }
}
