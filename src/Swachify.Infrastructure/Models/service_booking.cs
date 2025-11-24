using System;
using System.Collections.Generic;

namespace Swachify.Infrastructure.Models;

public partial class service_booking
{
    public long id { get; set; }

    public string? booking_id { get; set; }

    public long slot_id { get; set; }

    public long? created_by { get; set; }

    public DateTime? created_date { get; set; }

    public long? modified_by { get; set; }

    public DateTime? modified_date { get; set; }

    public bool? is_active { get; set; }

    public DateOnly? preferred_date { get; set; }

    public string? full_name { get; set; }

    public string? phone { get; set; }

    public string? email { get; set; }

    public string? address { get; set; }

    public long? status_id { get; set; }

    public long? assign_to { get; set; }

    public bool? unavailable { get; set; }

    public long? service_type_id { get; set; }

    public decimal? total { get; set; }

    public decimal? subtotal { get; set; }

    public decimal? customer_requested_amount { get; set; }

    public decimal? discount_amount { get; set; }

    public decimal? discount_percentage { get; set; }

    public decimal? discount_total { get; set; }

    public int? hours { get; set; }

    public int? add_on_hours { get; set; }

    public virtual user_registration? created_byNavigation { get; set; }

    public virtual user_registration? modified_byNavigation { get; set; }

    public virtual ICollection<otp_history> otp_histories { get; set; } = new List<otp_history>();

    public virtual ICollection<service_tracking> service_trackings { get; set; } = new List<service_tracking>();

    public virtual master_service_type? service_type { get; set; }

    public virtual master_slot slot { get; set; } = null!;

    public virtual master_status? status { get; set; }
}
