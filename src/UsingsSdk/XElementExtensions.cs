/*
 * XElementExtensions.cs
 *
 *   Created: 2022-11-27-05:39:27
 *   Modified: 2022-12-01-03:23:39
 *
 *   Author: Justin Chase <justin@justinwritescode.com>
 *
 *   Copyright © 2022 Justin Chase, All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace MSBuild.UsingsSdk;

using System.Configuration;
using System.Linq;
using System.Xml.Linq;
#pragma warning disable
using MSBC = Microsoft.Build.Construction;
using MSBEx = Microsoft.Build.Execution;
using XElementOrProjectItemInstance = AnyOf<System.Xml.Linq.XElement, ProjectItemInstance>;

public static class XElementExtensions
{
    public static XAttribute? GetAttribute(this XElement element, string name)
     => element.Attributes().FirstOrDefault(x => x.Name.LocalName == name);

    public static string? GetAttributeValue(this MSBC.ProjectItemElement element, string name)
        => GetAttributeValue((AnyOf<MSBC.ProjectItemElement, XElement>)element, name);
    public static string? GetAttributeValue(this XElement element, string name)
        => GetAttributeValue((AnyOf<MSBC.ProjectItemElement, XElement>)element, name);
    public static string? GetAttributeValue(this AnyOf<MSBC.ProjectItemElement, XElement> element, string name)
        => element.IsFirst ? element.First.GetMetadataValue(name) : element.Second.GetAttribute(name)?.Value;
    public static string? GetAttributeValue(this XElementOrProjectItemInstance element, string name)
        => element.IsFirst ? element.First.GetAttribute(name)?.Value : element.Second.GetMetadataValue(name);

    public static XElement[] GetItems(this XElement element, string name)
         => element.Descendants(name).ToArray();
    public static XElement[] GetXItems(this IEnumerable<ProjectTuple?> projects, string name)
        => projects.SelectMany(x => x?.XDocument.Descendants(name)!).Distinct(BuildUsingsPackage.Comparers)
            .OrderBy(x => x?.GetAttributeValue("Include"))
            .ToArray();

    public static ProjectItemInstance[]? GetItems(this IEnumerable<ProjectTuple?> projects, string name)
         => projects.SelectMany(x => x?.ProjectInstance.GetItems(name)!).Distinct(BuildUsingsPackage.Comparers).OrderBy(x => x.GetMetadataValue("Include")).ToArray();

    public static string? GetMetadataValue(this MSBC.ProjectItemElement @element, string name)
        => @element.Metadata.FirstOrDefault(x => x.Name?.Equals(name, StringComparison.InvariantCultureIgnoreCase) ?? false)?.Value;

    public static string GetPropertyValue(this IEnumerable<ProjectPropertyInstance> properties, string name, string? defaultValue = null)
        => properties.FirstOrDefault(x => x.Name?.Equals(name, StringComparison.InvariantCultureIgnoreCase) ?? false)?.EvaluatedValue
            ?? defaultValue ?? string.Empty;

    public static string? GetIncludeValue(this MSBC.ProjectItemElement @element) => @element.GetMetadataValue("Include");
    public static string? GetIncludeValue(this ProjectItemInstance @element) => @element.GetMetadataValue("Include");
    public static string? GetIncludeValue(this XElement @element) => @element.GetAttributeValue("Include");

    public static BuildUsingsPackage.ComparersImplementation ComparersImplementation = new BuildUsingsPackage.ComparersImplementation();

    public static ProjectItemTuple[] Join(this IEnumerable<XElement> xElements, IEnumerable<ProjectItemInstance> projectItems)
        => (from xElement in xElements from projectItem in projectItems
           select
            ComparersImplementation.Equals(xElement, projectItem) ?
            new ProjectItemTuple(xElement, projectItem) :
            new ProjectItemTuple(xElement, null)).ToArray();

        //xElements.Join(
        //    projectItems,
        //    x => (XElementOrProjectItemInstance)x,
        //    x => (XElementOrProjectItemInstance)x,
        //    (xPackageReference, packageReference) => new ProjectItemTuple(xPackageReference, packageReference),
        //    new BuildUsingsPackage.ComparersImplementation())
        //    .ToArray();
}
