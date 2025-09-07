using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BikeDealerMgtAPI.Models;

[Index("UserId", Name = "UX_Dealers_UserId", IsUnique = true)]
public partial class Dealer
{
    [Key]
    public int DealerId { get; set; }

    [StringLength(100)]
    public string DealerName { get; set; } = null!;

    [StringLength(200)]
    public string Address { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? City { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? State { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? ZipCode { get; set; }

    public int StorageCapacity { get; set; } = 0;

    public int Inventory { get; set; } = 0;

    public string? UserId { get; set; } = null!;

    [InverseProperty("Dealer")]
    public virtual ICollection<DealerMaster> DealerMasters { get; set; } = new List<DealerMaster>();

    [ForeignKey("UserId")]
    [InverseProperty("Dealer")]
    public virtual AspNetUser? User { get; set; } = null!;
}
