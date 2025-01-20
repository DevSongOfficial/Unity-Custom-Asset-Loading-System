using System;
using UnityEngine;

public static class ResourceLoader
{
    public static T GetResource<T>(Enum enumMember) where T : UnityEngine.Object
    {
        string fullPath = GetEnumPath(enumMember);

        var resource = Resources.Load<T>(fullPath);
        if (resource == null)
        {
            Debug.LogWarning($"Invalid name or path: {fullPath}");
            return null;
        }

        return resource;
    }

    private static string GetEnumPath(Enum enumMember)
    {
        var enumType = enumMember.GetType();
        var path = new System.Text.StringBuilder();

        while (enumType != null)
        {
            path.Insert(0, $"{enumType.Name}/");
            enumType = enumType.DeclaringType;
        }

        path.Append(enumMember.ToString());

        return path.ToString();
    }
}
