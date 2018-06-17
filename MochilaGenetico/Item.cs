using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MochilaGenetico
{
    class Item
    {
        protected String Valor;
        protected double Peso;
        protected double Ganancia;
        public String getValor()
        {
            return this.Valor;
        }
        public void setValor(String valor)
        {
            this.Valor = valor;
        }
        public double getPeso()
        {
            return this.Peso;
        }
        public void setPeso(double peso)
        {
            this.Peso = peso;
        }
        public double getGanacia()
        {
            return this.Ganancia;
        }
        public void setGanancia(double ganancia)
        {
            this.Ganancia = ganancia;
        }
    }
}
