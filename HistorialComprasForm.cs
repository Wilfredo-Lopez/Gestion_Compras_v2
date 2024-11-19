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

    public partial class HistorialComprasForm : Form
    {
        conexion conn = new conexion();
        public HistorialComprasForm()
        {
            InitializeComponent();
            
        }
        //carga y muestra el historial de compras
        public void CargarHistorialCompras(int idCliente)
        {
            using (SqlConnection connection = conn.ConexionServer())
            {
                connection.Open();
                string query = "SELECT h.idCompra, h.FechaCompra, p.Producto, h.cantidad, h.PrecioTotal " +
                               "FROM HistorialCompras2 h " +
                               "INNER JOIN Productos p ON h.idProductos = p.idProductos " +
                               "WHERE h.IdUsuarios = @idUsuarios";

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@idUsuarios", idCliente);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvHistorial.DataSource = dt;
            }
        }


        private void HistorialComprasForm_Load(object sender, EventArgs e)
        {
           
        }

        private void btnRegresar_Click(object sender, EventArgs e)
        {
            Form1 frm = new Form1();
            frm.Show();
            this.Hide();
        }
    }
}
