using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Data.SqlClient;


namespace DataInterface
{
    public partial class Form1 : Form
    {
        string[] ports = SerialPort.GetPortNames();
        public Form1()
        {
            InitializeComponent();
        }

        string sonuc;



        SqlConnection server = new SqlConnection("Data Source = DESKTOP-5TFF0DC\\SQLEXPRESS; Initial Catalog = Arduino; Integrated Security = True");






        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            {
                if (serialPort1.IsOpen == true)//Serial Portu kapatma
                {
                    serialPort1.Close();
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel3.Text = DateTime.Now.ToString();
            dataGridView1.Refresh();
            dataGridView2.Refresh();
            dataGridView3.Refresh();
            this.chart1.Update();
            this.chart2.Update();
            this.chart3.Update();
            dataGridView4.Refresh();

            this.sicaklikTableAdapter.Fill(this.arduinoDataSet.sicaklik);
            this.gazTableAdapter.Fill(this.arduinoDataSet.gaz);
            this.gazKritikTableAdapter.Fill(this.arduinoDataSet.gazKritik);
            this.sicaklikKritikTableAdapter.Fill(this.arduinoDataSet.sicaklikKritik);



        }

        public void sicaklikListele()
        {

            server.Open();
            SqlCommand cmd = new SqlCommand(("select * from sicaklik where nem>=50  or sicaklik>=25"), server);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            //SqlDataReader dr = cmd.ExecuteReader();
            da.Fill(dt);
            dataGridView3.DataSource = dt;
            this.chart2.DataBind();
            cmd.ExecuteNonQuery();
            server.Close();

            foreach (DataGridViewRow row in dataGridView3.Rows)
            {
                server.Open();

                if (!row.IsNewRow)
                {
                    SqlCommand cmd2 = new SqlCommand(("insert into sicaklikKritik(tarih,sicaklik,nem) values (@tarih, @sicaklik, @nem)"), server);
                    cmd2.Parameters.AddWithValue("@tarih", row.Cells[1].Value);
                    cmd2.Parameters.AddWithValue("@sicaklik", row.Cells[2].Value);
                    cmd2.Parameters.AddWithValue("@nem", row.Cells[3].Value);
                    cmd2.ExecuteNonQuery();
                }
                server.Close();
                this.chart2.DataBind();
                this.chart3.DataBind();
            }

        }
        public void gazListele()
        {
            int gaz = 500;
            server.Open();
            SqlCommand cmd = new SqlCommand(("select * from gaz where gaz>=500"), server);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView4.DataSource = dt;
            this.chart1.DataBind();
            cmd.ExecuteNonQuery();
            server.Close();
            foreach (DataGridViewRow row in dataGridView4.Rows)
            {
                server.Open();

                if (!row.IsNewRow)
                {
                    SqlCommand cmd2 = new SqlCommand(("insert into gazKritik(tarih,gaz) values (@tarih, @gaz)"), server);
                    cmd2.Parameters.AddWithValue("@tarih", row.Cells[1].Value);
                    cmd2.Parameters.AddWithValue("@gaz", row.Cells[2].Value);
                    cmd2.ExecuteNonQuery();
                }
                server.Close();
                this.chart1.DataBind();
            }

        }
        private void timer2_Tick(object sender, EventArgs e)
        {

            try
            {
                if (serialPort1.ReadExisting().EndsWith("\n"))
                {
                    sonuc = serialPort1.ReadLine();
                    label5.Text = sonuc;


                    if (Convert.ToInt32(sonuc) >= 1000 && Convert.ToInt32(sonuc) < 5000)
                    {
                        string sicaklik = sonuc;
                        label8.Text = sicaklik.Substring(0, 2);
                        string nem = sonuc;
                        label9.Text = sicaklik.Substring(2, 2);
                        server.Open();
                        SqlCommand cmd = new SqlCommand("insert into sicaklik(tarih,sicaklik,nem) values (@tarih, @sicaklik, @nem)", server);
                        cmd.Parameters.AddWithValue("@tarih", toolStripStatusLabel3.Text);
                        cmd.Parameters.AddWithValue("@sicaklik", Convert.ToInt32(label8.Text));
                        cmd.Parameters.AddWithValue("@nem", Convert.ToInt32(label9.Text));
                        cmd.ExecuteNonQuery();
                        server.Close();
                        sicaklikListele();
                        chart2.DataBind();
                        chart3.DataBind();

                    }
                    if (Convert.ToInt32(sonuc) < 1000 && Convert.ToInt32(sonuc) > 100)
                    {
                        server.Open();
                        SqlCommand cmd = new SqlCommand("insert into gaz(tarih,gaz) values (@tarih, @gaz)", server);
                        cmd.Parameters.AddWithValue("@tarih", toolStripStatusLabel3.Text);
                        cmd.Parameters.AddWithValue("@gaz", Convert.ToInt32(label5.Text));
                        cmd.ExecuteNonQuery();
                        server.Close();
                        gazListele();
                        chart1.DataBind();
                    }
                }




            }
            catch (Exception ex)
            {

                
                timer2.Stop();
            }

        }



        private void button1_Click(object sender, EventArgs e)
        {
            timer2.Start();

            if (serialPort1.IsOpen == false)
            {
                if (comboBox1.Text == "")
                    return;
                serialPort1.PortName = comboBox1.Text;
                serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                try
                {
                    serialPort1.Open();
                    label3.ForeColor = Color.Green;
                    label3.Text = "Bağlantı Açık";
                }
                catch (Exception hata)
                {
                    MessageBox.Show("Hata :" + hata.Message);


                }
            }
            else
            {
                label3.Text = "Bağlantı Kuruldu";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer2.Stop();
            if (serialPort1.IsOpen == true)
            {
                serialPort1.Close();
                label3.ForeColor = Color.Red;
                label3.Text = "Bağlantı Kapalı";
                label5.Text = "";
                label11.Text = "";
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            server.Open();
            SqlCommand cmd = new SqlCommand("TRUNCATE TABLE sicaklik", server);
            SqlCommand cmd2 = new SqlCommand("TRUNCATE TABLE sicaklikKritik", server);
            cmd.ExecuteNonQuery();
            cmd2.ExecuteNonQuery();
            server.Close();
            dataGridView3.DataSource = "";
            chart3.Series["nem"].Points.Clear();
            chart2.Series["sicaklik"].Points.Clear();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            server.Open();
            SqlCommand cmd = new SqlCommand("TRUNCATE TABLE gaz", server);
            SqlCommand cmd2 = new SqlCommand("TRUNCATE TABLE gazKritik", server);
            cmd.ExecuteNonQuery();
            cmd2.ExecuteNonQuery();
            server.Close();
            dataGridView4.DataSource = "";
            chart1.Series["gaz"].Points.Clear();
        }



        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            this.sicaklikTableAdapter.Fill(this.arduinoDataSet.sicaklik);
            this.gazTableAdapter.Fill(this.arduinoDataSet.gaz);
            this.gazKritikTableAdapter.Fill(this.arduinoDataSet.gazKritik);
            this.sicaklikKritikTableAdapter.Fill(this.arduinoDataSet.sicaklikKritik);

            


            sicaklikListele();
            gazListele();

            timer1.Start();

            toolStripStatusLabel1.Text = "EMİN ÖLMEZ - GİZEM ÖZGÜL                                                                             ";
            toolStripStatusLabel2.Text = "Arduino İle Ortam Takibi ve Güvenliği                                                                          ";
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
                comboBox1.SelectedIndex = 0;
            }
            comboBox2.Items.Add("2400");
            comboBox2.Items.Add("4800");
            comboBox2.Items.Add("9600");
            comboBox2.Items.Add("19200");
            comboBox2.Items.Add("115200");
            comboBox2.SelectedIndex = 2;
            label3.Text = "Bağlantı Kapalı";
            comboBox1.DataSource = SerialPort.GetPortNames();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
