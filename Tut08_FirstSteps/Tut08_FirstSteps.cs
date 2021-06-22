using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.Effects;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;
using Fusee.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FuseeApp
{
    [FuseeApplication(Name = "Tut08_FirstSteps", Description = "Yet another FUSEE App.")]
    public class Tut08_FirstSteps : RenderCanvas
    {
        private SceneContainer _scene;
        private SceneRendererForward _sceneRenderer;

        private Transform _cubeTransform;
        // Init is called on startup.

        // Referenz von Außen ersellen
        private DefaultSurfaceEffect _cubeShader;
        public override void Init()
        {
            // Set the clear color for the backbuffer to "greenery"
            RC.ClearColor = (float4)ColorUint.Greenery;

            _cubeTransform = new Transform
            {
                Translation = new float3(0, 0, 50),
                Rotation = new float3(0, 0, 2.3f),
            };
            _cubeShader = MakeEffect.FromDiffuseSpecular((float4)ColorUint.Blue, float4.Zero);
            _scene = new SceneContainer
            {
                Children =
                     {
                            new SceneNode
                         {
                           Components =
                            {
                                _cubeTransform,
                                _cubeShader,
                                SimpleMeshes.CreateCuboid(new float3(4,4,4))
                             }
                        },
                        new SceneNode
                        {
                            Components =
                            {
                                _cubeTransform, new Transform { 
                                    Translation = new float3(12,5,15),
                                    Rotation = new float3(20.8f, 55.3f ,6.9f),
                                    },
                                MakeEffect.FromDiffuseSpecular((float4)ColorUint.BlueViolet , float4.Zero),
                                SimpleMeshes.CreateCuboid(new float3(4,4,4))

                            }
                        },
                        new SceneNode
                        {
                            Components =
                            {
                               _cubeTransform, new Transform { 
                                    Translation = new float3(12, -15, 17),
                                    Rotation = new float3(34f, 0.03f * Time.DeltaTime, 0)
                                    },
                                MakeEffect.FromDiffuseSpecular((float4)ColorUint.Wheat , float4.Zero),
                                SimpleMeshes.CreateCuboid(new float3(6,6,6))

                            }
                        },
                        new SceneNode
                        {
                            Components =
                            {
                               _cubeTransform, new Transform {
                                    Translation = new float3(0,15,-15),
                                    Rotation = new float3(6.9f, 0.3f* Time.DeltaTime,0)
                                    },
                                MakeEffect.FromDiffuseSpecular((float4)ColorUint.Blue , float4.Zero),
                                SimpleMeshes.CreateCuboid(new float3(3,3,3))

                            }
                        },
                         new SceneNode
                        {
                            Components =
                            {
                               _cubeTransform, new Transform {
                                    Translation = new float3(-25,-10, 8),
                                    Rotation = new float3(0, 6.9f, 67.2f)
                                    },
                                MakeEffect.FromDiffuseSpecular((float4)ColorUint.Yellow,  float4.Zero),
                                SimpleMeshes.CreateCuboid(new float3(2,2,2))

                            }
                        }

                    }
            };

            // Create a scene renderer holding the scene above
            _sceneRenderer = new SceneRendererForward(_scene);

        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            SetProjectionAndViewport();

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            _cubeTransform.Rotation = _cubeTransform.Rotation + new float3(0, 0.0001f * Time.DeltaTime, 0);
            //Zeitschwankung durch Frameratenabhängikeit (einsehen?)
            //Diagnostics.Debug("DeltaTime: " + Time.DeltaTime);

            Diagnostics.Debug("Keyboard <-> axis:" + Keyboard.LeftRightAxis);

            _cubeTransform.Rotation = new float3(0, 90 * (3.141592f / 360.0f) * Time.TimeSinceStart, 0);

            Diagnostics.Debug("Keyboard <-> axis: " + Keyboard.LeftRightAxis);
            //Ausschlag des Würfels nach links und rechts
            _cubeTransform.Translation.x = 5 * M.Sin(2 * Time.TimeSinceStart);
            // Zeitausgabe auf der Console seit dem Start


            _cubeShader.SurfaceInput.Albedo = new float4(0.5f + 0.5f * M.Sin(2 * Time.TimeSinceStart), 0, 0, 1);


            // Render the scene on the current render context
            _sceneRenderer.Render(RC);

            // Swap buffers: Show the content 
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)

            //var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
            //RC.Projection = projection;

            Present();
        }
        public void SetProjectionAndViewport()
        {
            // Set the rendering area to the entire window size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
            RC.Projection = projection;
        }

    }
}