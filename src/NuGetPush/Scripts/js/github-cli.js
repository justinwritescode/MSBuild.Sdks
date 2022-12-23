"use strict";
/*
 * github-cli.ts
 *
 *   Created: 2022-11-27-05:39:27
 *   Modified: 2022-12-05-04:14:53
 *
 *   Author: Justin Chase <justin@justinwritescode.com>
 *
 *   Copyright © 2022 Justin Chase, All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */
var __createBinding = (this && this.__createBinding) || (Object.create ? (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    var desc = Object.getOwnPropertyDescriptor(m, k);
    if (!desc || ("get" in desc ? !m.__esModule : desc.writable || desc.configurable)) {
      desc = { enumerable: true, get: function() { return m[k]; } };
    }
    Object.defineProperty(o, k2, desc);
}) : (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    o[k2] = m[k];
}));
var __setModuleDefault = (this && this.__setModuleDefault) || (Object.create ? (function(o, v) {
    Object.defineProperty(o, "default", { enumerable: true, value: v });
}) : function(o, v) {
    o["default"] = v;
});
var __importStar = (this && this.__importStar) || function (mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (k !== "default" && Object.prototype.hasOwnProperty.call(mod, k)) __createBinding(result, mod, k);
    __setModuleDefault(result, mod);
    return result;
};
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
exports.deletePackageAsync = exports.deletePackageVersionAsync = void 0;
var child_process_1 = require("child_process");
var fs = __importStar(require("fs"));
var process = __importStar(require("process"));
function deletePackageVersionAsync(packageId, version) {
    return __awaiter(this, void 0, void 0, function () {
        var _this = this;
        return __generator(this, function (_a) {
            return [2 /*return*/, new Promise(function (resolve, reject) { return __awaiter(_this, void 0, void 0, function () {
                    var deleteResult, versionsResult, versionId, jsonOutput, deleteVersionResult, ex_1, ex_2;
                    var _a;
                    return __generator(this, function (_b) {
                        switch (_b.label) {
                            case 0:
                                deleteResult = undefined;
                                try {
                                    (0, child_process_1.execSync)("curl -H \"Authorization: Bearer ".concat(process.env.GIT_TOKEN, "\" -H \"Accept: application/vnd.github+json\" \"https://api.github.com/user/packages/nuget/").concat(packageId, "/versions\" 1> ").concat(packageId, ".versions.json 2> /dev/null"), { encoding: 'utf8', stdio: 'pipe' });
                                    deleteResult = fs.readFileSync(" ".concat(packageId, ".versions.json"), 'utf8');
                                    deleteResult = JSON.parse(deleteResult);
                                }
                                catch (ex) {
                                    // console.log(ex);
                                    //reject(ex);
                                    resolve();
                                }
                                versionsResult = JSON.parse(fs.readFileSync("".concat(packageId, ".versions.json"), 'utf8'));
                                if (!(versionsResult instanceof Array)) return [3 /*break*/, 9];
                                versionId = (_a = versionsResult.find(function (v) { return v.name === version; })) === null || _a === void 0 ? void 0 : _a.id;
                                if (!(versionId && versionId != undefined)) return [3 /*break*/, 8];
                                _b.label = 1;
                            case 1:
                                _b.trys.push([1, 7, , 8]);
                                jsonOutput = (0, child_process_1.execSync)("curl -X DELETE -H \"Authorization: Bearer ".concat(process.env.GIT_TOKEN, "\" -H \"Accept: application/vnd.github+json\" \"https://api.github.com/user/packages/nuget/").concat(packageId, "/versions/").concat(versionId, "\" 1> ").concat(packageId, ".delete-result.json 2> /dev/null"), { encoding: 'utf8', stdio: 'pipe' });
                                _b.label = 2;
                            case 2:
                                _b.trys.push([2, 5, , 6]);
                                deleteVersionResult = JSON.parse(jsonOutput);
                                if (!((deleteVersionResult === null || deleteVersionResult === void 0 ? void 0 : deleteVersionResult.message) == "You cannot delete the last version of a package. You must delete the package instead.")) return [3 /*break*/, 4];
                                return [4 /*yield*/, deletePackageAsync(packageId)];
                            case 3:
                                _b.sent();
                                _b.label = 4;
                            case 4:
                                console.log("deleteVersionResult: ".concat(deleteVersionResult, ": deleteVersionResult"));
                                return [3 /*break*/, 6];
                            case 5:
                                ex_1 = _b.sent();
                                // console.log(ex);
                                // reject(ex);
                                resolve();
                                return [3 /*break*/, 6];
                            case 6:
                                resolve();
                                return [3 /*break*/, 8];
                            case 7:
                                ex_2 = _b.sent();
                                // console.log(ex);
                                resolve();
                                return [3 /*break*/, 8];
                            case 8: return [3 /*break*/, 10];
                            case 9:
                                if (versionsResult.message == "Not Found" || versionsResult.message == "Package not found.") {
                                    console.log("Package ".concat(packageId, " with version number ").concat(version, " not found.  Skipping..."));
                                    resolve();
                                }
                                _b.label = 10;
                            case 10:
                                if (fs.existsSync("".concat(packageId, ".versions.json")))
                                    fs.unlinkSync("".concat(packageId, ".versions.json"));
                                if (fs.existsSync("".concat(packageId, ".delete-result.json")))
                                    fs.unlinkSync("".concat(packageId, ".delete-result.json"));
                                return [2 /*return*/];
                        }
                    });
                }); })];
        });
    });
}
exports.deletePackageVersionAsync = deletePackageVersionAsync;
function deletePackageAsync(packageId) {
    return new Promise(function (resolve, reject) {
        var deletePackageResultJsonString = "";
        console.log("Deleting package ".concat(packageId, "..."));
        try {
            deletePackageResultJsonString = (0, child_process_1.execSync)("curl -X DELETE -H \"Authorization: Bearer ".concat(process.env.GIT_TOKEN, "\" -H \"Accept: application/vnd.github+json\" \"https://api.github.com/user/packages/nuget/").concat(packageId, "\" &> ").concat(packageId, ".delete-package.result.json"), { encoding: 'utf8' });
        }
        catch (ex) {
            resolve();
        }
        var deletePackageResult = JSON.parse(deletePackageResultJsonString);
        if (deletePackageResult.message == "Not Found" || deletePackageResult.message == "Package not found.") {
            console.log("Package ".concat(packageId, " not found.  Skipping..."));
        }
        if (fs.existsSync("".concat(packageId, ".delete-package.result.json")))
            fs.unlinkSync("".concat(packageId, ".delete-package.result.json"));
        resolve();
    });
}
exports.deletePackageAsync = deletePackageAsync;
