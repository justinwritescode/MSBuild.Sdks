/*
 * BuildUsingsPackage.GenerateMarkdown.cs
 *
 *   Created: 2022-12-01-03:00:37
 *   Modified: 2022-12-01-03:00:38
 *
 *   Author: Justin Chase <justin@justinwritescode.com>
 *
 *   Copyright © 2022-2023 Justin Chase, All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace MSBuild.UsingsSdk;

public partial class BuildUsingsPackage
{
    public string GenerateMarkdownReadme()
    {
        var markdownReadme = new StringBuilder();
        markdownReadme.AppendFormat("---{0}title: {1}{0}version: {2}{0}authors: {3}{0}copyright: {4}{0}description: {5}{0}date: {6}{0}---{0}{0}", NewLine, PackageId, Version, Authors, Copyright, Description, Now.ToString("yyyy-MM-dd HH:mm:ss"));
        markdownReadme.AppendLine();
        markdownReadme.AppendLine($"## {PackageId}");
        markdownReadme.AppendLine();
        markdownReadme.AppendLine(Description);

        markdownReadme.AppendLine();
        markdownReadme.AppendLine("### Usings");
        markdownReadme.AppendLine();
        markdownReadme.AppendLine(string.Join(NewLine, XUsings.Select(x => $"- {x.GetIncludeValue()}{FormatIsStatic(x)}{FormatAlias(x)}")));

        markdownReadme.AppendLine();
        markdownReadme.AppendLine("### Package References");
        markdownReadme.AppendLine();
        markdownReadme.AppendLine(string.Join(NewLine, XPackageReferences.Select(FormatPackageReferenceMarkdown)));

        markdownReadme.AppendLine();
        markdownReadme.AppendLine("### Project References");
        markdownReadme.AppendLine();
        markdownReadme.AppendLine(string.Join(NewLine, XProjectReferences.Select(x => $"- {x.GetIncludeValue()}")));

        return markdownReadme.ToString();
    }
}
