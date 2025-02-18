﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TASVideos.Legacy.Data.Forum.Entity
{
	public class Forums
	{
		[Key]
		[Column("forum_id")]
		public int Id { get; set; }

		[Column("cat_id")]
		public int CategoryId { get; set; }

		// Note: this column is nullable in the database, but will never actually be null
		[Column("forum_name")]
		public string Name { get; set; } = "";

		[Column("forum_desc")]
		public string? Description { get; set; }

		[Column("forum_order")]
		public int Order { get; set; }

		[Column("forum_shortname")]
		public string ShortName { get; set; } = "";

		[Column("auth_view")]
		public int AuthView { get; set; }
	}
}
