using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NxtRemoteControl.WindowsClient.ViewModels;

namespace NxtRemoteControl.WindowsClient
{
    public class NxtBootstrapper : Bootstrapper<ShellViewModel>
    {
        private WindsorContainer _container;

        protected override void Configure()
        {
            _container = new WindsorContainer();
            _container.Register(Classes.FromThisAssembly().Pick().WithServiceDefaultInterfaces());
            _container.Register(Component.For<IWindowManager>().Instance(new WindowManager()));
            _container.Register(Component.For<IEventAggregator>().Instance(new EventAggregator()));
        }

        protected override object GetInstance(System.Type service, string key)
        {
            return string.IsNullOrWhiteSpace(key) ? _container.Resolve(service) : _container.Resolve(key, service);
        }

        protected override IEnumerable<object> GetAllInstances(System.Type service)
        {
            return _container.ResolveAll(service).Cast<object>();
        }

        protected override void BuildUp(object instance)
        {
            _container.Release(instance);
        }
    }
}