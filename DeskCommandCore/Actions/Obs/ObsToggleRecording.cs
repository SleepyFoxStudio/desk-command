using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OBSWebsocketDotNet;

namespace DeskCommandCore.Actions.Obs
{
    public class ObsToggleRecording : InterfaceAction
    {

        public void Do()
        {
            ObsWrapper.Instance.ToggleRecording();
        }
    }
}
