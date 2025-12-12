using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Numerics;

namespace Diamond.FileFormats;

public class MotKey
{
    public Vector3 Translation { get; set; }
    public Quaternion Rotation { get; set; }

    public static MotKey Read(BinaryReader br)
    {
        return new MotKey
        {
            Translation = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
            Rotation = new Quaternion(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle())
        };
    }

    public void Write(BinaryWriter bw)
    {
        bw.Write(Translation.X);
        bw.Write(Translation.Y);
        bw.Write(Translation.Z);
        bw.Write(Rotation.X);
        bw.Write(Rotation.Y);
        bw.Write(Rotation.Z);
        bw.Write(Rotation.W);
    }
}

public class MotBone
{
    public uint Id { get; set; }
    public List<MotKey> Keys { get; set; } = new();

    public static MotBone Read(BinaryReader br)
    {
        var bone = new MotBone
        {
            Id = br.ReadUInt32()
        };
        uint keyCount = br.ReadUInt32();
        bone.Keys = new List<MotKey>();
        for (int i = 0; i < keyCount; i++)
            bone.Keys.Add(MotKey.Read(br));
        return bone;
    }

    public void Write(BinaryWriter bw)
    {
        bw.Write(Id);
        bw.Write((uint)Keys.Count);
        foreach (var key in Keys)
            key.Write(bw);
    }
}

public class MotFile
{
    public uint Id { get; set; }
    public uint BndId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<MotBone> Bones { get; set; } = new();

    public static MotFile Read(string path) => Read(File.OpenRead(path));

    public static MotFile Read(Stream stream)
    {
        using var br = new BinaryReader(stream, EucKrEncoding.Instance, leaveOpen: true);
        var file = new MotFile
        {
            Id = br.ReadUInt32(),
            BndId = br.ReadUInt32()
        };
        uint nameLen = br.ReadUInt32();
        file.Name = EucKrEncoding.Instance.GetString(br.ReadBytes((int)nameLen));

        uint keyCount = br.ReadUInt32();
        uint boneCount = br.ReadUInt32();

        file.Bones = new List<MotBone>();
        for (int i = 0; i < boneCount; i++)
            file.Bones.Add(MotBone.Read(br));

        return file;
    }

    public void Write(string path) => Write(File.Create(path));

    public void Write(Stream stream)
    {
        using var bw = new BinaryWriter(stream, EucKrEncoding.Instance, leaveOpen: true);
        bw.Write(Id);
        bw.Write(BndId);
        bw.Write((uint)EucKrEncoding.Instance.GetByteCount(Name));
        bw.Write(EucKrEncoding.Instance.GetBytes(Name));
        bw.Write((uint)Bones.Max(b => b.Keys.Count));
        bw.Write((uint)Bones.Count);
        foreach (var bone in Bones)
            bone.Write(bw);
    }
}

public static class MotResampler
{
    public static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * ((2 * p1) +
            (-p0 + p2) * t +
            (2 * p0 - 5 * p1 + 4 * p2 - p3) * t2 +
            (-p0 + 3 * p1 - 3 * p2 + p3) * t3);
    }

    public static Quaternion Squad(Quaternion q0, Quaternion q1, Quaternion q2, Quaternion q3, float t)
    {
        Quaternion slerp1 = Quaternion.Slerp(q1, q2, t);
        Quaternion slerp2 = Quaternion.Slerp(Intermediate(q0, q1, q2), Intermediate(q1, q2, q3), t);
        return Quaternion.Slerp(slerp1, slerp2, 2f * t * (1f - t));
    }

    // Intermediate function to compute Squad tangents
    private static Quaternion Intermediate(Quaternion q0, Quaternion q1, Quaternion q2)
    {
        Quaternion invQ1 = Quaternion.Inverse(q1);
        Quaternion logPart = Log(Quaternion.Multiply(invQ1, q2)) * -0.25f;
        Quaternion logPrev = Log(Quaternion.Multiply(invQ1, q0)) * -0.25f;
        Quaternion sum = AddQuaternions(logPart, logPrev);
        return Quaternion.Multiply(q1, Exp(sum));
    }

    private static Quaternion Log(Quaternion q)
    {
        float a = (float)Math.Acos(q.W);
        float sina = (float)Math.Sin(a);
        if (sina > 0.0001f)
        {
            return new Quaternion(q.X / sina * a, q.Y / sina * a, q.Z / sina * a, 0f);
        }
        return new Quaternion(q.X, q.Y, q.Z, 0f);
    }

    private static Quaternion Exp(Quaternion q)
    {
        float a = (float)Math.Sqrt(q.X * q.X + q.Y * q.Y + q.Z * q.Z);
        float sina = (float)Math.Sin(a);
        float cosa = (float)Math.Cos(a);

        if (a > 0.0001f)
        {
            float coeff = sina / a;
            return new Quaternion(q.X * coeff, q.Y * coeff, q.Z * coeff, cosa);
        }
        return new Quaternion(q.X, q.Y, q.Z, cosa);
    }

    private static Quaternion AddQuaternions(Quaternion q1, Quaternion q2)
    {
        return Quaternion.Normalize(new Quaternion(q1.X + q2.X, q1.Y + q2.Y, q1.Z + q2.Z, q1.W + q2.W));
    }

    // Linear interpolation for Vector3
    public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        return a + (b - a) * t;
    }

    // Spherical linear interpolation for Quaternion
    public static Quaternion Slerp(Quaternion a, Quaternion b, float t)
    {
        // Ensure the shortest path
        if (Quaternion.Dot(a, b) < 0f)
        {
            b = new Quaternion(-b.X, -b.Y, -b.Z, -b.W);
        }
        return Quaternion.Normalize(Quaternion.Slerp(a, b, t));
    }

    public static List<MotKey> ResampleKeysLinear(List<MotKey> keys, float originalFps, float targetFps)
    {
        if (keys.Count < 2) return new List<MotKey>(keys);

        int factor = (int)(targetFps / originalFps);
        if (factor <= 1) return new List<MotKey>(keys);

        var result = new List<MotKey>();

        for (int i = 0; i < keys.Count - 1; i++)
        {
            var prev = keys[i];
            var next = keys[i + 1];

            // Always add the original key
            result.Add(prev);

            // Skip interpolation if segment has no motion
            bool segmentIsZero = prev.Translation == next.Translation && prev.Rotation == next.Rotation;
            if (segmentIsZero)
                continue;

            float segmentDuration = 1f / originalFps; // time between original keys is uniform

            // Insert interpolated keys
            for (int j = 1; j < factor; j++)
            {
                float alpha = j / (float)factor; // normalized [0,1] between prev and next

                var interp = new MotKey
                {
                    Translation = Lerp(prev.Translation, next.Translation, alpha),
                    Rotation = Slerp(prev.Rotation, next.Rotation, alpha)
                };

                result.Add(interp);
            }
        }

        // Add the last key
        result.Add(keys.Last());

        return result;
    }


    public static List<MotKey> ResampleKeysSmooth(List<MotKey> keys, float originalFps, float targetFps)
    {
        if (keys.Count < 2) return new List<MotKey>(keys);

        int factor = (int)(targetFps / originalFps);
        if (factor <= 1) return new List<MotKey>(keys);

        var result = new List<MotKey>();
        float deltaTime = 1f / originalFps;

        for (int i = 0; i < keys.Count - 1; i++)
        {
            var k0 = i > 0 ? keys[i - 1] : keys[i];       // previous key for Catmull-Rom
            var k1 = keys[i];                             // current key
            var k2 = keys[i + 1];                         // next key
            var k3 = (i + 2 < keys.Count) ? keys[i + 2] : keys[i + 1]; // key after next

            // Add the original key
            result.Add(k1);

            // Interpolated keys
            for (int j = 1; j < factor; j++)
            {
                float t = j / (float)factor; // normalized [0,1] between k1 and k2

                // Catmull-Rom interpolation for translation
                var interpPos = CatmullRom(k0.Translation, k1.Translation, k2.Translation, k3.Translation, t);

                // Squad/SLERP interpolation for rotation
                var interpRot = Squad(k0.Rotation, k1.Rotation, k2.Rotation, k3.Rotation, t);

                result.Add(new MotKey
                {
                    Translation = interpPos,
                    Rotation = interpRot
                });
            }
        }

        // Add the last key
        result.Add(keys.Last());

        return result;
    }
}