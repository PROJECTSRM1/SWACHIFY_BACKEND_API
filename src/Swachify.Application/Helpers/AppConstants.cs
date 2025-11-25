namespace Swachify.Application;

public static class AppConstants
{
    // Strings
    public const string CustomerAssignedAgent = "Service Agent Assigned Mail to Customer";
    public const string EMPAssignmentMail = "Service Assignment Mail to EMP";
    public const string CustomerAssignMail = "Service Completion Assigned Mail to Customer";
    public const string ServiceBookingMail = "Service Booking Mail";

    public const string ResetEmail = "Welcome! Please reset your password";

    public const string WelcomeSMSmessage = "Dear {#var#}, Thank you for consulting RM1 Codershub! We have received your request regarding IT Consultation and will assign an agent to your request shortly. Regards, RM1 CodersHub";

    public const string OtpMessage = "Dear {#var#}, your Swachify cleaning agent has arrived at your location. 🧑‍🔧 Agent Name:  {#var#}. OTP for verification: {#var#}. Please provide this OTP to the agent to begin the service. Regards, RM1 codershub";

    public const string otpsms = "Dear {CustomerName},OTP for RM1 coders login : {otp} Regard,RM1 codershub";
}
