using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.EF;

public partial class Person
{
    [Key]
    public int PersonId { get; set; }

    [StringLength(450)]
    public string UserId { get; set; } = null!;

    [Column(TypeName = "image")]
    public byte[]? Image { get; set; }

    public bool IsActive { get; set; }

}