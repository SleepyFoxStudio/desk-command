using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OBSWebsocketDotNet;

namespace DeskCommandCore.Actions.Obs
{
    public class ObsWrapper
    {
        private static ObsWrapper _instance;

        public OBSWebsocket Socket { get; private set; }

        private ObsWrapper()
        {
            Socket = new OBSWebsocket();
        }

        public void ToggleRecording()
        {
            EnsureConnected();
            Socket.ToggleRecording();
        }

        private void EnsureConnected()
        {
            var txtServerPassword = "";

            if (!Socket.IsConnected)
            {
                try
                {
                    //TODO: get from Config
                    Socket.Connect("ws://127.0.0.1:4444", txtServerPassword);
                }
                catch (AuthFailureException authFailure)
                {
                    //TODO: handle in GUI
                    throw;
                }
                catch (ErrorResponseException ex)
                {
                    //TODO: handle in GUI
                    throw;
                }
                if (!Socket.IsConnected)
                {
                    throw new Exception("FAILED TO CONNECT TO obs ws");
                }
            }
        }

        public static ObsWrapper Instance
        {

            get
            {
                if (_instance == null)
                {
                    _instance = new ObsWrapper();
                }

                return _instance;
            }
        }
    }
}
