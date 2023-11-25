namespace AsciiForge.Helpers
{
    public static class MathExt
    {
        public static float Lerp(float src, float dst, float percent)
        {
            percent = Math.Clamp(percent, 0, 1);
            return src * (1 - percent) + dst * percent;
        }
        public static double Lerp(double src, double dst, double percent)
        {
            percent = Math.Clamp(percent, 0, 1);
            return src * (1 - percent) + dst * percent;
        }
    }
}
