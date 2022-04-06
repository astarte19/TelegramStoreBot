using System;
namespace BotFFlowers
{
	public class Item
	{
		public string UrlImg { get; set; }
		public string Name { get; set; }
		public string Price { get; set; }

		public Item(string name, string price)
        {
			Name = name;
			Price = price;
			
        }
	}
}

