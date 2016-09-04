namespace BulletMLLib
{
    public static class BulletMLManager
    {
        private static IBulletMLManager _ib;

        public static void Init(IBulletMLManager ib1)
        {
            _ib = ib1;
        }

        public static float GetRandom(){ return _ib.GetRandom();}

        public static float GetRank() { return _ib.GetRank(); }

        public static float GetShipPosX() { return _ib.GetShipPosX(); }

        public static float GetShipPosY() { return _ib.GetShipPosY(); }
    }
}