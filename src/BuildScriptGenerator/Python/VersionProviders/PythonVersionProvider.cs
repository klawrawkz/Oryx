// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

using Microsoft.Extensions.Options;
using Microsoft.Oryx.Common;

namespace Microsoft.Oryx.BuildScriptGenerator.Python
{
    internal class PythonVersionProvider : IPythonVersionProvider
    {
        private readonly BuildScriptGeneratorOptions _options;
        private readonly IEnvironment _environment;
        private readonly PythonOnDiskVersionProvider _onDiskVersionProvider;
        private readonly PythonSdkStorageVersionProvider _sdkStorageVersionProvider;

        public PythonVersionProvider(
            IOptions<BuildScriptGeneratorOptions> options,
            IEnvironment environment,
            PythonOnDiskVersionProvider onDiskVersionProvider,
            PythonSdkStorageVersionProvider sdkStorageVersionProvider)
        {
            _options = options.Value;
            _environment = environment;
            _onDiskVersionProvider = onDiskVersionProvider;
            _sdkStorageVersionProvider = sdkStorageVersionProvider;
        }

        public PlatformVersionInfo GetVersionInfo()
        {
            if (_options.EnableDynamicInstall)
            {
                var useLatestVersion = _environment.GetBoolEnvironmentVariable(
                    SdkStorageConstants.UseLatestVersion);
                if (useLatestVersion.HasValue && useLatestVersion.Value)
                {
                    return _sdkStorageVersionProvider.GetVersionInfo();
                }
            }

            return _onDiskVersionProvider.GetVersionInfo();
        }
    }
}