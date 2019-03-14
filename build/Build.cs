using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.CoverallsNet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.DotNetSonarScanner;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.CoverallsNet.CoverallsNetTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.DotNetSonarScanner.DotNetSonarScannerTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.Test);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter] readonly string SonarKey;
    [Parameter] readonly string NuGetKey;
    [Parameter] readonly bool? Cover = true;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    readonly string NuGetSource = "https://api.nuget.org/v3/index.json";
    readonly string SonarProjectKey = "ubiety_Ubiety.Scram.Core";
    [Unlisted] [ProjectFrom(nameof(Solution))] readonly Project UbietyScramTestProject;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
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
                .SetAssemblyVersion(GitVersion.GetNormalizedAssemblyVersion())
                .SetFileVersion(GitVersion.GetNormalizedFileVersion())
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });

    Target SonarBegin => _ => _
        .Before(Compile)
        .Requires(() => SonarKey)
        .Unlisted()
        .Executes(() =>
        {
            DotNetSonarScannerBegin(s => s
                .SetLogin(SonarKey)
                .SetProjectKey(SonarProjectKey)
                .SetOrganization("ubiety")
                .SetServer("https://sonarcloud.io")
                .SetVersion(GitVersion.NuGetVersionV2)
                .SetOpenCoverPaths(ArtifactsDirectory / "coverage.opencover.xml"));
        });

    Target SonarEnd => _ => _
        .After(Test)
        .DependsOn(SonarBegin)
        .Requires(() => SonarKey)
        .Unlisted()
        .Executes(() =>
        {
            DotNetSonarScannerEnd(s => s
                .SetLogin(SonarKey));
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(UbietyScramTestProject)
                .EnableNoBuild()
                .SetConfiguration(Configuration)
                .SetArgumentConfigurator(args => args.Add("/p:CollectCoverage={0}", Cover)
                    .Add("/p:CoverletOutput={0}", ArtifactsDirectory / "coverage")
                    .Add("/p:CoverletOutputFormat={0}", "opencover")
                    .Add("/p:Exclude={0}", "[xunit.*]*")));
        });

    Target Coverage => _ => _
        .DependsOn(Test)
        .Executes(() =>
        {
            CoverallsNet(s => s
                .SetOpenCover(true)
                .SetInput(ArtifactsDirectory / "coverage.opencover.xml"));
        });

    Target Pack => _ => _
        .After(Test)
        .Executes(() =>
        {
            DotNetPack(s => s
                .EnableNoBuild()
                .SetOutputDirectory(ArtifactsDirectory)
                .SetVersion(GitVersion.NuGetVersionV2));
        });

    Target Publish => _ => _
        .DependsOn(Pack)
        .Requires(() => NuGetKey)
        .Requires(() => Configuration.Equals(Configuration.Release))
        .Executes(() =>
        {
            if (GitRepository.IsOnMasterBranch())
            {
                DotNetNuGetPush(s => s
                        .SetApiKey(NuGetKey)
                        .SetSource(NuGetSource)
                        .CombineWith(
                            ArtifactsDirectory.GlobFiles("*.nupkg").NotEmpty(), (cs, v) =>
                                cs.SetTargetPath(v)),
                    5,
                    true);                
            }
        });

    Target Appveyor => _ => _
        .DependsOn(Coverage, SonarEnd, Publish);
}
