using System;
using System.Collections.Generic;

namespace AMS202024113120.Models;

public partial class Asset
{
    public int AssetId { get; set; }

    public string AssetName { get; set; } = null!;

    public string? AssetSpec { get; set; }

    public decimal? Price { get; set; }

    public DateTime PurchaseDate { get; set; }

    public string? Location { get; set; }

    public int? CategoryId { get; set; }

    public string? ImgName { get; set; }

    public string? CustodianId { get; set; }

    public virtual AssetCategory? Category { get; set; }

    public virtual Employee? Custodian { get; set; }
}
