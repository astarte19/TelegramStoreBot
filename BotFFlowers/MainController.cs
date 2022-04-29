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
using System.Data.SQLite;
using System.Data;


namespace BotFFlowers
{
	public class MainController : BotController
	{
		
		
		Random random = new Random();
		//Instagram Temp
		public int Insta_temp { get; set; }
		
		//Бот отправкм заказов в приватный канал
		private static TelegramBotClient Notif = new TelegramBotClient("5213399849:AAHa_-r0-xgtplHmaMro9m8jmQ88qe8Nk8w");
		//Админ ChatID
		private static string admin_chatid = "387549112";

		private static string admin_chatid2 = "727043884";
		//Постройка товара
		private List<string> prices = new List<string>();
		private List<string> titles = new List<string>();
		private  List<string> urls = new List<string>();
		//Корзина
		private static List<Item> shop_cart = new List<Item>();
		private static Customer customer_info = new Customer();
		//CMS временное
		private static NewCMS temp_cms = new NewCMS();
		public int Temp_id { get; set; }
		
		//Парсинг
		string baseurl = "https://flowerskamensk.ru/products/category/";
		string header_tulps = "tulpany";
		string header_roses = "rossiyskie-rozy";
		 string header_equadorroses = "gollandskie-rozy";
		string header_boxes = "cveti-v-korobkah";
		string header_bouqets = "bukety";
		string header_baskets = "korziny";
		string header_toys = "plushevie-mishki";
		string header_baloons = "vozdushnie-shary";
		string header_candy = "konfety";
		string header_cakes = "torty";
		string header_fruits = "frukty-v-korzine";
		string header_postcards = "otkrytki";
		readonly ILogger<MainController> _logger;
		public MainController(ILogger<MainController> logger)
		{
			
			_logger = logger;
			
		}
		[Action("/start", "Меню")]
		public void Start()
        {
			//Проверка на админа
			if (ChatId.ToString().Equals(admin_chatid) || ChatId.ToString().Equals(admin_chatid2))
            {
				PushL($"✋ Привет, {Context.GetUserFullName()}!\n\n⚪ <b>Панель админа + CMS</b>");
				RowButton("💁 Режим обычного пользователя",Q(StartAdmin));
				RowButton("🗾 Показать товары",Q(ReadTable));
				RowButton("✅ Добавить товар",Q(CMS_ADD));					
				RowButton("❌ Удалить товар", Q(CMS_DELETE));
				RowButton("📱 Изменить товар", Q(Edit_product));
			}
			else
            {
				PushL($"✋ <b>Привет, {Context.GetUserFullName()}!</b>\n🌷 <b>Городские цветы Каменск-Шахтинский</b> \n🟢 Самые свежие цветы и букеты! \n🟢 Более 8 лет опыта и репутации! \n🟢 Наш <i>telegram</i> канал: <a href='https://t.me/gorodskie_cveti_kamensk'>Городские Цветы Каменск</a>");
				RowButton("🔥  Новинки!",Q(Instagram));
				RowButton("🌷  Тюльпаны", Q(PressTulps));
				RowButton("🌹  Российские Розы", Q(PressRURoses));
				RowButton("🌹  Эквадорские Розы", Q(PressEQRoses));
				RowButton("🌸  Цветы в коробках", Q(PressBoxes));
				RowButton("💐  Букеты", Q(PressBouqets));
				RowButton("🧺  Корзины", Q(PressBaskets));
				RowButton("🧸  Мягкие игрушки", Q(PushItem,header_toys,0,999999));
				RowButton("🎈  Воздушные шары", Q(PushItem,header_baloons,0,999999));
				RowButton("🍬  Конфеты", Q(PushItem,header_candy,0,999999));
				RowButton("🎂  Торты", Q(PushItem,header_cakes,0,999999));
				RowButton("🍏  Фрукты", Q(PushItem,header_fruits,0,999999));
				RowButton("🗾  Открытки", Q(PushItem,header_postcards,0,999999));
				RowButton("🚚 Доставка", Q(PressDelivery));
				RowButton("🛒 Корзина", Q(PressMainBasket));
				Button("⭐ Отзывы", Q(PressRate));
				Button("📱 Контакты", Q(PressContact));
				
			}						
		}
		//Админ действие
		[Action]
		public async void Instagram()
        {
	        
	        SQLiteConnection DB = new SQLiteConnection("Data Source=DBFlowers.db;");
			DB.Open();
			SQLiteCommand create = DB.CreateCommand();
			create.CommandText = "SELECT * FROM Products";
			
			SQLiteDataReader reader = create.ExecuteReader();
			while (reader.Read())
			{
				SendPhoto(reader["Guid"].ToString(),reader["Image"].ToString(),reader["Text"].ToString(),reader["Price"].ToString());
			}
			DB.Close();
		}
		
		[Action]
		public void StartAdmin()
        {
			PushL($"✋ <b>Привет, {Context.GetUserFullName()}!</b>\n🌷 <b>Городские цветы Каменск-Шахтинский</b> \n🟢 Самые свежие цветы и букеты! \n🟢 Более 8 лет опыта и репутации! \n🟢 Наш <i>telegram</i> канал: <a href='https://t.me/gorodskie_cveti_kamensk'>Городские Цветы Каменск</a>");
			RowButton("🔥  Новинки!",Q(Instagram));
			RowButton("🌷  Тюльпаны", Q(PressTulps));
			RowButton("🌹  Российские Розы", Q(PressRURoses));
			RowButton("🌹  Эквадорские Розы", Q(PressEQRoses));
			RowButton("🌸  Цветы в коробках", Q(PressBoxes));
			RowButton("💐  Букеты", Q(PressBouqets));
			RowButton("🧺  Корзины", Q(PressBaskets));
			RowButton("🧸  Мягкие игрушки", Q(PushItem,header_toys,0,999999));
			RowButton("🎈  Воздушные шары", Q(PushItem,header_baloons,0,999999));
			RowButton("🍬  Конфеты", Q(PushItem,header_candy,0,999999));
			RowButton("🎂  Торты", Q(PushItem,header_cakes,0,999999));
			RowButton("🍏  Фрукты", Q(PushItem,header_fruits,0,999999));
			RowButton("🗾  Открытки", Q(PushItem,header_postcards,0,999999));
			RowButton("🚚 Доставка", Q(PressDelivery));
			RowButton("🛒 Корзина", Q(PressMainBasket));
			Button("⭐ Отзывы", Q(PressRate));
			Button("📱 Контакты", Q(PressContact));
			RowButton("💻 Вернуться в админку", Q(Start));
		}
		//Стоимость доставки
		[Action]
		public async void PressDelivery()
		{
			PushL("🚚 <b>Стоимость доставки</b>\n<b>Самовывоз</b> - 0 ₽\n<b>Каменск - Шахтинский(центр и мкр.60 лет Октября)</b> - 150 ₽\n<b>Комбинат(район)</b> - 200 ₽\n<b>Старая Станица(район)</b> - 300 ₽\n<b>Шахтёрский(район)</b> - 200 ₽\n<b>Южный(район)</b> - 250 ₽\n<b>Абрамовка(посёлок)</b> - 300 ₽\n<b>Астахов(хутор)</b> - 550 ₽\n<b>Богданов(хутор)</b> - 550 ₽\n<b>Вишневецкий</b> - 800 ₽\n<b>Волченский(хутор)</b> - 500 ₽\n<b>Глубокий(посёлок)</b> - 650 ₽\n<b>Данилов(хутор)</b> - 1200 ₽\n<b>Донецк РФ</b> - 900 ₽\n<b>Диченский(хутор)</b> - 400 ₽\n<b>Заводской(микрорайон)</b> - 450 ₽\n<b>Калитвенская(станица)</b> - 500 ₽\n<b>Красновка(хутор)</b> - 350 ₽\n<b>Леcной(посёлок)</b> - 300 ₽\n<b>Лиховской(Лихая)</b> - 600 ₽\n<b>Лихая(за переездом)</b> - 700 ₽\n<b>Лихой(хутор)</b> - 800 ₽\n<b>Малая Каменка(хутор)</b> - 400 ₽\n<b>Масаловка(хутор)</b> - 550 ₽\n<b>Нижнеговейный(хутор)</b> - 300 ₽\n<b>Углеродовский</b> - 850 ₽\n<b>Филлипенков(хутор)</b> - 400 ₽\n<b>Чистоозерный(посёлок)</b> - 550 ₽\n<b>Шахта 17</b> - 500 ₽");
			RowButton("⏪ Назад", Q(Start));
		}
		//Товар - Доставка
		[Action]
		public async Task PushDelivery()
		{
			SQLiteConnection DB = new SQLiteConnection("Data Source=DBFlowers.db;");
			await Client.SendTextMessageAsync(ChatId, "🚚 <b>Стоимость доставки</b>", ParseMode.Html);
			DB.Open();
				SQLiteCommand create = DB.CreateCommand();
				create.CommandText = "SELECT * FROM Delivery";
				SQLiteDataReader reader = create.ExecuteReader();
				while (reader.Read())
				{
					InlineKeyboardMarkup inlineKeyboard = new(
						new[]
						{
							InlineKeyboardButton.WithCallbackData(text: "🛒 В корзину", callbackData: Q(DeliveryCall,reader["Guid"].ToString())),
						}
					);
					await Client.SendTextMessageAsync(ChatId, $"<b>{reader["Name"].ToString()}</b> - {reader["Price"].ToString()}", ParseMode.Html,
						replyMarkup: inlineKeyboard);
				}
				DB.Close();
			
		}
		[Action]
		public async Task DeliveryCall(string guid)
		{
			InlineKeyboardMarkup redirect_basket = new(

				new[]
				{
					InlineKeyboardButton.WithCallbackData(text: "⏪ Меню", callbackData: Q(Start)),
					InlineKeyboardButton.WithCallbackData(text: "🛒 К корзине", callbackData: Q(PressMainBasket)),
					InlineKeyboardButton.WithCallbackData(text: "✅ Оформление заказа", callbackData: Q(NotificateOrder)),
				}
			
			);
			
			SQLiteConnection DB = new SQLiteConnection("Data Source=DBFlowers.db;");
			DB.Open();
			SQLiteCommand create = DB.CreateCommand();
			create.CommandText = "SELECT * FROM Delivery WHERE Guid = @guid";
			create.Parameters.AddWithValue("@guid", guid);
			SQLiteDataReader reader = create.ExecuteReader();
			while (reader.Read())
			{
				shop_cart.Add(new Item("Доставка на "+reader["Name"].ToString(),reader["Price"].ToString()));
				await Client.SendTextMessageAsync(ChatId, $"✅ Зона доставки {reader["Name"].ToString()} добавлена!", Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: redirect_basket);
			}
			DB.Close();
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
			RowButton("🟩 5000 рублей и выше 🟩", Q(PushItem,header_roses,5000,999999));
        }

		[Action]
		public void PressEQRoses()
		{
			PushL("<b>Ценовые категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🟩 От 1700 До 2500 рублей 🟩", Q(PushItem,header_equadorroses,1700,2500));
			RowButton("🟩 От 2500 До 3500 рублей 🟩", Q(PushItem,header_equadorroses,2500,3500));
			RowButton("🟩 От 3500 До 5000 рублей 🟩", Q(PushItem,header_equadorroses,3500,5000));
			RowButton("🟩 5000 рублей и выше 🟩", Q(PushItem,header_equadorroses,5000,999999));
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
				InlineKeyboardButton.WithCallbackData(text: "🚚 Доставка", callbackData: Q(PushDelivery)),
			}

		);
			//Маркап пустой корзины
			InlineKeyboardMarkup redirect = new(
				new[]
			{
				InlineKeyboardButton.WithCallbackData(text: "⏪ Меню", callbackData: Q(Start)),
			}

		);
			await Client.SendTextMessageAsync(ChatId, "🛒 <b>Корзина:</b>\n❗ Доставка шаров, тортов и игрушек осуществляется только вместе с доставкой букета!\n❗ Для оформления заказа нажмите на выбор зон доставки(🚚 Доставка) и добавьте нужные зоны доставки\n", Telegram.Bot.Types.Enums.ParseMode.Html);
			if (shop_cart.Count>0)
            {
				List<int> total_list = new List<int>();
				int total = 0;
				for (int i = 0; i < shop_cart.Count; i++)
				{
					id = i;
					await Client.SendTextMessageAsync(ChatId, $"⭐ {shop_cart.ElementAt(i).Name} : {shop_cart.ElementAt(i).Price}", Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: delete_cart);
					string check = new string(shop_cart.ElementAt(i).Price.Where(t => char.IsDigit(t)).ToArray());
					int price = Convert.ToInt32(check);
					total_list.Add(price);
					
				}
				foreach (var i in total_list)
				{
					total += i;
				}
				await Client.SendTextMessageAsync(ChatId, $"💰 Итоговая сумма заказа: {total} ₽", Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: create_order);
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
		
		

		
		//Постинг товаров
		public async Task SendPhoto(string guid, string _imgurl, string _itemname, string _price)
		{
			InlineKeyboardMarkup inlineKeyboard = new(
				new[]
			{
				InlineKeyboardButton.WithCallbackData(text: "🛒 В корзину", callbackData: Q(CallDataV2,guid)),
			}
			);
			 await Client.SendPhotoAsync(ChatId,_imgurl,$"<b>{_itemname}</b>\n\nЦена: {_price}\n\n🚚 Доставка или самовывоз", Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: inlineKeyboard);
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
			customer_info.order_ID = random.Next(500000).ToString();;
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
				await Client.SendTextMessageAsync(ChatId, $"✅ <b>Заказ оформлен!</b>\nНомер заказа: #{customer_info.order_ID}\nВ ближайшее время с вами свяжется менеджер для подтверждения заказа!", ParseMode.Html,replyMarkup: back_menu);
				
				string Notif_message = $"🟨 <b>Новый заказ! #{customer_info.order_ID}</b>\n===============\n<b>Заказчик:</b> {customer_info.Customer_name} \nНомер заказчика: {customer_info.Customer_number}\nТелега заказчика: @{Context.GetUsername()} \n===============\n<b>Получатель:</b> {customer_info.Receive_name} \nНомер получателя: {customer_info.Receive_number} \n===============\n<b>Адрес:</b> {customer_info.Address} \n===============\n<b>Дополнительно:</b> {customer_info.Additional} \n===============\n<b>Заказанные товары</b> 👇\n";
			
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
				Notif_message += $"===============\n<b>Итоговая сумма</b>: {result_price} ₽";
				await Notif.SendTextMessageAsync(chatId: "-1001795322586", text: $"{Notif_message}", Telegram.Bot.Types.Enums.ParseMode.Html);			
			}
            else
            {
				await Client.SendTextMessageAsync(ChatId, "😕 Некорректные данные!\n Оформите заказ в корзине снова!", Telegram.Bot.Types.Enums.ParseMode.Html);
            }
			
		}

		//Сортировка и отправка
		[Action]
			public async Task PushItem(string _header, int from_price, int to_price)
			{
				
				SQLiteConnection DB = new SQLiteConnection("Data Source=DBFlowers.db;");
				await Send("⏳ Загрузка товаров...");
				ParseItem(baseurl + _header);
				if (urls.Count == 0 && titles.Count == 0 & prices.Count == 0)
				{
					await Send("В данной категории товары закончились 🥺");
				}
				else
				{
					//Очистить бд-буфер
					DB.Open();
					SQLiteCommand clear = DB.CreateCommand();
					clear.CommandText = "DELETE FROM Temp";
					clear.ExecuteNonQuery();
					DB.Close();
					for (int i = 0; i < prices.Count; i++)
					{
						string check = new string(prices.ElementAt(i).Where(t => char.IsDigit(t)).ToArray());
						int price = Convert.ToInt32(check);
						if (price >= from_price && price <= to_price)
						{
							//Уникальный ID, который я передаю в Callback, а потом вытаскиваю через него айтемы
							string ID = Guid.NewGuid().ToString("N");
							//Заполнить бд-буфер
							DB.Open();
							SQLiteCommand add = DB.CreateCommand();
							add.CommandText = "INSERT INTO Temp VALUES(@ID, @Name,@Price)";
							add.Parameters.AddWithValue("@ID", ID);
							add.Parameters.AddWithValue("@Name", titles.ElementAt(i));
							add.Parameters.AddWithValue("@Price", prices.ElementAt(i));
							add.ExecuteNonQuery();
							DB.Close();
							InlineKeyboardMarkup inlineKeyboard = new(
								new[]
								{
									InlineKeyboardButton.WithCallbackData(text: "🛒 В корзину", callbackData: Q(CallDataTest,ID)),
								}
							);
							
							await Client.SendPhotoAsync(ChatId,urls.ElementAt(i),$"<b>{titles.ElementAt(i)}</b>\n\nЦена: {prices.ElementAt(i)}\n\n🚚 Доставка или самовывоз", Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: inlineKeyboard);
						}
					}
				}
				
				
			}
//тест
		[Action]
		public async void CallDataTest(string id)
		{
			InlineKeyboardMarkup redirect_basket = new(

				new[]
				{
					InlineKeyboardButton.WithCallbackData(text: "⏪ Меню", callbackData: Q(Start)),
					InlineKeyboardButton.WithCallbackData(text: "🛒 К корзине", callbackData: Q(PressMainBasket)),
				}

			);
			SQLiteConnection DB = new SQLiteConnection("Data Source=DBFlowers.db;");
			DB.Open();
			SQLiteCommand create = DB.CreateCommand();
			create.CommandText = "SELECT * FROM Temp WHERE ID = @id";
			create.Parameters.AddWithValue("@id", id);
			SQLiteDataReader reader = create.ExecuteReader();
			while (reader.Read())
			{
				shop_cart.Add(new Item(reader["Name"].ToString(),reader["Price"].ToString()));
				await Client.SendTextMessageAsync(ChatId, $"✅ Товар {reader["Name"].ToString()} добавлен в корзину!", Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: redirect_basket);
			}
			DB.Close();
			
		}
//тест
//инст
		[Action]
		public async void CallDataV2(string id)
		{
			InlineKeyboardMarkup redirect_basket = new(

				new[]
				{
					InlineKeyboardButton.WithCallbackData(text: "⏪ Меню", callbackData: Q(Start)),
					InlineKeyboardButton.WithCallbackData(text: "🛒 К корзине", callbackData: Q(PressMainBasket)),
				}

			);
			SQLiteConnection DB = new SQLiteConnection("Data Source=DBFlowers.db;");
			DB.Open();
			SQLiteCommand create = DB.CreateCommand();
			create.CommandText = "SELECT * FROM Products WHERE Guid = @guid";
			create.Parameters.AddWithValue("@guid", id);
			SQLiteDataReader reader = create.ExecuteReader();
			while (reader.Read())
			{
				shop_cart.Add(new Item(reader["Text"].ToString(),reader["Price"].ToString()));
				await Client.SendTextMessageAsync(ChatId, $"✅ Товар {reader["Text"].ToString()} добавлен в корзину!", Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: redirect_basket);
			}
			DB.Close();
			
		}
			[On(Handle.Exception)]
			public async Task OnException(Exception e)
			{
				_logger.LogError(e, "Unhandled exception");
				if (Context.Update.Type == UpdateType.CallbackQuery)
				{
					await AnswerCallback("Error");
				}
				else if (Context.Update.Type == UpdateType.Message)
				{
					Push("Error");
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
			Button("➕ Добавить", Q(Add_product));
		}

		//Просмотр всех товаров
		
		//Формировка карточки
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
			var update = await AwaitNextUpdate();
			if (update.Update.Type == UpdateType.Message && update.Update.Message.Type == MessageType.Photo)
					{
				//temp_cms.Img = update.Update.Message.Document.FileId;
				temp_cms.Img = update.Update.Message.Photo[update.Update.Message.Photo.Length - 1].FileId;


					}
			else
            {
				await Client.SendTextMessageAsync(ChatId, "❌ Ошибка! Повторите еще раз!");
				Start();
            }
			PushL("Добавь название товара:");
			await Send();
			temp_cms.Text = await AwaitText();
			PushL("Добавь стоимость товара:");
			await Send();
			temp_cms.Price = await AwaitText() + " ₽";
			string guid = Guid.NewGuid().ToString("N");
			temp_cms.guid = guid;
			await Client.SendTextMessageAsync(ChatId, $"Карточка товара сформирована!", ParseMode.Html, replyMarkup: product_sample);
		}

		//Final добавление в бд
		[Action]
		public async Task CMS_Create()
		{
			
			var product = new Products();
			product.Image = temp_cms.Img;
			product.Text = temp_cms.Text;
			product.Price = temp_cms.Price;
			product.guid = temp_cms.guid;
			AddProduct(product);
			await Client.SendTextMessageAsync(ChatId, $"✅ Товар {product.Text} успешно добавлен в категорию!");
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
			//await Client.SendTextMessageAsync(ChatId, $"Картинка {temp_cms.Img}\nТекст{temp_cms.Text}\nЦена{temp_cms.Price}", replyMarkup: add_markup);
			await Client.SendPhotoAsync(ChatId, temp_cms.Img,caption:$"<b>{temp_cms.Text}</b>\n\nЦена: {temp_cms.Price}\n\n🚚 Доставка или самовывоз",ParseMode.Html);
		}

		
		[Action]
		public async Task Edit_product()
        {
			InlineKeyboardMarkup removeItem = new(
					new[]
			{
				InlineKeyboardButton.WithCallbackData(text: "📱 Список товаров", callbackData: Q(ReadTable)),
				InlineKeyboardButton.WithCallbackData(text: "⏪ Меню", callbackData: Q(Start)),
			}

		);
			PushL("❗ При неправильно введённом ID товара изменения не произойдет!");
			await Send();
			PushL("Введите ID товара, который хотите изменить:");
			await Send();
			string ID = await AwaitText();
			int id = Convert.ToInt32(ID);
			var new_product = new Products();
			PushL("Новое фото товара:");
			await Send();
			var update = await AwaitNextUpdate();
			if (update.Update.Type == UpdateType.Message && update.Update.Message.Type == MessageType.Photo)
			{
				new_product.Image = update.Update.Message.Photo[update.Update.Message.Photo.Length - 1].FileId;

			}
			else
			{
				await Client.SendTextMessageAsync(ChatId, "❌ Ошибка! Повторите еще раз!");
				Start();
			}

			PushL("Новое название товара:");
			await Send();
			new_product.Text = await AwaitText();
			PushL("Новая стоимость товара:");
			await Send();
			new_product.Price = await AwaitText() + " ₽";
			new_product.Id = id;
			EditProduct(new_product);
			await Client.SendTextMessageAsync(ChatId, "✅ Карточка товара изменена!", ParseMode.Html, replyMarkup: removeItem);
		}
		
		
		//Удалить товар
		[Action]
		public async Task CMS_DELETE()
        {
			InlineKeyboardMarkup removeItem = new(
					new[]
			{
				InlineKeyboardButton.WithCallbackData(text: "📱 Список товаров", callbackData: Q(ReadTable)),
				InlineKeyboardButton.WithCallbackData(text: "⏪ Меню", callbackData: Q(Start)),
			}

		);
			PushL("❗ При неправильно введённом ID товара удаления не произойдет!");
			await Send();
			PushL("Введите ID товара, который хотите удалить:");
			await Send();
			string ID = await AwaitText();
			int id = Convert.ToInt32(ID);
			DeleteProduct(id);
			await Client.SendTextMessageAsync(ChatId, "✅ Товар удален!", replyMarkup: removeItem);
			

		}
		

		//CRUD CMS

		//Создание, обновление, удаление
		private int ExecuteWrite(string query, Dictionary<string, object> args)
		{
			int numberOfRowsAffected;
		
			using (var con = new SQLiteConnection("Data Source=DBFlowers.db"))
			{
				con.Open();
	
				using (var cmd = new SQLiteCommand(query, con))
				{
					foreach (var pair in args)
					{
						cmd.Parameters.AddWithValue(pair.Key, pair.Value);
					}
					numberOfRowsAffected = cmd.ExecuteNonQuery();
				}
				return numberOfRowsAffected;
			}
		}
        //Чтение
        private DataTable ExecuteRead(string query, Dictionary<string,object> args)
        {
            if (string.IsNullOrEmpty(query.Trim()))
                return null;

            using (var con = new SQLiteConnection("Data Source=DBFlowers.db"))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(query, con))
                {
                    foreach (KeyValuePair<string, object> entry in args)
                    {
                        cmd.Parameters.AddWithValue(entry.Key, entry.Value);
                    }
                    var da = new SQLiteDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);
                    da.Dispose();
                    return dt;
                }
            }
        }
		//Добавление элемента
        private int AddProduct(Products product)
		{
			const string query = "INSERT INTO Products(Image, Text,Price,Guid) VALUES(@Image, @Text,@Price,@Guid)";

			var args = new Dictionary<string, object>
	{
		{"@Image", product.Image},
		{"@Text", product.Text},
		{"@Price",product.Price },
		{"@Guid",product.guid }
	};

			return ExecuteWrite(query, args);
		}
		//Изменение элемента по ID
		private int EditProduct(Products product)
		{
			const string query = "UPDATE Products SET Image = @Image, Text = @Text, Price = @Price WHERE Id = @id";

			var args = new Dictionary<string, object>
	{
		{"@id", product.Id},
		{"@Text", product.Text},
		{"@Image", product.Image},
		{"@Price", product.Price}
	};

			return ExecuteWrite(query, args);
		}
		//Удаление элемента по ID
		
		private int DeleteProduct(int id)
		{
			const string query = "Delete from Products WHERE Id = @id";

			var args = new Dictionary<string, object>
	{
		{"@id", id}
	};
			
			return ExecuteWrite(query, args);
		}
		
		//Получение элемента по ID
		private Products GetProductById(int id)
		{
			var query = "SELECT * FROM Products WHERE Id = @id";

			var args = new Dictionary<string, object>
	{
		{"@id", id}
	};
			DataTable dt = ExecuteRead(query, args);
			if (dt == null || dt.Rows.Count == 0)
			{
				return null;
			}
			var product = new Products
			{
				Id = Convert.ToInt32(dt.Rows[0]["Id"]),
				Image = Convert.ToString(dt.Rows[0]["Image"]),
				Text = Convert.ToString(dt.Rows[0]["Text"]),
				Price = Convert.ToString(dt.Rows[0]["Price"])
			};
			return product;
		}
		[Action]
		private async void ReadTable()
        {
			SQLiteConnection check_connection = new SQLiteConnection("Data Source=DBFlowers.db;");
			check_connection.Open();
			SQLiteCommand check_command = check_connection.CreateCommand();
			check_command.CommandText = "SELECT count(rowid) FROM Products"; 
			check_command.ExecuteNonQuery();
			int countRows = (int)(long)check_command.ExecuteScalar();
			check_connection.Close();
			if (countRows == 0)
			{
				await Client.SendTextMessageAsync(ChatId, "❌ Товаров нет! Зайди и добавь!", ParseMode.Html);
			}
			else
            {
				SQLiteConnection DB = new SQLiteConnection("Data Source=DBFlowers.db;");
				DB.Open();
				SQLiteCommand create = DB.CreateCommand();
				create.CommandText = "SELECT * FROM Products";
				SQLiteDataReader reader = create.ExecuteReader();
				while (reader.Read())
				{
					Temp_id = Convert.ToInt32(reader["Id"]);
					await Client.SendPhotoAsync(ChatId, reader["Image"].ToString(), caption: $"ID:{reader["Id"]}\n<b>{reader["Text"]}</b>\n\nЦена: {reader["Price"]}\n\n🚚 Доставка или самовывоз", ParseMode.Html);
				}
				DB.Close();
			}
        }
	}
}

