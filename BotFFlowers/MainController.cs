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

namespace BotFFlowers
{
	public class MainController : BotController
	{
		//Константы и токены

		#region Constants
		//Бот отправкм заказов в приватный канал
		private static TelegramBotClient
			Notif = new TelegramBotClient("5355673985:AAFi055Qt0RpnApk7eOwn1P_kLDmr1HZD_Y");

		//Админ ChatID
		private static string admin_chatid = "387549112";
		private static string admin_chatid2 = "727043884";

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
			SQLiteConnection USER = new SQLiteConnection("Data Source=DBFlowers.db;");
			USER.Open();
			SQLiteCommand cart_table = USER.CreateCommand();
			cart_table.CommandText =
				$"CREATE TABLE IF NOT EXISTS  cart{ChatId.ToString()} ( Id TEXT, Name TEXT, Price TEXT)";
			cart_table.ExecuteNonQuery();
			SQLiteCommand data_table = USER.CreateCommand();
			data_table.CommandText =
				$"CREATE TABLE IF NOT EXISTS  data{ChatId.ToString()} ( Id TEXT, C_name TEXT, C_number TEXT, R_name TEXT, R_number TEXT, Address TEXT, Additional TEXT)";
			data_table.ExecuteNonQuery();
			USER.Close();
			//Проверка на админа
			if (ChatId.ToString().Equals(admin_chatid) || ChatId.ToString().Equals(admin_chatid2))
			{
				PushL($"✋ Привет, {Context.GetUserFullName()}!\n\n⚪ <b>Панель админа + CMS</b>");
				RowButton("💁 Режим обычного пользователя", Q(StartAdmin));
				RowButton("🗾 Показать товары", Q(ReadTable));
				RowButton("✅ Добавить товар", Q(CMS_ADD));
				RowButton("❌ Удалить товар", Q(CMS_DELETE));
				RowButton("📱 Изменить товар", Q(Edit_product));
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
				RowButton("🧸  Мягкие игрушки", Q(PushItem, header_toys, 0, 999999, table_toys));
				RowButton("🎈  Воздушные шары", Q(PushItem, header_baloons, 0, 999999, table_baloons));
				RowButton("🍬  Конфеты", Q(PushItem, header_candy, 0, 999999, table_candy));
				RowButton("🎂  Торты", Q(PushItem, header_cakes, 0, 999999, table_cakes));
				RowButton("🍏  Фрукты", Q(PushItem, header_fruits, 0, 999999, table_fruits));
				RowButton("🗾  Открытки", Q(PushItem, header_postcards, 0, 999999, table_postcards));
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
			RowButton("🧸  Мягкие игрушки", Q(PushItem, header_toys, 0, 999999, table_toys));
			RowButton("🎈  Воздушные шары", Q(PushItem, header_baloons, 0, 999999, table_baloons));
			RowButton("🍬  Конфеты", Q(PushItem, header_candy, 0, 999999, table_candy));
			RowButton("🎂  Торты", Q(PushItem, header_cakes, 0, 999999, table_cakes));
			RowButton("🍏  Фрукты", Q(PushItem, header_fruits, 0, 999999, table_fruits));
			RowButton("🗾  Открытки", Q(PushItem, header_postcards, 0, 999999, table_postcards));
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
			RowButton("🟩 До 1500 рублей 🟩", Q(PushItem, header_tulps, 0, 1500, table_tulps));
			RowButton("🟩 От 1500 До 2500 рублей 🟩", Q(PushItem, header_tulps, 1500, 2500, table_tulps));
			RowButton("🟩 От 2500 До 3500 рублей 🟩", Q(PushItem, header_tulps, 2500, 3500, table_tulps));
			RowButton("🟩 От 3500 До 5000 рублей 🟩", Q(PushItem, header_tulps, 3500, 5000, table_tulps));
			RowButton("🟩 5000 рублей и выше 🟩", Q(PushItem, header_tulps, 5000, 999999, table_tulps));
		}

		//Российские розы
		[Action]
		public void PressRURoses()
		{
			PushL("<b>Ценовые категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🟩 От 1600 До 2500 рублей 🟩", Q(PushItem, header_roses, 1600, 2500, table_roses));
			RowButton("🟩 От 2500 До 3500 рублей 🟩", Q(PushItem, header_roses, 2500, 3500, table_roses));
			RowButton("🟩 От 3500 До 5000 рублей 🟩", Q(PushItem, header_roses, 3500, 5000, table_roses));
			RowButton("🟩 5000 рублей и выше 🟩", Q(PushItem, header_roses, 5000, 999999, table_roses));
		}

		[Action]
		public void PressEQRoses()
		{
			PushL("<b>Ценовые категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🟩 От 1700 До 2500 рублей 🟩", Q(PushItem, header_equadorroses, 1700, 2500, table_equadorroses));
			RowButton("🟩 От 2500 До 3500 рублей 🟩", Q(PushItem, header_equadorroses, 2500, 3500, table_equadorroses));
			RowButton("🟩 От 3500 До 5000 рублей 🟩", Q(PushItem, header_equadorroses, 3500, 5000, table_equadorroses));
			RowButton("🟩 5000 рублей и выше 🟩", Q(PushItem, header_equadorroses, 5000, 999999, table_equadorroses));
		}

		//Цветы в коробках
		[Action]
		public void PressBoxes()
		{
			PushL("<b>Ценовые категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🟩 От 1600 До 2500 рублей 🟩", Q(PushItem, header_boxes, 1600, 2500, table_boxes));
			RowButton("🟩 От 2500 До 3500 рублей 🟩", Q(PushItem, header_boxes, 2500, 3500, table_boxes));
			RowButton("🟩 От 3500 До 5000 рублей 🟩", Q(PushItem, header_boxes, 3500, 5000, table_boxes));
			RowButton("🟩 5000 рублей и выше 🟩", Q(PushItem, header_boxes, 5000, 999999, table_boxes));
		}

		//Букеты
		[Action]
		public void PressBouqets()
		{
			PushL("<b>Категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🟩 До 1500 рублей 🟩", Q(PushItem, header_bouqets, 0, 1500, table_bouqets));
			RowButton("🟩 От 1500 До 2500 рублей 🟩", Q(PushItem, header_bouqets, 1500, 2500, table_bouqets));
			RowButton("🟩 От 2500 До 3500 рублей 🟩", Q(PushItem, header_bouqets, 2500, 3500, table_bouqets));
			RowButton("🟩 От 3500 До 5000 рублей 🟩", Q(PushItem, header_bouqets, 3500, 5000, table_bouqets));
			RowButton("🟩 5000 рублей и выше 🟩", Q(PushItem, header_bouqets, 5000, 999999, table_bouqets));
		}

		//Корзины
		[Action]
		public void PressBaskets()
		{
			PushL("<b>Ценовые категории:</b>");
			RowButton("⏪ Назад", Q(Start));
			RowButton("🟩 До 2500 рублей 🟩", Q(PushItem, header_baskets, 0, 2500, table_baskets));
			RowButton("🟩 От 2500 До 4000 рублей 🟩", Q(PushItem, header_baskets, 2500, 4000, table_baskets));
			RowButton("🟩 От 4000 До 7000 рублей 🟩", Q(PushItem, header_baskets, 4000, 7000, table_baskets));
			RowButton("🟩 7000 рублей и выше 🟩", Q(PushItem, header_baskets, 7000, 999999, table_baskets));
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
			SQLiteConnection DB = new SQLiteConnection("Data Source=DBFlowers.db;");
			DB.Open();
			SQLiteCommand create = DB.CreateCommand();
			create.CommandText = "SELECT * FROM Products";
			SQLiteDataReader reader = create.ExecuteReader();
			while (reader.Read())
			{
				SendCMS(reader["Guid"].ToString(), reader["Image"].ToString(), reader["Text"].ToString(),
					reader["Price"].ToString());
			}

			DB.Close();
		}

		//Отправка товаров CMS юзерам
		public async Task SendCMS(string guid, string _imgurl, string _itemname, string _price)
		{
			InlineKeyboardMarkup inlineKeyboard = new(
				new[]
				{
					InlineKeyboardButton.WithCallbackData(text: "🛒 В корзину", callbackData: Q(CallDataCMS, guid)),
				}
			);
			await Client.SendPhotoAsync(ChatId, _imgurl,
				$"<b>{_itemname}</b>\n\nЦена: {_price}\n\n🚚 Доставка или самовывоз",
				Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: inlineKeyboard);
		}

		//Получение и отправка CMS товаров админам
		[Action]
		private async void ReadTable()
		{
			SQLiteConnection check_connection = new SQLiteConnection("Data Source=DBFlowers.db;");
			check_connection.Open();
			SQLiteCommand check_command = check_connection.CreateCommand();
			check_command.CommandText = "SELECT count(rowid) FROM Products";
			check_command.ExecuteNonQuery();
			int countRows = (int) (long) check_command.ExecuteScalar();
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
					await Client.SendPhotoAsync(ChatId, (InputOnlineFile)reader["Image"].ToString(),
						caption:
						$"ID:{reader["Id"]}\n<b>{reader["Text"]}</b>\n\nЦена: {reader["Price"]}\n\n🚚 Доставка или самовывоз",
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
			temp_cms.Text = await AwaitText();
			PushL("Добавь стоимость товара:");
			await Send();
			temp_cms.Price = await AwaitText() + " ₽";
			string guid = Guid.NewGuid().ToString("N");
			temp_cms.guid = guid;
			await Client.SendTextMessageAsync(ChatId, $"Карточка товара сформирована!", ParseMode.Html,
				replyMarkup: product_sample);
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
				caption: $"<b>{temp_cms.Text}</b>\n\nЦена: {temp_cms.Price}\n\n🚚 Доставка или самовывоз",
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
			new_product.Text = await AwaitText();
			PushL("Новая стоимость товара:");
			await Send();
			new_product.Price = await AwaitText() + " ₽";
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
		private DataTable ExecuteRead(string query, Dictionary<string, object> args)
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
				{"@Price", product.Price},
				{"@Guid", product.guid}
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


		#endregion

		//Парсинг и отправка

		#region PushingParsing

		//Парсинг
		public void ParseItem(string _baseurl, string header, string table_name)
		{
			SQLiteConnection DB = new SQLiteConnection("Data Source=DBFlowers.db;");
			urls.Clear();
			titles.Clear();
			prices.Clear();
			HtmlDocument HD = new HtmlDocument();
			var web = new HtmlWeb
			{
				AutoDetectEncoding = false,
				OverrideEncoding = Encoding.UTF8,
			};
			HD = web.Load(_baseurl + header);
			HtmlNodeCollection PricesElements = HD.DocumentNode.SelectNodes("//div[@class='product-item-price']");
			HtmlNodeCollection TitlesElements = HD.DocumentNode.SelectNodes("//div[@class='product-item__link']//a");
			HtmlNodeCollection UrlsElements =
				HD.DocumentNode.SelectNodes("//div[@class='product-item__content']//picture//img");
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

			switch (table_name)
			{
				case "tulpany":
					for (int i = 0; i < prices.Count; i++)
					{
						string id = i.ToString();
						DB.Open();
						SQLiteCommand add = DB.CreateCommand();
						add.CommandText = $"INSERT INTO tulpany VALUES(@Id, @Image,@Name,@Price)";
						add.Parameters.AddWithValue("@Id", id);
						add.Parameters.AddWithValue("@Image", urls.ElementAt(i));
						add.Parameters.AddWithValue("@Name", titles.ElementAt(i));
						add.Parameters.AddWithValue("@Price", prices.ElementAt(i));
						add.ExecuteNonQuery();
						DB.Close();
					}

					break;
				case "rossiyskierozy":
					for (int i = 0; i < prices.Count; i++)
					{
						string id = i.ToString();
						DB.Open();
						SQLiteCommand add = DB.CreateCommand();
						add.CommandText = $"INSERT INTO rossiyskierozy VALUES(@Id, @Image,@Name,@Price)";
						add.Parameters.AddWithValue("@Id", id);
						add.Parameters.AddWithValue("@Image", urls.ElementAt(i));
						add.Parameters.AddWithValue("@Name", titles.ElementAt(i));
						add.Parameters.AddWithValue("@Price", prices.ElementAt(i));
						add.ExecuteNonQuery();
						DB.Close();
					}

					break;
				case "gollandskierozy":
					for (int i = 0; i < prices.Count; i++)
					{
						string id = i.ToString();
						DB.Open();
						SQLiteCommand add = DB.CreateCommand();
						add.CommandText = $"INSERT INTO gollandskierozy VALUES(@Id, @Image,@Name,@Price)";
						add.Parameters.AddWithValue("@Id", id);
						add.Parameters.AddWithValue("@Image", urls.ElementAt(i));
						add.Parameters.AddWithValue("@Name", titles.ElementAt(i));
						add.Parameters.AddWithValue("@Price", prices.ElementAt(i));
						add.ExecuteNonQuery();
						DB.Close();
					}

					break;
				case "cvetivkorobkah":
					for (int i = 0; i < prices.Count; i++)
					{
						string id = i.ToString();
						DB.Open();
						SQLiteCommand add = DB.CreateCommand();
						add.CommandText = $"INSERT INTO cvetivkorobkah VALUES(@Id, @Image,@Name,@Price)";
						add.Parameters.AddWithValue("@Id", id);
						add.Parameters.AddWithValue("@Image", urls.ElementAt(i));
						add.Parameters.AddWithValue("@Name", titles.ElementAt(i));
						add.Parameters.AddWithValue("@Price", prices.ElementAt(i));
						add.ExecuteNonQuery();
						DB.Close();
					}

					break;
				case "bukety":
					for (int i = 0; i < prices.Count; i++)
					{
						string id = i.ToString();
						DB.Open();
						SQLiteCommand add = DB.CreateCommand();
						add.CommandText = $"INSERT INTO bukety VALUES(@Id, @Image,@Name,@Price)";
						add.Parameters.AddWithValue("@Id", id);
						add.Parameters.AddWithValue("@Image", urls.ElementAt(i));
						add.Parameters.AddWithValue("@Name", titles.ElementAt(i));
						add.Parameters.AddWithValue("@Price", prices.ElementAt(i));
						add.ExecuteNonQuery();
						DB.Close();
					}

					break;
				case "korziny":
					for (int i = 0; i < prices.Count; i++)
					{
						string id = i.ToString();
						DB.Open();
						SQLiteCommand add = DB.CreateCommand();
						add.CommandText = $"INSERT INTO korziny VALUES(@Id, @Image,@Name,@Price)";
						add.Parameters.AddWithValue("@Id", id);
						add.Parameters.AddWithValue("@Image", urls.ElementAt(i));
						add.Parameters.AddWithValue("@Name", titles.ElementAt(i));
						add.Parameters.AddWithValue("@Price", prices.ElementAt(i));
						add.ExecuteNonQuery();
						DB.Close();
					}

					break;
				case "plusheviemishki":
					for (int i = 0; i < prices.Count; i++)
					{
						string id = i.ToString();
						DB.Open();
						SQLiteCommand add = DB.CreateCommand();
						add.CommandText = $"INSERT INTO plusheviemishki VALUES(@Id, @Image,@Name,@Price)";
						add.Parameters.AddWithValue("@Id", id);
						add.Parameters.AddWithValue("@Image", urls.ElementAt(i));
						add.Parameters.AddWithValue("@Name", titles.ElementAt(i));
						add.Parameters.AddWithValue("@Price", prices.ElementAt(i));
						add.ExecuteNonQuery();
						DB.Close();
					}

					break;
				case "vozdushnieshary":
					for (int i = 0; i < prices.Count; i++)
					{
						string id = i.ToString();
						DB.Open();
						SQLiteCommand add = DB.CreateCommand();
						add.CommandText = $"INSERT INTO vozdushnieshary VALUES(@Id, @Image,@Name,@Price)";
						add.Parameters.AddWithValue("@Id", id);
						add.Parameters.AddWithValue("@Image", urls.ElementAt(i));
						add.Parameters.AddWithValue("@Name", titles.ElementAt(i));
						add.Parameters.AddWithValue("@Price", prices.ElementAt(i));
						add.ExecuteNonQuery();
						DB.Close();
					}

					break;
				case "konfety":
					for (int i = 0; i < prices.Count; i++)
					{
						string id = i.ToString();
						DB.Open();
						SQLiteCommand add = DB.CreateCommand();
						add.CommandText = $"INSERT INTO konfety VALUES(@Id, @Image,@Name,@Price)";
						add.Parameters.AddWithValue("@Id", id);
						add.Parameters.AddWithValue("@Image", urls.ElementAt(i));
						add.Parameters.AddWithValue("@Name", titles.ElementAt(i));
						add.Parameters.AddWithValue("@Price", prices.ElementAt(i));
						add.ExecuteNonQuery();
						DB.Close();
					}

					break;
				case "torty":
					for (int i = 0; i < prices.Count; i++)
					{
						string id = i.ToString();
						DB.Open();
						SQLiteCommand add = DB.CreateCommand();
						add.CommandText = $"INSERT INTO torty VALUES(@Id, @Image,@Name,@Price)";
						add.Parameters.AddWithValue("@Id", id);
						add.Parameters.AddWithValue("@Image", urls.ElementAt(i));
						add.Parameters.AddWithValue("@Name", titles.ElementAt(i));
						add.Parameters.AddWithValue("@Price", prices.ElementAt(i));
						add.ExecuteNonQuery();
						DB.Close();
					}

					break;
				case "fruktyvkorzine":
					for (int i = 0; i < prices.Count; i++)
					{
						string id = i.ToString();
						DB.Open();
						SQLiteCommand add = DB.CreateCommand();
						add.CommandText = $"INSERT INTO fruktyvkorzine VALUES(@Id, @Image,@Name,@Price)";
						add.Parameters.AddWithValue("@Id", id);
						add.Parameters.AddWithValue("@Image", urls.ElementAt(i));
						add.Parameters.AddWithValue("@Name", titles.ElementAt(i));
						add.Parameters.AddWithValue("@Price", prices.ElementAt(i));
						add.ExecuteNonQuery();
						DB.Close();
					}

					break;
				case "otkrytki":
					for (int i = 0; i < prices.Count; i++)
					{
						string id = i.ToString();
						DB.Open();
						SQLiteCommand add = DB.CreateCommand();
						add.CommandText = $"INSERT INTO otkrytki VALUES(@Id, @Image,@Name,@Price)";
						add.Parameters.AddWithValue("@Id", id);
						add.Parameters.AddWithValue("@Image", urls.ElementAt(i));
						add.Parameters.AddWithValue("@Name", titles.ElementAt(i));
						add.Parameters.AddWithValue("@Price", prices.ElementAt(i));
						add.ExecuteNonQuery();
						DB.Close();
					}
					break;
			}

		}

		[Action]
		public async Task FillProducts()
		{
			SQLiteConnection DB = new SQLiteConnection("Data Source=DBFlowers.db;");
			DB.Open();
			foreach (var item in tables)
			{
				SQLiteCommand clear = DB.CreateCommand();
				clear.CommandText = $"DELETE FROM {item}";
				clear.ExecuteNonQuery();
			}
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
		public async Task PushItem(string _header, int from_price, int to_price, string table_name)
		{
			await Send("⏳ Загрузка товаров...");
			SQLiteConnection check_connection = new SQLiteConnection("Data Source=DBFlowers.db;");
			check_connection.Open();
			SQLiteCommand check_command = check_connection.CreateCommand();
			check_command.CommandText = $"SELECT count(rowid) FROM {table_name}";
			check_command.ExecuteNonQuery();
			int countRows = (int) (long) check_command.ExecuteScalar();
			check_connection.Close();
			if (countRows == 0)
			{
				await Send("В данной категории товары закончились 🥺");
			}
			else
			{
				SQLiteConnection DB = new SQLiteConnection("Data Source=DBFlowers.db;");
				DB.Open();
				SQLiteCommand create = DB.CreateCommand();
				create.CommandText = $"SELECT * FROM {table_name}";
				SQLiteDataReader reader = create.ExecuteReader();
				while (reader.Read())
				{
					string check = new string(reader["Price"].ToString().Where(t => char.IsDigit(t)).ToArray());
					int price = Convert.ToInt32(check);
					if (price >= from_price && price <= to_price)
					{
						InlineKeyboardMarkup inlineKeyboard = new(
							new[]
							{
								InlineKeyboardButton.WithCallbackData(text: "🛒 В корзину",
									callbackData: Q(CallDataParse, reader["Id"].ToString(), table_name)),
							}
						);

						await Client.SendPhotoAsync(ChatId, reader["Image"].ToString(),
							$"<b>{reader["Name"].ToString()}</b>\n\nЦена: {reader["Price"].ToString()}\n\n🚚 Доставка или самовывоз",
							Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: inlineKeyboard);
					}
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
			SQLiteConnection DB = new SQLiteConnection("Data Source=DBFlowers.db;");
			await Client.SendTextMessageAsync(ChatId,
				"🚚 <b>Выберите зону(ы) доставки</b>\nСтоимость доставки добавится в корзину", ParseMode.Html);
			DB.Open();
			SQLiteCommand create = DB.CreateCommand();
			create.CommandText = "SELECT * FROM Delivery";
			SQLiteDataReader reader = create.ExecuteReader();
			while (reader.Read())
			{
				InlineKeyboardMarkup inlineKeyboard = new(
					new[]
					{
						InlineKeyboardButton.WithCallbackData(text: "🛒 В корзину",
							callbackData: Q(DeliveryCall, reader["Guid"].ToString())),
					}
				);
				await Client.SendTextMessageAsync(ChatId,
					$"<b>{reader["Name"].ToString()}</b> : +{reader["Price"].ToString()}", ParseMode.Html,
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
			SQLiteConnection USER = new SQLiteConnection("Data Source=DBFlowers.db;");
			USER.Open();
			SQLiteCommand clear_cart = USER.CreateCommand();
			clear_cart.CommandText = $"DELETE FROM cart{ChatId.ToString()}";
			clear_cart.ExecuteNonQuery();
			USER.Close();
			Start();
		}

		//Коллбэк доставки
		[Action]
		public async Task DeliveryCall(string guid)
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
			SQLiteConnection DB = new SQLiteConnection("Data Source=DBFlowers.db;");
			DB.Open();
			SQLiteCommand create = DB.CreateCommand();
			create.CommandText = "SELECT * FROM Delivery WHERE Guid = @guid";
			create.Parameters.AddWithValue("@guid", guid);
			SQLiteDataReader reader = create.ExecuteReader();
			while (reader.Read())
			{
				string _id = ShortId.Generate();
				SQLiteCommand fillcart = DB.CreateCommand();
				fillcart.CommandText = $"INSERT INTO cart{ChatId.ToString()} VALUES(@Id,@Name,@Price)";
				fillcart.Parameters.AddWithValue("@Id", _id);
				fillcart.Parameters.AddWithValue("@Name", "Доставка на " + reader["Name"].ToString());
				fillcart.Parameters.AddWithValue("@Price", reader["Price"].ToString());
				fillcart.ExecuteNonQuery();
				//shop_cart.Add(new Item("Доставка на "+reader["Name"].ToString(),reader["Price"].ToString()));
				await Client.SendTextMessageAsync(ChatId, $"✅ Зона доставки {reader["Name"].ToString()} добавлена!",
					Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: make_order);
			}
			DB.Close();
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
			SQLiteConnection USER = new SQLiteConnection("Data Source=DBFlowers.db;");
			USER.Open();
			SQLiteCommand clear_cart = USER.CreateCommand();
			clear_cart.CommandText = $"DELETE FROM cart{ChatId.ToString()}";
			clear_cart.ExecuteNonQuery();
			USER.Close();
			await Client.SendTextMessageAsync(ChatId, "✅ Корзина очищена", Telegram.Bot.Types.Enums.ParseMode.Html,
				replyMarkup: redirect_basket);
		}

		//Коллбэк спарсенных товаров
		[Action]
		public async Task CallDataParse(string id, string table_name)
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
			create.CommandText = $"SELECT * FROM {table_name} WHERE Id = @id";
			create.Parameters.AddWithValue("@id", id);
			SQLiteDataReader reader = create.ExecuteReader();
			while (reader.Read())
			{
				string _id = ShortId.Generate();
				SQLiteCommand fillcart = DB.CreateCommand();
				fillcart.CommandText = $"INSERT INTO cart{ChatId.ToString()} VALUES(@Id,@Name,@Price)";
				fillcart.Parameters.AddWithValue("@Id", _id);
				fillcart.Parameters.AddWithValue("@Name", reader["Name"].ToString());
				fillcart.Parameters.AddWithValue("@Price", reader["Price"].ToString());
				fillcart.ExecuteNonQuery();
				await Client.SendTextMessageAsync(ChatId, $"✅ Товар {reader["Name"].ToString()} добавлен в корзину!",
					Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: redirect_basket);
			}
			DB.Close();
		}

		//Коллбэк CMS товаров
		[Action]
		public async Task CallDataCMS(string id)
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
				string _id = ShortId.Generate();
				SQLiteCommand fillcart = DB.CreateCommand();
				fillcart.CommandText = $"INSERT INTO cart{ChatId.ToString()} VALUES(@Id,@Name,@Price)";
				fillcart.Parameters.AddWithValue("@Id", _id);
				fillcart.Parameters.AddWithValue("@Name", reader["Text"].ToString());
				fillcart.Parameters.AddWithValue("@Price", reader["Price"].ToString());
				fillcart.ExecuteNonQuery();
				//shop_cart.Add(new Item(reader["Text"].ToString(),reader["Price"].ToString()));
				await Client.SendTextMessageAsync(ChatId, $"✅ Товар {reader["Text"].ToString()} добавлен в корзину!",
					Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: redirect_basket);
			}
			DB.Close();
		}
		#endregion

		//Оформление заказа

		#region Order

		//Обработка сообщений пользователя и постройка уведомления нового заказа
		[Action]
		public async Task NotificateOrder()
		{
			SQLiteConnection DB = new SQLiteConnection("Data Source=DBFlowers.db;");

			DB.Open();
			SQLiteCommand cleardata = DB.CreateCommand();
			cleardata.CommandText = $"DELETE FROM data{ChatId.ToString()}";
			cleardata.ExecuteNonQuery();
			SQLiteCommand add = DB.CreateCommand();
			add.CommandText =
				$"INSERT INTO data{ChatId.ToString()} VALUES(@Id, @C_name,@C_number,@R_name,@R_number,@Address,@Additional)";
			string id = random.Next(500000).ToString();
			add.Parameters.AddWithValue("@Id", id);
			PushL("Пожалуйста, заполните форму ниже 👇");
			PushL("🙂 Ваше ФИО:");
			await Send();
			string c_name = await AwaitText();
			if (c_name.Equals("/start"))
			{
				Start();
			}
			else
			{
				add.Parameters.AddWithValue("@C_name", c_name);
				PushL("📱 Ваш номер телефона:");
				await Send();
				string c_number = await AwaitText();
				if (c_number.Equals("/start"))
				{
					Start();
				}
				else
				{
					add.Parameters.AddWithValue("@C_number", c_number);
					PushL("🙂 ФИО получателя:");
					await Send();
					string r_name = await AwaitText();
					if (r_name.Equals("/start"))
					{
						Start();
					}
					else
					{
						add.Parameters.AddWithValue("@R_name", r_name);
						PushL("📱 Номер телефона получателя:");
						await Send();
						string r_number = await AwaitText();
						if (r_number.Equals("/start"))
						{
							Start();
						}
						else
						{
							add.Parameters.AddWithValue("@R_number", r_number);
							PushL("🏠 Адрес получателя:");
							await Send();
							string address = await AwaitText();
							if (address.Equals("/start"))
							{
								Start();
							}
							else
							{
								add.Parameters.AddWithValue("@Address", address);
								PushL("🗒 Дополнительные пожелания:");
								await Send();
								string additional = await AwaitText();
								if (additional.Equals("/start"))
								{
									Start();
								}
								else
								{
									add.Parameters.AddWithValue("@Additional", additional);
									add.ExecuteNonQuery();
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
									SQLiteCommand read_notif = DB.CreateCommand();
									read_notif.CommandText = $"SELECT * FROM data{ChatId.ToString()}";
									SQLiteDataReader reader = read_notif.ExecuteReader();
									while (reader.Read())
									{
										Notif_message +=
											$"🟨 <b>Новый заказ! #{reader["Id"].ToString()}</b>\n===============\n<b>Заказчик:</b> {reader["C_name"].ToString()} \nНомер заказчика: {reader["C_number"].ToString()}\nТелега заказчика: @{Context.GetUsername()} \n===============\n<b>Получатель:</b> {reader["R_name"].ToString()} \nНомер получателя: {reader["R_number"].ToString()} \n===============\n<b>Адрес:</b> {reader["Address"].ToString()} \n===============\n<b>Дополнительно:</b> {reader["Additional"].ToString()} \n===============\n<b>Заказанные товары</b> 👇\n";
										await Client.SendTextMessageAsync(ChatId,
											$"✅ <b>Заказ оформлен!</b>\nНомер заказа: #{reader["Id"].ToString()}\nВ ближайшее время с вами свяжется менеджер для подтверждения заказа и способа оплаты!",
											ParseMode.Html, replyMarkup: back_menu);

									}

									SQLiteCommand read_cart = DB.CreateCommand();
									read_cart.CommandText = $"SELECT * FROM cart{ChatId.ToString()}";
									SQLiteDataReader reader_cart = read_cart.ExecuteReader();
									int result_price = 0;
									while (reader_cart.Read())
									{
										Notif_message +=
											$"⭐ Товар: {reader_cart["Name"].ToString()}  Стоимость: {reader_cart["Price"].ToString()}\n ";
										string check = new string(reader_cart["Price"].ToString()
											.Where(t => char.IsDigit(t)).ToArray());
										int price = Convert.ToInt32(check);
										result_price += price;
									}

									Notif_message += $"===============\n<b>Итоговая сумма</b>: {result_price} ₽";
									//Фикс
									await Notif.SendTextMessageAsync(chatId: notif_chatid, text: $"{Notif_message}",
										Telegram.Bot.Types.Enums.ParseMode.Html);
									SQLiteCommand clear_cart = DB.CreateCommand();
									clear_cart.CommandText = $"DELETE FROM cart{ChatId.ToString()}";
									clear_cart.ExecuteNonQuery();
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
			SQLiteConnection check_connection = new SQLiteConnection("Data Source=DBFlowers.db;");
			check_connection.Open();
			SQLiteCommand check_command = check_connection.CreateCommand();
			check_command.CommandText = $"SELECT count(rowid) FROM cart{ChatId.ToString()}";
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
				int result_price = 0;
				SQLiteConnection DB = new SQLiteConnection("Data Source=DBFlowers.db;");
				DB.Open();
				SQLiteCommand create = DB.CreateCommand();
				create.CommandText = $"SELECT * FROM cart{ChatId.ToString()}";
				SQLiteDataReader reader = create.ExecuteReader();
				while (reader.Read())
				{
					InlineKeyboardMarkup delete_cart = new(
						new[]
						{
							InlineKeyboardButton.WithCallbackData(text: "❌ Удалить товар",
								callbackData: Q(DeleteAtId, reader["Id"].ToString())),
						}

					);
					await Client.SendTextMessageAsync(ChatId,
						$"⭐ {reader["Name"].ToString()} : {reader["Price"].ToString()}",
						Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: delete_cart);
					string check = new string(reader["Price"].ToString().Where(t => char.IsDigit(t)).ToArray());
					int price = Convert.ToInt32(check);
					result_price += price;
				}

				DB.Close();
				await Client.SendTextMessageAsync(ChatId, $"💰 Сумма заказа: {result_price} ₽",
					Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: create_order);
			}
		}

		//Удаление одного товара из корзины по ID
		[Action]
		public async Task DeleteAtId(string id)
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
			SQLiteCommand delID = DB.CreateCommand();
			delID.CommandText = $"DELETE  FROM cart{ChatId.ToString()} WHERE Id = @id";
			delID.Parameters.AddWithValue("@id", id);
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

	}
}

