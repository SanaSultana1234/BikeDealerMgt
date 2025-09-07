using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BikeDealerMgtAPI.Models;

[Table("DealerMaster")]
public partial class DealerMaster
{
    [Key]
    public int DealerMasterId { get; set; }

    public int DealerId { get; set; }

    public int BikeId { get; set; }

    public int? BikesDelivered { get; set; }

    public DateTime? DeliveryDate { get; set; }

    [ForeignKey("BikeId")]
    [InverseProperty("DealerMasters")]
    public virtual BikeStore? Bike { get; set; } = null!;

    [ForeignKey("DealerId")]
    [InverseProperty("DealerMasters")]
    public virtual Dealer? Dealer { get; set; } = null!;
}
