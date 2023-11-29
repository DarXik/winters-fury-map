namespace Managers
{
    public static class BurnConverter
    {
        public static int GetFuelHours(int burnTime)
        {
            return burnTime / 60;
        }

        public static int GetFuelMinutes(int burnTime)
        {
            return burnTime % 60;
        }
    }
}