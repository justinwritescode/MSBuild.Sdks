// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var bup = new MSBuild.UsingsSdk.BuildUsingsPackage
{
    InputFile = Path.GetFullPath("./Test.usings")
};
bup.Execute();