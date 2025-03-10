using System.Collections.Generic;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.SonarScanner;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.SonarScanner.SonarScannerTasks;

[GitHubActions(
    "release",
    GitHubActionsImage.WindowsLatest,
    OnPushBranches = [MasterBranch, ReleaseBranchPrefix + "/*"],
    InvokedTargets = [nameof(Test), nameof(Publish)],
    ImportSecrets = [nameof(NuGetKey)],
    EnableGitHubToken = true,
    FetchDepth = 0)]
[GitHubActions(
    "continuous",
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.UbuntuLatest,
    GitHubActionsImage.MacOsLatest,
    OnPushBranchesIgnore = [MasterBranch, ReleaseBranchPrefix + "/*"],
    OnPullRequestBranches = [DevelopBranch],
    PublishArtifacts = true,
    InvokedTargets = [nameof(Test), nameof(Publish)],
    EnableGitHubToken = true,
    FetchDepth = 0)]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;
    [CI] readonly GitHubActions GitHubActions;
    
    [Parameter] readonly string SonarKey;
    [Parameter] readonly string NuGetKey;
    [Parameter] readonly bool Cover = true;
    [Parameter] readonly string GitHubToken;
    
    [Solution] readonly Solution Solution;
    Project TestProject => Solution.GetProject("Ubiety.Scram.Test");

    const string NuGetSource = "https://api.nuget.org/v3/index.json";
    const string SonarProjectKey = "ubiety_Ubiety.Scram.Core";
    string GitHubSource => $"https://nuget.pkg.github.com/{GitHubActions.RepositoryOwner}/index.json";
    string Source => Beta ? GitHubSource : NuGetSource;
    string ApiKey => Beta ? GitHubToken : NuGetKey;
    
    bool Beta => GitRepository.IsOnDevelopBranch() || GitRepository.IsOnFeatureBranch();
    IEnumerable<AbsolutePath> PackageFiles => ArtifactsDirectory.GlobFiles("*.nupkg");
    
    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    
    const string MasterBranch = "main";
    const string DevelopBranch = "develop";
    const string ReleaseBranchPrefix = "release";

    [UsedImplicitly]
    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").DeleteDirectories();
            TestsDirectory.GlobDirectories("**/bin", "**/obj").DeleteDirectories();
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
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

    Target SonarBegin => _ => _
        .Before(Compile)
        .Unlisted()
        .Executes(() =>
        {
            SonarScannerBegin(s => s
                .SetProjectKey(SonarProjectKey)
                .SetServer("https://sonarcloud.io")
                .SetVersion(GitVersion.NuGetVersion)
                .SetOpenCoverPaths(ArtifactsDirectory / "coverage.opencover.xml")
                .SetOrganization("ubiety")
                .SetFramework("net9.0"));
        });

    Target SonarEnd => _ => _
        .After(Test)
        .DependsOn(SonarBegin)
        .AssuredAfterFailure()
        .Unlisted()
        .Executes(() =>
        {
            SonarScannerEnd(s => s
                .SetFramework("net9.0"));
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(TestProject)
                .SetNoBuild(InvokedTargets.Contains(Compile))
                .SetConfiguration(Configuration)
                .When(Cover, _ => _
                    .EnableCollectCoverage()
                    .SetCoverletOutputFormat(CoverletOutputFormat.opencover)
                    .SetCoverletOutput(ArtifactsDirectory / "coverage")
                    .SetProcessAdditionalArguments("/p:Exclude=\"[xunit.*]*\"")));
        });

    Target Pack => _ => _
        .After(Test)
        .Executes(() =>
        {
            DotNetPack(s => s
                .EnableNoBuild()
                .SetConfiguration(Configuration)
                .SetOutputDirectory(ArtifactsDirectory)
                .SetVersion(GitVersion.NuGetVersionV2));
        });

    Target Publish => _ => _
        .DependsOn(Pack)
        .Consumes(Pack)
        .Requires(() => !NuGetKey.IsNullOrEmpty() || Beta)
        .Requires(() => Configuration.Equals(Configuration.Release))
        .Executes(() =>
        {
            if (Beta)
            {
                DotNetNuGetAddSource(_ => _
                    .SetSource(GitHubSource)
                    .SetUsername(GitHubActions.Actor)
                    .SetPassword(GitHubToken)
                    .SetStorePasswordInClearText(true));
            }
            
            DotNetNuGetPush(s => s
                    .SetApiKey(ApiKey)
                    .SetSource(Source)
                    .CombineWith(PackageFiles, (_, v) => _.SetTargetPath(v)),
                5,
                true);
        });

    public static int Main() => Execute<Build>(x => x.Test);
}
