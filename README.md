# GAM531-openTk-assignment2
1. Which library you used (SharpDX or OpenTK)
   
   with OpenTk.Mathematics


2. Which operations you implemented

   *vector in my code is a and b, but represented by x1 and x2 in this file*

  vector operations implemented:

  
     -addition: adds two vector together, for example : (x1+x2, y1+y2, z1+z2).
  
     -subtraction: subtracts two vector together, for example : (x1-x2, y1-y2, z1-z2).
  
     -dot: how much the two vectors point in the same direction, which uses formula like (x1*x2 + y1*y2 + z1*z2).
  
     -cross: finds a point(vector) that is perpendicular to vector x1 and x2, which uses formula like (y1*z2 - z1*y2, z1*x2 - x1*z2, x1*y2 - y1*x2).
  

  matrix operation implemented:

  
     -indentity: does nothing and prints diagonal elements of 1, but it's a starting point for generating other transformation matrices.
  
     -scale: scales all elements by scale value ( basically multiply each coordinates by scale value).
  
     -rotation: rotates vectors by 45 degree along Y-axis.
  
     -multiplication: to combine different transformations in 3D space, i did scaling and rotation.
  

     -lastly Apply matrix to vector: this applies the matrix to vector, in my code: it rotates vectors by 45 degrees around Y-axis, than scale it by 2 ( and I believe if I switch the order of transformations, it will scale first then rotate).
