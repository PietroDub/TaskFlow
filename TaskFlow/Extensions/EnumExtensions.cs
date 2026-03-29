using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace TaskFlow.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString() ?? "");
            if (field is null)
                return value.ToString() ?? "";

            var attr = field.GetCustomAttribute<DisplayAttribute>();
            return attr?.Name ?? value.ToString() ?? "";
        }
    }
}
