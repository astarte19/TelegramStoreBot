using System;
using Microsoft.EntityFrameworkCore;
namespace BotFFlowers
{
	[Keyless]
	public class Product
	{
		public string Img { get; set; }
		public string Text { get; set; }
		public string Price { get; set; }

		
	}
}

