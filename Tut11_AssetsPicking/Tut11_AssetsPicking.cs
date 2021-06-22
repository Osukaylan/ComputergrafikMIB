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
    [FuseeApplication(Name = "Tut11_AssetsPicking", Description = "Yet another FUSEE App.")]
    public class Tut11_AssetsPicking : RenderCanvas
    {
        private SceneContainer _scene;
        private SceneRendererForward _sceneRenderer;
        private Transform _rightWheelFrontTransform, _leftWheelFrontTransform, _rightWheelBackTransform, _leftWheelBackTransform, _firstExcavatorArmTransform, _secondExcavatorArmTransform, _thirdExcavatorArmTransform, _ExcavatorMainTransform, _bodyTransform, eftransform;
        private SurfaceEffect _rightWheelFrontEffect, _leftWheelFrontEffect, _rightWheelBackEffect, _leftWheelBackEffect;
        private ScenePicker _scenePicker;
        private PickResult _currentPick;
        private float4 _oldColor;
        private Transform _camTransform;
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

        // Init is called on startup. 
        public override void Init()
        {

            RC.ClearColor = new float4(0.8f, 0.9f, 0.7f, 1);

            // _scene = CreateScene();
            _scene = AssetStorage.Get<SceneContainer>("Excavator_new.fus");

            //Search children of _scene and find the nodes with the exact name. Then get the transform component of each and give the value to the variable.
            _firstExcavatorArmTransform = _scene.Children.FindNodes(node => node.Name == "FirstExcavatorArm")?.FirstOrDefault()?.GetTransform(); // or GetComponent<Transform>();

            _rightWheelFrontTransform = _scene.Children.FindNodes(node => node.Name == "RightWheelFront")?.FirstOrDefault()?.GetTransform(); // or GetComponent<Transform>();
            _leftWheelFrontTransform = _scene.Children.FindNodes(node => node.Name == "LeftWheelFront")?.FirstOrDefault()?.GetTransform();
            _rightWheelBackTransform = _scene.Children.FindNodes(node => node.Name == "RightWheelBack")?.FirstOrDefault()?.GetTransform();
            _leftWheelBackTransform = _scene.Children.FindNodes(node => node.Name == "LeftWheelBack")?.FirstOrDefault()?.GetTransform();

            _bodyTransform = _scene.Children.FindNodes(node => node.Name == "Body")?.FirstOrDefault()?.GetTransform();
            
            //Search children of _scene and find the nodes with the exact name. Then get the SourceEffect component of each and give the value to the variable.
            _rightWheelFrontEffect = _scene.Children.FindNodes(node => node.Name == "RightWheelFront")!.FirstOrDefault()?.GetComponent<SurfaceEffect>();
            _leftWheelFrontEffect = _scene.Children.FindNodes(node => node.Name == "LeftWheelFront")?.FirstOrDefault()?.GetComponent<SurfaceEffect>();
            _rightWheelBackEffect = _scene.Children.FindNodes(node => node.Name == "RightWheelBack")?.FirstOrDefault()?.GetComponent<SurfaceEffect>();
            _leftWheelBackEffect = _scene.Children.FindNodes(node => node.Name == "LeftWheelBack")?.FirstOrDefault()?.GetComponent<SurfaceEffect>();

            // Create a scene renderer holding the scene above
            _sceneRenderer = new SceneRendererForward(_scene);
            _scenePicker = new ScenePicker(_scene);
             
            //Change colors of nodes
            _rightWheelFrontEffect.SurfaceInput.Albedo = (float4)ColorUint.DarkOliveGreen;
            _leftWheelFrontEffect.SurfaceInput.Albedo = (float4)ColorUint.SkyBlue;
            _rightWheelBackEffect.SurfaceInput.Albedo = (float4)ColorUint.Cornsilk;
            _leftWheelBackEffect.SurfaceInput.Albedo = (float4)ColorUint.Green;
        }

        // RenderAFrame is called once a frame
        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            SetProjectionAndViewport();

            /*_rightWheelFrontTransform.Rotation = new float3(0, -M.MinAngle(TimeSinceStart), 0);
            _leftWheelFrontTransform.Rotation = new float3(0, -M.MinAngle(TimeSinceStart), 0);
            _rightWheelBackTransform.Rotation = new float3(0, -M.MinAngle(TimeSinceStart), 0);
            _leftWheelBackTransform.Rotation = new float3(0, -M.MinAngle(TimeSinceStart), 0);*/

            _rightWheelFrontTransform.Rotation.y += Keyboard.WSAxis * DeltaTime;
            _leftWheelFrontTransform.Rotation.y += Keyboard.WSAxis * DeltaTime;
            _rightWheelBackTransform.Rotation.y += Keyboard.WSAxis * DeltaTime;
            _leftWheelBackTransform.Rotation.y += Keyboard.WSAxis * DeltaTime;



            //_bodyTransform.Translation.xz += Keyboard.ADAxis * DeltaTime;
            _bodyTransform.Translation.z += Keyboard.LeftRightAxis / 10;
            _bodyTransform.Translation.x += Keyboard.WSAxis / 10;
            _bodyTransform.Rotation.y += Keyboard.ADAxis * DeltaTime;


            //_scenePick.Pick(RC, pickPosClip)


            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Setup the camera 
            RC.View = float4x4.CreateTranslation(0, 0, 40) * float4x4.CreateRotationX(-(float)Math.Atan(15.0 / 40.0));

            //Red Base movement
            _firstExcavatorArmTransform.Rotation.y += Keyboard.UpDownAxis * DeltaTime;

            //Green UpperArm movement
            //_secondExcavatorArmTransform.Rotation.x += Keyboard.UpDownAxis * DeltaTime;

            //Blue UpperArm movement
            //_thirdExcavatorArmTransform.Rotation.x += Keyboard.WSAxis * DeltaTime;


            //binding piece movement
            //_bindingHandTransform.Rotation.y += Keyboard.ADAxis * DeltaTime;

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

            var mtxRot = float4x4.CreateRotationX(_angelVert) * float4x4.CreateRotationY(_angelHorz);
            var mtxCam = float4x4.LookAt(0, 10, -30, 0, 0, 0, 0, 1, 0);
            RC.View = mtxCam * mtxRot;

            //_firstHandTransform.Rotation.x = -_angel;
            //_secondHandTransform.Rotation.x = _angel;

            if (opening)
            {
                if (_angel < 0.5f)
                {
                    _angel += 0.5f * DeltaTime;
                }
            }
            else
            {
                if (_angel > -0.5f)
                {
                    _angel -= 0.5f * DeltaTime;
                }
            }

            if (Keyboard.GetKey(KeyCodes.Space))
            {
                if (!spacePressed)
                {
                    opening = !opening;
                }
                spacePressed = true;
            }
            else
            {
                spacePressed = false;
            }

            if (Mouse.LeftButton)
            {
                float2 pickPosClip = Mouse.Position * new float2(2.0f / Width, -2.0f / Height) + new float2(-1, 1);

                PickResult newPick = _scenePicker.Pick(RC, pickPosClip).OrderBy(pr => pr.ClipPos.z).FirstOrDefault();
                _keys = false;


                if (newPick?.Node != _currentPick?.Node)
                {
                    if (_currentPick != null)
                    {
                        var ef = _currentPick.Node.GetComponent<DefaultSurfaceEffect>();
                        ef.SurfaceInput.Albedo = _oldColor;
                    }
                    if (newPick != null)
                    {
                        var ef = newPick.Node.GetComponent<SurfaceEffect>();
                        _oldColor = ef.SurfaceInput.Albedo;
                        ef.SurfaceInput.Albedo = (float4)ColorUint.OrangeRed;
                        Diagnostics.Debug("The picked object is " + newPick.Node.Name);
                        var eftransform =  newPick.Node.GetComponent<Transform>();
                        eftransform.Rotation.y += Keyboard.UpDownAxis * DeltaTime;       
                    }
                    _currentPick = newPick;
                }
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