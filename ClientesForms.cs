using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Gestion_Compras
{
    public partial class ClientesForms : Form
    {

        private conexion conn = new conexion();
        public ClientesForms()
        {
            InitializeComponent();
        }

       

       

        //añadimos el metodo al forms para que lo cargue automáticamente
        private void ClientesForms_Load(object sender, EventArgs e)
        {
            CargarUsuarios();
        }

        //modificamos el datagridview para que muestre los datos que solicitamos en una tabla
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        { // Verifica que la fila seleccionada sea válida (evita encabezado o clics fuera de la tabla)
            if (e.RowIndex >= 0)
            {
                // Obtiene la fila seleccionada
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Carga los valores de las celdas en los TextBox correspondientes
                textId.Text = row.Cells["IdUsuarios"].Value.ToString();
                textNombres.Text = row.Cells["Nombres"].Value.ToString();
                textApellidos.Text = row.Cells["Apellidos"].Value.ToString();
                textTelefono.Text = row.Cells["Telefono"].Value.ToString();
                textDireccion.Text = row.Cells["Direccion"].Value.ToString();
                textDepartamento.Text = row.Cells["Departamento"].Value.ToString();
            }
        }
        //botón que realiza toda la lógica para ingresar clientes y guardar la acción en el historial
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            using (SqlConnection conexion = conn.ConexionServer())
            {
                conexion.Open();

                // Insertar usuario en la tabla Usuarios
                string queryUsuario = "INSERT INTO Usuarios (NombreUsuario,Contrasena,Nombres,  apellidos, telefono, direccion, departamento, ciudad, rolId) VALUES (@nombre , @contrasena,@Nombres,  @apellidos, @telefono, @direccion, @departamento, @ciudad, @rolId); SELECT SCOPE_IDENTITY();";
                SqlCommand cmdUsuario = new SqlCommand(queryUsuario, conexion);
                cmdUsuario.Parameters.AddWithValue("@nombre", textUsuairo.Text);
                cmdUsuario.Parameters.AddWithValue ("@contrasena", textContra.Text);
                cmdUsuario.Parameters.AddWithValue("@Nombres", textNombres.Text);
                cmdUsuario.Parameters.AddWithValue("@apellidos", textApellidos.Text);
                cmdUsuario.Parameters.AddWithValue("@telefono", textTelefono.Text);
                cmdUsuario.Parameters.AddWithValue("@direccion", textDireccion.Text);
                cmdUsuario.Parameters.AddWithValue("@departamento", textDepartamento.Text);
                cmdUsuario.Parameters.AddWithValue("@ciudad", textCiudad.Text);
                //asignamos y quemamos el rol del usuario porque aqui se ingresan clientes y para identificarlo el id es 1
                cmdUsuario.Parameters.AddWithValue("@rolId", 1); // Asignar rolId = 1
                int idUsuario = Convert.ToInt32(cmdUsuario.ExecuteScalar());

                // Insertar registro en la tabla HistorialActividades2
                string queryHistorial = "INSERT INTO HistorialActividades2 (idUsuarios, Accion, FechaActividad) VALUES (@idUsuario, @accion, @fechaActividad)";
                using (SqlCommand cmdHistorial = new SqlCommand(queryHistorial, conexion))
                {
                    cmdHistorial.Parameters.AddWithValue("@idUsuario", idUsuario);
                    cmdHistorial.Parameters.AddWithValue("@accion", "Agregar usuario");
                    cmdHistorial.Parameters.AddWithValue("@fechaActividad", DateTime.Now);
                    cmdHistorial.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Usuario agregado exitosamente");
            CargarUsuarios(); // Actualiza la lista de usuarios
        }

        //refresh a los usuarios
        private void CargarUsuarios()
        {
            using (SqlConnection conexion = conn.ConexionServer())
            {
                conexion.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Usuarios", conexion);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
            }
        }


        //en caso no se actualice solo, está este botón de emergencia 
        private void btnRefrescar_Click(object sender, EventArgs e)
        {
            CargarUsuarios();
        }

        //botón que actualiza cualquier dato del cliente
        private void btnActualizar_Click(object sender, EventArgs e)
        {
            using (SqlConnection conexion = conn.ConexionServer())
            {
                conexion.Open();
                string query = "UPDATE Usuarios SET NombreUsuario = @nombreUsuario, Contrasena=@contrasena, Nombres = @nombre, Apellidos = @apellidos, Telefono = @telefono, Direccion = @direccion, Departamento = @departamento, Ciudad = @ciudad WHERE IdUsuarios = @id";
                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {

                    cmd.Parameters.AddWithValue("@id", int.Parse(textId.Text));
                    cmd.Parameters.AddWithValue("@nombreUsuario", textUsuairo.Text);
                    cmd.Parameters.AddWithValue("@contrasena", textContra.Text);
                    cmd.Parameters.AddWithValue("@nombre", textNombres.Text);
                    cmd.Parameters.AddWithValue("@apellidos", textApellidos.Text);
                    cmd.Parameters.AddWithValue("@telefono", textTelefono.Text);
                    cmd.Parameters.AddWithValue("@direccion", textDireccion.Text);
                    cmd.Parameters.AddWithValue("@departamento", textDepartamento.Text);
                    cmd.Parameters.AddWithValue("@ciudad", textCiudad.Text);
                    cmd.ExecuteNonQuery();

                }
            }
            MessageBox.Show("Usuario actualizado correctamente");
            CargarUsuarios(); // Cambio de CargarClientes a CargarUsuarios
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verifica que la fila seleccionada sea válida (evita encabezado o clics fuera de la tabla)
            if (e.RowIndex >= 0)
            {
                // Obtiene la fila seleccionada
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Carga los valores de las celdas en los TextBox correspondientes
                textId.Text = row.Cells["IdUsuarios"].Value.ToString();
                textUsuairo.Text = row.Cells["NombreUsuario"].Value.ToString();
                textNombres.Text = row.Cells["Nombres"].Value.ToString();
                textContra.Text = row.Cells["Contrasena"].Value.ToString();
                textApellidos.Text = row.Cells["Apellidos"].Value.ToString();
                textTelefono.Text = row.Cells["Telefono"].Value.ToString();
                textDireccion.Text = row.Cells["Direccion"].Value.ToString();
                textDepartamento.Text = row.Cells["Departamento"].Value.ToString();
                textCiudad.Text = row.Cells["Ciudad"].Value.ToString();
            }
        }
        //elimina a un cliente según su id
        private void btnEliminar_Click_1(object sender, EventArgs e)
        {
            using (SqlConnection conexion = conn.ConexionServer())
            {
                conexion.Open();

                // Eliminar los registros relacionados en HistorialActividades2 antes de eliminar el usuario
                string deleteRelatedQuery = "DELETE FROM HistorialActividades2 WHERE idUsuarios = @id;";
                using (SqlCommand deleteRelatedCmd = new SqlCommand(deleteRelatedQuery, conexion))
                {
                    deleteRelatedCmd.Parameters.AddWithValue("@id", int.Parse(textId.Text));
                    deleteRelatedCmd.ExecuteNonQuery();
                }

                // Eliminar el usuario
                string deleteUserQuery = "DELETE FROM Usuarios WHERE idUsuarios = @id;";
                using (SqlCommand deleteUserCmd = new SqlCommand(deleteUserQuery, conexion))
                {
                    deleteUserCmd.Parameters.AddWithValue("@id", int.Parse(textId.Text));
                    deleteUserCmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Usuario eliminado exitosamente");
            CargarUsuarios();
        }



        private void btnRegresar_Click(object sender, EventArgs e)
        {
            AdministradorForms admin = new AdministradorForms();
            admin.Show();
            this.Hide();
        }

        private void LimpiarCampos()
        {
            textId.Clear();
            textNombres.Clear();
            textContra.Clear();
            textApellidos.Clear();
            textDireccion.Clear();
            textCiudad.Clear();
            textDepartamento.Clear();
            textTelefono.Clear();
            textUsuairo.Clear();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }
    }
}
