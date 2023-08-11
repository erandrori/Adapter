using UnityEngine;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public static class Helper
{
    public static string GetScenePath(string path)
    {
        if (path.ToLower().LastIndexOf("assets/") == -1)
            path = "Assets/" + path;
        if (path.LastIndexOf(".unity") == -1)
            path += ".unity";
        return path;
    }

    public static void SetLayerRecursively(this Transform parent, int layer)
    {
        parent.gameObject.layer = layer;

        for (int i = 0, count = parent.childCount; i < count; i++)
        {
            parent.GetChild(i).SetLayerRecursively(layer);
        }

        Bounds b = new Bounds();
        b.size = new Vector3(0, 0, 0);
    }

    public static async Task PlayAsyncExtension(this Animator animator, int animationStateHash, int layerIndex = 0, CancellationToken cancellationToken = default)
    {
        if (animator == null)
            return;

        animator.Play(animationStateHash, layerIndex);
        var clipInfo = animator.GetCurrentAnimatorClipInfo(layerIndex)[0];

        await Task.Delay(Mathf.CeilToInt(clipInfo.clip.length * 1000), cancellationToken: cancellationToken);
    }

    public static async Task PlayAsync(this Animator animator, string animationName, CancellationToken cancellationToken = default)
    {
        if (animator == null)
            return;

        var clip = animator.GetCurrentAnimatorClipInfo(0);
        animator.Play(animationName);

        await Task.Delay(Mathf.CeilToInt(clip[0].clip.length * 1000), cancellationToken: cancellationToken);
    }

    public static async Task PlayAsync(this Animation animation, string animationName, CancellationToken cancellationToken = default)
    {
        if (animation == null)
            return;

        var clip = animation.GetClip(animationName);
        animation.Play(animationName);

        await Task.Delay(Mathf.CeilToInt(clip.length * 1000), cancellationToken: cancellationToken);
    }

    public static void DrawBounds(this Bounds b, Color color, string layerName, Transform parent, float lineWidth = 0.05f, float duration = 5)
    {

#if UNITY_EDITOR
        // bottom
        var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
        var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
        var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
        var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

        Debug.DrawLine(p1, p2, color, duration);
        Debug.DrawLine(p2, p3, color, duration);
        Debug.DrawLine(p3, p4, color, duration);
        Debug.DrawLine(p4, p1, color, duration);

        // top
        var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
        var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
        var p7 = new Vector3(b.max.x, b.max.y, b.max.z);
        var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

        Debug.DrawLine(p5, p6, color, duration);
        Debug.DrawLine(p6, p7, color, duration);
        Debug.DrawLine(p7, p8, color, duration);
        Debug.DrawLine(p8, p5, color, duration);

        // sides
        Debug.DrawLine(p1, p5, color, duration);
        Debug.DrawLine(p2, p6, color, duration);
        Debug.DrawLine(p3, p7, color, duration);
        Debug.DrawLine(p4, p8, color, duration);

        var boundsGo = new GameObject("Bounds");
        boundsGo.transform.SetParent(parent);
        boundsGo.layer = LayerMask.NameToLayer(layerName);
        var lineRenderer = boundsGo.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 4;
        lineRenderer.SetPositions(new[] { p1, p5, p6, p2 });
        lineRenderer.loop = true;
        lineRenderer.startWidth = lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material.color = color;

        Object.Destroy(boundsGo, duration);
#endif
    }

    //public static Rect GetScreenRect(this RectTransform rectTransform)
    //{
    //    var worldCorners = new Vector3[4];
    //    rectTransform.GetWorldCorners(worldCorners);
    //    var min = Bg.V.Cameras.UIcamera.WorldToScreenPoint(worldCorners[0]);
    //    var max = Bg.V.Cameras.UIcamera.WorldToScreenPoint(worldCorners[2]);
    //    var rect = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
    //    return rect;
    //}

    public static Bounds GetWorldBounds(this RectTransform transform)
    {
        var worldCorners = new Vector3[4];
        transform.GetWorldCorners(worldCorners);
        var bounds = new Bounds(worldCorners[0], Vector3.zero);
        for (int i = 1; i < 4; ++i)
        {
            bounds.Encapsulate(worldCorners[i]);
        }
        return bounds;
    }

    public static bool ContainBounds(this Bounds bounds, Bounds target)
    {
        return bounds.Contains(target.min) && bounds.Contains(target.max);
    }

    public static void SetX(this ref Vector3 vectorToSet, float value)
    {
        vectorToSet.Set(value, vectorToSet.y, vectorToSet.z);
    }

    public static void SetY(this ref Vector3 vectorToSet, float value)
    {
        vectorToSet.Set(vectorToSet.x, value, vectorToSet.z);
    }

    public static void SetZ(this ref Vector3 vectorToSet, float value)
    {
        vectorToSet.Set(vectorToSet.x, vectorToSet.y, value);
    }

    public static void SetX(this ref Vector2 vectorToSet, float value)
    {
        vectorToSet.Set(value, vectorToSet.y);
    }

    public static void SetY(this ref Vector2 vectorToSet, float value)
    {
        vectorToSet.Set(vectorToSet.x, value);
    }

    public static void AddIfNotContained<T>(this HashSet<T> set, T newElement)
    {
        if (!set.Contains(newElement))
        {
            set.Add(newElement);
        }
    }

    public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
    {
        var isCollectionNull = collection == null;
        if (isCollectionNull)
        {
            return true;
        }
        var isCollectionEmpty = collection.Count == 0;
        var isNullOrEmpty = isCollectionNull || isCollectionEmpty;
        return isNullOrEmpty;
    }

    public static T GetRandomElement<T>(this ICollection<T> collection)
    {
        var randomIndex = Random.Range(0, collection.Count);
        return collection.ElementAt(randomIndex);
    }
}

