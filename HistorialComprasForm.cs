using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using iTextSharp.text.factories;
using System.IO;





namespace Gestion_Compras
{

    public partial class HistorialComprasForm : Form
    {
        conexion conn = new conexion();

        private int idClienteActual;
        public HistorialComprasForm(int idCliente)
        {
            InitializeComponent();
            idClienteActual = idCliente;

            
        }
        //carga y muestra el historial de compras
        public void CargarHistorialCompras(int idCliente)
        {
            using (SqlConnection connection = conn.ConexionServer())
            {
                connection.Open();
                string query = "SELECT h.idCompra, h.FechaCompra, p.Producto, h.cantidad, h.PrecioTotal " +
                               "FROM HistorialCompras2 h " +
                               "INNER JOIN Productos p ON h.idProductos = p.idProductos " +
                               "WHERE h.IdUsuarios = @idUsuarios";

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@idUsuarios", idCliente);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvHistorial.DataSource = dt;
            }
        }


        private void HistorialComprasForm_Load(object sender, EventArgs e)
        {
           
        }

        private void btnRegresar_Click(object sender, EventArgs e)
        {
            Form1 frm = new Form1();
            frm.Show();
            this.Hide();
        }

        private void GenerarFacturaPDF(DataTable dt)
        {
            try
            {
                // Crear el documento PDF
                Document document = new Document(PageSize.A4, 50f, 50f, 50f, 50f);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream("Factura.pdf", FileMode.Create));

                // Abrir el documento
                document.Open();

                // Agregar el encabezado de la factura
                Font fontHeader = FontFactory.GetFont(FontFactory.HELVETICA, 18f, 1);
                Paragraph header = new Paragraph("Factura de Compras", fontHeader);
                header.Alignment = Element.ALIGN_CENTER;
                document.Add(header);
                document.Add(new Paragraph(" "));

                // Crear la tabla con los datos de las compras
                PdfPTable table = new PdfPTable(dt.Columns.Count);
                table.WidthPercentage = 100;

                // Agregar los encabezados de la tabla
                foreach (DataColumn column in dt.Columns)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(column.ColumnName));
                    cell.BackgroundColor = new BaseColor(240, 240, 240);
                    table.AddCell(cell);
                }

                // Agregar los datos de las compras a la tabla
                foreach (DataRow row in dt.Rows)
                {
                    foreach (object item in row.ItemArray)
                    {
                        table.AddCell(new Phrase(item.ToString()));
                    }
                }

                // Agregar la tabla al documento
                document.Add(table);

                // Cerrar el documento
                document.Close();

                // Abrir el archivo PDF
                System.Diagnostics.Process.Start("Factura.pdf");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el reporte PDF: " + ex.Message);
            }
        }



        private void GenerarReportePDF(int idCliente)
        {
            using (SqlConnection connection = conn.ConexionServer())
            {
                connection.Open();
                string query = "SELECT h.idCompra, h.FechaCompra, p.Producto, h.cantidad, h.PrecioTotal " +
                               "FROM HistorialCompras2 h " +
                               "INNER JOIN Productos p ON h.idProductos = p.idProductos " +
                               "WHERE h.idUsuarios = @idCliente";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        // Generar el reporte en formato PDF
                        GenerarFacturaPDF(dt);
                    }
                    else
                    {
                        MessageBox.Show("No hay compras registradas para este cliente.");
                    }
                }
            }
        }

        private void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            GenerarReportePDF(idClienteActual);
        }
    }
}
