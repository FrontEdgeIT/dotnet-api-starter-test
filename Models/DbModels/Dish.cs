using System.ComponentModel.DataAnnotations;

namespace dotnet_api_test
    

{
    public class Dish
    {
        [Key] [Required] public int Id { get; set; }

        [Required] public string Name { get; set; } = "";

        [Required] public string MadeBy { get; set; } = "";

        [Required] public double Cost { get; set; }
    }
}