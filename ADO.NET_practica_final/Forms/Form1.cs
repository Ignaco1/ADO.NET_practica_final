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

namespace ADO.NET_practica_final
{
    public partial class Form1 : Form
    {
        string cadena_conexion = "Data source=(local);Initial Catalog=DB_PRACTICA;Trusted_Connection=True";

        string vari;
        
        public Form1()
        {
            InitializeComponent();

            ModoVista();
            txt_edad.Enabled = false;
        }
       
        private void Form1_Load(object sender, EventArgs e)
        {
            CargaDatos();
        }

        private void btn_cerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ModoCarga()
        {
            groupBox1.Enabled = false;
            groupBox2.Enabled = true;
        }

        private void ModoVista()
        {
            groupBox1.Enabled = true;
            groupBox2.Enabled = false;
        }

        private void Conexion_exitosa()
        {
            using (SqlConnection conexion = new SqlConnection(cadena_conexion))
            {
                try
                {
                    conexion.Open();

                    MessageBox.Show("Conexion exitosa");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("error: " + ex.Message);
                }
            }
        }

        private void btn_conexion_Click(object sender, EventArgs e)
        {
            Conexion_exitosa();
        }

        private List<Persona> GetData()
        {
            List<Persona> lista_personas = new List<Persona>();

            string query = "select * from Usuarios";

            using (SqlConnection conexion = new SqlConnection(cadena_conexion))
            {
                SqlCommand command = new SqlCommand(query, conexion);

                try
                {
                    conexion.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Persona usuario = new Persona();

                        usuario.ID = reader.GetInt32(0);
                        usuario.NOMBRE = reader.GetString(1);
                        usuario.CORREO = reader.GetString(2);
                        usuario.FECHA_DE_NACIMIENTO = reader.GetDateTime(3);
                        usuario.EDAD = reader.GetInt32(4);

                        lista_personas.Add(usuario);
                    }

                    conexion.Close();
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("error en la obtencion de los usuarios: " + ex.Message);
                }
            }

            return lista_personas;
        }

        private void Limpiar()
        {
            txt_nombre.Text = "";
            txt_correo.Text = "";
            txt_edad.Text = "";
            fecha_picker.Text = "";
        }

        private void CargaDatos()
        {
            List<Persona> lista = new List<Persona>();

            lista = GetData();

            dataGridView1.DataSource = lista;
        }

        private Persona GetPersona()
        {
            int id = GetId();

            string query = "select * from Usuarios where Id=@id";

            using (SqlConnection conexion = new SqlConnection(cadena_conexion))
            {
                SqlCommand command = new SqlCommand(query, conexion);
                command.Parameters.AddWithValue("@id", id);
                

                try
                {
                    conexion.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    reader.Read();

                    Persona usuario = new Persona();

                    usuario.ID = reader.GetInt32(0);
                    usuario.NOMBRE = reader.GetString(1);
                    usuario.CORREO = reader.GetString(2);
                    usuario.FECHA_DE_NACIMIENTO = reader.GetDateTime(3);
                    usuario.EDAD = reader.GetInt32(4);

                    return usuario;

                }
                catch (Exception ex)
                {
                    throw new Exception("Error en la obtencio de personas: " + ex.Message);
                }
            }
        }

        private int GetEdad()
        {
            int edad = DateTime.Now.Year - fecha_picker.Value.Year;

            if(DateTime.Now.Month < fecha_picker.Value.Month)
            {
                edad -= 1;
            }
            else if (DateTime.Now.Month == fecha_picker.Value.Month  && DateTime.Now.Day < fecha_picker.Value.Day)
            {
                edad -= 1;
            }

            return edad;

        }

        private int GetId()
        {
            try
            {
                return Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("Error en la obtencion del ID: " + ex.Message);
            }
        }

        private void btn_agregar_Click(object sender, EventArgs e)
        {
            vari = "A";
            
            ModoCarga();
        }

        private void btn_mod_Click(object sender, EventArgs e)
        {
            vari = "M";

            Persona usuario = GetPersona();

            txt_nombre.Text = usuario.NOMBRE;
            txt_correo.Text = usuario.CORREO;
            fecha_picker.Value = usuario.FECHA_DE_NACIMIENTO;
            txt_edad.Text = Convert.ToString(usuario.EDAD);

            ModoCarga();
        }

        private void btn_eliminar_Click(object sender, EventArgs e)
        {
            int id = GetId();

            string query = "delete from Usuarios where Id=@id";

            Persona usuario = GetPersona();

            string fecha = usuario.FECHA_DE_NACIMIENTO.ToString("dd/MM/yyyy");

            using (SqlConnection conexion = new SqlConnection(cadena_conexion))
            {
                SqlCommand command = new SqlCommand(query, conexion);
                command.Parameters.AddWithValue("@id", id);

                try
                {
                    conexion.Open();

                    DialogResult respuesta = MessageBox.Show($"Esta seguro que desea eliminar al usuario:\n\nNombre: {usuario.NOMBRE}\n\nCorreo: {usuario.CORREO}\n\nFecha de nacimiento: {fecha}\n\nEdad: {usuario.EDAD}", "AVISO", MessageBoxButtons.YesNo);
                    
                    if (respuesta == DialogResult.Yes)
                    {
                        command.ExecuteNonQuery();
                    }                    

                    conexion.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error en la eliminacion del usuario: " + ex.Message);
                }

                CargaDatos();
            }


        }

        private void btn_guardar_Click(object sender, EventArgs e)
        {
            if (vari == "A")
            {
                int edad = GetEdad();

                string query = "insert into Usuarios(Nombre, Correo, [Fecha de nacimiento], Edad) values(@nombre, @correo, @fecha_de_nacimiento, @edad)";

                using (SqlConnection conexion = new SqlConnection(cadena_conexion))
                {
                    SqlCommand command = new SqlCommand(query, conexion);

                    command.Parameters.AddWithValue("@nombre", txt_nombre.Text);
                    command.Parameters.AddWithValue("@correo", txt_correo.Text);
                    command.Parameters.AddWithValue("@fecha_de_nacimiento", fecha_picker.Value.Date);
                    command.Parameters.AddWithValue("@edad", edad);

                    try
                    {
                        conexion.Open();

                        command.ExecuteNonQuery();

                        conexion.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("error en la agregacion de los usuarios: " + ex.Message);
                    }
                }
            }
            else if (vari == "M")
            {
                int edad = GetEdad();

                int id =  GetId();

                string query = "update Usuarios set Nombre=@nombre, Correo=@correo, [Fecha de nacimiento]=@fecha_de_nacimiento, Edad=@edad where Id=@id";

                using (SqlConnection conexion = new SqlConnection(cadena_conexion))
                {
                    SqlCommand command = new SqlCommand(query, conexion);

                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@nombre", txt_nombre.Text);
                    command.Parameters.AddWithValue("@correo", txt_correo.Text);
                    command.Parameters.AddWithValue("@fecha_de_nacimiento", fecha_picker.Value.Date);
                    command.Parameters.AddWithValue("@edad", edad);

                    try
                    {
                        conexion.Open();

                        command.ExecuteNonQuery();

                        conexion.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("error en la modificacion de los usuarios: " + ex.Message);
                    }
                }
            }

            CargaDatos();
            Limpiar();
            ModoVista();
        }

        private void btn_cancelar_Click(object sender, EventArgs e)
        {
            ModoVista();
        }

    }

        
}
