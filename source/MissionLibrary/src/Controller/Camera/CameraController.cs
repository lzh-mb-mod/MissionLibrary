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
