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
    public partial class OperadorForms : Form
    {
        // Instancia de la clase de conexión para conectarse a la base de datos
        conexion conn = new conexion();

        // Constructor de la clase OperadorForms
        public OperadorForms()
        {
            InitializeComponent();
        }

        // Método para cargar los datos de la tabla Productos en el DataGridView
        public void CargarProducto()
        {
            try
            {
                using (SqlConnection connection = conn.ConexionServer())
                {
                    connection.Open();
                    string query = "SELECT * FROM Productos";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("error: " + ex.Message);
            }
        }

        // Evento que se ejecuta al cargar el formulario, llama a CargarProducto para mostrar la lista de productos
        private void OperadorForms_Load(object sender, EventArgs e)
        {
            CargarProducto();
        }

        // Evento que se ejecuta al hacer clic en el botón Añadir Producto
        private void btnAnadirProducto_Click(object sender, EventArgs e)
        {
            // Recupera los valores de los campos de entrada
            string producto = textProducto.Text;
            string descripcion = textDescripcion.Text;
            int cantidadStock;
            if (int.TryParse(textCantStock.Text, out cantidadStock))
            {
                // La conversión a int fue exitosa, puedes usar la variable cantidadStock
            }
            else
            {
                // La conversión a int falló, maneja el error de acuerdo a tus necesidades
                MessageBox.Show("El valor ingresado en el campo de cantidad de stock no es válido. Por favor, ingrese un número entero.");
                return;
            }
            decimal precioVenta = decimal.Parse(textPrecioVenta.Text);
            DateTime fecha = dtpFecha.Value;

            // Verifica que los campos requeridos no estén vacíos y que los valores sean válidos
            if (string.IsNullOrWhiteSpace(producto) || cantidadStock <= 0 || precioVenta <= 0)
            {
                MessageBox.Show("completa todos los campos correctamente");
                return;
            }

            try
            {
                using (SqlConnection connection = conn.ConexionServer())
                {
                    connection.Open();
                    string query = "INSERT INTO Productos (Producto, descripcion, cantStock, Precio_venta, fecha)" +
                                   "VALUES (@producto, @descripcion, @cantStock, @precioVenta, @fecha)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        // Asigna los parámetros de la consulta SQL
                        cmd.Parameters.AddWithValue("@producto", producto);
                        cmd.Parameters.AddWithValue("@descripcion", descripcion);
                        cmd.Parameters.AddWithValue("@cantStock", cantidadStock);
                        cmd.Parameters.AddWithValue("@precioVenta", precioVenta);
                        cmd.Parameters.AddWithValue("@fecha", fecha);

                        // Ejecuta la consulta
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Producto añadido exitosamente");
                CargarProducto(); // Recarga la lista de productos
                LimpiarCampos();  // Limpia los campos de entrada

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Método para limpiar los campos de entrada del formulario
        private void LimpiarCampos()
        {
            textId.Clear();
            textProducto.Clear();
            textDescripcion.Clear();
            textCantStock.Clear();
            textPrecioVenta.Clear();
            dtpFecha.Value = DateTime.Now;
        }

        // Evento que se ejecuta al hacer clic en el botón Actualizar, actualiza el stock de un producto seleccionado
        private void btnActualizar_Click(object sender, EventArgs e)
        {
            // Verifica si hay un producto seleccionado en el DataGridView
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, selecciona un producto para actualizar el stock.");
                return;
            }

            // Obtiene el ID del producto seleccionado y la cantidad a añadir al stock
            int idProducto = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idProductos"].Value);
            int cantidadStock = Convert.ToInt32(textCantStock.Text);

            // Verifica que la cantidad sea válida
            if (cantidadStock <= 0)
            {
                MessageBox.Show("Por favor, ingresa una cantidad válida para añadir al stock.");
                return;
            }

            try
            {
                using (SqlConnection connection = conn.ConexionServer())
                {
                    connection.Open();
                    string query = "UPDATE Productos SET cantStock = cantStock + @cantStock WHERE idProductos = @idProducto";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        // Asigna los parámetros de la consulta SQL
                        cmd.Parameters.AddWithValue("@cantStock", cantidadStock);
                        cmd.Parameters.AddWithValue("@idProducto", idProducto);

                        // Ejecuta la consulta
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Stock actualizado exitosamente.");
                CargarProducto(); // Recarga la lista de productos
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar el stock: " + ex.Message);
            }
        }

        // Evento que se ejecuta al hacer clic en una celda del DataGridView, asigna valores a los campos de entrada
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Asegúrate de que el índice de la fila sea válido
            if (e.RowIndex >= 0)
            {
                // Obtiene la fila seleccionada y asigna los valores a los campos de entrada
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textId.Text = row.Cells["idProductos"].Value.ToString();
                textProducto.Text = row.Cells["Producto"].Value.ToString();
                textDescripcion.Text = row.Cells["descripcion"].Value.ToString();
                textCantStock.Text = row.Cells["cantStock"].Value.ToString();
                textPrecioVenta.Text = row.Cells["Precio_venta"].Value.ToString();
                dtpFecha.Value = Convert.ToDateTime(row.Cells["fecha"].Value);
            }
        }

        // Evento que se ejecuta al hacer clic en el botón Eliminar Producto
        private void btnEliminarProducto_Click(object sender, EventArgs e)
        {
            // Verifica si se ha seleccionado un producto en el DataGridView
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, selecciona un producto para eliminar.");
                return;
            }

            // Obtiene el ID del producto seleccionado
            int idProducto = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idProductos"].Value);

            // Confirma la eliminación del producto
            var confirmResult = MessageBox.Show("¿Estás seguro de que deseas eliminar este producto?",
                                                 "Confirmar eliminación",
                                                 MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = conn.ConexionServer())
                    {
                        connection.Open();
                        string query = "DELETE FROM Productos WHERE idProductos = @idProducto";
                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            // Asigna el parámetro de la consulta SQL
                            cmd.Parameters.AddWithValue("@idProducto", idProducto);

                            // Ejecuta la consulta
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Producto eliminado exitosamente.");
                    CargarProducto(); // Recarga la lista de productos
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar el producto: " + ex.Message);
                }
            }
        }

        // Evento que se ejecuta al hacer clic en el botón Regresar
        private void btnRegresar_Click(object sender, EventArgs e)
        {
            // Cierra el formulario actual y abre el formulario principal
            this.Close();
            Form1 formPrincipal = new Form1();
            formPrincipal.Show();
        }

        private void btnLimpiar_Click(Object sender, EventArgs e) {
            LimpiarCampos();
        }
    }
}
