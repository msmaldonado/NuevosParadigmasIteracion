//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.ControlsBasics
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using Microsoft.Kinect;
    using Microsoft.Kinect.Toolkit;
    using Microsoft.Kinect.Toolkit.Controls;
    using System.Windows.Media.Imaging;
    using System.Windows.Media;
    using System.Windows.Documents;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow
    {
        public static readonly DependencyProperty PageLeftEnabledProperty = DependencyProperty.Register(
            "PageLeftEnabled", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty PageRightEnabledProperty = DependencyProperty.Register(
            "PageRightEnabled", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        private const double ScrollErrorMargin = 0.001;

        private const int PixelScrollByAmount = 20;

        private readonly KinectSensorChooser sensorChooser;

        String tema = null; //almacena el nombre del tema elegido

        String opcion = null; //almacena la opcion que el usuario marca como respuesta

        String dificultad = null; // almacena el valor de la dificultad

        bool primera = true; // Comprobar si vamos a poner la primera pregunta

        int n_pregunta = 1; // Almacena por el numero de pregunta que vamos

        int n_correctas = 0; //Contador de las preguntas correctas

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class. 
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            // initialize the sensor chooser and UI
            this.sensorChooser = new KinectSensorChooser();
            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
            this.sensorChooser.Start();

            // Bind the sensor chooser's current sensor to the KinectRegion
            var regionSensorBinding = new Binding("Kinect") { Source = this.sensorChooser };
            BindingOperations.SetBinding(this.kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);

            // Clear out placeholder content
            this.wrapPanel.Children.Clear();

            // Pone la imagen de pedir pregunta inicial
            this.Imagen_pregunta.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("DIFICULTAD.png")));

            // Add in display content  
            //Creamos los botones de los temas
            var button = crearBoton("HISTORIA");
            this.wrapPanel.Children.Add(button);
            button = crearBoton("ARTE");
            this.wrapPanel.Children.Add(button);
            button = crearBoton("DEPORTES");
            this.wrapPanel.Children.Add(button);
            button = crearBoton("LITERATURA");
            this.wrapPanel.Children.Add(button);
            button = crearBoton("GEOGRAFIA");
            this.wrapPanel.Children.Add(button);
            button = crearBoton("CIENCIA");
            this.wrapPanel.Children.Add(button);


            //Creamos los botones de las opciones
            var button2 = crearBoton2("A");
            this.wrapPanel2.Children.Add(button2);
            button2 = crearBoton2("B");
            this.wrapPanel2.Children.Add(button2);
            button2 = crearBoton2("C");
            this.wrapPanel2.Children.Add(button2);


            // Bind listner to scrollviwer scroll position change, and check scroll viewer position
            this.UpdatePagingButtonState();
            scrollViewer.ScrollChanged += (o, e) => this.UpdatePagingButtonState();

            scrollViewer2.ScrollChanged += (o, e) => this.UpdatePagingButtonState();

        }

        /// <summary>
        /// CLR Property Wrappers for PageLeftEnabledProperty
        /// </summary>
        public bool PageLeftEnabled
        {
            get
            {
                return (bool)GetValue(PageLeftEnabledProperty);
            }

            set
            {
                this.SetValue(PageLeftEnabledProperty, value);
            }
        }

        /// <summary>
        /// CLR Property Wrappers for PageRightEnabledProperty
        /// </summary>
        public bool PageRightEnabled
        {
            get
            {
                return (bool)GetValue(PageRightEnabledProperty);
            }

            set
            {
                this.SetValue(PageRightEnabledProperty, value);
            }
        }

        /// <summary>
        /// Called when the KinectSensorChooser gets a new sensor
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="args">event arguments</param>
        private static void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {
            if (args.OldSensor != null)
            {
                try
                {
                    args.OldSensor.DepthStream.Range = DepthRange.Default;
                    args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }

            if (args.NewSensor != null)
            {
                try
                {
                    args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    args.NewSensor.SkeletonStream.Enable();

                    try
                    {
                        args.NewSensor.DepthStream.Range = DepthRange.Near;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
                    }
                    catch (InvalidOperationException)
                    {
                        // Non Kinect for Windows devices do not support Near mode, so reset back to default mode.
                        args.NewSensor.DepthStream.Range = DepthRange.Default;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    }
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
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
            this.sensorChooser.Stop();
        }

        /// <summary>
        /// Handle a button click from theme panel
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        /// Funcion que se llama cuando se pulsa uno de los temas
        private void KinectTileButtonClick(object sender, RoutedEventArgs e)
        {

            var button = (KinectTileButton)e.OriginalSource;
/*Si tengo una dificultad y no tengo tema, actualizo tema al pulsado y ya no dejo que se seleccione mas temas 
hasta que se acabe la ronda. En el cuadro de preguntas nos habilita para que pulsemos y aparezcan las preguntas,
y por ultimo activará la visibilidad del fondo y pone la imagen seleccionada en el tema*/
            if (tema == null && dificultad != null)
            {
                tema = (button.Label as string);
                this.Imagen_pregunta.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("S_PREGUNTA.png"))); 
                this.fondo.Visibility = Visibility.Visible; 
                this.fondo.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath(tema + ".png")));
            }
            e.Handled = true;
        }

        /// <summary>
        /// Handle a button click from abc panel
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        /// Botones de las opciones
        /// 
        private void KinectTileButtonClick2(object sender, RoutedEventArgs e)       {
            var button = (KinectTileButton)e.OriginalSource;
  /*Comprobamos si hemos pulsado para seleccionar dificultad u opcion. Para ello si es seleccionando dificultad,
  lo comprobamos viendo que esta tema== null, opcion null y no hay dificutad ya elegida. Si no hay tema ni dificultad y 
  pulsamos tema , este seguira siendo null por el metedo anterior.*/
            if (tema == null && dificultad == null && opcion == null)            {
                dificultad = (button.Label as string);
                this.Imagen_pregunta.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("S_Tema.png")));//aparezca que seleccionemos tema
            }//si lo que marcamos es una respuesta a una pregunta.
            else if (tema != null && opcion == null && dificultad != null && !primera)            {
                opcion = (button.Label as string);
                comprobarPregunta();
                if(n_pregunta<5) //si estamos en ronda salga la siguiente pregunta sino que hemos acabado ronda               
                    this.Imagen_pregunta.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("S_Pregunta.png")));
                if(n_pregunta==5)
                    this.Imagen_pregunta.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("RONDA.png")));

            }
            e.Handled = true;
        }


        /// <summary>
        /// Handle paging right (next button).
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void PageRightButtonClick(object sender, RoutedEventArgs e)
        {
            scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + PixelScrollByAmount);
        }

        /// <summary>
        /// Handle paging left (previous button).
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void PageLeftButtonClick(object sender, RoutedEventArgs e)
        {
            scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - PixelScrollByAmount);
        }

        /// <summary>
        /// Change button state depending on scroll viewer position
        /// </summary>
        private void UpdatePagingButtonState()
        {
            this.PageLeftEnabled = scrollViewer.HorizontalOffset > ScrollErrorMargin;
            this.PageRightEnabled = scrollViewer.HorizontalOffset < scrollViewer.ScrollableWidth - ScrollErrorMargin;
        }


     /// <summary>
     /// Metodo para crear los botones de los temas
     /// </summary>
     /// <param name="nombre"></param>
     /// <returns></returns>
        private KinectTileButton crearBoton(String nombre)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(nombre + ".png", UriKind.Relative);
            bi.EndInit();
            var button = new KinectTileButton
            {
                Label = nombre.ToString(CultureInfo.CurrentCulture),
                FontSize = 35,
                Background = new ImageBrush(bi),
                LabelBackground = new ImageBrush(),
                Height = 150,
                Width = 200,
                VerticalLabelAlignment = VerticalAlignment.Center,
                HorizontalLabelAlignment = HorizontalAlignment.Center,
            };
            return button;
        }

        /// <summary>
        /// Método para crear los botones de las opciones
        /// </summary>
        /// <param name="nombre"></param>
        /// <returns></returns>

        private KinectTileButton crearBoton2(String nombre)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(nombre + ".png", UriKind.Relative);
            bi.EndInit();
            var button = new KinectTileButton
            {
                Label = nombre.ToString(CultureInfo.CurrentCulture),
                FontSize = 1,
                Background = new ImageBrush(bi),
                LabelBackground = new ImageBrush(),
                Height = 150,
                Width = 150,
                VerticalLabelAlignment = VerticalAlignment.Center,
                HorizontalLabelAlignment = HorizontalAlignment.Center,
            };
            return button;
        }

        /// <summary>
        /// Método para pulsar en la zona de las preguntas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void FormulaPregunta(object sender, RoutedEventArgs e)
        {
            var button = (KinectTileButton)e.OriginalSource;
            //Si pulsamos y tenemos un tema y una dificultad ya aparezcan las preguntas
            if (tema != null && primera && dificultad != null)
            {
                this.Imagen_pregunta.Visibility = Visibility.Visible;

                this.Imagen_pregunta.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath(tema + n_pregunta + dificultad + ".png")));
                primera = false;
            }// si estamos en un pregunta dentro de la ronda y no es la ultima, aumentar el contador de preguntas, poner la opcion anterior a nulo y ocultar el icono de si es correcto
            else if (n_pregunta < 5 && tema != null && opcion != null && dificultad != null)
            {
                n_pregunta++;
                this.Imagen_pregunta.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath(tema + n_pregunta + dificultad + ".png")));
                this.correcto.Visibility = Visibility.Hidden;
                opcion = null;

            }//Si estabamos en la ultima pregunta, reiniciar todas las variables y comenzar desde el principio a pedir dificultad...
            else if (n_pregunta >= 5 && tema != null && dificultad != null)
            {
                this.Imagen_pregunta.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("DIFICULTAD.png")));
                this.fondo.Visibility = Visibility.Hidden;//Ocultamos fondo hasta tener nuevo tema
                tema = null;
                dificultad = null;
                n_correctas = 0;
                this.ContadorResultados.Text = n_correctas + "/5"; //Nos muestre que hemos reiniciado el contador
                n_pregunta = 1;
                this.correcto.Visibility = Visibility.Hidden;//Ocultamos el icono de correcto
                opcion = null;
                primera = true;
            }

            e.Handled = true;
        }

        /// <summary>
        /// Método que comprueba para una dificultad, un tema, un numero de pregunta si la opción es correcta
        /// si es correcta, aumenta el contador de respuestas acertadas y lo muestra y pone el icono de correcto,
        /// si no es correcta muestra el icono de incorrecta
        /// </summary>
        private void comprobarPregunta()
        {
            if (dificultad == "A")
            {
                if (tema == "DEPORTES")
                {
                    if (n_pregunta == 1)
                    {
                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 2)
                    {

                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 3)
                    {

                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 4)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 5)
                    {

                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }

                }

                else if (tema == "HISTORIA")
                {

                    if (n_pregunta == 1)
                    {
                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 2)
                    {

                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 3)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 4)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 5)
                    {

                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                }
                else if (tema == "ARTE")
                {

                    if (n_pregunta == 1)
                    {
                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 2)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 3)
                    {

                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 4)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 5)
                    {

                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                }
                else if (tema == "LITERATURA")
                {

                    if (n_pregunta == 1)
                    {
                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 2)
                    {

                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 3)
                    {

                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 4)
                    {

                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 5)
                    {

                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                }
                else if (tema == "GEOGRAFIA")
                {

                    if (n_pregunta == 1)
                    {
                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 2)
                    {

                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 3)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 4)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 5)
                    {

                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                }else if (tema == "CIENCIA")
                {
                    if (n_pregunta == 1)
                    {
                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 2)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 3)
                    {

                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 4)
                    {

                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 5)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }

                }



            }
            else if(dificultad == "B")
            {
                if (tema == "DEPORTES")
                {
                    if (n_pregunta == 1)
                    {
                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 2)
                    {

                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 3)
                    {

                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 4)
                    {

                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 5)
                    {

                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }

                }

                else if (tema == "HISTORIA")
                {

                    if (n_pregunta == 1)
                    {
                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 2)
                    {

                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 3)
                    {

                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 4)
                    {

                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 5)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                }
                else if (tema == "ARTE")
                {

                    if (n_pregunta == 1)
                    {
                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 2)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 3)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 4)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 5)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                }
                else if (tema == "LITERATURA")
                {

                    if (n_pregunta == 1)
                    {
                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 2)
                    {

                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 3)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 4)
                    {

                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 5)
                    {

                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                }
                else if (tema == "GEOGRAFIA")
                {

                    if (n_pregunta == 1)
                    {
                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 2)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 3)
                    {

                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 4)
                    {

                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 5)
                    {

                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                }else if (tema == "CIENCIA")
                {
                    if (n_pregunta == 1)
                    {
                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 2)
                    {

                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 3)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 4)
                    {

                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 5)
                    {

                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }

                }



            }
            else if(dificultad=="C")
            {
                if (tema == "DEPORTES")
                {
                    if (n_pregunta == 1)
                    {
                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 2)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 3)
                    {

                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 4)
                    {

                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 5)
                    {

                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }

                }

                else if (tema == "HISTORIA")
                {

                    if (n_pregunta == 1)
                    {
                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 2)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 3)
                    {

                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 4)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 5)
                    {

                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                }else if (tema == "CIENCIA")
                {
                    if (n_pregunta == 1)
                    {
                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 2)
                    {

                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 3)
                    {

                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 4)
                    {

                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 5)
                    {

                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }

                }

                else if (tema == "ARTE")
                {

                    if (n_pregunta == 1)
                    {
                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 2)
                    {

                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 3)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 4)
                    {

                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 5)
                    {

                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                }
                else if (tema == "LITERATURA")
                {

                    if (n_pregunta == 1)
                    {
                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 2)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 3)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 4)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 5)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                }
                else if (tema == "GEOGRAFIA")
                {

                    if (n_pregunta == 1)
                    {
                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 2)
                    {

                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 3)
                    {

                        if (opcion == "C")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 4)
                    {

                        if (opcion == "A")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                    else if (n_pregunta == 5)
                    {

                        if (opcion == "B")
                        {
                            n_correctas++;
                            this.ContadorResultados.Text = n_correctas + "/5";
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("correcto.png")));
                        }
                        else
                        {
                            this.correcto.Visibility = Visibility.Visible;
                            this.correcto.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("incorrecto.png")));

                        }

                    }
                }


            }



        }










    }
}
