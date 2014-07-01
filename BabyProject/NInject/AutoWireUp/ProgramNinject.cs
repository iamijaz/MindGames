using System;
using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using NUnit.Framework;

namespace BabyProject.NInject.AutoWireUp
{
    class IocContainerConfiguration : NinjectModule
    {
        public override void Load()
        {
            Bind<IShape>().To<Circle>();
            //Circle needs radius service
            Bind<IRadiusService>().To<RadiusService>().WhenAnyAncestorMatches(IsShapeUser<ShapeUser>);
            Bind<IRadiusService>().To<DeadRadiusService>().WhenAnyAncestorMatches(IsShapeUser<ShapeUser2>);            
        }

        private bool IsShapeUser<T>(IContext context)
        {
            var request = context.Request.ParentRequest;
            while (request != null)
            {
                if (request.Service == typeof(T))
                {
                    return true;
                }
                request = request.ParentRequest;
            }

            return false;
        }

        private bool IsShapeUser2(IContext context)
        {
            var request = context.Request.ParentRequest;
            while (request != null)
            {
                if (request.Service == typeof(ShapeUser2))
                {
                    return true;
                }
                request = request.ParentRequest;
            }

            return false;
        }
    }

  

    public class ShapeUser
    {
        private readonly IShape _shape;

        public ShapeUser(IShape shape)
        {
            _shape = shape;
        }

        public void Caller()
        {
            _shape.Draw();
        }
    }
    public class ShapeUser2
    {
        private readonly IShape _shape;

        public ShapeUser2(IShape shape)
        {
            _shape = shape;
        }

        public void Caller()
        {
            _shape.Draw();
        }
    }

    [TestFixture]
    public class NinjectTests
    {       
        [Test]
        public void M1()
        {
            var containerConfig = new IocContainerConfiguration();
            
            // 1.Singleton
           //erviceLocator.Instance.InitialiseServiceLocator(new INinjectModule[] { containerConfig });
            var kernel = new StandardKernel(containerConfig);
            //var shape = kernel.Get<IShape>();
            
            // 2.Routine
            //var shape=ServiceLocator.Instance.GetType<IShape>();           
            //shape.Draw();

            var shapeUser=kernel.Get<ShapeUser>();
            var shapeUser2 = kernel.Get<ShapeUser2>();
            
            shapeUser.Caller();
            shapeUser2.Caller();
        }
    }

    public interface IShape
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