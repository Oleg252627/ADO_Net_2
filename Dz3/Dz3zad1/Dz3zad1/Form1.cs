using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dz3zad1
{
    public partial class Form1 : Form
    {
        private SqlConnection connection = null;
        private SqlDataAdapter dataAdapter = null;
        private DataSet set = null;
        private SqlCommandBuilder builder = null;
        public Form1()
        {
            InitializeComponent();
            this.bunifuImageButton1_Close.Click += BunifuImageButton1_Close_Click;
            this.button1_Zakaz.Click += Button1_Zakaz_Click;
            Show_Table();
        }

        private void Button1_Zakaz_Click(object sender, EventArgs e)
        {
            Zakaz A=new Zakaz(set);
            if (A.ShowDialog() == DialogResult.OK)
            {
                Show_Table();
            }
        }

        private void BunifuImageButton1_Close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Show_Table()
        {
            connection = new SqlConnection();
            connection.ConnectionString = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            string Sql_command= @"SELECT * FROM Product;SELECT * FROM Buyer;SELECT
            Checks.DataSale AS'Дата продажи',
            Buyer.FirstName + ' ' + Buyer.LastName AS 'Покупатель',
            Seller.FirstName + ' ' + Seller.LastName AS 'Продавец',
            Checks.Position AS 'Название',
            Checks.AmountPosition AS 'Количество',
            Checks.Summa AS 'На сумму'
            FROM Buyer INNER JOIN Checks ON Buyer.Id = Checks.IdBuyer
            INNER JOIN Seller ON Checks.IdSeller = Seller.Id; SELECT * FROM Seller";
            set=new DataSet();
            dataAdapter=new SqlDataAdapter(Sql_command,connection);
            builder=new SqlCommandBuilder(dataAdapter);
            dataAdapter.Fill(set);
            this.dataGridView1_Produkt.DataSource = null;
            this.dataGridView1_Pokupat.DataSource = null;
            this.dataGridView1_cheks.DataSource = null;
            this.dataGridView1_Produkt.DataSource = set.Tables[0];
            this.dataGridView1_Pokupat.DataSource = set.Tables[1];
            this.dataGridView1_cheks.DataSource = set.Tables[2];
        }
    }
}
