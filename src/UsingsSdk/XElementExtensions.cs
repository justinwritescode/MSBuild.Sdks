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
using System.Linq;
using MSBC = Microsoft.Build.Construction;
using MSBEx = Microsoft.Build.Execution;
using System.Xml.Linq;

public static class XElementExtensions
{
	public static XAttribute GetAttribute(this XElement element, string name)
	{
		return element.Attributes().FirstOrDefault(x => x.Name.LocalName == name);
	}

	public static string? GetAttributeValue(this MSBC.ProjectItemElement element, string name) => GetAttributeValue(((AnyOf<MSBC.ProjectItemElement, XElement>)element), name);
	public static string? GetAttributeValue(this XElement element, string name) => GetAttributeValue(((AnyOf<MSBC.ProjectItemElement, XElement>)element), name);
	public static string? GetAttributeValue(this AnyOf<MSBC.ProjectItemElement, XElement> element, string name)
	{
		return element.IsFirst ? element.First.GetMetadataValue(name) : element.Second.GetAttribute(name)?.Value;
	}

	public static XElement[] GetItems(this XElement element, string name)
	{
		return element.Descendants(name).ToArray();
	}
	public static XElement[] GetXItems(this IEnumerable<(ProjectInstance? ProjectInstance, XDocument? XDocument)?> projects, string name)
	{
		return projects.SelectMany(x => x?.XDocument.Descendants(name)).Distinct(CreateUsingsProject.Comparers).OrderBy(x => x.GetAttributeValue("Include")).ToArray();
	}

	public static ProjectItemInstance[] GetItems(this IEnumerable<(ProjectInstance? ProjectInstance, XDocument? XDocument)?> projects, string name)
	{
		return projects.SelectMany(x => x?.ProjectInstance.GetItems(name)).Distinct(CreateUsingsProject.Comparers).OrderBy(x => x.GetMetadataValue("Include")).ToArray();
	}

	public static string? GetMetadataValue(this MSBC.ProjectItemElement @element, string name) => @element.Metadata.FirstOrDefault(x => x.Name?.Equals(name, StringComparison.OrdinalIgnoreCase) ?? false)?.Value;

	public static string GetPropertyValue(this IEnumerable<MSBEx.ProjectPropertyInstance> properties, string name, string? defaultValue = null)
	{
		return properties.FirstOrDefault(x => x.Name?.Equals(name, StringComparison.OrdinalIgnoreCase) ?? false)?.EvaluatedValue ?? defaultValue ?? string.Empty;
	}
}
