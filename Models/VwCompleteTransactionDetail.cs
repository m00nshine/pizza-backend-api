using System;
using System.Collections.Generic;

namespace pizza_backend_api.Models;

public partial class VwCompleteTransactionDetail
{
    public int OrderId { get; set; }

    public int Quantity { get; set; }

    public decimal? Price { get; set; }

    public decimal? Amount { get; set; }

    public string? Name { get; set; }

    public string? Category { get; set; }

    public string? Size { get; set; }
}
