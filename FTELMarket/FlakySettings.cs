using System.ComponentModel.DataAnnotations;

public class FlakySettings
{
    [Required(AllowEmptyStrings = false)]
    public string BaseAddress { get; set; } = string.Empty;
}