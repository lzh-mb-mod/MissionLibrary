using System;
using System.Collections.Generic;
using System.Text;

namespace MissionLibrary.Controller.Camera
{
    public static class CameraController
    {
        public static ICameraController Instance { get; set; }

        public static void Clear()
        {
            Instance = null;
        }
    }
}
