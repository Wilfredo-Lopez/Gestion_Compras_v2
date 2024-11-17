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

namespace Gestion_Compras
{
    public partial class Pedidos : Form
    {

        conexion connexion = new conexion();
        public Pedidos()
        {
            InitializeComponent();
        }


        private void CargarProductos() {
            using (SqlConnection conn = connexion.ConexionServer()) {
                conn.Open();
                string query = "SELECT * FROM Pedidos;";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private void Pedidos_Load(object sender, EventArgs e)
        {
            CargarProductos();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void btnRegresar_Click(object sender, EventArgs e)
        {
            AdministradorForms administradorForms = new AdministradorForms();
            administradorForms.Show();
            this.Hide();
        }
    }
}
