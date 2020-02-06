﻿// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Oryx.BuildScriptGenerator.Python;

namespace Microsoft.Oryx.BuildScriptGenerator
{
    internal static class PythonScriptGeneratorServiceCollectionExtensions
    {
        public static IServiceCollection AddPythonScriptGeneratorServices(this IServiceCollection services)
        {
            services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILanguageDetector, PythonLanguageDetector>());
            services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IProgrammingPlatform, PythonPlatform>());
            services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IConfigureOptions<PythonScriptGeneratorOptions>, PythonScriptGeneratorOptionsSetup>());
            services.AddSingleton<IPythonVersionProvider, PythonVersionProvider>();
            services.AddScoped<PythonLanguageDetector>();
            services.AddScoped<PythonPlatformInstaller>();
            services.AddSingleton<PythonOnDiskVersionProvider>();
            services.AddSingleton<PythonSdkStorageVersionProvider>();
            return services;
        }
    }
}