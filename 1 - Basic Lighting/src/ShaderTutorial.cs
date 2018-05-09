using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rtroe.tutorials.shaders
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class ShaderTutorial : Game
	{
		// Graphics Device Manager
		GraphicsDeviceManager graphics;

		// Spritebatch
		SpriteBatch spriteBatch;

        // the display font
		SpriteFont font;

		// blank texture for misc drawing
        Texture2D BlankTexture;


		// The Cameras
		Camera Camera;

        // A list containing all Entities to show.
        List<Entity> Entities = new List<Entity>();

        // The 3D model to draw.
        Entity entity;


		public ShaderTutorial()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);


            // Set up the Camera
            Camera = new Camera(GraphicsDevice);
            Camera.Zoom = 1000;

			// Load content
			BlankTexture = Content.Load<Texture2D>("txtrs/blank");

			font = Content.Load<SpriteFont>("fonts/Font");

            Effect modelEffect = Content.Load<Effect>("shaders/ModelEffect");

            entity = new Entity(this, "mdls/suzanne/model",modelEffect, Vector3.Zero);

		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
#if !__IOS__ && !__TVOS__
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();
#endif
            Camera.Update(gameTime);

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			// Now clear the screen
			graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

			// Set the Blend and Depth Stencil states, otherwise the model will render funny
			GraphicsDevice.BlendState = BlendState.AlphaBlend;
			GraphicsDevice.DepthStencilState = DepthStencilState.Default;


			// Relative Text position for each viewport
			Vector2 TextPosition = new Vector2(2);

			// Draw the model
            entity.Draw(gameTime, Camera);
			
            base.Draw(gameTime);
		}

		void DrawShadowedString(string Text, Vector2 Position)
		{
			spriteBatch.Begin();
			spriteBatch.DrawString(font, Text, Position + new Vector2(1), Color.Black);
			spriteBatch.DrawString(font, Text, Position, Color.White);
			spriteBatch.End();
		}
	}
}
