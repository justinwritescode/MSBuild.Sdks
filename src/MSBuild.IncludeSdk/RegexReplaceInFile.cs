namespace JustinWritesCode.MSBuild.IncludeSdk;
using System.IO;

class RegexReplaceInFile : Microsoft.Build.Utilities.Task
{
    [Microsoft.Build.Framework.Required]
    public string File { get; set; }
    [Microsoft.Build.Framework.Required]
    public string OutputFile { get; set; }

    [Microsoft.Build.Framework.Required]
    public string Pattern { get; set; }

    [Microsoft.Build.Framework.Required]
    public string Replacement { get; set; }

    public override bool Execute()
    {
        var file = new FileInfo(File);
        var outputFile = new FileInfo(OutputFile);
        var text = System.IO.File.ReadAllText(file.FullName);
        var regex = new System.Text.RegularExpressions.Regex(Pattern);
        var newText = regex.Replace(text, Replacement);
        System.IO.File.WriteAllText(outputFile.FullName, newText);
        return true;
    }
}