using Aiv.Fast2D;
using Aiv.Fast3D;
using OpenTK;
using System;

namespace OrbitalCamera
{
    internal class Ivysaur
    {
        private Mesh3[] meshes;
        private Material material;
        private float blend = 30f;
        public Vector3 PokemonForward;

        private float[] meshCurrentYRotations;

        public Ivysaur()
        {
            meshes = SceneImporter.LoadMesh("Assets/Pokemon.obj");
            material = new Material();

            meshCurrentYRotations = new float[meshes.Length];
            for (int i = 0; i < meshCurrentYRotations.Length; i++)
            {
                meshCurrentYRotations[i] = 0f;
            }

            GenerateMaterial();
        }

        public void SetPosition(Vector3 position)
        {
            for (int i = 0; i < meshes.Length; i++)
            {
                meshes[i].Position3 = position;
            }
        }

        public void SetMovement(Vector3 position)
        {
            for (int i = 0; i < meshes.Length; i++)
            {
                meshes[i].Position3 += position;
            }
        }

        public Vector3 GetPosition()
        {
            return meshes[0].Position3;
        }

        public void SetRotation(Vector3 rotation)
        {
            for (int i = 0; i < meshes.Length; i++)
            {
                meshes[i].EulerRotation3 += rotation;
                meshCurrentYRotations[i] += rotation.Y;
            }
        }

        public void SetForwardPokemon(PerspectiveCamera camera, float sign)
        {
            PokemonForward = new Vector3(camera.Forward.X, 0f, camera.Forward.Z);
            PokemonForward.Normalize();

            float targetYRotation = MathHelper.RadiansToDegrees((float)Math.Atan2(PokemonForward.X, PokemonForward.Z)) + (180f * sign);

            for (int i = 0; i < meshes.Length; i++)
            {
                meshCurrentYRotations[i] = LerpAngle(meshCurrentYRotations[i], targetYRotation, blend * Program.Window.DeltaTime);

                meshes[i].EulerRotation3 = new Vector3(
                    meshes[i].EulerRotation3.X,
                    meshCurrentYRotations[i],
                    meshes[i].EulerRotation3.Z
                );
            }
        }

        public void SetScale(Vector3 scale)
        {
            for (int i = 0; i < meshes.Length; i++)
            {
                meshes[i].Scale3 = scale;
            }
        }

        private void GenerateMaterial()
        {
            material.Diffuse = new Texture("Assets/Final_Pokemon_Diffuse.png");
            material.NormalMap = new Texture("Assets/Final_Pokemon_Glossiness.jpg");
            material.Lights = new Light[]
            {
                new DirectionalLight(new Vector3(1f, 0f, 0f)),
                new DirectionalLight(new Vector3(-1f, 0f, 0f)),
                new DirectionalLight(new Vector3(0f, 0f, 1f)),
                new DirectionalLight(new Vector3(0f, 0f, -1f)),
                new DirectionalLight(new Vector3(0f, 1f, 0f)),
                new DirectionalLight(new Vector3(0f, -1f, 0f))
            };
        }

        public void Draw()
        {
            for (int i = 0; i < meshes.Length; i++)
            {
                meshes[i].DrawPhong(material);
            }
        }

        private float LerpAngle(float current, float target, float t)
        {
            float delta = target - current;

            // Same as "player.cs", in order to avoid the wrap-around on Y axis.
            while (delta > 180f) delta -= 360f;
            while (delta < -180f) delta += 360f;

            float result = current + delta * MathHelper.Clamp(t, 0f, 1f);

            return NormalizeAngle(result);
        }

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