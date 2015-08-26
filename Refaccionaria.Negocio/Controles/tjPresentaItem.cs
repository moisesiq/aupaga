using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Refaccionaria.Negocio.Controles
{
    public partial class tjPresentaItem : UserControl
    {
        public int Item { get; protected set; }
        public bool Selected { get; private set; }
        public int IdProvee { get; set; }
        public string Proveedor { get; set; }
        private string objClickeado { get; set; }
        private string dir_Mapa;
        private bool equivalente { get; set; }
        private CuadroRedondo theShape;

        public event EventHandler<EventArgs> WasClicked;

        public tjPresentaItem(int index)
        {
            InitializeComponent();
            this.theShape = new CuadroRedondo(2, 2, 304, 89);
            this.theShape.Propietario = this;
            this.equivalente = false;

            this.Item = index;
            this.Selected = false;
            foreach (Control c in this.Controls)
                c.Click += new System.EventHandler(this.Shape_Click);
        }

        private void Shape_Click(object sender, EventArgs e)
        {
            this.objClickeado = ((Control)sender).Name;

            var wasClicked = WasClicked;
            if (wasClicked != null)
                WasClicked(this, EventArgs.Empty);
        }

        public void WasSelect()
        {
            this.theShape.CuadroSombra(true, this.equivalente);
            this.Selected = true;
        }

        public void UnSelect()
        {
            this.theShape.CuadroSombra(false, this.equivalente);
            this.Selected = false;
        }

        public void Titulo(string titulo)
        {
            this.Proveedor = titulo;
            lblTitulo.Text = titulo;
        }

        public void logo(Image imgLogo, string llave)
        {
            if (llave == "0")
                this.tjLogo.Image = this.tjLogo.ErrorImage;
            else
                this.tjLogo.Image = imgLogo;
        }

        public void shadow(bool generico)
        {
            //if (generico) Sombra.BackColor = Color.CornflowerBlue;
        }

        public void DatosContacto(string Nombre, string TelUno, string TelDos, string TelTres, string Direccion, string webPage, bool generico = false)
        {
            lblContacto.Text = Nombre;
            lblTelefono.Text = TelUno;
            lblTelDos.Text = TelDos;
            lblTelTres.Text = TelTres;

            // Crea vínculos web a la página principal y a la dirección con g.maps
            this.dir_Mapa = "www.google.com.mx/maps/place/" + Direccion + " México";
            this.AsignaLink(webPage, this.lnkWeb);
            if (Direccion.Length <= 15) lnkMap.Visible = false;
            this.equivalente = generico;
        }

        private void AsignaLink(string target, LinkLabel link)
        {
            if (target == null || target == "")
                link.Visible = false;
            else
            {
                link.Visible = true;
                link.Text = target;
                if (target.StartsWith("www."))
                {
                    link.Enabled = true;
                    link.Links[0].LinkData = target;
                }
                else
                    link.Enabled = false;
            }
        }

        public string ClickSobre()
        {
            string returnValue = this.objClickeado;
            this.objClickeado = "";
            return returnValue;
        }

        private void lnkWeb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string target = e.Link.LinkData as string;
            System.Diagnostics.Process.Start(target);
        }

        private void lnkMap_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(this.dir_Mapa);
        }

        private void tjPresentaItem_Paint(object sender, PaintEventArgs e)
        {
            if (this.Selected)
                this.theShape.CuadroSombra(true, this.equivalente);
            else
                this.theShape.CuadroSombra(false, this.equivalente);
        }
    }

    public class CuadroRedondo
    {
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int radio { get; set; }

        public Color color { get; set; }
        public Color colorBorde { get; set; }

        public int Sombra { get; set; }
        public Color colorSombra { get; set; }

        public Control Propietario;

        public CuadroRedondo(int x, int y, int width, int heigth, int radio = 7)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = heigth;
            this.radio = radio;

            this.color = System.Drawing.Color.White;
            this.colorSombra = System.Drawing.Color.DarkGray;
            this.colorBorde = System.Drawing.Color.Black;

            this.Sombra = 5;
        }


        public void esquinasRedondas(bool esCapaSuperior = true, bool esSeleccionado = false, bool equivale = false)
        {
            System.Drawing.Color cCara = this.color;
            System.Drawing.Color cBorde = (!esSeleccionado) ? this.colorBorde : Color.Blue;
            System.Drawing.Color CaraSombra;
            int x = this.x;
            int y = this.y;


            if (!esCapaSuperior)
            {
                x = this.x + this.Sombra;
                y = this.y + this.Sombra;
                if (equivale)
                    CaraSombra = (esSeleccionado) ? Color.MidnightBlue : Color.PowderBlue;
                else 
                    CaraSombra = (esSeleccionado) ? Color.Gray : Color.Silver;

                cCara = CaraSombra;
                cBorde = CaraSombra;
            }

            Rectangle miBase = new Rectangle(x, y, this.width, this.height);

            GraphicsPath gp = new GraphicsPath();
            LinearGradientBrush myBrush = new LinearGradientBrush(miBase, cCara, cCara, 45, false);

            System.Drawing.Graphics formGraphics = this.Propietario.CreateGraphics();

            gp.AddLine(x + this.radio, y, x + this.width - (this.radio * 2), y); // Line
            gp.AddArc(x + this.width - (this.radio * 2), y, this.radio * 2, this.radio * 2, 270, 90); // Corner
            gp.AddLine(x + this.width, y + this.radio, x + this.width, y + this.height - (this.radio * 2)); // Line
            gp.AddArc(x + this.width - (this.radio * 2), y + this.height - (this.radio * 2), this.radio * 2, this.radio * 2, 0, 90); // Corner
            gp.AddLine(x + this.width - (this.radio * 2), y + this.height, x + this.radio, y + this.height); // Line
            gp.AddArc(x, y + this.height - (this.radio * 2), this.radio * 2, this.radio * 2, 90, 90); // Corner
            gp.AddLine(x, y + this.height - (this.radio * 2), x, y + this.radio); // Line
            gp.AddArc(x, y, this.radio * 2, this.radio * 2, 180, 90); // Corner
            gp.CloseFigure();

            Pen contorno = new Pen(cBorde, 1);

            formGraphics.FillPath(myBrush, gp);
            formGraphics.DrawPath(contorno, gp);

            myBrush.Dispose();
            formGraphics.Dispose();

        }

        public void CuadroSombra(bool Seleccion = false, bool equivale = false)
        {
            this.esquinasRedondas(false, Seleccion, equivale);
            this.esquinasRedondas(true, Seleccion, equivale);

        }

    }

}