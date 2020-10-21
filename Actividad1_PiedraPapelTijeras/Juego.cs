using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Actividad1_PiedraPapelTijeras
{
    public enum Opcion { PIEDRA, PAPEL, TIJERAS }
    public class Juego : INotifyPropertyChanged
    {
        public string Jugador1 { get; set; } = "Jugador";
        public string Jugador2 { get; set; }

        public string PuntosJugador1 { get; set; }
        public string PuntosJugador2 { get; set; }

        public string SeleccionJugador1 { get; set; }
        public string SeleccionJugador2 { get; set; }
        public string Mensaje { get; set; }
        public string IP { get; set; }
        public bool MainWindowVisible { get; set; } = true;
        public ICommand SeleccionarOpcionCommand { get; set; }
        public ICommand IniciarCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        HttpListener servidor;
        ClientWebSocket cliente;
        Dispatcher currentDispatcher;
        public Juego()
        {
            currentDispatcher = Dispatcher.CurrentDispatcher;
            IniciarCommand = new RelayCommand<bool>(IniciarPartida);
        }

        private void Lobby_Closing(object sender, CancelEventArgs e)
        {
            MainWindowVisible = true;
            Actualizar();
            if (servidor != null)
            {
                servidor.Stop();
                servidor = null;
            }
        }

        Lobby lobby;
        private async void IniciarPartida(bool tipoPartida)
        {
            try
            {
                MainWindowVisible = false;
                lobby = new Lobby();
                lobby.Closing += Lobby_Closing;
                lobby.DataContext = this;
                lobby.Show();
                Actualizar();
                if (tipoPartida == true)
                {
                    CrearPartida();
                }
                else
                {
                    Mensaje = "Intentando conectar con el servidor en " + IP;
                    Actualizar("Mensaje");
                    await ConectarPartida();
                }
            }
            catch (Exception ex)

            {
                Mensaje = ex.Message;
                Actualizar();
            }
        }

        public async Task ConectarPartida()
        {
            cliente = new ClientWebSocket();
            await cliente.ConnectAsync(new Uri($"ws://{IP}:1000/ppt/"), CancellationToken.None);

        }

        public void CrearPartida()
        {
            servidor = new HttpListener();
            servidor.Prefixes.Add("http://*:1000/ppt/");
            servidor.Start();
            servidor.BeginGetContext(OnContext, null);

            Mensaje = "Esperando a que se conecte un adversario...";
            Actualizar();
        }

        private void OnContext(IAsyncResult ar)
        {

        }

        public void Actualizar(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
