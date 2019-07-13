﻿using System.IO;
using System.Linq;

using TASVideos.MovieParsers.Extensions;
using TASVideos.MovieParsers.Result;

namespace TASVideos.MovieParsers.Parsers
{
	[FileExtension("dsm")]
	internal class Dsm : IParser
	{
		private const string FileExtension = "dsm";

		public IParseResult Parse(Stream file)
		{
			using (var reader = new StreamReader(file))
			{
				var result = new ParseResult
				{
					Region = RegionType.Ntsc,
					FileExtension = FileExtension,
					SystemCode = SystemCodes.Ds
				};

				var lines = reader.ReadToEnd().LineSplit();
				var header = lines
					.WithoutPipes()
					.ToArray();

				result.Frames = lines.PipeCount();

				int? rerecordVal = header.GetValueFor(Keys.RerecordCount).ToInt();
				if (rerecordVal.HasValue)
				{
					result.RerecordCount = rerecordVal.Value;
				}
				else
				{
					result.WarningList.Add(ParseWarnings.MissingRerecordCount);
				}

				if (header.GetValueFor(Keys.StartsFromSavestate).Length > 1)
				{
					result.StartType = MovieStartType.Savestate;
				}

				if (header.GetValueFor(Keys.StartsFromSram).Length > 1)
				{
					result.StartType = MovieStartType.Sram;
				}

				return result;
			}
		}

		private static class Keys
		{
			public const string RerecordCount = "rerecordcount";
			public const string StartsFromSavestate = "savestate";
			public const string StartsFromSram = "sram";
		}
	}
}