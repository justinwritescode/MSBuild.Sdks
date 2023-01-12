/*
 * Tuples.cs
 *
 *   Created: 2022-12-01-03:26:05
 *   Modified: 2022-12-01-03:26:06
 *
 *   Author: Justin Chase <justin@justinwritescode.com>
 *
 *   Copyright © 2022-2023 Justin Chase, All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

public record struct ProjectItemTuple(XElement XItem, ProjectItemInstance Item);

public record struct ProjectTuple(ProjectInstance? ProjectInstance, XDocument? XDocument);
