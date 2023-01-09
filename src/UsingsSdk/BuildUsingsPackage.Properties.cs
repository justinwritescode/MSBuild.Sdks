/*
 * BuildUsingsPackage.Properties.cs
 *
 *   Created: 2022-12-01-02:58:50
 *   Modified: 2022-12-01-02:58:50
 *
 *   Author: Justin Chase <justin@justinwritescode.com>
 *
 *   Copyright © 2022-2023 Justin Chase, All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

#pragma warning disable
namespace MSBuild.UsingsSdk;

using System.Xml.Serialization;
using XElementOrProjectItemInstance = AnyOf<System.Xml.Linq.XElement, Microsoft.Build.Execution.ProjectItemInstance>;

public partial class BuildUsingsPackage
{
    [Required]
    public string InputFile { get; set; }
    private string? _version;
    public string? Version { get => _version ??= "1.0.0"; set => _version = value; }
    [Required]
    public string OutputFile { get; set; } = string.Empty;
    private string? _packageId;
    public string? PackageId
    {
        get => _packageId ??= AllProperties.GetPropertyValue("PackageId") ?? AllProperties.GetPropertyValue("PackageIdOverride") ??
        AllProperties.GetPropertyValue("AssemblyName") ?? Path.GetFileNameWithoutExtension(InputFile) + ".Usings";
        set => _packageId = value;
    }
    private string PackageReadmeFile => Path.Combine(OutputDirectory, "README.md");

    [Output]
    public string UsingsProjectFile => Path.Combine(OutputDirectory, $"{PackageId}.csproj");
    private string? OutputDirectory => Path.GetDirectoryName(OutputFile);
    private string? PackageLibDirectory => Path.GetDirectoryName(typeof(BuildUsingsPackage).Assembly.Location);
    private string? PackageContentFilesDirectory => Path.Combine(PackageLibDirectory, "../contentFiles");
    private IEnumerable<ProjectTuple?>? _allProjects;
    protected IEnumerable<ProjectTuple?> AllProjects => _allProjects ??= Load(InputFile)!;
    private IEnumerable<ProjectPropertyInstance>? _allProperties;
    protected IEnumerable<ProjectPropertyInstance> AllProperties => _allProperties ??= AllProjects.SelectMany(p => p?.ProjectInstance?.Properties ?? Enumerable.Empty<ProjectPropertyInstance>());
    protected string? IconFile => AllProjects.SelectMany(project => project?.XDocument.Descendants().Where(x => x.GetIncludeValue()?.EndsWith("icon.png", StringComparison.CurrentCultureIgnoreCase) ?? false)!).FirstOrDefault()?.GetAttributeValue("Include") ??
            AllProjects.SelectMany(project => project?.XDocument.Descendants().Where(x => x.GetIncludeValue()?.EndsWith("icon.jpg", StringComparison.CurrentCultureIgnoreCase) ?? false)!).FirstOrDefault()?.GetAttributeValue("Include") ??
            "Icon.png";
    protected string Description => $"This project contains a set of `using` statements and package and project imports for the `{Path.GetFileNameWithoutExtension(InputFile)}` namespace for reuse in other projects";
    protected string Authors => AllProperties.GetPropertyValue("Authors", "No Author Specified");
    protected string Copyright => AllProperties.GetPropertyValue(nameof(Copyright), "No Copyright Specified");
    protected string PackageOutputPath => AllProperties.GetPropertyValue("PackageOutputPath") ??
        AllProperties.GetPropertyValue("OutputPath") ??
        AllProperties.GetPropertyValue("OutDir") ??
        AllProperties.GetPropertyValue("BaseOutputPath") ??
        Path.Combine(Path.GetDirectoryName(InputFile), "artifacts");

    private XElement[] DefaultPackageReferences = Array.Empty<XElement>();/*new[]
    {
        new XElement("PackageReference", new XAttribute("Include", "Microsoft.SourceLink.GitHub"), new XAttribute("Version", "[1.1.1,)"), new XAttribute("PrivateAssets", "All")),
        new XElement("PackageReference", new XAttribute("Include", "Microsoft.Build.Tasks.Git"), new XAttribute("Version", "[1.1.1,)"), new XAttribute("PrivateAssets", "All")),
        new XElement("PackageReference", new XAttribute("Include", "GitInfo"), new XAttribute("Version", "[2.2.0,)"), new XAttribute("PrivateAssets", "All"))
    };*/
    private const string EmptyProjectFile = "<Project />";
    private const string? DefaultTargetFramework = null;
    private const string DefaultTargetFrameworks = "netstandard1.0;netstandard1.1;netstandard1.3;netstandard2.0;netstandard2.1;netcoreapp3.1;net6.0;net7.0";

    protected string TargetFramework => AllProperties.GetPropertyValue(nameof(TargetFramework), DefaultTargetFramework);
    protected string TargetFrameworks => AllProperties.GetPropertyValue(nameof(TargetFrameworks), DefaultTargetFrameworks);
    public static readonly ComparersImplementation Comparers = new();
    private string NuGetPackagesExistCachePath => Path.Combine(OutputDirectory, "../nugetPackagesExist.cache.json");
    private static Dictionary<string, bool>? _nuGetPackagesExistCache;
    private IDictionary<string, bool> NuGetPackagesExistCache => _nuGetPackagesExistCache ??=
        File.Exists(NuGetPackagesExistCachePath) ?
        JsonSerializer.Deserialize<Dictionary<string, bool>>(File.ReadAllText(NuGetPackagesExistCachePath))! : new Dictionary<string, bool>()!;

    private XElement[] XUsings => AllProjects.GetXItems("Using").ToArray();
    private ProjectItemInstance[] Usings => AllProjects.GetItems("Using").ToArray();
    private XElement[] XProjectReferences => AllProjects.GetXItems("ProjectReference").Distinct(Comparers).ToArray();
    private XElement[]  XFrameworkReferences => AllProjects.GetXItems("FrameworkReference").Distinct(Comparers).ToArray();
    private XElement[]  XPackageReferences => AllProjects.GetXItems("PackageReference").Distinct(Comparers).ToArray();
    private ProjectItemInstance[] ProjectReferences => AllProjects.GetItems("ProjectReference").Distinct(Comparers).ToArray();
    private ProjectItemInstance[] PackageReferences => AllProjects.GetItems("PackageReference").Distinct(Comparers).ToArray();
    private ProjectItemTuple[] ProjectReferenceTuples => XProjectReferences.Join(ProjectReferences);
    private ProjectItemTuple[] PackageReferenceTuples => XPackageReferences.Join(PackageReferences);

    private string PackageIdOverride => AllProperties.GetPropertyValue(nameof(PackageIdOverride), PackageId);
    private string PackageVersionOverride => AllProperties.GetPropertyValue(nameof(PackageVersionOverride), Version);
    private string PackageVersion => AllProperties.GetPropertyValue(nameof(PackageVersion), Version);

    private string MinVerVersionOverride => AllProperties.GetPropertyValue(nameof(MinVerVersionOverride), Version);
    private string FileVersion => AllProperties.GetPropertyValue(nameof(FileVersion), Version);
    private string AssemblyVersion => AllProperties.GetPropertyValue(nameof(AssemblyVersion), Regex.Replace(Version, "-.*", ""));
    private string PackageLicenseExpression => AllProperties.GetPropertyValue(nameof(PackageLicenseExpression), "MIT");
    private string PackageIcon => Path.GetFileName(IconFile);
    private const string GeneratePackageOnBuild = "true";
    private const string IsPackable = "true";
    private const string IsNuGetized = "true";
    private const string PublishRepositoryUrl = "true";
    private const string NoBuild = "true";
    private const string IncludeSource = "false";
    private const string IncludeBuiltProjectOutputGroup = "false";
    private const string IncludeSourceFilesProjectOutputGroup = "false";
    private const string IncludeContentFilesProjectOutputGroup = "false";
    private const string IncludeBuildOutput = "false";
    private const string IncludeSymbols = "false";
    private string PackageTags => "using usings namespace nuget package " + PackageId;
    private string Title => PackageId;
    private string Summary => Description;
}
