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
using Telegram.Bot.Types.Enums;

namespace BotFFlowers
{
	public class MainController : BotController
	{
		Random random = new Random();
		//Мэйн бот
		private static TelegramBotClient BotGen = new TelegramBotClient("5249074040:AAGjwQxQHo17Ut6ychH50QMHmgEwyndUbZo");
		//Бот отправкм заказов в приватный канал
		private static TelegramBotClient Notif = new TelegramBotClient("5213399849:AAHa_-r0-xgtplHmaMro9m8jmQ88qe8Nk8w");
		//Админ ChatID
		private static string admin_chatid = "387549112";
		//Постройка товара
		private List<string> prices = new List<string>();
		private List<string> titles = new List<string>();
		private  List<string> urls = new List<string>();
		//Корзина
		private static List<Item> shop_cart = new List<Item>();
		private static Customer customer_info = new Customer();
		//CMS временное
		private static NewCMS temp_cms = new NewCMS();
		//Парсинг
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
		
		
		[Action("/start", "Меню")]
		public void Start()
        {
	       
			if(ChatId.ToString().Equals(admin_chatid))
            {
				PushL($"✋ Привет, {Context.GetUserFullName()}!\n\n⚪ <b>Панель админа + CMS</b>");
				RowButton("💁 Режим обычного пользователя",Q(StartAdmin));
				RowButton("🗾 Показать товары");
				RowButton("✅ Добавить товар",Q(CMS_ADD));					
				RowButton("🤝 Доступ к CMS ");
			}
			else
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
				RowButton("🔥 12. Instagram");
				RowButton("⭐ Наши оценки", Q(PressRate));
				RowButton("🛒 Корзина", Q(PressMainBasket));
				Button("🚚 Доставка", Q(PressDelivery));
				Button("📱 Контакты", Q(PressContact));
				
			}						
		}
		//Админ действие
		[Action]
		public void StartAdmin()
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
			RowButton("🔥 12. Instagram");
			RowButton("⭐ Наши оценки", Q(PressRate));
			RowButton("🛒 Корзина", Q(PressMainBasket));
			Button("🚚 Доставка", Q(PressDelivery));
			Button("📱 Контакты", Q(PressContact));
			RowButton("💻 Вернуться в админку", Q(Start));
		}
		
		//Действия категорий
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
			RowButton("🟩 5000 рублей и выше 🟩", Q(PushItem,header_roses,5000,50000));
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
			RowButton("🟩 5000 рублей и выше 🟩", Q(PushItem,header_boxes,5000,50000));
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
			RowButton("🟩 5000 рублей и выше 🟩", Q(PushItem,header_bouqets,5000,50000));
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
			RowButton("🟩 7000 рублей и выше 🟩", Q(PushItem,header_baskets,7000,50000));
        }
		//Мягкие игрушки
		[Action]
		public void PressToys()
		{
			PushL("<b>Категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🧸 Показать товары", Q(PushItem,header_toys,0,50000));
		}
		//Воздушные шары
		[Action]
		public void PressBallons()
		{
			PushL("<b>Категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🎈 Показать товары", Q(PushItem,header_baloons,0,50000));
		}
		//Конфеты
		[Action]
		public void PressCandy()
		{
			PushL("<b>Категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🍬 Показать товары", Q(PushItem,header_candy,0,50000));
		}
		[Action]
		//Торты
		public void PressCakes()
        {
			PushL("<b>Категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🎂 Показать товары", Q(PushItem,header_cakes,0,50000));
        }
		//Фрукты
		[Action]
		public void PressFruits()
		{
			PushL("<b>Категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🍏 Показать товары", Q(PushItem,header_fruits,0,50000));
		}
		//Открытки
		[Action]
		public void PressPostcards()
		{
			PushL("<b>Категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🗾 Показать товары", Q(PushItem,header_postcards,0,50000));
		}
		//Стоимость доставки
		[Action]
		public async void PressDelivery()
		{
			PushL("🚚 <b>Стоимость доставки</b>\n<b>Самовывоз</b> - 0 ₽\n<b>Каменск - Шахтинский(центр и мкр.60 лет Октября)</b> - 150 ₽\n<b>Комбинат(район)</b> - 200 ₽\n<b>Старая Станица(район)</b> - 300 ₽\n<b>Шахтёрский(район)</b> - 200 ₽\n<b>Южный(район)</b> - 250 ₽\n<b>Абрамовка(посёлок)</b> - 300 ₽\n<b>Астахов(хутор)</b> - 550 ₽\n<b>Богданов(хутор)</b> - 550 ₽\n<b>Вишневецкий</b> - 800 ₽\n<b>Волченский(хутор)</b> - 500 ₽\n<b>Глубокий(посёлок)</b> - 650 ₽\n<b>Данилов(хутор)</b> - 1200 ₽\n<b>Донецк РФ</b> - 900 ₽\n<b>Диченский(хутор)</b> - 400 ₽\n<b>Заводской(микрорайон)</b> - 450 ₽\n<b>Калитвенская(станица)</b> - 500 ₽\n<b>Красновка(хутор)</b> - 350 ₽\n<b>Леcной(посёлок)</b> - 300 ₽\n<b>Лиховской(Лихая)</b> - 600 ₽\n<b>Лихая(за переездом)</b> - 700 ₽\n<b>Лихой(хутор)</b> - 800 ₽\n<b>Малая Каменка(хутор)</b> - 400 ₽\n<b>Масаловка(хутор)</b> - 550 ₽\n<b>Нижнеговейный(хутор)</b> - 300 ₽\n<b>Углеродовский</b> - 850 ₽\n<b>Филлипенков(хутор)</b> - 400 ₽\n<b>Чистоозерный(посёлок)</b> - 550 ₽\n<b>Шахта 17</b> - 500 ₽");
			RowButton("⏪ Назад", Q(Start));
		}
		//Контакты
		[Action]
		public async void PressContact()
		{
			PushL("📱 <b>Контакты</b>\n📍 <b>Адрес:</b>\n пр.Карла Маркса, 54г.Каменск-Шахтинский\n(Режим работы: Круглосуточно)\n📍 <b>Адрес:</b>\n пр.Карла Маркса, 79, Каменск-Шахтинский\n(Режим работы: 7:00-20:00)\n📞 <b>Телефоны:</b>\n +7-928-180-63-88\n +7-918-576-10-88\n📧 <b>E-mail:</b>\n flowerskamensk@mail.ru\n🌐 <b>Сайт:</b>\n https://flowerskamensk.ru/\n📲 <b>Whatsapp:</b>\n +7-928-180-63-88\n🕰 <b>Прием заказов:</b>\n с 8:00-22:00");
			RowButton("⏪ Назад", Q(Start));
		}
		//Рейтинги
		[Action]
		public void PressRate()
		{
			PushL("<b>Наш рейтинг:</b>\n\n4.8 ⭐ (РЕЙТИНГ ЯНДЕКС)\n*на основе 42 официальных отзывов в сервисах данной поисковой службы\n\n4.74 ⭐ (РЕЙТИНГ GOOGLE)\n*на основе 57 официальных отзывов в сервисах данной поисковой службы");
			RowButton("⏪ Назад", Q(Start));
		}

		
		
		//Корзина
		[Action]
		public async void PressMainBasket()
		{
			//Айди товара в корзине
			int id = 0;
			//Маркап товара
			InlineKeyboardMarkup delete_cart = new(
				new[]
			{
				InlineKeyboardButton.WithCallbackData(text: "❌ Удалить товар", callbackData: Q(DeleteAtId,id)),
			}

		);
			//Маркап корзины
			InlineKeyboardMarkup create_order = new(
				new[]
			{
				InlineKeyboardButton.WithCallbackData(text: "⏪ Назад", callbackData: Q(Start)),
				InlineKeyboardButton.WithCallbackData(text: "❌ Очистить", callbackData: Q(CartDeleteCallData)),
				InlineKeyboardButton.WithCallbackData(text: "✅ Оформить", callbackData: Q(NotificateOrder)),
			}

		);
			//Маркап пустой корзины
			InlineKeyboardMarkup redirect = new(
				new[]
			{
				InlineKeyboardButton.WithCallbackData(text: "⏪ Меню", callbackData: Q(Start)),
			}

		);
			await Client.SendTextMessageAsync(ChatId, "🛒 <b>Корзина:</b>\n❗ Доставка шаров, тортов и игрушек осуществляется только вместе с доставкой букета!\n", Telegram.Bot.Types.Enums.ParseMode.Html);
			if (shop_cart.Count>0)
            {
				List<int> total_list = new List<int>();
				int total = 0;
				for (int i = 0; i < shop_cart.Count; i++)
				{
					await Client.SendTextMessageAsync(ChatId, $"⭐ {shop_cart.ElementAt(i).Name} : {shop_cart.ElementAt(i).Price}", Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: delete_cart);
					string check = new string(shop_cart.ElementAt(i).Price.Where(t => char.IsDigit(t)).ToArray());
					int price = Convert.ToInt32(check);
					total_list.Add(price);
					id = i;
				}
				foreach (var i in total_list)
				{
					total += i;
				}
				await Client.SendTextMessageAsync(ChatId, $"💰 Итоговая сумма заказа(Без учёта доставки): {total} ₽", Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: create_order);
			}
            else
            {
	            await Client.SendTextMessageAsync(ChatId, $"Вы ничего не добавили в корзину 😔", Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: redirect);
			}
		}
		//Полная очистка корзины
		[Action]
		public async void CartDeleteCallData()
        {
			InlineKeyboardMarkup redirect_basket = new(
				new[]
			{
				InlineKeyboardButton.WithCallbackData(text: "⏪ Меню", callbackData: Q(Start)),
			}
		);
			shop_cart.Clear();
			await Client.SendTextMessageAsync(ChatId, "✅ Корзина очищена", Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: redirect_basket);
        }
		//Удаление одного товара из корзины по ID
		[Action]
		public async void DeleteAtId(int id)
        {
			InlineKeyboardMarkup redirect_basket = new(
				new[]
			{
				InlineKeyboardButton.WithCallbackData(text: "⏪ Меню", callbackData: Q(Start)),
				InlineKeyboardButton.WithCallbackData(text: "🛒 К корзине", callbackData: Q(PressMainBasket)),
			}

		);
			shop_cart.RemoveAt(id);
			await Client.SendTextMessageAsync(ChatId, "✅ Товар удалён", Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: redirect_basket);
		}
		
		//Коллбэк кнопки добавления в корзину
		[Action]
		public async void CallData(string item_name,string _price)
        {
			InlineKeyboardMarkup redirect_basket = new(

			new[]
			{
				InlineKeyboardButton.WithCallbackData(text: "⏪ Меню", callbackData: Q(Start)),
				InlineKeyboardButton.WithCallbackData(text: "🛒 К корзине", callbackData: Q(PressMainBasket)),
			}

		);
			shop_cart.Add(new Item(item_name,_price));
			await Client.SendTextMessageAsync(ChatId, $"✅ Товар {item_name} добавлен в корзину!", Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: redirect_basket);
        }

		
		//Постинг товаров
		public  void SendPhoto(string _imgurl,string  _itemname, string _price)
		{
			InlineKeyboardMarkup inlineKeyboard = new(
				new[]
			{
				InlineKeyboardButton.WithCallbackData(text: "🛒 В корзину", callbackData: Q(CallData,_itemname,_price)),
			}
			);
			Client.SendPhotoAsync(ChatId,_imgurl,$"<b>{_itemname}</b>\n\nЦена: {_price}\n\n🚚 Доставка или самовывоз", Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: inlineKeyboard);
		}

		//Обработка сообщений пользователя и постройка уведомления нового заказа
		[Action]
		public async Task NotificateOrder()
		{
			PushL("Пожалуйста, заполните форму ниже 👇");
			PushL("🙂 Ваше ФИО:");
			await Send();			 
			customer_info.Customer_name = await AwaitText();
			PushL("📱 Ваш номер телефона:");
			await Send();
			customer_info.Customer_number = await AwaitText();
			PushL("🙂 ФИО получателя:");
			await Send();
			customer_info.Receive_name = await AwaitText();
			PushL("📱 Номер телефона получателя:");
			await Send();
			customer_info.Receive_number = await AwaitText();
			PushL("🏠 Адрес получателя:");
			await Send();
			customer_info.Address = await AwaitText();
			PushL("🗒 Дополнительные пожелания:");
			await Send();
			customer_info.Additional = await AwaitText();
			List<int> total_price = new List<int>();
			int result_price = 0;
			if(customer_info.Customer_name is not null && customer_info.Customer_number is not null)
            {
				//Инлайн оформления заказа
				InlineKeyboardMarkup back_menu = new(
					new[]
			{
				InlineKeyboardButton.WithCallbackData(text: "⏪ Меню", callbackData: Q(Start)),
			}

		);
				//Увед в приватный канал
				await Client.SendTextMessageAsync(ChatId, "✅ <b>Заказ оформлен!</b>\nВ ближайшее время с вами свяжется менеджер для подтверждения заказа!", ParseMode.Html,replyMarkup: back_menu);
				string ID_ORDER = random.Next(500000).ToString();
				string Notif_message = $"🟩 <b>Новый заказ! #{ID_ORDER}</b>\n===============\n<b>Заказчик:</b> {customer_info.Customer_name} \nНомер заказчика: {customer_info.Customer_number}\nТелега заказчика: @{Context.GetUsername()} \n===============\n<b>Получатель:</b> {customer_info.Receive_name} \nНомер получателя: {customer_info.Receive_number} \n===============\n<b>Адрес:</b> {customer_info.Address} \n===============\n<b>Дополнительно:</b> {customer_info.Additional} \n===============\n<b>Заказанные товары</b> 👇\n";
			
				for (int i = 0; i < shop_cart.Count; i++)
				{
				string check = new string(shop_cart.ElementAt(i).Price.Where(t => char.IsDigit(t)).ToArray());
					int price = Convert.ToInt32(check);
					total_price.Add(price);
					Notif_message += $"⭐ Товар: {shop_cart.ElementAt(i).Name}  Стоимость: {shop_cart.ElementAt(i).Price}\n ";
				}

				foreach (var i in total_price)
				{
					result_price += i;
				}
				Notif_message += $"===============\n<b>Итоговая сумма</b> БЕЗ учета доставки: {result_price} ₽";
				await Notif.SendTextMessageAsync(chatId: "-1001795322586", text: $"{Notif_message}", Telegram.Bot.Types.Enums.ParseMode.Html);			
			}
            else
            {
				await Client.SendTextMessageAsync(ChatId, "😕 Некорректные данные!\n Оформите заказ в корзине снова!", Telegram.Bot.Types.Enums.ParseMode.Html);
            }
			
		}

		//Сортировка и отправка
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
		//Исключение
		[On(Handle.Unknown)]
		public void Unknown()
		{
			PushL("Команда не распознана!");
		}
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



		//CMS

		//Создание карточки
		[Action]
		public void CMS_ADD()
        {
			PushL("Добавление товара");
			RowButton("⏪ Назад", Q(Start));
			Button("➕ Добавить",Q(Add_product));
        }
		[Action]
		public async Task Add_product()
        {
			InlineKeyboardMarkup product_sample = new(

			new[]
			{
				new[]
                {
					InlineKeyboardButton.WithCallbackData(text: "📱 Предпросмотр", callbackData: Q(PreviewCMS)),
				},
				new []
                {
					InlineKeyboardButton.WithCallbackData(text: "✅ Добавить", callbackData: Q(CMS_Create)),
					InlineKeyboardButton.WithCallbackData(text: "❌ Отмена", callbackData: Q(CMS_ADD)),
				},
			}
			);
			PushL("Добавь фото товара:");
			await Send();
			temp_cms.Img = await AwaitText();
			PushL("Добавь название товара:");
			await Send();
			temp_cms.Text = await AwaitText();
			PushL("Добавь стоимость товара:");
			await Send();
			temp_cms.Price = await AwaitText();
			
			await Client.SendTextMessageAsync(ChatId, $"Карточка товара сформирована!", ParseMode.Html, replyMarkup: product_sample);
		}
		//Удалить товар
		[Action]
		public async void CMS_DELETE()
        {
			

		}
		//Добавление
		[Action]
		public async Task CMS_Create()
		{
			await Client.SendTextMessageAsync(ChatId,$"✅ Товар {temp_cms.Text} успешно добавлен в категорию!");
		}
		//Предпросмотр
		[Action]
		public async Task PreviewCMS()
		{
			InlineKeyboardMarkup add_markup = new(
				new[]
				{
					InlineKeyboardButton.WithCallbackData(text: "✅ Добавить", callbackData: Q(CMS_Create)),
					InlineKeyboardButton.WithCallbackData(text: "❌ Отмена", callbackData: Q(Start)),
				}
			);
			await Client.SendTextMessageAsync(ChatId,$"Картинка {temp_cms.Img}\nТекст{temp_cms.Text}\nЦена{temp_cms.Price}", replyMarkup:add_markup);
		}
	}
}

