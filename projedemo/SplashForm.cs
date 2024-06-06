using System;
using System.Media;
using System.Windows.Forms;

namespace projedemo
{
    public partial class SplashForm : Form
    {
        public SplashForm()
        {

            SoundPlayer intro;
            string soundFilePath = @"C:\Users\Furkan Sarışın\Desktop\DESIGN\FilmBOX Datas\Jenerik.wav";
            intro = new SoundPlayer(soundFilePath);
            intro.Play();

            InitializeComponent();
            this.Opacity = 0; // Başlangıçta tamamen şeffaf
            timer1.Interval = 10; // 10 ms aralıklarla çalışacak
            timer1.Tick += timer1_Tick;
            timer1.Start();
        }

        bool islem = false;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!islem)
            {
                this.Opacity += 0.0065;
            }
            if (this.Opacity == 1.0)
            {
                islem = true;
            }
            if (islem)
            {
                this.Opacity -= 0.0065;
                if (this.Opacity == 0)
                {
                    timer1.Enabled = false; // Zamanlayıcıyı durdur
                    Form1 getir = new Form1();
                    getir.Show();
                    this.Hide(); // SplashForm'u gizle
                }
            }
        }

        private void SplashForm_Load(object sender, EventArgs e)
        {

        }
    }
}
