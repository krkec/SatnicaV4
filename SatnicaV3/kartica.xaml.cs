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
using System.Globalization;

namespace SatnicaV3
{
    /// <summary>
    /// Interaction logic for kartica.xaml
    /// </summary>
    /// 

    public partial class kartica : Window
    {
        int Mjesec;
        int Godina;
        int ID;
        private DataTable dt;
        private kartica()
        {
            InitializeComponent();
            dt = new DataTable();
        }

        public kartica(int mjesec, int godina, int id)
        {
            InitializeComponent();
            Mjesec =mjesec;
            Godina =godina;
            ID = id;
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

                    DataTable dtt = new DataTable();
                    con.Open();
                    //upit = String.Format("SELECT ID, Date, RZ, Napomena FROM Dani where MONTH(Date) = {0} and year(Date) = {1} and UserIDs = {2}", Mjesec, Godina, ID);
                    //upit = String.Format("SELECT ID, Date, strftime('%w',Date) as Dan, RZ, Napomena FROM Dani where strftime('%m',Date) = '09' and strftime('%Y',Date) = '2017' and UserIDs = 2");
                    upit = "SELECT RZ FROM Dani where strftime('%m',Date) = '09' and strftime('%Y',Date) = '2017' and UserIDs = 2 group by RZ";
                    SQLiteCommand cmd = new SQLiteCommand(upit, con);
                    SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                    sda.Fill(dtt);
                    foreach (DataRow dr in dtt.Rows)
	                {
		               var T1 = dtt.Rows.w
	                }

                    Pivot(dt.Columns[]
                    //grdPPD.ItemsSource = dt.DefaultView;
                    con.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }


        }
        DataTable Pivot(DataTable dt, DataColumn pivotColumn, DataColumn pivotValue)
        {
            // find primary key columns 
            //(i.e. everything but pivot column and pivot value)
            DataTable temp = dt.Copy();
            temp.Columns.Remove(pivotColumn.ColumnName);
            temp.Columns.Remove(pivotValue.ColumnName);
            string[] pkColumnNames = temp.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToArray();

            // prep results table
            DataTable result = temp.DefaultView.ToTable(true, pkColumnNames).Copy();
            result.PrimaryKey = result.Columns.Cast<DataColumn>().ToArray();
            dt.AsEnumerable()
                .Select(r => r[pivotColumn.ColumnName].ToString())
                .Distinct().ToList()
                .ForEach(c => result.Columns.Add(c, pivotColumn.DataType));

            // load it
            foreach (DataRow row in dt.Rows)
            {
                // find row to update
                DataRow aggRow = result.Rows.Find(
                    pkColumnNames
                        .Select(c => row[c])
                        .ToArray());
                // the aggregate used here is LATEST 
                // adjust the next line if you want (SUM, MAX, etc...)
                aggRow[row[pivotColumn.ColumnName].ToString()] = row[pivotValue.ColumnName];
            }

            return result;
        }
        private void FillDataGrid(out DataTable dt)
        {
            dt = new DataTable("Pregled kartice");
            try
            {
                string ConString = System.Configuration.ConfigurationManager.ConnectionStrings["MyMSSOLS"].ConnectionString;

                string upit = string.Empty;

                using (SqlConnection con = new SqlConnection(ConString))
                {
                    upit = string.Format("EXEC pKartice @DatePeriod = '{0}-{1}-1', @ID = {2}", Godina, Mjesec,ID);
                    SqlCommand cmd = new SqlCommand(upit, con);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);

                    sda.Fill(dt);
                    grdKartica.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            

        }

        private void grdKartica_Loaded(object sender, RoutedEventArgs e)
        {
            FillDataGrid(out dt);
        }
    }
    static class DateTimeExtensions
    {
        static GregorianCalendar _gc = new GregorianCalendar();
        public static int GetWeekOfMonth(this DateTime time)
        {
            DateTime first = new DateTime(time.Year, time.Month, 1);
            return time.GetWeekOfYear() - first.GetWeekOfYear() + 1;
        }

        static int GetWeekOfYear(this DateTime time)
        {
            return _gc.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }
    }
}
