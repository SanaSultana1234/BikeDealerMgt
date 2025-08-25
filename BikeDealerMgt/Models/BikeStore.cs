using System;
using System.Collections.Generic;

namespace BikeDealerMgtAPI.Models;

public partial class BikeStore
{
    public int BikeId { get; set; }

    public string ModelName { get; set; } = null!;

    public int? ModelYear { get; set; }

    public int? EngineCc { get; set; }

    public string? Manufacturer { get; set; }

    public virtual ICollection<DealerMaster>? DealerMasters { get; set; } = new List<DealerMaster>();
}
