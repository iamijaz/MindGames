using System;
using System.Collections.Generic;
using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using Ninject.Planning.Bindings;

namespace BabyProject.NInject
{
    /// <summary>
    /// Singleton
    /// From http://www.yoda.arachsys.com/csharp/singleton.html
    /// </summary>
    public class ServiceLocator
    {

        public IKernel Kernel { get; private set; }
        private static ServiceLocator _instance;
        private static readonly object Padlock = new object();
        private Action<IKernel> _extensionPont;

        public static ServiceLocator Instance
        {
            get
            {
                lock (Padlock)
                {
                    if (_instance != null)
                    {
                        return _instance;
                    }

                    return _instance = new ServiceLocator();
                }
            }
        }

        public void InitialiseServiceLocator(INinjectModule[] ninjectModules, Action<IKernel> extensionPoint = null)
        {
            _extensionPont = extensionPoint ?? (x => { });

            Kernel = CreateKernel(ninjectModules);
        }

        public void InitialiseServiceLocator(IKernel kernel)
        {
            Kernel = kernel;
        }

        private IKernel CreateKernel(INinjectModule[] ninjectModules)
        {
            var kernel = new StandardKernel(ninjectModules);
            _extensionPont(kernel);
            return kernel;
        }

        public T GetType<T>()
        {
            return (T)Kernel.Get(typeof(T));
        }

        public bool Release(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return Kernel.Release(obj);
        }

        public T Get<T>(Func<IBindingMetadata, bool> predicate)
        {
            return Kernel.Get<T>(predicate);
        }

        public object Get(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return Kernel.Get(type);
        }

        public IEnumerable<T> GetAll<T>()
        {
            return Kernel.GetAll<T>();
        }

        //	    public T Get<T>(Func<IBindingMetadata, bool> predicate)
        //	    {
        //            Kernel.Get
        //	        return Kernel.GetAll<T>(predicate).FirstOrDefault();
        //	    }

        public IEnumerable<object> GetAll(Type t)
        {
            return Kernel.GetAll(t);
        }

        public void Register<T>(Func<IContext, T> resolver)
        {
            Kernel.Bind<T>().ToMethod(resolver);
        }
    }
}
