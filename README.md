# GAM531-openTk-assignment2
1. Which library you used (SharpDX or OpenTK)
   
   with OpenTk.Mathematics


2. Which operations you implemented

   applied only rotation and scaling to my rectangle

   **
//rotation
Matrix4 rotationMatrix = Matrix4.CreateRotationY(rotationAngle);

//Scaling
Matrix4 scaleMatrix = Matrix4.CreateScale(scaleFactor);

//Model matrix : combine translation, rotation, scaling
Matrix4 model = scaleMatrix * rotationMatrix;
