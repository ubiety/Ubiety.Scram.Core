// This is free and unencumbered software released into the public domain.
//
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
//
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// For more information, please refer to <http://unlicense.org/>

using System.Collections.Generic;
using System.Linq;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.GitHubActions.Configuration;
using Nuke.Common.Execution;
using Nuke.Common.Utilities;

namespace _build;

/// <summary>
///     Generates the workflow like <see cref="GitHubActionsAttribute" /> does, plus a NuGet
///     trusted publishing login step whose short-lived key is handed to the build invocation.
/// </summary>
/// <remarks>
///     The attribute has no hook for arbitrary steps, so the login has to be injected into the
///     generated job. Keeping it here means the workflow survives regeneration.
/// </remarks>
class CustomGitHubActionsAttribute(string name, GitHubActionsImage image, params GitHubActionsImage[] images)
    : GitHubActionsAttribute(name, image, images)
{
    protected override GitHubActionsJob GetJobs(
        GitHubActionsImage image,
        IReadOnlyCollection<ExecutableTarget> relevantTargets)
    {
        var job = base.GetJobs(image, relevantTargets);

        // The key is only valid for an hour, so login goes as late as possible - directly ahead
        // of the invocation that pushes.
        var steps = job.Steps.ToList();
        var index = steps.FindIndex(x => x is GitHubActionsRunStep);
        steps.Insert(index >= 0 ? index : steps.Count, new NuGetLoginStep());
        job.Steps = steps.ToArray();

        return job;
    }

    protected override IEnumerable<(string, string)> GetImports()
    {
        return base.GetImports().Concat([("NUGETKEY", $"${{{{ steps.{NuGetLoginStep.StepId}.outputs.NUGET_API_KEY }}}}")]);
    }
}

/// <summary>
///     Exchanges the job's OIDC token for a short-lived nuget.org API key.
/// </summary>
class NuGetLoginStep : GitHubActionsStep
{
    public const string StepId = "nuget-login";

    public override void Write(CustomFileWriter writer)
    {
        writer.WriteLine("- name: 'NuGet Login'");

        using (writer.Indent())
        {
            writer.WriteLine("uses: NuGet/login@v1");
            writer.WriteLine($"id: {StepId}");
            writer.WriteLine("with:");

            using (writer.Indent())
            {
                writer.WriteLine("user: ${{ secrets.NUGET_USERNAME }}");
            }
        }
    }
}
