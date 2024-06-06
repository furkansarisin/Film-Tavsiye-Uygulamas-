using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;


namespace projedemo
{
    public partial class Form1 : Form
    {
        // MySQL bağlantı dizesi
        private string connectionString = "server=localhost;port=3306;database=moviesdb;uid=root;password=1071;";

        // Dictionary koleksiyonları ComboBox'lar için kullanılacak
        private Dictionary<ComboBox, List<string>> comboBoxItems;

        public Form1()
        {

            InitializeComponent();
            InitializeFormStyles();
            LoadComboBoxData();

            // ComboBox'lar için verileri yükle
            LoadComboBoxData();

            // ComboBox'ların DropDownHeight özelliğini ayarla
            SetDropDownHeight(comboBoxActor1);
            SetDropDownHeight(comboBoxActor2);
            SetDropDownHeight(comboBoxDirector);
            SetDropDownHeight(comboBoxIMDB);
            SetDropDownHeight(comboBoxGenre);

            // ComboBox'lar için TextChanged olaylarını atayın
            comboBoxActor1.TextChanged += ComboBox_TextChanged;
            comboBoxActor2.TextChanged += ComboBox_TextChanged;
            comboBoxDirector.TextChanged += ComboBox_TextChanged;
            comboBoxIMDB.TextChanged += ComboBox_TextChanged;
            comboBoxGenre.TextChanged += ComboBox_TextChanged;

            // ListBox'ta film seçildiğinde resmi ve plot bilgisini göster
            listBoxMovies.SelectedIndexChanged += listBoxMovies_SelectedIndexChanged;

            this.buttonFragman.Click += new System.EventHandler(this.buttonFragman_Click);


        }

        //Arayüzünü daha estetik hale getirme
        private void InitializeFormStyles()
        {
            this.BackColor = Color.FromArgb(240, 240, 240); // Açık gri arka plan
            textBox1.BackColor = Color.White;
            textBox1.ForeColor = Color.Black;
            listBoxMovies.BackColor = Color.White;
            listBoxMovies.ForeColor = Color.Black;
            pictureBoxPoster.BorderStyle = BorderStyle.FixedSingle; // Hafifçe yuvarlak kenarlığı olan bir stil
            textBox1.Multiline = true;
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.Size = new Size(300, 200);
            SetDropDownHeight(comboBoxActor1);
            SetDropDownHeight(comboBoxActor2);
            SetDropDownHeight(comboBoxDirector);
            SetDropDownHeight(comboBoxIMDB);
            SetDropDownHeight(comboBoxGenre);

            Font formFont = new Font("Segoe UI", 8);
            Font listBoxFont = new Font("Segoe UI", 8);
            Font textBoxFont = new Font("Segoe UI", 8);

            this.Font = formFont;
            listBoxMovies.Font = listBoxFont;
            textBox1.Font = textBoxFont;



        }



        private void Form1_Load(object sender, EventArgs e)
        {

            // Form yüklendiğinde yapılacak işlemler buraya eklenebilir

        }


        // ListBox'ta bir öğe seçildiğinde resmi ve plot bilgisini göstermek için
        private void listBoxMovies_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxMovies.SelectedItem != null)
            {
                // Seçilen film adını alın
                string filmAdi = listBoxMovies.SelectedItem.ToString();

                // Veritabanından seçilen filmin MovieID'sini alın
                int filmID = FilmIDAl(filmAdi);

                // MovieID'ye göre resim yolunu oluşturun
                string resimYolu = Path.Combine("C:\\Users\\Furkan Sarışın\\Desktop\\DESIGN\\projedemo\\projedemo\\Images", $"{filmID}.jpg");

                SoundPlayer efekt;
                string soundFilePath = @"C:\Users\Furkan Sarışın\Desktop\DESIGN\FilmBOX Datas\FilmSelect.wav";
                efekt = new SoundPlayer(soundFilePath);
                efekt.Play();

                if (File.Exists(resimYolu))
                {
                    // Resmi PictureBox'a yükleyin
                    pictureBoxPoster.Image = Image.FromFile(resimYolu);
                }
                else
                {
                    // Resim dosyası bulunamazsa kullanıcıya mesaj gösterin
                    pictureBoxPoster.Image = null;
                    MessageBox.Show("Resim dosyası bulunamadı!");
                }

                // Plot bilgisini al ve textBox1'e göster
                string plot = PlotAl(filmAdi);

                // Plot metnini textBox1'e atayın
                textBox1.Multiline = true;
                textBox1.ScrollBars = ScrollBars.Vertical; // Dikey kaydırma çubuğunu etkinleştirin
                textBox1.Size = new Size(300, 200); // Boyutu ve istediğiniz büyüklüğü ayarlayabilirsiniz
                textBox1.Text = plot; // Plot metnini textBox1'e atayın
            }
        }
        private void buttonFragman_Click(object sender, EventArgs e)
        {
            // ListBox'tan seçilen film
            string selectedMovie = listBoxMovies.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(selectedMovie))
            {
                // Fragman URL'sini al
                string trailerUrl = GetTrailerUrlFromDatabase(selectedMovie);

                // URL'yi varsayılan tarayıcıda aç
                if (!string.IsNullOrEmpty(trailerUrl))
                {
                    OpenUrlInBrowser(trailerUrl);
                }
                else
                {
                    MessageBox.Show("Fragman URL'si bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Lütfen bir film seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private string GetTrailerUrlFromDatabase(string movieName)
        {
            string trailerUrl = string.Empty;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT Link FROM film WHERE filmName = @MovieName";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@MovieName", movieName);

                try
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        trailerUrl = result.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Veritabanında fragman URL'si bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanından URL alınırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return trailerUrl;
        }



        private void OpenUrlInBrowser(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("URL açılırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("URL boş veya geçersiz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }











        private int FilmIDAl(string filmAdi)
        {
            int filmID = 0;
            string sorgu = "SELECT FilmID FROM film WHERE filmName = @filmName";

            using (MySqlConnection baglanti = new MySqlConnection(connectionString))
            {
                MySqlCommand komut = new MySqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@filmName", filmAdi);

                try
                {
                    baglanti.Open();
                    object sonuc = komut.ExecuteScalar();
                    if (sonuc != null && sonuc != DBNull.Value)
                    {
                        filmID = Convert.ToInt32(sonuc);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanı bağlantısı sırasında bir hata oluştu: " + ex.Message);
                }
            }

            return filmID;
        }

        private string PlotAl(string filmAdi)
        {
            string plot = "";
            string sorgu = "SELECT Plot FROM film WHERE filmName = @filmName";

            using (MySqlConnection baglanti = new MySqlConnection(connectionString))
            {
                MySqlCommand komut = new MySqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@filmName", filmAdi);

                try
                {
                    baglanti.Open();
                    object sonuc = komut.ExecuteScalar();
                    if (sonuc != null && sonuc != DBNull.Value)
                    {
                        plot = sonuc.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanı bağlantısı sırasında bir hata oluştu: " + ex.Message);
                }
            }

            return plot;
        }

        private void listeleBtn_Click(object sender, EventArgs e)
        {
            string actor1 = comboBoxActor1.SelectedItem?.ToString();
            string actor2 = comboBoxActor2.SelectedItem?.ToString();
            string director = comboBoxDirector.SelectedItem?.ToString();
            double imdbRating = 0;
            double.TryParse(comboBoxIMDB.SelectedItem?.ToString(), out imdbRating);
            string genre = comboBoxGenre.SelectedItem?.ToString();

            // SQL sorgusunu oluşturun
            string query = "SELECT * FROM film WHERE 1=1";

            if (!string.IsNullOrEmpty(actor1) && !string.IsNullOrEmpty(actor2))
            {
                query += " AND ((actor1 = @actor1 AND actor2 = @actor2) OR (actor1 = @actor2 AND actor2 = @actor1))";
            }
            else if (!string.IsNullOrEmpty(actor1))
            {
                query += " AND (actor1 = @actor1 OR actor2 = @actor1)";
            }
            else if (!string.IsNullOrEmpty(actor2))
            {
                query += " AND (actor1 = @actor2 OR actor2 = @actor2)";
            }
            if (!string.IsNullOrEmpty(director))
            {
                query += " AND director = @director";
            }
            if (imdbRating > 0)
            {
                query += " AND imdb_rating >= @imdb_rating";
            }
            if (!string.IsNullOrEmpty(genre))
            {
                query += " AND Filmgenre = @genre";
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                if (!string.IsNullOrEmpty(actor1))
                {
                    command.Parameters.AddWithValue("@actor1", actor1);
                }
                if (!string.IsNullOrEmpty(actor2))
                {
                    command.Parameters.AddWithValue("@actor2", actor2);
                }
                if (!string.IsNullOrEmpty(director))
                {
                    command.Parameters.AddWithValue("@director", director);
                }
                if (imdbRating > 0)
                {
                    command.Parameters.AddWithValue("@imdb_rating", imdbRating);
                }
                if (!string.IsNullOrEmpty(genre))
                {
                    command.Parameters.AddWithValue("@genre", genre);
                }

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

                    // ListBox'u temizleyin
                    listBoxMovies.Items.Clear();

                    // Sonuçları ListBox'a ekleyin
                    while (reader.Read())
                    {
                        string movieTitle = reader["filmName"].ToString(); // Buradaki sütun adının doğru olduğundan emin olun
                        listBoxMovies.Items.Add(movieTitle);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanı bağlantısı sırasında bir hata oluştu: " + ex.Message);
                }
            }
        }

        private void ComboBox_TextChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string searchText = comboBox.Text.ToLower();

            // ComboBox'ın öğelerini arama için geçici bir liste olarak al
            List<string> items = comboBoxItems[comboBox];

            // Arama metnine göre ComboBox'ın öğelerini filtrele
            comboBox.BeginUpdate();
            comboBox.Items.Clear();
            foreach (string item in items)
            {
                if (item.ToLower().Contains(searchText))
                {
                    comboBox.Items.Add(item);
                }
            }
            comboBox.EndUpdate();

            // Aranan metin ComboBox'ta yoksa, ComboBox'i temizle ve orijinal öğeleri tekrar ekle
            if (comboBox.Items.Count == 0 && !string.IsNullOrWhiteSpace(searchText))
            {
                comboBox.Items.Clear();
                comboBox.Items.AddRange(items.ToArray());
                comboBox.Text = searchText; // Yazılan metni geri yükle
                comboBox.Select(searchText.Length, 0); // Yazılan metnin sonuna odaklanır
                comboBox.DroppedDown = false; // ComboBox'i kapat
            }
            else
            {
                comboBox.Select(searchText.Length, 0); // İlk harften sonra odaklanır
                comboBox.DroppedDown = true; // ComboBox'i aç
            }
        }


        private void LoadComboBoxData()
        {
            comboBoxItems = new Dictionary<ComboBox, List<string>>();

            // comboBoxActor1 için actor1 ve actor2 sütunlarını yükle
            LoadComboBoxData(comboBoxActor1, "actor1", "actor2");

            // comboBoxActor2 için actor1 ve actor2 sütunlarını yükle
            LoadComboBoxData(comboBoxActor2, "actor2", "actor1");

            // Diğer ComboBox'lar için verileri yükle
            LoadComboBoxData(comboBoxDirector, "director");
            LoadComboBoxData(comboBoxIMDB, "imdb_rating");
            LoadComboBoxData(comboBoxGenre, "Filmgenre");
        }

        private void LoadComboBoxData(ComboBox comboBox, string columnName1, string columnName2 = null)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            string query;

            if (columnName2 == null)
            {
                query = $"SELECT DISTINCT {columnName1} FROM film ORDER BY {columnName1}";
            }
            else
            {
                query = $"SELECT DISTINCT {columnName1} FROM film UNION SELECT DISTINCT {columnName2} FROM film ORDER BY {columnName1}";
            }

            MySqlCommand command = new MySqlCommand(query, connection);

            List<string> items = new List<string>();

            try
            {
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string item = reader[columnName1].ToString().Trim(); // Boşlukları temizle
                    comboBox.Items.Add(item);
                    items.Add(item);
                }

                if (columnName2 != null)
                {
                    reader.NextResult(); // İkinci sorgu için bir sonraki sonuca geç
                    while (reader.Read())
                    {
                        string item = reader[columnName2].ToString().Trim(); // Boşlukları temizle
                        comboBox.Items.Add(item);
                        items.Add(item);
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veritabanından {columnName1} ve {columnName2} verilerini yüklerken hata oluştu: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }

            if (!comboBoxItems.ContainsKey(comboBox))
            {
                comboBoxItems.Add(comboBox, items);
            }
            else
            {
                comboBoxItems[comboBox].AddRange(items);
            }
        }




        private void SetDropDownHeight(ComboBox comboBox)
        {
            comboBox.DropDownHeight = 200; // İstediğiniz yüksekliği burada ayarlayabilirsiniz
        }

        private void comboBoxActor1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Bu olay işleyicisi şimdilik boş kalabilir.
        }

        private void comboBoxActor2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Bu olay işleyicisi şimdilik boş kalabilir.
        }

        private void comboBoxDirector_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Bu olay işleyicisi şimdilik boş kalabilir.
        }

        private void comboBoxIMDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Bu olay işleyicisi şimdilik boş kalabilir.
        }

        private void comboBoxGenre_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Bu olay işleyicisi şimdilik boş kalabilir.
        }

        private void pictureActor1_Click(object sender, EventArgs e)
        {

        }

        private void pictureActor2_Click(object sender, EventArgs e)
        {

        }
        private bool isDarkMode = false; // Başlangıçta dark mode kapalı

        private void buttonDarkMode_Click(object sender, EventArgs e)
        {
            isDarkMode = !isDarkMode; // Durumu tersine çevir

            if (isDarkMode)
            {
                // Dark mode açıkken yapılacak işlemler
                EnableDarkMode();
            }
            else
            {
                // Dark mode kapalıyken yapılacak işlemler
                DisableDarkMode();
            }
        }

        private void EnableDarkMode()
        {
            // Dark mode açıkken yapılan ayarlamalar
            this.BackColor = Color.FromArgb(31, 31, 31); // Dark mode arka plan rengi
            this.ForeColor = Color.White; // Dark mode yazı rengi

            // Button rengi
            buttonDarkMode.BackColor = Color.FromArgb(50, 50, 50); // Dark mode button arka plan rengi
            buttonDarkMode.ForeColor = Color.Black; // Dark mode button yazı rengi
        }

        private void DisableDarkMode()
        {
            // Dark mode kapalıyken yapılan ayarlamalar
            this.BackColor = Color.FromArgb(240, 240, 240); // Açık gri arka plan
            this.ForeColor = Color. Black; // Siyah yazı rengi

            // Button rengi
            buttonDarkMode.BackColor = Color.White; // Light mode button arka plan rengi
            buttonDarkMode.ForeColor = Color.Black; // Light mode button yazı rengi
        }
        

    }
}