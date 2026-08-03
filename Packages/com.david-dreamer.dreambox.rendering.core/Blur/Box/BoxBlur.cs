using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Dreambox.Rendering.Core
{
    public class BoxBlur : IBlur
    {
        private BlurSettings Settings { get; }

        private Material Material { get; set; }

        private RTHandle RTHorizontal { get; set; }

        private RTHandle RTVertical { get; set; }

        public RTHandle Result { get; private set; }

        public BoxBlur(BlurSettings settings, GraphicsFormat graphicsFormat)
        {
            Settings = settings;

            Material = CoreUtils.CreateEngineMaterial("Hidden/Dreambox/PostProcessing/Blur/Box");

            Material.SetInteger(BlurShaderVariable.KernelSize, Settings.KernelSize * Settings.KernelSize);

            float radius = Settings.KernelSize / 2;
            Material.SetFloat(BlurShaderVariable.Radius, radius);
            Material.SetFloat(BlurShaderVariable.Scale, Settings.Scale);

            Vector2 scaleFactor = Vector2.one / Settings.Downsample;
            TextureDimension dimension = TextureXR.dimension;
            int slices = TextureXR.slices;

            RTHorizontal = AllocTexture("Horizontal");
            RTVertical = AllocTexture("Vertical");

            Result = Settings.Separable ? RTHorizontal : (Settings.Iterations % 2 == 0 ? RTHorizontal : RTVertical);

            RTHandle AllocTexture(string name)
            {
                return RTHandles.Alloc(
                    scaleFactor,
                    dimension: dimension,
                    slices: slices,
                    colorFormat: graphicsFormat,
                    autoGenerateMips: false,
                    useDynamicScale: true,
                    name: $"BlurTexture_{name}"
                    );
            }
        }

        public void Dispose()
        {
            CoreUtils.Destroy(Material);
            RTHorizontal.Release();
            RTVertical.Release();
        }

        public void Execute(CommandBuffer commandBuffer, RTHandle source)
        {
            commandBuffer.SetRenderTarget(RTHorizontal);
            Blitter.BlitTexture(commandBuffer, source, new Vector4(1, 1, 0, 0), 0, false);

            if (Settings.Separable)
            {
                for (int i = 0; i < Settings.Iterations; i++)
                {
                    Blitter.BlitTexture(commandBuffer, RTHorizontal, RTVertical, Material, BlurShaderPass.Horizontal);
                    Blitter.BlitTexture(commandBuffer, RTVertical, RTHorizontal, Material, BlurShaderPass.Vertical);
                }
            }
            else
            {
                source = RTHorizontal;
                RTHandle destination = RTVertical;

                for (int i = 0; i < Settings.Iterations; i++)
                {
                    Blitter.BlitTexture(commandBuffer, source, destination, Material, BlurShaderPass.NonSeparable);
                    (source, destination) = (destination, source);
                }
            }
        }
    }
}
