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
using System.Xml.Linq;
using MSBC = Microsoft.Build.Construction;
using MSBEx = Microsoft.Build.Execution;

public partial class CreateUsingsProject
{
    public class ComparersImplementation : IEqualityComparer<MSBEx.ProjectPropertyInstance>, IEqualityComparer<MSBEx.ProjectItemInstance>, IEqualityComparer<XElement>, IEqualityComparer<XAttribute>
	{
		public bool Equals(ProjectPropertyInstance? x, ProjectPropertyInstance? y)
		{
			return x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase) && x.EvaluatedValue.Equals(y.EvaluatedValue, StringComparison.OrdinalIgnoreCase);
		}

		public int GetHashCode(ProjectPropertyInstance? obj)
		{
			return obj.Name.GetHashCode() ^ obj.EvaluatedValue.GetHashCode();
		}

		public bool Equals(ProjectItemInstance? x, ProjectItemInstance? y)
		{
			return x.EvaluatedInclude.Equals(y.EvaluatedInclude, StringComparison.OrdinalIgnoreCase) && x.ItemType.Equals(y.ItemType, StringComparison.OrdinalIgnoreCase);
		}

		public int GetHashCode(ProjectItemInstance? obj)
		{
			return obj.EvaluatedInclude.GetHashCode() ^ obj.ItemType.GetHashCode();
		}

		public bool Equals(XElement? x, XElement? y)
		{
			return x.Name.Equals(y.Name) && x.GetAttributeValue("Include").Equals(y.GetAttributeValue("Include"));
		}

		public int GetHashCode(XElement? obj)
		{
			return obj.Name.GetHashCode() ^ obj.GetAttributeValue("Include").GetHashCode();
		}
		public bool Equals(XAttribute? x, XAttribute? y)
		{
			return x.Name.Equals(y.Name);
		}

		public int GetHashCode(XAttribute? obj)
		{
			return obj.Name.GetHashCode() ^ obj.Value.GetHashCode();
		}
	}
}
