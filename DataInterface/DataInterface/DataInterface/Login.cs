using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataInterface
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }
        SqlConnection server = new SqlConnection("Data Source = DESKTOP-5TFF0DC\\SQLEXPRESS; Initial Catalog = Arduino; Integrated Security = True");
        SqlCommand cmd = new SqlCommand();  //Komutları göndermek için yaratılan nesne.
        SqlDataReader dr;     //Veritabanından veri çekmek için oluşturulan reader nesnesi.
        private void Login_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            server.Open();
            SqlCommand cmd = new SqlCommand("select * from login where kullaniciAdi = '" + txtKullaniciAdi.Text + "'AND kullaniciSifre ='" + txtSifre.Text + "'", server);
            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                Form1 f1 = new Form1();
                f1.Show();
                this.Hide();
            }
            else
            {
                lblUyari.ForeColor = Color.Red;
                lblUyari.Text = "Kullanıcı Adı veya Şifre Hatalı !";

            }
            server.Close();
        }
    }
}
