using System.Text.Json;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace WindowTemplate
{
    [Serializable]
    public struct WindowProperties
    {
        public int width { get; set; }
        public int height { get; set; }
        public int positionx { get; set; }
        public int positiony { get; set; }
        public bool maximized { get; set; }

        public WindowProperties()
        {
            width = 1200;
            height = 800;
            positionx = 0;
            positiony = 0;
            maximized = false;
        }   
    }

    public class WindowSaveState
    {
        public WindowProperties properties = new();
        public WindowSaveState(WindowProperties Properties)
        {
            properties = Properties;
        }

        JsonSerializerOptions settings = new JsonSerializerOptions{ WriteIndented = true };
        string save_path = HostWindow.base_directory + "Save/windowstate.txt";

        unsafe public void SaveState(Window* WindowPtr)
        {
            if (Path.Exists(HostWindow.base_directory + "Save/windowstate.txt"))
            {
                Save();
            }

            else
            {
                Directory.CreateDirectory(HostWindow.base_directory + "Save");
                Save();
            }

            void Save()
            {
                GLFW.GetWindowSize(WindowPtr, out int width, out int height);
                properties.width = width;
                properties.height = height;

                GLFW.GetWindowPos(WindowPtr, out int x, out int y);
                properties.positionx = x;
                properties.positiony = y;

                properties.maximized = GLFW.GetWindowAttrib(WindowPtr, WindowAttributeGetBool.Maximized);

                string save_file = JsonSerializer.Serialize(properties, settings);
                using (StreamWriter writer = new StreamWriter(save_path))
                {
                    writer.Write(save_file);
                }

                Console.WriteLine("Saved window state");
            }
        }

        unsafe public void LoadState(Window* WindowPtr)
        {
            if (Path.Exists(save_path))
            {
                string json = File.ReadAllText(save_path);
                WindowProperties loaded_state = JsonSerializer.Deserialize<WindowProperties>(json);
                if (loaded_state.width > 0 && loaded_state.height > 0)
                {
                    properties = loaded_state;
                    Console.WriteLine("Succesfully loaded saved window state");
                    Console.WriteLine(json);
                }

                else
                {
                    properties = loaded_state;
                    properties.width = 500;
                    properties.height = 500;
                }
            }

            else
            {
                Console.WriteLine("Window state file path does not exist, creating save file:\n" + save_path);
                properties.positionx = 2560/2 - properties.width / 2;
                properties.positiony = 1440/2 - properties.height / 2;
            }

            GLFW.SetWindowSize(WindowPtr, properties.width, properties.height);
            GLFW.SetWindowPos(WindowPtr, properties.positionx, properties.positiony);
            if (properties.maximized)
            {
                GLFW.MaximizeWindow(WindowPtr);
                Console.WriteLine("Maximized Window");
            }
            
        }

        public void Resize(int Width, int Height)
        {
            properties.width = Width;
            properties.height = Height;
        }
    }
}