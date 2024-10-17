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
using Windows.Storage;

namespace App_Store.ViewModel
{
    public class BasketViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<BasketItem> basket;
        public ObservableCollection<BasketItem> Basket
        {
            get => basket;
            set
            {
                basket = value;
                OnPropertyChanged(nameof(Basket));
                UpdateCartSummary();
            }
        }

        private string productCountText;
        public string ProductCountText
        {
            get => productCountText;
            set
            {
                if (productCountText != value)
                {
                    productCountText = value;
                    OnPropertyChanged(nameof(ProductCountText));
                }
            }
        }

        private string totalPriceText;
        public string TotalPriceText
        {
            get => totalPriceText;
            set
            {
                if (totalPriceText != value)
                {
                    totalPriceText = value;
                    OnPropertyChanged(nameof(TotalPriceText));
                }
            }
        }

        public BasketViewModel()
        {
            Basket = new ObservableCollection<BasketItem>();
            LoadBasketAsync();
        }

        private async void LoadBasketAsync()
        {
            var items = await LoadBasketFromJsonAsync();
            foreach (var item in items)
            {
                Basket.Add(item);
            }
        }

        private async Task<List<BasketItem>> LoadBasketFromJsonAsync()
        {
            try
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.GetFileAsync("basket.json");
                string jsonText = await FileIO.ReadTextAsync(file);
                return JsonConvert.DeserializeObject<List<BasketItem>>(jsonText);
            }
            catch (FileNotFoundException)
            {
                return new List<BasketItem>();
            }
        }

        private void UpdateCartSummary()
        {
            int productCount = Basket.Sum(item => item.Count);
            double totalPrice = Basket.Sum(item => item.Price * item.Count);

            ProductCountText = $"Количество продуктов: {productCount}";
            TotalPriceText = $"Общая стоимость: {totalPrice:C}";
        }

        public async Task RemoveFromCartAsync(BasketItem item)
        {
            if (Basket.Contains(item))
            {
                Basket.Remove(item);
                await SaveBasketAsync();
            }
        }

        private async Task SaveBasketAsync()
        {
            var basketJson = JsonConvert.SerializeObject(Basket.ToList(), Formatting.Indented);
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync("basket.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, basketJson);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
