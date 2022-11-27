// 
// custom-commands.ts
// 
//   Created: 2022-11-03-02:34:46
//   Modified: 2022-11-03-02:34:47
// 
//   Author: Justin Chase <justin@justinwritescode.com>
//   
//   Copyright © 2022 Justin Chase, All Rights Reserved
//      License: MIT (https://opensource.org/licenses/MIT)
// 

import * as VSCode from 'vscode';
import * as childProcess from "child_process";
//const vscode = require('vscode');


type editorProps = { editor: VSCode.TextEditor, document: VSCode.TextDocument, selection: VSCode.Selection, textRange: VSCode.Range, editorText, selectedText:VSCode.Selection, variables:variables }  // ! Can be an empty object
type variables = {
      workspaceFolder:string,
      workspaceFolderBasename:string,
      currentFile:string,
      file:string,
      fileWorkspaceFolder:string,
      relativeFile:string,
      relativeFileDirname:string,
      fileBasename:string,
      fileBasenameNoExtension:string,
      fileDirname:string,
      fileExtname:string,
      pathSeparator:string
}; // predefined variables which can also be used to run shell commands

exports.Copy_Text = async (vscode, args, editorProps, output) => {
  let currentFilePath = args?.fsPath // ! Can be undefined
  const { editor, document, selection, textRange, editorText, selectedText, variables } = editorProps // ! Can be an empty object

const {
    workspaceFolder,
    workspaceFolderBasename,
    currentFile,
    file,
    fileWorkspaceFolder,
    relativeFile,
    relativeFileDirname,
    fileBasename,
    fileBasenameNoExtension,
    fileDirname,
    fileExtname,
    pathSeparator
  } = variables; // predefined variables which can also be used to run shell commands
  
  vscode.env.clipboard.writeText("Copy Text");
  vscode.window.showInformationMessage('Copied command to the clipboard 📋');
  
  output.appendLine("Copied command to the clipboard 📋") // this will be logged under `Custom Commands Log` output panel in vscode
}

exports.Npm_Version = "npm -v" // direct string value will be executed as a shell commands

exports.Run_File = "node ${currentFile}" // can use predefined variables

// exports.Generate_Solution = async(vscode, args, editorProps, output) => {
//     vscode.window.showInformationMessage('Generating solution...');
//     output.appendLine("Generating solution...") // this will be logged under `Custom Commands Log` output panel in vscode
//     const { editor, document, selection, textRange, editorText, selectedText, variables } = editorProps // ! Can be an empty object
//     const { workspaceFolder, workspaceFolderBasename, currentFile, file, fileWorkspaceFolder, relativeFile, relativeFileDirname, fileBasename, fileBasenameNoExtension, fileDirname, fileExtname, pathSeparator } = variables; // predefined variables which can also be used to run shell commands
//     const { exec } = require("child_process");
//     return `slngen ${file}`; // run shell command
// }

exports.Generate_Solution = "slngen ${file}"; // run shell command

// exports.Build_Local = "cd ${fileDirname}; slngen ${file}; dotnet build -c:Local ${file}"; // run shell command

exports.Build_Local = "cd ${fileDirname}; slngen ${file}; dotnet build -c:Local ${file}"; /*async (vscode, args, editorProps, output) => {
  output.appendLine("Building solution...") // this will be logged under `Custom Commands Log` output panel in vscode
  vscode.window.showInformationMessage(`Building project ${editorProps.variables.file}...`);
  var terminal = VSCode.window.createTerminal({
    name: "Build Local",
    cwd: "${fileDirname}"
  });
  terminal.sendText("cd ${fileDirname}; slngen ${file}; dotnet build -c:Local ${file}");
  terminal.show();
  //vscode.window.terminals[0].sendText("cd ${fileDirname}; slngen ${file}; dotnet build -c:Local ${file}");
}*/

//export const Build_Project_Local = //async  (vscode, args, editorProps, output)=> {
    //vscode.window.showInformationMessage("Getting projects...");
    // const project = await vscode.window.showQuickPick(await getProjects(editorProps.variables.workspaceFolder));
    // vscode.window.showInformationMessage(`You selected ${project}`);
    // output.appendLine("Building project...") // this will be logged under `Custom Commands Log` output panel in vscode
    //   // args[0] = "dotnet build -c:Local ${file}";
//};

// vscode.window.showInformationMessage('Building project...');

// async function getProjects(workspaceFolder): Promise<string[]> {
//   const result = childProcess.execSync("ls **/*proj", {stdio:"pipe", "cwd": workspaceFolder}).toString();
//   return result.split("\n");
// }

// VSCode.window.
