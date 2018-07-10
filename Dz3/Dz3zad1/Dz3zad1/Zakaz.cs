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

namespace Dz3zad1
{
    public partial class Zakaz : Form
    {
        private DataSet set = null;
        private int summ = 0;
        public Zakaz(DataSet dataSet)
        {
            InitializeComponent();
            set = dataSet;
            this.bunifuImageButton1_Close.Click += BunifuImageButton1_Close_Click;
            this.checkedListBox1_Tovar.SelectedIndexChanged += CheckedListBox1_Tovar_SelectedIndexChanged;
            this.button1_SummZakaz.Click += Button1_SummZakaz_Click;
            this.comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            this.button2.Click += Button2_Click;
            this.button2.Visible = false;
            Show_Zakaz();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (this.comboBox1_Prodavec.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите продовца", "Оповещение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите покупателя", "Оповещение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (Convert.ToInt32(label5_Mani.Text) < 0)
            {
                MessageBox.Show("Анатолий Сергеевич не балуйтесь!! У покупателя нет таких денег))", "Оповищение",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            SqlConnection connection = new SqlConnection(@"Data Source = DESKTOP-I859NEV; Initial Catalog=Score;Integrated Security=true;");
            SqlCommand command = connection.CreateCommand();
            SqlTransaction tran = null;
            try
            {
                connection.Open();
                tran = connection.BeginTransaction();
                command.Transaction = tran;
                DataRow row = set.Tables[2].NewRow();
                row[1] = DateTime.Today.ToString("d");
                for (int i = 0; i < set.Tables[0].Rows.Count; i++)
                {
                    foreach (var UPPER in this.panel1_count.Controls)
                    {
                        if (UPPER is ComboBox)
                        {
                            if (set.Tables[0].Rows[i][1].Equals((UPPER as ComboBox).Name))
                            {
                                command.CommandText =
                                    $"UPDATE Product SET Amount={(Convert.ToInt32(set.Tables[0].Rows[i][2]) - Convert.ToInt32((UPPER as ComboBox).SelectedItem))} WHERE Id={set.Tables[0].Rows[i][0]}";
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
                int count = 0;
                int Id_B = 0;
                for (int i = 0; i < set.Tables[1].Rows.Count; i++)
                {
                    if ($"{set.Tables[1].Rows[i][1]} {set.Tables[1].Rows[i][2]}".Equals(this.comboBox1.SelectedItem))
                    {
                        Id_B = Convert.ToInt32(set.Tables[1].Rows[i][0]);
                        count = 0;
                        foreach (var VARIABLE in this.panel1_count.Controls)
                        {
                            if (VARIABLE is ComboBox)
                            {
                                count += Convert.ToInt32((VARIABLE as ComboBox).SelectedItem);
                            }
                        }
                        command.CommandText =
                            $"UPDATE Buyer SET Purchases={(Convert.ToInt32(set.Tables[1].Rows[i][4]) + count)} WHERE Id={set.Tables[1].Rows[i][0]}";
                        command.ExecuteNonQuery();
                        command.CommandText =
                            $"UPDATE Buyer SET Moneyy={Convert.ToDecimal(this.label5_Mani.Text)} WHERE Id={set.Tables[1].Rows[i][0]}";
                        command.ExecuteNonQuery();
                    }
                }

                int Id_S = 0;
                for (int i = 0; i < set.Tables[3].Rows.Count; i++)
                {
                    if ($"{set.Tables[3].Rows[i][1]} {set.Tables[3].Rows[i][2]}".Equals(
                        this.comboBox1_Prodavec.SelectedItem))
                    {
                        Id_S = Convert.ToInt32(set.Tables[3].Rows[i][0]);
                    }
                }

                string []Position=new string[set.Tables[0].Rows.Count];
                int x = 0;
                foreach (var VARIABLE in panel1_count.Controls)
                {
                    if (VARIABLE is Label)
                    {
                        if ((VARIABLE as Label).Visible)
                        {
                            Position[x++] =(VARIABLE as Label).Text;
                        }
                    }
                }

                string Pos = String.Join(",", Position);
                int Count = count;
                decimal Sum = Convert.ToDecimal(this.label5_Summ.Text);
                command.CommandText =
                    $"insert into Checks values(('{DateTime.Now.Date}'), ({Id_B}), ({Id_S}), ('{Pos}'), ({Count}), ({Sum}));";
                command.ExecuteNonQuery();
                tran.Commit();
                DialogResult = DialogResult.OK;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                tran.Rollback();
                connection?.Close();

            }

        }

        private int Mony;
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.button2.Visible = false;
            Mony = 0;
            for (int i = 0; i < set.Tables[1].Rows.Count; i++)
            {
                if ($"{set.Tables[1].Rows[i][1]} {set.Tables[1].Rows[i][2]}".Equals(this.comboBox1.SelectedItem))
                {
                    Mony = Convert.ToInt32(set.Tables[1].Rows[i][3]);
                }
            }

            this.label5_Mani.Text = Mony.ToString();
        }

        private void Button1_SummZakaz_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите покупателя", "Оповещение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            summ = 0;
            for (int i = 0; i < set.Tables[0].Rows.Count; i++)
            {
                foreach (var UPPER in this.panel1_count.Controls)
                {
                    if (UPPER is ComboBox)
                    {
                        if (set.Tables[0].Rows[i][1].Equals((UPPER as ComboBox).Name))
                        {
                            summ += (Convert.ToInt32(set.Tables[0].Rows[i][3]) *
                                     Convert.ToInt32((UPPER as ComboBox).SelectedItem));

                        }
                    }
                }
            }

            this.label5_Summ.Text = summ.ToString();
            Mony -= summ;
            this.label5_Mani.Text = Mony.ToString();
            this.button2.Visible = true;
        }

        private void CheckedListBox1_Tovar_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < this.checkedListBox1_Tovar.Items.Count; i++)
            {
                if (checkedListBox1_Tovar.GetItemChecked(i))
                {
                    foreach (var VARIABLE in this.panel1_count.Controls)
                    {
                        if (VARIABLE is Label)
                        {
                            if (checkedListBox1_Tovar.Items[i].Equals((VARIABLE as Label).Text))
                            {
                                (VARIABLE as Label).Visible = true;
                            }
                        }
                        if (VARIABLE is ComboBox)
                        {
                            if (checkedListBox1_Tovar.Items[i].Equals((VARIABLE as ComboBox).Name))
                            {
                                (VARIABLE as ComboBox).Visible = true;
                            }
                        }
                    }
                }
                else if (!checkedListBox1_Tovar.GetItemChecked(i))
                {
                    foreach (var VARIABLE in this.panel1_count.Controls)
                    {
                        if (VARIABLE is Label)
                        {
                            if (checkedListBox1_Tovar.Items[i].Equals((VARIABLE as Label).Text))
                            {
                                (VARIABLE as Label).Visible = false;
                            }
                        }
                        if (VARIABLE is ComboBox)
                        {
                            if (checkedListBox1_Tovar.Items[i].Equals((VARIABLE as ComboBox).Name))
                            {
                                (VARIABLE as ComboBox).Visible = false;
                                (VARIABLE as ComboBox).SelectedIndex = 0;
                            }
                        }
                    }
                }
            }

        }

        private void BunifuImageButton1_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Show_Zakaz()
        {
            int x = 4, y = 5, z=121;
            for (int i = 0; i < set.Tables[3].Rows.Count; i++)
            {
                this.comboBox1_Prodavec.Items.Add($"{set.Tables[3].Rows[i][1]} {set.Tables[3].Rows[i][2]}");
            }

            for (int i = 0; i < set.Tables[0].Rows.Count; i++)
            {
                this.checkedListBox1_Tovar.Items.Add($"{set.Tables[0].Rows[i][1]}");
                ComboBox box=new ComboBox();
                Label label=new Label();
                label.ForeColor= Color.FromArgb(234, 97, 83);
                label.Text = set.Tables[0].Rows[i][1].ToString();
                label.Visible = false;
                label.Location = new Point(x,y);
                box.ForeColor=Color.FromArgb(234, 97, 83);
                box.BackColor = Color.FromArgb(43, 43, 43);
                box.Name = (set.Tables[0].Rows[i][1]).ToString();
                box.Location=new Point(z,y);
                box.SelectedIndexChanged += Box_SelectedIndexChanged;
                y += 23;
                for (int j = 0; j <= Convert.ToInt32(set.Tables[0].Rows[i][2]); j++)
                {
                    box.Items.Add((j).ToString());
                }
                box.SelectedIndex = 0;
                box.Visible = false;
                this.panel1_count.Controls.Add(label);
                this.panel1_count.Controls.Add(box);
            }

            for (int i = 0; i < set.Tables[1].Rows.Count; i++)
            {
                this.comboBox1.Items.Add($"{set.Tables[1].Rows[i][1]} {set.Tables[1].Rows[i][2]}");
            }
        }

        private void Box_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.button2.Visible = false;
            Mony = 0;
            for (int i = 0; i < set.Tables[1].Rows.Count; i++)
            {
                if ($"{set.Tables[1].Rows[i][1]} {set.Tables[1].Rows[i][2]}".Equals(this.comboBox1.SelectedItem))
                {
                    Mony = Convert.ToInt32(set.Tables[1].Rows[i][3]);
                }
            }

            this.label5_Mani.Text = Mony.ToString();
        }
    }
}
