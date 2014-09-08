using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using System.Linq;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.TheGRID;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Input;
using AlumnoEjemplos.TheGRID.Camara;
using TgcViewer.Utils.Terrain;
using Microsoft.DirectX.DirectInput;

namespace AlumnoEjemplos.MiGrupo
{
    public class EjemploAlumno : TgcExample
    {
        /// Categor�a a la que pertenece el ejemplo.
        /// Influye en donde se va a ver en el �rbol de la derecha de la pantalla.
        public override string getCategory(){ return "AlumnoEjemplos"; }
        /// Completar nombre del grupo en formato Grupo NN
        public override string getName(){ return "Grupo TheGRID"; }
        /// Completar con la descripci�n del TP
        public override string getDescription(){return "Viaje Interplanetario - Manejo: \nArriba/Abajo - Pitch                       \nIzq/Der - Roll                                  \nA/D - Yaw                 \nW - Acelerar                  \nS - Estabilizar                             \nEspacio - Disparo Principal";}
        //--------------------------------------------------------
        
        // ATRIBUTOS

        static EjemploAlumno singleton;
        TgcBox suelo;
        Dibujable nave;
        Dibujable objetoPrincipal;  //Este va a ser configurable con el panel de pantalla.

        List<Dibujable> listaDibujable = new List<Dibujable>();
        float timeLaser = 0;
        const float betweenTime = 0.15f;
        ManagerLaser laserManager;
        ManagerAsteroide asteroidManager;

        //Modificador de la camara del proyecto
        CambioCamara camara;
        TgcSkyBox skyBox;
        TgcArrow arrow;

        //--------------------------------------------------------

        public static EjemploAlumno workspace() { return singleton; }

        public override void init()
        {
            EjemploAlumno.singleton = this;
            //GuiController.Instance: acceso principal a todas las herramientas del Framework
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            //Carpeta de archivos Media del alumno
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
            singleton = this;


            d3dDevice.Clear(ClearFlags.Target, Color.FromArgb(22, 22, 22), 1.0f, 0);
            /*
            //Crear SkyBox 
            skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(0, 0, 0);
            skyBox.Size = new Vector3(15000, 15000, 150000);


            //Crear suelo
            TgcTexture pisoTexture = TgcTexture.createTexture(d3dDevice, alumnoMediaFolder + "TheGrid\\SkyBox\\adelante.jpg");
            suelo = TgcBox.fromSize(new Vector3(0, 0, 9500), new Vector3(1000, 1000, 0), pisoTexture);   

            //Configurar color
            //skyBox.Color = Color.OrangeRed;

            //Configurar las texturas para cada una de las 6 caras
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, alumnoMediaFolder + "TheGrid\\SkyBox\\arriba.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, alumnoMediaFolder + "TheGrid\\SkyBox\\abajo.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, alumnoMediaFolder + "TheGrid\\SkyBox\\izquierda.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, alumnoMediaFolder + "TheGrid\\SkyBox\\derecha.jpg");

            //Hay veces es necesario invertir las texturas Front y Back si se pasa de un sistema RightHanded a uno LeftHanded
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, alumnoMediaFolder + "TheGrid\\SkyBox\\adelante.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, alumnoMediaFolder + "TheGrid\\SkyBox\\atras.jpg");
            
            //Actualizar todos los valores para crear el SkyBox
            skyBox.updateValues();
            */

            //Crear manager Lasers
            laserManager = new ManagerLaser(50);
            
            //Crear 5 asteroides
            asteroidManager = new ManagerAsteroide(15);
            asteroidManager.creaUno();
            asteroidManager.fabricar(5);
            
            //Crear la nave
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene;
            nave = new Dibujable(0, 0, 0);
            scene = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "TheGrid\\Nave\\nave3-TgcScene.xml");
            nave.setObject(scene.Meshes[0], 200, 100, new Vector3(0, 180, 0), new Vector3(0.5f, 0.5f, 0.5f));
            nave.setFisica(100, 500, 100);
            nave.SetPropiedades(true, false, false);

            //Cargamos la nave como objeto principal.
            objetoPrincipal = nave;
            //Cargamos la camara
            camara = new CambioCamara(nave);

            //Flecha direccion objetivo
            arrow = new TgcArrow();
            //arrow.BodyColor = Color.Blue;
            //arrow.HeadColor = Color.Yellow;
            arrow.BodyColor = Color.FromArgb(230, Color.Cyan);
            arrow.HeadColor = Color.FromArgb(230, Color.Yellow);

            //Cargamos valores en el panel lateral
            GuiController.Instance.UserVars.addVar("Vel-Actual:");
            GuiController.Instance.UserVars.addVar("Integtidad Nave:");
            GuiController.Instance.UserVars.addVar("Integridad Escudos:");
            GuiController.Instance.UserVars.addVar("Posicion X:");
            GuiController.Instance.UserVars.addVar("Posicion Y:");
            GuiController.Instance.UserVars.addVar("Posicion Z:");
            //Cargar valor en UserVar
            GuiController.Instance.UserVars.setValue("Vel-Actual:", objetoPrincipal.velocidadActual());
            GuiController.Instance.UserVars.setValue("Integtidad Nave:", 100);
            GuiController.Instance.UserVars.setValue("Integridad Escudos:", 100);
            GuiController.Instance.UserVars.setValue("Posicion X:", objetoPrincipal.getPosicion().X);
            GuiController.Instance.UserVars.setValue("Posicion Y:", objetoPrincipal.getPosicion().Y);
            GuiController.Instance.UserVars.setValue("Posicion Z:", objetoPrincipal.getPosicion().Z);
            //Crear un modifier para un valor FLOAT
            GuiController.Instance.Modifiers.addFloat("Aceleracion", 0f,500f, objetoPrincipal.getAceleracion());
            GuiController.Instance.Modifiers.addFloat("Frenado", 0f, 1000f, objetoPrincipal.getAcelFrenado());
            //Crear un modifier para un ComboBox con opciones
            string[] opciones1 = new string[] { "Tercera Persona", "Camara FPS", "Libre" };
            GuiController.Instance.Modifiers.addInterval("Tipo de Camara", opciones1, 0);
            string[] opciones2 = new string[] { "Desactivado", "Activado" };
            GuiController.Instance.Modifiers.addInterval("Velocidad Manual", opciones2, 1);
            string[] opciones3 = new string[] { "Activado", "Desactivado" };
            GuiController.Instance.Modifiers.addInterval("Desplaz. Avanzado", opciones3, 1);
            string[] opciones4 = new string[] { "Activado", "Desactivado" };
            GuiController.Instance.Modifiers.addInterval("Rotacion Avanzada", opciones4, 1);            
        }
        //--------------------------------------------------------RENDER-----

        // <param name="elapsedTime">Tiempo en segundos transcurridos desde el �ltimo frame</param>
        public override void render(float elapsedTime)
        {
            //-----UPDATE-----
            TgcD3dInput input = GuiController.Instance.D3dInput;

            //Flechas
            if (input.keyDown(Key.Left)) { nave.rotacion = 1; }
            if (input.keyDown(Key.Right)) { nave.rotacion = -1; }
            if (input.keyDown(Key.Up)) { nave.inclinacion = 1; }
            if (input.keyDown(Key.Down)) { nave.inclinacion = -1; }
            //Letras
            if (input.keyDown(Key.A)) { nave.giro = -1; }
            if (input.keyDown(Key.D)) { nave.giro = 1; }
            if (input.keyDown(Key.W)) { nave.acelerar(); }
            if (input.keyDown(Key.S)) { nave.frenar(); }
            //Otros
            //if (input.keyDown(Key.LeftShift)) { nave.acelerar(1); }

            //if (input.keyDown(Key.F1)) { camara.modoFPS(); }
            //if (input.keyDown(Key.F2)) { camara.modoExterior(); }
            //if (input.keyDown(Key.F3)) { camara.modoTPS(); }

            if (input.keyDown(Key.P)) { asteroidManager.explotaAlPrimero(); }
            if (input.keyDown(Key.O)) { asteroidManager.creaUno(); }

            camara.cambiarPosicionCamara();

            
            if (input.keyDown(Key.Space))
            {
                timeLaser += elapsedTime;
                if (timeLaser > betweenTime)
                {
                    laserManager.fabricar(nave.Transform, nave.getEjes(),nave.getPosicion(),nave.getDireccion());                  
                    timeLaser = 0;
                }
            }          
                       
            //-----FIN-UPDATE-----


            //Device de DirectX para renderizar
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            d3dDevice.Clear(ClearFlags.Target, Color.FromArgb(22, 22, 22), 1.0f, 0);

            laserManager.operar(elapsedTime);
            asteroidManager.operar(elapsedTime);

            //Cargar valores de la flecha
            Vector3 navePos = nave.getCentro();
            Vector3 naveDir = Vector3.Subtract(new Vector3(0, 0, 10000), nave.getDireccion());
            naveDir.Normalize();
            naveDir.Multiply(75);
            arrow.PStart = navePos;
            arrow.PEnd = navePos + naveDir;
            arrow.Thickness = 0.5f;
            arrow.HeadSize = new Vector2(2,2);
            arrow.updateValues();
            arrow.render();
            
            //skyBox.render();
            //suelo.render();

            nave.rotar(elapsedTime,listaDibujable);
            nave.desplazarse(elapsedTime,listaDibujable);
            if(!camara.soyFPS())
                nave.render();

            //Refrescar panel lateral
            //case
            string opcionElegida = (string)GuiController.Instance.Modifiers["Tipo de Camara"];
            camara.chequearCambio(opcionElegida);
            opcionElegida = (string)GuiController.Instance.Modifiers["Velocidad Manual"];
            if (String.Compare(opcionElegida, "Activado") == 0) objetoPrincipal.velocidadManual = true; else objetoPrincipal.velocidadManual = false;
            opcionElegida = (string)GuiController.Instance.Modifiers["Desplaz. Avanzado"];
            if (String.Compare(opcionElegida, "Activado") == 0) objetoPrincipal.desplazamientoReal = true; else objetoPrincipal.desplazamientoReal = false;
            opcionElegida = (string)GuiController.Instance.Modifiers["Rotacion Avanzada"];
            if (String.Compare(opcionElegida, "Activado") == 0) objetoPrincipal.rotacionReal = true; else objetoPrincipal.rotacionReal = false;
            //Refrescar User Vars
            GuiController.Instance.UserVars.setValue("Vel-Actual:", objetoPrincipal.velocidadActual());
            GuiController.Instance.UserVars.setValue("Posicion X:", objetoPrincipal.getPosicion().X);
            GuiController.Instance.UserVars.setValue("Posicion Y:", objetoPrincipal.getPosicion().Y);
            GuiController.Instance.UserVars.setValue("Posicion Z:", objetoPrincipal.getPosicion().Z);
            //Obtener valores de Modifiers
            objetoPrincipal.fisica.aceleracion = (float)GuiController.Instance.Modifiers["Aceleracion"];
            objetoPrincipal.fisica.acelFrenado = (float)GuiController.Instance.Modifiers["Frenado"];
        }

        public override void close()
        {
           // laser.dispose();
            nave.dispose();
            //suelo.dispose();

        }





        public void metodoUselessInit()
        {
            ///////////////USER VARS//////////////////
            //Crear una UserVar
            GuiController.Instance.UserVars.addVar("variablePrueba");
            //Cargar valor en UserVar
            GuiController.Instance.UserVars.setValue("variablePrueba", 5451);

            ///////////////MODIFIERS//////////////////
            //Crear un modifier para un valor FLOAT
            GuiController.Instance.Modifiers.addFloat("valorFloat", -50f, 200f, 0f);
            //Crear un modifier para un ComboBox con opciones
            string[] opciones = new string[] { "opcion1", "opcion2", "opcion3" };
            GuiController.Instance.Modifiers.addInterval("valorIntervalo", opciones, 0);
            //Crear un modifier para modificar un v�rtice
            GuiController.Instance.Modifiers.addVertex3f("valorVertice", new Vector3(-100, -100, -100), new Vector3(50, 50, 50), new Vector3(0, 0, 0));

            ///////////////CONFIGURAR CAMARA ROTACIONAL//////////////////
            //Es la camara que viene por default, asi que no hace falta hacerlo siempre
            GuiController.Instance.RotCamera.Enable = true;
            //Configurar centro al que se mira y distancia desde la que se mira
            GuiController.Instance.RotCamera.setCamera(new Vector3(0, 0, 0), 100);

            /*
            ///////////////CONFIGURAR CAMARA PRIMERA PERSONA//////////////////
            //Camara en primera persona, tipo videojuego FPS
            //Solo puede haber una camara habilitada a la vez. Al habilitar la camara FPS se deshabilita la camara rotacional
            //Por default la camara FPS viene desactivada
            GuiController.Instance.FpsCamera.Enable = true;
            //Configurar posicion y hacia donde se mira
            GuiController.Instance.FpsCamera.setCamera(new Vector3(0, 0, -20), new Vector3(0, 0, 0));
            */

            ///////////////LISTAS EN C#//////////////////
            //crear
            List<string> lista = new List<string>();
            //agregar elementos
            lista.Add("elemento1");
            lista.Add("elemento2");
            //obtener elementos
            string elemento1 = lista[0];
            //bucle foreach
            foreach (string elemento in lista)
            {
                //Loggear por consola del Framework
                GuiController.Instance.Logger.log(elemento);
            }
            //bucle for
            for (int i = 0; i < lista.Count; i++)
            {
                string element = lista[i];
            }
        }

        public void metodoUselessRender()
        {
            //Obtener valor de UserVar (hay que castear)
            int valor = (int)GuiController.Instance.UserVars.getValue("variablePrueba");
            //Obtener valores de Modifiers
            float valorFloat = (float)GuiController.Instance.Modifiers["valorFloat"];
            string opcionElegida = (string)GuiController.Instance.Modifiers["valorIntervalo"];
            Vector3 valorVertice = (Vector3)GuiController.Instance.Modifiers["valorVertice"];
            ///////////////INPUT//////////////////
            //conviene deshabilitar ambas camaras para que no haya interferencia
            //Capturar Input teclado 
            if (GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.F))
            {
                //Tecla F apretada
            }
            //Capturar Input Mouse
            if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Boton izq apretado
            }
        }
    }
}

