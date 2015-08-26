
namespace Refaccionaria.App
{
    public class ConteoMonedas
    {
        #region [ Propiedades ]

        public int Monedas1000 { get; set; }
        public int Monedas500 { get; set; }
        public int Monedas200 { get; set; }
        public int Monedas100 { get; set; }
        public int Monedas50 { get; set; }
        public int Monedas20 { get; set; }
        public int Monedas10 { get; set; }
        public int Monedas5 { get; set; }
        public int Monedas2 { get; set; }
        public int Monedas1 { get; set; }
        public int Monedas05 { get; set; }
        public int Monedas02 { get; set; }
        public int Monedas01 { get; set; }

        #endregion

        #region [ Propiedades calculadas ]

        public decimal TotalDe1000 { get { return (this.Monedas1000 * 1000); } }
        public decimal TotalDe500 { get { return (this.Monedas500 * 500); } }
        public decimal TotalDe200 { get { return (this.Monedas200 * 200); } }
        public decimal TotalDe100 { get { return (this.Monedas100 * 100); } }
        public decimal TotalDe50 { get { return (this.Monedas50 * 50); } }
        public decimal TotalDe20 { get { return (this.Monedas20 * 20); } }
        public decimal TotalDe10 { get { return (this.Monedas10 * 10); } }
        public decimal TotalDe5 { get { return (this.Monedas5 * 5); } }
        public decimal TotalDe2 { get { return (this.Monedas2 * 2); } }
        public decimal TotalDe1 { get { return (this.Monedas1 * 1); } }
        public decimal TotalDe05 { get { return (this.Monedas05 * 0.5M); } }
        public decimal TotalDe02 { get { return (this.Monedas02 * 0.2M); } }
        public decimal TotalDe01 { get { return (this.Monedas01 * 0.1M); } }

        public decimal TotalConteoMonedas
        {
            get
            {
                return (
                    this.TotalDe1000
                    + this.TotalDe500
                    + this.TotalDe200
                    + this.TotalDe100
                    + this.TotalDe50
                    + this.TotalDe20
                    + this.TotalDe10
                    + this.TotalDe5
                    + this.TotalDe2
                    + this.TotalDe1
                    + this.TotalDe05
                    + this.TotalDe02
                    + this.TotalDe01
                );
            }
        }

        #endregion
    }
}
