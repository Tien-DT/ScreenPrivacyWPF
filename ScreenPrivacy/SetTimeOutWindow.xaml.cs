using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScreenPrivacy
{
    /// <summary>
    /// Interaction logic for SetTimeOutWindow.xaml
    /// </summary>
    public partial class SetTimeOutWindow : Window
    {
        public SetTimeOutWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SaveTimeOut_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TimeOutTextBox.Text, out int newTimeOut))
            {
                
                MainWindow.timeOut = newTimeOut;
                MessageBox.Show($"Timeout value set to {newTimeOut} seconds.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close(); 
            }
            else
            {
       
                MessageBox.Show("Please enter a valid number for timeout.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
