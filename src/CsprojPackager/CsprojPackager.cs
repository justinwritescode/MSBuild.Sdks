// 
// CsprojPackager.cs
// 
//   Created: 2022-11-10-10:56:25
//   Modified: 2022-11-10-11:14:45
// 
//   Author: Justin Chase <justin@justinwritescode.com>
//   
//   Copyright © 2022 Justin Chase, All Rights Reserved
//      License: MIT (https://opensource.org/licenses/MIT)
// 

using System.Xml.Linq;
// 
// CsprojPackager.cs
// 
//   Created: 2022-11-10-10:56:25
//   Modified: 2022-11-10-10:56:25
// 
//   Author: Justin Chase <justin@justinwritescode.com>
//   
//   Copyright © 2022 Justin Chase, All Rights Reserved
//      License: MIT (https://opensource.org/licenses/MIT)
// 
namespace CsprojPackager;

public class Packager : MSBTask
{
    [MSBF.Required]
    public string ProjectFile { get; set; } = string.Empty;

    public string? OutputDirectory { get; set; } = null;

    public override bool Execute()
    {
        var project = new MSBEx.ProjectInstance(ProjectFile);
        OutputDirectory ??= Path.GetDirectoryName(project.FullPath);
        
        var outputDirectory = new DirectoryInfo(OutputDirectory);

        if (!outputDirectory.Exists)
            outputDirectory.Create();
        
        var outputPath = Path.Combine(outputDirectory.FullName, project.GetPropertyValue("MSBuildProjectName") + ".Output.csproj");

        var outputProject = project.DeepCopy();
        outputProject.ToProjectRootElement().Save(outputPath);

        return true;
    }
}
