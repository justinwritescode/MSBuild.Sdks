namespace JustinWritesCode.MSBuild.IncludeSdk;

public class RegexReplaceInFile : MSBTask
{
    [Required]
    public string InputFile { get; set; } = string.Empty;
    [Required]
    public string OutputFile { get; set; } = string.Empty;

    [Required]
    public string Pattern { get; set; } = string.Empty;

    [Required]
    public string Replacement { get; set; } = string.Empty;

    public override bool Execute()
    {
        var file = new FileInfo(InputFile);
        var outputFile = new FileInfo(OutputFile);
        var text = File.ReadAllText(file.FullName);
        var regex = new Regex(Pattern);
        var newText = regex.Replace(text, Replacement);
        File.WriteAllText(outputFile.FullName, newText);
        return true;
    }
}
