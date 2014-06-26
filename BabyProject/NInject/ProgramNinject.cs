using System;
using Ninject;
using Ninject.Modules;

namespace BabyProject.NInject
{
    class IocContainerConfiguration : NinjectModule
    {
        public override void Load()
        {
            Bind<IShape>().To<Circle>();
            //Circle needs radius service
            Bind<IRadiusService>().To<RadiusService>();
            Bind<IRadiusService>().To<DeadRadiusService>();
        }
    }
    class ProgramNinject
    {
        void Main()
        {
            var containerConfig = new IocContainerConfiguration();
            
            // 1.Singleton
            ServiceLocator.Instance.InitialiseServiceLocator(new INinjectModule[] { containerConfig });
            var kernel = new StandardKernel(containerConfig);
            var shape = kernel.Get<IShape>();
            
            // 2.Routine
            //var shape=ServiceLocator.Instance.GetType<IShape>();           
            shape.Draw();
        }
    }

    internal interface IShape
    {
        void Draw();
    }

    class Circle : IShape
    {        
        private readonly IRadiusService _radiusService;
        
        public Circle(IRadiusService radiusService)
        {         
            _radiusService = radiusService;
        }

        public void Draw()
        {
            Console.WriteLine("Hi Circle with radius = {0} ", _radiusService.Radius());
        }
    }

    class RadiusService : IRadiusService
    {
        public decimal Radius()
        {
            return 420m;
        }
    }

    class Rectangle : IShape
    {
        public void Draw()
        {
            Console.WriteLine("Hi Rectangle");
        }
    }
}