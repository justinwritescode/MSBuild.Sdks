//
// CreateUsingsProject.cs
//
//   Created: 2022-11-12-08:52:03
//   Modified: 2022-11-12-03:59:07
//
//   Author: Justin Chase <justin@justinwritescode.com>
//
//   Copyright © 2022 Justin Chase, All Rights Reserved
//      License: MIT (https://opensource.org/licenses/MIT)
//
namespace MSBuild.UsingsSdk;


using System.Text;
using System.Xml;
using System.Net;
using System.Xml.Schema;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using Microsoft.Build.Execution;
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
	private string OutputDirectory => Path.GetDirectoryName(OutputFile);
	private string PackageLibDirectory => Path.GetDirectoryName(typeof(CreateUsingsProject).Assembly.Location);
	private IEnumerable<(ProjectInstance?, XDocument?)?>? _allProjects;
	protected IEnumerable<(ProjectInstance? ProjectInstance, XDocument? XDocument)?> AllProjects => _allProjects ??= Load(InputFile)!;
	private IEnumerable<ProjectPropertyInstance>? _allProperties;
	protected IEnumerable<ProjectPropertyInstance> AllProperties => _allProperties ??= AllProjects.SelectMany(p => p?.ProjectInstance?.Properties ?? Enumerable.Empty<ProjectPropertyInstance>());
	protected string IconFile => AllProjects.SelectMany(project => project?.XDocument.Descendants().Where(x => x.GetAttributeValue("Include")?.EndsWith("icon.png", StringComparison.CurrentCultureIgnoreCase) ?? false)).FirstOrDefault()?.GetAttributeValue("Include") ??
			AllProjects.SelectMany(project => project?.XDocument.Descendants().Where(x => x.GetAttributeValue("Include")?.EndsWith("icon.jpg", StringComparison.CurrentCultureIgnoreCase) ?? false)).FirstOrDefault()?.GetAttributeValue("Include") ??
			Path.Combine(PackageLibDirectory, "../ContentFiles/Icon.png");
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


	public virtual IEnumerable<(ProjectInstance? ProjectInstance, XDocument? XDocument)?>? Load(string? path)
    {
		if(path is null) return new[] { null as (ProjectInstance?, XDocument?)? };
        var project = new ProjectInstance(path);
		var xDocumentProject = XDocument.Load(path);
        return new [] { (project, xDocumentProject) as (ProjectInstance?, XDocument?)? }.Concat(xDocumentProject.Descendants("Import").SelectMany(x => Load(GetIncludeValue(x))))
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

        var usings = AllProjects.GetXItems("Using").Distinct(Comparers).ToArray();
		var xProjectReferences = AllProjects.GetXItems("ProjectReference").Distinct(Comparers).ToArray();
		var xPackageReferences = AllProjects.GetXItems("PackageReference").Distinct(Comparers).ToArray();
		var projectReferences = AllProjects.GetItems("ProjectReference").Distinct(Comparers).ToArray();
		var packageReferences = AllProjects.GetItems("PackageReference").Distinct(Comparers).ToArray();
        var properties = MakeProperties(AllProperties); //.OrderBy(x => x.Name)).ToArray();

        var usingsFile = new XDocument(
            new XComment("<auto-generated />"),
            new XComment("This code was generated by a tool.  Do not modify it."),
            new XElement("Project",
				new XComment("Usings: " + usings.Length),
                new XElement("ItemGroup",
                    new XAttribute("Label", "Usings"),
                    new XComment("⬇️ Usings ⬇️"),
                    usings.Select(FormatUsing)),
                new XElement("ItemGroup",
                    new XAttribute("Label", "Package References"),
                    new XComment("⬇️ Package References ⬇️"),
                    xPackageReferences.Select(FormatPackageReference)),
                new XElement("ItemGroup",
                    new XAttribute("Label", "Project References"),
                    new XComment("⬇️ Project References ⬇️"),
                    xProjectReferences.Select(FormatProjectReference))));


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
					packageReferences.Select(FormatPackageReference)),
				new XElement("ItemGroup",
					new XElement("PackageFile", new XAttribute("Include", "README.md"), new XAttribute("Pack", "true"), new XAttribute("PackagePath", "README.md")),
					new XElement("PackageFile", new XAttribute("Include", Path.Combine(Path.GetDirectoryName(InputFile), OutputFile)), new XAttribute("Pack", "true"), new XAttribute("PackagePath", "build/%(Filename)%(Extension)")),
					new XElement("PackageFile", new XAttribute("Include", IconFile), new XAttribute("Pack", "true"), new XAttribute("PackagePath", Path.GetFileName(IconFile))))));

        Log.LogMessage("Properties: " + properties.Length);
        Log.LogMessage("Usings: " + usings.Length);
        Log.LogMessage("ProjectReferences: " + xProjectReferences.Length);
        Log.LogMessage("PackageReference: " + xPackageReferences.Length);

		markdownReadme.AppendLine();
		markdownReadme.AppendLine("### Usings");
		markdownReadme.AppendLine();
		markdownReadme.AppendLine(string.Join(Environment.NewLine, usings.Select(x => $"- {GetIncludeValue(x)}{FormatIsStatic(x)}{FormatAlias(x)}")));

		markdownReadme.AppendLine();
		markdownReadme.AppendLine("### Package References");
		markdownReadme.AppendLine();
		markdownReadme.AppendLine(string.Join(Environment.NewLine, xPackageReferences.Select(x => $"- {GetIncludeValue(x)}")));

		markdownReadme.AppendLine();
		markdownReadme.AppendLine("### Project References");
		markdownReadme.AppendLine();
		markdownReadme.AppendLine(string.Join(Environment.NewLine, xProjectReferences.Select(x => $"- {GetIncludeValue(x)}")));


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

        return true;
    }

    private static string? GetIncludeValue(MSBC.ProjectItemElement @element) => @element.GetMetadataValue("Include");
	private static string? GetIncludeValue(ProjectItemInstance @element) => @element.GetMetadataValue("Include");
    private static string? GetIncludeValue(XElement @element) => @element.GetAttributeValue("Include");

	private XElement[] MakeProperties(IEnumerable<MSBEx.ProjectPropertyInstance> properties)
	{
		// var propertiesToReplace = new[] { "Description", "PackageReadme", "PackageId", "PackageIdOverride", "MSBuild" };
		// var propertiesToCopy = properties.Where(x => !propertiesToReplace.Contains(x.Name, StringComparer.CurrentCultureIgnoreCase)  && !propertiesToReplace.Any(p => x.Name.StartsWith(p, StringComparison.CurrentCultureIgnoreCase)));
		// propertiesToCopy = propertiesToCopy.Distinct(this).OrderBy(x => x.Name);
		// var copiedProperties = propertiesToCopy.Select(FormatProperty).ToList();
		// var description = properties.GetPropertyValue("Description");
		// copiedProperties.Add(new XElement("Description", $"This project contains a set of `using` statements and package and product imports for the `{Path.GetFileNameWithoutExtension(InputFile)}` namespace for reuse in other projects"));
		// copiedProperties.Add(new XElement("PackageReadme", "README.md"));
		// copiedProperties.Add(new XElement("PackageId", PackageId));
		// copiedProperties.Add(new XElement("PackageIdOverride", PackageId));
		// return copiedProperties.ToArray();
		var copiedProperties = new List<XElement>();
		copiedProperties.Add(new XElement("Description", Description));
		copiedProperties.Add(new XElement("PackageId", properties.GetPropertyValue("PackageId", PackageId)));
		copiedProperties.Add(new XElement("TargetFramework", TargetFramework));
		copiedProperties.Add(new XElement("TargetFrameworks", TargetFramework));
		copiedProperties.Add(new XElement("PackageIdOverride", properties.GetPropertyValue("PackageIdOverride", PackageId)));
		copiedProperties.Add(new XElement("Version", properties.GetPropertyValue("Version", Version)));
		copiedProperties.Add(new XElement("PackageVersion", properties.GetPropertyValue("PackageVersion", Version)));
		copiedProperties.Add(new XElement("MinVerVersionOverride", properties.GetPropertyValue("MinVerVersionOverride", Version)));
		copiedProperties.Add(new XElement("FileVersion", properties.GetPropertyValue("FileVersion", Version)));
		copiedProperties.Add(new XElement("AssemblyVersion", properties.GetPropertyValue("AssemblyVersion", System.Text.RegularExpressions.Regex.Replace(Version, "-.*", ""))));
		copiedProperties.Add(new XElement("PackageLicenseExpression", properties.GetPropertyValue("PackageLicenseExpression", "MIT")));
		copiedProperties.Add(new XElement("PackageOutputPath", PackageOutputPath));
		copiedProperties.Add(new XElement("PackageIcon", Path.GetFileName(IconFile)));
		copiedProperties.Add(new XElement("GeneratePackageOnBuild", "true"));
		copiedProperties.Add(new XElement("IsPackable", "true"));
		copiedProperties.Add(new XElement("IsNuGetized", "true"));
		copiedProperties.Add(new XElement("Title", PackageId));
		copiedProperties.Add(new XElement("Summary", Description));
		copiedProperties.Add(new XElement("Authors", Authors));
		copiedProperties.Add(new XElement("Copyright", Copyright));
		copiedProperties.Add(new XElement("PackageTags", "using usings namespace nuget package " + PackageId));
		return copiedProperties.OrderBy(p => p.Name.ToString()).ToArray();
	}

	// private static XElement FormatUsing(MSBC.ProjectItemElement @using) => FormatUsing(((AnyOf<MSBC.ProjectItemElement, XElement>)@using));
	private static XElement FormatUsing(XElement @using) //=> FormatUsing(((AnyOf<MSBC.ProjectItemElement, XElement>)@using));
	//private static XElement FormatUsing(AnyOf<ProjectItemInstance, XElement> @using)
	{
		// var include = @using.GetAttributeValue("Include");
		// var alias = @using.GetAttributeValue("Alias");
		// var isStatic = @using.GetAttributeValue("Static");
		return new XElement("Using", GetReferenceAttributes(@using));
	}

	private static XAttribute[] GetReferenceAttributes(AnyOf<ProjectItemInstance, XElement> @ref) =>
		@ref.IsFirst? new[] { new XAttribute("Include", @ref.First.EvaluatedInclude) }.Concat(@ref.First.Metadata.Select(x => new XAttribute(x.Name, x.EvaluatedValue))).Distinct(Comparers).ToArray() :
			new[] { new XAttribute("Include", @ref.Second.GetAttributeValue("Include")) }.Concat(@ref.Second.Attributes().Select(x => new XAttribute(x.Name, x.Value))).Distinct(Comparers).ToArray();

	private static XElement FormatPackageReference(ProjectItemInstance @ref) => new XElement("PackageReference", GetReferenceAttributes(@ref));
	private static XElement FormatPackageReference(XElement @ref) => new XElement("PackageReference", GetReferenceAttributes(@ref));
	private static XElement FormatProjectReference(ProjectItemInstance @ref) => new XElement("ProjectReference", GetReferenceAttributes(@ref));
	private static XElement FormatProjectReference(XElement @ref) => new XElement("ProjectReference", GetReferenceAttributes(@ref));

	private static XElement FormatProperty(MSBEx.ProjectPropertyInstance property)
	{
		return new XElement(property.Name, property.EvaluatedValue);
	}

	private string FormatIsStatic(AnyOf<MSBC.ProjectItemElement, XElement> x)
	{
		var metadataValue = x.GetAttributeValue("Static");
		if (metadataValue?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false)
		{
			return " *(static)*";
		}
		return string.Empty;
	}

	private string FormatAlias(MSBC.ProjectItemElement x) => FormatAlias(((AnyOf<MSBC.ProjectItemElement, XElement>)x));
	private string FormatAlias(XElement x) => FormatAlias(((AnyOf<MSBC.ProjectItemElement, XElement>)x));
	private string FormatAlias(AnyOf<MSBC.ProjectItemElement, XElement> x)
	{
		var alias = x.IsFirst ? x.First.GetMetadataValue("Alias") : x.Second.GetAttributeValue("Alias");
		if (string.IsNullOrWhiteSpace(alias))
			return string.Empty;
		return $" (Alias: *{alias}*)";
	}
}
