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
    public partial class Form1 : Form
    {
        conexion conn = new conexion();
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
           
        }



        private void LoginButton_Click(object sender, EventArgs e)
        {
            // Validar que los campos no estén vacíos
            if (string.IsNullOrWhiteSpace(username.Text) || string.IsNullOrWhiteSpace(password.Text))
            {
                MessageBox.Show("Por favor ingrese un usuario y contraseña.");
                return;
            }

            // Obtener los datos ingresados
            string NombreUsuario = username.Text;
            string contrasena = password.Text;

            // Buscar el usuario en la base de datos
            Usuario usuario = conn.ObtenerUsuario(NombreUsuario, contrasena);

            if (usuario != null)
            {
                // Guardar el RolId del usuario para futuras referencias
                int rolIdActual = usuario.RolId;

                // Insertar la actividad de inicio de sesión en la tabla HistorialActividades2
                using (SqlConnection connection = conn.ConexionServer())
                {
                    connection.Open();
                    string queryActividad = "INSERT INTO HistorialActividades2 (idUsuarios, Accion, FechaActividad) " +
                                            "VALUES (@idUsuario, @actividad, @fechaActividad)";
                    using (SqlCommand cmdActividad = new SqlCommand(queryActividad, connection))
                    {
                        cmdActividad.CommandTimeout = 60;
                        cmdActividad.Parameters.AddWithValue("@idUsuario", usuario.Id);
                        cmdActividad.Parameters.AddWithValue("@actividad", "Inicio de sesión");
                        cmdActividad.Parameters.AddWithValue("@fechaActividad", DateTime.Now);
                        cmdActividad.ExecuteNonQuery();
                    }
                }

                // Verificar el rol del usuario
                if (usuario.NombreRol == "Administrador")
                {
                    AdministradorForms adminForm = new AdministradorForms();
                    adminForm.Show();
                    MessageBox.Show("Bienvenido, Administrador.");
                }
                else if (usuario.NombreRol == "Cliente")
                {
                    ClienteForms clienteForms = new ClienteForms(usuario.Id, rolIdActual);
                    clienteForms.Show();
                    MessageBox.Show("Bienvenido, Cliente.");
                }
                else if (usuario.NombreRol == "Operador")
                {
                    OperadorForms oper = new OperadorForms();
                    oper.Show();
                    MessageBox.Show("Bienvenido, Operador.");
                }

                // Ocultar el formulario de inicio de sesión
                this.Hide();
            }
            else
            {
                // Mostrar mensaje de error si las credenciales son incorrectas
                MessageBox.Show("Credenciales incorrectas");
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            IntegrantesForms integrantesForms = new IntegrantesForms();
            integrantesForms.Show();
            this.Hide();
        }
        //prueba
        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
