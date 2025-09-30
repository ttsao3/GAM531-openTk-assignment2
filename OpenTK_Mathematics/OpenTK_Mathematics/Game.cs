using System;
using OpenTK;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace WindowOpenTK
{
    public class Game : GameWindow
    {
        private int vertexBufferHandle;
        private int shaderProgramHandle;
        private int vertexArrayHandle;

        private int modelLoc, viewLoc, projLoc;

        private float rotationAngle = 0.5f;
        private float scaleFactor = 1.0f;
        private bool scalingUp = true;

        // constructor for the game class
        public Game()
              : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            //set window size to 1280x768
            this.CenterWindow(new Vector2i(1280, 768));

            // Center the window on the screen
            this.CenterWindow(this.Size);
        }

        // Called automatically whenever the window is resized
        protected override void OnResize(ResizeEventArgs e)
        {
            // Update the OpenGL viewport to match the new window dimensions
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }

        // Called once when the game starts, ideal for loading resources
        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(new Color4(0.2f, 0.4f, 0.8f, 1f));

            float[] vertices = new float[]
            {

                -0.8f, 0.5f, 0.0f, //top left vertex
                -0.8f, -0.5f, 0.0f, //bottom left vertex
                0.8f, 0.5f, 0.0f, //top right vertex
                0.8f, -0.5f, 0.0f //bottom right vertex                
                
            };

            // generate a vertex buffer object (VBO) to store vertex data on GPU
            vertexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // generate a vertex array object (VAO) to store the VBO configuration
            vertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayHandle);

            // bind the VBO and define the layout of vertex data for sharders
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            // vertext sharder : position of each vertex + model, view, projection matrices
            string vertexShaderCode =
                @"
                    #version 330 core
                    layout(location=0) in vec3 aPosition; //vertex position input
                    
                    uniform mat4 uModel;
                    uniform mat4 uView;
                    uniform mat4 uProj;

                    void main()
                    {
                        gl_Position =  uProj * uView * uModel * vec4(aPosition, 1.0); //transform vertex position
                    }
                ";

            //fragment shader : outputs a single color
            string fragmentShaderCode =
                @"
                    #version 330 core
                    out vec4 fragColor; //output color  
                    void main()
                    {
                        fragColor = vec4(1.0, 0.5, 0.2, 1.0);
                    }
                ";

            //complier shaders
            int vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle, vertexShaderCode);
            GL.CompileShader(vertexShaderHandle);
            CheckShaderCompile(vertexShaderHandle, "Vertex Shader");

            int fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShaderHandle, fragmentShaderCode);
            GL.CompileShader(fragmentShaderHandle);
            CheckShaderCompile(fragmentShaderHandle, "Fragment Shader");

            // Create shader program and link shaders
            shaderProgramHandle = GL.CreateProgram();
            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, fragmentShaderHandle);
            GL.LinkProgram(shaderProgramHandle);

            // Cleanup shaders after linking (no longer needed individually)
            GL.DetachShader(shaderProgramHandle, vertexShaderHandle);
            GL.DetachShader(shaderProgramHandle, fragmentShaderHandle);
            GL.DeleteShader(vertexShaderHandle);
            GL.DeleteShader(fragmentShaderHandle);

            // Get uniform locations for model, view, projection matrices
            modelLoc = GL.GetUniformLocation(shaderProgramHandle, "uModel");
            viewLoc = GL.GetUniformLocation(shaderProgramHandle, "uView");
            projLoc = GL.GetUniformLocation(shaderProgramHandle, "uProj");
        }



        //called every from to update game logic, phisucs, or input handling
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            rotationAngle += (float)args.Time; //rotate 1 radian per second

            //oscillating scale between 0.5 and 1.5
            if (scalingUp)
            {
                scaleFactor += (float)args.Time; //increase scale
                if (scaleFactor >= 1.5f)
                    scalingUp = false;
            }
            else
            {
                scaleFactor -= (float)args.Time; //decrease scale
                if (scaleFactor <= 0.5f)
                    scalingUp = true;
            }
        }

        //called when i need to update any game visuals
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.UseProgram(shaderProgramHandle);

            //View matirx
            Matrix4 view = Matrix4.LookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.UnitY);

            //Projection matrix
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(60f),
                (float)Size.X / Size.Y,
                0.1f,
                100f
            );

            //send view and projection matrices to shaders
            GL.UniformMatrix4(viewLoc, false, ref view);
            GL.UniformMatrix4(projLoc, false, ref projection);

            GL.BindVertexArray(vertexArrayHandle);

            //Rotation quaternion 
            //Quaternion rotation = Quaternion.FromAxisAngle(Vector3.UnitY, rotationAngle);
            //Matrix4 rotationMatrix = Matrix4.CreateFromQuaternion(rotation);

            //rotation
            Matrix4 rotationMatrix = Matrix4.CreateRotationY(rotationAngle);

            //Scaling
            Matrix4 scaleMatrix = Matrix4.CreateScale(scaleFactor);

            //Model matrix : combine translation, rotation, scaling
            Matrix4 model = scaleMatrix * rotationMatrix;

            //send model matrix to shader
            GL.UniformMatrix4(modelLoc, false, ref model);
            //draw the square using triangle strip (4 vertices)
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

            //display the rendered frame
            GL.BindVertexArray(0);
            SwapBuffers();
        }

        // Called when the game is closing, ideal for cleaning up resources
        protected override void OnUnload()
        {
            //unbind and delete all the buffers and shader programs
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(vertexBufferHandle);
            GL.BindVertexArray(0);
            GL.DeleteVertexArray(vertexArrayHandle);
            GL.UseProgram(0);
            GL.DeleteProgram(shaderProgramHandle);

            base.OnUnload();
        }

        // Helper function to check for shader compilation errors
        private void CheckShaderCompile(int shaderHandle, string shaderName)
        {
            GL.GetShader(shaderHandle, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shaderHandle);
                Console.WriteLine($"Error compiling {shaderName}: {infoLog}");
            }
        }
    }
}
