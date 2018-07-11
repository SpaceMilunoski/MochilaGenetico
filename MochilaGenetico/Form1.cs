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
                tbDirectorio.Text = "";
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
            double ganancia;
            foreach (Item item in Individuos)
            {
                ganancia = 0;
                //lbPruebas.Items.Add(item.getValor());
                if (item.getPeso() <= capacidadMochila)
                {
                    item.setGanancia(funcion(item.getValor()));
                }
                else
                {
                    //item.setGanancia(0);
                    // ganancia = (funcion(item.getValor()))-penalizacion(item.getValor());//ganancia con penalizacion
                    //item.setValor(reparacionVoraz(item.getValor()));
                    //ganancia = (funcion(item.getValor()));
                    //item.setGanancia(ganancia);
                    String individuo = propio(item);
                    if (individuo == item.getValor())
                    {
                        item.setGanancia(0);
                    }
                    else
                    {
                        item.setValor(individuo);
                        item.setPeso(Peso(individuo));
                        item.setGanancia(funcion(individuo));
                    }
                }
                sumatoria += item.getGanacia();
            }
        }
        public string reparacionAleatoria(string individuo)
        {
            char[] it = individuo.ToCharArray();
            int random=0;
            do
            {
                do
                {
                    random = r.Next(0, individuo.Length);
                }
                while (it[random]=='0');
                it[random] = '0';
                individuo = Convert.ToString(it);
            }
            while (Peso(individuo) > capacidadMochila);
            return individuo;
        }
        public string reparacionVoraz(string individuo)
        {
            char[] it = individuo.ToCharArray();
            int id = 0;
            do
            {
                id = funcionMenor(individuo);
                it[id] = '0';
                individuo = Convert.ToString(it);
            } while (Peso(individuo) > capacidadMochila);
            return individuo;
        }
        public double penalizacion(string items)
        {
            double p=0,pen=0,mp=0;
            for(int i = 0; i < items.Length; i++)
            {
                if (items[i] == '1')
                {
                    if (mp<(itemsGanancia[i]/itemsPeso[i]))
                    {
                        mp = itemsGanancia[i] / itemsPeso[i];
                    }
                }
            }
            p = mp;
            for (int i = 0; i < items.Length; i++)
            {
                pen += p * (items[i] * itemsPeso[i] - capacidadMochila);
            }
            return pen;
        }
        private String propio(Item individio)
        {
            char[] aux = individio.getValor().ToCharArray();
            for (int i=0;i<individio.getValor().Length;i++)
            {
                if ((individio.getPeso()-itemsPeso[i]) <= capacidadMochila)
                {
                    aux[i] = '0';
                    break;
                }
            }
            return new String(aux);
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
        public int funcionMenor(String item)
        {
            int cont = 0;
            int menor = 0;
            foreach(char p in item)
            {
                if (p == '1')
                {
                    if(itemsGanancia[cont]/itemsPeso[cont]< itemsGanancia[menor] / itemsPeso[menor])
                    {
                        menor = cont;
                    }
                }
                cont++;
            }
            return Convert.ToInt32(menor);
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
            mayor = new Item();
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
            foreach (Item item in Individuos)
            {
                lbPruebas.Items.Add(item.getValor() + "  " + item.getPeso() + "  " + item.getGanacia() + "  " + Fnorm[i] + "  " + Acumulado[i]);
                i++;
            }
            //foreach(Item hijo in Hijos)
            // {
            //     lbPruebas.Items.Add(hijo.getValor());
            // }
            //foreach(String mutado in Mutacion)
            // {
            //     lbPruebas.Items.Add(mutado);
            // }
            //foreach(Item indi in Individuos)
            // {
            //     lbPruebas.Items.Add(indi.getValor());
            // }
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
        public void reinicio()//se limpian los individuos de agregan los que estan en mutacion a individuos y se limpian los demas valores
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
        public void calcularMayor()//se calcula la mejor convinacion de items
        {
            foreach(Item individuo in Individuos)
            {
                if (mayor.getGanacia()<individuo.getGanacia())
                {
                    mayor = individuo;
                }
            }
        }
        public void genetico()//se llaman todos los metodos para el algoritmo genetico
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
                fnorm();mostrar();
                ruleta();
                cruce();
                mutacion();
                reinicio();
                
                lbres.Text ="Combinacion de item: "+ mayor.getValor() + "\n Peso: " + mayor.getPeso() + "\n Ganancia: " + mayor.getGanacia();
                vueltas--;
            } while (vueltas!=0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tbDirectorio.Text != "" && tbPC.Text != "" && tbPM.Text != "" && tbPoblacion.Text != "" && tbVueltas.Text != "")
            {
                if (Convert.ToInt32(tbPoblacion.Text) % 2 == 0)
                {
                    if (Convert.ToDouble(tbPC.Text) >= 0.65 && Convert.ToDouble(tbPC.Text) <= 0.80)
                    {
                        if (Convert.ToDouble(tbPM.Text) >= 0.001 && Convert.ToDouble(tbPM.Text) <= 0.01)
                        {
                            if (Convert.ToInt32(tbVueltas.Text) > 0)
                            {
                                /*listBox1.Items.Add(Convert.ToString(funcion("10111011")));
                                listBox1.Items.Add(Convert.ToString(funcion("101010111000011")));
                                listBox1.Items.Add(Convert.ToString(funcion("110111000110100100000111")));*/
                                listBox1.Items.Clear();
                                listBox1.Items.Add("900");
                                listBox1.Items.Add("1458");
                                listBox1.Items.Add("13549094");
                                limpieza();
                                genetico();
                            }
                            else
                            {
                                MessageBox.Show("El numero de vueltas debe ser mayor a 0", "Error");
                            }
                        }
                        else
                        {
                            MessageBox.Show("La probabilidad de mutacion debe ser mayor o igual a 0.001 y menor o igual a 0.01", "Error");
                        }
                    }
                    else
                    {
                        MessageBox.Show("La probabilidad de cruce debe ser mayor o igual a 0.65 y menor o igual a 0.80", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("El numero de población debe ser par", "Error");
                }
            }
            else
            {
                MessageBox.Show("Falta llenar algun dato", "Error");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cargrarDatos();
        }

        private void tbPoblacion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar))
            {
                MessageBox.Show("Caracter no valido, Ingresa un número", "Número incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                e.Handled = true;
                tbPoblacion.Text = "";
            }
        }

        private void tbPC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar))
            {
                MessageBox.Show("Caracter no valido, Ingresa un número", "Número incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                e.Handled = true;
                tbPoblacion.Text = "";
            }
        }

        private void tbPM_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar))
            {
                MessageBox.Show("Caracter no valido, Ingresa un número", "Número incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                e.Handled = true;
                tbPoblacion.Text = "";
            }
        }

        private void tbVueltas_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar))
            {
                MessageBox.Show("Caracter no valido, Ingresa un número", "Número incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                e.Handled = true;
                tbPoblacion.Text = "";
            }
        }
    }
}
