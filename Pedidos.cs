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
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                textId.Text = selectedRow.Cells["idPedido"].Value.ToString();
                dtpFechaPedido.Value = (DateTime)selectedRow.Cells["fechaPedido"].Value;
                dtpFechaEntrega.Value = (DateTime)selectedRow.Cells["fechaEntrega"].Value;
                textComentarios.Text = selectedRow.Cells["comentarios"].Value.ToString();
                txtPrecioUnidad.Text = selectedRow.Cells["precio_unidad"].Value.ToString();
                txtProductos.Text = selectedRow.Cells["idProductos"].Value.ToString();
                txtDuenoPedido.Text = selectedRow.Cells["dueñoPedido"].Value.ToString();
                txtClientes.Text = selectedRow.Cells["idClientes"].Value.ToString();
                txtCategoria.Text = selectedRow.Cells["categoria"].Value.ToString();
            }

        }

        private void btnRegresar_Click(object sender, EventArgs e)
        {
            AdministradorForms administradorForms = new AdministradorForms();
            administradorForms.Show();
            this.Hide();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = connexion.ConexionServer())
            {
                conn.Open();
                string query = "INSERT INTO Pedidos (idPedido, fechaPedido, fechaEntrega, comentarios, precio_unidad, idProductos, duenoPedido, idClientes, categoria) " +
                              "VALUES (@idPedido, @fechaPedido, @fechaEntrega, @comentarios, @precio_unidad, @idProductos, @duenoPedido, @idClientes, @categoria)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idPedido", textId.Text);
                cmd.Parameters.AddWithValue("@fechaPedido", dtpFechaPedido.Value);
                cmd.Parameters.AddWithValue("@fechaEntrega", dtpFechaEntrega.Value);
                cmd.Parameters.AddWithValue("@comentarios", textComentarios.Text);

                if (decimal.TryParse(txtPrecioUnidad.Text, out decimal precioUnidad))
                {
                    cmd.Parameters.AddWithValue("@precio_unidad", precioUnidad);
                }
                else
                {
                    MessageBox.Show("Por favor, ingrese un valor numérico válido para el precio unitario.");
                    return;
                }

                cmd.Parameters.AddWithValue("@idProductos", txtProductos.Text);
                cmd.Parameters.AddWithValue("@duenoPedido", txtDuenoPedido.Text);
                cmd.Parameters.AddWithValue("@idClientes", txtClientes.Text);
                cmd.Parameters.AddWithValue("@categoria", txtCategoria.Text);
                cmd.ExecuteNonQuery();
            }
            CargarProductos();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                string idPedido = selectedRow.Cells["idPedido"].Value.ToString();

                using (SqlConnection conn = connexion.ConexionServer())
                {
                    conn.Open();
                    string query = "DELETE FROM Pedidos WHERE idPedido = @idPedido";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@idPedido", idPedido);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Pedido eliminado correctamente.");
                        CargarProductos();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar el pedido.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccione un pedido para eliminar.");
            }
        
    }
    }
}
