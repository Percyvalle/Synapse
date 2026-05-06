using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace Synapse.VisualStudio;

/// <summary>
/// The main entry point for the Synapse Studio Visual Studio extension.
/// </summary>
[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[ProvideAutoLoad(SynapseStudioPackage.CSharpUiContextGuidString, PackageAutoLoadFlags.BackgroundLoad)]
[ProvideUIContextRule(
    SynapseStudioPackage.CSharpUiContextGuidString,
    name: "Auto Load For CSharp",
    expression: "CSharp",
    termNames: ["CSharp"],
    termValues: ["SolutionHasProjectCapability:CSharp"])]
[Guid(SynapseStudioPackage.PackageGuidString)]
public sealed class SynapseStudioPackage : AsyncPackage
{
   /// <summary>
   /// SynapseStudioPackage GUID string.
   /// </summary>
   public const string PackageGuidString = "a95da6ce-0d62-4578-904a-2949618bb070";

   /// <summary>
   /// The UI context GUID used to auto-load the package for C# solutions.
   /// </summary>
   public const string CSharpUiContextGuidString = "9e6f2e6d-b8fe-492c-9d2d-0ea2bbce0fe1";

   /// <inheritdoc/>
   protected override async Task InitializeAsync(CancellationToken token, IProgress<ServiceProgressData> progress)
   {
      await JoinableTaskFactory.SwitchToMainThreadAsync(token);
   }
}
