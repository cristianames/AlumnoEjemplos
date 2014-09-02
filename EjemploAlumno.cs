using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.TheGRID;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Input;

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
        public override string getDescription(){return "Viaje Interplanetario - Misma idea";}
        //--------------------------------------------------------
        // ATRIBUTOS
        TgcBox suelo;
        Dibujable asteroide;
        //Dibujable caja;
        Dibujable nave;
        List<Dibujable> laserLista;
        
        


        //--------------------------------------------------------
        public override void init()
        {
            laserLista=new List<Dibujable>();
            //GuiController.Instance: acceso principal a todas las herramientas del Framework
            Device d3dDevice = GuiController.Instance.D3dDevice;
            //Carpeta de archivos Media del alumno
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;           
            //Crear suelo
            TgcTexture pisoTexture = TgcTexture.createTexture(d3dDevice, GuiController.Instance.ExamplesMediaDir + "Texturas\\Quake\\TexturePack2\\rock_floor1.jpg");
            suelo = TgcBox.fromSize(new Vector3(0, -5, 0), new Vector3(500, 0, 500), pisoTexture);            
            //Crear 1 asteroide

            Factory fabrica_dibujables = new Factory();

            asteroide = fabrica_dibujables.crearAsteroide(new Vector3(1, 1, 1));
            fabrica_dibujables.trasladar(asteroide, new Vector3(200, 100, 50));
            GuiController.Instance.RotCamera.targetObject(((TgcMesh)asteroide.objeto).BoundingBox);

           
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "Laser\\Laser_Box-TgcScene.xml");           
            nave = new Dibujable();
            scene = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "Nave\\nave-TgcScene.xml");
            nave.objeto = scene.Meshes[0];
            nave.velocidadRadial = 5;
            nave.velocidad = 50;
            //((TgcMesh)nave.objeto).AutoTransformEnable = false;
            GuiController.Instance.RotCamera.targetObject(suelo.BoundingBox);

          
            
            //Configurar camara en Tercer Persona
            /*GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(nave.Position, 30, -75);*/

            // Prueba vectores
            Vector3 dePrueba = new Vector3(0, 1, 0);
            //z.Normalize();
            float angulo = Geometry.DegreeToRadian(90);
            Matrix rotation = Matrix.RotationZ(angulo);
            //Matrix rotation = Matrix.RotationYawPitchRoll(0 * z.Y * angulo, 0 * z.X * angulo, z.Z * angulo);
            Vector4 normal4 = Vector3.Transform(dePrueba, rotation);
            Vector3 resultado = new Vector3(normal4.X, normal4.Y, normal4.Z);
            
            //-------------------

            GuiController.Instance.UserVars.addVar("Direccion-X");
            GuiController.Instance.UserVars.addVar("Direccion-Y");
            GuiController.Instance.UserVars.addVar("Direccion-Z");
            GuiController.Instance.UserVars.addVar("Normal-X");
            GuiController.Instance.UserVars.addVar("Normal-Y");
            GuiController.Instance.UserVars.addVar("Normal-Z");
            //Cargar valor en UserVar
            Vector3 temp = nave.direccion.getActual();
            GuiController.Instance.UserVars.setValue("Direccion-X", dePrueba.X);
            GuiController.Instance.UserVars.setValue("Direccion-Y", dePrueba.Y);
            GuiController.Instance.UserVars.setValue("Direccion-Z", dePrueba.Z);
            temp = nave.normal.getActual();
            GuiController.Instance.UserVars.setValue("Normal-X", resultado.X);
            GuiController.Instance.UserVars.setValue("Normal-Y", resultado.Y);
            GuiController.Instance.UserVars.setValue("Normal-Z", resultado.Z);

        }
        //--------------------------------------------------------

        // <param name="elapsedTime">Tiempo en segundos transcurridos desde el �ltimo frame</param>
        public override void render(float elapsedTime)
        {
            //-----UPDATE-----
            //Flechas
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Left)) { nave.rotacion = 1; }
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Right)) { nave.rotacion = -1; }
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Up)) { nave.inclinacion = -1; }
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Down)) { nave.inclinacion = 1; }
            //Letras
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.A)) { }
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.D)) { }
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.W)) { }
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.S)) { }
            //Otros
            if (GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.Space)) 
            {
                if (nave.traslacion == 0) nave.traslacion = 1;
                else nave.traslacion = 0;  
            }
            if (GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.LeftControl))
            {
                if (nave.traslacion == 0) nave.traslacion = -1;
                else nave.traslacion = 0;  
            }


            Factory fabrica = new Factory();
            Vector3 direccion = new Vector3(0,0,1);// hay que ajustar la direccion a la verdadera direccion que tiene el "ca�on"
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.A))
            {

                laserLista.Add(fabrica.crearLaser(nave.Position)); //arreglar haz de laser infinito, agregar colisiones
            }            
            if (laserLista.Count != 0)
            {

                foreach (Dibujable laser in laserLista)
                {
                    fabrica.dispararLaser(laser, direccion,elapsedTime);

                }
            }
                       
            //-----FIN-UPDATE-----


            //Device de DirectX para renderizar
            Device d3dDevice = GuiController.Instance.D3dDevice;

          
            //laser.trasladar(elapsedTime);

            if (laserLista.Count != 0)
            {

                foreach (Dibujable laser in laserLista)
                {
                    laser.render();

                }
            }
           
            asteroide.render();
            asteroide.renderBoundingBox();

            suelo.render();
            nave.rotar(elapsedTime);
            nave.trasladar(elapsedTime);
            nave.render();
        }

        public override void close()
        {

            nave.dispose();
           // laser.dispose();

            asteroide.dispose();
            suelo.dispose();

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
            /*
            //Creamos una caja 3D de color rojo, ubicada en el origen y lado 10
            Vector3 center = new Vector3(0, 0, 0);
            Vector3 size = new Vector3(10, 10, 10);
            Color color = Color.Red;
            caja = new Dibujable();
            caja.objeto = TgcBox.fromSize(center, size, color);
            TgcBox cajita = (TgcBox)caja.objeto;
            //GuiController.Instance.RotCamera.targetObject(cajita.BoundingBox);
            */
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

