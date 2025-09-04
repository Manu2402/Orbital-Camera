using Aiv.Fast2D;
using Aiv.Fast3D;
using OpenTK;
using System;

namespace OrbitalCamera
{
    internal class Player
    {
        private PerspectiveCamera camera;
        public Ivysaur Ivysaur;

        private Vector3 direction;
        private Vector3 velocity;
        private Vector3 offset;
        private float movSpd;
        private float rotSpd;

        private float distance = 3f;
        private float accumulator = MathHelper.ThreePiOver2;
        private float mouseSensX = 0.1f;
        private float scroll;
        private float scrollMultiplied;
        private float maxScroll = 8;
        private float minScroll = 4;

        private KeyCode charPressed;
        private Vector2 mouseRawPosition;
        private float cameraYRotation = 0f;

        public Player()
        {
            offset = new Vector3(0f, 1f, 2f);

            camera = new PerspectiveCamera(offset, Vector3.Zero, 60, 0.1f, 1000f);

            Ivysaur = new Ivysaur();
            Ivysaur.SetPosition(Vector3.Zero);
            Ivysaur.SetRotation(new Vector3(0, 0, 0));
            Ivysaur.SetScale(new Vector3(0.60f));

            direction = Vector3.Zero;
            velocity = Vector3.Zero;
            movSpd = 10f;
            rotSpd = 10f;

            mouseRawPosition = Program.Window.RawMousePosition;
        }

        public void Input()
        {
            Vector3 newVel = Vector3.Zero;
            direction = Vector3.Zero;

            //WASD
            if (Program.Window.GetKey(KeyCode.W))
            {
                charPressed = KeyCode.W;
                direction = camera.Forward;
            }
            else if (Program.Window.GetKey(KeyCode.S))
            {
                charPressed = KeyCode.S;
                direction = -camera.Forward;
            }
            else if (Program.Window.GetKey(KeyCode.A))
            {
                charPressed = KeyCode.A;
                direction = -camera.Right;
            }
            else if (Program.Window.GetKey(KeyCode.D))
            {
                charPressed = KeyCode.D;
                direction = camera.Right;
            }

            if (direction != Vector3.Zero)
            {
                newVel += direction.Normalized() * movSpd * Program.Window.DeltaTime;
            }
            else
            {
                newVel += direction * movSpd * Program.Window.DeltaTime;
            }

            if (newVel.Length > movSpd)
            {
                newVel = newVel.Normalized() * movSpd * Program.Window.DeltaTime;
            }

            velocity = newVel;

            Ivysaur.SetMovement(newVel);

            //MOUSE POSITION
            Vector2 deltaMouse = (Program.Window.RawMousePosition - mouseRawPosition) * rotSpd * Program.Window.DeltaTime;
            mouseRawPosition = Program.Window.RawMousePosition;
            SetCameraInSpace(new Vector3(0f, deltaMouse.X * mouseSensX, 0f));

            //MOUSE SCROLL
            SetZoom();
        }

        public void Update()
        {
            camera.Position3 = new Vector3((float)Math.Cos(accumulator) * distance * scrollMultiplied, offset.Y, (float)Math.Sin(accumulator) * distance * scrollMultiplied) + Ivysaur.GetPosition();
        }

        private void SetCameraInSpace(Vector3 eulerRot)
        {
            cameraYRotation += MathHelper.RadiansToDegrees(eulerRot.Y);
            cameraYRotation = NormalizeAngle(cameraYRotation);

            camera.EulerRotation3 = new Vector3(camera.EulerRotation3.X, cameraYRotation, camera.EulerRotation3.Z);

            if (direction != Vector3.Zero)
            {
                float sign = 0f;
                switch (charPressed)
                {
                    case KeyCode.W:
                    sign = 1f;
                    break;

                    case KeyCode.S:
                    sign = 0f;
                    break;

                    case KeyCode.A:
                    sign = -0.5f;
                    break;

                    case KeyCode.D:
                    sign = 0.5f;
                    break;
                }

                Ivysaur.SetForwardPokemon(camera, sign);
            }
            accumulator += eulerRot.Y;
        }

        private void SetZoom()
        {
            scroll = MathHelper.Clamp(Program.Window.MouseWheel + minScroll, minScroll, maxScroll);
            scrollMultiplied = scroll * 0.25f;
        }

        // To avoid wrap-around on Y axis (due to the missing of quaternions).
        private float NormalizeAngle(float angle)
        {
            while (angle > 180f)
            {
                angle -= 360f;
            }

            while (angle < -180f)
            {
                angle += 360f;
            }

            return angle;
        }
    }
}