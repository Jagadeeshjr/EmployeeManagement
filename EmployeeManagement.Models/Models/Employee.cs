using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.Models.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name should not be Empty"), StringLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Salary should not be Empty")]
        public long Salary { get; set; }

        [Required, StringLength(50)]
        public string Location { get; set; }

        [Required, StringLength(50)]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Your Email is not valid.")]
        public string Email { get; set; }

        [Required, StringLength(50)]
        public string Department { get; set; }

        [Required, StringLength(100)]
        public string Qualification { get; set; }

        [Required]
        [EnumDataType(typeof(Gender), ErrorMessage = "Please provide Male or Female")]
        public Gender Gender { get; set; }

        [Required, StringLength(10)]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone Number must be 10 digits")]
        [Column("PhoneNumber")]
        public string PhoneNo { get; set; }

        [DataType(DataType.Text)]
        public DateTime CreatedDate { get; set; }

        [DataType(DataType.Text)]
        public DateTime UpdatedDate { get; set; }
    }

    public enum Gender
    {
        Male,
        Female
    }
}
