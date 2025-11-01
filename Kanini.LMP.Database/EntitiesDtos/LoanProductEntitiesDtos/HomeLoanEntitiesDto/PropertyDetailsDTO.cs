using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.HomeLoanEntitiesDto
{
    public class PropertyDetailsDTO
    {
        // Unique identifier for this property record
        public int PropertyDetailsId { get; set; }

        // Foreign key to the base loan application
        [Required(ErrorMessage = "Loan Application Base ID is required.")]
        public int LoanApplicationBaseId { get; set; }

        // Foreign key to the linked user
        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        // Property characteristics
        [Required(ErrorMessage = "Property type is required.")]
        public int PropertyType { get; set; }  // Enum → PropertyType

        [Required(ErrorMessage = "Property address is required.")]
        [MaxLength(250, ErrorMessage = "Property address cannot exceed 250 characters.")]
        public string PropertyAddress { get; set; } = null!;

        [Required(ErrorMessage = "City is required.")]
        [MaxLength(100, ErrorMessage = "City name cannot exceed 100 characters.")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "State is required.")]
        [MaxLength(100, ErrorMessage = "State name cannot exceed 100 characters.")]
        public string State { get; set; } = null!;

        [Required(ErrorMessage = "Zip code is required.")]
        [Range(100000, 999999, ErrorMessage = "Please enter a valid 6-digit zip code.")]
        public int ZipCode { get; set; }

        [Required(ErrorMessage = "Ownership type is required.")]
        public int OwnershipType { get; set; }  // Enum → OwnershipType
    }
}
