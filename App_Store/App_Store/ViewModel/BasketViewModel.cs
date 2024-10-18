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

        /*Связка с Xaml для корзины*/
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

        /*Связка с Xaml для количества товаров*/
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

        /*Связка с Xaml для отображения финальной цены*/
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

        /*Связка с Xaml для кнопки удалить*/
        public RelayCommand RemoveFromCartCommand { get; }

        public BasketViewModel()
        {
            Basket = new ObservableCollection<BasketItem>();
            RemoveFromCartCommand = new RelayCommand(RemoveFromCartAsync);
            LoadBasketAsync();
        }

        /*Загрузка начальных данных*/
        private async void LoadBasketAsync()
        {
            Basket = await LoadBasketFromJsonAsync();
            UpdateCartSummary();
        }

        private async Task<ObservableCollection<BasketItem>> LoadBasketFromJsonAsync()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile file = await folder.GetFileAsync("basket.json");
                string jsonText = await FileIO.ReadTextAsync(file);
                return JsonConvert.DeserializeObject<ObservableCollection<BasketItem>>(jsonText);
            }
            catch (FileNotFoundException)
            {
                return new ObservableCollection<BasketItem>();
            }
        }

        private void UpdateCartSummary()
        {
            int productCount = Basket.Sum(item => item.Count);
            double totalPrice = Basket.Sum(item => item.Price * item.Count);

            ProductCountText = $"Количество продуктов: {productCount}";
            TotalPriceText = $"Общая стоимость: {totalPrice:C}";
        }

        private void RemoveFromCartAsync(object parameter)
        {
            var basket = parameter as BasketItem;
            if (basket == null)
            {
                return;  // Если параметр не является BasketItem, выходим из метода
            }

            if (Basket.Contains(basket))
            {
                Basket.Remove(basket);
                SaveBasketAsync();
            }

            return; // Return a completed task if basket is not found
        }

        private async Task SaveBasketAsync()
        {
            var basketJson = JsonConvert.SerializeObject(Basket.ToList(), Formatting.Indented);
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync("basket.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, basketJson);
            return;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
