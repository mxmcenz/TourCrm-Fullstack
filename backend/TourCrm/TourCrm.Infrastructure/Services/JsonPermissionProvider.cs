using System.Text.Json;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Permissions;
using TourCrm.Application.Interfaces;

namespace TourCrm.Infrastructure.Services;

public class JsonPermissionProvider(string filePath) : IPermissionProvider
{
    public async Task<IReadOnlyList<PermissionCategoryDto>> GetPermissionTreeAsync()
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException("Permissions file not found", filePath);

        await using var s = File.OpenRead(filePath);
        using var doc = await JsonDocument.ParseAsync(s);
        var root = doc.RootElement;
        if (root.ValueKind != JsonValueKind.Object)
            throw new InvalidDataException("Root must be an object of categories.");

        var cats = new List<PermissionCategoryDto>();
        var allKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var cat in root.EnumerateObject())
        {
            var node = cat.Value;

            if (!node.TryGetProperty("items", out var items) || items.ValueKind != JsonValueKind.Array)
                throw new InvalidDataException($"Category '{cat.Name}' must contain array 'items'.");

            var catName = node.TryGetProperty("name", out var cn) ? cn.GetString() : cat.Name;
            var list = new List<PermissionDto>();

            foreach (var it in items.EnumerateArray())
            {
                if (!it.TryGetProperty("key", out var k) || !it.TryGetProperty("name", out var n))
                    throw new InvalidDataException($"Each item in '{cat.Name}' must have 'key' and 'name'.");

                var key = k.GetString();
                var name = n.GetString();
                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(name))
                    throw new InvalidDataException($"Empty key/name in category '{cat.Name}'.");

                if (!allKeys.Add(key!))
                    throw new InvalidDataException($"Duplicate permission key: '{key}'.");

                list.Add(new PermissionDto { Key = key!, Name = name! });
            }

            cats.Add(new PermissionCategoryDto { Key = cat.Name, Name = catName ?? cat.Name, Items = list });
        }

        return cats;
    }

    public async Task<IReadOnlyList<PermissionDto>> GetPermissionsAsync()
    {
        var tree = await GetPermissionTreeAsync();
        return tree.SelectMany(c => c.Items).ToList();
    }
}