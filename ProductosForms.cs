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
    public partial class ProductosForms : Form
    {

        conexion conn = new conexion();
        public ProductosForms()
        {
            InitializeComponent();
        }
        //carga los productos directamente en el form
        private void ProductosForms_Load(object sender, EventArgs e)
        {
            CargarProducto();
        }

        //método donde se pueden visualizar la tabla de productos
        private void CargarProducto()
        {
            using (SqlConnection conexion = conn.ConexionServer()) { 
            
                conexion.Open();
                string query = "SELECT * FROM Productos";

                SqlDataAdapter adapter = new SqlDataAdapter(query, conexion);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvProductos.DataSource = dt;

            }
        }

        //metodo para limpiar los textbox
        private void LimpiarCampos()
        {
            textId.Clear();
            textProducto.Clear();
            textDescripcion.Clear();
            textStock.Clear();
            textPrecioVenta.Clear();
            textFecha.Clear();
        }

        //muestra los datos de la tabla en el datagridview
        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvProductos.Rows[e.RowIndex];
                textId.Text = row.Cells["idProductos"].Value.ToString();
                textProducto.Text = row.Cells["Producto"].Value.ToString();
                textDescripcion.Text = row.Cells["descripcion"].Value.ToString();
                textStock.Text = row.Cells["cantStock"].Value.ToString();
                textPrecioVenta.Text = row.Cells["Precio_venta"].Value.ToString();
                textFecha.Text = Convert.ToDateTime(row.Cells["fecha"].Value).ToString("yyyy-MM-dd");
            }
        }

        //botón donde agrega productos el admin
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            using(SqlConnection conexion = conn.ConexionServer())
            {
                conexion.Open();
                string query = "INSERT INTO Productos (Producto, descripcion, cantStock, Precio_venta, fecha) VALUES (@producto, @descripcion, @cantstock, @precio_venta, @fecha)";

                using (SqlCommand cmd = new SqlCommand(query, conexion)) {
                    cmd.Parameters.AddWithValue("@producto", textProducto.Text);
                    cmd.Parameters.AddWithValue("@descripcion", textDescripcion.Text);
                    cmd.Parameters.AddWithValue("@cantstock", int.Parse(textStock.Text));
                    cmd.Parameters.AddWithValue("@precio_venta", decimal.Parse(textPrecioVenta.Text));
                    cmd.Parameters.AddWithValue("@fecha", DateTime.Parse(textFecha.Text) );
                    cmd.ExecuteNonQuery();
                }
            }
            CargarProducto();
            LimpiarCampos();
        }
        //botón donde se actualizan los productos
        private void btnActualizar_Click(object sender, EventArgs e)
        {
            using(SqlConnection conexion = conn.ConexionServer())
            {
                conexion.Open();
                string query = "UPDATE Productos SET Producto = @Producto, descripcion = @Descripcion, cantStock = @CantStock, Precio_venta = @PrecioVenta, fecha = @Fecha WHERE idProductos = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Id", int.Parse(textId.Text));
                    cmd.Parameters.AddWithValue("@Producto", textProducto.Text);
                    cmd.Parameters.AddWithValue("@Descripcion", textDescripcion.Text);
                    cmd.Parameters.AddWithValue("@CantStock", int.Parse(textStock.Text));
                    cmd.Parameters.AddWithValue("@PrecioVenta", decimal.Parse(textPrecioVenta.Text));
                    cmd.Parameters.AddWithValue("@Fecha", DateTime.Parse(textFecha.Text));
                    cmd.ExecuteNonQuery();
                }
            }
            CargarProducto();
        LimpiarCampos();
        }

        //botón donde se eliminan los productos
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            using (SqlConnection conexion = conn.ConexionServer())
            {
                conexion.Open();

                // Eliminar los registros relacionados en HistorialCompras2 antes de eliminar el producto
                string deleteRelatedQuery = "DELETE FROM HistorialCompras2 WHERE idProductos = @id;";
                using (SqlCommand deleteRelatedCmd = new SqlCommand(deleteRelatedQuery, conexion))
                {
                    deleteRelatedCmd.Parameters.AddWithValue("@id", int.Parse(textId.Text));
                    deleteRelatedCmd.ExecuteNonQuery();
                }

                // Eliminar el producto
                string deleteProductQuery = "DELETE FROM Productos WHERE idProductos = @id;";
                using (SqlCommand deleteProductCmd = new SqlCommand(deleteProductQuery, conexion))
                {
                    deleteProductCmd.Parameters.AddWithValue("@id", int.Parse(textId.Text));
                    deleteProductCmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Producto eliminado exitosamente");
            CargarProducto();
            LimpiarCampos();
        }
        //limpia los campos
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void btnRegresar_Click(object sender, EventArgs e)
        {
            AdministradorForms forms = new AdministradorForms();
            forms.Show();
            this.Hide();
        }
    }
}
