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
    public partial class OpAd : Form
    {

        conexion conn = new conexion();
        
        private int usuarioSeleccionadoId = 0;
        public OpAd()
        {
            InitializeComponent();
            CargarRoles();
            CargarUsuarios();
        }

        // Método para cargar los roles disponibles en el ComboBox
        private void CargarRoles()
        {
            try
            {
                using (SqlConnection connection = conn.ConexionServer())
                {
                    connection.Open();
                    string query = "SELECT * FROM Roles";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader reader = cmd.ExecuteReader();

                    comboBoxRol.Items.Clear();
                    comboBoxRol.DisplayMember = "Nombre";
                    comboBoxRol.ValueMember = "Id";
                    comboBoxRol.Items.Add(new { Id = 0, Nombre = "Seleccionar rol" });

                    while (reader.Read())
                    {
                        comboBoxRol.Items.Add(new { Id = reader.GetInt32(0), Nombre = reader.GetString(1) });
                    }

                    reader.Close();
                    comboBoxRol.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los roles: " + ex.Message);
            }
        }


        // Método para cargar la lista de usuarios en el DataGridView
        private void CargarUsuarios()
        {
            try
            {
                using (SqlConnection connection = conn.ConexionServer())
                {
                    connection.Open();
                    string query = "SELECT u.IdUsuarios, u.NombreUsuario, u.Contrasena, u.RolId, u.Nombres, u.Apellidos, u.Telefono, u.Direccion, u.Departamento, u.Ciudad " +
                                  "FROM Usuarios u";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;

                    // Ajustar el ancho de las columnas
                    dataGridView1.Columns["IdUsuarios"].Width = 50;
                    dataGridView1.Columns["NombreUsuario"].Width = 150;
                    dataGridView1.Columns["Contrasena"].Width = 150;
                    dataGridView1.Columns["RolId"].Width = 150;
                    dataGridView1.Columns["Nombres"].Width = 150;
                    dataGridView1.Columns["Apellidos"].Width = 150;
                    dataGridView1.Columns["Telefono"].Width = 150;
                    dataGridView1.Columns["Direccion"].Width = 150;
                    dataGridView1.Columns["Departamento"].Width = 150;
                    dataGridView1.Columns["Ciudad"].Width = 150;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los usuarios: " + ex.Message);
            }
        }


        private void OpAd_Load(object sender, EventArgs e)
        {
            CargarRoles();
            CargarUsuarios();
        }

        private void comboBoxRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            dynamic selectedRole = comboBoxRol.SelectedItem;
            int rolId = selectedRole.Id;
        }

        private void btnGuardarUsuario_Click(object sender, EventArgs e)
        {
            // Obtener los datos del usuario
            string nombreUsuario = textUsuario.Text;
            string contrasena = textContrasena.Text;
            string nombres = textNombres.Text;
            string apellidos = textApellidos.Text;
            string telefono = textTelefono.Text;
            string direccion = textDireccion.Text;
            string departamento = textDepartamento.Text;
            string ciudad = textCiudad.Text;
            dynamic selectedRole = comboBoxRol.SelectedItem;
            int rolId = selectedRole.Id;

            // Validar que se haya seleccionado un rol
            if (rolId == 0)
            {
                MessageBox.Show("Selecciona un rol para el usuario.");
                return;
            }

            // Validar que se hayan ingresado todos los campos obligatorios
            if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(contrasena) || string.IsNullOrEmpty(nombres) || string.IsNullOrEmpty(apellidos))
            {
                MessageBox.Show("Debes ingresar todos los campos obligatorios.");
                return;
            }

            // Guardar el nuevo usuario
            GuardarUsuario(nombreUsuario, contrasena, rolId, nombres, apellidos, telefono, direccion, departamento, ciudad);

            // Limpiar los campos del formulario
            LimpiarCampos();

            // Actualizar la lista de usuarios en el DataGridView
            CargarUsuarios();
        }

        private void GuardarUsuario(string nombreUsuario, string contrasena, int rolId, string nombres, string apellidos, string telefono, string direccion, string departamento, string ciudad)
        {
            try
            {
                using (SqlConnection connection = conn.ConexionServer())
                {
                    connection.Open();
                    string query = "INSERT INTO Usuarios (NombreUsuario, Contrasena, RolId, Nombres, Apellidos, Telefono, Direccion, Departamento, Ciudad) " +
                                  "VALUES (@nombreUsuario, @contrasena, @rolId, @nombres, @apellidos, @telefono, @direccion, @departamento, @ciudad)";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);
                    cmd.Parameters.AddWithValue("@contrasena", contrasena);
                    cmd.Parameters.AddWithValue("@rolId", rolId);
                    cmd.Parameters.AddWithValue("@nombres", nombres);
                    cmd.Parameters.AddWithValue("@apellidos", apellidos);
                    cmd.Parameters.AddWithValue("@telefono", telefono);
                    cmd.Parameters.AddWithValue("@direccion", direccion);
                    cmd.Parameters.AddWithValue("@departamento", departamento);
                    cmd.Parameters.AddWithValue("@ciudad", ciudad);
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Usuario guardado exitosamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el usuario: " + ex.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                usuarioSeleccionadoId = Convert.ToInt32(row.Cells["IdUsuarios"].Value);
                textId.Text = usuarioSeleccionadoId.ToString(); // Asignar el ID al TextBox
                textUsuario.Text = row.Cells["NombreUsuario"].Value.ToString();
                textContrasena.Text = row.Cells["Contrasena"].Value.ToString();
                comboBoxRol.SelectedValue = row.Cells["RolId"].Value;
                textNombres.Text = row.Cells["Nombres"].Value.ToString();
                textApellidos.Text = row.Cells["Apellidos"].Value.ToString();
                textTelefono.Text = row.Cells["Telefono"].Value.ToString();
                textDireccion.Text = row.Cells["Direccion"].Value.ToString();
                textDepartamento.Text = row.Cells["Departamento"].Value.ToString();
                textCiudad.Text = row.Cells["Ciudad"].Value.ToString();
            }
        }

        private void ActualizarUsuario(int id, string nombreUsuario, string contrasena, int rolId, string nombres, string apellidos, string telefono, string direccion, string departamento, string ciudad)
        {
            try
            {
                using (SqlConnection connection = conn.ConexionServer())
                {
                    connection.Open();
                    string query = "UPDATE Usuarios SET NombreUsuario = @nombreUsuario, Contrasena = @contrasena, RolId = @rolId, Nombres = @nombres, Apellidos = @apellidos, Telefono = @telefono, Direccion = @direccion, Departamento = @departamento, Ciudad = @ciudad WHERE IdUsuarios = @id";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);
                    cmd.Parameters.AddWithValue("@contrasena", contrasena);
                    cmd.Parameters.AddWithValue("@rolId", rolId);
                    cmd.Parameters.AddWithValue("@nombres", nombres);
                    cmd.Parameters.AddWithValue("@apellidos", apellidos);
                    cmd.Parameters.AddWithValue("@telefono", telefono);
                    cmd.Parameters.AddWithValue("@direccion", direccion);
                    cmd.Parameters.AddWithValue("@departamento", departamento);
                    cmd.Parameters.AddWithValue("@ciudad", ciudad);
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Usuario actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar el usuario: " + ex.Message);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            // Obtener los datos del usuario
            string nombreUsuario = textUsuario.Text;
            string contrasena = textContrasena.Text;
            string nombres = textNombres.Text;
            string apellidos = textApellidos.Text;
            string telefono = textTelefono.Text;
            string direccion = textDireccion.Text;
            string departamento = textDepartamento.Text;
            string ciudad = textCiudad.Text;
            dynamic selectedRole = comboBoxRol.SelectedItem;
            int rolId = selectedRole.Id;

            // Validar que se haya seleccionado un usuario
            if (usuarioSeleccionadoId == 0)
            {
                MessageBox.Show("Selecciona un usuario para actualizar.");
                return;
            }

            // Validar que se hayan ingresado todos los campos obligatorios
            if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(contrasena) || string.IsNullOrEmpty(nombres) || string.IsNullOrEmpty(apellidos))
            {
                MessageBox.Show("Debes ingresar todos los campos obligatorios.");
                return;
            }

            // Actualizar el usuario
            ActualizarUsuario(usuarioSeleccionadoId, nombreUsuario, contrasena, rolId, nombres, apellidos, telefono, direccion, departamento, ciudad);

            // Limpiar los campos del formulario
            LimpiarCampos();

            // Actualizar la lista de usuarios en el DataGridView
            CargarUsuarios();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            // Validar que se haya seleccionado un usuario
            if (usuarioSeleccionadoId == 0)
            {
                MessageBox.Show("Selecciona un usuario para eliminar.");
                return;
            }

            // Eliminar el usuario
            EliminarUsuario(usuarioSeleccionadoId);

            // Limpiar los campos del formulario
           LimpiarCampos();

            // Actualizar la lista de usuarios en el DataGridView
            CargarUsuarios();
        }

        private void EliminarUsuario(int id)
        {
            try
            {
                using (SqlConnection connection = conn.ConexionServer())
                {
                    connection.Open();
                    string query = "DELETE FROM Usuarios WHERE IdUsuarios = @id";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Usuario eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar el usuario: " + ex.Message);
            }
        }

        private void LimpiarCampos()
        {
            textApellidos.Clear();
            textTelefono.Clear();
            textDireccion.Clear();
            textCiudad.Clear();
            textDepartamento.Clear();
            textContrasena.Clear();
            textId.Clear();
            textNombres.Clear();
            textUsuario.Clear();
        }

        private void btnRegresar_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.ShowDialog();
            this.Hide();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }
    }
}
