//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

//@autors: Cristina Zuheros Montes
//          Miguel Sánchez Maldonado
namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using Microsoft.Kinect;
    using System.Windows.Media.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Width of output drawing
        /// </summary>
        private const float RenderWidth = 640.0f;
        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private WriteableBitmap colorBitmap;

        /// <summary>
        /// Intermediate storage for the color data received from the camera
        /// </summary>
        private byte[] colorPixels;


        /// <summary>
        /// Fase en que se encuentran los movimientos.
        /// </summary>
        private int fase = 0;

        /// <summary>
        /// Variables que almacenarán las medidas del esqueleto
        /// </summary>
        public float hombroCodo = 0;
        public float codoMano = 0;     
        public float caderaRodilla = 0;      
        public float rodillaPie = 0;
       
        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 480.0f;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly Brush centerPointBrush = Brushes.Blue;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary >        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;

        /// <summary>
        /// Declaramos una lista de JoinType, uno para cada articulación que vamos a usar en el movimiento
        /// </summary>
        private static List<JointType> jointsTypes = new List<JointType> { JointType.ShoulderLeft, JointType.ElbowLeft, JointType.WristLeft, JointType.HandLeft,
                                                                            JointType.ShoulderRight, JointType.ElbowRight, JointType.WristRight, JointType.HandRight,
                                                                            JointType.HipLeft, JointType.KneeLeft, JointType.AnkleLeft, JointType.FootLeft,
                                                                            JointType.HipRight, JointType.KneeRight, JointType.AnkleRight, JointType.FootRight};

        /// <summary>
        /// Índice de error.
        /// </summary>
        private float ERROR = 0.1F;


        /// <summary>
        /// Lista de tamaño igual al número de JoinType que hay que comprobar, 
        /// en esta lista se almacenan los valores de error de cada posición.
        /// </summary>
        private List<int> diff_positions = new List<int>(new int[jointsTypes.Count]);

        /// <summary>
        /// Pen usado para pintar las partes de los brazos que están mal colocadas.
        /// </summary>
        private readonly Pen failBonePenRed = new Pen(Brushes.Red, 6);
        /// <summary>
        /// Pen usado para pintar las partes de los brazos que están mal colocadas.
        /// </summary>
        private readonly Pen failBonePenBlue = new Pen(Brushes.Blue, 6);
        /// Pen usado para pintar las partes de los brazos que están mal colocadas.
        /// </summary>
        private readonly Pen failBonePenPurple = new Pen(Brushes.Purple, 6);
        /// Pen usado para pintar las partes de los brazos que están mal colocadas.
        /// </summary>
        private readonly Pen failBonePenOrange = new Pen(Brushes.Orange, 6);


        /// <summary>
        /// Numero del frame actual para controlar que se realiace el movimiento correcto.
        /// </summary>
        private int frame = 0;

        /// <summary>
        /// Numero de frame guardado para hacer nuestros "sleep".
        /// </summary>
        private int frame_aux = 0;

        /// <summary>
        /// Boolean que se activa .
        /// </summary>
        private bool contador = false;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Draws indicators to show which edges are clipping skeleton data
        /// </summary>
        /// <param name="skeleton">skeleton to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private static void RenderClippedEdges(Skeleton skeleton, DrawingContext drawingContext)
        {
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }
        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {

                this.Esqueleto.Source = this.imageSource;
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;
                // Turn on the color stream to receive color frames
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                // Allocate space to put the pixels we'll receive
                this.colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];

                // This is the bitmap we'll display on-screen
                this.colorBitmap = new WriteableBitmap(this.sensor.ColorStream.FrameWidth, this.sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

                // Set the image we display to point to the bitmap where we'll put the image data
                this.ColorI.Source = this.colorBitmap;

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.ColorFrameReady += this.SensorColorFrameReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }


        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's ColorFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e) {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    // Copy the pixel data from the image to a temporary array
                    colorFrame.CopyPixelDataTo(this.colorPixels);

                    // Write the pixel data into our bitmap
                    this.colorBitmap.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                        this.colorPixels,
                        this.colorBitmap.PixelWidth * sizeof(int),
                        0);
                }
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e) {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                    frame = skeletonFrame.FrameNumber; //sacamos el numero de frame actual
                }
            }

            using (DrawingContext dc = this.drawingGroup.Open()) {
                // Draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Transparent, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
                        RenderClippedEdges(skel, dc);

                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            check(skel);
                            this.DrawBonesAndJoints(skel, dc);
                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            this.SkeletonPointToScreen(skel.Position),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }
                    }
                }

                // prevent drawing outside of our render area
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
        }

        /// <summary>
        /// Draws a skeleton's bones and joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)  {
            // Render Torso
            this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
            this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);

            // Left Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
            this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);

            // Right Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
            this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

            // Left Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);
            this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);

            // Right Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
            this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight);

            // comprueba los movimientos y dibuja la ayuda 
            dibujarAyuda(skeleton, drawingContext);

            CompruebaMovimientos(skeleton);
            // Render Joints
            foreach (Joint joint in skeleton.Joints)
            {
                Brush drawBrush = null;

                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                   // drawBrush = getBrush(joint);
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                }
            }
        }

        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint) {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }
        /// <summary>
        /// Función que tomará medidas de las articulaciones del usuario en todo momento para colocar las esferas
        /// ajustandose a las medidas del usuario que realice el movimiento. 
        /// Lo que hacemos es sacar el maximo de todas las medidas realizadas en el ejeX, ejeY, ejeZ, para que se coloque como se coloque
        /// el usuario podamos coger su medida real lo antes posible.
        /// </summary>
        /// <param name="esqueleto"></param>
        public void TomaMedidas(Skeleton esqueleto) {
            if (hombroCodo < diffabs(esqueleto.Joints[jointsTypes[0]].Position.X, esqueleto.Joints[jointsTypes[1]].Position.X))
                hombroCodo = diffabs(esqueleto.Joints[jointsTypes[0]].Position.X, esqueleto.Joints[jointsTypes[1]].Position.X);
            if (hombroCodo < diffabs(esqueleto.Joints[jointsTypes[0]].Position.Y, esqueleto.Joints[jointsTypes[1]].Position.Y))
                hombroCodo = diffabs(esqueleto.Joints[jointsTypes[0]].Position.Y, esqueleto.Joints[jointsTypes[1]].Position.Y);
            if (hombroCodo < diffabs(esqueleto.Joints[jointsTypes[0]].Position.Z, esqueleto.Joints[jointsTypes[1]].Position.Z))
                hombroCodo = diffabs(esqueleto.Joints[jointsTypes[0]].Position.Z, esqueleto.Joints[jointsTypes[1]].Position.Z);

            if (codoMano < diffabs(esqueleto.Joints[jointsTypes[1]].Position.X, esqueleto.Joints[jointsTypes[2]].Position.X))
                codoMano = diffabs(esqueleto.Joints[jointsTypes[1]].Position.X, esqueleto.Joints[jointsTypes[2]].Position.X);
            if (codoMano < diffabs(esqueleto.Joints[jointsTypes[1]].Position.Y, esqueleto.Joints[jointsTypes[2]].Position.Y))
                codoMano = diffabs(esqueleto.Joints[jointsTypes[1]].Position.Y, esqueleto.Joints[jointsTypes[2]].Position.Y);
            if (codoMano < diffabs(esqueleto.Joints[jointsTypes[1]].Position.Z, esqueleto.Joints[jointsTypes[2]].Position.Z))
                codoMano = diffabs(esqueleto.Joints[jointsTypes[1]].Position.Z, esqueleto.Joints[jointsTypes[2]].Position.Z);


            if (caderaRodilla < diffabs(esqueleto.Joints[jointsTypes[8]].Position.X, esqueleto.Joints[jointsTypes[9]].Position.X))
                caderaRodilla = diffabs(esqueleto.Joints[jointsTypes[8]].Position.X, esqueleto.Joints[jointsTypes[9]].Position.X);
            if (caderaRodilla < diffabs(esqueleto.Joints[jointsTypes[8]].Position.Y, esqueleto.Joints[jointsTypes[9]].Position.Y))
                caderaRodilla = diffabs(esqueleto.Joints[jointsTypes[8]].Position.Y, esqueleto.Joints[jointsTypes[9]].Position.Y);
            if (caderaRodilla < diffabs(esqueleto.Joints[jointsTypes[8]].Position.Z, esqueleto.Joints[jointsTypes[9]].Position.Z))
                caderaRodilla = diffabs(esqueleto.Joints[jointsTypes[8]].Position.Z, esqueleto.Joints[jointsTypes[9]].Position.Z);


            if (rodillaPie < diffabs(esqueleto.Joints[jointsTypes[9]].Position.X, esqueleto.Joints[jointsTypes[10]].Position.X))
                rodillaPie = diffabs(esqueleto.Joints[jointsTypes[9]].Position.X, esqueleto.Joints[jointsTypes[10]].Position.X);
            if (rodillaPie < diffabs(esqueleto.Joints[jointsTypes[9]].Position.Y, esqueleto.Joints[jointsTypes[10]].Position.Y))
                rodillaPie = diffabs(esqueleto.Joints[jointsTypes[9]].Position.Y, esqueleto.Joints[jointsTypes[10]].Position.Y);
            if (rodillaPie < diffabs(esqueleto.Joints[jointsTypes[9]].Position.Z, esqueleto.Joints[jointsTypes[10]].Position.Z))
                rodillaPie = diffabs(esqueleto.Joints[jointsTypes[9]].Position.Z, esqueleto.Joints[jointsTypes[10]].Position.Z);

        }

        /// <summary>
        /// Funcion encargada de que se realizen los ejercicios en orden. Consta de 3 fases, y solo podremos pasar a la 
        /// siguiente fase cuando se realice el movimiento actual.
        /// </summary>
        /// <param name="esqueleto"></param>
        /// 

        public void CompruebaMovimientos(Skeleton esqueleto) {
            if (this.fase == 0)            
                PosicionInicial(esqueleto);           
            if (this.fase == 1)          
                PosicionFinal(esqueleto);           
            if (this.fase == 2)           
                this.FeedbackTexBlock.Text = "\t Se acabo el ejercicio";         
        }

        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1) {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
            //  drawPen = this.trackedBonePen;
                drawPen = getPen(jointType1);
            }

            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
        }


        /// <summary>
        /// Calcula la diferencia entre dos valores en valor absoluto.
        /// </summary>
        private float diffabs(float v1, float v2) { return Math.Abs(v1 - v2); }

        /// <summary>
        /// Calcula la diferencia entre dos valores.
        /// </summary>
        private float diff(float v1, float v2) { return (v1 - v2); }

        /// <summary>
        /// Funcion que comprueba si estamos realizando movimiento inicial, es decir los brazos levantados y las piernas relajadas.
        /// Pasamos el esqueleto como parámetro y comprobamos si todas las articulaciones están colocadas en la posición correcta. 
        /// Para ello en lista de JoinType vamos cogiendo las articulaciones y haciendo comprobaciones de distancias para saber 
        /// la posición actual, en caso de estar en la posición correcta  y si es la primera vez que se realiza activamos el booleano
        /// para que comience a contar el frame, de esta manera obligamos a que el usuario se mantega un determinado tiempo en la posición.
        /// En caso de no estar en la posición correcta, devuelve que no estamos en la posición correcta y además si veniamos de estar realizando
        /// la posición correcta pone contador a false para asi reiniciar el frame
        /// <param name="esqueleto"></param>
        /// <returns></returns>
        public bool PosicionInicial(Skeleton esqueleto){
            this.FeedbackTexBlock.Text = "\t Coloquese en la posición inicial";
            bool enPosicion = false;

            if ((esqueleto.Joints[jointsTypes[0]].Position.Y < esqueleto.Joints[jointsTypes[1]].Position.Y) &&
                 (esqueleto.Joints[jointsTypes[4]].Position.Y < esqueleto.Joints[jointsTypes[5]].Position.Y) &&
                 (diff(esqueleto.Joints[jointsTypes[0]].Position.X, esqueleto.Joints[jointsTypes[1]].Position.X) < ERROR) &&
                 (diff(esqueleto.Joints[jointsTypes[5]].Position.X, esqueleto.Joints[jointsTypes[4]].Position.X) < ERROR) &&
                 (esqueleto.Joints[jointsTypes[1]].Position.X < (esqueleto.Joints[jointsTypes[2]].Position.X - 0.05)) &&
                 (esqueleto.Joints[jointsTypes[6]].Position.X < (esqueleto.Joints[jointsTypes[5]].Position.X - 0.05)) &&
                 (diff(esqueleto.Joints[jointsTypes[2]].Position.X, esqueleto.Joints[jointsTypes[3]].Position.X) < 0.5 * ERROR) &&
                 (diff(esqueleto.Joints[jointsTypes[7]].Position.X, esqueleto.Joints[jointsTypes[6]].Position.X) < 0.5 * ERROR) &&
                 (diff(esqueleto.Joints[jointsTypes[10]].Position.X, esqueleto.Joints[jointsTypes[9]].Position.X) < 0.02 * ERROR) &&
                 (diff(esqueleto.Joints[jointsTypes[13]].Position.X, esqueleto.Joints[jointsTypes[14]].Position.X) < 0.02 * ERROR))
            {
                this.FeedbackTexBlock.Text = "\t Bien hecho. Manténgase. ";
                if (contador == false){//Si es la primera vez que realizamos el movimiento correcto
                    contador = true;
                    frame_aux = frame;
                }
                else{
                    if (frame_aux + 45 < frame){
                        this.fase = 1;
                        contador = false;
                    }
                }

                enPosicion = true;
            }
            else{//si no es correcto el movimiento
                if (contador == true)//para reiniciar el frame
                    contador = false;

                this.FeedbackTexBlock.Text = "\t No esta en la posicion inicial.";
                enPosicion = false;
            }
            return enPosicion;
        }
        /// <summary>
        /// Metodo que comprueba si estamos en la posición final, es decir manos levantas y pie izquierdo levantado.
        /// Para ello le pasamos el esqueleto actual y comprobamos que todas las articulaciones estén en la posición correcta, 
        /// en ese caso activamos el booleano del frame para que comience a contar el tiempo estimado que el usuario mantenga la posición
        /// si no está realizando la posición correcta, dejamos el booleano desactivado para que luego se reinicie de nuevo el frame
        /// <param name="esqueleto"></param>

        public bool PosicionFinal(Skeleton esqueleto) {
            this.FeedbackTexBlock.Text = "\t Pongase en la posicion final";
            bool enPosicion = false;

            // dependiendo de la distancia el resultado sera true o false.
            if ((esqueleto.Joints[jointsTypes[0]].Position.Y < esqueleto.Joints[jointsTypes[1]].Position.Y) &&
                             (esqueleto.Joints[jointsTypes[4]].Position.Y < esqueleto.Joints[jointsTypes[5]].Position.Y) &&

                             (diff(esqueleto.Joints[jointsTypes[0]].Position.X, esqueleto.Joints[jointsTypes[1]].Position.X) < ERROR) &&
                             (diff(esqueleto.Joints[jointsTypes[5]].Position.X, esqueleto.Joints[jointsTypes[4]].Position.X) < ERROR) &&

                             (esqueleto.Joints[jointsTypes[1]].Position.X < (esqueleto.Joints[jointsTypes[2]].Position.X - 0.05)) &&
                             (esqueleto.Joints[jointsTypes[6]].Position.X < (esqueleto.Joints[jointsTypes[5]].Position.X - 0.05)) &&

                             (diff(esqueleto.Joints[jointsTypes[2]].Position.X, esqueleto.Joints[jointsTypes[3]].Position.X) < 0.5 * ERROR) &&
                             (diff(esqueleto.Joints[jointsTypes[7]].Position.X, esqueleto.Joints[jointsTypes[6]].Position.X) < 0.5 * ERROR) &&

                             (diff(esqueleto.Joints[jointsTypes[10]].Position.X, esqueleto.Joints[jointsTypes[9]].Position.X) > ERROR))
            {
                this.FeedbackTexBlock.Text = "\t Bien hecho. Manténgase. ";
                if (contador == false) {//Si acabamos de colocarnos en la posición correcta
                    contador = true;
                    frame_aux = frame;
                }
                else  {
                    if (frame_aux + 45 < frame) {
                        this.fase = 2;
                        contador = false;
                    }
                }
                enPosicion = true;

            }
            else {//Si no estamos realizando el movimiento correcto y antes estabamos correctamente, ponemos a false el booleano para reiniciar frame           
                if (contador == true)
                    contador = false;

                this.FeedbackTexBlock.Text = "\t No esta en la posicion final.";
                enPosicion = false;
            }
            return enPosicion;
        }

        /// <summary>
        /// 
        /// funcion dibujar ayuda la cual dependiendo de la fase en la que estemos dibujará unas determinas bolas y de
        /// unos determinados colores.
        /// </summary>
        /// <param name="esqueleto"></param>
        /// <param name="drawingContext"></param>

        public void dibujarAyuda(Skeleton esqueleto, DrawingContext drawingContext)
        {
            if (this.fase == 0)
                pintaBolasInicial(esqueleto, drawingContext);
            if (this.fase == 1) 
                pintaBolasFinal(esqueleto, drawingContext);
            if (this.fase == 2) 
                pintaBolasAcabado(esqueleto, drawingContext);
        }

        /// <summary>
        /// Funcion PitaBolasInicial que dibujará las esferas para que realicemos el movimiento inicial y las pinta 
        /// de color rojo. Para dibujarlas por ejemplo declaramos manosarriba que será la esfera encima de la cabeza y 
        /// pintamos la bola a la altura de la cabeza en el eje X y Z y en el eje Y la desplazamos hacia arriba la distancia
        /// entre codo y mano que obtiene el método tomaMedidas declarado anteriormente. Análogo para las demás esferas.
        /// Luego llamamos a drawelipse, indicando el sitio y la medida de la esfera.
        /// </summary>
        /// <param name="esqueleto"></param>
        /// <param name="drawingContext"></param>
        private void pintaBolasInicial(Skeleton esqueleto, DrawingContext drawingContext) {

            SkeletonPoint manosarriba = new SkeletonPoint();
            SkeletonPoint codod = new SkeletonPoint();
            SkeletonPoint codoi = new SkeletonPoint();

            //La X sera la distancia euclidea entre la muñeca y el hombro,
            // en el primer caso y el codo y el hombro en el segundo 
            manosarriba.X = esqueleto.Joints[JointType.ShoulderCenter].Position.X;
            manosarriba.Y = esqueleto.Joints[JointType.ShoulderCenter].Position.Y + codoMano;
            manosarriba.Z = esqueleto.Joints[JointType.ShoulderCenter].Position.Z;

            codod.X = esqueleto.Joints[JointType.ShoulderCenter].Position.X + hombroCodo;
            codod.Y = esqueleto.Joints[JointType.ShoulderCenter].Position.Y + 0.05F;
            codod.Z = esqueleto.Joints[JointType.ShoulderCenter].Position.Z;

            codoi.X = esqueleto.Joints[JointType.ShoulderCenter].Position.X - hombroCodo;
            codoi.Y = esqueleto.Joints[JointType.ShoulderCenter].Position.Y + 0.05F;
            codoi.Z = esqueleto.Joints[JointType.ShoulderCenter].Position.Z;

            Point pos1 = this.SkeletonPointToScreen(manosarriba);
            drawingContext.DrawEllipse(Brushes.Red, null, pos1, 10, 10);
            Point pos2 = this.SkeletonPointToScreen(codod);
            drawingContext.DrawEllipse(Brushes.Red, null, pos2, 10, 10);
            Point pos3 = this.SkeletonPointToScreen(codoi);
            drawingContext.DrawEllipse(Brushes.Red, null, pos3, 10, 10);
    
        }

        /// <summary>
        /// Funcion PintaBolasFinal, función análoga a la anterior solo que pintará una bola más aparte de las anteriores a la altura
        /// de la rodilla y ahora las pintará de color naranja para así identificar que ya estamos en movimiento de posición final.
        /// </summary>
        /// <param name="esqueleto"></param>
        /// <param name="drawingContext"></param>

        private void pintaBolasFinal(Skeleton esqueleto, DrawingContext drawingContext){
            SkeletonPoint manosarriba = new SkeletonPoint();
            SkeletonPoint codod = new SkeletonPoint();
            SkeletonPoint codoi = new SkeletonPoint();
            SkeletonPoint rodillaizquierda = new SkeletonPoint();
            
            manosarriba.X = esqueleto.Joints[JointType.ShoulderCenter].Position.X;
            manosarriba.Y = esqueleto.Joints[JointType.ShoulderCenter].Position.Y + codoMano;
            manosarriba.Z = esqueleto.Joints[JointType.ShoulderCenter].Position.Z;

            codod.X = esqueleto.Joints[JointType.ShoulderCenter].Position.X + hombroCodo;
            codod.Y = esqueleto.Joints[JointType.ShoulderCenter].Position.Y +0.05F;
            codod.Z = esqueleto.Joints[JointType.ShoulderCenter].Position.Z;

            codoi.X = esqueleto.Joints[JointType.ShoulderCenter].Position.X - hombroCodo;
            codoi.Y = esqueleto.Joints[JointType.ShoulderCenter].Position.Y + 0.05F;
            codoi.Z = esqueleto.Joints[JointType.ShoulderCenter].Position.Z;
            
            rodillaizquierda.X = esqueleto.Joints[JointType.KneeRight].Position.X - rodillaPie;
            rodillaizquierda.Y = esqueleto.Joints[JointType.KneeRight].Position.Y + 0.1F;
            rodillaizquierda.Z = esqueleto.Joints[JointType.KneeRight].Position.Z;

            Point pos1 = this.SkeletonPointToScreen(manosarriba);
            drawingContext.DrawEllipse(Brushes.Orange, null, pos1, 10, 10);
            Point pos2 = this.SkeletonPointToScreen(codod);
            drawingContext.DrawEllipse(Brushes.Orange, null, pos2, 10, 10);
            Point pos3 = this.SkeletonPointToScreen(codoi);
            drawingContext.DrawEllipse(Brushes.Orange, null, pos3, 10, 10);
            Point pos4 = this.SkeletonPointToScreen(rodillaizquierda);
            drawingContext.DrawEllipse(Brushes.Orange, null, pos4, 10, 10);
        }
        /// <summary>
        /// Método exactamente igual que PintaBolasFinal solo que ahora las esferas las pintará de color verde, para así
        /// indicar que el ejercicio está acabado
        /// </summary>
        /// <param name="esqueleto"></param>
        /// <param name="drawingContext"></param>
        private void pintaBolasAcabado(Skeleton esqueleto, DrawingContext drawingContext) {

            SkeletonPoint manosarriba = new SkeletonPoint();
            SkeletonPoint codod = new SkeletonPoint();
            SkeletonPoint codoi = new SkeletonPoint();
            SkeletonPoint rodillaizquierda = new SkeletonPoint();

            manosarriba.X = esqueleto.Joints[JointType.ShoulderCenter].Position.X;
            manosarriba.Y = esqueleto.Joints[JointType.ShoulderCenter].Position.Y + codoMano;
            manosarriba.Z = esqueleto.Joints[JointType.ShoulderCenter].Position.Z;

            codod.X = esqueleto.Joints[JointType.ShoulderCenter].Position.X + hombroCodo;
            codod.Y = esqueleto.Joints[JointType.ShoulderCenter].Position.Y + 0.05F;
            codod.Z = esqueleto.Joints[JointType.ShoulderCenter].Position.Z;

            codoi.X = esqueleto.Joints[JointType.ShoulderCenter].Position.X - hombroCodo;
            codoi.Y = esqueleto.Joints[JointType.ShoulderCenter].Position.Y + 0.05F;
            codoi.Z = esqueleto.Joints[JointType.ShoulderCenter].Position.Z;

            rodillaizquierda.X = esqueleto.Joints[JointType.KneeRight].Position.X - rodillaPie;
            rodillaizquierda.Y = esqueleto.Joints[JointType.KneeRight].Position.Y + 0.1F;
            rodillaizquierda.Z = esqueleto.Joints[JointType.KneeRight].Position.Z;

            Point pos1 = this.SkeletonPointToScreen(manosarriba);
            drawingContext.DrawEllipse(Brushes.Green, null, pos1, 10, 10);
            Point pos2 = this.SkeletonPointToScreen(codod);
            drawingContext.DrawEllipse(Brushes.Green, null, pos2, 10, 10);
            Point pos3 = this.SkeletonPointToScreen(codoi);
            drawingContext.DrawEllipse(Brushes.Green, null, pos3, 10, 10);
            Point pos4 = this.SkeletonPointToScreen(rodillaizquierda);
            drawingContext.DrawEllipse(Brushes.Green, null, pos4, 10, 10);
        }

        /// <summary>
        /// Devuelve el objeto Pen con el que pintar el hueso que se une con la parte del cuerpo JointType. Los distintos colores
        /// los usamos para pintar los huesos que están en una posición incorrecta, por ejemplo -1 será azul y nos indica que ese hueso
        /// no está bien colocado en el eje x.
        /// </summary>
        public Pen getPen(JointType j){
            if (jointsTypes.Contains(j)) {
                int i = jointsTypes.IndexOf(j);
                if (diff_positions[i] == 2)
                    return failBonePenPurple;
                else if (diff_positions[i] == 1)
                    return failBonePenOrange;
                else if (diff_positions[i] == -1)
                    return failBonePenBlue;
                else if (diff_positions[i] == 3)
                    return failBonePenRed;
            }

            return trackedBonePen;
        }

        /// <summary>
        /// Funcion check la cual recorre la lista de JoinType que almacena las articulaciones y lo que hacemos es medir la 
        /// longitud entre dos aticulacioens adyacente. Los if de abajo son para cuando cambiamos de las articulaciones superiores izquierdas 
        /// a las articulaciones superiores derechas, ya que no tiene sentido medir la distancia entre la mano izquierda con el hombre drecho
        /// </summary>
        /// <param name="esqueleto"></param>
        private void check(Skeleton esqueleto){
            if (esqueleto != null) {
                SkeletonPoint p1, p2;
                for (int i = 0; i < 14; i++)  {
                    p1 = esqueleto.Joints[jointsTypes[i]].Position;
                    p2 = esqueleto.Joints[jointsTypes[i + 1]].Position;

                    diff_positions[i + 1] = checkPosicion(p1, p2, esqueleto);
                    TomaMedidas(esqueleto);
                    
                    if (i == 2)
                        i++;
                    if (i == 10)
                        i++;

                }
            }
        }

        /// <summary>
        /// Comprueba si hay error entre dos puntos, si es así nos devuelve un entero que será un usado para pintar de un color
        /// el hueso y de está manera ayudarle al usuario para saber que está realizando mal.Si está bien se pinta verde.
        /// </summary>
        /// <return> 0 : Posición correcta. </return>
        /// <return> 1: Posición incorrecta y punto por encima. </return>
        /// <return> -1: Posición incorrecta y punto por debajo. </return>
        private int checkPosicion(SkeletonPoint P1, SkeletonPoint P2, Skeleton esqueleto) {
            if (fase == 0)  {//Realizar el movimiento inicial
                //brazos en profundidad correcta...
                if (diffabs(P1.Z, P2.Z) > 2 * ERROR)
                    return 1;//naranja
                
                //brazos arriba...
                if (P1 == esqueleto.Joints[jointsTypes[0]].Position && P2 == esqueleto.Joints[jointsTypes[1]].Position && P1.Y > P2.Y)
                    return 2;
                else if (P1 == esqueleto.Joints[jointsTypes[4]].Position && P2 == esqueleto.Joints[jointsTypes[5]].Position && P1.Y > P2.Y)      
                    return 2;                
                //hombro-codo
                else if (P1 == esqueleto.Joints[jointsTypes[0]].Position && P2 == esqueleto.Joints[jointsTypes[1]].Position && diff(P1.X, P2.X) > ERROR)                
                    return -1;                
                else if (P1 == esqueleto.Joints[jointsTypes[4]].Position && P2 == esqueleto.Joints[jointsTypes[5]].Position && diff(P2.X, P1.X) > ERROR)                
                    return -1;
                //codo-muñeca
                else if (P1 == esqueleto.Joints[jointsTypes[1]].Position && P2 == esqueleto.Joints[jointsTypes[2]].Position && P1.X > P2.X - 0.05)               
                    return -1;               
                else if (P1 == esqueleto.Joints[jointsTypes[5]].Position && P2 == esqueleto.Joints[jointsTypes[6]].Position && P2.X > P1.X - 0.05)               
                    return -1;              
                //muñeca-mano
                else if (P1 == esqueleto.Joints[jointsTypes[2]].Position && P2 == esqueleto.Joints[jointsTypes[3]].Position && diff(P1.X, P2.X) > 0.5 * ERROR)
                    return -1;
                else if (P1 == esqueleto.Joints[jointsTypes[6]].Position && P2 == esqueleto.Joints[jointsTypes[7]].Position && diff(P2.X, P1.X) > 0.5 * ERROR)
                    return -1;
                else
                    return 0;
            }
            else if (fase == 1) {//Realizar el movimiento final
                if (diffabs(P1.Z, P2.Z) > 2 * ERROR) 
                    return 1;//naranja
                //brazos arriba...
                if (P1 == esqueleto.Joints[jointsTypes[0]].Position && P2 == esqueleto.Joints[jointsTypes[1]].Position && P1.Y > P2.Y)
                    return 2;
                else if (P1 == esqueleto.Joints[jointsTypes[4]].Position && P2 == esqueleto.Joints[jointsTypes[5]].Position && P1.Y > P2.Y)
                    return 2;
                //hombro-codo
                else if (P1 == esqueleto.Joints[jointsTypes[0]].Position && P2 == esqueleto.Joints[jointsTypes[1]].Position && diff(P1.X, P2.X) > ERROR)
                    return -1;
                else if (P1 == esqueleto.Joints[jointsTypes[4]].Position && P2 == esqueleto.Joints[jointsTypes[5]].Position && diff(P2.X, P1.X) > ERROR)
                    return -1;
                //codo-muñeca
                else if (P1 == esqueleto.Joints[jointsTypes[1]].Position && P2 == esqueleto.Joints[jointsTypes[2]].Position && P1.X > P2.X - 0.05)
                    return -1;
                else if (P1 == esqueleto.Joints[jointsTypes[5]].Position && P2 == esqueleto.Joints[jointsTypes[6]].Position && P2.X > P1.X - 0.05)
                    return -1;
                //muñeca-mano
                else if (P1 == esqueleto.Joints[jointsTypes[2]].Position && P2 == esqueleto.Joints[jointsTypes[3]].Position && diff(P1.X, P2.X) > 0.5 * ERROR)
                    return -1;
                else if (P1 == esqueleto.Joints[jointsTypes[6]].Position && P2 == esqueleto.Joints[jointsTypes[7]].Position && diff(P2.X, P1.X) > 0.5 * ERROR)
                    return -1;
                //posicion correcta de la pierna. 
                else if (P1 == esqueleto.Joints[jointsTypes[9]].Position && P2 == esqueleto.Joints[jointsTypes[10]].Position && diff(P2.X, P1.X) < ERROR)
                    return 2;
                else
                    return 0;
            }
            else return 0;

            }
           

         }
    }




