using JCertPreApplication.Application.Dtos;
using System.ComponentModel;
namespace JCertPreApplication.Application.Utilities;
public static class EnumHelper
{
    public static List<EnumValueDto> GetEnumValuesWithDescriptions<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T))
            .Cast<T>()
            .Select(e => new EnumValueDto
            {
                Name = e.ToString(),
                Description = (typeof(T).GetField(e.ToString())?
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .FirstOrDefault() as DescriptionAttribute)?.Description ?? e.ToString(),
                Value = Convert.ToInt32(e)
            })
            .ToList();
    }

    public static string GetEnumDescription(Enum value)
    {
        var fi = value.GetType().GetField(value.ToString());
        if (fi != null)
        {
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
                return attributes[0].Description;
        }
        return value.ToString();
    }
}