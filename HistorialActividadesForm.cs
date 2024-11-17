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
using static System.Net.Mime.MediaTypeNames;

namespace Gestion_Compras
{
    public partial class HistorialActividadesForm : Form
    {
        conexion conn = new conexion();

        public HistorialActividadesForm()
        {
            InitializeComponent();
        }


        //clase filtros
        public class FiltrosHistorial
        {
            public string FiltroCliente { get; set; }
            public string FiltroProducto { get; set; }
            public string FiltroPrecio { get; set; }
            public string FiltroApellido { get; set; }
        }

        //Método para cargar el historial, usando varias tablas con inner join 
        //donde se pueda visualizar y filtrar por varios campos
        public void CargarHistorial(FiltrosHistorial filtros)
        {
            try
            {
                using (SqlConnection connection = conn.ConexionServer())
                {
                    connection.Open();
                    string query = "SELECT HA.*, U.Nombres AS Nombre, U.Apellidos AS Apellido, P.Producto, HA.PrecioTotal " +
               "FROM HistorialActividades2 HA " +
               "JOIN Usuarios U ON HA.IdUsuarios = U.IdUsuarios " +
               "LEFT JOIN Productos P ON HA.Producto = P.Producto " +
               "WHERE 1=1";


                    // Agregar condiciones según los filtros ingresados
                    if (!string.IsNullOrEmpty(filtros.FiltroApellido))
                        query += " AND U.Apellidos LIKE @filtroApellido";
                    if (!string.IsNullOrEmpty(filtros.FiltroCliente))
                        query += " AND (U.Nombres LIKE @filtroCliente OR U.Apellidos LIKE @filtroCliente)";
                    if (!string.IsNullOrEmpty(filtros.FiltroProducto))
                        query += " AND (P.Producto LIKE @filtroProductoNombre OR HA.Accion LIKE @filtroProductoNombre)";
                    if (!string.IsNullOrEmpty(filtros.FiltroPrecio))
                        query += " AND HA.PrecioTotal LIKE @filtroPrecio";


                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        // Asignar valores a los parámetros de filtro
                        if (!string.IsNullOrEmpty(filtros.FiltroApellido))
                            cmd.Parameters.AddWithValue("@filtroApellido", "%" + filtros.FiltroApellido + "%");
                        if (!string.IsNullOrEmpty(filtros.FiltroCliente))
                            cmd.Parameters.AddWithValue("@filtroCliente", "%" + filtros.FiltroCliente + "%");
                        if (!string.IsNullOrEmpty(filtros.FiltroProducto))
                            cmd.Parameters.AddWithValue("@filtroProductoNombre", "%" + filtros.FiltroProducto + "%");
                        if (!string.IsNullOrEmpty(filtros.FiltroPrecio))
                            cmd.Parameters.AddWithValue("@filtroPrecio", "%" + filtros.FiltroPrecio + "%");


                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        // Asignar el DataTable al DataGridView
                        dgvHistorial.DataSource = dt;

                        if (dt.Rows.Count == 0)
                        {
                            MessageBox.Show("No se encontraron registros.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el historial: " + ex.Message);
            }
        }



        // Evento Load del formulario
        private void HistorialActividadesForm_Load(object sender, EventArgs e)
        {
     

            // Cargar el historial sin filtros
            FiltrosHistorial filtros = new FiltrosHistorial
            {
                FiltroCliente = null,
                FiltroProducto = null,
                FiltroPrecio = null,
                FiltroApellido = null
            };

            CargarHistorial(filtros);
        }

        private void btnAplicarFiltros_Click(object sender, EventArgs e)
        {
            FiltrosHistorial filtros = new FiltrosHistorial
            {
                FiltroCliente = txtCliente.Text,
                FiltroProducto = txtProducto.Text, // Utiliza el nuevo TextBox
                FiltroPrecio = txtPrecio.Text,
                FiltroApellido = txtApellido.Text
            };

            CargarHistorial(filtros);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            AdministradorForms forms = new AdministradorForms();
            forms.Show();
            this.Hide();
        }
        private void LimpiarCampos()
        {
            txtProducto.Clear();
            txtApellido.Clear();
            txtCliente.Clear();
            txtPrecio.Clear();
           
        }
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }
    }
}