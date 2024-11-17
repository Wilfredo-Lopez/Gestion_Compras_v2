using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gestion_Compras
{
    public partial class AdministradorForms : Form
    {
        public AdministradorForms()
        {
            InitializeComponent();
        }
        //botón que redirije a la sección de clientes y puede hacer el crud de ellos
        private void button1_Click(object sender, EventArgs e)
        {
            ClientesForms clientes = new ClientesForms();
            clientes.Show();
            this.Hide();
        }
        //botón que redirije al form de productos y puede hacer el crud de ellos
        private void btnProductos_Click(object sender, EventArgs e)
        {
            ProductosForms productos = new ProductosForms();
            productos.Show();
            this.Hide();
        }

        private void AdministradorForms_Load(object sender, EventArgs e)
        {

        }
        //botón que redirije al form de historial donde solo puede ver los registros de compras y acciones y uso de filtros
        private void btnHistorialActividades_Click(object sender, EventArgs e)
        {
            HistorialActividadesForm form = new HistorialActividadesForm();
            form.Show();
        }

        private void btnRegresar_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            form.Show();
            this.Hide();
        }

        private void btnPedidos_Click(object sender, EventArgs e)
        {
            Pedidos pedidos = new Pedidos();
            pedidos.Show();
            this.Hide();
        }
    }
}
