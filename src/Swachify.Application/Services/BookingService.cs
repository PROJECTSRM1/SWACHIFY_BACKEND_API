using System.Text.Json;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Swachify.Application.DTOs;
using Swachify.Application.Interfaces;
using Swachify.Application.Models;
using Swachify.Infrastructure.Data;
using Swachify.Infrastructure.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Swachify.Application.Services
{
  public class BookingService(MyDbContext _db, IEmailService _emailService, ISMSService smsService, IOtpService otpService) : IBookingService
  {
    public async Task<List<AllBookingsDtos>> GetAllBookingsAsync(long status_id = -1, int limit = 10, int offset = 0)
    {
      var query = string.Format(DbConstants.fn_service_booking_list, -1, -1, -1, status_id, limit, offset);
      using var conn = _db.Database.GetDbConnection();
      if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
      var rawData = await conn.QueryAsync<AllBookingsDtos>(query);
      var jsonOptions = new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      };
      foreach (var rawD in rawData.ToList())
      {
        rawD.serviceslist = JsonSerializer.Deserialize<List<BookingServiceDto>>(rawD.services, jsonOptions)
?? new List<BookingServiceDto>();
        rawD.services = "";
      }
      return rawData.ToList();
    }

    public async Task<List<AllBookingsDtos>> GetAllBookingByBookingIDAsync(long bookingId, int limit = 10, int offset = 0)
    {
      string query = string.Format(DbConstants.fn_service_booking_list, bookingId, -1, -1, -1, limit, offset);

      using var conn = _db.Database.GetDbConnection();
      if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
      var rawData = await conn.QueryAsync<AllBookingsDtos>(query);
      var jsonOptions = new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      };
      foreach (var rawD in rawData.ToList())
      {
        rawD.serviceslist = JsonSerializer.Deserialize<List<BookingServiceDto>>(rawD.services, jsonOptions)
         ?? new List<BookingServiceDto>();
        rawD.services = "";
      }
      return rawData.ToList();
    }

    public async Task<List<AllBookingsDtos>> GetAllBookingByUserIDAsync(long userid, long empid, int limit = 10, int offset = 0)
    {
      userid = userid > 0 ? userid : -1;
      empid = empid > 0 ? empid : -1;
      string query = string.Format(DbConstants.fn_service_booking_list, -1, userid, empid, -1, limit, offset);
      using var conn = _db.Database.GetDbConnection();
      if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
      var rawData = await conn.QueryAsync<AllBookingsDtos>(query);
      var jsonOptions = new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      };
      foreach (var rawD in rawData.ToList())
      {
        rawD.serviceslist = JsonSerializer.Deserialize<List<BookingServiceDto>>(rawD.services, jsonOptions)
         ?? new List<BookingServiceDto>();
        rawD.services = "";
      }
      return rawData.ToList();
    }

    public async Task<long> CreateAsync(service_booking booking, CancellationToken ct = default)
    {
      var bookingID = Guid.NewGuid().ToString();
      booking.created_date = DateTime.Now;
      booking.is_active = true;
      booking.booking_id ??= bookingID;
      booking.full_name = booking.full_name;
      booking.address = booking.address;
      booking.phone = booking.phone;
      booking.email = booking.email;
      booking.status_id = 1;
      booking.total = booking.total;
      booking.subtotal = booking.subtotal;
      booking.customer_requested_amount = booking.customer_requested_amount;
      booking.discount_amount = booking.discount_amount;
      booking.discount_percentage = booking.discount_percentage;
      booking.discount_total = booking.discount_total;
      booking.hours = booking.hours;
      booking.add_on_hours = booking.add_on_hours;

      _db.service_bookings.Add(booking);

      await _db.SaveChangesAsync(ct);

      var newTrackings = booking.service_trackings
    .Select(item => new service_tracking
    {
      service_booking_id = booking.id,
      booking_id = bookingID,
      dept_id = item.dept_id,
      service_id = item.service_id,
      service_type_id = item.service_type_id,
      room_sqfts = item.room_sqfts,
      with_basement = item.with_basement,
    })
    .ToList();
      _db.service_trackings.AddRange(newTrackings);
      await _db.SaveChangesAsync(ct);

      //Save custromer information 
      var user = new user_registration
      {
        email = booking.email,
        first_name = booking.full_name,
        last_name = "",
        mobile = booking.phone,
        role_id = 4,
      };
      _db.user_registrations.AddRange(user);
      await _db.SaveChangesAsync(ct);

      if (!string.IsNullOrEmpty(booking.phone))
      {
                var message = AppConstants.WelcomeSMSmessage.Replace("{#var#}", booking.full_name);
        var request = new SMSRequestDto(booking?.phone, message);
        await smsService.SendSMSAsync(request);
      }

      if (!string.IsNullOrEmpty(booking.email))
      {
        var serviceName = await _db.master_departments.FirstOrDefaultAsync(d => d.id == booking.id);
        var subject = $"Thank You for Choosing Swachify Cleaning Service!";
        var mailtemplate = await _db.email_templates.FirstOrDefaultAsync(b => b.title == AppConstants.ServiceBookingMail);
        string emailBody = mailtemplate.description
        .Replace("{0}", booking.full_name)
        .Replace("{1}", serviceName?.department_name + " Service");
        if (mailtemplate != null)
        {
          await _emailService.SendEmailAsync(booking.email, subject, emailBody);
        }
      }
      return booking.id;
    }

    public async Task<bool> UpdateAsync(long id, service_booking updatedBooking, CancellationToken ct = default)
    {
      var existing = await _db.service_bookings.FirstOrDefaultAsync(b => b.id == id, ct);
      if (existing == null) return false;

      existing.slot_id = updatedBooking.slot_id;
      existing.modified_by = updatedBooking.modified_by;
      existing.modified_date = DateTime.UtcNow;
      existing.preferred_date = updatedBooking.preferred_date;
      existing.is_active = updatedBooking.is_active;
      existing.full_name = updatedBooking.full_name;
      existing.address = updatedBooking.address;
      existing.phone = updatedBooking.phone;
      existing.email = updatedBooking.email;
      existing.status_id = updatedBooking.status_id;
      await _db.SaveChangesAsync(ct);
      return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
      var booking = await _db.service_bookings.FirstOrDefaultAsync(b => b.id == id, ct);
      if (booking == null) return false;

      // _db.service_bookings.Remove(booking);
      booking.is_active = false;
      await _db.SaveChangesAsync(ct);
      return true;
    }

    public async Task<bool> UpdateTicketByEmployeeInprogress(long id)
    {
      var existing = await _db.service_bookings.FirstOrDefaultAsync(b => b.id == id);
      if (existing == null) return false;
      existing.status_id = 3;
      await _db.SaveChangesAsync();
      return true;
    }

    public async Task<bool> UpdateTicketByEmployeeCompleted(long id)
    {
      var existing = await _db.service_bookings.FirstOrDefaultAsync(b => b.id == id);
      if (existing == null) return false;
      existing.status_id = 4;
      await _db.SaveChangesAsync();
      var subject = $"Your Cleaning Service Is Completed!";
      var mailtemplate = await _db.email_templates.FirstOrDefaultAsync(b => b.title == AppConstants.CustomerAssignMail);
      string emailBody = mailtemplate.description
      .Replace("{0}", existing?.full_name);

      if (mailtemplate != null)
      {
        await _emailService.SendEmailAsync(existing.email, subject, emailBody);
      }

      return true;
    }
    public async Task<bool> AssignEmployee(long id, long user_id)
    {
      var existing = await _db.service_bookings.FirstOrDefaultAsync(b => b.id == id);
      var mailtemplate = await _db.email_templates.FirstOrDefaultAsync(b => b.title == AppConstants.CustomerAssignedAgent);
      var agentmailtemplate = await _db.email_templates.FirstOrDefaultAsync(b => b.title == AppConstants.EMPAssignmentMail);
      if (existing == null) return false;
      existing.status_id = 2;
      existing.assign_to = user_id;
      await _db.SaveChangesAsync();

      var resultBookings = await GetAllBookingByBookingIDAsync(id);
      var departnames = string.Join(",", resultBookings
       .Where(b => b?.serviceslist != null)
       .SelectMany(b => b.serviceslist)
       .Select(s => $"[{s.department_name} - {s.service_name}]")
       .Where(name => !string.IsNullOrEmpty(name))
       .ToList());
      var agentemail = resultBookings.FirstOrDefault().employee_email;
      var agentname = resultBookings.FirstOrDefault().employee_name;

      string emailBody = mailtemplate.description
      .Replace("{0}", existing?.full_name)
      .Replace("{1}", agentname)
      .Replace("{2}", existing.preferred_date.ToString() ?? DateTime.Now.ToString())
      .Replace("{3}", departnames)
      .Replace("{4}", "India");
      if (mailtemplate != null)
      {
        await _emailService.SendEmailAsync(existing.email, AppConstants.CustomerAssignedAgent, emailBody);
      }
      string agentEmailBody = agentmailtemplate?.description.ToString()
       .Replace("{0}", existing?.id.ToString() + departnames)
       .Replace("{1}", agentname)
       .Replace("{2}", existing?.id.ToString())
       .Replace("{3}", existing?.full_name)
      .Replace("{4}", "India")
      .Replace("{5}", existing.preferred_date.ToString());

      var subject = $"New Service Assigned - {existing?.id}";
      if (mailtemplate != null)
      {
        await _emailService.SendEmailAsync(agentemail, subject, agentEmailBody);
      }

      var requestdto = resultBookings.FirstOrDefault();
      //send otp to the Mail and phone 
      var request2 = new CustomerOTPDto
      (
        0, 0,
        requestdto.phone,
        requestdto.id,
        requestdto.email,
        requestdto.full_name,
        requestdto.employee_name
      );
      await otpService.SendCustomerOtpAsync(request2);

      return true;
    }

  }
}
