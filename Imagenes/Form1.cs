using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Imagenes
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofdSelecionar = new OpenFileDialog();
            ofdSelecionar.Filter = "Imagenes|*.jpg; *.png";
            ofdSelecionar.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            ofdSelecionar.Title = "Seleccinar imagen";

            if (ofdSelecionar.ShowDialog() == DialogResult.OK)
            {
                pbImagen.Image = Image.FromFile(ofdSelecionar.FileName);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            //combersion de una imagen a bite

            MemoryStream ms = new MemoryStream();
            pbImagen.Image.Save(ms, ImageFormat.Jpeg);
            byte[] aByte = ms.ToArray();

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();

            try
            {
                MySqlCommand comando = new MySqlCommand("INSERT INTO fotos (nombre, imagen) VALUES ('"+txtNombre.Text+"', @imagen)", conexionBD);
                comando.Parameters.AddWithValue("imagen", aByte);
                comando.ExecuteNonQuery();
                MessageBox.Show("Imagen Guardada");
                pbImagen.Image = null;
            }
            catch (MySqlException ex)
            {

                MessageBox.Show("Error al guardar" + ex.Message);
            }
            
        }

        private void btnCargar_Click(object sender, EventArgs e)
        {
            int id = int.Parse(txtId.Text);
            string sql = "SELECT nombre, imagen FROM fotos WHERE id='" + id + "'";

            MySqlConnection conexionBD = Conexion.conexion();
            conexionBD.Open();

            try
            {
                MySqlCommand comando = new MySqlCommand(sql, conexionBD);
                MySqlDataReader reader = comando.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    MemoryStream ms = new MemoryStream((byte[])reader["imagen"]);
                    Bitmap bm = new Bitmap(ms);
                    pbImagen.Image = bm;
                    txtNombre.Text = reader["nombre"].ToString();
                }
                else
                {
                    MessageBox.Show("No se encontraron Registros");
                }
               
            }
            catch (MySqlException ex)
            {

                MessageBox.Show("Error al buscar" + ex.Message);
            }
            
        }
    }
}
