using System;
namespace GameBooster.Core.Entities
{
	public class BaseEntity
	{
		public int Id { get; set; }

		public DateTime CreatedDate { get; set; } = DateTime.Now;	

	}
}

