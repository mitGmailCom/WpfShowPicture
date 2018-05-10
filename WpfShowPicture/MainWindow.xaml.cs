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
        List<string> listPicture; // colection exts
        List<string> masPicture; // array Img
        //List<double> masSizeImg; // array height/width or width/height
        int count = 0; // count img
        double value = 0;
        Image mainImg;
        StackPanel tempStPanel;
        int ob = 0; // номер объекта на который пользователь кликнул в StPanel
        bool isHeightBigger { get; set; } = false;
        List<double> mass1; // height img
        List<double> mass2;// width img

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            masPicture = new List<string>();
            tempStPanel = new StackPanel();
            mainImg = new Image();
            listPicture = new List<string>() { ".jpg", ".png", ".bmp" };
            mass1 = new List<double>();
            mass2 = new List<double>();
        }

        private void btnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            txtBPath.Text = "";
            mass1.Clear();
            mass2.Clear();
            masPicture.Clear();
            tempStPanel.Children.Clear();
            lstBoxMainImage.Source = null;
            mainImg.Source = null;
            value = 0;
            ob = 0;
            sliderZoom.Value = 515;

            StackPanel stpanel = new StackPanel();
            stpanel.VerticalAlignment = VerticalAlignment.Top;

            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult dlgRes = fbd.ShowDialog();
            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                try
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
                catch { };
            }

            prgBarDownloadPict.Minimum = 0;
            prgBarDownloadPict.Maximum = masPicture.Count;
            prgBarDownloadPict.Value = 0;
            PrBarDownload(masPicture, stpanel);

            AddClickToImg(stpanel);
            //stpanel.Children.Clear();
        }



        private Button createImage(int count, string str)
        {
            //masSizeImg.Clear();
            BitmapImage tempBmp = new BitmapImage();
            tempBmp.BeginInit();
            tempBmp.UriSource = new Uri(str);
            tempBmp.EndInit();
           
            mass1.Add(tempBmp.Height);
            mass2.Add(tempBmp.Width);

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
            count = 0;
            tempStPanel = null;
            UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(prgBarDownloadPict.SetValue);

            Border borderStPanel = new Border();
            borderStPanel.Margin = new Thickness(3);
            borderStPanel.BorderBrush = Brushes.Black;
            borderStPanel.BorderThickness = new Thickness(2);
            borderStPanel.CornerRadius = new CornerRadius(5);
            if (masPicture.Count != 0)
            {
                do
                {
                    borderStPanel.Child = null;
                    scrlViewerImg.Content = null;
                    value++;//= 1;

                    stpanel.Children.Add(createImage(count, masPicture[count]));
                    scrlViewerImg.Content = borderStPanel;
                    borderStPanel.Child = stpanel;
                    prgBarDownloadPict.Value = count++;// 1;
                                                       //count++;
                    Dispatcher.Invoke(updatePbDelegate,
                      System.Windows.Threading.DispatcherPriority.Background, new object[] { ProgressBar.ValueProperty, value });

                    if (prgBarDownloadPict.Value == prgBarDownloadPict.Maximum)
                        Thread.Sleep(500);
                } while (prgBarDownloadPict.Value != prgBarDownloadPict.Maximum);
            }
            tempStPanel = stpanel;
            prgBarDownloadPict.Value = 0;
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
            
            txblExp.Text = "";
            Button tempBtn = new Button();
            tempBtn = sender as Button;
            for (int i = 0; i < tempStPanel.Children.Count; i++)
            {
                if ((tempStPanel.Children[i] as Button).Name == tempBtn.Name)
                {
                    lstBoxMainImage.MaxHeight = mass1[i];
                    lstBoxMainImage.MaxWidth = mass2[i];
                    
                    BitmapImage tempBmp = new BitmapImage();
                    tempBmp.BeginInit();
                    tempBmp.UriSource = new Uri(masPicture[i]);
                    tempBmp.EndInit();
                    mainImg.Source = tempBmp;
                    ob = i;
                    
                    txtBPath.Text = masPicture[i];
                    FileInfo fInfo = new FileInfo(masPicture[i]);
                    txblExp.Inlines.Add("Имя ф. - ");
                    txblExp.Inlines.Add(new Run($"{fInfo.Name.ToString()}") {FontWeight = FontWeights.Bold });

                    double dleng = fInfo.Length;
                    dleng = dleng / (1024*1024);
                    dleng = Math.Round(dleng, 1);
                    txblExp.Inlines.Add(" Размер ф. - ");
                    txblExp.Inlines.Add(new Run($"{ dleng.ToString()}") { FontWeight = FontWeights.Bold });

                    txblExp.Inlines.Add("мб Тип ф. - ");
                    txblExp.Inlines.Add(new Run($"{fInfo.Extension.ToString()}") { FontWeight = FontWeights.Bold });

                    txblExp.Inlines.Add(" Дата создания - ");
                    txblExp.Inlines.Add(new Run($"{fInfo.CreationTime.ToString()}") { FontWeight = FontWeights.Bold });

                    txblExp.Inlines.Add(" Дата открытия - ");
                    txblExp.Inlines.Add(new Run($"{fInfo.LastAccessTime.ToString()}") { FontWeight = FontWeights.Bold });
                }
            }
            lstBoxMainImage.Height = gridMainImg.Height;
            lstBoxMainImage.Source = mainImg.Source;
        }
    }
}


