using OpenTK.Mathematics;

class Program
{
    static void Main()
    {
        // create vectors
        Vector3 a = new Vector3(3, 5, 7);
        Vector3 b = new Vector3(2, 4, 6);

        // vector operations
        Vector3 add = a + b;
        Vector3 sub = a - b;
        float dot = Vector3.Dot(a, b);
        Vector3 cross = Vector3.Cross(a, b);

        //print VO results
        Console.WriteLine("Vecotr Operation Results");
        Console.WriteLine($"Addition: {add}");
        Console.WriteLine($"Subtraction: {sub}");
        Console.WriteLine($"Dot: {dot}");
        Console.WriteLine($"Cross = {cross}");

        //matrix operations
        Matrix4 identity = Matrix4.Identity;
        Matrix4 scale = Matrix4.CreateScale(2.0f);
        Matrix4 rotationY = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(45));
        Matrix4 multiply = Matrix4.Mult(scale, rotationY);

        // apply matrix to vector
        Vector3 result = Vector3.TransformPosition(a, multiply);

        //print MO results
        Console.WriteLine("Matrix Operation Results");
        Console.WriteLine($"Identity: {identity}");
        Console.WriteLine($"Scale: {scale}");
        Console.WriteLine($"Rotation on Y axis: {rotationY}");
        Console.WriteLine($"Multiplication: {multiply}");
        Console.WriteLine($"Applying matrix to vector: {result}");
    }
}
