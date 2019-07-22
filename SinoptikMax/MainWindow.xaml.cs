using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
using System.Xml.Linq;

namespace SinoptikMax
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WebClient client = new WebClient();
        byte[] buff;
        string url;
        string path;
        List<City> city = new List<City>()
        {
            new City(){ Name = "Винница", Path = @"..\..\Data\Vinnitsa.xml", Url = "http://www.pogoda.by/rss2/cityrss.php?q=33562" },
            new City(){ Name = "Днепр", Path = @"..\..\Data\Dnepr.xml", Url = "http://www.pogoda.by/rss2/cityrss.php?q=34504" },
            new City(){ Name = "Житомир", Path = @"..\..\Data\Jitomir.xml", Url = "http://www.pogoda.by/rss2/cityrss.php?q=33325" },
            new City(){ Name = "Запорожье", Path = @"..\..\Data\Zaporozhie.xml", Url = "http://www.pogoda.by/rss2/cityrss.php?q=34601" },
            new City(){ Name = "И.Франковск", Path = @"..\..\Data\Frankovsk.xml", Url = "http://www.pogoda.by/rss2/cityrss.php?q=33526" },
            new City(){ Name = "Киев", Path = @"..\..\Data\Kyiv.xml", Url = "http://www.pogoda.by/rss2/cityrss.php?q=33345" },
            new City(){ Name = "Луцк", Path = @"..\..\Data\Lutsk.xml", Url = "http://www.pogoda.by/rss2/cityrss.php?q=33187" },
            new City(){ Name = "Львов", Path = @"..\..\Data\Lvov.xml", Url = "http://www.pogoda.by/rss2/cityrss.php?q=33393" },
            new City(){ Name = "Николаев", Path = @"..\..\Data\Nikolaev.xml", Url = "http://www.pogoda.by/rss2/cityrss.php?q=33788" },
            new City(){ Name = "Одесса", Path = @"..\..\Data\Odessa.xml", Url = "http://www.pogoda.by/rss2/cityrss.php?q=33837" },
            new City(){ Name = "Ровно", Path = @"..\..\Data\Rovno.xml", Url = "http://www.pogoda.by/rss2/cityrss.php?q=33301" },
            new City(){ Name = "Сумы", Path = @"..\..\Data\Sumi.xml", Url = "http://www.pogoda.by/rss2/cityrss.php?q=33275" },
            new City(){ Name = "Тернополь", Path = @"..\..\Data\Ternopol.xml", Url = "http://www.pogoda.by/rss2/cityrss.php?q=33415" },
            new City(){ Name = "Ужгород", Path = @"..\..\Data\Uzghorod.xml", Url = "http://www.pogoda.by/rss2/cityrss.php?q=33631" },
            new City(){ Name = "Харьков", Path = @"..\..\Data\Harkov.xml", Url = "http://www.pogoda.by/rss2/cityrss.php?q=34300" },
            new City(){ Name = "Херсон", Path = @"..\..\Data\Herson.xml", Url = "http://www.pogoda.by/rss2/cityrss.php?q=33902" },
            new City(){ Name = "Хмельницкий", Path = @"..\..\Data\Khmelnitskiy.xml", Url = "http://www.pogoda.by/rss2/cityrss.php?q=33429" },
            new City(){ Name = "Чернигов", Path = @"..\..\Data\Chernihov.xml", Url = "http://www.pogoda.by/rss2/cityrss.php?q=33135" }
        };

        public MainWindow()
        {
            InitializeComponent();
            LoadItems();
        }

        public void LoadItems()
        {
            date_label.Content = DateTime.Now.ToString();

            foreach (City c in city)
                city_comboBox.Items.Add(c.Name);

            city_comboBox.SelectedIndex = 5;
        }

        public void LoadFile(string _url, string _path)
        {
            buff = client.DownloadData(_url);
            string rss = Encoding.UTF8.GetString(buff);

            using (FileStream fs = new FileStream(_path,
                FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(rss);
                }
            }
            //MessageBox.Show("Saved");
            XDocument doc = XDocument.Load(_path);
            var res = doc.Element("rss").Element("channel").Element("item").Element("description");
            //MessageBox.Show(res.ToString());
            string[] subStrings = Convert.ToString(res).Split('|');             // строка в теге description
            string[] firstsubstr = subStrings[0].Split(' ');                    // строка с температурой и символами в конце
            string[] secondsubstr = firstsubstr[1].Split('&');                  // строка с температурой

            temperature_label.Content = secondsubstr[0] + " °C";    // температура

            presure_label.Content = subStrings[1];                                 // давление        
            wind_label.Content = subStrings[2];                                    // ветер
            describ_label.Content = subStrings[3];                                 // особые явления
            view_label.Content = subStrings[4];                                    // видимость

            string[] second_humidity_str = subStrings[5].Split(']');            // строка с влажностью и лишними символами в конце  
            humidity_label.Content = second_humidity_str[0];                       // строка с влажностью
        }

        private void City_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            date_label.Content = DateTime.Now.ToString();
            url = city.Where(c => c.Name == city_comboBox.SelectedItem.ToString()).FirstOrDefault().Url;
            path = city.Where(c => c.Name == city_comboBox.SelectedItem.ToString()).FirstOrDefault().Path;
            LoadFile(url, path);
        }

        private void Reload_button1_Click(object sender, RoutedEventArgs e)
        {
            date_label.Content = DateTime.Now.ToString();
            LoadFile(url, path);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start("http://www.pogoda.by/rss2/ ");
        }
    }
}
