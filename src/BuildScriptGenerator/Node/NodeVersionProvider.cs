﻿// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace Microsoft.Oryx.BuildScriptGenerator.Node
{
    internal class NodeVersionProvider : INodeVersionProvider
    {
        private readonly NodeScriptGeneratorOptions _options;
        private IEnumerable<string> _supportedNodeVersions;
        private IEnumerable<string> _supportedNpmVersions;

        public NodeVersionProvider(IOptions<NodeScriptGeneratorOptions> options)
        {
            _options = options.Value;
        }

        public IEnumerable<string> SupportedNodeVersions
        {
            get
            {
                if (_supportedNodeVersions == null)
                {
                    _supportedNodeVersions = VersionProviderHelper.GetSupportedVersions(
                        _options.SupportedNodeVersions,
                        _options.InstalledNodeVersionsDir);
                }

                return _supportedNodeVersions;
            }
        }

        public IEnumerable<string> SupportedNpmVersions
        {
            get
            {
                if (_supportedNpmVersions == null)
                {
                    _supportedNpmVersions = VersionProviderHelper.GetSupportedVersions(
                        _options.SupportedNpmVersions,
                        _options.InstalledNpmVersionsDir);
                }

                return _supportedNpmVersions;
            }
        }
    }
}