using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Gestion_Compras
{
    internal class conexion
    {
        public SqlConnection ConexionServer()
        {

            SqlConnection connect;
            //aqui hacemos el string que llevará la conexion y los datos necesarios para hacerla
            //hace el connection y mira si puede conectarlo, sino, manda un error
            try
            {
                string conexion = "Server=Wilfredo\\SQLEXPRESS; Database=GestionCompras;User Id=sa; Password=Paradise30#;";
                connect = new SqlConnection(conexion);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Error al conectar", e);
            }
            return connect;
        }

        //método para obtener un usuario y poder verificar su rol
        public Usuario ObtenerUsuario(string nombreUsuario, string contrasena)
        {
            Usuario usuario = null;

            using (SqlConnection conexion = ConexionServer())
            {
                conexion.Open();

                string query = "SELECT u.idUsuarios, u.NombreUsuario, r.NombreRol, u.RolId " +
                               "FROM Usuarios u INNER JOIN Roles r ON u.RolId = r.idRoles " +
                               "WHERE u.NombreUsuario = @nombreUsuario AND u.Contrasena = @contrasena";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@nombreUsuario", nombreUsuario.Trim());
                    cmd.Parameters.AddWithValue("@contrasena", contrasena.Trim());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                Id = reader.GetInt32(0),
                                NombreUsuario = reader.GetString(1),
                                NombreRol = reader.GetString(2),
                                RolId = reader.GetInt32(3) // Agrega el RolId
                            };
                            MessageBox.Show("Usuario encontrado: " + usuario.NombreUsuario + " con rol: " + usuario.NombreRol);
                        }
                        else
                        {
                            MessageBox.Show("Usuario o contraseña incorrectos.");
                        }
                    }
                }
            }
            return usuario;
        }

        //metodo que registra la compra del cliente
        public void RegistrarCompra(int idCliente, int idProducto, int cantidad, decimal precioTotal)
        {

            using (SqlConnection connection = ConexionServer()) {
            
                connection.Open();
                string query = "INSERT INTO HistorialCompras (idClientes, idProductos, cantidad, PrecioTotal)"+
                    "VALUES (@idCliente, @idProducto, @cantidad, @precioTotal)";
                using (SqlCommand cmd = new SqlCommand(query, connection)) {

                    cmd.Parameters.AddWithValue("@idCliente", idCliente);
                    cmd.Parameters.AddWithValue("@idProducto", idProducto);
                    cmd.Parameters.AddWithValue("@cantidad", cantidad);
                    cmd.Parameters.AddWithValue("@precioTotal", precioTotal);
                    cmd.ExecuteNonQuery();

                }

            }

        }

    }
    //clase que define los datos del usuario
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreRol { get; set; }

        public int RolId { get; set; }
    }

}
