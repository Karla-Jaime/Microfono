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
            //el 0 es el predeterminado en el sistema
            //Establecer el formato muestreo. prof. bits y canales de entrada.
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
            throw new NotImplementedException();
        }
    }
}
