using Microsoft.Kinect.Xaml.Controls;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Unity;
using PatientControl.Controls;
using PatientControl.Interfaces;
using PatientControl.Services;
using PatientControl.ViewModels;
using PatientControl.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace PatientControl
{
    /// <summary>
    /// Proporciona un comportamiento específico de la aplicación para complementar la clase Application predeterminada.
    /// </summary>
    sealed partial class App : Microsoft.Practices.Prism.Mvvm.MvvmAppBase
    {
        static readonly UnityContainer _container = new UnityContainer();
        /// <summary>
        /// Inicializa el objeto de aplicación Singleton.  Esta es la primera línea de código creado
        /// ejecutado y, como tal, es el equivalente lógico de main() o WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        protected override System.Threading.Tasks.Task OnInitializeAsync(IActivatedEventArgs args)
        {
            _container.RegisterType<IDialogService, DialogService>(new ContainerControlledLifetimeManager());
            _container.RegisterInstance<IEventAggregator>(new EventAggregator());
            _container.RegisterInstance<ISessionStateService>(SessionStateService);
            _container.RegisterInstance<INavigationService>(this.NavigationService);
            _container.RegisterInstance<IResourceLoader>(new ResourceLoaderAdapter(new ResourceLoader()));

            // Register child view models
            _container.RegisterType<IEjercicioPageViewModel, EjercicioPageViewModel>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IMainPageViewModel, MainPageViewModel>(new ContainerControlledLifetimeManager());
            _container.RegisterType<ILoginPageViewModel, LoginPageViewModel>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IDatosPageViewModel, DatosPageViewModel>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IRegistroUserControlViewModel, RegistroUserControlViewModel>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IMedicalInfoViewModel, MedicalInfoViewModel>(new ContainerControlledLifetimeManager());

            return System.Threading.Tasks.Task.FromResult<object>(null);
        }

        protected override object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        public enum Experiences { Main, Login, Ejercicio, Datos, Registro }

        /// <summary>
        /// Se invoca cuando la aplicación la inicia normalmente el usuario final.  Se usarán otros puntos
        /// se usará por ejemplo cuando la aplicación se inicie para abrir un archivo específico.
        /// </summary>
        /// <param name="args">Información detallada acerca de la solicitud y el proceso de inicio.</param>
        protected override System.Threading.Tasks.Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            _container.Resolve<IEventAggregator>()
                .GetEvent<Events.Logout>()
                .Publish("Debe de entrar con el nombre válido de un paciente");
            _container.Resolve<IEventAggregator>()
                .GetEvent<Events.Login>()
                .Publish("Ha entrado de forma satisfactoria");

            // Navigate to the initial page

            this.NavigationService.Navigate(Experiences.Login.ToString(), null);

            return System.Threading.Tasks.Task.FromResult<object>(null);
        }


    }
}
