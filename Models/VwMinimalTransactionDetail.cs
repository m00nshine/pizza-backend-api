using System;
using System.Collections.Generic;

namespace pizza_backend_api.Models;

public partial class VwMinimalTransactionDetail
{
    public int OrderId { get; set; }

    public decimal? TotalPrice { get; set; }

    public DateTime CreatedDate { get; set; }
}
