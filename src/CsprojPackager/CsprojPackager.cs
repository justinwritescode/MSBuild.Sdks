/*
 * CsprojPackager.cs
 *
 *   Created: 2022-11-16-04:27:09
 *   Modified: 2022-11-26-03:15:02
 *
 *   Author: Justin Chase <justin@justinwritescode.com>
 *
 *   Copyright © 2022 Justin Chase, All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

using System.Diagnostics.CodeAnalysis;
using Microsoft.Build.Construction;

namespace CsprojPackager;

public class Packager : MSBTask, IEqualityComparer<ProjectUsingTaskElement>
{
    [MSBF.Required]
    public string ProjectFile { get; set; } = string.Empty;

    public string? OutputDirectory { get; set; } = null;

	public bool Equals(ProjectUsingTaskElement? x, ProjectUsingTaskElement? y)
		=> x.TaskName == y.TaskName && x.AssemblyFile == y.AssemblyFile && x.AssemblyName == y.AssemblyName;

	public override bool Execute()
    {
        var project = new MSBEx.ProjectInstance(ProjectFile);
        OutputDirectory ??= Path.GetDirectoryName(project.FullPath);
		var inputProjectRootElement = ProjectRootElement.Open(ProjectFile, new Microsoft.Build.Evaluation.ProjectCollection(), true);
		var usingTasks = inputProjectRootElement.UsingTasks.Distinct(this);

		var outputDirectory = new DirectoryInfo(OutputDirectory);

        if (!outputDirectory.Exists)
            outputDirectory.Create();

        var outputPath = Path.Combine(outputDirectory.FullName, project.GetPropertyValue("MSBuildProjectName") + ".Output.csproj");

        var outputProjectInstance = project.DeepCopy();
		var outputProjectRootElement = ProjectRootElement.Create(outputPath);
		foreach(var usingTask in usingTasks)
		{
			outputProjectRootElement.AddUsingTask(usingTask.TaskName, usingTask.AssemblyFile, usingTask.AssemblyName);
		}
		foreach(var prop in inputProjectRootElement.Properties)
		{
			outputProjectRootElement.AddProperty(prop.ElementName, prop.Value).CopyFrom(prop);
		}
		foreach(var projectItem in project.Items)
		{
			outputProjectRootElement.AddItem(projectItem.ItemType, projectItem.EvaluatedInclude, projectItem.Metadata.ToDictionary(x => x.Name, x => x.EvaluatedValue));
		}

		outputProjectRootElement.Save(outputPath);

        return true;
    }

	public int GetHashCode([DisallowNull] ProjectUsingTaskElement obj) => $"{obj.TaskName}{obj.AssemblyFile}{obj.AssemblyName}".GetHashCode();
}
