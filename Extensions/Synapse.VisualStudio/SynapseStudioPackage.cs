using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace Synapse.VisualStudio;

[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[ProvideAutoLoad(SynapseStudioPackage.PackageGuidString, PackageAutoLoadFlags.BackgroundLoad)]
[ProvideUIContextRule(
    SynapseStudioPackage.PackageGuidString,
    name: "Auto Load For CSharp",
    expression: "CSharp",
    termNames: ["CSharp"],
    termValues: ["SolutionHasProjectCapability:CSharp"])]
[Guid(SynapseStudioPackage.PackageGuidString)]
public sealed class SynapseStudioPackage : AsyncPackage
{
   public const string PackageGuidString = "a95da6ce-0d62-4578-904a-2949618bb070";

   /// <inheritdoc/>
   protected override async Task InitializeAsync(CancellationToken token, IProgress<ServiceProgressData> progress)
   {
      await JoinableTaskFactory.SwitchToMainThreadAsync(token);
   }
}
