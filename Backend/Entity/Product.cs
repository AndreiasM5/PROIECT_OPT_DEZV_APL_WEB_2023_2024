using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Backend.Entity;
[Table("product")]
public class Product : IValidatableObject {
    [Column("product_id"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int ProductId {get;set;}
    [Column("name"), Required, StringLength(200)] public string Name {get;set;} = "";
    [Column("price"), Range(typeof(decimal), "0", "9999999999.99")] public decimal Price {get;set;}
    public IEnumerable<ValidationResult> Validate(ValidationContext context) {
        if (decimal.Round(Price, 2) != Price) yield return new ValidationResult("Price must have at most two decimal places.", [nameof(Price)]);
    }
}
