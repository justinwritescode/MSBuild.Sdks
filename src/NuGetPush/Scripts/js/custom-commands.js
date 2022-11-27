"use strict";
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
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
exports.__esModule = true;
exports.Copy_Text = function (vscode, args, editorProps, output) { return __awaiter(void 0, void 0, void 0, function () {
    var currentFilePath, editor, document, selection, textRange, editorText, selectedText, variables, workspaceFolder, workspaceFolderBasename, currentFile, file, fileWorkspaceFolder, relativeFile, relativeFileDirname, fileBasename, fileBasenameNoExtension, fileDirname, fileExtname, pathSeparator;
    return __generator(this, function (_a) {
        currentFilePath = args === null || args === void 0 ? void 0 : args.fsPath // ! Can be undefined
        ;
        editor = editorProps // ! Can be an empty object
        .editor, document = editorProps // ! Can be an empty object
        .document, selection = editorProps // ! Can be an empty object
        .selection, textRange = editorProps // ! Can be an empty object
        .textRange, editorText = editorProps // ! Can be an empty object
        .editorText, selectedText = editorProps // ! Can be an empty object
        .selectedText, variables = editorProps // ! Can be an empty object
        .variables;
        workspaceFolder = variables.workspaceFolder, workspaceFolderBasename = variables.workspaceFolderBasename, currentFile = variables.currentFile, file = variables.file, fileWorkspaceFolder = variables.fileWorkspaceFolder, relativeFile = variables.relativeFile, relativeFileDirname = variables.relativeFileDirname, fileBasename = variables.fileBasename, fileBasenameNoExtension = variables.fileBasenameNoExtension, fileDirname = variables.fileDirname, fileExtname = variables.fileExtname, pathSeparator = variables.pathSeparator;
        vscode.env.clipboard.writeText("Copy Text");
        vscode.window.showInformationMessage('Copied command to the clipboard 📋');
        output.appendLine("Copied command to the clipboard 📋"); // this will be logged under `Custom Commands Log` output panel in vscode
        return [2 /*return*/];
    });
}); };
exports.Npm_Version = "npm -v"; // direct string value will be executed as a shell commands
exports.Run_File = "node ${currentFile}"; // can use predefined variables
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
