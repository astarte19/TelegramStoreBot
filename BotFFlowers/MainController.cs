using System;
using Deployf.Botf;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;

namespace BotFFlowers
{
	public class MainController : BotControllerBase
	{
		private static TelegramBotClient Bot = new TelegramBotClient("5249074040:AAGjwQxQHo17Ut6ychH50QMHmgEwyndUbZo");
		List<string> prices = new List<string>();
		List<string> titles = new List<string>();
		List<string> urls = new List<string>();

		List<Item> shop_cart = new List<Item>();
		

		string baseurl = "https://flowerskamensk.ru/products/category/";
		string header_tulps = "tulpany";
		string header_roses = "rossiyskie-rozy";
		string header_boxes = "cveti-v-korobkah";
		string header_bouqets = "bukety";
		string header_baskets = "korziny";
		string header_toys = "plushevie-mishki";
		string header_baloons = "vozdushnie-shary";
		string header_candy = "konfety";
		string header_cakes = "torty";
		string header_fruits = "frukty-v-korzine";
		string header_postcards = "otkrytki";
		

		[Action("/start", "Главное меню")]
		public void Start()
        {
			
			PushL($"✋ <b>Привет, {Context.GetUserFullName()}!</b>\n🌷 <b>Городские цветы Каменск-Шахтинский</b> \n🟢 Самые свежие цветы и букеты! \n🟢 Более 8 лет опыта и репутации! \n🟢 Наш <i>telegram</i> канал: <a href='https://t.me/gorodskie_cveti_kamensk'>Городские Цветы Каменск</a>");
			RowButton("🌷 1. Тюльпаны", Q(PressTulps));
			RowButton("🌹 2. Российские Розы", Q(PressRURoses));
			RowButton("🌸 3. Цветы в коробках", Q(PressBoxes));
			RowButton("💐 4. Букеты", Q(PressBouqets));
			RowButton("🧺 5. Корзины", Q(PressBaskets));
			RowButton("🧸 6. Мягкие игрушки", Q(PressToys));
			RowButton("🎈 7. Воздушные шары", Q(PressBallons));
			RowButton("🍬 8. Конфеты", Q(PressCandy));
			RowButton("🎂 9. Торты", Q(PressCakes));
			RowButton("🍏 10. Фрукты", Q(PressFruits));
			RowButton("🗾 11. Открытки", Q(PressPostcards));
			RowButton("⭐ Наши оценки", Q(PressRate));
			RowButton("🛒 Корзина", Q(PressMainBasket));
			Button("🚚 Доставка", Q(PressDelivery));
			Button("📱 Контакты", Q(PressContact));


			
			
		}
		
		[Action]
		public void PressContact()
        {

			Bot.SendPhotoAsync(ChatId, "https://i.siteapi.org/jZcycCnxSz_otO-zGfPlcmFy0nc=/fit-in/330x/top/s.siteapi.org/ac20a296e8e485f.ru/img/at32995njz4ksckckw88gkg0gcosgs", "📱 <b>Контакты</b>\n📍 <b>Адрес:</b>\n пр.Карла Маркса, 54г.Каменск-Шахтинский\n(Режим работы: Круглосуточно)\n📍 <b>Адрес:</b>\n пр.Карла Маркса, 79, Каменск-Шахтинский\n(Режим работы: 7:00-20:00)\n📞 <b>Телефоны:</b>\n +7-928-180-63-88\n +7-918-576-10-88\n📧 <b>E-mail:</b>\n flowerskamensk@mail.ru\n🌐 <b>Сайт:</b>\n https://flowerskamensk.ru/\n📲 <b>Whatsapp:</b>\n +7-928-180-63-88\n🕰 <b>Прием заказов:</b>\n с 8:00-22:00", Telegram.Bot.Types.Enums.ParseMode.Html);
			
		}

		//Buttons Categories
		
		//Тюльпаны
		[Action]
		public void PressTulps()
        {
			PushL("<b>Ценовые категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			
			RowButton("🟩 До 1500 рублей 🟩", Q(PushItem,header_tulps,0,1500));
			RowButton("🟩 От 1500 До 2500 рублей 🟩", Q(PushItem,header_tulps,1500,2500));
			RowButton("🟩 От 2500 До 3500 рублей 🟩", Q(PushItem,header_tulps,2500,3500));
			RowButton("🟩 От 3500 До 5000 рублей 🟩", Q(PushItem,header_tulps,3500,5000));
			RowButton("🟩 5000 рублей и выше 🟩", Q(PushItem,header_tulps,5000,999999));


		}

		
		//Российские розы
		[Action]
		public void PressRURoses()
        {
			PushL("<b>Ценовые категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			
			RowButton("🟩 От 1600 До 2500 рублей 🟩", Q(PushItem,header_roses,1600,2500));
			RowButton("🟩 От 2500 До 3500 рублей 🟩", Q(PushItem,header_roses,2500,3500));
			RowButton("🟩 От 3500 До 5000 рублей 🟩", Q(PushItem,header_roses,3500,5000));
			RowButton("🟩 5000 рублей и выше 🟩", Q(PushItem,header_roses,5000,999999));
			
		}

		
		//Цветы в коробках
		[Action]
		public void PressBoxes()
        {
			PushL("<b>Ценовые категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			
			RowButton("🟩 От 1600 До 2500 рублей 🟩", Q(PushItem,header_boxes,1600,2500));
			RowButton("🟩 От 2500 До 3500 рублей 🟩", Q(PushItem,header_boxes,2500,3500));
			RowButton("🟩 От 3500 До 5000 рублей 🟩", Q(PushItem,header_boxes,3500,5000));
			RowButton("🟩 5000 рублей и выше 🟩", Q(PushItem,header_boxes,5000,999999));
			
		}


		//Букеты
		[Action]
		public void PressBouqets()
        {
			PushL("<b>Категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🟩 До 1500 рублей 🟩", Q(PushItem,header_bouqets,0,1500));
			RowButton("🟩 От 1500 До 2500 рублей 🟩", Q(PushItem,header_bouqets,1500,2500));
			RowButton("🟩 От 2500 До 3500 рублей 🟩", Q(PushItem,header_bouqets,2500,3500));
			RowButton("🟩 От 3500 До 5000 рублей 🟩", Q(PushItem,header_bouqets,3500,5000));
			RowButton("🟩 5000 рублей и выше 🟩", Q(PushItem,header_bouqets,5000,999999));
			
		}
		//Корзины
		[Action]
		public void PressBaskets()
        {	
			PushL("<b>Ценовые категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🟩 До 2500 рублей 🟩", Q(PushItem,header_baskets,0,2500));
			RowButton("🟩 От 2500 До 4000 рублей 🟩", Q(PushItem,header_baskets,2500,4000));
			RowButton("🟩 От 4000 До 7000 рублей 🟩", Q(PushItem,header_baskets,4000,7000));
			RowButton("🟩 7000 рублей и выше 🟩", Q(PushItem,header_baskets,7000,999999));
			
			
		}
		//Мягкие игрушки
		[Action]
		public void PressToys()
		{
			PushL("<b>Категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🧸 Показать товары", Q(PushItem,header_toys,0,999999999));
			
		}
		//Воздушные шары
		[Action]
		public void PressBallons()
		{
			PushL("<b>Категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🎈 Показать товары", Q(PushItem,header_baloons,0,9999999));
			
		}
		//Конфеты
		[Action]
		public void PressCandy()
		{
			PushL("<b>Категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🍬 Показать товары", Q(PushItem,header_candy,0,9999999));
			
		}
		[Action]
		//Торты
		public void PressCakes()
        {
			PushL("<b>Категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🎂 Показать товары", Q(PushItem,header_cakes,0,999999999));

			
		}
		//Фрукты
		[Action]
		public void PressFruits()
		{
			PushL("<b>Категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🍏 Показать товары", Q(PushItem,header_fruits,0,999999999));

		}
		//Открытки
		[Action]
		public void PressPostcards()
		{
			PushL("<b>Категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🗾 Показать товары", Q(PushItem,header_postcards,0,99999999));

		}
		//Корзина
		[Action]
		public async void PressMainBasket()
		{

			foreach(var item in shop_cart)
            {
				await Bot.SendTextMessageAsync(ChatId, $"Наименование: {item.Name} Цена:{item.Price}", Telegram.Bot.Types.Enums.ParseMode.Html);
			}
			

		}
		//Стоимость доставки
		[Action]
		public void PressDelivery()
		{
			
			Bot.SendPhotoAsync(ChatId, "https://i.siteapi.org/7EEvg7hzsPJrNpOqfsoyA6C4D8E=/0x44:618x824/ac20a296e8e485f.ru.s.siteapi.org/img/6920e1k6n5kw4gwgcgwkgs0g4gwooo", "🚚 <b>Стоимость доставки</b>\n<b>Самовывоз</b> - 0 ₽\n<b>Каменск - Шахтинский(центр и мкр.60 лет Октября)</b> - 150 ₽\n<b>Комбинат(район)</b> - 200 ₽\n<b>Старая Станица(район)</b> - 300 ₽\n<b>Шахтёрский(район)</b> - 200 ₽\n<b>Южный(район)</b> - 250 ₽\n<b>Абрамовка(посёлок)</b> - 300 ₽\n<b>Астахов(хутор)</b> - 550 ₽\n<b>Богданов(хутор)</b> - 550 ₽\n<b>Вишневецкий</b> - 800 ₽\n<b>Волченский(хутор)</b> - 500 ₽\n<b>Глубокий(посёлок)</b> - 650 ₽\n<b>Данилов(хутор)</b> - 1200 ₽\n<b>Донецк РФ</b> - 900 ₽\n<b>Диченский(хутор)</b> - 400 ₽\n<b>Заводской(микрорайон)</b> - 450 ₽\n<b>Калитвенская(станица)</b> - 500 ₽\n<b>Красновка(хутор)</b> - 350 ₽\n<b>Леcной(посёлок)</b> - 300 ₽\n<b>Лиховской(Лихая)</b> - 600 ₽\n<b>Лихая(за переездом)</b> - 700 ₽\n<b>Лихой(хутор)</b> - 800 ₽\n<b>Малая Каменка(хутор)</b> - 400 ₽\n<b>Масаловка(хутор)</b> - 550 ₽\n<b>Нижнеговейный(хутор)</b> - 300 ₽\n<b>Углеродовский</b> - 850 ₽\n<b>Филлипенков(хутор)</b> - 400 ₽\n<b>Чистоозерный(посёлок)</b> - 550 ₽\n<b>Шахта 17</b> - 500 ₽", Telegram.Bot.Types.Enums.ParseMode.Html);
			
			Send();
		}
		
		//Рейтинги
		[Action]
		public void PressRate()
        {
			PushL("<b>Наш рейтинг:</b>\n\n4.8 ⭐ (РЕЙТИНГ ЯНДЕКС)\n*на основе 42 официальных отзывов в сервисах данной поисковой службы\n\n4.74 ⭐ (РЕЙТИНГ GOOGLE)\n*на основе 57 официальных отзывов в сервисах данной поисковой службы");
			RowButton("⏪ Назад", Q(Start));
		}
		//Исключение
		[On(Handle.Unknown)]
		public void Unknown()
        {
			PushL("Команда не распознана!");
        }

		//Постинг зона

		

		[Action]
		public async void CallData(string item_name,string _price)
        {

			shop_cart.Add(new Item(item_name,_price));
			await Bot.SendTextMessageAsync(ChatId, $"✅ Товар {item_name} добавлен в корзину!", Telegram.Bot.Types.Enums.ParseMode.Html);
            
        }

		

		public void SendPhoto(string _imgurl,string  _itemname, string _price)
		{

			InlineKeyboardMarkup inlineKeyboard = new(

			new[]
			{
				InlineKeyboardButton.WithCallbackData(text: "🛒 В корзину", callbackData: Q(CallData,$"{_itemname}",_price)),

			}

		);

			Bot.SendPhotoAsync(ChatId,_imgurl,$"<b>{_itemname}</b>\n\nЦена: {_price}\n\n🚚 Доставка или самовывоз", Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: inlineKeyboard);

			


		}
		

		//Маркап


		
			
			
		//Сортировка и постинг
		[Action]
		public void PushItem(string _header, int from_price, int to_price)
		{

			ParseItem(baseurl + _header);
			for (int i = 0; i < prices.Count; i++)
			{
				string check = new string(prices.ElementAt(i).Where(t => char.IsDigit(t)).ToArray());
				int price = Convert.ToInt32(check);
				if (price >= from_price && price <= to_price)
				{
					SendPhoto(urls.ElementAt(i), titles.ElementAt(i), prices.ElementAt(i));
				}

			}
		}
		
		//Callback

		//Парсинг
		public void ParseItem(string _baseurl)
		{		
			HtmlDocument HD = new HtmlDocument();
			var web = new HtmlWeb
			{
				AutoDetectEncoding = false,
				OverrideEncoding = Encoding.UTF8,
			};
			HD = web.Load(_baseurl);

			// Собственно, здесь и производится выборка интересующих нам нодов
			// В данном случае выбираем блочные элементы с классом eTitle
			HtmlNodeCollection PricesElements = HD.DocumentNode.SelectNodes("//div[@class='product-item-price']");
			HtmlNodeCollection TitlesElements = HD.DocumentNode.SelectNodes("//div[@class='product-item__link']//a");
			HtmlNodeCollection UrlsElements = HD.DocumentNode.SelectNodes("//div[@class='product-item__content']//picture//img");
			// Проверяем наличие узлов
			if (PricesElements != null)
			{
				foreach (HtmlNode HN in PricesElements)
				{
					// Получаем строчки
					string outputText = HN.InnerText;
					prices.Add(outputText);
				}

			}
			if (TitlesElements != null)
			{
				foreach (HtmlNode Title in TitlesElements)
				{
					string outputText = Title.InnerText;
					titles.Add(outputText);
				}
			}
			if (UrlsElements != null)
			{
				foreach (HtmlNode Url in UrlsElements)
				{
					string outputText = Url.GetAttributeValue("src", "");
					urls.Add("https:" + outputText);
				}
			}
		}
	}
}

