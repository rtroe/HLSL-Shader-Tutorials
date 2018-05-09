using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rtroe.tutorials.shaders
{
    /// <summary>
    /// Entity.
    /// </summary>
    public class Entity
    {
        public  Model Model
        {
            get { return model; }
            set{
                model = value;
                InitialiseShaderValues();
            }
        }
        Model model;

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>The position.</value>
        public Vector3 Position
        {
            get { return World.Translation; }
        }

        Matrix World;

        /// <summary>
        /// Gets or sets the diffuse texture.
        /// </summary>
        /// <value>The diffuse texture.</value>
        Texture2D DiffuseTexture
        {
            get { return _diffuseTexture; }
            set { _diffuseTexture = value; }
        }
        Texture2D _diffuseTexture;

        Game Game;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:rtroe.tutorials.shaders.Entity"/> class.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="Position">Position.</param>
        public Entity(Game Game, string path, Effect effect, Vector3 Position)
        {
            this.Game = Game;

            Model = LoadModel(path, effect);

            World = Matrix.CreateTranslation(Position);
        }

        public Model LoadModel(string pathToModel, Effect effect)
        {
            Model tempModel = Game.Content.Load<Model>(pathToModel);

            DiffuseTexture = Game.Content.Load<Texture2D>(pathToModel + "_diffuse");

            // Replace BasicEffect with Specified Effect
            Dictionary<Effect, Effect> effectMapping = new Dictionary<Effect, Effect>();

            foreach (ModelMesh mesh in tempModel.Meshes)
            {
                // Scan over all the effects currently on the mesh.
                foreach (Effect oldEffect in mesh.Effects)
                {
                    // If we haven't already seen this effect...
                    if (!effectMapping.ContainsKey(oldEffect))
                    {
                        // Make a clone of our replacement effect. We can't just use
                        // it directly, because the same effect might need to be
                        // applied several times to different parts of the model using
                        // a different texture each time, so we need a fresh copy each
                        // time we want to set a different texture into it.
                        Effect newEffect = effect.Clone();

                        effectMapping.Add(oldEffect, newEffect);
                    }
                }

                // Now that we've found all the effects in use on this mesh,
                // update it to use our new replacement versions.
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = effectMapping[meshPart.Effect];
                }
            }

            return tempModel;
        }


        void InitialiseShaderValues()
        {
            foreach (var part in Model.Meshes.SelectMany(m => m.MeshParts))
            {
                part.Effect.Parameters["DiffuseTexture"].SetValue(DiffuseTexture);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            
        }

        public virtual void Draw(GameTime gameTime, Camera Camera)
        {
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in model.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (Effect effect in mesh.Effects)
                {
                    effect.Parameters["World"].SetValue(World);
                    effect.Parameters["WorldViewProjection"].SetValue(World * Camera.View * Camera.Projection);
                    effect.Parameters["LightDirection"].SetValue(Vector3.One);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }

        public virtual void DrawBasicEffect(GameTime gameTime, Camera Camera)
        {
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in model.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.DiffuseColor = new Vector3(0.5f);
                    effect.PreferPerPixelLighting = true;
                    effect.SpecularColor = new Vector3(0.15f);
                    //effect.AmbientLightColor = new Vector3(0.5f);
                    effect.EmissiveColor = new Vector3(0.35f);
                    //effect.LightingEnabled = true;
                    //effect.VertexColorEnabled = true;
                    effect.TextureEnabled = true;
                    effect.Texture = DiffuseTexture;
                    effect.World = World;
                    effect.View = Camera.View;
                    effect.Projection = Camera.Projection;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }
    }
}
