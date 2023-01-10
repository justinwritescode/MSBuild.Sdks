/*
 * ProjectFilePackager.cs
 *
 *   Created: 2022-12-03-04:24:33
 *   Modified: 2022-12-03-04:24:34
 *
 *   Author: Justin Chase <justin@justinwritescode.com>
 *
 *   Copyright © 2022-2023 Justin Chase, All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace EmbedProjectFile;
using MSBuild.Extensions;
using Microsoft.Build.Tasks;

    public class EmbedProjectFile : MSBTask
    {
        public override bool Execute()
        {
            var project = this.TryGetProjectInstance();
            var duplicateProject = project.DeepCopy();
            duplicateProject.ToProjectRootElement().Save(Path.Combine(project.GetPropertyValue("IntermediateOutputPath"), "project.csproj"));
            Log.LogMessage("High", $"Embedded project file at {Path.Combine(project.GetPropertyValue("IntermediateOutputPath"), "project.csproj")}");
            return true;
        }
    }
