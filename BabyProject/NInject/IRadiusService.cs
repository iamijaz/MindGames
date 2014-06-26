namespace BabyProject.NInject
{
    internal interface IRadiusService
    {
        decimal Radius();
    }

    class DeadRadiusService : IRadiusService
    {
        public decimal Radius()
        {
            return decimal.MaxValue;
        }
    }
}