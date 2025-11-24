using Microsoft.AspNetCore.Mvc;
using Swachify.Application;
using Swachify.Application.Interfaces;
using Swachify.Application.Models;

namespace Swachify.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OtpController : ControllerBase
    {
        private readonly IOtpService _otpService;
        private readonly ISMSService _smsService;

        public OtpController(IOtpService otpService,ISMSService smsService)
        {
            _otpService = otpService;
            _smsService=smsService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMobileOtp([FromQuery] string phoneNumber)
        {
            var sent = await _otpService.SendMobileOtpAsync(phoneNumber);
            return sent ? Ok("Mobile OTP sent successfully.") : BadRequest("Failed to send Mobile OTP.");
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyMobileOtp([FromQuery] string phoneNumber, [FromQuery] string code)
        {
            var verified = await _otpService.VerifyMobileOtpAsync(phoneNumber, code);
            return verified ? Ok("Mobile OTP verified successfully.") : BadRequest("Invalid Mobile OTP.");
        }
        [HttpPost("sendcustomerotp")]
        public async Task<IActionResult> SendCustomerOtp(CustomerOTPDto requestOTP)
        {

            var sent = await _otpService.SendCustomerOtpAsync(requestOTP);
            return Ok("Customer OTP sent successfully."); 
            
        }

        
        [HttpPost("verifycustomerotp")]
        public async Task<IActionResult> VerifyCustomerOtp(CustomerOTPDto requestOTP)
        {
            var verified = await _otpService.VerifyCustomerOtpAsync(requestOTP);
            return Ok("Customer OTP verified successfully.");
        }

        [HttpPost("sentsms")]
        public async Task<IActionResult> SentSmS(SMSRequestDto requestOTP)
        {
            var verified = await _smsService.SendSMSAsync(requestOTP);
            return Ok(verified);
        }


    }
}
