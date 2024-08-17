using System;
using System.Collections.Generic;

namespace AutorizationMicroService.Models;

public partial class Admin
{
    public int PersonId { get; set; }

    public string Shapass { get; set; } = null!;

    public bool? Stat { get; set; }

    public int Occ_id { get; set; }

    //public virtual Person Person { get; set; } = null!;

}
