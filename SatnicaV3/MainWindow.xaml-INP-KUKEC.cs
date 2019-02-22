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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace SatnicaV3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int ID = 0;


        public MainWindow()
        {
            string put = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\data.dat";
            try
            {
                if (!File.Exists(put))
                {
                    try
                    {
                        string ConString = System.Configuration.ConfigurationManager.ConnectionStrings["MyMSSOLS"].ConnectionString;
                        StreamWriter sw = new StreamWriter(put);
                        using (SqlConnection con = new SqlConnection(ConString))
                        {
                            con.Open();
                            string upit = string.Format("insert into Users (imeK)  Values ('{0}')", System.Environment.MachineName);
                            string upit2 = string.Format("select ID from Users where imeK = '{0}'", System.Environment.MachineName);
                            SqlCommand cmd = new SqlCommand(upit, con);
                            cmd.ExecuteNonQuery();
                            cmd = new SqlCommand(upit2, con);
                            ID = Convert.ToInt32(cmd.ExecuteScalar());
                            sw.WriteLine(ID.ToString());
                            sw.Flush();
                            sw.Close();
                        }
                    }
                    catch (Exception)
                    {
                        
                        throw;
                    }
                    
                }
                else
                {
                    try
                    {
                        StreamReader sr = new StreamReader(put);
                        ID = Convert.ToInt32(sr.ReadLine());
                        sr.Close();
                    }
                    catch (Exception)
                    {
                        
                        throw;
                    }
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            InitializeComponent();
            DTP_OdMjesec.SelectedDate = DateTime.Now;
            int C=0;
            int dc = DateTime.Now.Month;
            for (int i = 0; i < 12; i++)
			{
                if (i+1 == dc)
                {
                    C = i;
                }
                CB_OD_mjesec.Items.Add(i+1);
			}
            for (int i =DateTime.Now.Year-5; i <= DateTime.Now.Year; i++)
            {
                CB_OD_GOdinu.Items.Add(i);
            }
            CB_OD_GOdinu.SelectedIndex = 5;
            CB_OD_mjesec.SelectedIndex = C;
        }
        public int provjeriUbazi(DateTime dt)
        {
            try
            {
                string ConString = System.Configuration.ConfigurationManager.ConnectionStrings["MyMSSOLS"].ConnectionString;
                DateTime  sada = DateTime.Now;

                string upit = String.Format("select count(ID) from Dani where Date= '{0}-{1}-01' and UserIDs={2}",dt.Year, dt.Month, ID);
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand(upit, con);
                    int rez = (int)cmd.ExecuteScalar();
                    con.Close();
                    return rez;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return 0;
            }
        }
        public void unesiDane()
        {
            try
            {
                DateTime s = DateTime.Now;
                string ConString = System.Configuration.ConfigurationManager.ConnectionStrings["MyMSSOLS"].ConnectionString;
                using (SqlConnection con = new SqlConnection(ConString))
                    {
                        con.Open();
                        for (int i = 0; i < DateTime.DaysInMonth(s.Year,s.Month); i++)
                        {
                            int d = i + 1;
                            DateTime dan = new DateTime(s.Year, s.Month, d);
                            Int16 dw = Convert.ToInt16(dan.DayOfWeek);
                            string upit;
                            if (dw ==6||dw==0)
                            {
                                upit = String.Format("insert into Dani (Date,RZ,RadniDan, UserIDs) Values ('{0}-{1}-{2}', '0000', {3},{4})",s.Year, s.Month, d,0,ID);
                            }
                            else
	                        {
                                upit = String.Format("insert into Dani (Date,RZ,RadniDan, UserIDs) Values ('{0}-{1}-{2}', '0000', {3},{4})", s.Year, s.Month, d, 8,ID);
	                        }
                            SqlCommand cmd = new SqlCommand(upit, con);
                            cmd.ExecuteNonQuery();
                        }
                        
                        con.Close();
                    }

            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message);
            }
        }
        public int danUtjednu(DateTime dan)
        {
            Int16 dw = Convert.ToInt16(dan.DayOfWeek);
            if (dw ==6||dw==0)
            {
                return 0;
            }
            else
	        {
                return 8;
            }
        }
        private void B_Unesi_Click(object sender, RoutedEventArgs e)
        {
            if (provjeriUbazi(DTP_OdMjesec.SelectedDate.Value)==0)
            {
                unesiDane();
            }
            DateTime date = default(DateTime);
            string rz = "";
            string napomena = "";
            rz = CB_OdRZ.Text;
            if (rz !="Unesite RZ")    
            {
                date = DTP_OdMjesec.SelectedDate.Value;
                if (danUtjednu(date)==0)
                {
                    MessageBox.Show("Odabrani dan je vikend");
                    return;
                }
                if (TB_Napomena.Text != "Unesite napomenu")
                {
                    napomena = TB_Napomena.Text;
                }
                try 
	            {	        
		            String upit = String.Format("Update Dani Set RZ='{0}', Napomena='{1}' where Date = '{2}-{3}-{4}' and UserIDs = {5}",rz,napomena,date.Year,date.Month, date.Day, ID);
                    string ConString = System.Configuration.ConfigurationManager.ConnectionStrings["MyMSSOLS"].ConnectionString;
                    using (SqlConnection con = new SqlConnection(ConString))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand(upit, con);
                        cmd.ExecuteNonQuery();
                    }
	            }
	            catch (Exception)
	            {
		
		            throw;
	            }
            }
            System.Windows.Application.Current.Shutdown();
        }
        private void B_PoDanu_Click(object sender, RoutedEventArgs e)
        {
            PregledPoDanu ppd = new PregledPoDanu(Convert.ToInt32(CB_OD_mjesec.Text),Convert.ToInt32(CB_OD_GOdinu.Text),ID);
            ppd.Show();
            
        }

             

        private void B_PregledKartice_Click(object sender, RoutedEventArgs e)
        {
            string s = CB_OD_mjesec.Text.ToString();
            int ss = Convert.ToInt32(CB_OD_mjesec.Text);
            kartica k = new kartica(Convert.ToInt32(CB_OD_mjesec.Text), Convert.ToInt32(CB_OD_GOdinu.Text),ID);
            k.Show();
        }

        private void CB_OdRZ_GotFocus(object sender, RoutedEventArgs e)
        {
            if (CB_OdRZ.Text == "Unesite RZ")
            {
                CB_OdRZ.Text = "";
            }
        }

        private void CB_OdRZ_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CB_OdRZ.Text == "")
            {
                CB_OdRZ.Text = "Unesite RZ";
            }
        }

        
    }
}
