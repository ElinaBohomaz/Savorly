using Savorly.Data;
using Savorly.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Savorly.Services
{
    public static class UserService
    {
        private static User _currentUser = null;
        private static readonly string AccountsFile = "saved_accounts.json";
        private static readonly string UserDataFile = "user_data.json";

        public static User CurrentUser
        {
            get => _currentUser;
            private set => _currentUser = value;
        }

        public static bool IsLoggedIn => CurrentUser != null;

        public static void SaveUserData()
        {
            if (!IsLoggedIn) return;

            try
            {
                var userData = new
                {
                    UserId = CurrentUser.UserId,
                    FavoriteRecipes = CurrentUser.FavoriteRecipes,
                    ShoppingList = CurrentUser.ShoppingList,
                    CreatedRecipesIds = CurrentUser.CreatedRecipesIds,
                    LastLogin = DateTime.Now
                };

                File.WriteAllText(UserDataFile, JsonSerializer.Serialize(userData));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Помилка збереження даних: {ex.Message}");
            }
        }

        public static void LoadUserData()
        {
            if (!IsLoggedIn) return;

            try
            {
                if (File.Exists(UserDataFile))
                {
                    var json = File.ReadAllText(UserDataFile);
                    var userData = JsonSerializer.Deserialize<UserData>(json);

                    if (userData != null && userData.UserId == CurrentUser.UserId)
                    {
                        using var context = new AppDbContext();
                        var user = context.Users.Find(CurrentUser.UserId);
                        if (user != null)
                        {
                            user.FavoriteRecipes = userData.FavoriteRecipes;
                            user.ShoppingList = userData.ShoppingList;
                            user.CreatedRecipesIds = userData.CreatedRecipesIds;
                            context.SaveChanges();

                            CurrentUser = user;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Помилка завантаження даних: {ex.Message}");
            }
        }

        public static void UpdateFavoriteRecipes(List<int> recipeIds)
        {
            if (!IsLoggedIn) return;

            try
            {
                using var context = new AppDbContext();
                var user = context.Users.Find(CurrentUser.UserId);
                if (user != null)
                {
                    user.FavoriteRecipes = JsonSerializer.Serialize(recipeIds);
                    context.SaveChanges();
                    CurrentUser.FavoriteRecipes = user.FavoriteRecipes;
                    SaveUserData();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Помилка оновлення улюблених: {ex.Message}");
            }
        }

        public static List<int> GetFavoriteRecipes()
        {
            if (!IsLoggedIn || string.IsNullOrEmpty(CurrentUser.FavoriteRecipes))
                return new List<int>();

            try
            {
                return JsonSerializer.Deserialize<List<int>>(CurrentUser.FavoriteRecipes) ?? new List<int>();
            }
            catch
            {
                return new List<int>();
            }
        }

        public static void UpdateShoppingList(string shoppingListJson)
        {
            if (!IsLoggedIn) return;

            try
            {
                using var context = new AppDbContext();
                var user = context.Users.Find(CurrentUser.UserId);
                if (user != null)
                {
                    user.ShoppingList = shoppingListJson;
                    context.SaveChanges();
                    CurrentUser.ShoppingList = shoppingListJson;
                    SaveUserData();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Помилка оновлення списку: {ex.Message}");
            }
        }

        public static string GetShoppingList()
        {
            return IsLoggedIn ? CurrentUser.ShoppingList : string.Empty;
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

        public static (bool success, string message) Register(string username, string email, string password, string confirmPassword)
        {
            try
            {
                using var context = new AppDbContext();

                if (password != confirmPassword)
                    return (false, "Паролі не співпадають");

                if (context.Users.Any(u => u.Email == email))
                    return (false, "Користувач з таким email вже існує");

                if (context.Users.Any(u => u.Username == username))
                    return (false, "Користувач з таким іменем вже існує");

                var user = new User
                {
                    Username = username.Trim(),
                    Email = email.Trim().ToLower(),
                    PasswordHash = HashPassword(password),
                    CreatedAt = DateTime.Now,
                    FavoriteRecipes = "[]",
                    ShoppingList = "[]",
                    CreatedRecipesIds = "[]"
                };

                context.Users.Add(user);
                context.SaveChanges();

                CurrentUser = user;
                SaveUserData();
                return (true, "Реєстрація успішна!");
            }
            catch (Exception ex)
            {
                return (false, $"Помилка реєстрації: {GetInnerExceptionMessage(ex)}");
            }
        }

        public static (bool success, string message) Login(string email, string password)
        {
            try
            {
                using var context = new AppDbContext();

                var user = context.Users
                    .FirstOrDefault(u => u.Email == email.Trim().ToLower());

                if (user == null)
                    return (false, "Користувача з таким email не знайдено");

                if (user.PasswordHash != HashPassword(password))
                    return (false, "Невірний пароль");

                CurrentUser = user;
                LoadUserData(); 
                SaveAccount(email);
                return (true, "Вхід успішний!");
            }
            catch (Exception ex)
            {
                return (false, $"Помилка входу: {GetInnerExceptionMessage(ex)}");
            }
        }

        public static void SaveAccount(string email)
        {
            try
            {
                var accounts = GetSavedAccounts();
                if (!accounts.Contains(email))
                {
                    accounts.Add(email);
                    File.WriteAllText(AccountsFile, JsonSerializer.Serialize(accounts));
                }
            }
            catch (Exception)
            {
            }
        }

        public static List<string> GetSavedAccounts()
        {
            try
            {
                if (File.Exists(AccountsFile))
                {
                    var json = File.ReadAllText(AccountsFile);
                    return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
                }
            }
            catch (Exception)
            {
            }
            return new List<string>();
        }

        public static void Logout()
        {
            SaveUserData(); 
            CurrentUser = null;
        }

        public static User GetCurrentUser()
        {
            return CurrentUser;
        }

        private static string GetInnerExceptionMessage(Exception ex)
        {
            var message = ex.Message;
            var inner = ex.InnerException;
            while (inner != null)
            {
                message += $"\n→ {inner.Message}";
                inner = inner.InnerException;
            }
            return message;
        }
    }

    public class UserData
    {
        public int UserId { get; set; }
        public string FavoriteRecipes { get; set; } = string.Empty;
        public string ShoppingList { get; set; } = string.Empty;
        public string CreatedRecipesIds { get; set; } = string.Empty;
        public DateTime LastLogin { get; set; }
    }
}