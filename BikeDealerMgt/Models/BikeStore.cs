using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BikeDealerMgtAPI.Models;

public partial class BikeStore
{
    [Key]
    public int BikeId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string ModelName { get; set; } = null!;

    public int? ModelYear { get; set; }

    [Column("EngineCC")]
    public int? EngineCc { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Manufacturer { get; set; }

    [InverseProperty("Bike")]
    public virtual ICollection<DealerMaster> DealerMasters { get; set; } = new List<DealerMaster>();
}
