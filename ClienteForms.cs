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
    public partial class ClienteForms : Form
    {
        conexion conn = new conexion();

        private int idClienteActual;

        private int rolIdActual;

        //constructor que recibe el id del cliente y del rol del cliente para utilizarlo dentro de los procesos
        //toma estos datos y los usa para la tabla historial e identificar usuario y su rol
        public ClienteForms(int idCliente, int rolIdActual)
        {
            InitializeComponent();
            idClienteActual = idCliente;
            CargarProductos();
            this.rolIdActual = rolIdActual;
        }

        //Método para hacer un refresh a los datos cuando se actualicen
        private void CargarProductos() {

            using (SqlConnection connection = conn.ConexionServer()) {
            
                connection.Open();
                string query = "SELECT idProductos, Producto, descripcion, cantStock, Precio_Venta FROM Productos;";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvProductos.DataSource = dt;

            }

        }

        private void ClienteForms_Load(object sender, EventArgs e)
        {
            CargarProductos();
        }

        //Muestra los datos en filas
        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) {
            
                DataGridViewRow row = dgvProductos.Rows[e.RowIndex];
                textId.Text = row.Cells["idProductos"].Value.ToString();
                textProducto.Text = row.Cells["Producto"].Value.ToString();
                textDescripcion.Text = row.Cells["descripcion"].Value.ToString();
                textStock.Text = row.Cells["cantStock"].Value.ToString();
                textPrecio.Text = row.Cells["Precio_Venta"].Value.ToString();
            }
        }
        //Realiza toda la logica de ingresar y actuaizar datos en varias tablas
        private void btnComprar_Click(object sender, EventArgs e)
        {
            int productoId = int.Parse(textId.Text);
            int stockDisponible = int.Parse(textStock.Text);
            int cantidadSolicitada;
            decimal precioUnitario = decimal.Parse(textPrecio.Text);

            if (!int.TryParse(textCantidad.Text, out cantidadSolicitada) || cantidadSolicitada <= 0)
            {
                MessageBox.Show("Ingrese una cantidad válida");
                return;
            }

            if (cantidadSolicitada > stockDisponible)
            {
                MessageBox.Show("No hay suficiente Stock disponible");
                return;
            }
            //formulas para calcular el nuevo stock y actualizarlo en la base y el precio total igual
            int nuevoStock = stockDisponible - cantidadSolicitada;
            decimal precioTotal = cantidadSolicitada * precioUnitario;

            using (SqlConnection connection = conn.ConexionServer())
            {
                connection.Open();

                // Primera transacción para actualizar el stock
                using (SqlTransaction transactionStock = connection.BeginTransaction())
                {
                    try
                    {
                        //se actualiza el stock de la tabla productos segun lo que el cliente solicite comprar
                        string queryStock = "UPDATE Productos SET cantStock = @nuevoStock WHERE idProductos = @productoId";
                        using (SqlCommand cmdStock = new SqlCommand(queryStock, connection, transactionStock))
                        {
                            cmdStock.CommandTimeout = 30;
                            cmdStock.Parameters.AddWithValue("@nuevoStock", nuevoStock);
                            cmdStock.Parameters.AddWithValue("@productoId", productoId);
                            cmdStock.ExecuteNonQuery();
                        }
                        transactionStock.Commit();
                    }
                    catch (Exception ex)
                    {
                        transactionStock.Rollback();
                        MessageBox.Show("Error al actualizar el stock: " + ex.Message);
                        return;
                    }
                }

                // Segunda transacción para insertar en el historial de compras
                using (SqlTransaction transactionHistorial = connection.BeginTransaction())
                {
                    try
                    {
                        string queryHistorial = "INSERT INTO HistorialCompras2 (idUsuarios, idProductos, Cantidad, FechaCompra, PrecioTotal) " +
                                                "VALUES (@idUsuario, @idProducto, @cantidad, @fechaCompra, @precioTotal)";
                        using (SqlCommand cmdHistorial = new SqlCommand(queryHistorial, connection, transactionHistorial))
                        {
                            //el timeout es para evitar que el sistema tarde mucho si ocurre algo
                            cmdHistorial.CommandTimeout = 30;
                            cmdHistorial.Parameters.AddWithValue("@idUsuario", idClienteActual);
                            cmdHistorial.Parameters.AddWithValue("@idProducto", productoId);
                            cmdHistorial.Parameters.AddWithValue("@cantidad", cantidadSolicitada);
                            cmdHistorial.Parameters.AddWithValue("@fechaCompra", DateTime.Now);
                            cmdHistorial.Parameters.AddWithValue("@precioTotal", precioTotal);
                            cmdHistorial.ExecuteNonQuery();
                        }
                        transactionHistorial.Commit();
                    }
                    catch (Exception ex)
                    {
                        transactionHistorial.Rollback();
                        MessageBox.Show("Error al registrar la compra en el historial: " + ex.Message);
                        return;
                    }
                }

                // Tercera transacción para insertar en el historial de actividades
                using (SqlTransaction transactionActividad = connection.BeginTransaction())
                {
                    try
                    {
                        string queryActividad = "INSERT INTO HistorialActividades2 (idUsuarios, Accion, FechaActividad, Producto, idProductos, PrecioTotal) " +
                                                "VALUES (@idUsuario, @actividad, @fechaActividad, @producto, @idProducto, @precioTotal)";
                        using (SqlCommand cmdActividad = new SqlCommand(queryActividad, connection, transactionActividad))
                        {
                            cmdActividad.CommandTimeout = 60;
                            cmdActividad.Parameters.AddWithValue("@idUsuario", idClienteActual);
                            cmdActividad.Parameters.AddWithValue("@actividad", "Compra");
                            cmdActividad.Parameters.AddWithValue("@fechaActividad", DateTime.Now);
                            cmdActividad.Parameters.AddWithValue("@producto", textProducto.Text);
                            cmdActividad.Parameters.AddWithValue("@idProducto", productoId);
                            cmdActividad.Parameters.AddWithValue("@precioTotal", precioTotal); // Agregar el precio total de la compra
                            cmdActividad.ExecuteNonQuery();
                        }
                        transactionActividad.Commit();
                    }
                    catch (Exception ex)
                    {
                        transactionActividad.Rollback();
                        MessageBox.Show("Error al registrar la actividad en el historial: " + ex.Message);
                    }
                }

                // Cuarta transacción para insertar en la tabla Pedidos
                using (SqlTransaction transactionPedido = connection.BeginTransaction())
                {
                    try
                    {
                        string queryPedido = "INSERT INTO Pedidos (fechaPedido, fechaEntrega, comentarios, precio_unidad, idProductos, dueñoPedido, idClientes, categoria) " +
                                             "VALUES (@fechaPedido, @fechaEntrega, @comentarios, @precioUnidad, @idProducto, @dueñoPedido, @idCliente, @categoria)";
                        using (SqlCommand cmdPedido = new SqlCommand(queryPedido, connection, transactionPedido))
                        {
                            // Establece el tiempo de espera máximo para la ejecución del comando SQL en 60 segundos
                            cmdPedido.CommandTimeout = 60;

                            // Asigna los valores de los parámetros de la consulta SQL
                            cmdPedido.Parameters.AddWithValue("@fechaPedido", dtpFechaPedido.Value);
                            cmdPedido.Parameters.AddWithValue("@fechaEntrega", dtpFechaEntrega.Value);
                            cmdPedido.Parameters.AddWithValue("@comentarios", textComentarios.Text);
                            cmdPedido.Parameters.AddWithValue("@precioUnidad", precioUnitario);
                            cmdPedido.Parameters.AddWithValue("@idProducto", productoId);
                            cmdPedido.Parameters.AddWithValue("@dueñoPedido", textDueñoPedido.Text);
                            cmdPedido.Parameters.AddWithValue("@idCliente", idClienteActual);
                            cmdPedido.Parameters.AddWithValue("@categoria", textCategoria.Text);

                            // Ejecuta la consulta SQL para insertar el nuevo registro en la tabla "Pedidos"
                            cmdPedido.ExecuteNonQuery();
                        }
                        //Confirma la transacción
                        transactionPedido.Commit();
                        MessageBox.Show("Pedido registrado en la tabla Pedidos");
                    }
                    catch (Exception ex)
                    {
                        //Si falla, revierte la transacción
                        transactionPedido.Rollback();
                        MessageBox.Show("Error al registrar el pedido: " + ex.Message);
                    }
                }
                CargarProductos();
            }
        }
        //Aqui logro obtener el id del cliente desde el formulario para guardarlo y utilizarlo
        private int ObtenerIdCliente()
        {

            return idClienteActual;
        }

        //Botón que redirije a el formulario de historial por cliente
        private void btnHistorial_Click(object sender, EventArgs e)
        {
            int idCliente = ObtenerIdCliente();
            HistorialComprasForm form = new HistorialComprasForm();
            form.CargarHistorialCompras(idCliente);
            form.Show();
            this.Hide();

        }
        //botón que regresa al login
        private void btnRegresar_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            form.Show();
            this.Hide();
        }

       
    }
    
    }