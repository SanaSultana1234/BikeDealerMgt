using System;
using System.Collections.Generic;

namespace BikeDealerMgtAPI.Models;

public partial class Dealer
{
    public int DealerId { get; set; }

    public string DealerName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public string ZipCode { get; set; } = null!;

    public int? StorageCapacity { get; set; }

    public int? Inventory { get; set; }

    public virtual ICollection<DealerMaster>? DealerMasters { get; set; } = new List<DealerMaster>();
}
