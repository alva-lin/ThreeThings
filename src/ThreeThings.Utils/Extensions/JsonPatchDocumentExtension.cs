using System.Reflection;

using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;

namespace ThreeThings.Utils.Extensions;

public static class JsonPatchDocumentExtension
{
    private static readonly Lazy<IMemoryCache> Cache = new Lazy<IMemoryCache>(() => new MemoryCache(new MemoryCacheOptions()));

    public static void ApplyToSafely<T>(this JsonPatchDocument<T> patchDoc, T objectToApplyTo, ModelStateDictionary? modelState = null)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(patchDoc);
        ArgumentNullException.ThrowIfNull(objectToApplyTo);

        var properties = Cache.Value.GetOrCreate(typeof(T).Name, _ =>
        {
            // get public non-static properties up the dependency tree
            var attrs    = BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance;
            var nameList = typeof(T).GetProperties(attrs).Select(p => p.Name).ToList();
            return nameList;
        });

        var operations = new List<Operation<T>>();
        foreach (var op in patchDoc.Operations)
        {
            if (!string.IsNullOrWhiteSpace(op.path))
            {
                var segments = op.path.TrimStart('/').Split('/');
                var target   = segments.First();
                if (properties.Contains(target, StringComparer.OrdinalIgnoreCase))
                {
                    operations.Add(op);
                }
            }
        }
        var newPatchDoc = new JsonPatchDocument<T>(operations, patchDoc.ContractResolver);

        if (modelState != null)
        {
            newPatchDoc.ApplyTo(objectToApplyTo, modelState);
        }
        else
        {
            newPatchDoc.ApplyTo(objectToApplyTo);
        }
    }
}
