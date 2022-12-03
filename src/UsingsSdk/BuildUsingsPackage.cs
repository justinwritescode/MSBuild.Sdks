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
#pragma warning disable
namespace MSBuild.UsingsSdk;

using XElementOrProjectItemInstance = AnyOf<System.Xml.Linq.XElement, Microsoft.Build.Execution.ProjectItemInstance>;
public partial class BuildUsingsPackage : MSBTask
{

    public virtual IEnumerable<ProjectTuple?>? Load(string? path)
    {
        if(path is null) return new[] { null as ProjectTuple? };
        var project = new ProjectInstance(path);
        var xDocumentProject = XDocument.Load(path);
        return new [] { new ProjectTuple(project, xDocumentProject) as ProjectTuple? }.Concat(xDocumentProject.Descendants("Import").SelectMany(x => Load(x.GetIncludeValue()).Where(p => p is not null)))
        .Where(x => x is not null);
    }

    private XElement[] MakeProperties()
    {
        var properties = AllProperties;
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
            new XElement("PublishRepositoryUrl", "true"),
            new XElement("PackageTags", "using usings namespace nuget package " + PackageId)
        };
        return copiedProperties.OrderBy(p => p.Name.ToString()).ToArray();
    }

    private static XAttribute[] GetReferenceAttributes(AnyOf<ProjectItemInstance, XElement> @ref, bool includeVersion = true) =>
        @ref.IsFirst? new[] { new XAttribute("Include", @ref.First.EvaluatedInclude) }.Concat(@ref.First.Metadata.Select(x => new XAttribute(x.Name, x.EvaluatedValue)).Where(x => includeVersion || x.Name.LocalName != "Version")).Distinct(Comparers).ToArray() :
            new[] { new XAttribute("Include", @ref.Second.GetAttributeValue("Include")) }.Concat(@ref.Second.Attributes().Select(x => new XAttribute(x.Name, x.Value)).Where(x => includeVersion || x.Name.LocalName != "Version")).Distinct(Comparers).ToArray();

    private static XElement FormatReference(ProjectItemTuple @ref, string type = "ProjectReference", bool includeVersion = true) =>
        new(type, GetReferenceAttributes(@ref.XItem, includeVersion).Concat(GetReferenceAttributes(@ref.Item, includeVersion)).Distinct(Comparers).ToArray());

    private static XElement FormatReference(XElement @ref, string type = "ProjectReference", bool includeVersion = true) =>
        new(type, GetReferenceAttributes(@ref).Distinct(Comparers).ToArray());

    private string FormatIsStatic(AnyOf<MSBC.ProjectItemElement, XElement> x)
    {
        var metadataValue = x.GetAttributeValue("Static");
        if (metadataValue?.Equals("true", StringComparison.InvariantCultureIgnoreCase) ?? false)
        {
            return " *(static)*";
        }
        return string.Empty;
    }

    private string GetNuGetUri(XElement @element) => $"https://www.nuget.org/packages/{@element.GetIncludeValue()}";


    private bool GetNuGetPackageExists(XElement @element) =>
         NuGetPackagesExistCache[@element.GetIncludeValue()] =
             NuGetPackagesExistCache.TryGetValue(@element.GetIncludeValue(), out var exists) ? exists : false;

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
