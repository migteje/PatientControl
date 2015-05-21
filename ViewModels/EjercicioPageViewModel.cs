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

namespace PatientControl.ViewModels
{
    public class EjercicioPageViewModel : ViewModel, Interfaces.IEjercicioPageViewModel
    {

        IEventAggregator _eventAggregator;
        INavigationService _navigationService;

        public DelegateCommand IniciarCommand { get; private set; }
        public DelegateCommand PausarCommand { get; private set; }
        public DelegateCommand PararCommand { get; private set; }
        public DelegateCommand GrabarCommand { get; private set; }
        public DelegateCommand MasCommand { get; private set; }
        public DelegateCommand MenosCommand { get; private set; }
        public DelegateCommand<string> CheckedCommand { get; private set; }

        private string _angulo = default(string);
        public string Angulo { get { return _angulo; } set { SetProperty(ref _angulo, value);} }
        private string _nombrePaciente = default(string);
        public string NombrePaciente { get { return _nombrePaciente; } set { SetProperty(ref _nombrePaciente, value); } }
        private string _title = default(string);
        public string Title { get { return _title; } set { SetProperty(ref _title, value); } }
        private bool _isSelected;
        public bool IsSelected { get { return _isSelected; } set { SetProperty(ref _isSelected, value); OnPropertyChanged("IsSelected"); } }
        private PacienteViewModel _paciente;
        public PacienteViewModel Paciente { get { return _paciente; } set { SetProperty(ref _paciente, value); } }
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

        private EjercicioViewModel ejerSelected;

        /// <summary>
        /// Kinect DPI.
        /// </summary>
        public static readonly double DPI = 96.0;

        /// <summary>
        /// Intermediate storage for receiving frame data from the sensor
        /// </summary>
        private byte[] pixels = null;

        /// <summary>
        /// The bitmap source.
        /// </summary>
        public WriteableBitmap _Bitmap = default(WriteableBitmap);
        public WriteableBitmap Bitmap { get { return _Bitmap; } set { SetProperty(ref _Bitmap, value); OnPropertyChanged(null); } }


        public Canvas _KinectCanvas = default(Canvas);
        public Canvas KinectCanvas { get { return _KinectCanvas; } set { SetProperty(ref _KinectCanvas, value); OnPropertyChanged(null); } }

        /// <summary>
        /// Number of bodies tracked
        /// </summary>
        private int bodyCount;

        private List<double> myList = new List<double>();
        private List<double> myMedian = new List<double>();
        private double[] myAngles = new double[5];

        private double angle;

        private Boolean inicial = true;
        private double anguloInicial = 0;
        private int calibration;

        private double UnderShoulderZ;

        private double median;

        private int confra;

        private Boolean comenzado;

        /// <summary>
        /// Reader for color frames
        /// </summary>
        private MultiSourceFrameReader reader;

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor kinectSensor;

        /// <summary>
        /// Array for the bodies
        /// </summary>
        private IList<Body> bodies = null;

        /// <summary>
        /// Width of display (depth space)
        /// </summary>
        private int displayWidth;


        /// <summary>
        /// Height of display (depth space)
        /// </summary>
        private int displayHeight;


        double anteriorZ=0;
        double anteriorY=0;
        double anteriorX=0;

        public EjercicioPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;
            Paciente = new PacienteViewModel(new Paciente());
            IniciarCommand = DelegateCommand.FromAsyncHandler(Iniciar);
            PausarCommand = DelegateCommand.FromAsyncHandler(Pausar);
            PararCommand = DelegateCommand.FromAsyncHandler(Parar);
            GrabarCommand = DelegateCommand.FromAsyncHandler(Grabar);
            MasCommand = DelegateCommand.FromAsyncHandler(Mas);
            MenosCommand = DelegateCommand.FromAsyncHandler(Menos);
            this.CheckedCommand = new DelegateCommand<string>(Checked);
        }

        public async override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            this.ejerSelected = navigationParameter as EjercicioViewModel;
            await this.Paciente.ObtenerPacientePorId(this.ejerSelected.Paciente_Id.ToString());
            this.Title = ejerSelected.Title;
            this.NombrePaciente = Paciente.Nombre;
            if (this.Title == "Abduccion-Aduccion")
            {
                this.Activado = "Aduccion";
                this.Desactivado = "Abduccion";
            }
            else
            {
                this.Activado = "Extension";
                this.Desactivado = "Flexion";
            }
            //this.Colores = "White";
            this.Angulo = "";
            this.Repeticion = 0;
            if (Paciente.ZonaLesion.Equals("Izquierdo"))
            {
                Izquierdo = true; Derecho = false;
            }
            else { Derecho = true; Izquierdo = false; }

            KinectConect();
        }
        
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

        public override void OnNavigatedFrom(Dictionary<string, object> viewModelState, bool suspending)
        {
            var patientControlApp = App.Current as App;
            if (patientControlApp != null && !patientControlApp.IsSuspending)
            {
                // Dispose of the frame reader and allow GC
                if (reader != null)
                {
                    reader.Dispose();
                    reader = null;
                }

                // Dispose of the Kinect device and allow GC
                if (kinectSensor == null) return;

                kinectSensor.Close();
                kinectSensor = null;
            }
        }

        /// <summary>
        /// Handles the color frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            // Color
            try
            {
                var frameReference = e.FrameReference.AcquireFrame();

                using (var colorFrame = frameReference.ColorFrameReference.AcquireFrame())
                {
                    if (colorFrame != null)
                    {

                        // Get info on what format frames will take as they arrive
                        var colorFrameDescription = kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);

                        // ColorFrame is IDisposable
                        displayWidth = colorFrameDescription.Width;
                        displayHeight = colorFrameDescription.Height;

                        pixels = new byte[this.displayWidth * this.displayHeight * colorFrameDescription.BytesPerPixel];

                        if (Bitmap == null)
                        {
                            // get size of joint space
                            Bitmap = new WriteableBitmap(displayWidth, displayHeight);
                        }

                        // Copy the image data into the temp pixel buffer (we have to use an intermdiate 
                        // byte array buffer as we can't  access the bitmap's buffer directly as an array of bytes)
                        if (colorFrame.RawColorImageFormat == ColorImageFormat.Bgra)
                            colorFrame.CopyRawFrameDataToArray(pixels);
                        else
                            colorFrame.CopyConvertedFrameDataToArray(pixels, ColorImageFormat.Bgra);

                        // Write the contents of the temp pixel buffer into the re-writable bitmap's buffer
                        Bitmap.PixelBuffer.AsStream().Write(pixels, 0, pixels.Length);
                        Bitmap.Invalidate(); // Invalidating the bitmap forces it to be redrawn
                    }
                }

                // Body
                using (var frame = frameReference.BodyFrameReference.AcquireFrame())
                {
                    if (frame != null)
                    {
                        if (KinectCanvas == null)
                            // get size of joint space
                            KinectCanvas = new Canvas();
                        
                        // BodyFrame is IDisposable
                        KinectCanvas.Children.Clear();

                        if (bodies == null)
                        bodies = new Body[frame.BodyFrameSource.BodyCount];

                        // set the maximum number of bodies that would be tracked by Kinect
                        this.bodyCount = kinectSensor.BodyFrameSource.BodyCount;

                        // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                        // As long as those body objects are not disposed and not set to null in the array,
                        // those body objects will be re-used.
                        frame.GetAndRefreshBodyData(bodies);
                        foreach (Body body in bodies)
                        {
                            if (body.IsTracked)
                            {
                                IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

                                // convert the joint points to depth (display) space
                                Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                                foreach (Joint joint in body.Joints.Values)
                                {
                                    if (joint.TrackingState == TrackingState.Tracked)
                                    {
                                        // 3D space point
                                        CameraSpacePoint position = joint.Position;

                                        // 2D space point
                                        Point point = new Point();

                                        ColorSpacePoint colorPoint = kinectSensor.CoordinateMapper.MapCameraPointToColorSpace(position);

                                        point.X = ((float.IsInfinity(colorPoint.X) ? 0 : colorPoint.X) - 35) / 1.05;
                                        point.Y = ((float.IsInfinity(colorPoint.Y) ? 0 : colorPoint.Y) - 50) / 1.1;

                                        jointPoints[joint.JointType] = point;

                                        if (IsSelected) DrawJoint(KinectCanvas, point);
                                    }

                                }
                                Point UnderShoulder = new Point();
                                double d1 = Math.Sqrt(Math.Pow((joints[JointType.SpineBase].Position.X - joints[JointType.ShoulderRight].Position.X), 2) + Math.Pow((joints[JointType.SpineBase].Position.Y - joints[JointType.ShoulderLeft].Position.Y), 2));
                                double d2 = Math.Sqrt(Math.Pow((joints[JointType.SpineBase].Position.Y - joints[JointType.ShoulderLeft].Position.Y), 2));
                                double dx = Math.Sqrt(Math.Pow(d1, 2) - Math.Pow(d2, 2));
                                Point UnderShoulderDraw = new Point();
                                double dr1 = Math.Sqrt(Math.Pow((jointPoints[JointType.SpineBase].X - jointPoints[JointType.ShoulderRight].X), 2) + Math.Pow((jointPoints[JointType.SpineBase].Y - jointPoints[JointType.ShoulderLeft].Y), 2));
                                double dr2 = Math.Sqrt(Math.Pow((jointPoints[JointType.SpineBase].Y - jointPoints[JointType.ShoulderLeft].Y), 2));
                                double drx = Math.Sqrt(Math.Pow(dr1, 2) - Math.Pow(dr2, 2));
                                if (Izquierdo)
                                {
                                    UnderShoulderDraw.X = jointPoints[JointType.SpineBase].X - drx;
                                    UnderShoulderDraw.Y = jointPoints[JointType.SpineBase].Y;
                                    UnderShoulder.X = joints[JointType.SpineBase].Position.X - dx;
                                    UnderShoulder.Y = joints[JointType.SpineBase].Position.Y;

                                }
                                else
                                {
                                    UnderShoulderDraw.X = jointPoints[JointType.SpineBase].X + drx;
                                    UnderShoulderDraw.Y = jointPoints[JointType.SpineBase].Y;
                                    UnderShoulder.X = joints[JointType.SpineBase].Position.X + dx;
                                    UnderShoulder.Y = joints[JointType.SpineBase].Position.Y;
                                }

                                UnderShoulderZ = joints[JointType.SpineBase].Position.Z;
                                DrawJoint(KinectCanvas, UnderShoulderDraw);

                                if (IsSelected) DrawBody(joints, jointPoints, KinectCanvas);
                                if (comenzado)
                                {                               
                                    switch (Title) 
                                    {
                                        case "Abduccion-Aduccion":
                                            if (Izquierdo)
                                            {
                                                if (anteriorZ == 0) anteriorZ = joints[JointType.ElbowLeft].Position.Z;
                                                //Debug.WriteLine(anteriorZ + " y " + joints[JointType.ElbowLeft].Position.Z);
                                                if ((anteriorZ - 0.15 <= joints[JointType.ElbowLeft].Position.Z) && (joints[JointType.ElbowLeft].Position.Z <= anteriorZ + 0.15))
                                                    angle = AngleBetweenJoints(joints[JointType.ElbowLeft], joints[JointType.ShoulderLeft], UnderShoulder, UnderShoulderZ);
                                            }
                                            if (Derecho)
                                            {
                                                if (anteriorZ == 0) anteriorZ = joints[JointType.ElbowRight].Position.Z;
                                                if ((anteriorZ - 0.15 <= joints[JointType.ElbowRight].Position.Z) && (joints[JointType.ElbowRight].Position.Z <= anteriorZ + 0.15))
                                                    angle = AngleBetweenJoints(joints[JointType.ElbowRight], joints[JointType.ShoulderRight], UnderShoulder, UnderShoulderZ);
                                            }
                                            break;
                                        case "FlexoExtension":
                                            if (Izquierdo)
                                            {
                                                if (anteriorX == 0) anteriorX = joints[JointType.ElbowLeft].Position.X;
                                                if ((anteriorX - 0.15 <= joints[JointType.ElbowLeft].Position.X) && (joints[JointType.ElbowLeft].Position.X <= anteriorX + 0.15))
                                                    angle = AngleBetweenJoints(joints[JointType.ElbowLeft], joints[JointType.ShoulderLeft], UnderShoulder, UnderShoulderZ);
                                            }
                                            if (Derecho)
                                            {
                                                if (anteriorX == 0) anteriorX = joints[JointType.ElbowRight].Position.X;
                                                if ((anteriorX - 0.15 <= joints[JointType.ElbowRight].Position.X) && (joints[JointType.ElbowRight].Position.X <= anteriorX + 0.15))
                                                angle = AngleBetweenJoints(joints[JointType.ElbowRight], joints[JointType.ShoulderRight], UnderShoulder, UnderShoulderZ);
                                            }
                                            break;
                                        case "FlexExHorizontal":
                                            if (Izquierdo)
                                            {
                                                if (anteriorY == 0) anteriorY = joints[JointType.ElbowLeft].Position.Y;
                                                if ((anteriorY - 0.15 <= joints[JointType.ElbowLeft].Position.Y) && (joints[JointType.ElbowLeft].Position.Y <= anteriorY + 0.15)){
                                                    /*if (joints[JointType.ElbowRight].Position.X == joints[JointType.ShoulderRight].Position.X)
                                                        angle = 90 + CalcularAnguloAlterno(joints[JointType.ElbowRight], joints[JointType.ShoulderRight], joints[JointType.ShoulderLeft], UnderShoulder, UnderShoulderZ);
                                                    else*/ angle = 90 - CalcularAnguloAlterno(joints[JointType.ElbowRight], joints[JointType.ShoulderRight], joints[JointType.ShoulderLeft], UnderShoulder, UnderShoulderZ);
                                                }
                                            }
                                            if (Derecho)
                                            {
                                                if (anteriorY == 0) anteriorY = joints[JointType.ElbowRight].Position.Y;
                                                if ((anteriorY - 0.3 <= joints[JointType.ElbowRight].Position.Y) && (joints[JointType.ElbowRight].Position.Y <= anteriorY + 0.3))
                                                {
                                                    /*if (joints[JointType.ElbowRight].Position.X == joints[JointType.ShoulderRight].Position.X)
                                                        angle = 90 + CalcularAnguloAlterno(joints[JointType.ElbowRight], joints[JointType.ShoulderRight], joints[JointType.ShoulderLeft], UnderShoulder, UnderShoulderZ);
                                                    else*/ angle = 90 - CalcularAnguloAlterno(joints[JointType.ElbowRight], joints[JointType.ShoulderRight], joints[JointType.ShoulderLeft], UnderShoulder, UnderShoulderZ);
                                                }
                                            }
                                            break;
                                        case "CodoFlexEx":
                                            if (Izquierdo)
                                                angle = AngleBetweenJoints(joints[JointType.WristLeft], joints[JointType.ElbowLeft], joints[JointType.ShoulderLeft]);
                                            if (Derecho)
                                                angle = AngleBetweenJoints(joints[JointType.WristRight], joints[JointType.ElbowRight], joints[JointType.ShoulderRight]);                                            break;
                                }

                                    myList.Add(angle);
                                    if (confra == 15)
                                    {
                                        CalculateMedian(myList, out median);
                                        myList.CopyTo((myList.Count - 5), myAngles, 0, 5);
                                        myList.Clear();
                                        myList.AddRange(myAngles);
                                        myMedian.Add(median);
                                        //Debug.WriteLine(median);
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
                                            WriteAngle(median,inicial);
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
        /// Draws a body
        /// </summary>
        /// <param name="joints">joints to draw</param>
        /// <param name="jointPoints">translated positions of joints to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="drawingPen">specifies color to draw a specific body</param>
        private void DrawBody(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, Canvas c)
        {
            // Draw the bones
            // Torso
            this.DrawBone(joints, jointPoints, JointType.Head, JointType.Neck, c);
            this.DrawBone(joints, jointPoints, JointType.Neck, JointType.SpineShoulder, c);
            this.DrawBone(joints, jointPoints, JointType.SpineShoulder, JointType.SpineMid, c);
            this.DrawBone(joints, jointPoints, JointType.SpineMid, JointType.SpineBase, c);
            this.DrawBone(joints, jointPoints, JointType.SpineShoulder, JointType.ShoulderRight, c);
            this.DrawBone(joints, jointPoints, JointType.SpineShoulder, JointType.ShoulderLeft, c);
            this.DrawBone(joints, jointPoints, JointType.SpineBase, JointType.HipRight, c);
            this.DrawBone(joints, jointPoints, JointType.SpineBase, JointType.HipLeft, c);

            // Right Arm    
            this.DrawBone(joints, jointPoints, JointType.ShoulderRight, JointType.ElbowRight, c);
            this.DrawBone(joints, jointPoints, JointType.ElbowRight, JointType.WristRight, c);
            this.DrawBone(joints, jointPoints, JointType.WristRight, JointType.HandRight, c);
            this.DrawBone(joints, jointPoints, JointType.HandRight, JointType.HandTipRight, c);
            this.DrawBone(joints, jointPoints, JointType.WristRight, JointType.ThumbRight, c);

            // Left Arm
            this.DrawBone(joints, jointPoints, JointType.ShoulderLeft, JointType.ElbowLeft, c);
            this.DrawBone(joints, jointPoints, JointType.ElbowLeft, JointType.WristLeft, c);
            this.DrawBone(joints, jointPoints, JointType.WristLeft, JointType.HandLeft, c);
            this.DrawBone(joints, jointPoints, JointType.HandLeft, JointType.HandTipLeft, c);
            this.DrawBone(joints, jointPoints, JointType.WristLeft, JointType.ThumbLeft, c);

            // Right Leg
            this.DrawBone(joints, jointPoints, JointType.HipRight, JointType.KneeRight, c);
            this.DrawBone(joints, jointPoints, JointType.KneeRight, JointType.AnkleRight, c);
            this.DrawBone(joints, jointPoints, JointType.AnkleRight, JointType.FootRight, c);

            // Left Leg
            this.DrawBone(joints, jointPoints, JointType.HipLeft, JointType.KneeLeft, c);
            this.DrawBone(joints, jointPoints, JointType.KneeLeft, JointType.AnkleLeft, c);
            this.DrawBone(joints, jointPoints, JointType.AnkleLeft, JointType.FootLeft, c);
        }

        public void DrawJoint(Canvas canvas, Point point)
        {
            // Create an ellipse.
            Ellipse ellipse = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = new SolidColorBrush(Colors.LightBlue)
            };

            // Position the ellipse according to the point's coordinates.
            Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);

            // Add the ellipse to the canvas.
            canvas.Children.Add(ellipse);
        }

        /// <summary>
        /// Draws one bone of a body (joint to joint)
        /// </summary>
        /// <param name="joints">joints to draw</param>
        /// <param name="jointPoints">translated positions of joints to draw</param>
        /// <param name="jointType0">first joint of bone to draw</param>
        /// <param name="jointType1">second joint of bone to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// /// <param name="drawingPen">specifies color to draw a specific bone</param>
        private void DrawBone(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, Canvas c)
        {
            Joint joint0 = joints[jointType0];

            Joint joint1 = joints[jointType1];
            // If we can't find either of these joints, exit
            if (joint0.TrackingState == TrackingState.NotTracked ||
                joint1.TrackingState == TrackingState.NotTracked)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            if ((joint0.TrackingState == TrackingState.Tracked) && (joint1.TrackingState == TrackingState.Tracked))
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
        /// Regresa el ángulo interno dadas 3 Joints
        /// </summary>
        /// <param name="j1"></param>
        /// <param name="j2"></param>
        /// <param name="j3"></param>
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
        /// Regresa el ángulo interno dadas 3 Joints
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
        /// Euclidean norm of 3-component Vector
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private static double vectorNorm(double x, double y, double z)
        {
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
        }

        private void WriteAngle(double p, bool correcto)
        {
            if (correcto) this.Colores = "Red";
            else
            this.Colores = "White";
            this.Angulo = p.ToString();
        }

        /// <summary>
        /// Converts rotation quaternion to Euler angles 
        /// And then maps them to a specified range of values to control the refresh rate
        /// </summary>
        /// <param name="rotQuaternion">face rotation quaternion</param>
        /// <param name="pitch">rotation about the X-axis</param>
        /// <param name="yaw">rotation about the Y-axis</param>
        /// <param name="roll">rotation about the Z-axis</param>
        private void CalculateMedian(List<double> myList, out double median)
        {
            var query = from numbers in myList //select the numbers
                        orderby numbers ascending
                        select numbers;

            if (myList.Count % 2 == 0)
            { //we know its even
                int element = myList.Count / 2; ;
                median = (double)((query.ElementAt(element - 1) + query.ElementAt(element)) / 2);
            }
            else
            {
                //we know its odd
                double element = (double)myList.Count / 2;
                element = Math.Round(element, MidpointRounding.AwayFromZero);
                median = (double)query.ElementAt((int)(element - 1));
            }
        }
        
        private double CalcularAnguloAlterno(Joint c, Joint h, Joint s, Point p, double pZ)
        {
            double Angulo = 0;

            double AF = ((s.Position.Y * pZ) - (s.Position.Y * h.Position.Z) - (h.Position.Y * pZ) - (s.Position.Z * p.Y) + (s.Position.Z * h.Position.Y) + (h.Position.Z * p.Y));
            double BF = ((s.Position.Z * p.X) - (s.Position.Z * h.Position.X) - (h.Position.Z * p.X) - (s.Position.X * pZ) + (s.Position.X * h.Position.Z) + (h.Position.X * pZ));
            double CF = ((s.Position.X * p.Y) - (s.Position.X * h.Position.Y) - (h.Position.X * p.Y) - (s.Position.Y * p.X) + (s.Position.Y * h.Position.X) + (h.Position.Y * p.X));

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
            //Debug.WriteLine(Math.Round(Angulo, 0));
            return Math.Round(Angulo, 0);
        }

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
        }

        private async Task Mas()
        {
            Repeticion++;
        }

        private async Task Menos()
        {
            if (Repeticion != 0) Repeticion--;
        }

        private async Task Pausar()
        {
            comenzado = false;
        }

        private async Task Grabar()
        {
            if (this.ejerSelected != null & this.Angulo != null)
            {
                /*if (On)
                {
                   switch (Title)
                    {
                        case "Abduccion-Aduccion":
                            ejerSelected.AnguloAd = Angulo;
                            ejerSelected.RepeticionesAbd = Repeticion;
                            break;
                        case "FlexoExtension":
                            ejerSelected.AnguloEx = Angulo;
                            ejerSelected.RepeticionesFlEx = Repeticion;
                            break;
                        case "FlexExHorizontal":
                            ejerSelected.AnguloExh = Angulo;
                            ejerSelected.RepeticionesFlExh = Repeticion;
                            break;
                        case "CodoFlexEx":
                            ejerSelected.AnguloCex = Angulo;
                            ejerSelected.RepeticionesCflEx = Repeticion;
                            break;
                    }
                }
                else
                {
                    switch (Title)
                    {
                        case "Abduccion-Aduccion":
                            ejerSelected.AnguloAb = Angulo;
                            ejerSelected.RepeticionesAbd = Repeticion;
                            break;
                        case "FlexoExtension":
                            ejerSelected.AnguloFl = Angulo;
                            ejerSelected.RepeticionesFlEx = Repeticion;
                            break;
                        case "FlexExHorizontal":
                            ejerSelected.AnguloFlh = Angulo;
                            ejerSelected.RepeticionesFlExh = Repeticion;
                            break;
                        case "CodoFlexEx":
                            ejerSelected.AnguloCfl = Angulo;
                            ejerSelected.RepeticionesCflEx = Repeticion;
                            break;
                    }
                }*/
                ejerSelected.Angulo = Angulo;
                ejerSelected.Repeticiones = Repeticion;
                    if (On)
                ejerSelected.Tipo = Activado;
                    else
                ejerSelected.Tipo = Desactivado;

                await ejerSelected.InsertarNuevoEjercicio();
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
    }
}
