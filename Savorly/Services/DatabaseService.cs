using Savorly.Data;
using Savorly.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

namespace Savorly.Services
{
    public static class DatabaseService
    {
        public static void InitializeDatabase()
        {
            using var context = new AppDbContext();

            try
            {
                context.Database.EnsureDeleted();
                bool created = context.Database.EnsureCreated();

                if (created || !context.Recipes.Any())
                {
                    AddSampleData(context);
                    System.Diagnostics.Debug.WriteLine("Database initialized with recipes!");
                }

                PrintDatabaseInfo(context);

                var foodCount = context.Recipes.Count(r => r.Type == RecipeType.Food);
                var drinkCount = context.Recipes.Count(r => r.Type == RecipeType.Drink);
                var totalRecipes = context.Recipes.Count();
                var totalIngredients = context.Ingredients.Count();
                var totalSteps = context.Steps.Count();
                var totalTags = context.Tags.Count();

                System.Diagnostics.Debug.WriteLine("=== ДЕТАЛЬНА ПЕРЕВІРКА БАЗИ ДАНИХ ===");
                System.Diagnostics.Debug.WriteLine($"🍳 Страв: {foodCount}");
                System.Diagnostics.Debug.WriteLine($"🥤 Напоїв: {drinkCount}");
                System.Diagnostics.Debug.WriteLine($"📊 Всього рецептів: {totalRecipes}");
                System.Diagnostics.Debug.WriteLine($"🧅 Всього інгредієнтів: {totalIngredients}");
                System.Diagnostics.Debug.WriteLine($"👨‍🍳 Всього кроків: {totalSteps}");
                System.Diagnostics.Debug.WriteLine($"🏷️ Всього тегів: {totalTags}");

                var drinks = context.Recipes.Where(r => r.Type == RecipeType.Drink).Take(5).ToList();
                System.Diagnostics.Debug.WriteLine("🔍 Перші 30 напоїв у базі:");
                foreach (var drink in drinks)
                {
                    System.Diagnostics.Debug.WriteLine($"   - {drink.Title} (ID: {drink.RecipeId})");
                }

                System.Diagnostics.Debug.WriteLine("=====================================");

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Помилка ініціалізації бази: {ex.Message}");
            }
        }

        public static List<Recipe> GetRecipesByTypeWithDetails(RecipeType type)
        {
            using var context = new AppDbContext();
            return context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .Include(r => r.Tags)
                .Where(r => r.Type == type)
                .ToList();
        }

        public static List<Recipe> GetFavoriteRecipesWithDetails()
        {
            using var context = new AppDbContext();
            return context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .Include(r => r.Tags)
                .Where(r => r.IsFavorite)
                .ToList();
        }

        private static void PrintDatabaseInfo(AppDbContext context)
        {
            try
            {
                var userCount = context.Users.Count();
                var recipeCount = context.Recipes.Count();
                var foodCount = context.Recipes.Count(r => r.Type == RecipeType.Food);
                var drinkCount = context.Recipes.Count(r => r.Type == RecipeType.Drink);
                var favoriteCount = context.Recipes.Count(r => r.IsFavorite);

                System.Diagnostics.Debug.WriteLine("=== ІНФОРМАЦІЯ ПРО БАЗУ ДАНИХ ===");
                System.Diagnostics.Debug.WriteLine($"Користувачі: {userCount}");
                System.Diagnostics.Debug.WriteLine($"Рецепти: {recipeCount}");
                System.Diagnostics.Debug.WriteLine($"- Страви: {foodCount}");
                System.Diagnostics.Debug.WriteLine($"- Напої: {drinkCount}");
                System.Diagnostics.Debug.WriteLine($"- В обраному: {favoriteCount}");
                System.Diagnostics.Debug.WriteLine("================================");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Помилка отримання інформації про базу: {ex.Message}");
            }
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private static void AddSampleData(AppDbContext context)
        {
            var tags = AddTags(context);
            Add40FoodRecipes(context, tags);
            AddDrinkRecipes(context, tags);
            AddRecipeDetails(context);
        }

        private static List<Tag> AddTags(AppDbContext context)
        {
            var tags = new[]
            {
                new Tag { Name = "#сніданок" },
                new Tag { Name = "#обід" },
                new Tag { Name = "#вечеря" },
                new Tag { Name = "#десерт" },
                new Tag { Name = "#швидко" },
                new Tag { Name = "#здорове" },
                new Tag { Name = "#м'ясо" },
                new Tag { Name = "#риба" },
                new Tag { Name = "#салат" },
                new Tag { Name = "#суп" },
                new Tag { Name = "#випічка" },
                new Tag { Name = "#паста" },
                new Tag { Name = "#гриль" },
                new Tag { Name = "#свята" },
                new Tag { Name = "#легке" },
                new Tag { Name = "#кремове" },
                new Tag { Name = "#українська" },
                new Tag { Name = "#італійська" },
                new Tag { Name = "#азійська" },
                new Tag { Name = "#вегетаріанське" },
                new Tag { Name = "#курка" },
                new Tag { Name = "#яловичина" },
                new Tag { Name = "#лосось" },
                new Tag { Name = "#креветки" },
                new Tag { Name = "#сир" },
                new Tag { Name = "#овочі" },
                new Tag { Name = "#фрукти" },
                new Tag { Name = "#крем" },
                new Tag { Name = "#шоколад" },
                new Tag { Name = "#закуска" },
                new Tag { Name = "#основна страва" },
                new Tag { Name = "#національна" },
                new Tag { Name = "#традиційне" },
                new Tag { Name = "#сучасне" },
                new Tag { Name = "#для дітей" },
                new Tag { Name = "#піца" },
                new Tag { Name = "#суши" },
                new Tag { Name = "#бургер" },
                new Tag { Name = "#кава" },
                new Tag { Name = "#чай" },
                new Tag { Name = "#смузі" },
                new Tag { Name = "#сік" },
                new Tag { Name = "#лимонад" },
                new Tag { Name = "#коктейль" },
                new Tag { Name = "#молочний" },
                new Tag { Name = "#гарячий" },
                new Tag { Name = "#холодний" },
                new Tag { Name = "#освіжаючий" },
                new Tag { Name = "#зігріваючий" },
                new Tag { Name = "#фруктовий" },
                new Tag { Name = "#ягідний" },
                new Tag { Name = "#тропічний" },
                new Tag { Name = "#корисний" },
                new Tag { Name = "#вітамінний" },
                new Tag { Name = "#енергетичний" },
                new Tag { Name = "#десертний" },
                new Tag { Name = "#шоколадний" },
                new Tag { Name = "#ванільний" },
                new Tag { Name = "#м'ятний" },
                new Tag { Name = "#імбирний" },
                new Tag { Name = "#цитрусовий" },
                new Tag { Name = "#літній" },
                new Tag { Name = "#зимовий" },
                new Tag { Name = "#дитячий" },
                new Tag { Name = "#святковий" },
                new Tag { Name = "#ефектний" },
                new Tag { Name = "#легкий" },
                new Tag { Name = "#багатий" },
                new Tag { Name = "#ароматний" },
                new Tag { Name = "#нєжний" },
                new Tag { Name = "#кислий" },
                new Tag { Name = "#солодкий" },
                new Tag { Name = "#основній" },
                new Tag { Name = "#додатковий" },
                new Tag { Name = "#безалкогольні" }
            };

            context.Tags.AddRange(tags);
            context.SaveChanges();
            return tags.ToList();
        }

        private static void Add40FoodRecipes(AppDbContext context, List<Tag> tags)
        {
            var foodRecipes = new List<Recipe>
            {
                new Recipe
                {
                    Title = "Сирники з ягодами",
                    ShortDescription = "Ніжні домашні сирники з полуницею та малиною",
                    Description = "Традиційні українські сирники, приготовані з натурального творогу та свіжих ягід. Ідеальний сніданок для всієї родини.",
                    ImagePath = "https://images.unsplash.com/photo-1563174573-62b036d41ba0?auto=format&fit=crop&w=400&h=300",
                    PreparationTime = 25,
                    Servings = 3,
                    Type = RecipeType.Food,
                    IsFavorite = false,
                    CreatedBy = "chef_maria"
                },
new Recipe
{
    Title = "Омлет з овочами",
    ShortDescription = "Поживний омлет з перцем, помідорами та цибулею",
    Description = "Легкий та корисний омлет з свіжими сезонними овочами. Багато білка та вітамінів для енергії на цілий день.",
    ImagePath = "https://images.unsplash.com/photo-1755531567087-b1169c42aa3c?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 15,
    Servings = 2,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "admin"
},
new Recipe
{
    Title = "Авокадо-тост з яйцем",
    ShortDescription = "Сучасний сніданок з авокадо, яйцем пашот та насінням",
    Description = "Сучасний та корисний сніданок, багатий на корисні жири та білки. Ідеально для здорового початку дня.",
    ImagePath = "https://images.unsplash.com/photo-1548365329-701a48b295b3?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 10,
    Servings = 1,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "john_cook"
},
new Recipe
{
    Title = "Вівсяна каша з фруктами",
    ShortDescription = "Кремова вівсянка з бананом, ягодами та медом",
    Description = "Тепла затишна каша, яка насичує надовго. Чудовий вибір для сніданку взимку або коли потрібен заряд енергії.",
    ImagePath = "https://images.unsplash.com/photo-1702648982253-8b851013e81f?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 20,
    Servings = 2,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "chef_maria"
},
new Recipe
{
    Title = "Французькі тости",
    ShortDescription = "Хліб, запечений в яєчній суміші з ваніллю",
    Description = "Класичні французькі тости з хрусткою скоринкою та ніжною серединкою. Подаються з кленовим сиропом та ягодами.",
    ImagePath = "https://images.unsplash.com/photo-1484723091739-30a097e8f929?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 15,
    Servings = 2,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "admin"
},

new Recipe
{
    Title = "Салат Цезар з куркою",
    ShortDescription = "Класичний салат з гренками та пармезаном",
    Description = "Всесвітньо відомий салат з ніжною куркою, хрусткими гренками та унікальним соусом Цезар.",
    ImagePath = "https://images.unsplash.com/photo-1546793665-c74683f339c1?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 20,
    Servings = 2,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "chef_maria"
},
new Recipe
{
    Title = "Паста Карбонара",
    ShortDescription = "Італійська паста з беконом та сирним соусом",
    Description = "Автентична римська паста з гуанчале, яйцем та пекорино романо. Секрет у правильній консистенції соусу.",
    ImagePath = "https://images.unsplash.com/photo-1608756687911-aa1599ab3bd9?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 30,
    Servings = 2,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "john_cook"
},
new Recipe
{
    Title = "Курячий суп з локшиною",
    ShortDescription = "Ароматний суп з домашньою куркою та овочами",
    Description = "Затишний суп, який лікує не тільки від застуди, але й від поганого настрою. Готується на міцному курячому бульйоні.",
    ImagePath = "https://images.unsplash.com/photo-1607330289024-1535c6b4e1c1?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 45,
    Servings = 4,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "ukrainian_chef"
},
new Recipe
{
    Title = "Лазанья з м'ясом",
    ShortDescription = "Шари пасти, м'ясного рагу та сирного соусу",
    Description = "Італійська класика - шари пасти лазанья, соковите м'ясне рагу та ніжний сирний соус бешамель.",
    ImagePath = "https://images.unsplash.com/photo-1619894991209-9f9694be045a?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 90,
    Servings = 6,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "chef_maria"
},
new Recipe
{
    Title = "Грецький салат",
    ShortDescription = "Свіжий салат з фетою, оливками та овочами",
    Description = "Легкий та освіжаючий салат з свіжих сезонних овочів, сиру фета та оливок. Ідеально підходить для літнього обіду.",
    ImagePath = "https://images.unsplash.com/photo-1599021419847-d8a7a6aba5b4?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 15,
    Servings = 2,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "john_cook"
},
new Recipe
{
    Title = "Бургери з яловичиною",
    ShortDescription = "Соковиті бургери з домашньою булочкою",
    Description = "Соковиті бургери з яловичим фаршем, свіжими овочами та домашнім соусом. Ідеальний обід для вихідного дня.",
    ImagePath = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 40,
    Servings = 4,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "admin"
},
new Recipe
{
    Title = "Рамен з куркою",
    ShortDescription = "Японський суп з локшиною та яйцем",
    Description = "Автентичний японський рамен з куркою, яйцем та овочами. Багатий бульйон готується кілька годин для ідеального смаку.",
    ImagePath = "https://images.unsplash.com/photo-1740813626726-d811be64edbf?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 60,
    Servings = 2,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "john_cook"
},
new Recipe
{
    Title = "Кускус з овочами",
    ShortDescription = "Легка страва з кускусу та сезонних овочів",
    Description = "Вегетаріанська страва з кускусу, свіжих овочів та прянощів. Ідеально підходить для легкого обіду.",
    ImagePath = "https://images.unsplash.com/photo-1607116685391-29c9e9726561?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 25,
    Servings = 3,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "chef_maria"
},

new Recipe
{
    Title = "Лосось у соусі терякі",
    ShortDescription = "Ніжний лосось з солодко-солоним соусом",
    Description = "Лосось, запечений у соусі терякі з медом, імбиром та часником. Подається з рисом та овочами на пару.",
    ImagePath = "https://images.unsplash.com/photo-1718522200359-96a72b8994b1?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 35,
    Servings = 2,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "john_cook"
},
new Recipe
{
    Title = "Курячі котлети",
    ShortDescription = "Ніжні котлети з курячого фаршу",
    Description = "Курячі котлети з панірувальною скоринкою, соковиті всередині. Чудово поєднуються з картоплею пюре або гречкою.",
    ImagePath = "https://plus.unsplash.com/premium_photo-1711477343719-fcf22c5a28a5?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 35,
    Servings = 4,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "ukrainian_chef"
},
new Recipe
{
    Title = "Овочеве рагу",
    ShortDescription = "Тушковані овочі в томатному соусі",
    Description = "Ароматне овочеве рагу з баклажанами, кабачками, перцем та томатами. Вегетаріанська насолода.",
    ImagePath = "https://plus.unsplash.com/premium_photo-1664391973158-a232138c87fa?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 40,
    Servings = 4,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "chef_maria"
},
new Recipe
{
    Title = "Стейк з яловичини",
    ShortDescription = "Соковитий стейк з картоплею фрі",
    Description = "Ідеально приготований стейк з яловичини з рожевою серединкою. Подається з картоплею фрі та соусом беарнез.",
    ImagePath = "https://images.unsplash.com/photo-1677027201352-3c3981cb8b5c?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 25,
    Servings = 2,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "admin"
},
new Recipe
{
    Title = "Креветки в часниковому соусі",
    ShortDescription = "Креветки обсмажені з часником та перцем",
    Description = "Великі креветки, швидко обсмажені в оливковій олії з часником, перцем чилі та лимонним соком.",
    ImagePath = "https://images.unsplash.com/photo-1659951226926-a75791782250?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 20,
    Servings = 2,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "john_cook"
},
new Recipe
{
    Title = "Фажітас з куркою",
    ShortDescription = "Мексиканська страва з куркою та перцем",
    Description = "Курка з перцем та цибулею, приготована в мексиканському стилі. Подається з тортильями та гуакамоле.",
    ImagePath = "https://plus.unsplash.com/premium_photo-1679986029475-d1e6519903d6?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 30,
    Servings = 3,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "chef_maria"
},
new Recipe
{
    Title = "Голубці з м'ясом",
    ShortDescription = "Традиційні українські голубці",
    Description = "Капустяні листя з м'ясно-рисовою начинкою, тушковані в томатному соусі. Справжня смакота української кухні.",
    ImagePath = "https://images.unsplash.com/photo-1622220734058-23ce1f89d84d?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 120,
    Servings = 6,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "ukrainian_chef"
},

new Recipe
{
    Title = "Шоколадний торт",
    ShortDescription = "Багатошаровий шоколадний торт",
    Description = "Розкішний шоколадний торт з ніжним кремом та шоколадною глазур'ю. Ідеальний для святкового столу.",
    ImagePath = "https://images.unsplash.com/photo-1578985545062-69928b1d9587?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 90,
    Servings = 8,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "chef_maria"
},
new Recipe
{
    Title = "Чізкейк Нью-Йорк",
    ShortDescription = "Класичний чізкейк з печивом",
    Description = "Ніжний кремовий чізкейк з основою з печива. Традиційний рецепт з Нью-Йорка з ідеальною текстурою.",
    ImagePath = "https://images.unsplash.com/photo-1524351199678-941a58a3df50?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 180,
    Servings = 10,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "admin"
},
new Recipe
{
    Title = "Яблучний пиріг",
    ShortDescription = "Пісочний пиріг з яблуками та корицею",
    Description = "Ароматний яблучний пиріг з пісочного тіста з додаванням кориці та ванілі. Затишна домашня випічка.",
    ImagePath = "https://plus.unsplash.com/premium_photo-1694336203192-c9e7f2891b95?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 60,
    Servings = 6,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "ukrainian_chef"
},
new Recipe
{
    Title = "Тірамісу",
    ShortDescription = "Італійський десерт з кави та маскарпоне",
    Description = "Класичний італійський десерт з просочених кавою бісквітів та ніжного крему з маскарпоне.",
    ImagePath = "https://images.unsplash.com/photo-1571877227200-a0d98ea607e9?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 45,
    Servings = 6,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "john_cook"
},
new Recipe
{
    Title = "Млинці з варенням",
    ShortDescription = "Тонкі млинці з полуничним варенням",
    Description = "Традиційні українські млинці, тонкі та ніжні. Подаються з домашнім полуничним варенням та сметаною.",
    ImagePath = "https://images.unsplash.com/photo-1676287258876-388ff4f4b48b?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 30,
    Servings = 4,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "ukrainian_chef"
},
new Recipe
{
    Title = "Шоколадний мусс",
    ShortDescription = "Повітряний шоколадний десерт",
    Description = "Легкий та повітряний шоколадний мусс з темного шоколаду. Ідеальний легкий десерт після вечері.",
    ImagePath = "https://images.unsplash.com/photo-1603032305813-be7441bc1037?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 20,
    Servings = 4,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "chef_maria"
},

new Recipe
{
    Title = "Борщ український",
    ShortDescription = "Традиційний борщ з м'ясом та сметаною",
    Description = "Легендарний український борщ з яловичиною, свіжою капустою, буряком та картоплею. Подається з часником та сметаною.",
    ImagePath = "https://images.unsplash.com/photo-1648726445011-9fbf3a5ddb90?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 90,
    Servings = 6,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "ukrainian_chef"
},
new Recipe
{
    Title = "Вареники з картоплею",
    ShortDescription = "Вареники з картопляною начинкою",
    Description = "Традиційні українські вареники з картопляною начинкою та обсмаженою цибулею. Подаються зі сметаною.",
    ImagePath = "https://images.unsplash.com/photo-1513862153653-f8b7324e1779?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 60,
    Servings = 4,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "ukrainian_chef"
},
new Recipe
{
    Title = "Сало з часником",
    ShortDescription = "Солоне сало з часником та перцем",
    Description = "Традиційна українська закуска - сало, просочене часником, чорним перцем та спеціями.",
    ImagePath = "https://images.unsplash.com/photo-1700843256667-2d9b249c73f7?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 1440, 
    Servings = 8,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "ukrainian_chef"
},
new Recipe
{
    Title = "Котлети по-київськи",
    ShortDescription = "Курячі котлети з масляною начинкою",
    Description = "Знамениті київські котлети з курячого філе з масляно-зеленою начинкою всередині.",
    ImagePath = "https://images.unsplash.com/photo-1719789254388-c6c3dbc2b0ee?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 45,
    Servings = 4,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "ukrainian_chef"
},
new Recipe
{
    Title = "Каша гречана",
    ShortDescription = "Гречана каша з цибулею та грибами",
    Description = "Традиційна гречана каша з обсмаженою цибулею та грибами. Ситна та поживна страва.",
    ImagePath = "https://images.unsplash.com/photo-1673646960449-45c61e27ef47?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 30,
    Servings = 3,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "ukrainian_chef"
},

new Recipe
{
    Title = "Піца Маргаріта",
    ShortDescription = "Класична італійська піца",
    Description = "Автентична італійська піца з томатним соусом, моцарелою та базиліком. Просто, але неймовірно смачно.",
    ImagePath = "https://images.unsplash.com/photo-1574071318508-1cdbab80d002?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 40,
    Servings = 2,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "john_cook"
},
new Recipe
{
    Title = "Різотто з грибами",
    ShortDescription = "Кремове різотто з білими грибами",
    Description = "Ароматне італійське різотто з арборіо рисом, білими грибами та пармезаном.",
    ImagePath = "https://images.unsplash.com/photo-1476124369491-e7addf5db371?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 35,
    Servings = 3,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "chef_maria"
},
new Recipe
{
    Title = "Брускета з томатами",
    ShortDescription = "Хліб з томатами та базиліком",
    Description = "Класична італійська закуска - грінки з томатами, часником, базиліком та оливковою олією.",
    ImagePath = "https://images.unsplash.com/photo-1572695157366-5e585ab2b69f?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 10,
    Servings = 2,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "admin"
},
new Recipe
{
    Title = "Лазанья з шпинатом",
    ShortDescription = "Вегетаріанська лазанья з шпинатом",
    Description = "Лазанья з шпинатом, рикотою та моцарелою. Легка та корисна версія класичної страви.",
    ImagePath = "https://images.unsplash.com/photo-1574894709920-11b28e7367e3?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 60,
    Servings = 6,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "chef_maria"
},

new Recipe
{
    Title = "Суші з лососем",
    ShortDescription = "Нігірі з лососем та авокадо",
    Description = "Традиційні японські суші з свіжим лососем, авокадо та рисом. Подаються з соєвим соусом та васабі.",
    ImagePath = "https://images.unsplash.com/photo-1579584425555-c3ce17fd4351?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 40,
    Servings = 2,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "john_cook"
},
new Recipe
{
    Title = "Курка терякі",
    ShortDescription = "Курка в солодко-солоному соусі",
    Description = "Курка, приготована в соусі терякі з медом, імбиром та соєвим соусом. Подається з рисом.",
    ImagePath = "https://images.unsplash.com/photo-1609183480237-ccbb2d7c5772?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 30,
    Servings = 3,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "john_cook"
},
new Recipe
{
    Title = "Вок з овочами",
    ShortDescription = "Овочі, смажені в воку",
    Description = "Свіжі овочі, швидко обсмажені в воку з імбиром, часником та соєвим соусом.",
    ImagePath = "https://images.unsplash.com/photo-1563379926898-05f4575a45d8?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 20,
    Servings = 2,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "chef_maria"
},
new Recipe
{
    Title = "Пхо бо",
    ShortDescription = "В'єтнамський суп з яловичиною",
    Description = "Ароматний в'єтнамський суп з яловичиною, рисовою локшиною та свіжими травами.",
    ImagePath = "https://images.unsplash.com/photo-1631709497146-a239ef373cf1?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 50,
    Servings = 2,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "john_cook"
},
new Recipe
{
    Title = "Спрінг роли",
    ShortDescription = "В'єтнамські спрінг роли з соусом",
    Description = "Легкі спрінг роли з рисового паперу з овочами, креветками та м'ятою. Подаються з арахісовим соусом.",
    ImagePath = "https://images.unsplash.com/photo-1618406854423-ef169758d6a6?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 35,
    Servings = 4,
    Type = RecipeType.Food,
    IsFavorite = false,
    CreatedBy = "chef_maria"
}
};

            context.Recipes.AddRange(foodRecipes);
            context.SaveChanges();

            AddTagsToFoodRecipes(context, tags);
            AddRecipeDetails(context);
        }

        private static void AddTagsToFoodRecipes(AppDbContext context, List<Tag> tags)
        {
            var recipes = context.Recipes.Include(r => r.Tags).Where(r => r.Type == RecipeType.Food).ToList();

            foreach (var recipe in recipes)
            {
                if (recipe.Title.Contains("Сирники") || recipe.Title.Contains("Омлет") ||
                    recipe.Title.Contains("тост") || recipe.Title.Contains("каша") ||
                    recipe.Title.Contains("Французькі тости") || recipe.Title.Contains("Вівсяна"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#сніданок"));
                }

                if (recipe.Title.Contains("салат") || recipe.Title.Contains("Салат") ||
                    recipe.Title.Contains("суп") || recipe.Title.Contains("Суп") ||
                    recipe.Title.Contains("паста") || recipe.Title.Contains("Паста") ||
                    recipe.Title.Contains("бургер") || recipe.Title.Contains("Бургери") ||
                    recipe.Title.Contains("Лазанья") || recipe.Title.Contains("Кускус") ||
                    recipe.Title.Contains("Рамен"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#обід"));
                }

                if (recipe.Title.Contains("стейк") || recipe.Title.Contains("Стейк") ||
                    recipe.Title.Contains("котлети") || recipe.Title.Contains("Котлети") ||
                    recipe.Title.Contains("голубці") || recipe.Title.Contains("Голубці") ||
                    recipe.Title.Contains("рагу") || recipe.Title.Contains("Рагу") ||
                    recipe.Title.Contains("лосось") || recipe.Title.Contains("Лосось") ||
                    recipe.Title.Contains("креветки") || recipe.Title.Contains("Креветки") ||
                    recipe.Title.Contains("фажітас") || recipe.Title.Contains("Фажітас"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#вечеря"));
                }

                if (recipe.Title.Contains("торт") || recipe.Title.Contains("Торт") ||
                    recipe.Title.Contains("пиріг") || recipe.Title.Contains("Пиріг") ||
                    recipe.Title.Contains("чізкейк") || recipe.Title.Contains("Чізкейк") ||
                    recipe.Title.Contains("мусс") || recipe.Title.Contains("Мусс") ||
                    recipe.Title.Contains("Тірамісу") || recipe.Title.Contains("млинці"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#десерт"));
                }

                if (recipe.Title.Contains("українська") || recipe.Title.Contains("Українська") ||
                    recipe.Title.Contains("борщ") || recipe.Title.Contains("Борщ") ||
                    recipe.Title.Contains("вареник") || recipe.Title.Contains("Вареники") ||
                    recipe.Title.Contains("голубці") || recipe.Title.Contains("Голубці") ||
                    recipe.Title.Contains("сирник") || recipe.Title.Contains("Сирники") ||
                    recipe.Title.Contains("сало") || recipe.Title.Contains("Сало") ||
                    recipe.Title.Contains("каша") || recipe.Title.Contains("Каша") ||
                    recipe.Title.Contains("млинці") || recipe.Title.Contains("Млинці"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#українська"));
                }

                if (recipe.Title.Contains("італійська") || recipe.Title.Contains("Італійська") ||
                    recipe.Title.Contains("паста") || recipe.Title.Contains("Паста") ||
                    recipe.Title.Contains("піца") || recipe.Title.Contains("Піца") ||
                    recipe.Title.Contains("різотто") || recipe.Title.Contains("Різотто") ||
                    recipe.Title.Contains("лазанья") || recipe.Title.Contains("Лазанья") ||
                    recipe.Title.Contains("брускет") || recipe.Title.Contains("Брускет") ||
                    recipe.Title.Contains("карбонара") || recipe.Title.Contains("Карбонара") ||
                    recipe.Title.Contains("Тірамісу"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#італійська"));
                }

                if (recipe.Title.Contains("азійська") || recipe.Title.Contains("Азійська") ||
                    recipe.Title.Contains("суші") || recipe.Title.Contains("Суші") ||
                    recipe.Title.Contains("териякі") || recipe.Title.Contains("теріякі") ||
                    recipe.Title.Contains("вок") || recipe.Title.Contains("Вок") ||
                    recipe.Title.Contains("рамен") || recipe.Title.Contains("Рамен") ||
                    recipe.Title.Contains("пхо бо") || recipe.Title.Contains("Пхо бо") ||
                    recipe.Title.Contains("спрінг рол") || recipe.Title.Contains("Спрінг рол"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#азійська"));
                }

                if (recipe.PreparationTime <= 20)
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#швидко"));
                }

                if (recipe.Title.Contains("салат") || recipe.Title.Contains("Салат") ||
                    recipe.Title.Contains("овочі") || recipe.Title.Contains("Овочі") ||
                    recipe.Title.Contains("каша") || recipe.Title.Contains("Каша") ||
                    recipe.Title.Contains("вівсяна") || recipe.Title.Contains("Вівсяна"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#здорове"));
                }

                if (recipe.Title.Contains("курка") || recipe.Title.Contains("Курка") ||
                    recipe.Title.Contains("яловичина") || recipe.Title.Contains("Яловичина") ||
                    recipe.Title.Contains("м'ясо") || recipe.Title.Contains("М'ясо") ||
                    recipe.Title.Contains("стейк") || recipe.Title.Contains("Стейк") ||
                    recipe.Title.Contains("котлети") || recipe.Title.Contains("Котлети"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#м'ясо"));
                }

                if (recipe.Title.Contains("лосось") || recipe.Title.Contains("Лосось") ||
                    recipe.Title.Contains("креветк") || recipe.Title.Contains("Креветк") ||
                    recipe.Title.Contains("риба") || recipe.Title.Contains("Риба") ||
                    recipe.Title.Contains("суші") || recipe.Title.Contains("Суші"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#риба"));
                }

                if (recipe.Title.Contains("торт") || recipe.Title.Contains("Торт") ||
                    recipe.Title.Contains("пиріг") || recipe.Title.Contains("Пиріг") ||
                    recipe.Title.Contains("чізкейк") || recipe.Title.Contains("Чізкейк") ||
                    recipe.Title.Contains("тост") || recipe.Title.Contains("Тост") ||
                    recipe.Title.Contains("млинці") || recipe.Title.Contains("Млинці"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#випічка"));
                }

                if (recipe.Title.Contains("паста") || recipe.Title.Contains("Паста") ||
                    recipe.Title.Contains("лазанья") || recipe.Title.Contains("Лазанья") ||
                    recipe.Title.Contains("різотто") || recipe.Title.Contains("Різотто"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#паста"));
                }

                if (recipe.Title.Contains("гриль") || recipe.Title.Contains("Гриль") ||
                    recipe.Title.Contains("стейк") || recipe.Title.Contains("Стейк") ||
                    recipe.Title.Contains("бургер") || recipe.Title.Contains("Бургери"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#гриль"));
                }

                if (recipe.Title.Contains("торт") || recipe.Title.Contains("Торт") ||
                    recipe.Title.Contains("чізкейк") || recipe.Title.Contains("Чізкейк") ||
                    recipe.Title.Contains("свята") || recipe.Title.Contains("Свята"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#свята"));
                }

                if (recipe.Title.Contains("салат") || recipe.Title.Contains("Салат") ||
                    recipe.Title.Contains("суп") || recipe.Title.Contains("Суп") ||
                    recipe.Title.Contains("овочі") || recipe.Title.Contains("Овоч") ||
                    recipe.PreparationTime <= 25)
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#легке"));
                }

                if (recipe.Title.Contains("крем") || recipe.Title.Contains("Крем") ||
                    recipe.Title.Contains("чізкейк") || recipe.Title.Contains("Чізкейк") ||
                    recipe.Title.Contains("мусс") || recipe.Title.Contains("Мусс") ||
                    recipe.Title.Contains("різотто") || recipe.Title.Contains("Різотто"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#кремове"));
                }
                if (recipe.Title.Contains("овочі") || recipe.Title.Contains("Овочі") ||
                    recipe.Title.Contains("салат") || recipe.Title.Contains("Салат") ||
                    recipe.Title.Contains("каша") || recipe.Title.Contains("Каша") ||
                    recipe.Title.Contains("гриби") || recipe.Title.Contains("Гриби") ||
                    recipe.Title.Contains("вегетаріанське") || recipe.Title.Contains("Вегетаріанське"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#вегетаріанське"));
                }
                if (recipe.Title.Contains("кава") || recipe.Title.Contains("Кава") ||
                    recipe.Title.Contains("фрапе") || recipe.Title.Contains("Фрапе") ||
                    recipe.Title.Contains("лате") || recipe.Title.Contains("Лате") ||
                    recipe.Title.Contains("еспресо") || recipe.Title.Contains("Еспресо"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#кава"));
                }

                if (recipe.Title.Contains("чай") || recipe.Title.Contains("Чай") ||
                    recipe.Title.Contains("матча") || recipe.Title.Contains("Матча") ||
                    recipe.Title.Contains("ройбуш") || recipe.Title.Contains("Ройбуш") ||
                    recipe.Title.Contains("глінтвейн") || recipe.Title.Contains("Глінтвейн"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#чай"));
                }

                if (recipe.Title.Contains("смузі") || recipe.Title.Contains("Смузі") ||
                    recipe.Title.Contains("сік") || recipe.Title.Contains("Сік") ||
                    recipe.Title.Contains("морс") || recipe.Title.Contains("Морс") ||
                    recipe.Title.Contains("компот") || recipe.Title.Contains("Компот"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#смузі"));
                }

                if (recipe.Title.Contains("лимонад") || recipe.Title.Contains("Лимонад") ||
                    recipe.Title.Contains("мохіто") || recipe.Title.Contains("Мохіто") ||
                    recipe.Title.Contains("содова") || recipe.Title.Contains("Содова") ||
                    recipe.PreparationTime <= 10 && recipe.Title.Contains("холодний"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#освіжаючий"));
                }

                if (recipe.Title.Contains("молочний") || recipe.Title.Contains("Молочний") ||
                    recipe.Title.Contains("мілкшейк") || recipe.Title.Contains("Мілкшейк") ||
                    recipe.Title.Contains("какао") || recipe.Title.Contains("Какао") ||
                    recipe.Title.Contains("вершки") || recipe.Title.Contains("Вершки"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#молочний"));
                }

                if (recipe.Title.Contains("гарячий") || recipe.Title.Contains("Гарячий") ||
                    recipe.Title.Contains("зігріваючий") || recipe.Title.Contains("Зігріваючий") ||
                    recipe.Title.Contains("какао") || recipe.Title.Contains("Какао") ||
                    recipe.Title.Contains("глінтвейн") || recipe.Title.Contains("Глінтвейн"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#гарячий"));
                }

                if (recipe.Title.Contains("коктейль") || recipe.Title.Contains("Коктейль") ||
                    recipe.Title.Contains("мохіто") || recipe.Title.Contains("Мохіто") ||
                    recipe.Title.Contains("піна колада") || recipe.Title.Contains("Піна Колада"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#коктейль"));
                }

                if (recipe.Title.Contains("корисний") || recipe.Title.Contains("Корисний") ||
                    recipe.Title.Contains("вітамін") || recipe.Title.Contains("Вітамін") ||
                    recipe.Title.Contains("імбир") || recipe.Title.Contains("Імбир") ||
                    recipe.Title.Contains("журавлин") || recipe.Title.Contains("Журавлин"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#корисний"));
                }

                if (recipe.Title.Contains("фруктовий") || recipe.Title.Contains("Фруктовий") ||
                    recipe.Title.Contains("ягідний") || recipe.Title.Contains("Ягідний") ||
                    recipe.Title.Contains("тропічний") || recipe.Title.Contains("Тропічний") ||
                    recipe.Title.Contains("апельсин") || recipe.Title.Contains("Апельсин"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#фруктовий"));
                }

                if (recipe.Title.Contains("шоколад") || recipe.Title.Contains("Шоколад") ||
                    recipe.Title.Contains("ваніл") || recipe.Title.Contains("Ваніл") ||
                    recipe.Title.Contains("десертний") || recipe.Title.Contains("Десертний") ||
                    recipe.Title.Contains("морозиво") || recipe.Title.Contains("Морозиво"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#десертний"));
                }

                if (recipe.PreparationTime <= 5)
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#швидко"));
                }

                if (recipe.Title.Contains("енергі") || recipe.Title.Contains("Енергі") ||
                    recipe.Title.Contains("поживний") || recipe.Title.Contains("Поживний") ||
                    recipe.Title.Contains("банан") || recipe.Title.Contains("Банан"))
                {
                    recipe.Tags.Add(tags.First(t => t.Name == "#енергетичний"));
                }
                context.SaveChanges();
            }
        }
        private static void AddDrinkRecipes(AppDbContext context, List<Tag> tags)
        {
            var drinkRecipes = new[]
            {
                new Recipe
                {
                    Title = "Мохіто",
                    ShortDescription = "Освіжаючий коктейль з м'ятою та лаймом",
                    Description = "Класичний мохіто з свіжою м'ятою, лаймом, цукром та содовою. Ідеально для спекотного дня.",
                    ImagePath = "https://images.unsplash.com/photo-1551538827-9c037cb4f32a?auto=format&fit=crop&w=400&h=300",
                    PreparationTime = 10,
                    Servings = 1,
                    Type = RecipeType.Drink,
                    IsFavorite = false,
                    CreatedBy = "admin"
                },
                new Recipe
{
    Title = "Фрапе з кави",
    ShortDescription = "Холодна кава з льодом та молоком",
    Description = "Освіжаюча холодна кава з льодом, молоком та цукром. Ідеальний напій у спекотний день.",
    ImagePath = "https://images.unsplash.com/photo-1461023058943-07fcbe16d735?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 5,
    Servings = 1,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "chef_maria"
},
new Recipe
{
    Title = "Ягідний смузі",
    ShortDescription = "Поживний смузі з ягід та банану",
    Description = "Корисний та смачний смузі з міксу ягід, банану, йогурту та меду. Багато вітамінів та енергії.",
    ImagePath = "https://images.unsplash.com/photo-1553530666-ba11a7da3888?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 7,
    Servings = 2,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "john_cook"
},
new Recipe
{
    Title = "Лимонад класичний",
    ShortDescription = "Освіжаючий лимонад з лимону та м'яти",
    Description = "Традиційний лимонад з свіжих лимонів, цукру, води та м'яти. Найкраща спрага улітку.",
    ImagePath = "https://images.unsplash.com/photo-1621506289937-a8e4df240d0b?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 10,
    Servings = 4,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "admin"
},
new Recipe
{
    Title = "Молочний коктейль полуничний",
    ShortDescription = "Ніжний коктейль з полуниці та морозива",
    Description = "Кремовий молочний коктейль з свіжої полуниці, ванільного морозива та молока. Улюблений десертний напій.",
    ImagePath = "https://images.unsplash.com/photo-1611928237590-087afc90c6fd?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 8,
    Servings = 2,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "chef_maria"
},
new Recipe
{
    Title = "Чай холодний з персиком",
    ShortDescription = "Освіжаючий чай з персиком та м'ятою",
    Description = "Холодний чай з натуральним персиковим соком, м'ятою та льодом. Легкий та ароматний напій.",
    ImagePath = "https://images.unsplash.com/photo-1573812914274-226dc19fbe17?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 15,
    Servings = 2,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "john_cook"
},
new Recipe
{
    Title = "Горячий шоколад",
    ShortDescription = "Густий гарячий шоколад з вершками",
    Description = "Багатий та ароматний гарячий шоколад з темного шоколаду, молока та вершків. Затишний напій для холодних днів.",
    ImagePath = "https://images.unsplash.com/photo-1542990253-0d0f5be5f0ed?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 12,
    Servings = 2,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "admin"
},
new Recipe
{
    Title = "Сік апельсиновий свіжий",
    ShortDescription = "Свіжовичавлений апельсиновий сік",
    Description = "Насичений вітаміном C апельсиновий сік, вичавлений зі свіжих апельсинів. Енергія та здоров'я в кожній склянці.",
    ImagePath = "https://images.unsplash.com/photo-1613478223719-2ab802602423?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 5,
    Servings = 2,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "chef_maria"
},
new Recipe
{
    Title = "Коктейль Манго-Ківі",
    ShortDescription = "Тропічний коктейль з манго та ківі",
    Description = "Екзотичний коктейль з манго, ківі, апельсинового соку та льоду. Смак тропіків у вашій склянці.",
    ImagePath = "https://images.unsplash.com/photo-1544145945-f90425340c7e?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 10,
    Servings = 2,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "john_cook"
},
new Recipe
{
    Title = "Імбирний чай з лимоном",
    ShortDescription = "Зігріваючий чай з імбиром та лимоном",
    Description = "Ароматний чай з свіжого імбиру, лимону та меду. Чудово зігріває та піднімає імунітет.",
    ImagePath = "https://images.unsplash.com/photo-1606444006818-3e66c09f2724?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 15,
    Servings = 2,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "admin"
},
new Recipe
{
    Title = "Кока-Кола з лимоном",
    ShortDescription = "Класична Кока-Кола з лимоном",
    Description = "Освіжаюча Кока-Кола зі свіжим лимоном та льодом. Просто та смачно.",
    ImagePath = "https://images.unsplash.com/photo-1596203356350-3ffb1fa178b6?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 3,
    Servings = 1,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "chef_maria"
},
new Recipe
{
    Title = "Смузі з шпинату та яблука",
    ShortDescription = "Зелений смузі з шпинату та яблука",
    Description = "Корисний зелений смузі з шпинату, яблука, банану та імбирного соку. Енергія на весь день.",
    ImagePath = "https://images.unsplash.com/photo-1601091566377-17adfa2fa02e?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 8,
    Servings = 2,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "john_cook"
},
new Recipe
{
    Title = "Какао з зефірками",
    ShortDescription = "Ніжне какао з молока та зефірок",
    Description = "Традиційне какао з молока, какао-порошку та цукру, прикрашене білими зефірками. Улюблений напій дітей.",
    ImagePath = "https://images.unsplash.com/photo-1533550127910-72ec5d02d36d?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 10,
    Servings = 2,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "admin"
},
new Recipe
{
    Title = "Морс з чорниці",
    ShortDescription = "Корисний морс з чорниці",
    Description = "Натуральний морс з чорниці, води та меду. Багатий антиоксидантами та вітамінами.",
    ImagePath = "https://images.unsplash.com/photo-1662186341099-56d0131327b5?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 20,
    Servings = 4,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "chef_maria"
},
new Recipe
{
    Title = "Лате з ваніллю",
    ShortDescription = "Ніжний лате з ваніллю та молоком",
    Description = "Кремовий кавовий напій з еспресо, пароного молока та ванільного сиропу. Ідеальний для початку дня.",
    ImagePath = "https://images.unsplash.com/photo-1561047029-3000c68339ca?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 7,
    Servings = 1,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "john_cook"
},
new Recipe
{
    Title = "Глінтвейн безалкогольний",
    ShortDescription = "Зігріваючий напій з прянощами",
    Description = "Ароматний безалкогольний глінтвейн з соку, апельсинів, меду та прянощів. Чудово зігріває взимку.",
    ImagePath = "https://images.unsplash.com/photo-1542143708653-1a0d78f64f21?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 25,
    Servings = 4,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "admin"
},
new Recipe
{
    Title = "Смузі Баунті",
    ShortDescription = "Десертний смузі з кокосом та шоколадом",
    Description = "Ніжний смузі з кокосового молока, банану, какао та меду. Смак популярного батончика у формі напою.",
    ImagePath = "https://images.unsplash.com/photo-1756132539966-8d65f7a9eed8?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 8,
    Servings = 2,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "chef_maria"
},
new Recipe
{
    Title = "Лимонад з полуницею",
    ShortDescription = "Фруктовий лимонад з полуницею",
    Description = "Освіжаючий лимонад з лимону, свіжої полуниці, м'яти та цукру. Яскравий та фруктовий смак.",
    ImagePath = "https://images.unsplash.com/photo-1573500883495-6c9b16d88d8c?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 12,
    Servings = 4,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "john_cook"
},
new Recipe
{
    Title = "Чай матча лате",
    ShortDescription = "Японський чай матча з молоком",
    Description = "Традиційний японський чай матча, збитий з пароним молоком. Багатий антиоксидантами та дає спокійну енергію.",
    ImagePath = "https://images.unsplash.com/photo-1515823064-d6e0c04616a7?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 10,
    Servings = 1,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "admin"
},
new Recipe
{
    Title = "Коктейль Піна Колада безалкогольний",
    ShortDescription = "Тропічний коктейль з ананасом та кокосом",
    Description = "Знаменитий тропічний коктейль без рому з ананасовим соком, кокосовим молоком та вершками.",
    ImagePath = "https://images.unsplash.com/photo-1681102523359-2cb988cbb37d?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 8,
    Servings = 2,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "chef_maria"
},
new Recipe
{
    Title = "Сік томатний з селерою",
    ShortDescription = "Поживний томатний сік з селерою",
    Description = "Корисний томатний сік з свіжих томатів, селери, спецій та лимонного соку. Ідеальний для легкого перекусу.",
    ImagePath = "https://images.unsplash.com/photo-1601627901831-2d9626bea5b8?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 10,
    Servings = 2,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "john_cook"
},
new Recipe
{
    Title = "Молочний коктейль шоколадний",
    ShortDescription = "Класичний шоколадний молочний коктейль",
    Description = "Багатий шоколадний коктейль з шоколадного морозива, молока та шоколадного сиропу. Справжня насолода.",
    ImagePath = "https://images.unsplash.com/photo-1553787499-6f9133860278?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 6,
    Servings = 2,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "admin"
},
new Recipe
{
    Title = "Чай з журавлиною",
    ShortDescription = "Корисний чай з журавлиною та медом",
    Description = "Ароматний чай з журавлини, меду та апельсинової цедри. Підтримує імунітет та має приємний кисло-солодкий смак.",
    ImagePath = "https://images.unsplash.com/photo-1681564326988-29964ef9e0bb?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 15,
    Servings = 2,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "chef_maria"
},
new Recipe
{
    Title = "Смузі з авокадо",
    ShortDescription = "Кремовий смузі з авокадо та медом",
    Description = "Поживний смузі з авокадо, банану, молока та меду. Багатий корисними жирами та дає тривалу ситість.",
    ImagePath = "https://images.unsplash.com/photo-1622704430673-59c152a9991c?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 8,
    Servings = 2,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "john_cook"
},
new Recipe
{
    Title = "Кава по-турецьки",
    ShortDescription = "Ароматна кава по-турецьки",
    Description = "Традиційна турецька кава, приготована в джезві з мелкого помелу кави. Міцна та ароматна.",
    ImagePath = "https://images.unsplash.com/photo-1576685880864-50b3b35f1c55?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 12,
    Servings = 2,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "admin"
},
new Recipe
{
    Title = "Компот з сухофруктів",
    ShortDescription = "Традиційний компот з сухофруктів",
    Description = "Натуральний компот з сушених яблук, груш, родзинок та меду. Смак дитинства та користь для здоров'я.",
    ImagePath = "https://images.unsplash.com/photo-1534336294469-f77e4c59ada6?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 30,
    Servings = 6,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "chef_maria"
},
new Recipe
{
    Title = "Мохіто полуничний",
    ShortDescription = "Фруктовий мохіто з полуницею",
    Description = "Освіжаючий варіант мохіто з свіжої полуниці, м'яти, лайму та содової. Яскраво та фруктово.",
    ImagePath = "https://images.unsplash.com/photo-1662550577541-5e8f192e6514?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 10,
    Servings = 1,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "john_cook"
},
new Recipe
{
    Title = "Сік грейпфрутовий",
    ShortDescription = "Свіжовичавлений грейпфрутовий сік",
    Description = "Освіжаючий грейпфрутовий сік з легкою гірчинкою. Багатий вітаміном C та допомагає пробудитись.",
    ImagePath = "https://images.unsplash.com/photo-1626160200749-fbcbf5d7c456?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 5,
    Servings = 2,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "admin"
},
new Recipe
{
    Title = "Коктейль Мілкшейк ванільний",
    ShortDescription = "Класичний ванільний мілкшейк",
    Description = "Ніжний ванільний мілкшейк з ванільного морозива, молока та ванільного екстракту. Просто та смачно.",
    ImagePath = "https://plus.unsplash.com/premium_photo-1695868328902-b8a3b093da74?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 6,
    Servings = 2,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "chef_maria"
},
new Recipe
{
    Title = "Чай ройбуш з апельсином",
    ShortDescription = "Африканський чай ройбуш з цитрусами",
    Description = "Ароматний південноафриканський чай ройбуш з апельсином, медом та корицею. Не містить кофеїну.",
    ImagePath = "https://images.unsplash.com/photo-1606695960140-79f3e342f9c6?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 10,
    Servings = 2,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "john_cook"
},
new Recipe
{
    Title = "Смузі тропічний",
    ShortDescription = "Яскравий смузі з тропічних фруктів",
    Description = "Енергетичний смузі з манго, ананасу, банану та апельсинового соку. Смак літа цілий рік.",
    ImagePath = "https://plus.unsplash.com/premium_photo-1695055513638-92886435c7eb?auto=format&fit=crop&w=400&h=300",
    PreparationTime = 8,
    Servings = 2,
    Type = RecipeType.Drink,
    IsFavorite = false,
    CreatedBy = "admin"

}
            };


            context.Recipes.AddRange(drinkRecipes);
            context.SaveChanges();
        }

        private static void AddRecipeDetails(AppDbContext mainContext)
        {
            AddBreakfastRecipes();
            AddLunchRecipes();
            AddDinnerRecipes();
            AddDessertRecipes();
          
        }

        private static void AddBreakfastRecipes()
        {
            using var context = new AppDbContext();
            AddSyrnikiDetails(context);
            AddOmeletDetails(context);
            AddAvocadoToastDetails(context);
            AddOatmealDetails(context);
            AddFrenchToastDetails(context);
            context.SaveChanges();
        }

        private static void AddLunchRecipes()
        {
            using var context = new AppDbContext();
            AddBorshchDetails(context);
            AddPastaCarbonaraDetails(context);
            AddChickenSoupDetails(context);
            AddLasagnaDetails(context);
            AddGreekSaladDetails(context);
            AddSaladCaesarDetails(context);
            AddBurgersDetails(context);
            AddRamenDetails(context);
            AddCouscousDetails(context);
            context.SaveChanges();
        }

        private static void AddDinnerRecipes()
        {
            using var context = new AppDbContext();
            AddSalmonTeriyakiDetails(context);
            AddChickenCutletsDetails(context);
            AddVegetableStewDetails(context);
            AddSteakDetails(context);
            AddShrimpGarlicDetails(context);
            AddFajitasDetails(context);
            AddHolubtsiDetails(context);
            context.SaveChanges();
        }

        private static void AddDessertRecipes()
        {
            using var context = new AppDbContext();
            AddChocolateCakeDetails(context);
            AddCheesecakeDetails(context);
            AddApplePieDetails(context);
            AddTiramisuDetails(context);
            AddPancakesDetails(context);
            AddChocolateMousseDetails(context);
            AddVarenykyDetails(context);
            AddSaloDetails(context);
            AddKyivCutletsDetails(context);
            AddBuckwheatDetails(context);
            AddSpringRollsDetails(context);
            AddPhoBoDetails(context);
            AddWokVegetablesDetails(context);
            AddChickenTeriyakiDetails(context);
            AddSushiDetails(context);
            AddPizzaMargheritaDetails(context);
            AddRisottoDetails(context);
            AddBruschettaDetails(context);
            AddSpinachLasagnaDetails(context);
            context.SaveChanges();
        }

        private static void AddDrinkRecipes()
        {
            using var context = new AppDbContext();
            AddMojitoDetails(context);
            AddFrappeDetails(context);
            AddBerrySmoothieDetails(context);
            AddLemonadeDetails(context);
            AddStrawberryMilkshakeDetails(context);
            AddPeachIcedTeaDetails(context);
            AddHotChocolateDetails(context);
            AddOrangeJuiceDetails(context);
            AddMangoKiwiCocktailDetails(context);
            AddGingerTeaDetails(context);
            AddCocaColaLemonDetails(context);
            AddSpinachAppleSmoothieDetails(context);
            AddCocoaMarshmallowDetails(context);
            AddBlueberryMorseDetails(context);
            AddVanillaLatteDetails(context);
            AddNonAlcoholicMulledWineDetails(context);
            AddBountySmoothieDetails(context);
            AddStrawberryLemonadeDetails(context);
            AddMatchaLatteDetails(context);
            AddPinaColadaNonAlcoholicDetails(context);
            AddTomatoCeleryJuiceDetails(context);
            AddChocolateMilkshakeDetails(context);
            AddCranberryTeaDetails(context);
            AddAvocadoSmoothieDetails(context);
            AddTurkishCoffeeDetails(context);
            AddDriedFruitCompoteDetails(context);
            AddStrawberryMojitoDetails(context);
            AddGrapefruitJuiceDetails(context);
            AddRooibosTeaDetails(context);
            AddTropicalSmoothieDetails(context);
            AddVanillaMilkshakeDetails(context);
            context.SaveChanges();
        }

        private static void AddSyrnikiDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Сирники з ягодами");

            var ingredients = new[]
            {
                new Ingredient { Name = "Сир  500г", RecipeId = recipe.RecipeId },
                new Ingredient { Name = "Борошно  100г", RecipeId = recipe.RecipeId },
                new Ingredient { Name = "Яйце  2шт", RecipeId = recipe.RecipeId },
                new Ingredient { Name = "Цукор  50г",  RecipeId = recipe.RecipeId },
                new Ingredient { Name = "Ванільний цукор  1ч.л.",  RecipeId = recipe.RecipeId },
                new Ingredient { Name = "Полуниця  200г",  RecipeId = recipe.RecipeId },
                new Ingredient { Name = "Малина  100г",  RecipeId = recipe.RecipeId },
                new Ingredient { Name = "Сметана  100г",  RecipeId = recipe.RecipeId }
            };

            var steps = new[]
            {
                new RecipeStep { StepNumber = 1, Instruction = "У глибокій мисці змішайте сир з яйцями, цукром та ванільним цукром", RecipeId = recipe.RecipeId },
                new RecipeStep { StepNumber = 2, Instruction = "Поступово додавайте борошно, вимішуючи до отримання однорідної маси", RecipeId = recipe.RecipeId },
                new RecipeStep { StepNumber = 3, Instruction = "Сформуйте невеликі круглі сирники товщиною 1.5-2 см", RecipeId = recipe.RecipeId },
                new RecipeStep { StepNumber = 4, Instruction = "Розігрійте сковороду з олією, обсмажте сирники з обох сторін до золотистої скоринки", RecipeId = recipe.RecipeId },
                new RecipeStep { StepNumber = 5, Instruction = "Подавайте гарячими зі свіжими ягодами та сметаною", RecipeId = recipe.RecipeId }
            };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddOmeletDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Омлет з овочами");

            var ingredients = new[]
            {
        new Ingredient { Name = "Яйце  4шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Молоко  50мл", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Перець солодкий  1шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Помідор  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цибуля  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Олія  2ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Сіль  0.5ч.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Зелень  10г",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Наріжте овочі: перець соломкою, помідор кубиками, цибулю півкільцями", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Збийте яйця з молоком та сіллю до однорідності", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Обсмажте овочі на розігрітій олії 5 хвилин", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Залейте овочі яєчною сумішшю, накрийте кришкою", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Готуйте на середньому вогні 7-8 хвилин, посипте зеленню", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddAvocadoToastDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Авокадо-тост з яйцем");

            var ingredients = new[]
            {
        new Ingredient { Name = "Хліб  2 скибки", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Авокадо  1шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Яйце  2шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лимонний сік  1ч.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Насіння  1ч.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Сіль  0.25ч.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Перець  0.25ч.л.",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Підсмажте хліб у тостері або на сковороді до золотистого кольору", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Приготуйте яйця пашот: занурте у киплячу воду з оцтом на 3-4 хвилини", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Розімніть авокадо виделкою, додайте лимонний сік, сіль та перець", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Намажте пасту з авокадо на тости", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Покладіть яйце пашот на тост, посипте насінням", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddOatmealDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Вівсяна каша з фруктами");

            var ingredients = new[]
            {
        new Ingredient { Name = "Вівсяні пластівці  100г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Молоко  250мл", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Банан  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Ягоди  100г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Мед  4ч.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Ваніль  0.5ч.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Кориця  0.5ч.л.",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "У каструлі змішайте вівсяні пластівці з молоком", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Доведіть до кипіння, зменшіть вогонь та варіть 7-10 хвилин", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Додайте ваніль та корицю, перемішайте", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Наріжте банан, помийте ягоди", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Подавайте кашу з фруктами, полийте медом", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddFrenchToastDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Французькі тости");

            var ingredients = new[]
            {
        new Ingredient { Name = "Білий хліб  4 скибки",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Яйце  2шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Молоко  100мл", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Ванільний цукор  1ч.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Кориця  0.5ч.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вершкове масло  2ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Кленовий сироп  4ст.л.", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Збийте яйця з молоком, ванільним цукром та корицею", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Обсмажте кожну скибку хліба у вершковому маслі до золотистого кольору", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Обсмажуйте з обох сторін по 2-3 хвилини", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Висушіть тости на паперових рушниках", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Подавайте з кленовим сиропом та свіжими ягодами", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddBorshchDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Борщ український");

            var ingredients = new[]
            {
        new Ingredient { Name = "Яловичина  500г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Буряк  2шт ", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Капуста  300г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Картопля  3шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Морква  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цибуля  1шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Томатна паста  2ст.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Сметана  200г",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Зваріть міцний бульйон з яловичини протягом 1.5 години", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Наріжте овочі: буряк соломкою, капусту, картоплю кубиками", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Обсмажте цибулю, моркву та буряк з томатною пастою", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додайте овочі до бульйону та варіть 20 хвилин", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Подавайте зі сметаною та свіжим часником", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddPastaCarbonaraDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Паста Карбонара");

            var ingredients = new[]
            {
        new Ingredient { Name = "Спагеті  200г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Бекон  150г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Яйце  2шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Жовток  2шт",RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Пармезан  50г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Часник  2 зубки", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Чорний перець  1ч.л.",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Варіть спагеті в підсоленій воді до стану аль денте", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Обсмажте бекон з часником до хрусткості", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Змішайте яйця, жовтки та тертий пармезан", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "З'єднайте гарячі спагеті з яєчною сумішшю та беконом", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Поситіть свіжемелем перцем та подавайте негайно", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddChickenSoupDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Курячий суп з локшиною");

            var ingredients = new[]
            {
        new Ingredient { Name = "Куряче філе  300г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Локшина  150г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Морква  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цибуля  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Картопля  2шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Зелень  20г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лавровий лист  2шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Сіль  1ч.л.", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Зваріть курячий бульйон з філе, цибулі та моркви", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Вийміть м'ясо, наріжте кубиками", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Додайте нарізану картоплю до бульйону, варіть 15 хвилин", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додайте локшину та варіть ще 8-10 хвилин", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Поверніть м'ясо, додайте зелень, подавайте гарячим", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddLasagnaDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Лазанья з м'ясом");

            var ingredients = new[]
            {
        new Ingredient { Name = "Листи лазаньї  12шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Фарш яловичий  500г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Соус бешамель  500мл", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Моцарела  200г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Пармезан  100г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Томатний соус  400мл",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цибуля  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Часник  2 зубки", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Обсмажте фарш з цибулею та часником, додайте томатний соус", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Приготуйте соус бешамель: масло, борошно, молоко", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Зберіть лазанью: лист, м'ясо, соус бешамель, сир", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Повторюйте шари, завершіть сиром", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Запікайте 40 хвилин при 180°C до золотистої скоринки", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddGreekSaladDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Грецький салат");

            var ingredients = new[]
            {
        new Ingredient { Name = "Помідори  3шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Огірки  2шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Перець  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Червона цибуля  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Фета  200г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Оливки  100г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Оливкова олія  3ст.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Орегано  1ч.л.",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Наріжте помідори, огірки та перець великими кубиками", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Наріжте цибулю тонкими кільцями", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Додайте оливки та нарізану кубиками фету", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Змішайте оливкову олію з орегано, полейте салат", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Перемішайте, подавайте зі свіжим хлібом", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddSaladCaesarDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Салат Цезар з куркою");

            var ingredients = new[]
            {
        new Ingredient { Name = "Куряче філе  200г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Айсберг 1 качан", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Пармезан  50г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Гренки  100г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Соус Цезар  3ст.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лимонний сік  1ст.л.",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Обсмажте куряче філе до готовності та наріжте смужками", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Порвіть салат Айсберг великими шматочками", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Змішайте салат з соусом Цезар та лимонним соком", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додайте куряче філе, гренки та тертий пармезан", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Перемішайте та подавайте негайно", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddBurgersDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Бургери з яловичиною");

            var ingredients = new[]
            {
        new Ingredient { Name = "Яловичий фарш  500г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Булочки  4шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Сир  4 скибки",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Помідори  2шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Листя салату  8шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цибуля  1шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Соус  4ст.л.",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Сформуйте котлети з фаршу, посоліть, поперчіть", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Обсмажте котлети на грилі або сковороді до готовності", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Підсмажте булочки, наріжте овочі", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Зберіть бургери: булочка, соус, салат, котлета, сир, овочі", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Подавайте з картоплею фрі або овочами", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddRamenDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Рамен з куркою");

            var ingredients = new[]
            {
        new Ingredient { Name = "Локшина рамен  200г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Куряче філе  200г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Яйце  2шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Бульйон  1л", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Соєвий соус  3ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Імбир  1шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Зелена цибуля  2шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Кунжут  1ч.л.", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Приготуйте яйця: варіть 6-7 хвилин, охолодіть, очистіть", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Обсмажте куряче філе, наріжте смужками", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Зваріть локшину згідно інструкції", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Змішайте бульйон з соєвим соусом та імбиром", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Зберіть рамен: локшина, бульйон, курка, яйце, цибуля, кунжут", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddCouscousDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Кускус з овочами");

            var ingredients = new[]
            {
        new Ingredient { Name = "Кускус  200г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Кабачок  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Перець  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цибуля  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Оливкова олія  3ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лимонний сік  2ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Зелень  20г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Спеції  1ч.л.", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Наріжте овочі кубиками середнього розміру", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Обсмажте овочі на оливковій олії до м'якості", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Залейте кускус окропом у співвідношенні 1:1.5", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Накрийте кришкою та дайте настоятись 5 хвилин", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Змішайте кускус з овочами, додайте лимонний сік та зелень", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddSalmonTeriyakiDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Лосось у соусі терякі");

            var ingredients = new[]
            {
        new Ingredient { Name = "Філе лосося  400г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Соус терякі  4ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Мед  1ст.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Імбир  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Часник  2 зубки",RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Кунжут  1ч.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Оливкова олія  2ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Зелена цибуля  2шт",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Змішайте соус терякі з медом, тертим імбиром та часником", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Замаринуйте лосось у соусі на 15-20 хвилин", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Розігрійте олію у сковороді, обсмажте лосось шкіряною стороною донизу", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Обсмажте по 4-5 хвилин з кожного боку", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Полийте залишками соусу, посипте кунжутом та цибулею", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddChickenCutletsDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Курячі котлети");

            var ingredients = new[]
            {
        new Ingredient { Name = "Курячий фарш  500г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цибуля  1шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Часник  2 зубки",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Яйце  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Панірувальні сухарі  100г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Борошно  50г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Олія  4ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Сіль  1ч.л.", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Дрібно наріжте цибулю та часник, змішайте з фаршем", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Додайте яйце, сіль, перець, ретельно вимішайте", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Сформуйте котлети, обваляйте у борошні", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Обваляйте у панірувальних сухарях", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Обсмажте на розігрітій олії з обох сторін до золотистого кольору", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddVegetableStewDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Овочеве рагу");

            var ingredients = new[]
            {
        new Ingredient { Name = "Баклажан  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Кабачок 1шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Перець  2шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Помідори  3шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цибуля  1шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Часник  3 зубки",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Томатна паста  2ст.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Оливкова олія  3ст.л.",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Наріжте всі овочі кубиками середнього розміру", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Обсмажте цибулю та часник на оливковій олії", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Додайте баклажани та кабачки, обсмажте 5 хвилин", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додайте перець, помідори та томатну пасту", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Тушкуйте під кришкою 20-25 хвилин на повільному вогні", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddSteakDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Стейк з яловичини");

            var ingredients = new[]
            {
        new Ingredient { Name = "Стейк яловичий  300г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Оливкова олія  2ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Часник  3 зубки", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Розмарин  2 гылочки",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вершкове масло  50г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Сіль  1ч.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Перець  1ч.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Тим'ян  0.5ч.л.",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Достаньте стейк з холодильника за 30 хвилин до приготування", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Обсушіть, посипте сіллю та перцем з обох сторін", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Розігрійте сковороду, додайте олію, обсмажте стейк по 2-3 хвилини з кожного боку", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додайте масло, часник та розмарин, поливайте стейк маслом", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Дайте відпочити 5 хвилин перед подачею", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddShrimpGarlicDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Креветки в часниковому соусі");

            var ingredients = new[]
            {
        new Ingredient { Name = "Креветки великі  400г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Часник  5 зубчиков", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Оливкова олія  3ст.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лимонний сік  2ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Петрушка  20г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Перець чилі  0.5шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Біле вино  50мл",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Сіль  0.5ч.л.", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Очисніть креветки, залишивши хвостики", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Дрібно наріжте часник та перець чилі", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Розігрійте оливкову олію, обсмажте часник 1 хвилину", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додайте креветки, обсмажте по 2 хвилини з кожного боку", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Додайте вино, лимонний сік, петрушку, посоліть", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddFajitasDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Фажітас з куркою");

            var ingredients = new[]
            {
        new Ingredient { Name = "Куряче філе  400г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Перець різнокольоровий  3шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цибуля  2шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Тортильї  8шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лайм  1шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Кмин  1ч.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Паприка  1ч.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Оливкова олія  3ст.л.", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Наріжте курку та овочі тонкими смужками", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Змішайте спеції: кмин, паприку, сіль, перець", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Обсмажте курку на сильному вогні до золотистості", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додайте овочі, обсмажте ще 5-7 хвилин", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Подавайте з тортильями, полийте соком лайма", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddHolubtsiDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Голубці з м'ясом");

            var ingredients = new[]
            {
        new Ingredient { Name = "Капуста  1 качан", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "М'ясний фарш  500г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Рис  150г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цибуля  2шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Морква  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Томатна паста  3ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Сметана  200г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лавровий лист  2шт", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Відваріть капусту, розберіть на листя", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Змішайте фарш з відвареним рисом та обсмаженою цибулею", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Загорніть начинку в капустяні листя", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Викладіть голубці в каструлю, залейте соусом з томатної пасти та сметани", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Тушкуйте 40-50 хвилин на повільному вогні", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddChocolateCakeDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Шоколадний торт");

            var ingredients = new[]
            {
        new Ingredient { Name = "Борошно  200г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Какао  50г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цукор  250г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Яйце  4шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вершкове масло  200г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Розпушувач  2ч.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Молоко  200мл", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Ванільний екстракт  1ч.л.",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Змішайте борошно, какао та розпушувач", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Збийте масло з цукром, додавайте яйця по одному", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "По черзі додавайте сухі інгредієнти та молоко", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Випікайте у формі 30-35 хвилин при 180°C", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Охолодіть, розріжте на коржі, змастіть кремом", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddCheesecakeDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Чізкейк Нью-Йорк");

            var ingredients = new[]
            {
        new Ingredient { Name = "Вершковий сир  600г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Печиво  200г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вершкове масло  100г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цукор  150г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Яйце  3шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Ванільний цукор  1ч.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Сметана  200г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лимонна цедра  1ч.л.",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Подрібніть печиво, змішайте з розтопленим маслом", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Викладіть основу у форму, ущільніть", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Збийте вершковий сир з цукром до кремообразної консистенції", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додавайте яйця по одному, потім сметану та ваніль", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Випікайте на водяній бані 60 хвилин при 160°C, охолодіть у вимкненій духовці", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddApplePieDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Яблучний пиріг");

            var ingredients = new[]
            {
        new Ingredient { Name = "Яблука  6шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Борошно  250г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вершкове масло  150г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цукор  120г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Яйце  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Кориця  1ч.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лимонний сік  1ст.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Ванільний цукор  1ч.л.",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Змішайте борошно, 100г цукру та масло, зробіть крихту", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Додайте яйце, замісіть тісто, охолодіть 30 хвилин", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Наріжте яблука, змішайте з рештою цукру, корицею та лимонним соком", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Розкатайте тісто, викладіть у форму, зробьте бортики", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Викладіть яблука, випікайте 40 хвилин при 180°C", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddTiramisuDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Тірамісу");

            var ingredients = new[]
            {
        new Ingredient { Name = "Маскарпоне  500г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Яйце  4шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цукор  100г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Печиво савоярді  24шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Кава еспресо  300мл ", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Какао  2ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Марсала  50мл", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Ванільний екстракт  1ч.л.",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Збийте жовтки з половиною цукру до світлої маси", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Додайте маскарпоне, збийте до гладкості", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Збийте білки з рештою цукру, обережно з'єднайте з сирною масою", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Змішайте каву з марсалою, обмакніть печиво", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Викладіть шари: печиво, крем, какао. Охолодіть 6 годин", RecipeId = recipe.RecipeId }
    };
            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }


        private static void AddPancakesDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Млинці з варенням");

            var ingredients = new[]
            {
        new Ingredient { Name = "Борошно  200г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Молоко  500мл",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Яйце  2шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цукор  2ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Сіль  0.5ч.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Олія  2ст.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Варення полуничне  200г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Сметана  150г",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Змішайте яйця, цукор та сіль, додайте молоко", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Поступово додавайте борошно, розмішуючи до однорідності", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Додайте олію, дайте тісту постояти 15 хвилин", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Випікайте млинці на розігрітій сковороді з обох сторін", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Подавайте з варенням та сметаною", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddChocolateMousseDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Шоколадний мусс");

            var ingredients = new[]
            {
        new Ingredient { Name = "Темний шоколад  200г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вершки 35%  300мл",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Яйце  3шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цукор  50г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вершкове масло  50г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Ванільний екстракт  1ч.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Сіль  0.25ч.л.",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Розтопіть шоколад з маслом на водяній бані", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Збийте жовтки з половиною цукру до світлої маси", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Збийте білки з рештою цукру до щільних піків", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Збийте вершки до м'яких піків", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Обережно з'єднайте всі інгредієнти, розлийте по формах, охолодіть 4 години", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddVarenykyDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Вареники з картоплею");

            var ingredients = new[]
            {
        new Ingredient { Name = "Борошно  400г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вода  200мл",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Яйце  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Картопля  500г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цибуля  2шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вершкове масло  50г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Сіль  1ч.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Сметана  200г", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Замісіть тісто з борошна, води, яйця та солі", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Відваріть картоплю, зімніть у пюре", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Обсмажте цибулю на маслі, змішайте з картоплею", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Розкатайте тісто, виріжте кружечки, начинте картоплею", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Варіть у підсоленій воді 5-7 хвилин, подавайте зі сметаною", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddSaloDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Сало з часником");

            var ingredients = new[]
            {
        new Ingredient { Name = "Сало свіже  500г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Часник  1 головка",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Сіль  3ст.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Перець чорний  1ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Паприка  1ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Кріп  0.5 пучок",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лавровий лист  3шт  ",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Наріжте сало пластами товщиною 2-3 см", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Зробіть надрізи на шкірці", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Змішайте сіль з перцем, паприкою та подрібненим кропом", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Натріть сало сумішшю спецій, вкладіть у надрізи пластики часнику", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Загорніть у пергамент, витримайте в холодильнику 3-5 днів", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddKyivCutletsDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Котлети по-київськи");

            var ingredients = new[]
            {
        new Ingredient { Name = "Куряче філе  400г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вершкове масло  100г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Петрушка  20г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Часник  2 зубчика",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Яйце  2шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Борошно  100г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Панірувальні сухарі  150г ",   RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Олія  200мл",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Змішайте розм'якшене масло з подрібненою петрушкою та часником", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Сформуйте з масла циліндри, охолодіть", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Відб'йте куряче філе, покладіть масляну начинку, зліпіть котлети", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Обваляйте у борошні, яйці та сухарях", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Обсмажте у глибокій олії до золотистого кольору", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddBuckwheatDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Каша гречана");

            var ingredients = new[]
            {
        new Ingredient { Name = "Гречка  200г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вода  400мл",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цибуля  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Гриби  200г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Морква  1шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Олія  3ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Сіль  1ч.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Зелень  20г", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Переберіть гречку, прожаріть на сухій сковороді 5 хвилин", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Залейте водою, посоліть, варіть 15-20 хвилин", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Наріжте овочі та гриби, обсмажте на олії", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Змішайте готову гречку з овочами та грибами", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Посипте зеленню, подавайте гарячою", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }
        private static void AddSpringRollsDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Спрінг роли");

            var ingredients = new[]
            {
        new Ingredient { Name = "Рисовий папір  8шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Креветки  200г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Морква  1шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Огірок  1шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "М'ята  10г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вермішель рисова  100г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Соус арахісовий  2ст.л.", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Відваріть креветки та рисову вермішель", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Наріжте овочі тонкою соломкою", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Змочіть рисовий папір у теплій воді на 10 секунд", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Викладіть начинку: вермішель, овочі, креветки, м'яту", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Згорніть роли, подавайте з арахісовим соусом", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddPhoBoDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Пхо бо");

            var ingredients = new[]
            {
        new Ingredient { Name = "Яловичина  300г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Локшина рисова  200г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Бульйон яловичий  1.5л", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Імбир  1шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Коріандр  0.5 пучок", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "М'ята  0.5 пучок", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лайм  1шт", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Приготуйте бульйон: варіть яловичину з імбиром 2 години", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Відваріть рисову локшину згідно інструкції", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Тонко наріжте сиру яловичину", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Зберіть суп: локшина, сире м'ясо, залити гарячим бульйоном", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Додайте зелень та сік лайму", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddWokVegetablesDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Вок з овочами");

            var ingredients = new[]
            {
        new Ingredient { Name = "Брокколі  200г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Перець  2шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Морква  2шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цибуля  1шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Імбир  1шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Соєвий соус  3ст.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Олія  3ст.л.", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Наріжте всі овочі тонкою соломкою", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Розігрійте вок, додайте олію, обсмажте імбир", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Додайте тверді овочі (морква, брокколі), обсмажте 3 хвилини", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додайте м'які овочі (перець, цибуля), обсмажте ще 2 хвилини", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Додайте соєвий соус, подавайте гарячим", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddChickenTeriyakiDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Курка терякі");

            var ingredients = new[]
            {
        new Ingredient { Name = "Куряче філе  400г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Соус терякі  5ст.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Мед  1ст.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Імбир  1шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Часник  2 зубчика", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Оливкова олія  2ст.л.", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Наріжте курку шматочками, посоліть, поперчіть", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Змішайте соус терякі з медом, тертим імбиром та часником", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Обсмажте курку на олії до золотистого кольору", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додайте соус, тушкуйте 5-7 хвилин на повільному вогні", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Подавайте з рисом", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddSushiDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Суші з лососем");

            var ingredients = new[]
            {
        new Ingredient { Name = "Рис для суші  200г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лосось  150г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Авокадо  1шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Норі  4 листа", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Рисовий оцет  3ст.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цукор  1ст.л.", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Відваріть рис, змішайте з оцтом, цукром та сіллю", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Наріжте лосось та авокадо тонкими смужками", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "На лист норі викладіть рис, залишивши 2 см зверху", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додайте лосось та авокадо, згорніть за допомогою макісу", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Наріжте на ролки, подавайте з соєвим соусом", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddSpinachLasagnaDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Лазанья з шпинатом");

            var ingredients = new[]
            {
        new Ingredient { Name = "Листи лазаньї  12шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Шпинат  500г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Рикота  400г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Моцарела  200г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Пармезан  100г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Соус бешамель  500мл", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Часник  3 зубчика", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Підсмажте шпинат з часником, дайте стікти зайвій рідині", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Змішайте шпинат з рикотою, сіллю та перцем", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Зберіть лазанью: лист, соус бешамель, шпинатна суміш, сир", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Повторюйте шари, завершіть соусом бешамель та пармезаном", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Запікайте 35-40 хвилин при 180°C до золотистої скоринки", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddBruschettaDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Брускета з томатами");

            var ingredients = new[]
            {
        new Ingredient { Name = "Хліб багет  1шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Помідори  4шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Часник  2 зубчика", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Базилік  10г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Оливкова олія  3ст.л.", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Наріжте багет на скоринки товщиною 2 см", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Підсмажте хліб на грилі або в тостері до золотистості", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Потріть кожну скоринку часником", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Наріжте помідори кубиками, змішайте з базиліком та олією", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Викладіть томатну суміш на хліб, подавайте", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddRisottoDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Різотто з грибами");

            var ingredients = new[]
            {
        new Ingredient { Name = "Рис арборіо  200г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Гриби білі  300г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цибуля  1шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Часник 2 зубчика", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Біле вино  100мл", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Бульйон овочевий  800мл", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Підготуйте бульйон: підігрійте його та тримайте на повільному вогні", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Обсмажте цибулю та часник на оливковій олії до м'якості", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Додайте рис, обсмажте 2 хвилини, потім додайте вино", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додавайте бульйон по черзі, помішуючи, поки рис не поглине рідину", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Додайте обсмажені гриби, подавайте гарячим", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddPizzaMargheritaDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Піца Маргаріта");

            var ingredients = new[]
            {
        new Ingredient { Name = "Борошно  300г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вода  200мл", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Дріжджі  7г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Моцарела  200г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Помідори  3шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Соус томатний  5ст.л.", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Замісіть тісто з борошна, води, дріжджів та солі, дайте підійти", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Розкатайте тісто у круг, зробьте бортики", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Намажте томатний соус, викладіть нарізані помідори", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Посипте тертою моцарелою", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Випікайте 12-15 хвилин при 220°C", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }


        private static void AddMojitoDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Мохіто");

            var ingredients = new[]
            {
        new Ingredient { Name = "Свіжа м'ята  10 гулочок", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лайм  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цукор  4ч.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Содова  150мл",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лід  1 склянка",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вода газована  50мл",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Помістіть м'яту та нарізаний лайм у високий бокал", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Додайте цукор та розімніть інгредієнти мадлером", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Наповніть бокал льодом", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додайте содову та газовану воду", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Перемішайте, прикрасьте гілочкою м'яти", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddFrappeDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Фрапе з кави");

            var ingredients = new[]
            {
        new Ingredient { Name = "Кава розчинна  2ч.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цукор  2ч.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Холодна вода  2ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Молоко  150мл",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лід  1 склянка",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Ванільний сироп  1ч.л.",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "У шейкері змішайте каву, цукор та холодну воду", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Інтенсивно збивайте 2-3 хвилини до утворення пишної піни", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Наповніть склянку льодом", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додайте молоко та ванільний сироп", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Перелийте кавову піну в склянку, подавайте з соломинкою", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddBerrySmoothieDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Ягідний смузі");

            var ingredients = new[]
            {
        new Ingredient { Name = "Мікс ягід  200г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Банан  1шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Йогурт натуральний  150мл", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Мед  1ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лід  0.5 склянки",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Молоко  100мл",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Помістіть всі інгредієнти в блендер", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Збивайте на високій швидкості 1-2 хвилини", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Перевірте консистенцію, додайте більше льоду або молока за потреби", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Збийте ще 30 секунд до однорідності", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Перелийте в склянку, подавайте негайно", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddLemonadeDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Лимонад класичний");

            var ingredients = new[]
            {
        new Ingredient { Name = "Лимони  4шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цукор  100г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вода  1л",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "М'ята  5 гілочок",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лід  2 склянки",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Вичавліть сік з лимонів", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Змішайте цукор з частиною води, розмішайте до розчинення", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Додайте лимонний сік та решту води", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додайте м'яту та лід", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Охолодіть у холодильнику 30 хвилин перед подачею", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddStrawberryMilkshakeDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Молочний коктейль полуничний");

            var ingredients = new[]
            {
        new Ingredient { Name = "Полуниця свіжа  200г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Морозиво ванільне  3 кульки", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Молоко  200мл", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цукор  1ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вершки  2ст.л.", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Помийте та очистіть полуницю", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Помістіть всі інгредієнти в блендер", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Збивайте на середній швидкості 1-2 хвилини", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Перелийте в високий бокал", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Прикрасьте свіжою полуницею та вершками", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddPeachIcedTeaDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Чай холодний з персиком");

            var ingredients = new[]
            {
        new Ingredient { Name = "Чай чорний  3ч.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Персики  2шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Мед  2ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вода  1л",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лід  2 склянки",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "М'ята  4 гілочки", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Заваріть чай у 500 мл окропу, настоїть 5 хвилин", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Наріжте персики, змішайте з медом", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Процідіть чай, додайте холодну воду", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додайте персики з медом та м'яту", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Охолодіть у холодильнику 2 години, подавайте з льодом", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddHotChocolateDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Горячий шоколад");

            var ingredients = new[]
            {
        new Ingredient { Name = "Молоко  400мл", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Шоколад чорний  100г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вершки  50мл",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цукор  1ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Ванільний екстракт  0.5ч.л.",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Наріжте шоколад дрібними шматочками", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Нагрійте молоко у каструлі, не доводячи до кипіння", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Додайте шоколад, розмішуйте до повного розчинення", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додайте цукор та ванільний екстракт", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Збийте вінчиком до появи піни, подавайте з вершками", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddOrangeJuiceDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Сік апельсиновий свіжий");

            var ingredients = new[]
            {
            new Ingredient { Name = "Апельсини  4шт",  RecipeId = recipe.RecipeId },
            new Ingredient { Name = "Лимон  0.5шт",  RecipeId = recipe.RecipeId },
            new Ingredient { Name = "Мед  1ч.л.",  RecipeId = recipe.RecipeId },
            new Ingredient { Name = "Лід  1 склянка", RecipeId = recipe.RecipeId }
        };

            var steps = new[]
            {
            new RecipeStep { StepNumber = 1, Instruction = "Обмийте апельсини та лимон", RecipeId = recipe.RecipeId },
            new RecipeStep { StepNumber = 2, Instruction = "Розріжте фрукти навпіл", RecipeId = recipe.RecipeId },
            new RecipeStep { StepNumber = 3, Instruction = "Вичавліть сік за допомогою соковижималки", RecipeId = recipe.RecipeId },
            new RecipeStep { StepNumber = 4, Instruction = "Додайте мед за бажанням", RecipeId = recipe.RecipeId },
            new RecipeStep { StepNumber = 5, Instruction = "Подавайте з льодом, прикрасьте скибкою апельсина", RecipeId = recipe.RecipeId }
        };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddMangoKiwiCocktailDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Коктейль Манго-Ківі");

            var ingredients = new[]
            {
        new Ingredient { Name = "Манго  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Ківі  2шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Сік апельсиновий  200мл",RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лимонний сік  1ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лід  1 склянка",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Мед  1ч.л.",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Очистіть манго та ківі, наріжте кубиками", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Помістіть фрукти в блендер", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Додайте апельсиновий сік, лимонний сік та мед", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додайте лід та збивайте 1-2 хвилини", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Перелийте в бокал, прикрасьте скибкою ківі", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddGingerTeaDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Імбирний чай з лимоном");

            var ingredients = new[]
            {
        new Ingredient { Name = "Імбир  50г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лимон  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Мед  2ст.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вода  500мл", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "М'ята  3 гілочки",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Очистіть імбир, наріжте тонкими скибочками", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Доведіть воду до кипіння, додайте імбир", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Варить на повільному вогні 10 хвилин", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додайте сік лимона та мед", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Процідіть, подавайте з м'ятою", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddCocaColaLemonDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Кока-Кола з лимоном");

            var ingredients = new[]
            {
        new Ingredient { Name = "Кока-Кола  200мл",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лимон  2 скибки",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лід  1 склянка",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Наповніть склянку льодом", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Обережно налийте Кока-Колу", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Додайте скибки лимона", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Перемішайте соломинкою", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Подавайте негайно", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddSpinachAppleSmoothieDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Смузі з шпинату та яблука");

            var ingredients = new[]
            {
        new Ingredient { Name = "Шпинат свіжий  100г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Яблуко  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Банан  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Імбир  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вода  200мл", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лимонний сік  1ст.л.",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Помийте шпинат та яблуко", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Наріжте яблуко, очистіть банан", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Помістіть всі інгредієнти в блендер", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Збивайте 2-3 хвилини до однорідної консистенції", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Перелийте в склянку, подавайте свіжим", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddCocoaMarshmallowDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Какао з зефірками");

            var ingredients = new[]
            {
        new Ingredient { Name = "Молоко  300мл", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Какао-порошок  2ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цукор  ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Зефір  4шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Ванільний екстракт  0.5ч.л.",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Нагрійте молоко у каструлі", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Змішайте какао з цукром", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Додайте какао-суміш до теплого молока", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Розмішайте до однорідності, додайте ваніль", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Розлийте по чашках, додайте зефір", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddBlueberryMorseDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Морс з чорниці");

            var ingredients = new[]
            {
        new Ingredient { Name = "Чорниця  300г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вода 1л",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Мед  3ст.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лимонний сік  1ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лід  2 склянки", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Переберіть та помийте чорницю", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Розімніть ягоди дерев'яною ложкою", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Залейте окропом, настоюйте 30 хвилин", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Процідіть через сито", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Додайте мед та лимонний сік, охолодіть", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddVanillaLatteDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Лате з ваніллю");

            var ingredients = new[]
            {
        new Ingredient { Name = "Еспресо  1 порція",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Молоко  200мл", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Ванільний сироп  2ч.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цукор  1ч.л.",RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Приготуйте порцію еспресо", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Зігрійте молоко до 65°C", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Збийте молоко капучинатором до утворення піни", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додайте ванільний сироп та цукор до еспресо", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Обережно влийте молоко з піною", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddNonAlcoholicMulledWineDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Глінтвейн безалкогольний");

            var ingredients = new[]
            {
            new Ingredient { Name = "Сік виноградний  1л",  RecipeId = recipe.RecipeId },
            new Ingredient { Name = "Апельсин  1шт",  RecipeId = recipe.RecipeId },
            new Ingredient { Name = "Лимон  0.5шт", RecipeId = recipe.RecipeId },
            new Ingredient { Name = "Мед  3ст.л.",  RecipeId = recipe.RecipeId },
            new Ingredient { Name = "Кориця  2 палички", RecipeId = recipe.RecipeId },
            new Ingredient { Name = "Гвоздика  5шт",  RecipeId = recipe.RecipeId }
        };

            var steps = new[]
            {
            new RecipeStep { StepNumber = 1, Instruction = "Наріжте цитруси кружечками", RecipeId = recipe.RecipeId },
            new RecipeStep { StepNumber = 2, Instruction = "Нагрійте виноградний сік у каструлі", RecipeId = recipe.RecipeId },
            new RecipeStep { StepNumber = 3, Instruction = "Додайте цитруси та спеції", RecipeId = recipe.RecipeId },
            new RecipeStep { StepNumber = 4, Instruction = "Томіть на повільному вогні 15 хвилин", RecipeId = recipe.RecipeId },
            new RecipeStep { StepNumber = 5, Instruction = "Додайте мед, подавайте гарячим", RecipeId = recipe.RecipeId }
        };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }
        private static void AddBountySmoothieDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Смузі Баунті");

            var ingredients = new[]
            {
        new Ingredient { Name = "Кокосове молоко  200мл",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Банан  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Какао-порошок  1ст.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Мед  1ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лід  1 склянка",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Кокосова стружка  1ч.л.",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Очистіть банан, наріжте шматочками", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Помістіть всі інгредієнти в блендер", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Збивайте 2 хвилини до однорідної консистенції", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Перелийте в склянку", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Посипте кокосовою стружкою", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddStrawberryLemonadeDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Лимонад з полуницею");

            var ingredients = new[]
            {
        new Ingredient { Name = "Полуниця  200г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лимони  2шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цукор  80г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вода  1л", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "М'ята  5 гілочок",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лід  2 склянки",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Помийте полуницю, розімніть половину", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Вичавліть сік з лимонів", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Змішайте цукор з частиною води", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додайте лимонний сік, полуницю та решту води", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Охолодіть, подавайте з льодом та м'ятою", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddMatchaLatteDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Чай матча лате");

            var ingredients = new[]
            {
        new Ingredient { Name = "Порошок матча  1ч.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Гаряча вода  50мл",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Молоко  200мл", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Мед  1ч.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Ванільний екстракт  0.25ч.л.",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Просейте порошок матча через ситечко", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Додайте гарячу воду, збийте вінчиком до піни", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Зігрійте молоко, збийте капучинатором", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додайте мед та ваніль до матча", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Обережно влийте молоко з піною", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddPinaColadaNonAlcoholicDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Коктейль Піна Колада безалкогольний");

            var ingredients = new[]
            {
        new Ingredient { Name = "Ананасовий сік  150мл", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Кокосове молоко  50мл", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вершки  2ст.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лід  1 склянка", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Ананас  2 скибки",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Наріжте ананас кубиками", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Помістіть всі інгредієнти в блендер", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Збивайте 1-2 хвилини до кремообразної консистенції", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Перелийте в бокал", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Прикрасьте скибкою ананаса", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddTomatoCeleryJuiceDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Сік томатний з селерою");

            var ingredients = new[]
            {
            new Ingredient { Name = "Помідори  4шт",  RecipeId = recipe.RecipeId },
            new Ingredient { Name = "Селера  2 стебла",  RecipeId = recipe.RecipeId },
            new Ingredient { Name = "Лимонний сік  1ст.л.", RecipeId = recipe.RecipeId },
            new Ingredient { Name = "Сіль  0.5ч.л.",  RecipeId = recipe.RecipeId },
            new Ingredient { Name = "Чорний перець  0.25ч.л.", RecipeId = recipe.RecipeId },
            new Ingredient { Name = "Табаско  3 краплі",  RecipeId = recipe.RecipeId }
        };

            var steps = new[]
            {
            new RecipeStep { StepNumber = 1, Instruction = "Помийте помідори та селеру", RecipeId = recipe.RecipeId },
            new RecipeStep { StepNumber = 2, Instruction = "Наріжте помідори, селеру наріжте шматочками", RecipeId = recipe.RecipeId },
            new RecipeStep { StepNumber = 3, Instruction = "Помістіть у соковижималку", RecipeId = recipe.RecipeId },
            new RecipeStep { StepNumber = 4, Instruction = "Додайте лимонний сік, сіль, перець", RecipeId = recipe.RecipeId },
            new RecipeStep { StepNumber = 5, Instruction = "Додайте табаско, перемішайте", RecipeId = recipe.RecipeId }
        };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }
        private static void AddChocolateMilkshakeDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Молочний коктейль шоколадний");

            var ingredients = new[]
            {
        new Ingredient { Name = "Морозиво шоколадне  3 кульки",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Молоко  200мл",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Шоколадний сироп  2ст.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вершки  3ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Шоколадна стружка  1ч.л.", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Помістіть морозиво, молоко та шоколадний сироп в блендер", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Збивайте 1-2 хвилини до однорідної консистенції", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Перелийте в високий бокал", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Збийте вершки, викладіть на коктейль", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Посипте шоколадною стружкою", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddCranberryTeaDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Чай з журавлиною");

            var ingredients = new[]
            {
        new Ingredient { Name = "Журавлина  150г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Чай чорний  2ч.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Мед  2ст.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Апельсинова цедра  1ч.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вода  500мл",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Помийте журавлину, розімніть половину ягід", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Заваріть чай у окропі, настоюйте 5 хвилин", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Процідіть чай, додайте журавлину", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додайте мед та апельсинову цедру", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Настоюйте 10 хвилин, подавайте гарячим", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }
        private static void AddVanillaMilkshakeDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Коктейль Мілкшейк ванільний");

            var ingredients = new[]
            {
        new Ingredient { Name = "Ванільне морозиво  3 кульки", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Молоко  200мл", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Ванільний сироп  2ч.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цукор  1ст.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вершки  3ст.л.", RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Помістіть морозиво, молоко та ванільний сироп в блендер", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Додайте цукор та збивайте 1-2 хвилини", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Перелийте в високий бокал", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Збийте вершки, викладіть на коктейль", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Подавайте з трубочкою", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }
        private static void AddAvocadoSmoothieDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Смузі з авокадо");

            var ingredients = new[]
            {
        new Ingredient { Name = "Авокадо  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Банан  1шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Молоко  200мл",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Мед  1ст.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лимонний сік  1ч.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лід  0.5 склянки",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Очистіть авокадо, видаліть кісточку", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Очистіть банан, наріжте шматочками", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Помістіть всі інгредієнти в блендер", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Збивайте 2 хвилини до кремової консистенції", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Перелийте в склянку, подавайте свіжим", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddTurkishCoffeeDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Кава по-турецьки");

            var ingredients = new[]
            {
        new Ingredient { Name = "Кава мелене  2ч.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вода  100мл",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цукор  1ч.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Кардамон  0.25ч.л.",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Насипте каву в джезву", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Додайте цукор та кардамон", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Додайте холодну воду", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Нагрівайте на повільному вогні до появи піни", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Зніміть з вогню, дайте піні осісти, повторіть 3 рази", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddDriedFruitCompoteDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Компот з сухофруктів");

            var ingredients = new[]
            {
        new Ingredient { Name = "Суміш сухофруктів  300г",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вода  2л",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Мед  3ст.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лимон  0.5шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Кориця  1 паличка",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Промійте сухофрукти, залийте окропом на 15 хвилин", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Доведіть воду до кипіння, додайте сухофрукти", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Додайте корицю, варіть 20 хвилин на повільному вогні", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додайте мед та сік лимона", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Настоюйте 1 годину, подавайте охолодженим", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddStrawberryMojitoDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Мохіто полуничний");

            var ingredients = new[]
            {
        new Ingredient { Name = "Полуниця  100г", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "М'ята  8 гулочок",RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лайм  1шт",RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Цукор  2ч.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Содова  150мл",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лід  1 склянка",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Помийте полуницю, розімніть половину", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Помістіть м'яту, нарізаний лайм та цукор у бокал", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Розімніть інгредієнти мадлером", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додайте лід, полуницю та содову", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Перемішайте, прикрасьте полуницею та м'ятою", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddGrapefruitJuiceDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Сік грейпфрутовий");

            var ingredients = new[]
            {
        new Ingredient { Name = "Грейпфрут  2шт", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Мед  1ч.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Лід  1 склянка", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "М'ята  2 гілочки",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Обмийте грейпфрути", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Розріжте фрукти навпіл", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Вичавліть сік за допомогою соковижималки", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Додайте мед за бажанням", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Подавайте з льодом, прикрасьте м'ятою", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }
        private static void AddRooibosTeaDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Чай ройбуш з апельсином");

            var ingredients = new[]
            {
        new Ingredient { Name = "Чай ройбуш  2ч.л.",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Апельсин  1шт",  RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Мед  1ст.л.", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Кориця  1 паличка", RecipeId = recipe.RecipeId },
        new Ingredient { Name = "Вода  500мл",  RecipeId = recipe.RecipeId }
    };

            var steps = new[]
            {
        new RecipeStep { StepNumber = 1, Instruction = "Наріжте апельсин кружечками", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 2, Instruction = "Заваріть ройбуш у окропі", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 3, Instruction = "Додайте апельсин та корицю", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 4, Instruction = "Настоюйте 5-7 хвилин", RecipeId = recipe.RecipeId },
        new RecipeStep { StepNumber = 5, Instruction = "Додайте мед, подавайте гарячим", RecipeId = recipe.RecipeId }
    };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

        private static void AddTropicalSmoothieDetails(AppDbContext context)
        {
            var recipe = context.Recipes.AsNoTracking().First(r => r.Title == "Смузі тропічний");

            var ingredients = new[]
            {
                new Ingredient { Name = "Манго  1шт",  RecipeId = recipe.RecipeId },
                new Ingredient { Name = "Ананас  100г", RecipeId = recipe.RecipeId },
                new Ingredient { Name = "Банан  1шт",  RecipeId = recipe.RecipeId },
                new Ingredient { Name = "Сік апельсиновий  150мл", RecipeId = recipe.RecipeId },
                new Ingredient { Name = "Лід  1 склянка", RecipeId = recipe.RecipeId },
                new Ingredient { Name = "Кокосова стружка  1ч.л.",  RecipeId = recipe.RecipeId }
            };

            var steps = new[]
            {
                new RecipeStep { StepNumber = 1, Instruction = "Очистіть манго, ананас та банан", RecipeId = recipe.RecipeId },
                new RecipeStep { StepNumber = 2, Instruction = "Наріжте фрукти шматочками", RecipeId = recipe.RecipeId },
                new RecipeStep { StepNumber = 3, Instruction = "Помістіть всі інгредієнти в блендер", RecipeId = recipe.RecipeId },
                new RecipeStep { StepNumber = 4, Instruction = "Збивайте 2 хвилини до однорідної консистенції", RecipeId = recipe.RecipeId },
                new RecipeStep { StepNumber = 5, Instruction = "Перелийте в склянку, посипте кокосовою стружкою", RecipeId = recipe.RecipeId }
            };

            context.Ingredients.AddRange(ingredients);
            context.Steps.AddRange(steps);
            context.SaveChanges();
        }

    }
}

