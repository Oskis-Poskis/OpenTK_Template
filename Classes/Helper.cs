using OpenTK.Windowing.Common;
using OpenTK.Mathematics;

namespace WindowTemplate.Common
{
    public class HelperClass
    {
        public static bool ToggleBool(bool toggle_bool)
        {
            return !toggle_bool;
        }

        public static float MapRange(float value, float input_min, float input_max, float output_min, float output_max)
        {
            return (value - input_min) / (input_max - input_min) * (output_max - output_min) + output_min;
        }

        public static Vector2 Pixels_To_NDC(float x, float y)
        {
            return new Vector2(MapRange(x, 0, HostWindow.window_size.X, -1.0f, 1.0f),
                               MapRange(y, 0, HostWindow.window_size.Y, -1.0f, 1.0f));
        }

        public static float RandFloat()
        {
            Random rand = new Random();
            return (float)rand.NextDouble();
        }
    }

    public class StatCounter
    {
        public int frameCount = 0;
        public double elapsedTime = 0.0, fps = 0.0, ms;

        public void Count(FrameEventArgs args)
        {
            frameCount++;
            elapsedTime += args.Time;
            if (elapsedTime >= 1f)
            {
                fps = frameCount / elapsedTime;
                ms = 1000 * elapsedTime / frameCount;
                frameCount = 0;
                elapsedTime = 0.0;
            }
        }
    }
}