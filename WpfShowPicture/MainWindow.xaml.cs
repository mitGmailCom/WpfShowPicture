using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfShowPicture
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);
        List<string> listPicture;
        List<string> masPicture;
        List<double> masSizeImg;
        int count = 0;
        double value = 0;
        Image mainImg;
        StackPanel tempStPanel;
        int ob = 0; // номер объекта на который пользователь кликнул в StPanel
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            masPicture = new List<string>();
            tempStPanel = new StackPanel();
            mainImg = new Image();
            masSizeImg = new List<double>();
            listPicture = new List<string>() { ".jpg", ".png", ".bmp" };
            
        }

        private void btnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            StackPanel stpanel = new StackPanel();
            stpanel.VerticalAlignment = VerticalAlignment.Top;

            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult dlgRes = fbd.ShowDialog();
            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                string[] files = Directory.GetFiles(fbd.SelectedPath);
                for (int i = 0; i < files.Length; i++)
                {
                    for (int j = 0; j < listPicture.Count; j++)
                    {
                        if (files[i].Substring(files[i].Length - 4) == listPicture[j])
                        {
                            masPicture.Add(files[i]);
                        }
                    }
                }
            }

            prgBarDownloadPict.Minimum = 0;
            prgBarDownloadPict.Maximum = masPicture.Count;
            prgBarDownloadPict.Value = 0;
            PrBarDownload(masPicture, stpanel);

            //scrlViewerImg.Content = stpanel;
            AddClickToImg(stpanel);
        }

       
       


        private Button createImage(int count, string str)
        {
            BitmapImage tempBmp = new BitmapImage();
            tempBmp.BeginInit();
            tempBmp.UriSource = new Uri(str);
            tempBmp.EndInit();
            //double w = tempBmp.Width;
            masSizeImg.Add(tempBmp.Height / tempBmp.Width);


            Image tempImg = new Image();
            tempImg.Margin = new Thickness(5);
            tempImg.StretchDirection = StretchDirection.DownOnly;
            tempImg.Source = tempBmp;


            string temp = "btnImg";
            Button btnImg = new Button();
            btnImg.Name = temp + count;
            btnImg.Content = tempImg;
            btnImg.Margin = new Thickness(3);

            return btnImg;
        }



        private void PrBarDownload(List<string> masPicture, StackPanel stpanel)
        {
            tempStPanel = null;
            UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(prgBarDownloadPict.SetValue);

                //Tight Loop:  Loop until the ProgressBar.Value reaches the max
              do
              {
                scrlViewerImg.Content = null;
                value += 1;
                stpanel.Children.Add(createImage(count, masPicture[count]));
                prgBarDownloadPict.Value = count + 1;
                count++;
                    /*Update the Value of the ProgressBar:
                      1)  Pass the "updatePbDelegate" delegate that points to the ProgressBar1.SetValue method
                      2)  Set the DispatcherPriority to "Background"
                      3)  Pass an Object() Array containing the property to update (ProgressBar.ValueProperty) and the new value */
                Dispatcher.Invoke(updatePbDelegate,
                    System.Windows.Threading.DispatcherPriority.Background, new object[] { ProgressBar.ValueProperty, value });

                scrlViewerImg.Content = stpanel;
              } while (prgBarDownloadPict.Value != prgBarDownloadPict.Maximum);

            tempStPanel = stpanel;
        }



        private void AddClickToImg(StackPanel stpanel)
        {
            if (stpanel.Children.Count != 0)
            {
                for (int i = 0; i < stpanel.Children.Count; i++)
                {
                    (stpanel.Children[i] as Button).Click += MainWindow_Click;
                }
            }
        }

        private void MainWindow_Click(object sender, RoutedEventArgs e)
        {
            Button tempBtn = new Button();
            tempBtn = sender as Button;
            for (int i = 0; i < tempStPanel.Children.Count; i++)
            {
                if ((tempStPanel.Children[i] as Button).Name == tempBtn.Name)
                {
                    BitmapImage tempBmp = new BitmapImage();
                    tempBmp.BeginInit();
                    tempBmp.UriSource = new Uri(masPicture[i]);
                    tempBmp.EndInit();
                    mainImg.Source = tempBmp;
                    ob = i;
                    txtBPath.Text = masPicture[i];
                }
            }
            //lstBoxMainImage.Items.Add(mainImg);
            //lstboxItemImg.Content = mainImg;
            lstBoxMainImage.Source = mainImg.Source;
        }

        private void sliderZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(lstBoxMainImage.Height > 0 && lstBoxMainImage.Width >0)
                lstBoxMainImage.MaxHeight = lstBoxMainImage.Width * masSizeImg[ob];
        }
    }
}


