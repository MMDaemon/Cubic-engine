using OpenTK;

namespace CubicEngine.View
{
	public class CameraFirstPerson
	{
		public CameraFirstPerson()
		{
			FovY = 90;
			Aspect = 1;
			NearClip = 0.1f;
			FarClip = 1;

			Position = Vector3.Zero;
			Tilt = 0;
			Heading = 0;
		}

		public float FovY { get; set; }
		public float Aspect { get; set; }
		public float NearClip { get; set; }
		public float FarClip { get; set; }

		public Vector3 Position { get; set; }
		public float Heading { get; set; }
		public float Tilt { get; set; }

		public Matrix4 CalcMatrix()
		{
			FovY = MathHelper.Clamp(FovY, 0.1f, 180);
			var mtxProjection = Matrix4.Transpose(Matrix4.CreatePerspectiveFieldOfView(
				MathHelper.DegreesToRadians(FovY),
				Aspect, NearClip, FarClip));

			Matrix4 rotationZ = Matrix4.CreateRotationZ(Tilt);
			Matrix4 rotationY = Matrix4.CreateRotationY(Heading);
			Matrix4 rotation = rotationY * rotationZ;
			Vector3 target = new Vector3(rotation.M11, rotation.M21, rotation.M31);
			var mtxLookAt = Matrix4.Transpose(Matrix4.LookAt(Position, Position + target, Vector3.UnitY));

			var camera = mtxProjection * mtxLookAt;
			return camera;
		}
	}
}
