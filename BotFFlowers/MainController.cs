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
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using CSharpVitamins;
using shortid;
using Npgsql;

namespace BotFFlowers
{
	public class MainController : BotController
	{
		//Константы и токены

		#region Constants
		//Бот отправкм заказов в приватный канал
		private static TelegramBotClient
			Notif = new TelegramBotClient("5355673985:AAFi055Qt0RpnApk7eOwn1P_kLDmr1HZD_Y");
		//Строка подключения к контейнеру
		private static string PostgresConnectionString = "Server=localhost;Port=5432;Database=mydbname;User Id=app;Password=app;";

		//Админ ChatID
		// private static string admin_chatid = "387549112";
		// private static string admin_chatid2 = "727043884";

		private static string notif_chatid = "-1001868442078";

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

		//Таблицы
		static string table_tulps = "tulpany";
		static string table_roses = "rossiyskierozy";
		static string table_equadorroses = "gollandskierozy";
		static string table_boxes = "cvetivkorobkah";
		static string table_bouqets = "bukety";
		static string table_baskets = "korziny";
		static string table_toys = "plusheviemishki";
		static string table_baloons = "vozdushnieshary";
		static string table_candy = "konfety";
		static string table_cakes = "torty";
		static string table_fruits = "fruktyvkorzine";
		static string table_postcards = "otkrytki";
		//CategoryId
		static int table_tulps_cat = 10;
		static int table_roses_cat = 8;
		static int table_equadorroses_cat = 2;
		static int table_boxes_cat = 4;
		static int table_bouqets_cat = 12;
		static int table_baskets_cat = 5;
		static int table_toys_cat = 7;
		static int table_baloons_cat = 11;
		static int table_candy_cat = 3;
		static int table_cakes_cat = 9;
		static int table_fruits_cat = 1;
		static int table_postcards_cat = 6;
		private string[] tables =
		{
			"tulpany", "rossiyskierozy", "gollandskierozy", "cvetivkorobkah", "bukety", "korziny", "plusheviemishki",
			"vozdushnieshary", "konfety", "torty", "fruktyvkorzine", "otkrytki"
		};

		//Для номера заказа
		Random random = new Random();

		//Логгер
		readonly ILogger<MainController> _logger;

		#endregion

		//Буферы

		#region Temps

		//CMS Temp
		public int Insta_temp { get; set; }

		//Постройка товара
		
		private List<string> prices = new List<string>();
		private List<string> titles = new List<string>();
		private List<string> urls = new List<string>();

		//Корзина
		private static List<Item> shop_cart = new List<Item>();

		private static Customer customer_info = new Customer();

		//CMS временное
		private static NewCMS temp_cms = new NewCMS();

		#endregion

		//Основная навигация

		#region MainNavigation

		[Action("/start", "Меню")]
		public void Start()
		{
			NpgsqlConnection DB_Access = new NpgsqlConnection(PostgresConnectionString);
			DB_Access.Open();
			NpgsqlCommand check_access = DB_Access.CreateCommand();
			check_access.CommandText = "SELECT * FROM public.\"AdminAccess\";";
			var access_reader = check_access.ExecuteReader();
			bool isAdmin = false;
			while (access_reader.Read())
			{
				if (isAdmin == false)
				{
					isAdmin = ChatId.ToString().Equals(access_reader["ChatId"].ToString());
				}
			}
			//Проверка на админа
			if (isAdmin)
			{
				PushL($"✋ Привет, {Context.GetUserFullName()}!\n\n⚪ <b>Панель админа</b>");
				RowButton("💁 Режим обычного пользователя", Q(StartAdmin));
				RowButton("🗾 Показать товары", Q(ReadTable));
				RowButton("✅ Добавить товар", Q(CMS_ADD));
				RowButton("❌ Удалить товар", Q(CMS_DELETE));
				RowButton("📱 Изменить товар", Q(Edit_product));
				//RowButton("📄 Экспорт отчета в Excel");
				RowButton("💁💁 Показать администраторов",Q(ShowAdmins));
				RowButton("✅💁 Добавление администратора",Q(Admin_Add));
				RowButton("❌💁 Удаление администратора",Q(RemoveAdmin));
				RowButton("🔄 Обновить товары", Q(RefreshItems));
			}
			else
			{
				PushL(
					$"✋ <b>Привет, {Context.GetUserFullName()}!</b>\n🌷 <b>Городские цветы Каменск-Шахтинский</b> \n🟢 Лучшие ЦВЕТЫ в городе! \n🟢 Профессиональные флористы! \n🟢 Доставка курьерской службы в любой район!\n 🟢 Наш <i>telegram</i> канал: <a href='https://t.me/gorodskie_cveti_kamensk'>Городские Цветы Каменск</a>");
				RowButton("🔥  Новинки!", Q(Instagram));
				//	RowButton("🌷  Тюльпаны", Q(PressTulps));
				RowButton("🌹  Российские Розы", Q(PressRURoses));
				RowButton("🌹  Эквадорские Розы", Q(PressEQRoses));
				RowButton("🌸  Цветы в коробках", Q(PressBoxes));
				RowButton("💐  Букеты", Q(PressBouqets));
				RowButton("🧺  Корзины", Q(PressBaskets));
				RowButton("🧸  Мягкие игрушки", Q(PushItem, header_toys, 0, 999999, table_toys_cat));
				RowButton("🎈  Воздушные шары", Q(PushItem, header_baloons, 0, 999999, table_baloons_cat));
				RowButton("🍬  Конфеты", Q(PushItem, header_candy, 0, 999999, table_candy_cat));
				RowButton("🎂  Торты", Q(PushItem, header_cakes, 0, 999999, table_cakes_cat));
				RowButton("🍏  Фрукты", Q(PushItem, header_fruits, 0, 999999, table_fruits_cat));
				RowButton("🗾  Открытки", Q(PushItem, header_postcards, 0, 999999, table_postcards_cat));
				RowButton("🚚 Доставка", Q(PressDelivery));
				RowButton("🛒 Корзина", Q(PressMainBasket));
				Button("⭐ Отзывы", Q(PressRate));
				Button("📱 Контакты", Q(PressContact));
			}
		}

		//Админ старт
		[Action]
		public void StartAdmin()
		{
			PushL(
				$"✋ <b>Привет, {Context.GetUserFullName()}!</b>\n🌷 <b>Городские цветы Каменск-Шахтинский</b> \n🟢 Самые свежие цветы и букеты! \n🟢 Более 8 лет опыта и репутации! \n🟢 Наш <i>telegram</i> канал: <a href='https://t.me/gorodskie_cveti_kamensk'>Городские Цветы Каменск</a>");
			RowButton("🔥  Новинки!", Q(Instagram));
			//	RowButton("🌷  Тюльпаны", Q(PressTulps));
			RowButton("🌹  Российские Розы", Q(PressRURoses));
			RowButton("🌹  Эквадорские Розы", Q(PressEQRoses));
			RowButton("🌸  Цветы в коробках", Q(PressBoxes));
			RowButton("💐  Букеты", Q(PressBouqets));
			RowButton("🧺  Корзины", Q(PressBaskets));
			RowButton("🧸  Мягкие игрушки", Q(PushItem, header_toys, 0, 999999, table_toys_cat));
			RowButton("🎈  Воздушные шары", Q(PushItem, header_baloons, 0, 999999, table_baloons_cat));
			RowButton("🍬  Конфеты", Q(PushItem, header_candy, 0, 999999, table_candy_cat));
			RowButton("🎂  Торты", Q(PushItem, header_cakes, 0, 999999, table_cakes_cat));
			RowButton("🍏  Фрукты", Q(PushItem, header_fruits, 0, 999999, table_fruits_cat));
			RowButton("🗾  Открытки", Q(PushItem, header_postcards, 0, 999999, table_postcards_cat));
			RowButton("🚚 Доставка", Q(PressDelivery));
			RowButton("🛒 Корзина", Q(PressMainBasket));
			Button("⭐ Отзывы", Q(PressRate));
			Button("📱 Контакты", Q(PressContact));
			RowButton("💻 Вернуться в админку", Q(Start));
		}

		[Action]
		public void RefreshItems()
		{
			InlineKeyboardMarkup refreh = new InlineKeyboardMarkup(

				new InlineKeyboardButton[][]
				{
					new InlineKeyboardButton[]
					{

						InlineKeyboardButton.WithCallbackData(text: "✅ Обновить", callbackData: Q(FillProducts)),
					},
					new InlineKeyboardButton[]
					{
						InlineKeyboardButton.WithCallbackData(text: "❌ Стоп. Назад", callbackData: Q(Start)),

					}
				}
			);
			Client.SendTextMessageAsync(ChatId, "Точно обновить все товары?", ParseMode.Html, replyMarkup: refreh);
		}

		#endregion

		//Вторичная навигация

		#region 2ndNavigation

		//Навигация категорий
		//Тюльпаны
		[Action]
		public void PressTulps()
		{
			PushL("<b>Ценовые категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🟩 До 1500 рублей 🟩", Q(PushItem, header_tulps, 0, 1500, table_tulps_cat));
			RowButton("🟩 От 1500 До 2500 рублей 🟩", Q(PushItem, header_tulps, 1500, 2500, table_tulps_cat));
			RowButton("🟩 От 2500 До 3500 рублей 🟩", Q(PushItem, header_tulps, 2500, 3500, table_tulps_cat));
			RowButton("🟩 От 3500 До 5000 рублей 🟩", Q(PushItem, header_tulps, 3500, 5000, table_tulps_cat));
			RowButton("🟩 5000 рублей и выше 🟩", Q(PushItem, header_tulps, 5000, 999999, table_tulps_cat));
		}

		//Российские розы
		[Action]
		public void PressRURoses()
		{
			PushL("<b>Ценовые категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🟩 От 1600 До 2500 рублей 🟩", Q(PushItem, header_roses, 1600, 2500, table_roses_cat));
			RowButton("🟩 От 2500 До 3500 рублей 🟩", Q(PushItem, header_roses, 2500, 3500, table_roses_cat));
			RowButton("🟩 От 3500 До 5000 рублей 🟩", Q(PushItem, header_roses, 3500, 5000, table_roses_cat));
			RowButton("🟩 5000 рублей и выше 🟩", Q(PushItem, header_roses, 5000, 999999, table_roses_cat));
		}

		[Action]
		public void PressEQRoses()
		{
			PushL("<b>Ценовые категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🟩 От 1700 До 2500 рублей 🟩", Q(PushItem, header_equadorroses, 1700, 2500, table_equadorroses_cat));
			RowButton("🟩 От 2500 До 3500 рублей 🟩", Q(PushItem, header_equadorroses, 2500, 3500, table_equadorroses_cat));
			RowButton("🟩 От 3500 До 5000 рублей 🟩", Q(PushItem, header_equadorroses, 3500, 5000, table_equadorroses_cat));
			RowButton("🟩 5000 рублей и выше 🟩", Q(PushItem, header_equadorroses, 5000, 999999, table_equadorroses_cat));
		}

		//Цветы в коробках
		[Action]
		public void PressBoxes()
		{
			PushL("<b>Ценовые категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🟩 От 1600 До 2500 рублей 🟩", Q(PushItem, header_boxes, 1600, 2500, table_boxes_cat));
			RowButton("🟩 От 2500 До 3500 рублей 🟩", Q(PushItem, header_boxes, 2500, 3500, table_boxes_cat));
			RowButton("🟩 От 3500 До 5000 рублей 🟩", Q(PushItem, header_boxes, 3500, 5000, table_boxes_cat));
			RowButton("🟩 5000 рублей и выше 🟩", Q(PushItem, header_boxes, 5000, 999999, table_boxes_cat));
		}

		//Букеты
		[Action]
		public void PressBouqets()
		{
			PushL("<b>Категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🟩 До 1500 рублей 🟩", Q(PushItem, header_bouqets, 0, 1500, table_bouqets_cat));
			RowButton("🟩 От 1500 До 2500 рублей 🟩", Q(PushItem, header_bouqets, 1500, 2500, table_bouqets_cat));
			RowButton("🟩 От 2500 До 3500 рублей 🟩", Q(PushItem, header_bouqets, 2500, 3500, table_bouqets_cat));
			RowButton("🟩 От 3500 До 5000 рублей 🟩", Q(PushItem, header_bouqets, 3500, 5000, table_bouqets_cat));
			RowButton("🟩 5000 рублей и выше 🟩", Q(PushItem, header_bouqets, 5000, 999999, table_bouqets_cat));
		}

		//Корзины
		[Action]
		public void PressBaskets()
		{
			PushL("<b>Ценовые категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🟩 До 2500 рублей 🟩", Q(PushItem, header_baskets, 0, 2500, table_baskets_cat));
			RowButton("🟩 От 2500 До 4000 рублей 🟩", Q(PushItem, header_baskets, 2500, 4000, table_baskets_cat));
			RowButton("🟩 От 4000 До 7000 рублей 🟩", Q(PushItem, header_baskets, 4000, 7000, table_baskets_cat));
			RowButton("🟩 7000 рублей и выше 🟩", Q(PushItem, header_baskets, 7000, 999999, table_baskets_cat));
		}


		//Навигация допов
		//Контакты
		[Action]
		public async void PressContact()
		{
			PushL(
				"📱 <b>Контакты</b>\n📍 <b>Адрес:</b>\n пр.Карла Маркса, 54г.Каменск-Шахтинский\n(Режим работы: Круглосуточно)\n📍 <b>Адрес:</b>\n пр.Карла Маркса, 79, Каменск-Шахтинский\n(Режим работы: 7:00-20:00)\n📞 <b>Телефоны:</b>\n +7-928-180-63-88\n +7-918-576-10-88\n📧 <b>E-mail:</b>\n flowerskamensk@mail.ru\n🌐 <b>Сайт:</b>\n https://flowerskamensk.ru/\n📲 <b>Whatsapp:</b>\n +7-928-180-63-88\n🕰 <b>Прием заказов:</b>\n с 8:00-22:00");
			RowButton("⏪ Назад", Q(Start));
		}

		//Рейтинги
		[Action]
		public void PressRate()
		{
			PushL(
				"<b>Наш рейтинг:</b>\n\n4.8 ⭐ (РЕЙТИНГ ЯНДЕКС)\n*на основе 42 официальных отзывов в сервисах данной поисковой службы\n\n4.74 ⭐ (РЕЙТИНГ GOOGLE)\n*на основе 57 официальных отзывов в сервисах данной поисковой службы");
			RowButton("⏪ Назад", Q(Start));
		}

		//Стоимость доставки
		[Action]
		public async void PressDelivery()
		{
			PushL(
				"🚚 <b>Стоимость доставки</b>\n<b>Самовывоз</b> - 0 ₽\n<b>Каменск - Шахтинский(центр и мкр.60 лет Октября)</b> - 150 ₽\n<b>Комбинат(район)</b> - 200 ₽\n<b>Старая Станица(район)</b> - 300 ₽\n<b>Шахтёрский(район)</b> - 200 ₽\n<b>Южный(район)</b> - 250 ₽\n<b>Абрамовка(посёлок)</b> - 300 ₽\n<b>Астахов(хутор)</b> - 550 ₽\n<b>Богданов(хутор)</b> - 550 ₽\n<b>Вишневецкий</b> - 800 ₽\n<b>Волченский(хутор)</b> - 500 ₽\n<b>Глубокий(посёлок)</b> - 650 ₽\n<b>Данилов(хутор)</b> - 1200 ₽\n<b>Донецк РФ</b> - 900 ₽\n<b>Диченский(хутор)</b> - 400 ₽\n<b>Заводской(микрорайон)</b> - 450 ₽\n<b>Калитвенская(станица)</b> - 500 ₽\n<b>Красновка(хутор)</b> - 350 ₽\n<b>Леcной(посёлок)</b> - 300 ₽\n<b>Лиховской(Лихая)</b> - 600 ₽\n<b>Лихая(за переездом)</b> - 700 ₽\n<b>Лихой(хутор)</b> - 800 ₽\n<b>Малая Каменка(хутор)</b> - 400 ₽\n<b>Масаловка(хутор)</b> - 550 ₽\n<b>Нижнеговейный(хутор)</b> - 300 ₽\n<b>Углеродовский</b> - 850 ₽\n<b>Филлипенков(хутор)</b> - 400 ₽\n<b>Чистоозерный(посёлок)</b> - 550 ₽\n<b>Шахта 17</b> - 500 ₽");
			RowButton("⏪ Назад", Q(Start));
		}

		#endregion

		//Логика CMS

		#region CMSlogic

		//Получение CMS айтемов для юзеров
		[Action]
		public async void Instagram()
		{
			NpgsqlConnection DB = new NpgsqlConnection(PostgresConnectionString);
			DB.Open();
			NpgsqlCommand create = DB.CreateCommand();
			create.CommandText = "SELECT * FROM public.\"LimitedProducts\"";
			var reader = create.ExecuteReader();
			while (reader.Read())
			{
				SendCMS(Convert.ToInt32(reader["Id"]), reader["Image"].ToString(), reader["Name"].ToString(),
					reader["Price"].ToString(),reader["Description"].ToString());
			}

			DB.Close();
		}

		//Отправка товаров CMS юзерам
		public async Task SendCMS(int guid, string _imgurl, string _itemname, string _price,string _description)
		{
			InlineKeyboardMarkup inlineKeyboard = new(
				new[]
				{
					InlineKeyboardButton.WithCallbackData(text: "🛒 В корзину", callbackData: Q(CallDataCMS, guid)),
				}
			);
			await Client.SendPhotoAsync(ChatId, _imgurl,
				$"<b>{_itemname}</b>\n\n{_price}\n\nЦена: {_price} ₽\n\n🚚 Доставка или самовывоз",
				Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: inlineKeyboard);
		}

		//Получение и отправка CMS товаров админам
		[Action]
		private async void ReadTable()
		{
			NpgsqlConnection check_connection = new NpgsqlConnection(PostgresConnectionString);
			check_connection.Open();
			NpgsqlCommand check_command = check_connection.CreateCommand();
			check_command.CommandText = "SELECT count(*) FROM public.\"LimitedProducts\"";
			check_command.ExecuteNonQuery();
			int countRows = (int) (long) check_command.ExecuteScalar();
			check_connection.Close();
			if (countRows == 0)
			{
				await Client.SendTextMessageAsync(ChatId, "❌ Товаров нет! Зайди и добавь!", ParseMode.Html);
			}
			else
			{
				NpgsqlConnection DB = new NpgsqlConnection(PostgresConnectionString);
				DB.Open();
				NpgsqlCommand create = DB.CreateCommand();
				create.CommandText = "SELECT * FROM public.\"LimitedProducts\"";
				var reader = create.ExecuteReader();
				while (reader.Read())
				{
					await Client.SendPhotoAsync(ChatId, (InputOnlineFile)reader["Image"].ToString(),
						caption:
						$"ID:{reader["Id"]}\n<b>{reader["Name"]}</b>\n\nОписание: {reader["Description"]}\n\nЦена: {reader["Price"]} ₽\n\n🚚 Доставка или самовывоз",
						ParseMode.Html);
				}

				DB.Close();
			}
		}

		//Навигация создания карточки товара
		[Action]
		public void CMS_ADD()
		{
			PushL("Добавление товара");
			RowButton("⏪ Назад", Q(Start));
			Button("➕ Добавить", Q(Add_product));
		}

		//Формирование карточки
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
					new[]
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
				temp_cms.Img = update.Update.Message.Photo[update.Update.Message.Photo.Length - 1].FileId;
			}
			else
			{
				await Client.SendTextMessageAsync(ChatId, "❌ Ошибка! Повторите еще раз!");
				Start();
			}

			PushL("Добавь название товара:");
			await Send();
			temp_cms.Name = await AwaitText();
			PushL("Добавь описание товара:");
			await Send();
			temp_cms.Description = await AwaitText();
			PushL("Добавь стоимость товара:");
			await Send();
			temp_cms.Price = await AwaitText();
			
			await Client.SendTextMessageAsync(ChatId, $"Карточка товара сформирована!", ParseMode.Html,
				replyMarkup: product_sample);
		}

		//Final добавление в бд
		[Action]
		public async Task CMS_Create()
		{

			var product = new Products();
			product.Image = temp_cms.Img;
			product.Name = temp_cms.Name;
			product.Price = temp_cms.Price;
			product.Description = temp_cms.Description;
			
			AddProduct(product);
			await Client.SendTextMessageAsync(ChatId, $"✅ Товар {product.Name} успешно добавлен в категорию!");
		}

		//Предпросмотр карточки
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
			await Client.SendPhotoAsync(ChatId, temp_cms.Img,
				caption: $"<b>{temp_cms.Name}</b>\n\n{temp_cms.Description}\n\nЦена: {temp_cms.Price}\n\n🚚 Доставка или самовывоз",
				ParseMode.Html);
		}

		//Изменение карточки товаров
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
			new_product.Name = await AwaitText();
			PushL("Новое описание товара:");
			await Send();
			new_product.Description = await AwaitText();
			PushL("Новая стоимость товара:");
			await Send();
			new_product.Price = await AwaitText();
			new_product.Id = id;
			EditProduct(new_product);
			await Client.SendTextMessageAsync(ChatId, "✅ Карточка товара изменена!", ParseMode.Html,
				replyMarkup: removeItem);
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


		#endregion

		//Сервисы CMS CRUD

		#region CMSservices

		//CRUD CMS
		//Создание, обновление, удаление
		private int ExecuteWrite(string query, Dictionary<string, object> args)
		{
			int numberOfRowsAffected;

			using (var con = new NpgsqlConnection(PostgresConnectionString))
			{
				con.Open();

				using (var cmd = new NpgsqlCommand(query, con))
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
		private DataTable ExecuteRead(string query, Dictionary<string, object> args)
		{
			if (string.IsNullOrEmpty(query.Trim()))
				return null;

			using (var con = new NpgsqlConnection(PostgresConnectionString))
			{
				con.Open();
				using (var cmd = new NpgsqlCommand(query, con))
				{
					foreach (KeyValuePair<string, object> entry in args)
					{
						cmd.Parameters.AddWithValue(entry.Key, entry.Value);
					}
					
					var da = new NpgsqlDataAdapter(cmd);
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
			 string query = $"INSERT INTO public.\"LimitedProducts\"(\"Name\", \"Price\", \"Description\", \"Image\") VALUES(@Name, {product.Price},@Description,@Image)";

			var args = new Dictionary<string, object>
			{
				{"@Name", product.Name},
				
				{"@Description", product.Description},
				{"@Image", product.Image}
			};

			return ExecuteWrite(query, args);
		}

		//Изменение элемента по ID
		private int EditProduct(Products product)
		{
			//const string query = "UPDATE Products SET Image = @Image, Text = @Text, Price = @Price WHERE Id = @id";
			 string query = $"UPDATE public.\"LimitedProducts\" SET \"Name\"=@Name, \"Price\"={product.Price}, \"Description\"=@Description, \"Image\"=@Image WHERE \"Id\" = {product.Id}";
			var args = new Dictionary<string, object>
			{
				
				{"@Name", product.Name},
				{"@Description", product.Description},
				{"@Image", product.Image}
				
				
			};

			return ExecuteWrite(query, args);
		}
		//Удаление элемента по ID

		private int DeleteProduct(int id)
		{
			const string query = "DELETE FROM public.\"LimitedProducts\" WHERE \"Id\" = @Id";

			var args = new Dictionary<string, object>
			{
				{"@Id", id}
			};

			return ExecuteWrite(query, args);
		}

		//Получение элемента по ID
		private Products GetProductById(int id)
		{
			var query = "SELECT * FROM public.\"LimitedProducts\" WHERE \"Id\" = @Id";

			var args = new Dictionary<string, object>
			{
				{"@Id", id}
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
				Name = Convert.ToString(dt.Rows[0]["Name"]),
				Description = Convert.ToString(dt.Rows[0]["Description"]),
				Price = Convert.ToString(dt.Rows[0]["Price"])
			};
			return product;
		}


		#endregion

		//Парсинг и отправка

		#region PushingParsing

		//Парсинг
		public void ParseItem(string _baseurl, string header, string table_name)
		{
			
			NpgsqlConnection DB = new NpgsqlConnection(PostgresConnectionString);
			
			HtmlDocument HD = new HtmlDocument();
			var web = new HtmlWeb
			{
				AutoDetectEncoding = false,
				OverrideEncoding = Encoding.UTF8,
			};
			HD = web.Load(_baseurl + header);
			HtmlNodeCollection PricesElements = HD.DocumentNode.SelectNodes("//div[@class='product-item-price']");
			HtmlNodeCollection TitlesElements = HD.DocumentNode.SelectNodes("//div[@class='product-item__link']//a");
			HtmlNodeCollection UrlsElements = HD.DocumentNode.SelectNodes("//div[@class='product-item__content']//picture//img");
			// Проверяем наличие узлов
			if (PricesElements != null && TitlesElements!=null && UrlsElements!=null)
			{
				urls.Clear();
				titles.Clear();
				prices.Clear();
				//Fill Prices collection
				foreach (HtmlNode HN in PricesElements)
				{
					// Получаем строчки
					string outputText = HN.InnerText;
					prices.Add(outputText);

				}
				//Fill Titles collection
				foreach (HtmlNode Title in TitlesElements)
				{
					string outputText = Title.InnerText;
					titles.Add(outputText);
				}
				//Fill Urls collection
				foreach (HtmlNode Url in UrlsElements)
				{
					string outputText = Url.GetAttributeValue("src", "");
					urls.Add("https:" + outputText);
				}
			}
			
			if (prices.Count != 0 && titles.Count != 0 && urls.Count != 0)
			{
				DB.Open();
				NpgsqlCommand cmd = DB.CreateCommand();
				
				for (int i = 0; i < prices.Count; i++)
				{
					cmd.CommandText = $"INSERT INTO public.\"Products\"(\"ImageURL\", \"Name\", \"Price\", \"CategoryId\") VALUES ( '{urls.ElementAt(i)}','{titles.ElementAt(i)}',{string.Join("", prices.ElementAt(i).Where(c => char.IsDigit(c)))}, (SELECT \"Id\" AS \"CategoryId\" FROM public.\"ProductsCategories\" where \"Name\" = '{table_name}'))";
					cmd.ExecuteNonQuery();
					
				}
				DB.Close();
			}
			
			

		}

		[Action]
		public async Task FillProducts()
		{
			NpgsqlConnection DB = new NpgsqlConnection(PostgresConnectionString);
			DB.Open();
			
				NpgsqlCommand cmd = DB.CreateCommand();
				cmd.CommandText = "DELETE FROM public.\"Products\"";
				cmd.ExecuteNonQuery();
			
			DB.Close();
			ParseItem(baseurl, header_equadorroses, table_equadorroses);
			ParseItem(baseurl, header_roses, table_roses);
			ParseItem(baseurl, header_boxes, table_boxes);

			ParseItem(baseurl, header_bouqets, table_bouqets);

			ParseItem(baseurl, header_baskets, table_baskets);

			ParseItem(baseurl, header_toys, table_toys);

			ParseItem(baseurl, header_baloons, table_baloons);

			ParseItem(baseurl, header_candy, table_candy);

			ParseItem(baseurl, header_cakes, table_cakes);

			ParseItem(baseurl, header_fruits, table_fruits);

			ParseItem(baseurl, header_postcards, table_postcards);
			InlineKeyboardMarkup redirect_refresh = new InlineKeyboardMarkup(

				new InlineKeyboardButton[][]
				{
					new InlineKeyboardButton[]
					{

						InlineKeyboardButton.WithCallbackData(text: "💻 К Админке", callbackData: Q(Start)),
					},
					new InlineKeyboardButton[]
					{
						InlineKeyboardButton.WithCallbackData(text: "🌹 К товарам", callbackData: Q(StartAdmin)),

					}
				}
			);
			await Client.SendTextMessageAsync(ChatId, "✅ Товары в базе данных обновлены!", ParseMode.Html,
				replyMarkup: redirect_refresh);

		}

		//Сортировка и отправка спарсенных товаров
		[Action]
		public async Task PushItem(string _header, int from_price, int to_price, int categoryId)
		{
			await Send("⏳ Загрузка товаров...");
			NpgsqlConnection check_connection = new NpgsqlConnection(PostgresConnectionString);
			check_connection.Open();
			NpgsqlCommand check_command = check_connection.CreateCommand();
			check_command.CommandText = $"SELECT count(*) FROM public.\"Products\" WHERE \"CategoryId\" = {categoryId} AND \"Price\" BETWEEN CAST('{from_price}' AS money) AND CAST('{to_price}' AS money)";
			check_command.ExecuteNonQuery();
			int countRows = (int) (long) check_command.ExecuteScalar();
			check_connection.Close();
			if (countRows == 0)
			{
				await Send("В данной категории товары закончились 🥺");
			}
			else
			{
				NpgsqlConnection DB = new NpgsqlConnection(PostgresConnectionString);
				DB.Open();
				NpgsqlCommand create = DB.CreateCommand();
				create.CommandText = $"SELECT * FROM public.\"Products\" WHERE \"CategoryId\" = {categoryId} AND \"Price\" BETWEEN CAST('{from_price}' AS money) AND CAST('{to_price}' AS money)";
				var reader = create.ExecuteReader();
				while (reader.Read())
				{
					// string check = new string(reader["Price"].ToString().Where(t => char.IsDigit(t)).ToArray());
					// int price = Convert.ToInt32(check);
					// if (price >= from_price && price <= to_price)
					// {
						InlineKeyboardMarkup inlineKeyboard = new(
							new[]
							{
								InlineKeyboardButton.WithCallbackData(text: "🛒 В корзину",
									callbackData: Q(CallDataParse, Convert.ToInt32(reader["Id"]))),
							}
						);

						await Client.SendPhotoAsync(ChatId, reader["ImageURL"].ToString(),
							$"<b>{reader["Name"].ToString()}</b>\n\nЦена: {reader["Price"].ToString()} ₽\n\n🚚 Доставка или самовывоз",
							Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: inlineKeyboard);
					//}
				}
				DB.Close();
			}
		}
		#endregion

		//Доставка

		#region Delivery
		//Вывод айтемов доставки
		[Action]
		public async Task PushDelivery()
		{
			NpgsqlConnection DB = new NpgsqlConnection(PostgresConnectionString);
			await Client.SendTextMessageAsync(ChatId,
				"🚚 <b>Выберите зону(ы) доставки</b>\nСтоимость доставки добавится в корзину", ParseMode.Html);
			DB.Open();
			NpgsqlCommand create = DB.CreateCommand();
			create.CommandText = "SELECT * FROM \"Delivery\"";
			var reader = create.ExecuteReader();
			while (reader.Read())
			{
				InlineKeyboardMarkup inlineKeyboard = new(
					new[]
					{
						InlineKeyboardButton.WithCallbackData(text: "🛒 В корзину",
							callbackData: Q(DeliveryCall, Convert.ToInt32(reader["Id"]))),
					}
				);
				await Client.SendTextMessageAsync(ChatId,
					$"<b>{reader["District"].ToString()}</b> : +{reader["Price"].ToString()} ₽", ParseMode.Html,
					replyMarkup: inlineKeyboard);
			}
			DB.Close();
		}
		#endregion

		//Коллбэки

		#region Callbacks
		//Скип доставки
		[Action]
		public async Task CancelDelivery()
		{
			NpgsqlConnection USER = new NpgsqlConnection(PostgresConnectionString);
			USER.Open();
			NpgsqlCommand clear_cart = USER.CreateCommand();
			clear_cart.CommandText = $"DELETE FROM public.\"Cart\" WHERE \"UserId\" = {Convert.ToInt32(ChatId)}";
			clear_cart.ExecuteNonQuery();
			USER.Close();
			Start();
		}

		//Коллбэк доставки
		[Action]
		public async Task DeliveryCall(int guid)
		{

			InlineKeyboardMarkup make_order = new InlineKeyboardMarkup(

				new InlineKeyboardButton[][]
				{
					new InlineKeyboardButton[]
					{

						InlineKeyboardButton.WithCallbackData(text: "✅ Оформление заказа",
							callbackData: Q(NotificateOrder)),
					},
					new InlineKeyboardButton[]
					{
						InlineKeyboardButton.WithCallbackData(text: "❌ Отмена", callbackData: Q(CancelDelivery)),

					}
				}
			);
			NpgsqlConnection DB = new NpgsqlConnection(PostgresConnectionString);
			DB.Open();
			NpgsqlConnection DB1 = new NpgsqlConnection(PostgresConnectionString);
			DB1.Open();
			NpgsqlCommand create = DB.CreateCommand();
			create.CommandText = $"SELECT * FROM \"Delivery\" WHERE \"Id\" = {guid}";
			//create.Parameters.AddWithValue("@guid", guid);
			var reader = create.ExecuteReader();
			while (reader.Read())
			{
				//string _id = ShortId.Generate();
				NpgsqlCommand fillcart = DB1.CreateCommand();
				fillcart.CommandText = $"INSERT INTO public.\"Cart\"(\"Name\", \"Price\", \"UserId\") VALUES('{reader["District"].ToString()}',CAST({reader["Price"].ToString()} as MONEY),{Convert.ToInt32(ChatId)})";

				/*fillcart.Parameters.AddWithValue("@Id", _id);
				fillcart.Parameters.AddWithValue("@Name", "Доставка на " + reader["Name"].ToString());
				fillcart.Parameters.AddWithValue("@Price", reader["Price"].ToString());*/
				fillcart.ExecuteNonQuery();
				//shop_cart.Add(new Item("Доставка на "+reader["Name"].ToString(),reader["Price"].ToString()));
				await Client.SendTextMessageAsync(ChatId, $"✅ Зона доставки {reader["District"].ToString()} добавлена!",
					Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: make_order);
			}
			DB.Close();
			DB1.Close();
		}

		//Коллбэк полной очистки корзины
		[Action]
		public async Task CartDeleteCallData()
		{
			InlineKeyboardMarkup redirect_basket = new(
				new[]
				{
					InlineKeyboardButton.WithCallbackData(text: "⏪ Меню", callbackData: Q(Start)),
				}
			);
			NpgsqlConnection USER = new NpgsqlConnection(PostgresConnectionString);
			USER.Open();
			NpgsqlCommand clear_cart = USER.CreateCommand();
			clear_cart.CommandText = $"DELETE FROM public.\"Cart\" WHERE \"UserId\" = {Convert.ToInt32(ChatId)}";
			clear_cart.ExecuteNonQuery();
			USER.Close();
			await Client.SendTextMessageAsync(ChatId, "✅ Корзина очищена", Telegram.Bot.Types.Enums.ParseMode.Html,
				replyMarkup: redirect_basket);
		}

		//Коллбэк спарсенных товаров
		[Action]
		public async Task CallDataParse(int id)
		{
			InlineKeyboardMarkup redirect_basket = new(

				new[]
				{
					InlineKeyboardButton.WithCallbackData(text: "⏪ Меню", callbackData: Q(Start)),
					InlineKeyboardButton.WithCallbackData(text: "🛒 К корзине", callbackData: Q(PressMainBasket)),
				}

			);
			NpgsqlConnection DB = new NpgsqlConnection(PostgresConnectionString);
			NpgsqlConnection DB1 = new NpgsqlConnection(PostgresConnectionString);
			DB.Open();
			DB1.Open();
			NpgsqlCommand create = DB.CreateCommand();
			create.CommandText = $"SELECT * FROM public.\"Products\" WHERE \"Id\" = {id}";
			//create.Parameters.AddWithValue("@id", id);
			var reader = create.ExecuteReader();
			while (reader.Read())
			{
				//string _id = ShortId.Generate();
				NpgsqlCommand fillcart = DB1.CreateCommand();
				fillcart.CommandText = $"INSERT INTO public.\"Cart\"(\"Name\", \"Price\", \"UserId\") VALUES('{reader["Name"].ToString()}',CAST({reader["Price"].ToString()} as MONEY),{Convert.ToInt32(ChatId)})";
				//fillcart.Parameters.AddWithValue("@Id", id);
				// fillcart.Parameters.AddWithValue("@Name", reader["Name"].ToString());
				// fillcart.Parameters.AddWithValue("@Price", reader["Price"].ToString());
				// fillcart.Parameters.AddWithValue("@Price", reader["Price"].ToString());
				fillcart.ExecuteNonQuery();
				await Client.SendTextMessageAsync(ChatId, $"✅ Товар {reader["Name"].ToString()} добавлен в корзину!",
					Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: redirect_basket);
			}
			DB.Close();
			DB1.Close();
		}

		//Коллбэк CMS товаров
		[Action]
		public async Task CallDataCMS(int id)
		{
			InlineKeyboardMarkup redirect_basket = new(

				new[]
				{
					InlineKeyboardButton.WithCallbackData(text: "⏪ Меню", callbackData: Q(Start)),
					InlineKeyboardButton.WithCallbackData(text: "🛒 К корзине", callbackData: Q(PressMainBasket)),
				}

			);
			NpgsqlConnection DB = new NpgsqlConnection(PostgresConnectionString);
			NpgsqlConnection DB1 = new NpgsqlConnection(PostgresConnectionString);
			DB.Open();
			DB1.Open();
			NpgsqlCommand create = DB.CreateCommand();
			create.CommandText = $"SELECT * FROM \"LimitedProducts\" WHERE \"Id\" = {id}";
			//create.Parameters.AddWithValue("@guid", id);
			var reader = create.ExecuteReader();
			while (reader.Read())
			{
				//string _id = ShortId.Generate();
				NpgsqlCommand fillcart = DB1.CreateCommand();
				fillcart.CommandText = $"INSERT INTO public.\"Cart\"(\"Name\", \"Price\", \"UserId\") VALUES('{reader["Name"].ToString()}',CAST({reader["Price"].ToString()} as MONEY),{Convert.ToInt32(ChatId)})";

				// fillcart.Parameters.AddWithValue("@Id", _id);
				// fillcart.Parameters.AddWithValue("@Name", reader["Text"].ToString());
				// fillcart.Parameters.AddWithValue("@Price", reader["Price"].ToString());
				fillcart.ExecuteNonQuery();
				//shop_cart.Add(new Item(reader["Text"].ToString(),reader["Price"].ToString()));
				await Client.SendTextMessageAsync(ChatId, $"✅ Товар {reader["Name"].ToString()} добавлен в корзину!",
					Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: redirect_basket);
			}
			DB.Close();
			DB1.Close();
		}
		#endregion

		//Оформление заказа

		#region Order

		//Обработка сообщений пользователя и постройка уведомления нового заказа
		[Action]
		public async Task NotificateOrder()
		{
			NpgsqlConnection DB = new NpgsqlConnection(PostgresConnectionString);
			
			DB.Open();
			
			// NpgsqlCommand cleardata = DB.CreateCommand();
			// cleardata.CommandText = $"DELETE FROM data{ChatId.ToString()}";
			// cleardata.ExecuteNonQuery();
			NpgsqlCommand add = DB.CreateCommand();
			
				
			string c_name = "";
			string c_number = "";
			string r_name = "";
			string r_number = "";
			string address = "";
			string additional = "";
			//string id = random.Next(500000).ToString();
			//add.Parameters.AddWithValue("@Id", id);
			PushL("Пожалуйста, заполните форму ниже 👇");
			PushL("🙂 Ваше ФИО:");
			await Send();
			 c_name = await AwaitText();
			if (c_name.Equals("/start"))
			{
				Start();
			}
			else
			{
				//add.Parameters.AddWithValue("@C_name", c_name);
				PushL("📱 Ваш номер телефона:");
				await Send();
				 c_number = await AwaitText();
				if (c_number.Equals("/start"))
				{
					Start();
				}
				else
				{
					//add.Parameters.AddWithValue("@C_number", c_number);
					PushL("🙂 ФИО получателя:");
					await Send();
					 r_name = await AwaitText();
					if (r_name.Equals("/start"))
					{
						Start();
					}
					else
					{
						//add.Parameters.AddWithValue("@R_name", r_name);
						PushL("📱 Номер телефона получателя:");
						await Send();
						 r_number = await AwaitText();
						if (r_number.Equals("/start"))
						{
							Start();
						}
						else
						{
							//add.Parameters.AddWithValue("@R_number", r_number);
							PushL("🏠 Адрес получателя:");
							await Send(); address = await AwaitText();
							if (address.Equals("/start"))
							{
								Start();
							}
							else
							{
								//add.Parameters.AddWithValue("@Address", address);
								PushL("🗒 Дополнительные пожелания:");
								await Send(); 
								additional = await AwaitText();
								if (additional.Equals("/start"))
								{
									Start();
								}
								else
								{
									//Идентификатор заказ-товары
									var order_product_id = Guid.NewGuid();
									
									add.CommandText =
										"INSERT INTO public.\"Orders\"(\"Customer_name\", \"Customer_number\", \"Receiver_name\", \"Receiver_number\", \"Address\", \"Description\", \"Datetime\", \"Order_products_id\", \"UserId\")";
									add.CommandText += $" VALUES ( '{c_name}', '{c_name}', '{r_name}', '{r_number}', '{address}', '{additional}', '{DateTime.Now}', '{order_product_id.ToString()}',{ChatId});";
									add.ExecuteNonQuery();
									//add.Parameters.AddWithValue("@Additional", additional);
									//Заполнение таблицы Order_products
									NpgsqlConnection DB1 = new NpgsqlConnection(PostgresConnectionString);
									NpgsqlConnection DB2 = new NpgsqlConnection(PostgresConnectionString);
									NpgsqlConnection DB3 = new NpgsqlConnection(PostgresConnectionString);
									DB3.Open();
									NpgsqlCommand order_customer = DB3.CreateCommand();
									order_customer.CommandText = $"INSERT INTO public.\"OrderCustomer\"(\"OrderId\", \"CustomerId\") VALUES ('{order_product_id.ToString()}', {Convert.ToInt32(ChatId)});";
									order_customer.ExecuteNonQuery();
									DB3.Close();
									DB1.Open();
									DB2.Open();
									NpgsqlCommand read_Usercart = DB1.CreateCommand();
									read_Usercart.CommandText = $"SELECT * FROM \"Cart\" WHERE \"UserId\" = {Convert.ToInt32(ChatId)}";
									var reader_userCart = read_Usercart.ExecuteReader();
									NpgsqlCommand products_order = DB2.CreateCommand();
									while (reader_userCart.Read())
									{
										products_order.CommandText = $"INSERT INTO public.\"Order_products\"(\"Product_name\", \"Product_price\", \"OrderId\") VALUES ('{reader_userCart["Name"].ToString()}', CAST({reader_userCart["Price"].ToString()} as MONEY), '{order_product_id.ToString()}');";
										products_order.ExecuteNonQuery();
									}
									
									DB1.Close();
									DB2.Close();
									//Инлайн оформления заказа
									InlineKeyboardMarkup back_menu = new(
										new[]
										{
											InlineKeyboardButton.WithCallbackData(text: "⏪ Меню",
												callbackData: Q(Start)),
										}
									);
									//Увед в приватный канал
									string Notif_message = "";
									DB3.Open();
									NpgsqlCommand read_notif = DB3.CreateCommand();
									read_notif.CommandText = $"SELECT  * FROM public.\"Orders\" WHERE \"Order_products_id\" = '{order_product_id.ToString()}'";
									
									var reader = read_notif.ExecuteReader();
									while (reader.Read())
									{
										Notif_message +=
											$"🟨 <b>Новый заказ! #{reader["Id"].ToString()}</b>\n===============\n<b>Заказчик:</b> {reader["Customer_name"].ToString()} \nНомер заказчика: {reader["Customer_number"].ToString()}\nТелега заказчика: @{Context.GetUsername()} \n===============\n<b>Получатель:</b> {reader["Receiver_name"].ToString()} \nНомер получателя: {reader["Receiver_number"].ToString()} \n===============\n<b>Адрес:</b> {reader["Address"].ToString()} \n===============\n<b>Дополнительно:</b> {reader["Description"].ToString()} \n===============\n<b>Заказанные товары</b> 👇\n";
										await Client.SendTextMessageAsync(ChatId,
											$"✅ <b>Заказ оформлен!</b>\nНомер заказа: #{reader["Id"].ToString()}\nВ ближайшее время с вами свяжется менеджер для подтверждения заказа и способа оплаты!",
											ParseMode.Html, replyMarkup: back_menu);

									}
									DB3.Close();
									DB3.Open();
									NpgsqlCommand read_cart = DB3.CreateCommand();
									read_cart.CommandText = $"SELECT * FROM public.\"Order_products\" WHERE \"OrderId\" = '{order_product_id.ToString()}'";
									var reader_cart = read_cart.ExecuteReader();
									double result_price = 0;
									while (reader_cart.Read())
									{
										Notif_message +=
											$"⭐ Товар: {reader_cart["Product_name"].ToString()}  Стоимость: {reader_cart["Product_price"].ToString()} ₽\n ";
										//string check = new string(reader_cart["Product_price"].ToString()
											//.Where(t => char.IsDigit(t)).ToArray());
										//int price = Convert.ToInt32(check);
										result_price += Convert.ToDouble(reader_cart["Product_price"]);
									}
									DB3.Close();
									Notif_message += $"===============\n<b>Итоговая сумма</b>: {result_price.ToString()} ₽";
									//Фикс
									await Notif.SendTextMessageAsync(chatId: notif_chatid, text: $"{Notif_message}",
										Telegram.Bot.Types.Enums.ParseMode.Html);
									DB3.Open();
									NpgsqlCommand clear_cart = DB3.CreateCommand();
									clear_cart.CommandText = $"DELETE FROM public.\"Cart\" WHERE \"UserId\" = {Convert.ToInt32(ChatId)}";
									clear_cart.ExecuteNonQuery();
									DB3.Close();
									DB.Close();
								}
							}
						}
					}
				}
			}
			DB.Close();
			
		}
		#endregion

		//Корзина

		#region Basket

		//Вывод товаров корзины
		[Action]
		public async Task PressMainBasket()
		{

			InlineKeyboardMarkup create_order = new InlineKeyboardMarkup(

				new InlineKeyboardButton[][]
				{
					
					new InlineKeyboardButton[]
					{

						InlineKeyboardButton.WithCallbackData(text: "⏪ Назад", callbackData: Q(Start)),
						InlineKeyboardButton.WithCallbackData(text: "❌ Очистить", callbackData: Q(CartDeleteCallData)),
					},
					new InlineKeyboardButton[]
					{
						InlineKeyboardButton.WithCallbackData(text: "🚚 Выбор зон(ы) доставки",
							callbackData: Q(PushDelivery)),

					},
				}
			);
			//Маркап пустой корзины
			InlineKeyboardMarkup redirect = new(
				new[]
				{
					InlineKeyboardButton.WithCallbackData(text: "⏪ Меню", callbackData: Q(Start)),
				}

			);
			await Client.SendTextMessageAsync(ChatId,
				"🛒 <b>Корзина:</b>\n❗ Доставка шаров, тортов и игрушек осуществляется только вместе с доставкой букета!\n❗ Для оформления заказа выберите зону(ы) доставки и заполните форму 😉\n",
				Telegram.Bot.Types.Enums.ParseMode.Html);
			NpgsqlConnection check_connection = new NpgsqlConnection(PostgresConnectionString);
			check_connection.Open();
			NpgsqlCommand check_command = check_connection.CreateCommand();
			check_command.CommandText = $"SELECT count(*) FROM \"Cart\" WHERE \"UserId\" = {Convert.ToInt32(ChatId)}";
			check_command.ExecuteNonQuery();
			int countRows = (int) (long) check_command.ExecuteScalar();
			check_connection.Close();

			if (countRows == 0)
			{
				await Client.SendTextMessageAsync(ChatId, $"Вы ничего не добавили в корзину 😔",
					Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: redirect);
			}
			else
			{
				double result_price = 0;
				NpgsqlConnection DB = new NpgsqlConnection(PostgresConnectionString);
				DB.Open();
				NpgsqlCommand create = DB.CreateCommand();
				create.CommandText = $"SELECT * FROM \"Cart\" WHERE \"UserId\" = {Convert.ToInt32(ChatId)}";
				var reader = create.ExecuteReader();
				while (reader.Read())
				{
					InlineKeyboardMarkup delete_cart = new(
						new[]
						{
							InlineKeyboardButton.WithCallbackData(text: "❌ Удалить товар",
								callbackData: Q(DeleteAtId, Convert.ToInt32(reader["Id"]))),
						}

					);
					await Client.SendTextMessageAsync(ChatId,
						$"⭐ {reader["Name"].ToString()} : {reader["Price"].ToString()} ₽",
						Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: delete_cart);
					//string check = new string(reader["Price"].ToString().Where(t => char.IsDigit(t)).ToArray());
					//double price = Convert.ToDouble(check);
					result_price += Convert.ToDouble(reader["Price"]);;
				}

				DB.Close();
				await Client.SendTextMessageAsync(ChatId, $"💰 Сумма заказа: {result_price.ToString()} ₽",
					Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: create_order);
			}
		}

		//Удаление одного товара из корзины по ID
		[Action]
		public async Task DeleteAtId(int id)
		{
			InlineKeyboardMarkup redirect_basket = new(
				new[]
				{
					InlineKeyboardButton.WithCallbackData(text: "⏪ Меню", callbackData: Q(Start)),
					InlineKeyboardButton.WithCallbackData(text: "🛒 К корзине", callbackData: Q(PressMainBasket)),
				}

			);
			NpgsqlConnection DB = new NpgsqlConnection(PostgresConnectionString);
			DB.Open();
			NpgsqlCommand delID = DB.CreateCommand();
			delID.CommandText = $"DELETE  FROM \"Cart\" WHERE \"Id\" = {id}";
			//delID.Parameters.AddWithValue("@id", id);
			delID.ExecuteNonQuery();
			await Client.SendTextMessageAsync(ChatId, "✅ Товар удалён", Telegram.Bot.Types.Enums.ParseMode.Html,
				replyMarkup: redirect_basket);
		}

		#endregion

		//Обработчки и Конструкторы

		#region Handlers

		//Обработчки ошибок
		[On(Handle.Exception)]
		public async Task OnException(Exception e)
		{
			_logger.LogError(e, "Unhandled exception");
			if (Context.Update.Type == UpdateType.CallbackQuery)
			{
				await AnswerCallback("Ошибка, используйте кнопку Меню");
			}
			else if (Context.Update.Type == UpdateType.Message)
			{
				Push("Ошибка, используйте кнопку Меню");
			}
		}

		//Обработчик исключений
		[On(Handle.Unknown)]
		public void Unknown()
		{
			PushL("Команда не распознана!");
		}

		//Конструктор контроллера
		public MainController(ILogger<MainController> logger)
		{
			_logger = logger;
		}

		#endregion
		
		//Администрование

		#region AdminsAccess
		
		[Action]
		public void Admin_Add()
		{
			InlineKeyboardMarkup refreh = new InlineKeyboardMarkup(

				new InlineKeyboardButton[][]
				{
					new InlineKeyboardButton[]
					{

						InlineKeyboardButton.WithCallbackData(text: "✅ Добавить", callbackData: Q(AddAdmin)),
					},
					new InlineKeyboardButton[]
					{
						InlineKeyboardButton.WithCallbackData(text: "❌ Стоп. Назад", callbackData: Q(Start)),

					}
				}
			);
			Client.SendTextMessageAsync(ChatId, "Точно добавить администратора?", ParseMode.Html, replyMarkup: refreh);
		}
		[Action]
		public async Task ShowAdmins()
		{
			string admin_message = "";
			NpgsqlConnection DB = new NpgsqlConnection(PostgresConnectionString);
			DB.Open();
			NpgsqlCommand cmd = DB.CreateCommand();
			cmd.CommandText = "SELECT * FROM public.\"AdminAccess\"";
			var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				admin_message += $"\n🌹 ID: {reader["Id"].ToString()}\n Администратор: {reader["Name"].ToString()}\n ChatId: {reader["ChatId"].ToString()}\n =============\n";
			}
			InlineKeyboardMarkup back_menu = new(
				new[]
				{
					InlineKeyboardButton.WithCallbackData(text: "⏪ Меню",
						callbackData: Q(Start)),
				}
			);
			await Client.SendTextMessageAsync(ChatId,
				admin_message,
				ParseMode.Html, replyMarkup: back_menu);
		}
		[Action]
		public async Task RemoveAdmin()
		{
			
			NpgsqlConnection DB = new NpgsqlConnection(PostgresConnectionString);
			DB.Open();
			NpgsqlCommand cmd = DB.CreateCommand();
			string admin_id = "";
			PushL("🗒 Введите ID администратора для удаления:");
			await Send(); 
			admin_id = await AwaitText();
			if (admin_id.Equals("/start"))
			{
				Start();
			}
			cmd.CommandText = $"DELETE FROM public.\"AdminAccess\" WHERE \"Id\" = {Convert.ToInt32(admin_id)}";
			var reader = cmd.ExecuteNonQuery();
			
			InlineKeyboardMarkup back_menu = new(
				new[]
				{
					InlineKeyboardButton.WithCallbackData(text: "⏪ Меню",
						callbackData: Q(Start)),
				}
			);
			await Client.SendTextMessageAsync(ChatId,
				"✅ Администратор удалён!",
				ParseMode.Html, replyMarkup: back_menu);
		}
		[Action]
		public async Task AddAdmin()
		{
			
			NpgsqlConnection DB = new NpgsqlConnection(PostgresConnectionString);
			DB.Open();
			NpgsqlCommand cmd = DB.CreateCommand();
			string admin_Name = "";
			string admin_ChatId = "";
			PushL("🗒 Введите имя администратора:");
			await Send(); 
			admin_Name = await AwaitText();
			if (admin_Name.Equals("/start"))
			{
				Start();
			}
			else
			{
				PushL("🗒 Введите ChatId администратора:");
				await Send(); 
				admin_ChatId = await AwaitText();
				if (admin_ChatId.Equals("/start"))
				{
					Start();
				}
				else
				{
					cmd.CommandText = $"INSERT INTO public.\"AdminAccess\" (\"Name\",\"ChatId\") VALUES ('{admin_Name}','{admin_ChatId}')";
					var reader = cmd.ExecuteNonQuery();
			
					InlineKeyboardMarkup back_menu = new(
						new[]
						{
							InlineKeyboardButton.WithCallbackData(text: "⏪ Меню",
								callbackData: Q(Start)),
						}
					);
					await Client.SendTextMessageAsync(ChatId,
						"✅ Администратор добавлен!",
						ParseMode.Html, replyMarkup: back_menu);
				}
			}
			
		}
		#endregion
	}
}

