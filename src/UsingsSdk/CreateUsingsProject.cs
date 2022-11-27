/*
 * CreateUsingsProject.cs
 *
 *   Created: 2022-11-22-03:14:00
 *   Modified: 2022-11-26-02:33:22
 *
 *   Author: Justin Chase <justin@justinwritescode.com>
 *
 *   Copyright © 2022 Justin Chase, All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */
#pragma warning disable CA3003
namespace MSBuild.UsingsSdk;


using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Build.Execution;
using static XElementExtensions;
using MSBC = Microsoft.Build.Construction;
using MSBEx = Microsoft.Build.Execution;

public partial class CreateUsingsProject : MSBTask
{
    [Required]
    public string InputFile { get; set; } = string.Empty;
	private string? _version;
	public string? Version { get => _version ??= "1.0.0"; set => _version = value; }
    [Required]
    public string OutputFile { get; set; } = string.Empty;
	private string? _packageId;
    public string? PackageId
	{
		get  => _packageId ??= AllProperties.GetPropertyValue("PackageId") ?? AllProperties.GetPropertyValue("PackageIdOverride") ??
		AllProperties.GetPropertyValue("AssemblyName") ?? Path.GetFileNameWithoutExtension(InputFile) + ".Usings";
		set => _packageId = value;
	}
	private string PackageReadmeFile => Path.Combine(OutputDirectory, "README.md");
	[Output]
	public string UsingsProjectFile => Path.Combine(OutputDirectory, $"{PackageId}.proj");
	private string? OutputDirectory => Path.GetDirectoryName(OutputFile);
	private string? PackageLibDirectory => Path.GetDirectoryName(typeof(CreateUsingsProject).Assembly.Location);
	private string? PackageContentFilesDirectory => Path.Combine(PackageLibDirectory, "../contentFiles");
	private IEnumerable<(ProjectInstance?, XDocument?)?>? _allProjects;
	protected IEnumerable<(ProjectInstance? ProjectInstance, XDocument? XDocument)?> AllProjects => _allProjects ??= Load(InputFile)!;
	private IEnumerable<ProjectPropertyInstance>? _allProperties;
	protected IEnumerable<ProjectPropertyInstance> AllProperties => _allProperties ??= AllProjects.SelectMany(p => p?.ProjectInstance?.Properties ?? Enumerable.Empty<ProjectPropertyInstance>());
	protected string? IconFile => AllProjects.SelectMany(project => project?.XDocument.Descendants().Where(x => x.GetIncludeValue()?.EndsWith("icon.png", StringComparison.CurrentCultureIgnoreCase) ?? false)!).FirstOrDefault()?.GetAttributeValue("Include") ??
			AllProjects.SelectMany(project => project?.XDocument.Descendants().Where(x => x.GetIncludeValue()?.EndsWith("icon.jpg", StringComparison.CurrentCultureIgnoreCase) ?? false)!).FirstOrDefault()?.GetAttributeValue("Include") ??
			"Icon.png";
	protected string Description => $"This project contains a set of `using` statements and package and project imports for the `{Path.GetFileNameWithoutExtension(InputFile)}` namespace for reuse in other projects";
	protected string Authors => AllProperties.GetPropertyValue("Authors", "No Author Specified");
	protected string Copyright => AllProperties.GetPropertyValue("Copyright", "No Copyright Specified");
	protected string PackageOutputPath => AllProperties.GetPropertyValue("PackageOutputPath") ??
		AllProperties.GetPropertyValue("OutputPath") ??
		AllProperties.GetPropertyValue("OutDir") ??
		AllProperties.GetPropertyValue("BaseOutputPath") ??
		Path.Combine(Path.GetDirectoryName(InputFile), "artifacts");
	private const string EmptyProjectFile = "<Project></Project>";
	protected string TargetFramework => AllProperties.GetPropertyValue("TargetFramework", "netstandard2.0");
	public static readonly ComparersImplementation Comparers = new();
	private string NuGetPackagesExistCachePath => Path.Combine(OutputDirectory, "../nugetPackagesExist.cache.json");
	private static Dictionary<string, bool>? _nuGetPackagesExistCache;
	private IDictionary<string, bool> NuGetPackagesExistCache => _nuGetPackagesExistCache ??=
		File.Exists(NuGetPackagesExistCachePath) ?
		JsonSerializer.Deserialize<Dictionary<string, bool>>(File.ReadAllText(NuGetPackagesExistCachePath))! : new Dictionary<string, bool>()!;

	public virtual IEnumerable<(ProjectInstance? ProjectInstance, XDocument? XDocument)?>? Load(string? path)
    {
		if(path is null) return new[] { null as (ProjectInstance?, XDocument?)? };
        var project = new ProjectInstance(path);
		var xDocumentProject = XDocument.Load(path);
        return new [] { (project, xDocumentProject) as (ProjectInstance?, XDocument?)? }.Concat(xDocumentProject.Descendants("Import").SelectMany(x => Load(x.GetIncludeValue()).Where(p => p is not null)))
		.Where(x => x is not null);
    }

    public override bool Execute()
    {
		var markdownReadme = new StringBuilder();
		markdownReadme.AppendFormat("---{0}title: {1}{0}version: {2}{0}authors: {3}{0}copyright: {4}{0}description: {5}{0}date: {6}{0}---{0}{0}", Environment.NewLine, PackageId, Version, Authors, Copyright, Description, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
		markdownReadme.AppendLine();
		markdownReadme.AppendLine($"## {PackageId}");
		markdownReadme.AppendLine();
		markdownReadme.AppendLine(Description);

        Log.LogMessage("Found " + AllProjects.Count() + " imported projects to process");

        var xUsings = AllProjects.GetXItems("Using").Distinct(Comparers).ToArray();
		var usings = AllProjects.GetItems("Using").Distinct(Comparers).ToArray();
		var xProjectReferences = AllProjects.GetXItems("ProjectReference").Distinct(Comparers).ToArray();
		var xPackageReferences = AllProjects.GetXItems("PackageReference").Distinct(Comparers).ToArray();
		var projectReferences = AllProjects.GetItems("ProjectReference").Distinct(Comparers).ToArray();
		var packageReferences = AllProjects.GetItems("PackageReference").Distinct(Comparers).ToArray();
        var properties = MakeProperties(AllProperties); //.OrderBy(x => x.Name)).ToArray();
		var projectReferenceTuples = from XProjectReference in xProjectReferences
		join ProjectReference in projectReferences on XProjectReference.GetAttributeValue("Include") equals ProjectReference.EvaluatedInclude
		select (XProjectReference, ProjectReference);
		var packageReferenceTuples = from XPackageReference in xPackageReferences
		join PackageReference in packageReferences on XPackageReference.GetAttributeValue("Include") equals PackageReference.EvaluatedInclude
		select (XPackageReference, PackageReference);
		var usingsTuples = from XUsing in xUsings
		join Using in usings on XUsing.GetAttributeValue("Include") equals Using.EvaluatedInclude
		select (XUsing, Using);

        var usingsFile = new XDocument(
            new XComment("<auto-generated />"),
            new XComment("This code was generated by a tool.  Do not modify it."),
            new XElement("Project",
				new XComment("Usings: " + xUsings.Length),
                new XElement("ItemGroup",
                    new XAttribute("Label", "Usings"),
                    new XComment("⬇️ Global Usings ⬇️"),
                    usingsTuples.Select(x => FormatReference(x, "Using")).Append<XNode>(
					new XComment("⬆️  🫴🏻 💪🏻  ⬆️"))),
                new XElement("ItemGroup",
                    new XAttribute("Label", "Package References"),
                    new XComment("📦 ⬇️ Package References ⬇️  📦"),
                    packageReferenceTuples.Select(x => FormatReference(x, "PackageReference", false)).Append<XNode>(
					new XComment("📦  ⬆️  ⬆️  📦"))),
                new XElement("ItemGroup",
                    new XAttribute("Label", "Project References"),
                    new XComment("⬇️ Project References ⬇️"),
					projectReferenceTuples.Select(x => FormatReference(x, "ProjectReference")).Append<XNode>(
					new XComment("⬆️    💻    ⬆️")))));


        var usingsProjectFile = new XDocument(
			new XComment("<auto-generated />"),
			new XComment("This code was generated by a tool.  Do not modify it."),
            new XElement("Project",
				new XAttribute("Sdk", "Microsoft.Build.NoTargets"),
				new XComment("properties: " + properties.Length),
				new XComment("⬇️ Properties ⬇️"),
				new XElement("PropertyGroup",
                    properties),
				new XElement("ItemGroup",
					packageReferenceTuples.Select(x => FormatReference(x, "PackageReference", true))),
				new XElement("ItemGroup",
					new XElement("Content", new XAttribute("Include", "README.md"), new XAttribute("Pack", "true"), new XAttribute("PackagePath", "README.md")),
					new XElement("Content", new XAttribute("Include", Path.Combine(Path.GetDirectoryName(InputFile), OutputFile)), new XAttribute("Pack", "true"), new XAttribute("PackagePath", "build/%(Filename)%(Extension)")),
					new XElement("Content", new XAttribute("Include", IconFile), new XAttribute("Pack", "true"), new XAttribute("PackagePath", Path.GetFileName(IconFile))))));

        Log.LogMessage("Properties: " + properties.Length);
        Log.LogMessage("Usings: " + xUsings.Length);
        Log.LogMessage("ProjectReferences: " + xProjectReferences.Length);
        Log.LogMessage("PackageReference: " + xPackageReferences.Length);

		markdownReadme.AppendLine();
		markdownReadme.AppendLine("### Usings");
		markdownReadme.AppendLine();
		markdownReadme.AppendLine(string.Join(Environment.NewLine, xUsings.Select(x => $"- {x.GetIncludeValue()}{FormatIsStatic(x)}{FormatAlias(x)}")));

		markdownReadme.AppendLine();
		markdownReadme.AppendLine("### Package References");
		markdownReadme.AppendLine();
		markdownReadme.AppendLine(string.Join(Environment.NewLine, xPackageReferences.Select(FormatPackageReferenceMarkdown)));

		markdownReadme.AppendLine();
		markdownReadme.AppendLine("### Project References");
		markdownReadme.AppendLine();
		markdownReadme.AppendLine(string.Join(Environment.NewLine, xProjectReferences.Select(x => $"- {x.GetIncludeValue()}")));


		if(!Directory.Exists(OutputDirectory)) Directory.CreateDirectory(OutputDirectory);

        using (var outFile = File.CreateText(OutputFile))
        {
            usingsFile.Save(outFile);
        }
        using(var outFile = File.CreateText(UsingsProjectFile))
		{
			usingsProjectFile.Save(outFile);
		}
		using (var outFile = File.CreateText(PackageReadmeFile))
		{
			outFile.WriteLine(markdownReadme.ToString());
		}
		using (var outFile = File.CreateText(Path.Combine(OutputDirectory, "Directory.Build.props")))
		{
			outFile.WriteLine(EmptyProjectFile);
		}
		using (var outFile = File.CreateText(Path.Combine(OutputDirectory, "Directory.Build.targets")))
		{
			outFile.WriteLine(EmptyProjectFile);
		}
		File.Copy(Path.Combine(PackageLibDirectory, "../ContentFiles/global.json"), Path.Combine(OutputDirectory, "global.json"), true);
        Log.LogMessage(usingsFile.ToString());
        Log.LogMessage("Wrote file: " + OutputFile);

		File.Copy(IconFile, Path.Combine(OutputDirectory, "Icon.png"), true);

		File.WriteAllText(NuGetPackagesExistCachePath, JsonSerializer.Serialize(NuGetPackagesExistCache));

        return true;
    }

	private XElement[] MakeProperties(IEnumerable<MSBEx.ProjectPropertyInstance> properties)
	{
		var copiedProperties = new List<XElement>
		{
			new XElement("Description", Description),
			new XElement("PackageId", properties.GetPropertyValue("PackageId", PackageId)),
			new XElement("TargetFramework", TargetFramework),
			new XElement("TargetFrameworks", TargetFramework),
			new XElement("PackageIdOverride", properties.GetPropertyValue("PackageIdOverride", PackageId)),
			new XElement("Version", properties.GetPropertyValue("Version", Version)),
			new XElement("PackageVersion", properties.GetPropertyValue("PackageVersion", Version)),
			new XElement("MinVerVersionOverride", properties.GetPropertyValue("MinVerVersionOverride", Version)),
			new XElement("FileVersion", properties.GetPropertyValue("FileVersion", Version)),
			new XElement("AssemblyVersion", properties.GetPropertyValue("AssemblyVersion", Regex.Replace(Version, "-.*", ""))),
			new XElement("PackageLicenseExpression", properties.GetPropertyValue("PackageLicenseExpression", "MIT")),
			new XElement("PackageOutputPath", PackageOutputPath),
			new XElement("PackageIcon", Path.GetFileName(IconFile)),
			new XElement("GeneratePackageOnBuild", "true"),
			new XElement("IsPackable", "true"),
			new XElement("IsNuGetized", "true"),
			new XElement("Title", PackageId),
			new XElement("Summary", Description),
			new XElement("Authors", Authors),
			new XElement("Copyright", Copyright),
			new XElement("PackageTags", "using usings namespace nuget package " + PackageId)
		};
		return copiedProperties.OrderBy(p => p.Name.ToString()).ToArray();
	}

	// private static XElement FormatUsing(MSBC.ProjectItemElement @using) => FormatUsing(((AnyOf<MSBC.ProjectItemElement, XElement>)@using));
	// private static XElement FormatUsing(XElement @using) //=> FormatUsing(((AnyOf<MSBC.ProjectItemElement, XElement>)@using));
	// //private static XElement FormatUsing(AnyOf<ProjectItemInstance, XElement> @using)
	// {
	// 	// var include = @using.GetAttributeValue("Include");
	// 	// var alias = @using.GetAttributeValue("Alias");
	// 	// var isStatic = @using.GetAttributeValue("Static");
	// 	return new XElement("Using", GetReferenceAttributes(@using));
	// }

	private static XAttribute[] GetReferenceAttributes(AnyOf<ProjectItemInstance, XElement> @ref, bool includeVersion = true) =>
		@ref.IsFirst? new[] { new XAttribute("Include", @ref.First.EvaluatedInclude) }.Concat(@ref.First.Metadata.Select(x => new XAttribute(x.Name, x.EvaluatedValue)).Where(x => includeVersion || x.Name.LocalName != "Version")).Distinct(Comparers).ToArray() :
			new[] { new XAttribute("Include", @ref.Second.GetAttributeValue("Include")) }.Concat(@ref.Second.Attributes().Select(x => new XAttribute(x.Name, x.Value)).Where(x => includeVersion || x.Name.LocalName != "Version")).Distinct(Comparers).ToArray();

	private static XElement FormatReference((XElement XItem, ProjectItemInstance Item) @ref, string type = "ProjectReference", bool includeVersion = true) =>
		new(type, GetReferenceAttributes(@ref.XItem, includeVersion).Concat(GetReferenceAttributes(@ref.Item, includeVersion)).Distinct(Comparers).ToArray());

	private string FormatIsStatic(AnyOf<MSBC.ProjectItemElement, XElement> x)
	{
		var metadataValue = x.GetAttributeValue("Static");
		if (metadataValue?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false)
		{
			return " *(static)*";
		}
		return string.Empty;
	}

	private string GetNuGetUri(XElement @element) => $"https://www.nuget.org/packages/{@element.GetIncludeValue()}";

	private bool GetNuGetPackageExists(XElement @element) =>
		 NuGetPackagesExistCache[@element.GetIncludeValue()] =
		 	NuGetPackagesExistCache.ContainsKey(@element.GetIncludeValue()) ? NuGetPackagesExistCache[@element.GetIncludeValue()] :
			new HttpClient().SendAsync(new HttpRequestMessage(HttpMethod.Get, GetNuGetUri(@element))).Result.IsSuccessStatusCode;

	private string? FormatPackageReferenceMarkdown(XElement @element) =>
		GetNuGetPackageExists(@element) ? $"- [{@element.GetIncludeValue()}]({GetNuGetUri(@element)})" : $"- {@element.GetIncludeValue()}";
	private string FormatAlias(AnyOf<MSBC.ProjectItemElement, XElement> x)
	{
		var alias = x.IsFirst ? x.First.GetMetadataValue("Alias") : x.Second.GetAttributeValue("Alias");
		if (string.IsNullOrWhiteSpace(alias))
			return string.Empty;
		return $" (Alias: *{alias}*)";
	}
}
