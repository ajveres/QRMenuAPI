using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace QRMenuAPI.Models
{
	public class ApplicationUser: IdentityUser
	{

        [StringLength(100, MinimumLength = 2)]
        [Column(TypeName = "nvarchar(100)")]
        public override string? UserName { get => base.UserName; set => base.UserName = value; }

        [StringLength(100, MinimumLength = 2)]
        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; } = "";

        public DateTime RegisterDate { get; set; }


        [ForeignKey("StateId")]
        public State? State { get; set; }
        public byte StateId { get; set; }

        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }
        public int CompanyId { get; set; }


        [EmailAddress]
        [StringLength(100, MinimumLength = 5)]
        [Column(TypeName = "nvarchar(100)")]
        public override string Email { get; set; } = "";


        [Phone]
        [StringLength(30)]
        [Column(TypeName = "varchar(30)")]
        public override string PhoneNumber { get => base.PhoneNumber; set => base.PhoneNumber = value; }



    }
}

