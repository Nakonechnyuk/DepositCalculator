using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DepositeCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Official exchange rate of the NBU as of 14.07.2023
        /// </summary>
        private const decimal USDEURExchangeCourse = 0.89m;

        private const decimal USDGBPExchangeCourse = 0.76m;

        private const int CapitalizationsPerYear = 12;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(txtAmount.Text, out decimal amount) && int.TryParse(txtTerm.Text, out int term))
            {
                var interestRate = 0.05m; 
                var expectedIncome = 0m;

                if (rbtnCapitalization.IsChecked == true)
                {
                    expectedIncome = CalculateCompoundInterest(amount, interestRate, term);
                }
                else if (rbtnMonthlyPayment.IsChecked == true)
                {
                    expectedIncome = amount * (interestRate / CapitalizationsPerYear) * term;
                }

                var currency = ((ComboBoxItem)cmbCurrency.SelectedItem).Content.ToString();
                if (currency == "GBP")
                {
                    expectedIncome *= USDGBPExchangeCourse;
                }
                if (currency == "EUR")
                {
                    expectedIncome *= USDEURExchangeCourse;
                }

                var result = $"{expectedIncome:C} {currency}";
                var fullSum = expectedIncome + amount;
                txtResult.Text = result;
                txtSum.Text = $"{fullSum:C} {currency}";
            }
            else
            {
                MessageBox.Show("Please enter valid values for the amount and term.");
            }
        }

        private decimal CalculateCompoundInterest(decimal amount, decimal interestRate, int term)
        {
            var compoundInterest = 1m;
            var monthlyInterestRate = interestRate / CapitalizationsPerYear;

            for (var i = 0; i < term; ++i)
            {
                compoundInterest *= 1 + monthlyInterestRate;
            }

            var finalAmount = amount * compoundInterest;
            var expectedIncome = finalAmount - amount;
            return expectedIncome;
        }
    }
}
