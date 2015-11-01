using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WordAndGrid
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeDataGrid();
        }

        private void InitializeDataGrid()
        {
            //All Initialization stuff
            dataGridView1.DefaultCellStyle.SelectionBackColor = dataGridView1.DefaultCellStyle.BackColor;
            dataGridView1.DefaultCellStyle.SelectionForeColor = dataGridView1.DefaultCellStyle.ForeColor;

            dataGridView1.ColumnHeadersVisible = false;
            dataGridView1.RowHeadersVisible = false;
            for(int i=0; i<15; i++)
            {
                dataGridView1.Columns.Add("Col" + i.ToString(), "");
            }
            dataGridView1.Rows.Add(15);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

        private void FillGridWithRandomLetters()
        {
            //Fill up the grid's empty cells with random alphabets
            Random rn = new Random();
            char[] allLetters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                for(int i=0; i< dataGridView1.ColumnCount; i++)
                {
                    if(row.Cells[i].Value == null)
                    {
                        int index = rn.Next(26);
                        row.Cells[i].Value = allLetters[index];
                        dataGridView1.Refresh();
                    }
                }
            }
            //No interactivity and read-only grid
            dataGridView1.Enabled = false;
            lblStatus.ForeColor = Color.Green;
            lblStatus.Text = "All Done";
        }


        private void placeWordHoriz(string word, int row, int col)
        {
            //figure out if there are sufficient spots available
            int wordLength = word.ToCharArray().Length;
            int numOfSpots = dataGridView1.ColumnCount - col;
            if(wordLength > numOfSpots)
            {
                //word is too long
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = "Word is too long";
                return;
            }
            //place the word horizontally (left to right)
            char[] wArray = word.ToCharArray();
            for(int i=0; i<wArray.Length; i++)
            {
                dataGridView1.Rows[row].Cells[col+i].Value = wArray[i];
                dataGridView1.Rows[row].Cells[col + i].Style.ForeColor = Color.Red;
                dataGridView1.Rows[row].Cells[col + i].Style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                dataGridView1.Refresh();
            }
        }

        private void placeWordVert(string word, int row, int col)
        {
            //figure out if there are sufficient spots available
            int wordLength = word.ToCharArray().Length;
            int numOfSpots = dataGridView1.RowCount - row;
            if (wordLength > numOfSpots)
            {
                //word is too long
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = "Word is too long";
                return;
            }
            //place the word vertically (top to bottom)
            char[] wArray = word.ToCharArray();
            for (int i = 0; i < wArray.Length; i++)
            {
                dataGridView1.Rows[row+i].Cells[col].Value = wArray[i];
                dataGridView1.Rows[row + i].Cells[col].Style.ForeColor = Color.Red;
                dataGridView1.Rows[row + i].Cells[col].Style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                dataGridView1.Refresh();
            }
        }

        
        private void AddWord_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "";
            //No validations here. Just grab the inputs
            string word = textBox1.Text;
            int align = ddlAlign.SelectedIndex;
            int x = int.Parse(txtX.Text);
            int y = int.Parse(txtY.Text);
            if(align == 1)
            {
                //Vertically alligned
                placeWordVert(word, y, x);
            }
            else
            {
                //Horizontally alligned
                placeWordHoriz(word, y, x);
            }
            textBox1.Text = "";
            ddlAlign.SelectedIndex = -1;
            txtX.Text = "";
            txtY.Text = "";
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            FillGridWithRandomLetters();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            //Re-initialize all the controls
            lblStatus.Text = "";
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.ClearSelection();
            InitializeDataGrid();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            //Dump all the datagrid's content to a datatable
            DataTable dt = new DataTable();
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                dt.Columns.Add();
            }
            object[] cellValues = new object[dataGridView1.Columns.Count];
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    cellValues[i] = row.Cells[i].Value;
                }
                dt.Rows.Add(cellValues);
            }
            //Export the datatable
            StringBuilder sb = new StringBuilder();
            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));
            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }
            File.WriteAllText("WordSearch.csv", sb.ToString());
        }

    }
}
