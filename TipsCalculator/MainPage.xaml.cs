using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace TipsCalculator
{
   


    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            button1.Click += (sender, args) => WriteANumber(1);
            button2.Click += (sender, args) => WriteANumber(2);
            button3.Click += (sender, args) => WriteANumber(3);
            button4.Click += (sender, args) => WriteANumber(4);
            button5.Click += (sender, args) => WriteANumber(5);
            button6.Click += (sender, args) => WriteANumber(6);
            button7.Click += (sender, args) => WriteANumber(7);
            button8.Click += (sender, args) => WriteANumber(8);
            button9.Click += (sender, args) => WriteANumber(9);
            button0.Click += (sender, args) => WriteANumber(0);
            button_coma.Click += (sender, args) =>
                                     {
                                         textBlock1.Text += ".";
                                         Calc(textBlock1.Text, (int)slider1.Value, int.Parse(peoplePicker.Text));
                                     };
            button_delete.Click += OnButtonDeleteOnClick;

           
            this.DataContext = this;
            InitStage();
            HideControls();
            
        }

        private void OnButtonDeleteOnClick(object sender, RoutedEventArgs args)
        {
            if (textBlock1.Text != string.Empty)
                textBlock1.Text = textBlock1.Text.Substring(0, textBlock1.Text.Length - 1);
            if (textBlock1.Text == string.Empty)
                textBlock1.Text = "0";

            Calc(textBlock1.Text, (int) slider1.Value, int.Parse(peoplePicker.Text));
        }

        private Cost _cost;

        public Cost Cost
        {
            get { return _cost; }
            set
            {
                _cost = value;
                InvokeOnPropertyChanged(new PropertyChangedEventArgs("Cost"));
            }
        }

        private Cost _tip;

        public Cost Tip
        {
            get { return _tip; }
            set
            {
                _tip = value;
                InvokeOnPropertyChanged(new PropertyChangedEventArgs("Tip"));
            }
        }

        public string PV
        {
            get { return _pv; }
            set {
                _pv = value;

                InvokeOnPropertyChanged(new PropertyChangedEventArgs("PV"));
            }
        }

        //public string SlideValue
        //{
        //    get { return _slideValue; }
        //    set
        //    {
        //        _slideValue = value;

        //        InvokeOnPropertyChanged(new PropertyChangedEventArgs("SlideValue"));
                
        //        //
        //    }
        //}

        private bool CurrentThemeIsDark
        {
            get { return (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible; }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (CurrentThemeIsDark)
            {
                button_delete.Background = new ImageBrush { ImageSource = new BitmapImage { UriSource = new Uri("appbar.clear.inverse.reflect.horizontal_New1.png", UriKind.Relative) } };
                button10.Background = new ImageBrush { ImageSource = new BitmapImage { UriSource = new Uri("appbar.arrow.left.light.png", UriKind.Relative) } };
                button11.Background = new ImageBrush { ImageSource = new BitmapImage { UriSource = new Uri("appbar.arrow.right.light.png", UriKind.Relative) } };
            }
            else
            {
                button_delete.Background = new ImageBrush { ImageSource = new BitmapImage { UriSource = new Uri("appbar.clear.inverse.reflect.horizontal.png", UriKind.Relative) } };
                button10.Background = new ImageBrush { ImageSource = new BitmapImage { UriSource = new Uri("appbar.arrow.left.dark.png", UriKind.Relative) } };
                button11.Background = new ImageBrush { ImageSource = new BitmapImage { UriSource = new Uri("appbar.arrow.right.dark.png", UriKind.Relative) } };
            }
        }

        private int value = 1;
        //private string _slideValue;
        private string _pv = "Person";

        public int Value 
        {
            get { return value; }
            set
            {
                this.value = value;
                
                PV = value == 1 ? "Person" : "People";

                if (value > 1)
                    ShowControls();
                else
                    HideControls();

                InvokeOnPropertyChanged(new PropertyChangedEventArgs("Value"));

                if (textBlock1 != null)
                    Calc(textBlock1.Text, (int)slider1.Value, int.Parse(peoplePicker.Text));
            }
        }
        private const int minValue = 1;
        private const int maxValue = 100;

        private void WriteANumber(int number)
        {
            if (textBlock1.Text == "0")
                textBlock1.Text = "";
            textBlock1.Text += number;


            if (ValidInput())
                Calc(textBlock1.Text, (int) slider1.Value, int.Parse(peoplePicker.Text));
            else
                OnButtonDeleteOnClick(this, new RoutedEventArgs());

        }

        private bool ValidInput()
        {
            return textBlock1.Text.Length <= 6;
        }

        private void HideControls()
        {
            this.individualCost.Visibility = Visibility.Collapsed;
            this.individualTip.Visibility = Visibility.Collapsed;
            this.individualTipBlock.Visibility = Visibility.Collapsed;
            this.individualCostBlock.Visibility = Visibility.Collapsed;
        }

        private void ShowControls()
        {
            this.individualCost.Visibility = Visibility.Visible;
            this.individualTip.Visibility = Visibility.Visible;
            this.individualTipBlock.Visibility = Visibility.Visible;
            this.individualCostBlock.Visibility = Visibility.Visible;
        }

        private void InitStage()
        {
            Cost = new Cost();
            Tip = new Cost();

            button0.IsEnabled = button_coma.IsEnabled = false;
            percentTextBlock.Text = slider1.Minimum.ToString();
        }

        private void Calc(string ToPay, double Percent, int HowManyPeople)
        {
            if (ToPay == string.Empty)
            {
                InitStage();
                return;
            }
            try
            {
                button0.IsEnabled = button_coma.IsEnabled = true;
                var toPay = double.Parse(ToPay);

                var totalCost = ((Percent*toPay/100) + toPay);
                var individualCost = totalCost/HowManyPeople;

                var individualTip = (Percent*toPay/100)/HowManyPeople;
                var totalTip = (Percent*toPay/100);

                Cost = new Cost {Individual = individualCost, Total = totalCost};
                Tip = new Cost {Individual = individualTip, Total = totalTip};
            }
            catch
            {
                textBlock1.Text = textBlock1.Text.Substring(0, textBlock1.Text.Length - 1);
            }
        }

        private void percerntSlide_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (percentTextBlock != null)
                percentTextBlock.Text = ((int) slider1.Value).ToString();
           if (textBlock1 != null && textBlock1.Text != "")
               Calc(textBlock1.Text, (int)slider1.Value, int.Parse(peoplePicker.Text));
        }
     
        private void button11_Click(object sender, RoutedEventArgs e)
        {
            if (Value < maxValue)
                Value++;
        }

        private void button10_Click(object sender, RoutedEventArgs e)
        {
            if (Value > minValue)
                Value--;
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokeOnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, e);
        }

        #endregion
    }

    public class Cost : INotifyPropertyChanged
    {
        private double _total;
        public double Total
        {
            get { return _total; }
            set
            {
                _total = value;
                InvokeOnPropertyChanged(new PropertyChangedEventArgs("Total"));
            }
        }

        private double _individual;
        public double Individual
        {
            get { return _individual; }
            set
            {
                _individual = value;
                InvokeOnPropertyChanged(new PropertyChangedEventArgs("Individual"));
            }
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokeOnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, e);
        }

        #endregion
    }
}