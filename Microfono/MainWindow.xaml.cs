using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio;
using NAudio.Wave;
using NAudio.Dsp;//procesador digital de señales.

namespace Microfono
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WaveIn wavein; //entrada. Conexión con el microfono
        WaveFormat formato; 




        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnIniciar_Click(object sender, RoutedEventArgs e) 
        {
            //Inicializar conexión 
            wavein = new WaveIn();
            //el 0 es el predeterminado en el sistema //Establecer el formato muestreo. profundidad de bits y canales de entrada.
            wavein.WaveFormat = new WaveFormat(44100,16,1);
            formato = wavein.WaveFormat;

            //Conf. la duración del buffer --> Esta en ms 1 seg = 500ms
            wavein.BufferMilliseconds = 500;

            //Con que función resondemos cuando se llena el buffer
            wavein.DataAvailable += Wavein_DataAvailable;

            wavein.StartRecording();//Escucha el microfono
        }

        private void Wavein_DataAvailable(object sender, WaveInEventArgs e)
        {
            byte[] buffer = e.Buffer;
            int bytesGrabados = e.BytesRecorded;

            int numMuestras = bytesGrabados / 2;

            int exponente = 0;            
            int numBits = 0;
            do
            {
                exponente++;
                numBits = (int)Math.Pow(2, exponente);
            } while (numBits < numMuestras);
            exponente -= 1;
            
            numBits = (int) Math.Pow(2, exponente);
            Complex[] muestrasComplejas = new Complex[numBits];
            
            for (int i = 0; i < bytesGrabados; i +=2)
            {
                short muestra = (short) (buffer[i + 1] << 8 | buffer[i]);
                float muestra32bits = (float)muestra / 32768.0f ;
                if (i/2 < numBits)
                {
                    muestrasComplejas[i / 2].X = muestra32bits;
                }
            }
            FastFourierTransform.FFT(true,exponente,muestrasComplejas);
            //arreglo valAbsolutos
            float[] valoresAbsolutos = new float[muestrasComplejas.Length];

            for(int i = 0; i <muestrasComplejas.Length; i++)
            {
                valoresAbsolutos[i] = (float)Math.Sqrt((muestrasComplejas[i].X + muestrasComplejas[i].X)
                    + (muestrasComplejas[i].Y * muestrasComplejas[i].Y));

            }
            int indiceValorMaximo = valoresAbsolutos.ToList().IndexOf(valoresAbsolutos.Max());
            float frecuenciaFundamental = (float)(indiceValorMaximo * formato.SampleRate) / (float)(valoresAbsolutos.Length);

            lblHertz.Text = frecuenciaFundamental.ToString("N") + "H";
        }

        private void BtnDetener_Click(object sender, RoutedEventArgs e)
        {
            wavein.StartRecording();
        }
    }
}
