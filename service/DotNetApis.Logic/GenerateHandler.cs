﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DotNetApis.Cecil;
using DotNetApis.Common;
using DotNetApis.Logic.Assemblies;
using DotNetApis.Logic.Formatting;
using DotNetApis.Logic.Messages;
using DotNetApis.Nuget;
using DotNetApis.Storage;
using DotNetApis.Structure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DotNetApis.Logic
{
    public sealed class GenerateHandler
    {
        private readonly ILogger _logger;
        private readonly InMemoryLogger _inMemoryLogger;
        private readonly LogCombinedStorage _logStorage;
        private readonly PackageDownloader _packageDownloader;
        private readonly PlatformResolver _platformResolver;
        private readonly NugetPackageDependencyResolver _dependencyResolver;
        private readonly ReferenceAssemblies _referenceAssemblies;
        private readonly IReferenceStorage _referenceStorage;
        private readonly MemberDefinitionFormatter _memberDefinitionFormatter;
        private readonly AttributeFormatter _attributeFormatter;

        public GenerateHandler(ILogger logger, InMemoryLogger inMemoryLogger, LogCombinedStorage logStorage, PackageDownloader packageDownloader, PlatformResolver platformResolver,
            NugetPackageDependencyResolver dependencyResolver, ReferenceAssemblies referenceAssemblies, IReferenceStorage referenceStorage, MemberDefinitionFormatter memberDefinitionFormatter,
            AttributeFormatter attributeFormatter)
        {
            _logger = logger;
            _logStorage = logStorage;
            _packageDownloader = packageDownloader;
            _platformResolver = platformResolver;
            _dependencyResolver = dependencyResolver;
            _referenceAssemblies = referenceAssemblies;
            _referenceStorage = referenceStorage;
            _memberDefinitionFormatter = memberDefinitionFormatter;
            _attributeFormatter = attributeFormatter;
            _inMemoryLogger = inMemoryLogger;
        }
        
        public async Task HandleAsync(GenerateRequestMessage message)
        {
            var idver = NugetPackageIdVersion.TryCreate(message.NormalizedPackageId, NugetVersion.TryParse(message.NormalizedPackageVersion));
            var target = PlatformTarget.TryParse(message.NormalizedFrameworkTarget);
            if (idver == null || target == null)
                throw new InvalidOperationException("Invalid generation request");
            try
            {
                var json = await HandleAsync(idver, target).ConfigureAwait(false); // TODO: handle result
                Debugger.Break();
                await _logStorage.WriteAsync(idver, target, message.Timestamp, Status.Succeeded, string.Join("\n", _inMemoryLogger?.Messages ?? new List<string>())).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(0, ex, "Error handling message {message}", JsonConvert.SerializeObject(message, Constants.JsonSerializerSettings));
                await _logStorage.WriteAsync(idver, target, message.Timestamp, Status.Failed, string.Join("\n", _inMemoryLogger?.Messages ?? new List<string>())).ConfigureAwait(false);
            }
        }

        private async Task<PackageJson> HandleAsync(NugetPackageIdVersion idver, PlatformTarget target)
        {
            // Load the package.
            var publishedPackage = await _packageDownloader.GetPackageAsync(idver).ConfigureAwait(false);
            var currentPackage = publishedPackage.Package;

            // Create the assembly collection for this request.
            var assemblies = new AssemblyCollection(_logger, currentPackage);

            // Determine all supported targets for the package.
            var allTargets = await _platformResolver.AllSupportedPlatformsAsync(currentPackage).ConfigureAwait(false);

            // Add assemblies for the current package to our context.
            foreach (var path in publishedPackage.Package.GetCompatibleAssemblyReferences(target))
                assemblies.AddCurrentPackageAssembly(path);
            _logger.LogDebug("Documentation will be generated for {assemblies}", assemblies.CurrentPackageAssemblies.Dump());

            // Add assemblies for all dependency packages to our context.
            var dependencyPackages = await _dependencyResolver.ResolveAsync(publishedPackage.Package, target).ConfigureAwait(false);
            var dependencyAssemblyCount = 0;
            foreach (var dependentPackage in dependencyPackages)
            {
                foreach (var path in dependentPackage.GetCompatibleAssemblyReferences(target))
                {
                    ++dependencyAssemblyCount;
                    assemblies.AddDependencyPackageAssembly(dependentPackage, path);
                }
            }
            _logger.LogDebug("Added {assemblyCount} assemblies from {packageCount} dependency packages", dependencyAssemblyCount, dependencyPackages.Count);

            // Sanity check: we'd better have something to generate documentation on.
            if (assemblies.CurrentPackageAssemblies.Count == 0 && assemblies.DependencyPackageAssemblies.Count == 0)
                throw new ExpectedException(HttpStatusCode.BadRequest, $"Neither package {idver} nor its dependencies have any assemblies for target {target}");

            // Add all target framework reference dlls to our context.
            var referenceTarget = _referenceAssemblies.ReferenceTargets.FirstOrDefault(x => NugetUtility.IsCompatible(x.Target.FrameworkName, target.FrameworkName));
            if (referenceTarget != null)
            {
                foreach (var path in referenceTarget.Paths.Where(x => x.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase)))
                    assemblies.AddReferenceAssembly(path, () => _referenceStorage.Download(path));
            }

            using (GenerationScope.Create(target, assemblies))
            {
                // Produce JSON structure for all package dependencies.
                var immediateDependencies = _platformResolver.GetCompatiblePackageDependencies(publishedPackage.Package, target);
                var dependencies = dependencyPackages.Select(x =>
                {
                    immediateDependencies.TryGetValue(x.Metadata.PackageId, out var immediateDependency);
                    return new PackageDependencyJson
                    {
                        Title = x.Metadata.Title,
                        PackageId = x.Metadata.PackageId,
                        VersionRange = immediateDependency?.VersionRange?.ToString(),
                        Version = x.Metadata.Version.ToString(),
                        Summary = x.Metadata.Summary,
                        Authors = x.Metadata.Authors,
                        IconUrl = x.Metadata.IconUrl,
                        ProjectUrl = x.Metadata.ProjectUrl,
                    };
                }).OrderBy(x => x.VersionRange == null).ThenBy(x => x.PackageId, StringComparer.InvariantCultureIgnoreCase).ToList();

                // Produce JSON structure for the primary package.
                return new PackageJson
                {
                    PackageId = publishedPackage.Package.Metadata.PackageId,
                    Version = publishedPackage.Package.Metadata.Version.ToString(),
                    Target = target.ToString(),
                    Description = publishedPackage.Package.Metadata.Description,
                    Authors = publishedPackage.Package.Metadata.Authors,
                    IconUrl = publishedPackage.Package.Metadata.IconUrl,
                    ProjectUrl = publishedPackage.Package.Metadata.ProjectUrl,
                    SupportedTargets = allTargets.Select(x => x.ToString()).ToList(),
                    Dependencies = dependencies,
                    Published = publishedPackage.ExternalMetadata.Published,
                    IsReleaseVersion = publishedPackage.Package.Metadata.Version.IsReleaseVersion,
                    Assemblies = assemblies.CurrentPackageAssemblies.Where(x => x.AssemblyDefinition != null).Select(GenerateJsonForAssembly).ToList(),
                };
            }
        }

        private AssemblyJson GenerateJsonForAssembly(CurrentPackageAssembly assembly)
        {
            using (AssemblyScope.Create(assembly.Xmldoc))
            {
                return new AssemblyJson
                {
                    FullName = assembly.AssemblyDefinition.FullName,
                    Path = assembly.Path,
                    FileLength = assembly.FileLength,
                    Attributes = _attributeFormatter.Attributes(assembly.AssemblyDefinition, "assembly").ToList(),
                    Types = assembly.AssemblyDefinition.Modules.SelectMany(x => x.Types).Where(x => x.IsExposed()).Select(x => _memberDefinitionFormatter.MemberDefinition(x, assembly.Xmldoc)).ToList(),
                };
            }
        }
    }
}
