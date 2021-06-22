using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
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
    [FuseeApplication(Name = "Tut09_HierarchyAndInput", Description = "Yet another FUSEE App.")]
    public class Tut09_HierarchyAndInput : RenderCanvas
    {
        private SceneContainer _scene;
        private SceneRendererForward _sceneRenderer;
        private float _camAngle = 0;
        private Transform _baseTransform;
        private Transform _bodyTransform;
        private Transform _upperArmTransform;
        private Transform _foreArmTransform;
        private Transform _bindingHandTransform;
        private Transform _firstHandTransform;
        private Transform _secondHandTransform;
        private static float _angelHorz = M.PiOver4, _angelVert;

        // Horizontal and vertical angular speed
        private static float _angelVelHorz, _angelVelVert;
        private const float RotationSpeed = 7;
        // Damping factor 
        private const float Damping = 0.8f;
        private bool _keys;

         private Boolean opening = true;
        private float _angel = 0.5f;
        private Boolean spacePressed = false;


        SceneContainer CreateScene()
        {
            // Initialize transform components that need to be changed inside "RenderAFrame"
            _baseTransform = new Transform
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 0, 0)
            };

            _bodyTransform = new Transform
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 6, 0)
            };

            _upperArmTransform = new Transform
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(2, 4, 0)
            };

            _foreArmTransform = new Transform
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(-2, 4, 0)
            };

            _bindingHandTransform = new Transform
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 6, 0)
            };

            _firstHandTransform = new Transform
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 0, 0)
            };

            _secondHandTransform = new Transform
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 0, 0)
            };

            // Setup the scene graph
            return new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    //Grey Base
                    new SceneNode
                    {
                        Components = new List<SceneComponent>
                        {
                            // TRANSFORM COMPONENT
                            _baseTransform,

                            // SHADER EFFECT COMPONENT
                            MakeEffect.FromDiffuseSpecular((float4) ColorUint.LightGrey, float4.Zero),

                            // MESH COMPONENT
                            SimpleMeshes.CreateCuboid(new float3(10, 2, 10))
                        },
                        Children =
                        {
                            //red Body
                            new SceneNode
                            {
                                Components =
                                {
                                    _bodyTransform,
                                    MakeEffect.FromDiffuseSpecular((float4) ColorUint.Red , float4.Zero),
                                    SimpleMeshes.CreateCuboid(new float3(2, 10, 2))
                                },
                                Children =
                                {
                                    //green UpperArm
                                    new SceneNode
                                    {
                                        Components =
                                        {
                                            _upperArmTransform,
                                        },
                                        Children =
                                        {
                                            new SceneNode
                                            {
                                                Components =
                                                {
                                                    new Transform{ Translation = new float3(0 , 4, 0)},
                                                    MakeEffect.FromDiffuseSpecular((float4) ColorUint.Green , float4.Zero),
                                                    SimpleMeshes.CreateCuboid(new float3(2, 10, 2))
                                                },
                                                Children =
                                                {
                                                    //blue ForeArm
                                                    new SceneNode
                                                    {
                                                        Components =
                                                        {
                                                            _foreArmTransform,
                                                        },
                                                        Children =
                                                        {
                                                            new SceneNode
                                                            {
                                                                Components =
                                                                {
                                                                    new Transform{ Translation = new float3(0 , 4, 0)},
                                                                    MakeEffect.FromDiffuseSpecular((float4) ColorUint.Blue , float4.Zero),
                                                                    SimpleMeshes.CreateCuboid(new float3(2, 10, 2))
                                                                },
                                                                Children = 
                                                                {
                                                                    //bindingPiece
                                                                    new SceneNode 
                                                                    {
                                                                        Components =
                                                                        {
                                                                            _bindingHandTransform,
                                                                            MakeEffect.FromDiffuseSpecular((float4) ColorUint.Aquamarine , float4.Zero),
                                                                            SimpleMeshes.CreateCuboid(new float3(3, 2, 3))
                                                                        },
                                                                        Children =
                                                                        {
                                                                            //Greifhand firsthand
                                                                            new SceneNode
                                                                            {
                                                                                Components =
                                                                                {
                                                                                    _firstHandTransform,
                                                                                },
                                                                                    Children =
                                                                                    {
                                                                                        new SceneNode
                                                                                        {
                                                                                            Components =
                                                                                            {
                                                                                                new Transform{ Translation = new float3(0,2 ,-1)},
                                                                                                MakeEffect.FromDiffuseSpecular((float4) ColorUint.Turquoise , float4.Zero),
                                                                                                SimpleMeshes.CreateCuboid(new float3(1, 4, 1))
                                                                                            }
                                                                                        },
                                                                                        // Greifhand secondHand
                                                                                        new SceneNode
                                                                                        {
                                                                                            Components =
                                                                                            {
                                                                                                _secondHandTransform,
                                                                                            },
                                                                                            Children =
                                                                                            {
                                                                                                new SceneNode
                                                                                                {
                                                                                                    Components =
                                                                                                    {
                                                                                                        new Transform{ Translation = new float3(0 , 2, 1)},
                                                                                                        MakeEffect.FromDiffuseSpecular((float4) ColorUint.Turquoise , float4.Zero),
                                                                                                        SimpleMeshes.CreateCuboid(new float3(1, 4, 1))
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
            }
        // Init is called on startup. 
        public override void Init()
        {
            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0.8f, 0.9f, 0.7f, 1);

            _scene = CreateScene();

            // Create a scene renderer holding the scene above
            _sceneRenderer = new SceneRendererForward(_scene);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            SetProjectionAndViewport();

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Setup the camera 
            RC.View = float4x4.CreateTranslation(0, -10, 50) * float4x4.CreateRotationY(_camAngle);

            //Red Base movement
            _bodyTransform.Rotation.y += Keyboard.LeftRightAxis * DeltaTime;

            //Green UpperArm movement
            _upperArmTransform.Rotation.x += Keyboard.UpDownAxis * DeltaTime;

            //Blue UpperArm movement
            _foreArmTransform.Rotation.x += Keyboard.WSAxis * DeltaTime;


            //binding piece movement
            _bindingHandTransform.Rotation.y += Keyboard.ADAxis * DeltaTime;


            //Create Moving possibility with mouse to look around
            if (Mouse.LeftButton)
            {
                _keys = false;
                _angelVelHorz = -RotationSpeed * Mouse.XVel * DeltaTime * 0.0005f;
                _angelVelVert = -RotationSpeed * Mouse.YVel * DeltaTime * 0.0005f;
            }
            else if (Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _keys = false;
                var touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angelVelHorz = -RotationSpeed * touchVel.x * DeltaTime * 0.0005f;
                _angelVelVert = -RotationSpeed * touchVel.y * DeltaTime * 0.0005f;
            }
            else
            {
                if (_keys)
                {
                    _angelVelHorz = -RotationSpeed * Keyboard.LeftRightAxis * DeltaTime;
                    _angelVelVert = -RotationSpeed * Keyboard.UpDownAxis * DeltaTime;
                }
                else
                {
                    var curDamp = (float)System.Math.Exp(-Damping * DeltaTime);
                    _angelVelHorz *= curDamp;
                    _angelVelVert *= curDamp;
                }
            }

            _angelHorz += _angelVelHorz;
            _angelVert += _angelVelVert;

            // Create the camera matrix and set it as the current View transformation
            var mtxRot = float4x4.CreateRotationX(_angelVert) * float4x4.CreateRotationY(_angelHorz);
            var mtxCam = float4x4.LookAt(0, 20, -60, 0, 15, 0, 0, 1, 0);
            RC.View = mtxCam * mtxRot;

  
            _firstHandTransform.Rotation.x = -_angel;
            _secondHandTransform.Rotation.x = _angel;

            if (opening) {
                if (_angel < 0.5f) {
                    _angel += 0.5f * DeltaTime;
                }
            } else {
                if (_angel > -0.5f) {
                    _angel -= 0.5f * DeltaTime;
                }
            }

            if (Keyboard.GetKey(KeyCodes.Space)) {
                if (!spacePressed) {
                    opening = !opening;
                }
                spacePressed = true;
            } else {
                spacePressed = false;
            }





            // Render the scene on the current render context
            _sceneRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
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