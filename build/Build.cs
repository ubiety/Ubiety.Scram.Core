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

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.AppVeyor;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.SonarScanner;
using Nuke.Common.Utilities;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.SonarScanner.SonarScannerTasks;

namespace _build;

// Pre-releases go to GitHub Packages, so this workflow needs the GitHub token and nothing from
// nuget.org. Declaring any permission drops the rest to none, hence contents for the checkout.
[GitHubActions("continuous",
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.MacOsLatest,
    GitHubActionsImage.UbuntuLatest,
    OnPushBranchesIgnore = [ReleaseBranchPrefix, MasterBranch],
    PublishArtifacts = false,
    InvokedTargets = [nameof(Test), nameof(Publish)],
    EnableGitHubToken = true,
    ReadPermissions = [GitHubActionsPermissions.Contents],
    WritePermissions = [GitHubActionsPermissions.Packages],
    FetchDepth = 0)]
// Pull requests test only. A pull_request event checks out the detached merge ref, so there is no
// branch for GitRepository to resolve, Beta is false whatever the source branch is called, and
// Publish would demand a nuget.org key this workflow has no reason to hold. Keeping Publish out
// of the pull request build lets it keep a hard Requires for the release path.
[GitHubActions("pr",
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.MacOsLatest,
    GitHubActionsImage.UbuntuLatest,
    OnPullRequestBranches = [DevelopBranch, MasterBranch],
    PublishArtifacts = false,
    InvokedTargets = [nameof(Test)],
    ReadPermissions = [GitHubActionsPermissions.Contents],
    FetchDepth = 0)]
// Releases go to nuget.org via trusted publishing, which needs id-token to exchange the OIDC
// token for a short-lived key. A single image keeps the push from racing itself.
//
// The tag is the trigger, not the merge into main: GitVersion only resolves the release version
// once the tag exists, so a branch push would publish whatever build-metadata version it computed
// for the untagged merge commit.
[CustomGitHubActions("release",
    GitHubActionsImage.UbuntuLatest,
    OnPushTags = ["v*"],
    PublishArtifacts = false,
    InvokedTargets = [nameof(Test), nameof(Publish)],
    ReadPermissions = [GitHubActionsPermissions.Contents],
    WritePermissions = [GitHubActionsPermissions.IdToken],
    FetchDepth = 0)]
[AppVeyor(
    AppVeyorImage.VisualStudioLatest,
    InvokedTargets = [nameof(Test), nameof(SonarEnd)],
    SkipTags = true,
    AutoGenerate = true)]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Required][GitRepository] readonly GitRepository GitRepository;
    [Required][GitVersion] readonly GitVersion GitVersion;
    [Required][Solution] readonly Solution Solution;

    [Parameter] readonly bool Cover = true;
    [Parameter] readonly string NuGetKey;
    [Parameter] readonly string GitHubToken;

    [CI] readonly GitHubActions GitHubActions;

    const string NuGetSource = "https://api.nuget.org/v3/index.json";
    string GitHubSource => $"https://nuget.pkg.github.com/{GitHubActions.RepositoryOwner}/index.json";

    bool Beta => GitRepository.IsOnDevelopBranch() || GitRepository.IsOnFeatureBranch();

    string Source => Beta ? GitHubSource : NuGetSource;
    string ApiKey => Beta ? GitHubToken : NuGetKey;

    const string SonarProjectKey = "ubiety_Ubiety.Scram.Core";

    static AbsolutePath SourceDirectory => RootDirectory / "src";
    static AbsolutePath TestsDirectory => RootDirectory / "tests";
    static AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    IEnumerable<AbsolutePath> PackageFiles => ArtifactsDirectory.GlobFiles("*.nupkg");

    const string MasterBranch = "main";
    const string DevelopBranch = "develop";
    const string ReleaseBranchPrefix = "release/*";

    [UsedImplicitly]
    Target Clean => t => t
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").DeleteDirectories();
            TestsDirectory.GlobDirectories("**/bin", "**/obj").DeleteDirectories();
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => t => t
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => t => t
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .SetNoRestore(InvokedTargets.Contains(Restore)));
        });

    Target SonarBegin => t => t
        .Before(Compile)
        .Unlisted()
        .Executes(() =>
        {
            SonarScannerBegin(s => s
                .SetProjectKey(SonarProjectKey)
                .SetServer("https://sonarcloud.io")
                .SetVersion(GitVersion.SemVer)
                .SetOpenCoverPaths(ArtifactsDirectory / "coverage.opencover.xml")
                .SetOrganization("ubiety")
                .SetFramework("net9.0"));
        });

    Target SonarEnd => t => t
        .After(Test)
        .DependsOn(SonarBegin)
        .AssuredAfterFailure()
        .Unlisted()
        .Executes(() =>
        {
            SonarScannerEnd(s => s
                .SetFramework("net9.0"));
        });

    Target Test => t => t
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(Solution.GetProject("Ubiety.Scram.Test"))
                .SetNoBuild(InvokedTargets.Contains(Compile))
                .SetConfiguration(Configuration)
                .When(Cover, c => c
                    .EnableCollectCoverage()
                    .SetCoverletOutput(ArtifactsDirectory / "coverage")
                    .SetCoverletOutputFormat(CoverletOutputFormat.opencover)
                    .SetProcessAdditionalArguments("/p:Exclude=[xunit.*]*")));
        });

    Target Docs => t => t
        .Description("Builds the documentation site into docs/_site")
        .Executes(() =>
        {
            // docfx is a local tool so contributors and CI get the pinned version without a
            // global install; see .config/dotnet-tools.json.
            DotNet("tool restore");
            DotNet($"docfx {RootDirectory / "docs" / "docfx.json"}");
        });

    Target Pack => t => t
        .After(Test)
        .DependsOn(Compile)
        .Produces(ArtifactsDirectory / "*.nupkg")
        .Executes(() =>
        {
            DotNetPack(s => s
                .SetNoBuild(InvokedTargets.Contains(Compile))
                .SetConfiguration(Configuration)
                .SetOutputDirectory(ArtifactsDirectory)
                .SetVersion(GitVersion.SemVer));
        });

    Target Publish => t => t
        .DependsOn(Pack)
        .Consumes(Pack)
        .Requires(() => !NuGetKey.IsNullOrEmpty() || Beta)
        .Requires(() => Configuration.Equals(Configuration.Release))
        .Executes(() =>
        {
            if (Beta)
            {
                DotNetNuGetAddSource(c => c
                    .SetSource(GitHubSource)
                    .SetUsername(GitHubActions.Actor)
                    .SetPassword(GitHubToken)
                    .SetStorePasswordInClearText(true));
            }

            // The workflow runs this target on every image in the matrix, and they all compute the
            // same version, so whichever job gets there first wins and the rest see 409 Conflict.
            // Skipping duplicates makes the push idempotent instead of a race.
            DotNetNuGetPush(s => s
                    .SetApiKey(ApiKey)
                    .SetSource(Source)
                    .EnableSkipDuplicate()
                    .CombineWith(PackageFiles, (f, p) => f.SetTargetPath(p)),
                5,
                true);
        });

    public static int Main()
    {
        // GitVersion normalises the repository before calculating a version, and on AppVeyor that
        // normalisation moves HEAD and then aborts because it moved:
        //
        //   GitVersion has a bug, your HEAD has moved after repo normalisation after step
        //   'EnsureLocalBranchExistsForCurrentBranch'
        //
        // Every AppVeyor build has failed on this since 2.0.1, which also means SonarCloud has had
        // no analysis in that time - AppVeyor is the only host that runs SonarEnd. The variable is
        // GitVersion's own documented escape hatch for it. Setting it here rather than in
        // appveyor.yml keeps it from being dropped the next time Nuke regenerates that file, and
        // scoping it to AppVeyor leaves the check in force everywhere else.
        if (Environment.GetEnvironmentVariable("APPVEYOR") is not null)
        {
            Environment.SetEnvironmentVariable("IGNORE_NORMALISATION_GIT_HEAD_MOVE", "1");
        }

        return Execute<Build>(x => x.Test);
    }
}
