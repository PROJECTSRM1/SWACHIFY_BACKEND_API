using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Swachify.Application.Interfaces;
using Swachify.Application.Models;
using Swachify.Infrastructure.Data;
using Swachify.Infrastructure.Models;
using Twilio;
using Twilio.Rest.Verify.V2.Service;
using Twilio.Types;

namespace Swachify.Application;

public class OtpService : IOtpService
{
    private readonly string _accountSid;
    private readonly string _authToken;
    private readonly string _verifyServiceSid;
    private readonly MyDbContext _db;
    private readonly ISMSService _smsService;
    private readonly IEmailService _emailService;
    public OtpService(IConfiguration configuration, MyDbContext db, ISMSService smsService, IEmailService emailService)
    {
        _accountSid = configuration["Twilio:AccountSid"];
        _authToken = configuration["Twilio:AuthToken"];
        _verifyServiceSid = configuration["Twilio:VerifyServiceSid"];
        TwilioClient.Init(_accountSid, _authToken);
        _db = db;
        _smsService = smsService;
        _emailService = emailService;
    }

    public async Task<bool> SendMobileOtpAsync(string phoneNumber)
    {
        var verification = await VerificationResource.CreateAsync(
            to: phoneNumber,
            channel: "sms",
            pathServiceSid: _verifyServiceSid
        );
        return verification.Status == "pending";
    }

    public async Task<bool> VerifyMobileOtpAsync(string phoneNumber, string code)
    {
        var verificationCheck = await VerificationCheckResource.CreateAsync(
            to: phoneNumber,
            code: code,
            pathServiceSid: _verifyServiceSid
        );
        return verificationCheck.Status == "approved";
    }

    public async Task<string> SendCustomerOtpAsync(CustomerOTPDto request)
    {
        var user = await _db.user_registrations.FirstOrDefaultAsync(d => d.id == request.user_id
        || d.mobile == request.phoneNumber || d.email == request.email);
        long newotp = 0;
        var bookingidOTP = await _db.otp_histories.FirstOrDefaultAsync(d => d.booking_id == request.booking_id && d.is_active == true);
        if (bookingidOTP == null || bookingidOTP?.otp == 0)
        {
            if (user == null)
                return "";
            newotp = Generate6DigitOtp();
            var otphistory = new otp_history
            {
                otp = newotp,
                user_id = request.user_id,
                booking_id = request.booking_id,
                is_active = true
            };

            await _db.otp_histories.AddAsync(otphistory);
            await _db.SaveChangesAsync();
        }
        else
        {
            newotp = bookingidOTP.otp;
        }

        //string[] values = { request.customer_name, request.agent_name, newotp.ToString() };
        //int index = 0;
        //string message = Regex.Replace(AppConstants.OtpMessage, @"\{#var#\}", m => values[index++]);

        var msg = AppConstants.otpsms.Replace("{customername}", request.customer_name).Replace("{otp}", newotp.ToString());

        if (!string.IsNullOrEmpty(request?.phoneNumber))
        {

            var requestcmd = new SMSRequestDto(request?.phoneNumber, msg);
            await _smsService.SendSMSAsync(requestcmd);
        }
        if (!string.IsNullOrEmpty(request?.email))
        {
            await _emailService.SendEmailAsync(request?.email, "Your Swachify Service OTP", msg);
        }
        return "OTP sent successfully";
    }

    public async Task<string> VerifyCustomerOtpAsync(CustomerOTPDto request)
    {
        if (request?.booking_id > 0)
        {
            var otphistory = await _db.otp_histories.FirstOrDefaultAsync(d => d.booking_id == request.booking_id);
            if (otphistory.otp == request.otp)
            {
                otphistory.is_active = false;
                await _db.SaveChangesAsync();
                return "OTP verified Successfully";
            }
            else
            {
                return "please sent valid otp";
            }
        }
        return "Invalid Booking ID Provided for otp verification";

    }

    public long Generate6DigitOtp()
    {
        var random = new Random();
        return random.Next(100000, 999999);
    }


}
