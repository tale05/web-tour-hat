using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_tour.Models.Company
{
    [Table("company")]
    public class CompanyModel
    {
        [Key]
        [Column("company_id", TypeName = "char(20)")]
        [StringLength(20)]
        public string CompanyId { get; set; }

        [Column("name_vie", TypeName = "nvarchar(255)")]
        [StringLength(255)]
        public string NameVie { get; set; }

        [Column("name_eng", TypeName = "nvarchar(255)")]
        [StringLength(255)]
        public string NameEng { get; set; }

        [Column("name_abbr", TypeName = "nvarchar(255)")]
        [StringLength(255)]
        public string NameAbbr { get; set; }

        [Column("company_email", TypeName = "nvarchar(100)")]
        [StringLength(100)]
        public string CompanyEmail { get; set; }

        [Column("company_phone", TypeName = "varchar(20)")]
        [StringLength(20)]
        public string CompanyPhone { get; set; }

        [Column("company_address", TypeName = "nvarchar(255)")]
        [StringLength(255)]
        public string CompanyAddress { get; set; }

        [Column("company_description", TypeName = "nvarchar(max)")]
        public string CompanyDescription { get; set; }

        [Column("business_license_no", TypeName = "varchar(100)")]
        [StringLength(100)]
        public string BusinessLicenseNo { get; set; }

        [Column("business_license_date", TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? BusinessLicenseDate { get; set; }

        [Column("issued_by", TypeName = "nvarchar(255)")]
        [StringLength(255)]
        public string IssuedBy { get; set; }

        [Column("international_travel_license_no", TypeName = "varchar(100)")]
        [StringLength(100)]
        public string InternationalTravelLicenseNo { get; set; }

        [Column("international_travel_license_date", TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? InternationalTravelLicenseDate { get; set; }

        [Column("facebook_url", TypeName = "nvarchar(255)")]
        [StringLength(255)]
        public string FacebookUrl { get; set; }
    }
}