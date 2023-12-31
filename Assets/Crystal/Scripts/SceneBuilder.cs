using UnityEngine;
using Unity.Mathematics;
using System;
using Random = Unity.Mathematics.Random;

namespace Sketch {

// Configuration struct for SceneBuilder
[Serializable]
struct SceneConfig
{
    #region Editable attributes

    public int InstanceCount;
    public float InnerRadius;
    public float OuterRadius;
    public uint Seed;

    #endregion

    #region Default configuration

    public static SceneConfig Default()
      => new SceneConfig()
        { InstanceCount = 100,
          InnerRadius = 1,
          OuterRadius = 2,
          Seed = 12345 };

    #endregion
}

// SceneBuilder: Model-level scene building
static class SceneBuilder
{
    // Public entry point
    public static Span<Modeler> Build
      (in SceneConfig cfg,
       ReadOnlySpan<GeometryCacheRef> shapes,
       Span<Modeler> buffer)
    {
        // Buffer output count
        var count = 0;

        for (var i = 0; i < cfg.InstanceCount; i++)
        {
            // Per-node PRNG
            var rand = Random.CreateFromIndex((uint)(cfg.Seed + i));

            // Node
            var slice = AddNode(cfg, shapes, rand, buffer.Slice(count));
            count += slice.Length;
        }

        // Used area in the model buffer
        return buffer.Slice(0, count);
    }

    // Builder method
    static Span<Modeler> AddNode
      (in SceneConfig cfg,
       ReadOnlySpan<GeometryCacheRef> shapes,
       Random rand,
       Span<Modeler> buffer)
    {
        var pos = math.float3(rand.NextFloat2Direction(), 0).xzy;
        pos *= math.lerp(cfg.InnerRadius, cfg.OuterRadius, rand.NextFloat());

        var ry = quaternion.RotateY(rand.NextFloat(math.PI * 2));
        var rx = quaternion.RotateX(rand.NextFloat(math.PI * 0.5f));
        var rot = math.mul(ry, rx);

        var scale = math.lerp(0.2f, 1, math.pow(rand.NextFloat(), 1.5f));

        buffer[0] = new Modeler(position: pos,
                                rotation: rot,
                                scale: scale,
                                color: Color.white,
                                shape: shapes[rand.NextInt(shapes.Length)]);

        return buffer.Slice(0, 1);
    }
}

} // namespace Sketch
