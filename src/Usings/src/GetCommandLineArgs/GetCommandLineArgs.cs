/*
 * GetCommandLineArgs.cs
 *
 *   Created: 2022-11-12-04:15:12
 *   Modified: 2022-11-19-04:04:29
 *
 *   Author: Justin Chase <justin@justinwritescode.com>
 *
 *   Copyright © 2022 Justin Chase, All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */


// Taken from https://stackoverflow.com/questions/3260913/how-to-access-the-msbuild-command-line-parameters-from-within-the-project-file-b
using System;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
namespace MSBuild.Utils;
public sealed class GetCommandLineArgs : MSBTask
{
	[Output]
	public ITaskItem[] CommandLineArgs { get; private set; } = Array.Empty<ITaskItem>();
	[Output]
	public string CommandLine { get; private set; } = string.Empty;

	public override bool Execute()
	{
		CommandLineArgs = Environment.GetCommandLineArgs().Skip(1).Select(a => new TaskItem(a)).ToArray();
		CommandLine = Environment.CommandLine;
		return true;
	}
}
