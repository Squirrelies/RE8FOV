namespace RE8FOV
{
    public class Settings
    {
        internal float normalFOV;
        public float NormalFOV { get => normalFOV; set => normalFOV = value; }

        internal float aimingFOV;
        public float AimingFOV { get => aimingFOV; set => aimingFOV = value; }

        public Settings()
        {
            normalFOV = 81f;
            aimingFOV = 70f;
        }
    }
}
