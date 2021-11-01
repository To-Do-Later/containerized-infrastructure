using Cake.Core;
using Cake.Core.IO;

public static class ProcessArgumentBuilderExtensions
{
    public static ProcessArgumentBuilder AppendProperty(this ProcessArgumentBuilder builder, string property, string value)
    {
        if (string.IsNullOrEmpty(property))
        {
            throw new System.ArgumentException($"'{nameof(property)}' cannot be null or empty.", nameof(property));
        }

        if (!string.IsNullOrEmpty(value))
        {
            builder.Append($"/p:{property}={value}");
        }

        return builder;
    }
}

