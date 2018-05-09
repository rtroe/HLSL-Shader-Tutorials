using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rtroe.tutorials.shaders
{
	public class Camera
	{
		/// <summary>
		/// The graphics device.
		/// </summary>
		GraphicsDevice GraphicsDevice;


        /// <summary>
        /// The Camera's viewport.
        /// </summary>
        public Viewport Viewport;


        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>The view.</value>
        public Matrix View
        {
            get { return _viewMatrix; }
            set
            {
                _viewMatrix = value;
                _viewProjection = _viewMatrix * _projectionMatrix;
            }
        }
        private Matrix _viewMatrix;


        /// <summary>
        /// Gets or sets the projection.
        /// </summary>
        /// <value>The projection.</value>
        public Matrix Projection
        {
            get { return _projectionMatrix; }
            set
            {
                _projectionMatrix = value;
                _viewProjection = _viewMatrix * _projectionMatrix;
            }
        }
        private Matrix _projectionMatrix;


        public Matrix ViewProjection
        {
            get { return _viewProjection; }
        }
        Matrix _viewProjection;


        public BoundingFrustum BoundingFrustum;


        /// <summary>
        /// Gets or sets the field of view.
        /// </summary>
        /// <value>The field of view.</value>
        public float FieldOfView
        {
            get { return _fieldOfView; }
            set { _fieldOfView = value; CalculateProjectionMatrix(); }
        }
        private float _fieldOfView;


        /// <summary>
        /// Gets or sets the aspect ratio.
        /// </summary>
        /// <value>The aspect ratio.</value>
        public float AspectRatio
        {
            get { return _aspectRatio; }
            set { _aspectRatio = value; CalculateProjectionMatrix(); }
        }
        private float _aspectRatio;


        /// <summary>
        /// Gets or sets the near plane.
        /// </summary>
        /// <value>The near plane.</value>
        public float NearPlane
        {
            get { return _nearPlane; }
            set { _nearPlane = value; CalculateProjectionMatrix(); }
        }
        private float _nearPlane;


        /// <summary>
        /// Gets or sets the far plane.
        /// </summary>
        /// <value>The far plane.</value>
        public float FarPlane
        {
            get { return _farPlane; }
            set { _farPlane = value; CalculateProjectionMatrix(); }
        }
        private float _farPlane;

        /// <summary>
        /// Focal Distance Used During Depth of Field Calculations.
        /// </summary>
        public float FocalDistance
        {
            get { return _focalDistance; }
            set { _focalDistance = value; }
        }
        float _focalDistance = 40;


        /// <summary>
        /// Focal Width Used in the Depth of Field Calculations.
        /// </summary>
        public float FocalWidth
        {
            get { return _focalWidth; }
            set { _focalWidth = value; }
        }
        float _focalWidth = 75;

        /// <summary>
        /// Gets the world transformation of the camera.
        /// </summary>
        public Matrix WorldMatrix
        {
            get { return _worldMatrix; }
        }
        private Matrix _worldMatrix;

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>The position.</value>
        public Vector3 Position
        {
            get { return WorldMatrix.Translation; }
        }

        /// <summary>
        /// Gets or sets the yaw rotation of the camera.
        /// </summary>
        public float Yaw
        {
            get { return _yaw; }
            set { _yaw = MathHelper.WrapAngle(value); }
        }
        private float _yaw;

        /// <summary>
        /// Gets or sets the pitch rotation of the camera.
        /// </summary>
        public float Pitch
        {
            get { return _pitch; }
            set
            {
                _pitch = value;
                if (_pitch > MathHelper.PiOver2 * .99f)
                    _pitch = MathHelper.PiOver2 * .99f;
                else if (_pitch < -MathHelper.PiOver2 * .99f)
                    _pitch = -MathHelper.PiOver2 * .99f;
            }
        }
        private float _pitch;

        /// <summary>
        /// The orbit target of the Camera in Orbit mode.
        /// </summary>
        public Vector3 LookAt = Vector3.Zero;

        /// <summary>
        /// Gets or sets the Requested orbit zoom factor.
        /// </summary>
        /// <value>The orbit zoom.</value>
        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
        }
        float _zoom = -15;


        MouseState MouseState;

        MouseState PreviousMouseState;

        Point FirstMouseDownPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:rtroe.tutorials.shaders.Camera"/> class.
        /// </summary>
        /// <param name="GraphicsDevice">Graphics device.</param>
        /// <param name="Pitch">Pitch.</param>
        /// <param name="Yaw">Yaw.</param>
        /// <param name="NearPlane">Near plane.</param>
        /// <param name="FarPlane">Far plane.</param>
        /// <param name="FieldOfView">Field of view.</param>
		public Camera(GraphicsDevice GraphicsDevice,
                        float Pitch = 0, float Yaw = 0,
                          float NearPlane = 0.1f, float FarPlane = 1000,
                         float FieldOfView = MathHelper.PiOver4)
		{
			this.GraphicsDevice = GraphicsDevice;

			// Always use this as the default viewport.
			Viewport = GraphicsDevice.Viewport;

            _fieldOfView = FieldOfView;
            _nearPlane = NearPlane;
            _farPlane = FarPlane;
            _aspectRatio = Viewport.AspectRatio;

            this.Pitch = Pitch;
            this.Yaw = Yaw;


			// Initalise Camera View and Projections
            View = Matrix.CreateLookAt(Position, Vector3.Zero, Vector3.Up);	

			//Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), AspectRatio, 1.0f, 10000.0f);

            CalculateProjectionMatrix();

            // Initialise Inputs
            MouseState = new MouseState();
            PreviousMouseState = new MouseState();
        }


        private void CalculateProjectionMatrix()
        {
            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(_fieldOfView, _aspectRatio, _nearPlane, _farPlane);
        }


        public virtual void Update(GameTime gameTime)
        {
            // Get new Mouse State
            MouseState = Mouse.GetState();

            // Get the time difference
            float dt = ((float)gameTime.ElapsedGameTime.Milliseconds) / 1000;

            if(MouseState.MiddleButton == ButtonState.Pressed &&
               PreviousMouseState.MiddleButton == ButtonState.Released)
            {
                FirstMouseDownPosition = MouseState.Position;
            }

            if (MouseState.MiddleButton == ButtonState.Pressed)
            {
                Yaw += (FirstMouseDownPosition.X - MouseState.X) * dt * .12f;
                Pitch += (FirstMouseDownPosition.Y - MouseState.Y) * dt * .12f;

                // reset mouse to first down position
                Mouse.SetPosition(FirstMouseDownPosition.X, FirstMouseDownPosition.Y);
            }

            // Set Zoom Value
            float WheelDelta = this.MouseState.ScrollWheelValue - this.PreviousMouseState.ScrollWheelValue;
            _zoom += WheelDelta / 5;
            _zoom = Math.Max(_zoom, 40);

            // Create Zoom
            _worldMatrix = Matrix.CreateTranslation(new Vector3(0, 0, (_zoom) / 200));

            // Rotate Based on Pitch and Yaw
            _worldMatrix *= Matrix.CreateFromAxisAngle(Vector3.Right, Pitch) * Matrix.CreateFromAxisAngle(Vector3.Up, Yaw);

            // Now Translate to the Target
            _worldMatrix *= Matrix.CreateTranslation(LookAt);

            View = Matrix.Invert(WorldMatrix);

            PreviousMouseState = MouseState;

            // Recalculate the Bounding Frustrum
            BoundingFrustum = new BoundingFrustum(View * Projection);
        }

	}
}
