using App_Store.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using Windows.Storage;
using Formatting = Newtonsoft.Json.Formatting;


namespace App_Store.ViewModel
{
    public class StoreViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Product> _products;
        public ObservableCollection<Product> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged(nameof(Products));
            }
        }

        private ObservableCollection<BasketItem> _basket;
        public ObservableCollection<BasketItem> Basket
        {
            get => _basket;
            set
            {
                _basket = value;
                OnPropertyChanged(nameof(Basket));
            }
        }

        public RelayCommand AddToCartCommand { get; }


        public StoreViewModel()
        {
            Products = new ObservableCollection<Product>();
            Basket = new ObservableCollection<BasketItem>();
            AddToCartCommand = new RelayCommand(AddToCart);
            LoadData();
        }

        private async void LoadData()
        {
            Products = await LoadProductsAsync();
            Basket = await LoadBasketsAsync();
        }

        private async Task<ObservableCollection<Product>> LoadProductsAsync()
        {
            /*var products = new List<Product>();
            return products;*/
            // Логика загрузки продуктов
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            try
            {
                // Получаем файл products.json из локальной папки
                StorageFile file = await folder.GetFileAsync("products.json");
                // Читаем текст из файла
                string jsonText = await FileIO.ReadTextAsync(file);
                // Десериализуем JSON в список продуктов
                return JsonConvert.DeserializeObject<ObservableCollection<Product>>(jsonText);
            }
            catch (FileNotFoundException)
            {

                var games = new Dictionary<int, (string name, double price, string img)>
                {
                    { 1, ("Ticket to Ride: Европа", 4990d,"Ticket_to_Ride.png") },
                    { 2, ("Candamir: The First Settlers", 3990d,"Candamir_The First Settlers.png") },
                    { 3, ("Пандемия", 3190d,"Пандемия.png") },
                    { 4, ("Каркассон", 1990d,"Каркассон.png") },
                    { 5, ("Scrabble", 780d,"Scrabble.png") },
                    { 6, ("Monopoly: The Office", 5990d,"Monopoly_The Office.png") },
                    { 7, ("Cluedo. Паутина лжи", 2690d,"Cluedo_Паутина лжи.png") },
                    { 8, ("Risk Junior", 1690d,"Risk Junior.png") },
                    { 9, ("В погоне за счастьем", 3690d,"В погоне за счастьем.png") },
                    { 10, ("Chess set: Minecraft", 8790d,"Chess_set_Minecraft.png") },
                    { 11, ("Шашки деревянные", 500d,"Шашки деревянные.png") },
                    { 12, ("Мистериум: Пленник времени", 1750d,"Мистериум_Пленник времени.png") },
                    { 13, ("Азул", 3990d,"Азул.png") },
                    { 14, ("7 чудес", 3990d,"7 чудес.png") },
                    { 15, ("Бэнг!", 1290d,"Бэнг.png") },
                    { 16, ("Дюна: Война за Арракис. Космическая гильдия", 4990d,"Дюна_Война за Арракис. Космическая гильдия.png") },
                    { 17, ("Сорви башню!", 690d,"Сорви башню.png") },
                    { 18, ("Бамбук", 2990d,"Бамбук.png") },
                    { 19, ("Игра UNO", 790d,"Игра UNO.png") },
                    { 20, ("Диксит", 1990d,"Диксит.png") }
                };
                // Если файл не найден, создаем его с начальными данными
                var initialProducts = new ObservableCollection<Product>();
                foreach (var game in games)
                {
                    /*string imagePath = await ImageGenerator.GenerateProductImageAsync(game.Value.name);*/
                    initialProducts.Add(new Product
                    {
                        Id = game.Key,
                        Name = game.Value.name,
                        Price = $"{game.Value.price:C}",
                        Image = await ImageMoveLocalFolder.ImageMoveLocalFolderAsync(game.Value.img) /*game.Value.img*/ // Сохраняем путь к изображению
                    });
                }

                // Сериализуем начальные данные в JSON
                var jsonText = JsonConvert.SerializeObject(initialProducts, Formatting.Indented);

                // Создаем файл и записываем в него данные
                StorageFile newFile = await folder.CreateFileAsync("products.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(newFile, jsonText);

                // Возвращаем начальные данные
                return initialProducts;
            }
        }

        private async Task<ObservableCollection<BasketItem>> LoadBasketsAsync()
        {
            /*var basket = new ObservableCollection<Product>();
            return basket;*/
            // Логика загрузки корзины
            try
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.GetFileAsync("basket.json");
                string jsonText = await FileIO.ReadTextAsync(file);
                return JsonConvert.DeserializeObject<ObservableCollection<BasketItem>>(jsonText);
            }
            catch (FileNotFoundException)
            {
                // Если файл не найден, возвращаем пустой список
                return new ObservableCollection<BasketItem>();
            }
        }

        private void AddToCart(object parameter)
        {
            // Приведение параметра к типу
            var product = parameter as Product;
            if (product == null)
            {
                return; // Если параметр не является , выходим из метода
            }

            // Логика добавления товара в корзину
            var existingItem = Basket.FirstOrDefault(p => p.Id == product.Id);
            if (existingItem != null)
            {
                existingItem.Count++;
            }
            else
            {
                double d;
                string s = product.Price;
                string numberPart = s.Split(' ')[0];
                double.TryParse(numberPart, out d);
                Basket.Add(new BasketItem
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = d,
                    Image = product.Image,
                    Count = 1
                });
            }
            SaveBasketAsync();
        }

        private async Task SaveBasketAsync()
        {
            var basketJson = JsonConvert.SerializeObject(Basket, Formatting.Indented);
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync("basket.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, basketJson); 
            return;
            // Логика сохранения корзины
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
