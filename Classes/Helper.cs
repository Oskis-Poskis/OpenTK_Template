using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;

namespace WindowTemplate.Common
{
    public class HelperClass
    {
        public bool ToggleBool(bool toggle_bool)
        {
            return !toggle_bool;
        }

        public float MapRange(float value, float input_min, float input_max, float output_min, float output_max)
        {
            return (value - input_min) / (input_max - input_min) * (output_max - output_min) + output_min;
        }

        public Vector2 Pixels_To_DC(float x, float y)
        {
            return new Vector2(MapRange(x, 0, HostWindow.window_size.X, -1.0f, 1.0f),
                               MapRange(y, 0, HostWindow.window_size.Y, -1.0f, 1.0f));
        }

        public float RandFloat()
        {
            Random rand = new Random();
            return (float)rand.NextDouble();
        }
    }

    public class StatCounter
    {
        public int frame_count = 0;
        public double elapsed_time = 0.0, fps = 0.0, ms;

        public float total_memory;
        public float total_video_memory;
        public float free_video_memory;

        public void Count(FrameEventArgs args)
        {
            frame_count++;
            elapsed_time += args.Time;
            if (elapsed_time >= 1f)
            {
                fps = frame_count / elapsed_time;
                ms = 1000 * elapsed_time / frame_count;
                frame_count = 0;
                elapsed_time = 0.0;
            }

            if (GLFW.ExtensionSupported("GL_NVX_gpu_memory_info") && GL.GetString(StringName.Vendor) == "NVIDIA Corporation")
            {
                total_video_memory = GL.GetInteger((GetPName)0x9048) / 1024.0f;
                free_video_memory = total_video_memory - GL.GetInteger((GetPName)0x9049) / 1024.0f;
            }

            if (GLFW.ExtensionSupported("GL_ATI_meminfo") && GL.GetString(StringName.Vendor) == "AMD")
            {
                total_video_memory = 0.0f;
                free_video_memory = GL.GetInteger((GetPName)0x87FB) / 1024.0f;
            }

            GCMemoryInfo gcMemoryInfo = GC.GetGCMemoryInfo();
            total_memory = gcMemoryInfo.TotalAvailableMemoryBytes / (1000.0f * 1000 * 1000);
        }
    }
}