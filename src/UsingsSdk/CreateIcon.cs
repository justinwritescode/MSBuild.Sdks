// /*
//  * CreateIcon.cs
//  *
//  *   Created: 2022-11-12-08:52:03
//  *   Modified: 2022-11-12-08:54:31
//  *
//  *   Author: Justin Chase <justin@justinwritescode.com>
//  *
//  *   Copyright © 2022 Justin Chase, All Rights Reserved
//  *      License: MIT (https://opensource.org/licenses/MIT)
//  */

// namespace UsingsSdk;

// using System.Drawing;
// using System.Drawing.Imaging;
// public class CreateIcon : MSBTask
// {
//     [Required]
//     public string OutputPath {get; set;} = null!;
//     [Required]
//     public string PackageName { get; set; } = null!;

//     // from top: 275
//     // from left: 5
//     // width: 363
//     const int MaxWidth = 363;
//     const int MaxHeight = 75;

//     public override bool Execute()
//     {
//         var icon = Image.FromStream(typeof(CreateIcon).Assembly.GetManifestResourceStream("UsingsSdk.IconWithSpace.png"));
//         var fontSize = 10f;
//         var font = new Font("Arial", fontSize, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Pixel);
//         var graphics = Graphics.FromImage(icon);
//         SizeF size;
//         do
//         {
//             size = graphics.MeasureString(PackageName, font, new SizeF(MaxWidth, MaxHeight), StringFormat.GenericDefault, out var charactersFitted, out var linesFilled);
//             fontSize += 0.5f;
//             font = new Font("Arial", fontSize, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Pixel);
//         } while(size.Height < MaxHeight && size.Width < MaxWidth);
//         graphics.DrawString(PackageName, font, Brushes.Black, new PointF((373 - size.Width) / 2, 275));
//         icon.Save(OutputPath, ImageFormat.Png);
//         Log.LogMessage("Created icon at {0}", OutputPath);
//         return true;
//     }
// }
