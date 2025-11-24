

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Swachify.Application.DTOs;
using Swachify.Application.Interfaces;
using Swachify.Application.Models;

namespace Swachify.Application.Services;

public class SMSService(IConfiguration configuration) : ISMSService
{
    public async Task<string> SendSMSAsync(SMSRequestDto request)
    {
        if (string.IsNullOrEmpty(request?.To))
            return "Please provide phone number";
        var smsSection = configuration.GetSection("SMSSettings");
        var baseUrl = smsSection["BaseUrl"];
        var clientid = smsSection["ClientID"];
        var clientscreat = smsSection["ClientSecret"];
        var senderID = smsSection["SenderID"];
        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(30);
        var byteArray = Encoding.ASCII.GetBytes($"{clientid}:{clientscreat}");
        string base64Auth = Convert.ToBase64String(byteArray);
        client.DefaultRequestHeaders.Add("Authorization", $"Basic {base64Auth}");
        var payload = new
        {
            apiver = "1.0",
            sms = new
            {
                ver = "2.0",
                dlr = new { url = "" },
                messages = new[]
                {
                    new
                    {
                        udh = "0",
                        coding = 1,
                        text = request.message,
                        property = 0,
                        id = "1",
                        addresses = new[]
                        {
                            new { from = "RMCODE", to = request.To, seq = "1", tag = "" }
                        }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var resp = await client.PostAsync(baseUrl, content);

        var respBody = await resp.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<SMSOutputDtos>(respBody);
        if (result.statuscode == 200)
        {
            var errortext = result?.messageack?.guids?
                            .Where(g => g?.errors != null)                // ensure errors is not null
                            .SelectMany(g => g.errors)
                            .FirstOrDefault(e => e?.errorcode > 0)
                            ?.errortext;
            return errortext;
        }
        else if (result?.status.ToLower() == "error")
        {
            return result.statustext.ToString();
        }

        return "SMS sent Successfully";
    }
}
