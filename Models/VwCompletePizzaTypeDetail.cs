using System;
using System.Collections.Generic;

namespace pizza_backend_api.Models;

public partial class VwCompletePizzaTypeDetail
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Category { get; set; }

    public string? Description { get; set; }
}
