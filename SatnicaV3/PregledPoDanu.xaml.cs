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
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace SatnicaV3
{
    /// <summary>
    /// Interaction logic for PregledPoDanu.xaml
    /// </summary>
    public partial class PregledPoDanu : Window
    {
        public DataTable dt;
        int Godina;
        int Mjesec;
        int ID;
        PregledPoDanu()
        {
            InitializeComponent();
            //dt = new DataTable();
        }
        public PregledPoDanu(int mjesec, int godina, int id)
        {
            InitializeComponent();
            Mjesec = mjesec;
            Godina = godina;
            ID = id;
        }


        private void W_PregledPoDanu_Loaded(object sender, RoutedEventArgs e)
        {
            dt = new DataTable("Pregled po danima");
            FillDataGrid2(out dt);
        }
        private void FillDataGrid(out DataTable dt)
        {
            dt = new DataTable("Pregled po danima");
            try
            {
                string ConString = System.Configuration.ConfigurationManager.ConnectionStrings["MyMSSOLS"].ConnectionString;

                string upit = string.Empty;
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    con.Open();
                    upit = String.Format("SELECT ID, dbo.dut(Date) as Dan, Date, RZ, Napomena FROM dbo.Dani where MONTH(Date) = {0} and year(Date) = {1} and UserIDs = {2}", Mjesec, Godina, ID);
                    SqlCommand cmd = new SqlCommand(upit, con);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    
                    sda.Fill(dt);
                    grdPPD.ItemsSource = dt.DefaultView;
                    con.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            

        }
        private void FillDataGrid2(out DataTable dt)
        {
            dt = new DataTable("Pregled po danima");
            try
            {
                string ConString = System.Configuration.ConfigurationManager.ConnectionStrings["MyMSSOLS"].ConnectionString;
                String constrsqlite = "Data Source=" + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\baza.db3";
                string upit = string.Empty;
                using (SQLiteConnection con = new SQLiteConnection(constrsqlite))
                {
                    con.Open();
                    //upit = String.Format("SELECT ID, Date, RZ, Napomena FROM Dani where MONTH(Date) = {0} and year(Date) = {1} and UserIDs = {2}", Mjesec, Godina, ID);
                    upit = String.Format("SELECT ID, Date, strftime('%w',Date) as Dan, RZ, Napomena FROM Dani where strftime('%m',Date) = '09' and strftime('%Y',Date) = '2017' and UserIDs = 2");
                    SQLiteCommand cmd = new SQLiteCommand(upit, con);
                    SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);

                    sda.Fill(dt);
                    grdPPD.ItemsSource = dt.DefaultView;
                    con.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }


        }
        private void UpdateDB(string upit)
        {
            try
            {
                string ConString = System.Configuration.ConfigurationManager.ConnectionStrings["MyMSSOLS"].ConnectionString;


                using (SqlConnection con = new SqlConnection(ConString))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand(upit, con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            

        }




        private void grdPPD_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            int row_index = ((DataGrid)sender).ItemContainerGenerator.IndexFromContainer(e.Row);
            DataRow dr = dt.Rows[row_index];
            int id = Convert.ToInt16(dr.ItemArray[0]);
            string rz = dr.ItemArray[3].ToString();
            string napomena = dr.ItemArray[4].ToString();
            string upit = String.Format("update Dani set RZ='{0}', Napomena= '{1}' where ID = {2};",rz , napomena, id);
            UpdateDB(upit);
        }

        private void grdPPD_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void grdPPD_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
