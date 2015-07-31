using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using WindowsPreview.Kinect;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;
using PatientControl.Models;
using Windows.Graphics.Imaging;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Provider;
using System.IO;
using Windows.UI.Xaml;

namespace PatientControl.ViewModels
{
    public class EjercicioPageViewModel : ViewModel, Interfaces.IEjercicioPageViewModel
    {
        /// <summary>
        /// variables de la aplicación WStore
        /// </summary>
        IEventAggregator _eventAggregator;
        INavigationService _navigationService;

        /// <summary>
        /// Comandos de la IU
        /// </summary>
        public DelegateCommand IniciarCommand { get; private set; }
        public DelegateCommand PararCommand { get; private set; }
        public DelegateCommand GrabarCommand { get; private set; }
        public DelegateCommand MasCommand { get; private set; }
        public DelegateCommand MenosCommand { get; private set; }
        public DelegateCommand<string> CheckedCommand { get; private set; }
        public DelegateCommand<string> LadoCommand { get; private set; }
        public DelegateCommand<FrameworkElement> CapturaCommand { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private string _angulo = default(string);
        public string Angulo { get { return _angulo; } set { SetProperty(ref _angulo, value);} }
        private string _nombrePaciente = default(string);
        public string NombrePaciente { get { return _nombrePaciente; } set { SetProperty(ref _nombrePaciente, value); } }
        private string _title = default(string);
        public string Title { get { return _title; } set { SetProperty(ref _title, value); } }
        private bool _isSelected;
        public bool IsSelected { get { return _isSelected; } set { SetProperty(ref _isSelected, value); OnPropertyChanged("IsSelected"); } }
        private bool _on;
        public bool On { get { return _on; } set { SetProperty(ref _on, value); OnPropertyChanged("On"); } }
        private string _activado = default(string);
        public string Activado { get { return _activado; } set { SetProperty(ref _activado, value); } }
        private string _desactivado = default(string);
        public string Desactivado { get { return _desactivado; } set { SetProperty(ref _desactivado, value); } }
        private bool _derecho;
        public bool Derecho { get { return _derecho; } set { SetProperty(ref _derecho, value); OnPropertyChanged("Derecho"); } }
        private bool _izquierdo;
        public bool Izquierdo { get { return _izquierdo; } set { SetProperty(ref _izquierdo, value); OnPropertyChanged("Izquierdo"); } }
        private int _repeticion = default(int);
        public int Repeticion { get { return _repeticion; } set { SetProperty(ref _repeticion, value); } }
        private string _colores;
        public string Colores { get { return _colores; } set { SetProperty(ref _colores, value); } }
        private string _text;
        public string Text { get { return _text; } set { SetProperty(ref _text, value); } }


        /// <summary>
        /// Para identificar el ejercicio escogido y el paciente que lo está realizando
        /// </summary>
        private PacienteViewModel _paciente;
        public PacienteViewModel Paciente { get { return _paciente; } set { SetProperty(ref _paciente, value); } }
        private EjercicioViewModel _ejerSelected;
        public EjercicioViewModel EjerSelected { get { return _ejerSelected; } set { SetProperty(ref _ejerSelected, value); } }

        /// <summary>
        /// Identifica el sensor Kinect y su DPI
        /// </summary>
        /// 
        private KinectSensor kinectSensor;
        public static readonly double DPI = 96.0;

        /// <summary>
        /// Variables para capturar y almacenar los datos 
        /// del frame a color y del cuerpo que capta el sensor directamente
        /// </summary>
        private MultiSourceFrameReader reader;
        private int displayWidth;
        private int displayHeight;
        private byte[] pixels = null;
        private WriteableBitmap _Bitmap = default(WriteableBitmap);
        public WriteableBitmap Bitmap { get { return _Bitmap; } set { SetProperty(ref _Bitmap, value); OnPropertyChanged(null); } }
        private Canvas _KinectCanvas = default(Canvas);
        public Canvas KinectCanvas { get { return _KinectCanvas; } set { SetProperty(ref _KinectCanvas, value); OnPropertyChanged(null); } }
        private int bodyCount;
        private IList<Body> bodies = null;

        /// <summary>
        /// Variables globales para los algoritmos
        /// </summary>
        double AF;
        double BF;
        double CF;
        private List<double> myList = new List<double>();
        private List<double> myMedian = new List<double>();
        private double[] myAngles = new double[5];
        private double angle;
        private Boolean inicial = true;
        private double anguloInicial = 0;
        private int calibration;
        private Boolean comenzado;
        private double median;
        private int confra;

        /// <summary>
        /// Capturar posiciones previas al movimiento
        /// </summary>
        Point anteriorHombro = new Point(0,0);
        double anteriorHombroZ = 0;
        double anteriorZ = 0;
        double anteriorY = 0;
        double anteriorX = 0;


        public EjercicioPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;

            Paciente = new PacienteViewModel(new Paciente());

            IniciarCommand = DelegateCommand.FromAsyncHandler(Iniciar);
            PararCommand = DelegateCommand.FromAsyncHandler(Parar);
            GrabarCommand = DelegateCommand.FromAsyncHandler(Grabar);
            MasCommand = DelegateCommand.FromAsyncHandler(Mas);
            MenosCommand = DelegateCommand.FromAsyncHandler(Menos);
            CapturaCommand = new DelegateCommand<FrameworkElement>(Capturar);
            CheckedCommand = new DelegateCommand<string>(Checked);
            LadoCommand = new DelegateCommand<string>(Lado);
        }

        public async override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            EjerSelected = navigationParameter as EjercicioViewModel;
            await Paciente.ObtenerPacientePorId(EjerSelected.Paciente_Id.ToString());
            Title = EjerSelected.Title;
            NombrePaciente = Paciente.Nombre;

            if (Title == "Abduccion-Aduccion")
            {
                Activado = "Aduccion";
                Desactivado = "Abduccion";
            }
            else
            {
                Activado = "Extension";
                Desactivado = "Flexion";
            }

            Angulo = "";
            Repeticion = 0;

            if (Paciente.ZonaLesion.Equals("Izquierdo"))
            {
                Izquierdo = true; Derecho = false;
            }
            else { Derecho = true; Izquierdo = false; }

            KinectConect();
        }
        
        /// <summary>
        /// Inicializa el sensor Kinect
        /// </summary>
        public void KinectConect()
        {
            this.kinectSensor = KinectSensor.GetDefault();
            if (this.kinectSensor != null)
            {
                // open the reader for the color frames
                if (!this.kinectSensor.IsOpen) this.kinectSensor.Open();

                reader = this.kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Body);
            }
            if (reader != null)
            {
                reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
            }
        }

        public async override void OnNavigatedFrom(Dictionary<string, object> viewModelState, bool suspending)
        {
            var patientControlApp = App.Current as App;
            if (patientControlApp != null && !patientControlApp.IsSuspending)
            {
                await this.Parar();

                if (reader != null)
                {
                    reader.Dispose();
                    reader = null;
                }

                if (kinectSensor == null) return;

                kinectSensor.Close();
                kinectSensor = null;
            }
        }

        /// <summary>
        /// Captura frames de distintas fuentes, en este caso el cuerpo y el color.
        /// </summary>
        /// <param name="sender">objeto que envia el evento</param>
        /// <param name="e">argumentos del evento</param>
        private void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            /* TRATAMIENTO DE LOS FRAMES A COLOR */
            try
            {
                var frameReference = e.FrameReference.AcquireFrame();

                using (var colorFrame = frameReference.ColorFrameReference.AcquireFrame())
                {
                    if (colorFrame != null)
                    {
                        //Se obtiene la información del formato en el que llegan los frames
                        var colorFrameDescription = kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);
                        displayWidth = colorFrameDescription.Width;
                        displayHeight = colorFrameDescription.Height;

                        pixels = new byte[this.displayWidth * this.displayHeight * colorFrameDescription.BytesPerPixel];

                        if (Bitmap == null)
                        {
                            Bitmap = new WriteableBitmap(displayWidth, displayHeight);
                        }

                        //Se copia la imagen en el mapa de pixeles mediante un buffer intermedio
                        if (colorFrame.RawColorImageFormat == ColorImageFormat.Bgra)
                            colorFrame.CopyRawFrameDataToArray(pixels);
                        else
                            colorFrame.CopyConvertedFrameDataToArray(pixels, ColorImageFormat.Bgra);

                        Bitmap.PixelBuffer.AsStream().Write(pixels, 0, pixels.Length);
                        Bitmap.Invalidate();
                    }
                }

                /* TRATAMIENTO DE LOS FRAMES CON LA INFORMACIÓN DEL CUERPO */
                using (var frame = frameReference.BodyFrameReference.AcquireFrame())
                {
                    if (frame != null)
                    {
                        if (KinectCanvas == null)
                            KinectCanvas = new Canvas();
                        
                        KinectCanvas.Children.Clear();

                        if (bodies == null)
                            bodies = new Body[frame.BodyFrameSource.BodyCount];

                        this.bodyCount = kinectSensor.BodyFrameSource.BodyCount;
                        frame.GetAndRefreshBodyData(bodies);

                        foreach (Body body in bodies)
                        {
                            if (body.IsTracked)
                            {
                                IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
                                Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                                foreach (Joint joint in body.Joints.Values)
                                {
                                    if (joint.TrackingState == TrackingState.Tracked)
                                    {
                                        // Puntos en el espacio 3D y 2D
                                        CameraSpacePoint position = joint.Position;
                                        Point point = new Point();

                                        // Mapeo
                                        ColorSpacePoint colorPoint = kinectSensor.CoordinateMapper.MapCameraPointToColorSpace(position);
                                        point.X = ((float.IsInfinity(colorPoint.X) ? 0 : colorPoint.X) );
                                        point.Y = ((float.IsInfinity(colorPoint.Y) ? 0 : colorPoint.Y));
                                        jointPoints[joint.JointType] = point;

                                        if (IsSelected) DrawJoint(KinectCanvas, point);
                                    }

                                }

                                /* CALCULO DEL PUNTO PROYECCION DEL HOMBRO */
                                double d1 = Math.Sqrt(Math.Pow((joints[JointType.SpineBase].Position.X - joints[JointType.ShoulderRight].Position.X), 2) + Math.Pow((joints[JointType.SpineBase].Position.Y - joints[JointType.ShoulderLeft].Position.Y), 2)); ;
                                double d2 = Math.Sqrt(Math.Pow((joints[JointType.SpineBase].Position.Y - joints[JointType.ShoulderLeft].Position.Y), 2)); ;
                                double dx = Math.Sqrt(Math.Pow(d1, 2) - Math.Pow(d2, 2));
                                Point UnderShoulder = new Point();
                                double UnderShoulderZ = joints[JointType.SpineBase].Position.Z;

                                if (Izquierdo)
                                {
                                    UnderShoulder.X = joints[JointType.SpineBase].Position.X - dx;
                                    UnderShoulder.Y = joints[JointType.SpineBase].Position.Y;
                                }
                                else
                                {
                                    UnderShoulder.X = joints[JointType.SpineBase].Position.X + dx;
                                    UnderShoulder.Y = joints[JointType.SpineBase].Position.Y;
                                }

                                if (IsSelected) DrawBody(joints, jointPoints, KinectCanvas);

                                /* VARIABLES PARA EL CALCULO DEL ERRROR*/
                                double opuesto = 1;
                                double contiguo = 1;
                                double alfa = 1;
                                double dist1 = 1;
                                double angRad = 1;
                                double Error = 1;

                                /* CALCULO DE LOS ANGULOS PARA CADA EJERCICIO */
                                if (comenzado)
                                {
                                    switch (Title)
                                    {
                                        case "Abduccion-Aduccion":
                                            if (Izquierdo)
                                            {
                                                /* FIJAMOS POSICIONES */
                                                if (anteriorZ == 0 && !inicial) anteriorZ = joints[JointType.ElbowLeft].Position.Z;
                                                if (anteriorHombro.X == 0 && !inicial) anteriorHombro.X = joints[JointType.ShoulderLeft].Position.X;
                                                if (anteriorHombro.Y == 0 && !inicial) anteriorHombro.Y = joints[JointType.ShoulderLeft].Position.Y;
                                                if (anteriorHombroZ == 0 && !inicial) anteriorHombroZ = joints[JointType.ShoulderLeft].Position.Z;

                                                /* EL MOVIMIENTO HA DE SER DENTRO DE UN RANGO LIMITADO */
                                                if ((anteriorZ - 0.15 <= joints[JointType.ElbowLeft].Position.Z) && (joints[JointType.ElbowLeft].Position.Z <= anteriorZ + 0.15))
                                                {
                                                    // Calculo del error
                                                    dist1 = Math.Abs(((joints[JointType.ShoulderLeft].Position.X - joints[JointType.ShoulderRight].Position.X) * (anteriorHombro.X - joints[JointType.ShoulderRight].Position.X)) + ((joints[JointType.ShoulderLeft].Position.Y - joints[JointType.ShoulderRight].Position.Y) * (anteriorHombro.Y - joints[JointType.ShoulderRight].Position.Y)) + ((joints[JointType.ShoulderLeft].Position.Z - joints[JointType.ShoulderRight].Position.Z) * (anteriorHombroZ - joints[JointType.ShoulderRight].Position.Z)));
                                                    angRad = Math.Acos(dist1 / (Math.Sqrt(Math.Pow(joints[JointType.ShoulderLeft].Position.X - joints[JointType.ShoulderRight].Position.X, 2) + Math.Pow(joints[JointType.ShoulderLeft].Position.Y - joints[JointType.ShoulderRight].Position.Y, 2) + Math.Pow(joints[JointType.ShoulderLeft].Position.Z - joints[JointType.ShoulderRight].Position.Z, 2))) * Math.Sqrt(Math.Pow(anteriorHombro.X - joints[JointType.ShoulderRight].Position.X, 2) + Math.Pow(anteriorHombro.Y - joints[JointType.ShoulderRight].Position.Y, 2) + Math.Pow(anteriorHombroZ - joints[JointType.ShoulderRight].Position.Z, 2)));
                                                    Error = Math.Round(angRad * (180.0 / Math.PI), 0);

                                                    // Aduccion
                                                    if (On && joints[JointType.ElbowLeft].Position.X >= joints[JointType.ShoulderLeft].Position.X && joints[JointType.ElbowLeft].Position.Y < joints[JointType.ShoulderLeft].Position.Y)
                                                    {
                                                        // Calculamos el angulo de otra forma aplicando tambien otro error
                                                        UnderShoulderZ = joints[JointType.ShoulderLeft].Position.Z;
                                                        opuesto = Math.Sqrt(Math.Pow(joints[JointType.ShoulderLeft].Position.X - anteriorHombro.X, 2) + Math.Pow(joints[JointType.ShoulderLeft].Position.Y - anteriorHombro.Y, 2) + Math.Pow(joints[JointType.ShoulderLeft].Position.Z - anteriorHombroZ, 2));
                                                        contiguo = Math.Sqrt(Math.Pow(UnderShoulder.X - anteriorHombro.X, 2) + Math.Pow(UnderShoulder.Y - anteriorHombro.Y, 2) + Math.Pow(UnderShoulderZ - anteriorHombroZ, 2));
                                                        alfa = Math.Round(Math.Asin(opuesto / contiguo) * (180.0 / Math.PI), 0);
                                                        angle = (-1) * (AngleBetweenJoints(joints[JointType.ElbowLeft], joints[JointType.ShoulderLeft], UnderShoulder, UnderShoulderZ) - alfa);
                                                    }
                                                    else // Abduccion
                                                    {
                                                        // Para la calibracion es necesario realizar un calculo inicial sin tener en cuenta el error y el valor almacenado del hombro
                                                        angle = AngleBetweenJoints(joints[JointType.ElbowLeft], joints[JointType.ShoulderLeft], UnderShoulder, UnderShoulderZ);
                                                        if (!inicial)
                                                            angle = AngleBetweenJoints(joints[JointType.ElbowLeft], anteriorHombro, anteriorHombroZ, UnderShoulder, UnderShoulderZ) + (Error - 85);
                                                    }
                                                }
                                            }
                                            if (Derecho)
                                            {
                                                if (anteriorZ == 0) anteriorZ = joints[JointType.ElbowRight].Position.Z;
                                                if (anteriorHombro.X == 0 && !inicial) anteriorHombro.X = joints[JointType.ShoulderRight].Position.X;
                                                if (anteriorHombro.Y == 0 && !inicial) anteriorHombro.Y = joints[JointType.ShoulderRight].Position.Y;
                                                if (anteriorHombroZ == 0 && !inicial) anteriorHombroZ = joints[JointType.ShoulderRight].Position.Z;

                                                if ((anteriorZ - 0.15 <= joints[JointType.ElbowRight].Position.Z) && (joints[JointType.ElbowRight].Position.Z <= anteriorZ + 0.15))
                                                {
                                                    dist1 = Math.Abs(((joints[JointType.ShoulderRight].Position.X - joints[JointType.ShoulderLeft].Position.X) * (anteriorHombro.X - joints[JointType.ShoulderLeft].Position.X)) + ((joints[JointType.ShoulderRight].Position.Y - joints[JointType.ShoulderLeft].Position.Y) * (anteriorHombro.Y - joints[JointType.ShoulderLeft].Position.Y)) + ((joints[JointType.ShoulderLeft].Position.Z - joints[JointType.ShoulderLeft].Position.Z) * (anteriorHombroZ - joints[JointType.ShoulderLeft].Position.Z)));
                                                    angRad = Math.Acos(dist1 / (Math.Sqrt(Math.Pow(joints[JointType.ShoulderRight].Position.X - joints[JointType.ShoulderLeft].Position.X, 2) + Math.Pow(joints[JointType.ShoulderRight].Position.Y - joints[JointType.ShoulderLeft].Position.Y, 2) + Math.Pow(joints[JointType.ShoulderRight].Position.Z - joints[JointType.ShoulderLeft].Position.Z, 2))) * Math.Sqrt(Math.Pow(anteriorHombro.X - joints[JointType.ShoulderLeft].Position.X, 2) + Math.Pow(anteriorHombro.Y - joints[JointType.ShoulderLeft].Position.Y, 2) + Math.Pow(anteriorHombroZ - joints[JointType.ShoulderLeft].Position.Z, 2)));
                                                    Error = Math.Round(angRad * (180.0 / Math.PI), 0);

                                                    if (On && joints[JointType.ElbowRight].Position.X <= joints[JointType.ShoulderRight].Position.X && joints[JointType.ElbowRight].Position.Y < joints[JointType.ShoulderRight].Position.Y)
                                                    {
                                                        UnderShoulderZ = joints[JointType.ShoulderRight].Position.Z;
                                                        opuesto = Math.Sqrt(Math.Pow(joints[JointType.ShoulderRight].Position.X - anteriorHombro.X, 2) + Math.Pow(joints[JointType.ShoulderRight].Position.Y - anteriorHombro.Y, 2) + Math.Pow(joints[JointType.ShoulderRight].Position.Z - anteriorHombroZ, 2));
                                                        contiguo = Math.Sqrt(Math.Pow(UnderShoulder.X - anteriorHombro.X, 2) + Math.Pow(UnderShoulder.Y - anteriorHombro.Y, 2) + Math.Pow(UnderShoulderZ - anteriorHombroZ, 2));
                                                        alfa = Math.Round(Math.Asin(opuesto / contiguo) * (180.0 / Math.PI), 0);
                                                        angle = (-1) * (AngleBetweenJoints(joints[JointType.ElbowRight], joints[JointType.ShoulderRight], UnderShoulder, UnderShoulderZ) - alfa);
                                                    }
                                                    else
                                                    {
                                                        angle = AngleBetweenJoints(joints[JointType.ElbowRight], joints[JointType.ShoulderRight], UnderShoulder, UnderShoulderZ);
                                                        if (!inicial)
                                                            angle = AngleBetweenJoints(joints[JointType.ElbowRight], anteriorHombro, anteriorHombroZ, UnderShoulder, UnderShoulderZ) + (Error - 85);
                                                    }
                                                }
                                            }
                                            break;
                                        case "FlexoExtension":
                                            if (Izquierdo)
                                            {
                                                if (anteriorX == 0) anteriorX = joints[JointType.ElbowLeft].Position.X;
                                                if (anteriorHombro.X == 0 && !inicial) anteriorHombro.X = joints[JointType.ShoulderLeft].Position.X;
                                                if (anteriorHombro.Y == 0 && !inicial) anteriorHombro.Y = joints[JointType.ShoulderLeft].Position.Y;
                                                if (anteriorHombroZ == 0 && !inicial) anteriorHombroZ = joints[JointType.ShoulderLeft].Position.Z;

                                                if ((anteriorX - 0.15 <= joints[JointType.ElbowLeft].Position.X) && (joints[JointType.ElbowLeft].Position.X <= anteriorX + 0.15))
                                                {
                                                    dist1 = Math.Abs(((joints[JointType.ShoulderLeft].Position.X - joints[JointType.ShoulderRight].Position.X) * (anteriorHombro.X - joints[JointType.ShoulderRight].Position.X)) + ((joints[JointType.ShoulderLeft].Position.Y - joints[JointType.ShoulderRight].Position.Y) * (anteriorHombro.Y - joints[JointType.ShoulderRight].Position.Y)) + ((joints[JointType.ShoulderLeft].Position.Z - joints[JointType.ShoulderRight].Position.Z) * (anteriorHombroZ - joints[JointType.ShoulderRight].Position.Z)));
                                                    angRad = Math.Acos(dist1 / (Math.Sqrt(Math.Pow(joints[JointType.ShoulderLeft].Position.X - joints[JointType.ShoulderRight].Position.X, 2) + Math.Pow(joints[JointType.ShoulderLeft].Position.Y - joints[JointType.ShoulderRight].Position.Y, 2) + Math.Pow(joints[JointType.ShoulderLeft].Position.Z - joints[JointType.ShoulderRight].Position.Z, 2))) * Math.Sqrt(Math.Pow(anteriorHombro.X - joints[JointType.ShoulderRight].Position.X, 2) + Math.Pow(anteriorHombro.Y - joints[JointType.ShoulderRight].Position.Y, 2) + Math.Pow(anteriorHombroZ - joints[JointType.ShoulderRight].Position.Z, 2)));
                                                    Error = Math.Round(angRad * (180.0 / Math.PI), 0);

                                                    if (On && joints[JointType.ElbowLeft].Position.Z >= joints[JointType.ShoulderLeft].Position.Z + 0.05 && joints[JointType.ElbowLeft].Position.Y < joints[JointType.ShoulderLeft].Position.Y)
                                                        angle = (-1) * AngleBetweenJoints(joints[JointType.ElbowLeft], joints[JointType.ShoulderLeft], UnderShoulder, UnderShoulderZ);
                                                    else
                                                    {
                                                        angle = AngleBetweenJoints(joints[JointType.ElbowLeft], joints[JointType.ShoulderLeft], UnderShoulder, UnderShoulderZ);
                                                        if (!inicial)
                                                            angle = AngleBetweenJoints(joints[JointType.ElbowLeft], anteriorHombro, anteriorHombroZ, UnderShoulder, UnderShoulderZ) + (Error - 85);
                                                    }
                                                }
                                            }
                                            if (Derecho)
                                            {
                                                if (anteriorX == 0) anteriorX = joints[JointType.ElbowRight].Position.X;
                                                if (anteriorHombro.X == 0 && !inicial) anteriorHombro.X = joints[JointType.ShoulderRight].Position.X;
                                                if (anteriorHombro.Y == 0 && !inicial) anteriorHombro.Y = joints[JointType.ShoulderRight].Position.Y;
                                                if (anteriorHombroZ == 0 && !inicial) anteriorHombroZ = joints[JointType.ShoulderRight].Position.Z;

                                                if ((anteriorX - 0.15 <= joints[JointType.ElbowRight].Position.X) && (joints[JointType.ElbowRight].Position.X <= anteriorX + 0.15))
                                                {
                                                    dist1 = Math.Abs(((joints[JointType.ShoulderRight].Position.X - joints[JointType.ShoulderLeft].Position.X) * (anteriorHombro.X - joints[JointType.ShoulderLeft].Position.X)) + ((joints[JointType.ShoulderRight].Position.Y - joints[JointType.ShoulderLeft].Position.Y) * (anteriorHombro.Y - joints[JointType.ShoulderLeft].Position.Y)) + ((joints[JointType.ShoulderLeft].Position.Z - joints[JointType.ShoulderLeft].Position.Z) * (anteriorHombroZ - joints[JointType.ShoulderLeft].Position.Z)));
                                                    angRad = Math.Acos(dist1 / (Math.Sqrt(Math.Pow(joints[JointType.ShoulderRight].Position.X - joints[JointType.ShoulderLeft].Position.X, 2) + Math.Pow(joints[JointType.ShoulderRight].Position.Y - joints[JointType.ShoulderLeft].Position.Y, 2) + Math.Pow(joints[JointType.ShoulderRight].Position.Z - joints[JointType.ShoulderLeft].Position.Z, 2))) * Math.Sqrt(Math.Pow(anteriorHombro.X - joints[JointType.ShoulderLeft].Position.X, 2) + Math.Pow(anteriorHombro.Y - joints[JointType.ShoulderLeft].Position.Y, 2) + Math.Pow(anteriorHombroZ - joints[JointType.ShoulderLeft].Position.Z, 2)));
                                                    Error = Math.Round(angRad * (180.0 / Math.PI), 0);

                                                    if (On && joints[JointType.ElbowRight].Position.Z >= joints[JointType.ShoulderRight].Position.Z + 0.05 && joints[JointType.ElbowRight].Position.Y < joints[JointType.ShoulderRight].Position.Y)
                                                        angle = (-1) * AngleBetweenJoints(joints[JointType.ElbowRight], joints[JointType.ShoulderRight], UnderShoulder, UnderShoulderZ);
                                                    else
                                                    {
                                                        angle = AngleBetweenJoints(joints[JointType.ElbowRight], joints[JointType.ShoulderRight], UnderShoulder, UnderShoulderZ);
                                                        if (!inicial)
                                                            angle = AngleBetweenJoints(joints[JointType.ElbowRight], anteriorHombro, anteriorHombroZ, UnderShoulder, UnderShoulderZ) + (Error - 85);
                                                    }
                                                }
                                            }
                                            break;
                                        case "FlexExHorizontal":
                                            this.Activado = "Extension Horizontal";
                                            this.Desactivado = "Flexion Horizontal";

                                            if (Izquierdo)
                                            {
                                                if (anteriorY == 0) anteriorY = joints[JointType.ElbowLeft].Position.Y;
                                                if (anteriorHombro.X == 0 && !inicial) anteriorHombro.X = joints[JointType.ShoulderLeft].Position.X;
                                                if (anteriorHombro.Y == 0 && !inicial) anteriorHombro.Y = joints[JointType.ShoulderLeft].Position.Y;
                                                if (anteriorHombroZ == 0 && !inicial) anteriorHombroZ = joints[JointType.ShoulderLeft].Position.Z;

                                                if ((anteriorY - 0.1 <= joints[JointType.ElbowLeft].Position.Y) && (joints[JointType.ElbowLeft].Position.Y <= anteriorY + 0.1))
                                                {
                                                    if (joints[JointType.ElbowLeft].Position.X > (joints[JointType.ShoulderLeft].Position.X + 0.02) || joints[JointType.ElbowLeft].Position.X > (anteriorHombro.X - 0.02))
                                                        angle = 90 + CalcularAnguloAlterno(joints[JointType.ElbowLeft], anteriorHombro, anteriorHombroZ, joints[JointType.ShoulderRight], UnderShoulder, UnderShoulderZ) - 5;
                                                    else
                                                    {
                                                        angle = 90 - CalcularAnguloAlterno(joints[JointType.ElbowLeft], joints[JointType.ShoulderLeft], joints[JointType.ShoulderRight], UnderShoulder, UnderShoulderZ);
                                                        if (!inicial)
                                                            angle = 90 - CalcularAnguloAlterno(joints[JointType.ElbowLeft], anteriorHombro, anteriorHombroZ, joints[JointType.ShoulderRight], UnderShoulder, UnderShoulderZ) - 5;

                                                        if (On && !inicial && joints[JointType.ElbowLeft].Position.Z >= joints[JointType.ShoulderLeft].Position.Z)
                                                            angle = (-1) * angle;
                                                    }
                                                }
                                            }
                                            if (Derecho)
                                            {
                                                if (anteriorY == 0) anteriorY = joints[JointType.ElbowRight].Position.Y;
                                                if (anteriorHombro.X == 0 && !inicial) anteriorHombro.X = joints[JointType.ShoulderRight].Position.X;
                                                if (anteriorHombro.Y == 0 && !inicial) anteriorHombro.Y = joints[JointType.ShoulderRight].Position.Y;
                                                if (anteriorHombroZ == 0 && !inicial) anteriorHombroZ = joints[JointType.ShoulderRight].Position.Z;

                                                if ((anteriorY - 0.1 <= joints[JointType.ElbowRight].Position.Y) && (joints[JointType.ElbowRight].Position.Y <= anteriorY + 0.1))
                                                {
                                                    if (joints[JointType.ElbowRight].Position.X < (joints[JointType.ShoulderRight].Position.X - 0.02) || joints[JointType.ElbowRight].Position.X < (anteriorHombro.X - 0.02))
                                                    {
                                                        angle = 90 + CalcularAnguloAlterno(joints[JointType.ElbowRight], anteriorHombro, anteriorHombroZ, joints[JointType.ShoulderLeft], UnderShoulder, UnderShoulderZ) - 3;
                                                    }
                                                    else
                                                    {
                                                        angle = 90 - CalcularAnguloAlterno(joints[JointType.ElbowRight], joints[JointType.ShoulderRight], joints[JointType.ShoulderLeft], UnderShoulder, UnderShoulderZ);
                                                        if (!inicial)
                                                            angle = 90 - CalcularAnguloAlterno(joints[JointType.ElbowRight], anteriorHombro, anteriorHombroZ, joints[JointType.ShoulderLeft], UnderShoulder, UnderShoulderZ) - 3;

                                                        if (On && !inicial && joints[JointType.ElbowRight].Position.Z >= joints[JointType.ShoulderRight].Position.Z)// && joints[JointType.ElbowLeft].Position.X <= anteriorX)
                                                            angle = (-1) * angle;
                                                    }
                                                }
                                            }
                                            break;
                                        case "CodoFlexEx":
                                            if (Izquierdo)
                                                angle = 180 - AngleBetweenJoints(joints[JointType.WristLeft], joints[JointType.ElbowLeft], joints[JointType.ShoulderLeft]);
                                            if (Derecho)
                                                angle = 180 - AngleBetweenJoints(joints[JointType.WristRight], joints[JointType.ElbowRight], joints[JointType.ShoulderRight]);
                                            break;
                                    }

                                    /* SE TRATAN LOS ANGULOS CAPTURADOS EN CADA FRAME PARA PODER MOSTRARLES */
                                    myList.Add(angle);

                                    if (confra == 15)
                                    {
                                        CalculateMedian(myList, out median);
                                        myList.CopyTo((myList.Count - 5), myAngles, 0, 5);
                                        myList.Clear();
                                        myList.AddRange(myAngles);
                                        myMedian.Add(median);
                                        WriteAngle(median, inicial);
                                    }

                                    if (confra == 30)
                                    {
                                        CalculateMedian(myMedian, out median);
                                        if (calibration == 3 && inicial)
                                        {
                                            calibration = 0;
                                            inicial = false;
                                        }
                                        if (anguloInicial != 0 && anguloInicial == median && inicial)
                                            calibration++;
                                        else calibration = 0;
                                        anguloInicial = median;
                                        if (!inicial)
                                            WriteAngle(median, inicial);
                                        myMedian.Clear();
                                        confra = 0;
                                    }
                                    confra++;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Dibuja las articulaciones detectadas por Kinect
        /// </summary>
        /// <param name="canvas">canvas donde se dibujaran las articulaciones</param>
        /// <param name="point">posicion 2D de las articulaciones</param>
        public void DrawJoint(Canvas canvas, Point point)
        {
            Ellipse ellipse = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = new SolidColorBrush(Colors.LightBlue)
            };

            Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);

            // Se añade al canvas
            canvas.Children.Add(ellipse);
        }

        /// <summary>
        /// Dibuja un cuerpo
        /// </summary>
        /// <param name="joints">articulaciones a dibujar</param>
        /// <param name="jointPoints">posicion de la articulacion a dibujar</param>
        /// <param name="c">canvas donde se dibujara el cuerpo</param>
        private void DrawBody(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, Canvas c)
        {

            // Torso
            this.DrawBone(joints, jointPoints, JointType.Head, JointType.Neck, c);
            this.DrawBone(joints, jointPoints, JointType.Neck, JointType.SpineShoulder, c);
            this.DrawBone(joints, jointPoints, JointType.SpineShoulder, JointType.SpineMid, c);
            this.DrawBone(joints, jointPoints, JointType.SpineMid, JointType.SpineBase, c);
            this.DrawBone(joints, jointPoints, JointType.SpineShoulder, JointType.ShoulderRight, c);
            this.DrawBone(joints, jointPoints, JointType.SpineShoulder, JointType.ShoulderLeft, c);
            this.DrawBone(joints, jointPoints, JointType.SpineBase, JointType.HipRight, c);
            this.DrawBone(joints, jointPoints, JointType.SpineBase, JointType.HipLeft, c);

            // Brazo derecho 
            this.DrawBone(joints, jointPoints, JointType.ShoulderRight, JointType.ElbowRight, c);
            this.DrawBone(joints, jointPoints, JointType.ElbowRight, JointType.WristRight, c);
            this.DrawBone(joints, jointPoints, JointType.WristRight, JointType.HandRight, c);
            this.DrawBone(joints, jointPoints, JointType.HandRight, JointType.HandTipRight, c);
            this.DrawBone(joints, jointPoints, JointType.WristRight, JointType.ThumbRight, c);

            // Brazo izquierdo
            this.DrawBone(joints, jointPoints, JointType.ShoulderLeft, JointType.ElbowLeft, c);
            this.DrawBone(joints, jointPoints, JointType.ElbowLeft, JointType.WristLeft, c);
            this.DrawBone(joints, jointPoints, JointType.WristLeft, JointType.HandLeft, c);
            this.DrawBone(joints, jointPoints, JointType.HandLeft, JointType.HandTipLeft, c);
            this.DrawBone(joints, jointPoints, JointType.WristLeft, JointType.ThumbLeft, c);

            // Pierna derecha
            this.DrawBone(joints, jointPoints, JointType.HipRight, JointType.KneeRight, c);
            this.DrawBone(joints, jointPoints, JointType.KneeRight, JointType.AnkleRight, c);
            this.DrawBone(joints, jointPoints, JointType.AnkleRight, JointType.FootRight, c);

            // Pierna izquierda
            this.DrawBone(joints, jointPoints, JointType.HipLeft, JointType.KneeLeft, c);
            this.DrawBone(joints, jointPoints, JointType.KneeLeft, JointType.AnkleLeft, c);
            this.DrawBone(joints, jointPoints, JointType.AnkleLeft, JointType.FootLeft, c);
        }

        /// <summary>
        /// Dibuja cada uno de los huesos del cuerpo (une las articulaciones)
        /// </summary>
        /// <param name="joints">articulaciones a unir</param>
        /// <param name="jointPoints">posicion de las articulaciones</param>
        /// <param name="jointType0">primera articulacion del hueso a dibujar</param>
        /// <param name="jointType1">segunda articulacion del hueso</param>
        /// <param name="c">canvas donde se dibuja</param>
        private void DrawBone(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, Canvas c)
        {
            Joint joint0 = joints[jointType0];
            Joint joint1 = joints[jointType1];

            if (joint0.TrackingState == TrackingState.NotTracked ||
                joint1.TrackingState == TrackingState.NotTracked)
            {
                return;
            }

            if ((joint0.TrackingState == TrackingState.Tracked) && 
                (joint1.TrackingState == TrackingState.Tracked))
            {
                Line line = new Line
                {
                    X1 = (float)jointPoints[jointType0].X,
                    Y1 = (float)jointPoints[jointType0].Y,
                    X2 = (float)jointPoints[jointType1].X,
                    Y2 = (float)jointPoints[jointType1].Y,
                    StrokeThickness = 5,
                    Stroke = new SolidColorBrush(Colors.LightBlue)
                };
                c.Children.Add(line);
            }
        }

        /// <summary>
        /// Regresa el ángulo interno dadas 3 articulaciones
        /// Se emplea calculo de un angulo mediante los vectores que las unen
        /// Se emplea la proyeccion del hombro
        /// </summary>
        /// <param name="j1"></param>
        /// <param name="j2"></param>
        /// <param name="j3"></param>
        /// <param name="j3Z"></param>
        /// <returns></returns>
        public static double AngleBetweenJoints(Joint j1, Joint j2, Point j3, double j3Z)
        {
            double angulo = 0;
            double shrhX = j1.Position.X - j2.Position.X;
            double shrhY = j1.Position.Y - j2.Position.Y;
            double shrhZ = j1.Position.Z - j2.Position.Z;
            double hsl = vectorNorm(shrhX, shrhY, shrhZ);
            double unrhX = j3.X - j2.Position.X;
            double unrhY = j3.Y - j2.Position.Y;
            double unrhZ = j3Z - j2.Position.Z;
            double hul = vectorNorm(unrhX, unrhY, unrhZ);
            double mhshu = shrhX * unrhX + shrhY * unrhY + shrhZ * unrhZ;
            double x = mhshu / (hul * hsl);
            if (x != Double.NaN)
            {
                if (-1 <= x && x <= 1)
                {
                    double angleRad = Math.Acos(x);
                    angulo = angleRad * (180.0 / Math.PI);
                }
                else
                    angulo = 0;
            }
            else
                angulo = 0;
            return Math.Round(angulo, 0);
        }
        
        /// <summary>
        /// Regresa el ángulo interno dadas 3 articulaciones
        /// Se emplea calculo de un angulo mediante los vectores que las unen
        /// Se emplea la proyeccion del hombro y una posicion fija
        /// </summary>
        /// <param name="j1"></param>
        /// <param name="j2"></param>
        /// <param name="j3"></param>
        /// <returns></returns>
        public static double AngleBetweenJoints(Joint j1, Point j2, double j2Z, Point j3, double j3Z)
        {
            double angulo = 0;
            double shrhX = j1.Position.X - j2.X;
            double shrhY = j1.Position.Y - j2.Y;
            double shrhZ = j1.Position.Z - j2Z;
            double hsl = vectorNorm(shrhX, shrhY, shrhZ);
            double unrhX = j3.X - j2.X;
            double unrhY = j3.Y - j2.Y;
            double unrhZ = j3Z - j2Z;
            double hul = vectorNorm(unrhX, unrhY, unrhZ);
            double mhshu = shrhX * unrhX + shrhY * unrhY + shrhZ * unrhZ;
            double x = mhshu / (hul * hsl);
            if (x != Double.NaN)
            {
                if (-1 <= x && x <= 1)
                {
                    double angleRad = Math.Acos(x);
                    angulo = angleRad * (180.0 / Math.PI);
                }
                else
                    angulo = 0;
            }
            else
                angulo = 0;
            return Math.Round(angulo, 0);
        }
        
        
        /// <summary>
        /// Regresa el ángulo interno dadas 3 articulaciones
        /// Se emplea calculo de un angulo mediante los vectores que las unen
        /// </summary>
        /// <param name="j1"></param>
        /// <param name="j2"></param>
        /// <param name="j3"></param>
        /// <returns></returns>
        public static double AngleBetweenJoints(Joint j1, Joint j2, Joint j3)
        {
            double Angulo = 0;
            double shrhX = j1.Position.X - j2.Position.X;
            double shrhY = j1.Position.Y - j2.Position.Y;
            double shrhZ = j1.Position.Z - j2.Position.Z;
            double hsl = vectorNorm(shrhX, shrhY, shrhZ);
            double unrhX = j3.Position.X - j2.Position.X;
            double unrhY = j3.Position.Y - j2.Position.Y;
            double unrhZ = j3.Position.Z - j2.Position.Z;
            double hul = vectorNorm(unrhX, unrhY, unrhZ);
            double mhshu = shrhX * unrhX + shrhY * unrhY + shrhZ * unrhZ;
            double x = mhshu / (hul * hsl);
            if (x != Double.NaN)
            {
                if (-1 <= x && x <= 1)
                {
                    double angleRad = Math.Acos(x);
                    Angulo = angleRad * (180.0 / Math.PI);
                }
                else
                    Angulo = 0;
            }
            else
                Angulo = 0;
            return Math.Round(Angulo, 0);
        }
        
        /// <summary>
        /// Norma euclidea de los 3 componentes de un vector
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private static double vectorNorm(double x, double y, double z)
        {
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
        }

        /// <summary>
        /// Muestra por pantalla el valor del angulo
        /// </summary>
        /// <param name="p">valor del angulo</param>
        /// <param name="correcto">determina el color del valor mostrado</param>
        private void WriteAngle(double p, bool correcto)
        {
            if (correcto) this.Colores = "Red";
            else
            this.Colores = "White";
            this.Angulo = p.ToString();
        }

        /// <summary>
        /// Calcula la media de una lista de valores
        /// </summary>
        /// <param name="myList"></param>
        /// <param name="median"></param>
        private void CalculateMedian(List<double> myList, out double median)
        {
            var query = from numbers in myList
                        orderby numbers ascending
                        select numbers;

            if (myList.Count % 2 == 0)
            { 
                //es par
                int element = myList.Count / 2; ;
                median = (double)((query.ElementAt(element - 1) + query.ElementAt(element)) / 2);
            }
            else
            {
                //es impar
                double element = (double)myList.Count / 2;
                element = Math.Round(element, MidpointRounding.AwayFromZero);
                median = (double)query.ElementAt((int)(element - 1));
            }
        }
        
        /// <summary>
        /// Calcula el angulo par la Flexion y Extension Horizontal
        /// En este caso se emplea tambien la proyeccion del punto.
        /// Es necesario para la calibracion inicial
        /// </summary>
        /// <param name="c"></param>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="p"></param>
        /// <param name="pZ"></param>
        /// <returns></returns>
        private double CalcularAnguloAlterno(Joint c, Joint h, Joint s, Point p, double pZ)
        {
            double Angulo = 0;
            bool calculado = false;

            if (!calculado)
            {
                AF = ((s.Position.Y * pZ) - (s.Position.Y * h.Position.Z) - (h.Position.Y * pZ) - (s.Position.Z * p.Y) + (s.Position.Z * h.Position.Y) + (h.Position.Z * p.Y));
                BF = ((s.Position.Z * p.X) - (s.Position.Z * h.Position.X) - (h.Position.Z * p.X) - (s.Position.X * pZ) + (s.Position.X * h.Position.Z) + (h.Position.X * pZ));
                CF = ((s.Position.X * p.Y) - (s.Position.X * h.Position.Y) - (h.Position.X * p.Y) - (s.Position.Y * p.X) + (s.Position.Y * h.Position.X) + (h.Position.Y * p.X));
                calculado = true;
            }

            double u1 = c.Position.X - h.Position.X;
            double u2 = c.Position.Y - h.Position.Y;
            double u3 = c.Position.Z - h.Position.Z;

            double A = (CF * (p.Y - h.Position.Y)) - (BF * (pZ - h.Position.Z));
            double B = (AF * (pZ - h.Position.Z)) - (CF * (p.X - h.Position.X));
            double C = (BF * (p.X - h.Position.X)) - (AF * (p.Y - h.Position.Y));

            double x = Math.Abs((A * u1) + (B * u2) + (C * u3)) / (vectorNorm(A, B, C) * vectorNorm(u1, u2, u3));

            if (x != Double.NaN)
            {
                if (-1 <= x && x <= 1)
                {
                    double angleRad = Math.Asin(x);
                    Angulo = angleRad * (180.0 / Math.PI);
                }
                else
                    Angulo = 0;
            }
            else
                Angulo = 0;
            return Math.Round(Angulo, 0);
        }

        /// <summary>
        /// Calcula el angulo par la Flexion y Extension Horizontal
        /// En este caso se emplea tambien la proyeccion del punto y ademas la posicion fija del hombro
        /// para evitar que se pierda el valor al alcanzar 90º
        /// </summary>
        /// <param name="c"></param>
        /// <param name="h"></param>
        /// <param name="hZ"></param>
        /// <param name="s"></param>
        /// <param name="p"></param>
        /// <param name="pZ"></param>
        /// <returns></returns>
        private double CalcularAnguloAlterno(Joint c, Point h, double hZ, Joint s, Point p, double pZ)
        {
            double Angulo = 0;
            bool calculado = false;

            if (!calculado)
            {
                AF = ((s.Position.Y * pZ) - (s.Position.Y * hZ) - (h.Y * pZ) - (s.Position.Z * p.Y) + (s.Position.Z * h.Y) + (hZ * p.Y));
                BF = ((s.Position.Z * p.X) - (s.Position.Z * h.X) - (hZ * p.X) - (s.Position.X * pZ) + (s.Position.X * hZ) + (h.X * pZ));
                CF = ((s.Position.X * p.Y) - (s.Position.X * h.Y) - (h.X * p.Y) - (s.Position.Y * p.X) + (s.Position.Y * h.X) + (h.Y * p.X));
                calculado = true;
            }

            double u1 = c.Position.X - h.X;
            double u2 = c.Position.Y - h.Y;
            double u3 = c.Position.Z - hZ;

            double A = (CF * (p.Y - h.Y)) - (BF * (pZ - hZ));
            double B = (AF * (pZ - hZ)) - (CF * (p.X - h.X));
            double C = (BF * (p.X - h.X)) - (AF * (p.Y - h.Y));

            double x = Math.Abs((A * u1) + (B * u2) + (C * u3)) / (vectorNorm(A, B, C) * vectorNorm(u1, u2, u3));

            if (x != Double.NaN)
            {
                if (-1 <= x && x <= 1)
                {
                    double angleRad = Math.Asin(x);
                    Angulo = angleRad * (180.0 / Math.PI);
                }
                else
                    Angulo = 0;
            }
            else
                Angulo = 0;
            return Math.Round(Angulo, 0);
        }

        /* METODOS DE LOS COMANDOS */

        private async Task Iniciar()
        {
            comenzado = true;
        }

        private async Task Parar()
        {
            comenzado = false;
            inicial = true;
            if (myList != null) myList.Clear();
            if (this.Angulo != null)
                this.Angulo = "";
            confra = 0;
            anteriorX = anteriorY = anteriorZ = 0;
            anteriorHombro.X = anteriorHombro.Y = anteriorHombroZ = 0;
        }

        private async Task Mas()
        {
            Repeticion++;
        }

        private async Task Menos()
        {
            if (Repeticion != 0) Repeticion--;
        }

        private async Task Grabar()
        {
            if (this.EjerSelected != null & !(this.Angulo == null | this.Angulo.Equals("") | this.Angulo.Equals(" ")))
            {
                EjerSelected.Angulo = Angulo;
                EjerSelected.Repeticiones = Repeticion;
                if (Izquierdo)
                        EjerSelected.Lado = "Izquierdo";
                    else
                        EjerSelected.Lado = "Derecho";
                    if (On)
                        EjerSelected.Tipo = Activado + " " + EjerSelected.Articulacion + " " + EjerSelected.Lado;
                    else
                        EjerSelected.Tipo = Desactivado + " " + EjerSelected.Articulacion + " " + EjerSelected.Lado;
                    EjerSelected.FechaRealizado = new DateTime();
                    EjerSelected.FechaRealizado = DateTime.UtcNow.AddHours(2);
                await EjerSelected.InsertarNuevoEjercicio();
            }
        }

        private void Checked(object parameter)
        {
            if (parameter.ToString() == "Activado")
            {
                On = true;
            }
            else
            {
                On = false;
            }
        }

        private void Lado(object parameter)
        {
            if (parameter.ToString() == "Izquierdo")
            {
                Izquierdo = true;
                Derecho = false;
            }
            else
            {
                Izquierdo = false;
                Derecho = true;
            }
            anteriorHombro.X = anteriorHombro.Y = anteriorHombroZ = anteriorX = anteriorY = anteriorZ = 0;
        }

        private async void Capturar(FrameworkElement element)
        {
            if (Bitmap != null)
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add("Imagen", new List<string>() { ".png"});
                // Default file name if the user does not type one in or select a file to replace
                savePicker.SuggestedFileName = "ImagenMedida";

                StorageFile file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    CachedFileManager.DeferUpdates(file);

                    await FileIO.WriteTextAsync(file, file.Name);

                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                    if (status == FileUpdateStatus.Complete)
                    {
                        Text = "Captura correcta.";
                    }
                    else
                    {
                        Text = "No se pudo capturar";
                    }
                    using (var destFileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        var renderTargetBitmap = new Windows.UI.Xaml.Media.Imaging.RenderTargetBitmap();
                        await renderTargetBitmap.RenderAsync(element);
                        var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();
                        var dpi = Windows.Graphics.Display.DisplayInformation.GetForCurrentView().LogicalDpi;
                        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, destFileStream);
                        Stream pixelStream = Bitmap.PixelBuffer.AsStream();
                        byte[] pixels = new byte[pixelStream.Length];
                        await pixelStream.ReadAsync(pixels, 0, pixels.Length);
                        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)renderTargetBitmap.PixelWidth, (uint)renderTargetBitmap.PixelHeight, dpi, dpi, pixelBuffer.ToArray());
                        await encoder.FlushAsync();
                }
                    }
                }
                else
                {
                    Text = "Cancelado";
                }
       }
    }
}
