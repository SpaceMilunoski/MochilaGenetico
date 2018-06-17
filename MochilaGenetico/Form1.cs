using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MochilaGenetico
{
    public partial class Form1 : Form
    {        
        List<Item> Individuos = new List<Item>();
        List<Item> Padres = new List<Item>();
        List<double> Fnorm = new List<double>();
        List<Item> Hijos = new List<Item>();
        List<double> Acumulado = new List<double>();
        List<double> itemsPeso = new List<double>();
        List<double> itemsGanancia = new List<double>();
        List<String> Mutacion = new List<String>();
        Random r = new Random();
        Item mayor = new Item();
        int Poblacion;
        double sumatoria = 0;
        int capacidadMochila;
        string line;
        public Form1()
        {
            InitializeComponent();
        }
        public void cargrarDatos()
        {
            using (var fd = new FolderBrowserDialog())
            {
                if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fd.SelectedPath))
                {
                    tbDirectorio.Text = fd.SelectedPath;
                }
            }
            llenarDatos(tbDirectorio.Text + @"\c.txt",1);//capacidad de mochila
            llenarDatos(tbDirectorio.Text + @"\w.txt", 2);//llenado de pesos
            llenarDatos(tbDirectorio.Text + @"\p.txt", 3);//llenado de ganancias
        }
        public void llenarDatos(String directrorio,int v)
        {
            try
            {
                StreamReader fpeso = new StreamReader(directrorio);
                while ((line = fpeso.ReadLine()) != null)
                {
                    //System.Console.WriteLine(line);
                    if (v == 1)
                    {
                        capacidadMochila = Convert.ToInt32(line);
                    }
                    if (v==2)
                    {
                        itemsPeso.Add(Convert.ToDouble(line));
                    }
                    if (v==3)
                    {
                        itemsGanancia.Add(Convert.ToDouble(line));
                    }
                    //lbPruebas.Items.Add(line);
                }

                fpeso.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void inicializacion()
        {
            if (itemsPeso.Count == itemsGanancia.Count)
            {
                Poblacion = Convert.ToInt32(tbPoblacion.Text);
                for (int i = 0; i < Poblacion; i++)
                {
                    string binario = "";
                    for (int j = 0; j < itemsPeso.Count; j++)//Generar numero binario aleatorio
                    {
                        int random = r.Next(0, 2);
                        binario = binario + Convert.ToString(random);
                    }
                    Item item = new Item();
                    item.setValor(binario);
                    Individuos.Add(item);
                    //lbPruebas.Items.Add(binario);
                }
            }
            else
            {
                MessageBox.Show("Error","El numero de items en peso y ganancia no son los mismos");
            }           
        }
        public double Peso(String item)
        {
            int cont=0;
            double peso=0;
            foreach (char p in item)
            {
                if (p=='1')
                {
                    peso += itemsPeso[cont];
                }
                cont++;
            }
            return peso;
        }
        public void restricion()
        {
            sumatoria = 0;
            foreach (Item item in Individuos)
            {
                //lbPruebas.Items.Add(item.getValor());
                if (item.getPeso() < capacidadMochila)
                {
                    item.setGanancia(funcion(item.getValor()));
                }
                else
                {
                    item.setGanancia(0);
                }
                sumatoria += item.getGanacia();
            }
        }
        public double funcion(String item)
        {
            double ganancia=0;
            int cont = 0;
            foreach (char p in item)
            {
                if (p == '1')
                {
                    ganancia += itemsGanancia[cont];
                }
                cont++;
            }
            return ganancia;
        }
        public void fnorm()
        {
            int i = 0;
            double acumulado = 0;
            foreach(Item item in Individuos)
            {                                                
                Fnorm.Add(item.getGanacia() /sumatoria);
                acumulado = acumulado + Convert.ToDouble(Fnorm[i]);//Calcular acumulado de fnorm
                Acumulado.Add(acumulado);
                i++;
            }
        }
        public void limpieza()
        {
            lbPruebas.Items.Clear();
            Individuos.Clear();
            Padres.Clear();
            Hijos.Clear();
            Fnorm.Clear();
            Acumulado.Clear();
            Mutacion.Clear();
        }
        public void mostrar()
        {
            int i = 0;
           //foreach(Item item in Individuos)
           // {
           //     lbPruebas.Items.Add(item.getValor()+"  "+item.getPeso()+"  "+ item.getGanacia()+"  "+Fnorm[i]+"  "+Acumulado[i]);
           //     i++;
           // }
           //foreach(Item hijo in Hijos)
           // {
           //     lbPruebas.Items.Add(hijo.getValor());
           // }
           //foreach(String mutado in Mutacion)
           // {
           //     lbPruebas.Items.Add(mutado);
           // }
           foreach(Item indi in Individuos)
            {
                lbPruebas.Items.Add(indi.getValor());
            }
        }
        public void ruleta()
        {
            calcularMayor();
            for (int i = 0; i < Poblacion; i++) //seleccion por ruleta
                {
                    double random = r.NextDouble();//Numero random entre 0 y 1
                    for (int j = 0; j < Poblacion; j++)
                    {
                        if (Acumulado[j] > random)
                        {
                            Padres.Add(Individuos[j]);
                            j = Poblacion;
                        }
                    }
                }
        }
        public void cruce()
        {
            String aux1 = "", aux2 = "";
            double cruce;
            int pcrt1, pcrt2;
            for (int i = 0; i < Poblacion - 1; i++)
            {
                cruce = r.NextDouble();
                if (cruce <= Convert.ToDouble(tbPC.Text))
                {
                    do
                    {
                        pcrt1 = r.Next(0, itemsPeso.Count);//random de 0 a 7
                        pcrt2 = r.Next(0, itemsPeso.Count);
                    } while (pcrt1 == pcrt2);
                    if (pcrt1 < pcrt2)
                    {
                        for (int j = 0; j < itemsPeso.Count; j++)
                        {
                            if (j >= pcrt1 && j <= pcrt2)
                            {
                                aux1 += Padres[i+1].getValor()[j];
                                aux2 += Padres[i].getValor()[j];
                            }
                            else
                            {
                                aux1 += Padres[i].getValor()[j];
                                aux2 += Padres[i + 1].getValor()[j];
                            }

                        }
                        Item hijo = new Item();
                        hijo.setValor(aux1);
                        Hijos.Add(hijo);
                        aux1 = "";
                        hijo = new Item();
                        hijo.setValor(aux2);
                        Hijos.Add(hijo);
                        aux2 = "";
                    }
                    else
                    {
                        for (int j = 0; j < itemsPeso.Count; j++)
                        {
                            if (j <= pcrt2 || j >= pcrt1)
                            {
                                aux1 += Padres[i + 1].getValor()[j];
                                aux2 += Padres[i].getValor()[j];
                            }
                            else
                            {
                                aux1 += Padres[i].getValor()[j];
                                aux2 += Padres[i + 1].getValor()[j];
                            }

                        }
                        Item hijo = new Item();
                        hijo.setValor(aux1);
                        Hijos.Add(hijo);
                        aux1 = "";
                        hijo = new Item();
                        hijo.setValor(aux2);
                        Hijos.Add(hijo);
                        aux2 = "";
                    }
                    i++;
                }
                else
                {
                    Hijos.Add(Padres[i]);
                    i++;
                    Hijos.Add(Padres[i]);
                }
            }
        }
        public void mutacion()
        {
            for (int i = 0; i < Poblacion; i++) //Recorrer el numero de hijos
            {
                string aux = "";
                for (int j = 0; j < itemsPeso.Count; j++)//Recorrer el numero de elementos de cada hijo
                {
                    double al1 = r.NextDouble();
                    if (al1 < Convert.ToDouble(tbPM.Text))//Si el numero aleatorio es menor que la probabilidad de mutacion
                    {
                        if (Hijos[i].getValor()[j] == '0')//si la posicion actual de j en hijo es 0
                        {
                            aux += "1";
                        }
                        else
                        {
                            aux += "0";
                        }
                    }
                    else
                    {
                        aux += Hijos[i].getValor().ToString()[j];
                    }
                }
                Mutacion.Add(aux);
                aux = "";
            }
        }
        public void reinicio()
        {
            //lbPruebas.Items.Clear();
            Individuos.Clear();
            foreach (String individuo in Mutacion)
            {
                Item ind = new Item();
                ind.setValor(individuo);
                Individuos.Add(ind);
            }
            Padres.Clear();
            Hijos.Clear();
            Fnorm.Clear();
            Acumulado.Clear();
            Mutacion.Clear();
        }
        public void calcularMayor()
        {
            foreach(Item individuo in Individuos)
            {
                if (mayor.getGanacia()<individuo.getGanacia())
                {
                    mayor = individuo;
                }
            }
        }
        public void genetico()
        {
            int vueltas = Convert.ToInt32(tbVueltas.Text);
            inicializacion();
            do
            {
            foreach(Item item in Individuos)
            {
                item.setPeso(Peso(item.getValor()));
            }
            
                restricion();//se aplica la restricion y se calcula la ganancia y la sumatoria
                fnorm();
                ruleta();
                cruce();
                mutacion();
                reinicio();
                mostrar();
                lbres.Text ="Combinacion de item: "+ mayor.getValor() + "\n Peso: " + mayor.getPeso() + "\n Ganancia: " + mayor.getGanacia();
                vueltas--;
            } while (vueltas!=0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            limpieza();
            genetico();
           
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cargrarDatos();
        }
    }
}
