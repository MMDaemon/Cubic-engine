using OpenTK;

namespace CubicEngine.View
{
	public class CameraOrbit
	{
		public CameraOrbit()
		{
			Aspect = 1;
			Distance = 1;
			FarClip = 1;
			FovY = 90;
			Heading = 0;
			NearClip = 0.1f;
			Target = Vector3.Zero;
			Tilt = 0;
		}

		public float Aspect { get; set; }
		public float Distance { get; set; }
		public float FarClip { get; set; }
		public float FovY { get; set; }
		public float Heading { get; set; }
		public float NearClip { get; set; }
		public Vector3 Target { get; set; }
		public float Tilt { get; set; }

		public Matrix4 CalcMatrix()
		{
			FovY = MathHelper.Clamp(FovY, 0.1f, 180);
			Distance = MathHelper.Clamp(Distance, NearClip, FarClip);
			var mtxProjection = Matrix4.Transpose(Matrix4.CreatePerspectiveFieldOfView(
				MathHelper.DegreesToRadians(FovY),
				Aspect, NearClip, FarClip));
			var mtxDistance = Matrix4.Transpose(Matrix4.CreateTranslation(0, 0, -Distance));
			var mtxTilt = Matrix4.Transpose(Matrix4.CreateRotationX(Tilt));
			var mtxHeading = Matrix4.Transpose(Matrix4.CreateRotationY(Heading));
			var mtxTarget = Matrix4.Transpose(Matrix4.CreateTranslation(-Target));
			var camera = mtxProjection * mtxDistance * mtxTilt * mtxHeading * mtxTarget;
			return camera;
		}
	}
}
