//
// CreateUsingsProject.cs
//
//   Created: 2022-11-12-08:52:03
//   Modified: 2022-11-12-03:59:07
//
//   Author: Justin Chase <justin@justinwritescode.com>
//
//   Copyright © 2022-2023 Justin Chase, All Rights Reserved
//      License: MIT (https://opensource.org/licenses/MIT)
//
#pragma warning disable
namespace MSBuild.UsingsSdk;
using System.Xml.Linq;
using Microsoft.Build.Utilities;
using MSBC = Microsoft.Build.Construction;
using MSBEx = Microsoft.Build.Execution;
using ProjectItemInstance = Microsoft.Build.Execution.ProjectItemInstance;
using XElement = System.Xml.Linq.XElement;
using XElementOrProjectItemInstance = AnyOf<
    System.Xml.Linq.XElement,
    Microsoft.Build.Execution.ProjectItemInstance
>;

public partial class BuildUsingsPackage
{
    public class ComparersImplementation
        : IEqualityComparer<MSBEx.ProjectPropertyInstance>,
            IEqualityComparer<MSBEx.ProjectItemInstance>,
            IEqualityComparer<XElement>,
            IEqualityComparer<XAttribute>,
            IEqualityComparer<XElementOrProjectItemInstance>
    {
        public static TaskLog? Logger { get; set; }

        public bool Equals(ProjectPropertyInstance? x, ProjectPropertyInstance? y) =>
            x.Name.Equals(y.Name, StringComparison.InvariantCultureIgnoreCase)
            && x.EvaluatedValue.Equals(
                y.EvaluatedValue,
                StringComparison.InvariantCultureIgnoreCase
            );

        public int GetHashCode(ProjectPropertyInstance? obj) =>
            obj.Name.GetHashCode() ^ obj.EvaluatedValue.GetHashCode();

        public bool Equals(ProjectItemInstance? x, ProjectItemInstance? y) =>
            GetComparisonString(x)
                .Equals(GetComparisonString(y), StringComparison.InvariantCultureIgnoreCase);

        public int GetHashCode(ProjectItemInstance? obj) =>
            obj.EvaluatedInclude.GetHashCode()
            ^ obj.ItemType.GetHashCode()
            ^ string.Join(",", obj.MetadataNames).GetHashCode();

        public bool Equals(XElementOrProjectItemInstance x, XElementOrProjectItemInstance y) =>
            GetComparisonString(x)
                .Equals(GetComparisonString(y), StringComparison.InvariantCultureIgnoreCase);

        public static string GetComparisonString(XElementOrProjectItemInstance x)
        {
            var @string =
                x.GetAttributeValue("Include")
                + (
                    x.GetAttributeValue("Static")
                        ?.Equals("true", StringComparison.InvariantCultureIgnoreCase) ?? false
                        ? " ( Static)"
                        : ""
                )
                + (
                    !string.IsNullOrEmpty(x.GetAttributeValue("Alias"))
                        ? $" (Alias: {x.GetAttributeValue("Alias")})"
                        : ""
                );

            //Logger?.LogMessage($"GetComparisonString ({(x.IsFirst ? "XElement" : "ProjectItemInstance")} {x}): {@string}");
            return @string;
        }

        public bool Equals(XElement? x, XElement? y) =>
            GetComparisonString(x)
                .Equals(GetComparisonString(y), StringComparison.InvariantCultureIgnoreCase);

        public int GetHashCode(XElement? obj) => GetComparisonString(obj).GetHashCode();

        public bool Equals(XAttribute? x, XAttribute? y) =>
            x.Name.ToString().Equals(y.Name.ToString(), StringComparison.InvariantCultureIgnoreCase)
            && (
                x.Value
                    ?.ToString()
                    ?.Equals(y.Value?.ToString(), StringComparison.InvariantCultureIgnoreCase)
                ?? false
            );

        public int GetHashCode(XAttribute? obj) => (obj.Name + obj.Value).GetHashCode();

        int IEqualityComparer<XElementOrProjectItemInstance>.GetHashCode(
            XElementOrProjectItemInstance obj
        ) => obj.IsFirst ? GetHashCode(obj.First) : GetHashCode(obj.Second);
    }
}
