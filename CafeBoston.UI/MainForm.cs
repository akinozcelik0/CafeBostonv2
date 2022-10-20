using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using CafeBoston.DATA;

namespace CafeBoston.UI
{
    public partial class MainForm : Form
    {
        CafeData db = new CafeData();
        private TableMoveHandler frmOrder_TableMoving;

        public MainForm()
        {
            InitializeComponent();
            LoadSavedData();
            SeedSampleProducts();
            LoadTables();
        }

        private void LoadSavedData()
        {
            try
            {
                string json = File.ReadAllText("data.json");
                db = JsonSerializer.Deserialize<CafeData>(json);
            }
            catch (Exception)
            {
            }
        }

        private void SeedSampleProducts()
        {
            if (db.Products.Any()) return;
            db.Products.Add(new Product() { ProductName = "Cola", UnitPrice = 14.50m });
            db.Products.Add(new Product() { ProductName = "Tea", UnitPrice = 9.50m });
        }

        private void LoadTables()
        {
            for (int i = 1; i <= db.TableCount; i++)
            {
                var lvi = new ListViewItem($"Table {i}");
                lvi.Tag = i;
                bool varMi = false;
                foreach (Order order in db.ActiveOrders)
                {
                    if (order.TableNo == i)
                    {
                        varMi = true;
                        break;
                    }
                }
                lvi.ImageKey = varMi ? "full" : "empty";
                //lvi.ImageKey = db.ActiveOrders.Any(x => x.TableNo == i) ? "full" : "empty" ;
                lvwTables.Items.Add(lvi);



            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void lvwTables_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lvwTables_DoubleClick(object sender, EventArgs e)
        {
            var selectedLvi = lvwTables.SelectedItems[0];
            int tableNo = (int)selectedLvi.Tag;


            var order = db.ActiveOrders.FirstOrDefault(x => x.TableNo == tableNo);
            if (order == null)
            {
                order = new Order() { TableNo = tableNo };
                db.ActiveOrders.Add(order);
                selectedLvi.ImageKey = "full";
            }

            var frmOrder = new OrderForm(db, order);
            frmOrder.TableMoving += FrmOrder_TableMoving;
            var dr = frmOrder.ShowDialog();

            if(dr == DialogResult.OK)
            {
                selectedLvi.ImageKey = "empty";
            }


            //MessageBox.Show(tableNo.ToString());
            //var frmOrder = new OrderForm();
            //frmOrder.ShowDialog();



        }

        private void FrmOrder_TableMoving(int oldTableNo, int newTableNo)
        {
            foreach (ListViewItem lvi in lvwTables.Items)
            {
                int tableNo = (int)lvi.Tag;

                if (tableNo == oldTableNo)
                {
                    lvi.ImageKey = "empty";
                }
                else if (tableNo == newTableNo)
                {
                    lvi.ImageKey = "full";
                }
            }
        }


        private void tsmiOrderHistory_Click(object sender, EventArgs e)
        {
            new OrderHistoryForm(db).ShowDialog();
        }

        private void msTop_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void tsmiProducts_Click(object sender, EventArgs e)
        {
            new ProductForm(db).ShowDialog();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            string json = JsonSerializer.Serialize(db);
            File.WriteAllText("data.json", json);
        }
    }
}