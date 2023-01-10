/*
 * RegisterProject.cs
 *
 *   Created: 2022-12-19-10:25:18
 *   Modified: 2022-12-19-10:25:18
 *
 *   Author: Justin Chase <justin@justinwritescode.com>
 *
 *   Copyright © 2022-2023 Justin Chase, All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using MSBuild.Extensions;
using System.IO;
using System.Text;

public partial class ConvertProjectReferencesToPackageReferences : MSBTask
{
    protected string ProjectFile => this.BuildEngine9.ProjectFileOfTaskNode;

    public override bool Execute()
    {
        var project = this.TryGetProjectInstance();
        var projectReferneces = project.GetItems("ProjectReference");
        var packageReferences = project.GetItems("PackageReference");
        var projectReferencesWithPropsAndTargetsFiles =
            projectReferneces.Select(x =>
                (ProjectFilePath : x.EvaluatedInclude,
                PropsFilePath : x.EvaluatedInclude.Replace(".csproj", ".props"),
                TargetsFilePath : x.EvaluatedInclude.Replace(".csproj", ".targets")));
        var projectReferencesWithPropsAndTargetsFilesComplete =
            projectReferencesWithPropsAndTargetsFiles.Select(x =>
            (x.ProjectFilePath, x.PropsFilePath, x.TargetsFilePath,
            PropsFileExists: File.Exists(x.PropsFilePath), TargetsFileExists: File.Exists(x.TargetsFilePath)));

        var fileOutput = new StringBuilder();
        fileOutput.Append(
        $"""""
            <Project>
                <ItemGroup>
                    <ProjectReference Remove="@(ProjectReference)" />
                    {string.Join(Environment.NewLine, projectReferencesWithPropsAndTargetsFilesComplete.Select(x =>
                        $"<ProjectReference Include=\"{x.ProjectFilePath}\" />"))}
                </ItemGroup>
            </Project>
        """""
        );

        return true;
    }
}
