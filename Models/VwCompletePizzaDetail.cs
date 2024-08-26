using System;
using System.Collections.Generic;

namespace pizza_backend_api.Models;

public partial class VwCompletePizzaDetail
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Code { get; set; }

    public decimal Price { get; set; }
}
